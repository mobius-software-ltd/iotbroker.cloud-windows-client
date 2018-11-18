using Org.BouncyCastle.Crypto.Tls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto;

/**
 * Mobius Software LTD
 * Copyright 2015-2017, Mobius Software LTD
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 *
 * This software is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this software; if not, write to the Free
 * Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
 * 02110-1301 USA, or see the FSF site: http://www.fsf.org.
 */

namespace com.mobius.software.windows.iotbroker.network.dtls
{
    public class DeferredHash : TlsHandshakeHash
    {
        protected static int BUFFERING_HASH_LIMIT = 4;

        protected TlsContext context;

        private DigestInputBuffer buf;
        private Dictionary<Int32, IDigest> hashes;
        private Int16? prfHashAlgorithm;

        public DeferredHash()
        {
            this.buf = new DigestInputBuffer();
            this.hashes = new Dictionary<Int32, IDigest>();
            this.prfHashAlgorithm = null;
        }

        private DeferredHash(Int16 prfHashAlgorithm, IDigest prfHash)
        {
            this.buf = null;
            this.hashes = new Dictionary<Int32, IDigest>();
            this.prfHashAlgorithm = prfHashAlgorithm;
            hashes[prfHashAlgorithm] = prfHash;
        }

        public string AlgorithmName
        {
            get
            {
                throw new InvalidOperationException("Use fork() to get a definite Digest");
            }
        }

        public void BlockUpdate(byte[] input, int inOff, int length)
        {
            if (buf != null)
            {
                buf.Write(input, inOff, length);
                return;
            }
            
            foreach(Int16 curr in hashes.Keys)
            {
                IDigest hash = hashes[curr];
                hash.BlockUpdate(input, inOff, length);
            }
        }

        public int DoFinal(byte[] output, int outOff)
        {
            throw new InvalidOperationException("Use fork() to get a definite Digest");
        }

        public IDigest ForkPrfHash()
        {
            CheckStopBuffering();

            if (buf != null)
            {
                IDigest prfHash = DtlsHelper.CreateHash((short)prfHashAlgorithm);
                buf.UpdateDigest(prfHash);
                return prfHash;
            }

            return DtlsHelper.CloneHash((short)prfHashAlgorithm, (IDigest)hashes[prfHashAlgorithm.Value]);
        }

        public int GetByteLength()
        {
            throw new InvalidOperationException("Use fork() to get a definite Digest");
        }

        public int GetDigestSize()
        {
            throw new InvalidOperationException("Use fork() to get a definite Digest");
        }

        public byte[] GetFinalHash(byte hashAlgorithm)
        {
            IDigest d = (IDigest)hashes[hashAlgorithm];
            if (d == null)
            {
                throw new InvalidOperationException("HashAlgorithm." + HashAlgorithm.GetText(hashAlgorithm) + " is not being tracked");
            }

            d = DtlsHelper.CloneHash(hashAlgorithm, d);
            if (buf != null)
                buf.UpdateDigest(d);

            byte[] bs = new byte[d.GetDigestSize()];
            d.DoFinal(bs, 0);
            return bs;
        }

        public void Init(TlsContext context)
        {
            this.context = context;
        }

        public TlsHandshakeHash NotifyPrfDetermined()
        {
            int prfAlgorithm = context.SecurityParameters.PrfAlgorithm;
            if (prfAlgorithm == PrfAlgorithm.tls_prf_legacy)
            {
                CombinedHash legacyHash = new CombinedHash();
                legacyHash.Init(context);
                buf.UpdateDigest(legacyHash);
                return legacyHash.NotifyPrfDetermined();
            }

            switch (prfAlgorithm)
            {
                case PrfAlgorithm.tls_prf_legacy:
                    throw new InvalidOperationException("legacy PRF not a valid algorithm");
                case PrfAlgorithm.tls_prf_sha256:
                    this.prfHashAlgorithm = HashAlgorithm.sha256;
                    break;
                case PrfAlgorithm.tls_prf_sha384:
                    this.prfHashAlgorithm = HashAlgorithm.sha384;
                    break;
                default:
                    throw new InvalidOperationException("unknown PRFAlgorithm");
            }

            CheckTrackingHash(prfHashAlgorithm.Value);

            return this;
        }

        public void Reset()
        {
            if (buf != null)
            {
                buf.SetLength(0);
                return;
            }

            foreach (Int16 curr in hashes.Keys)
            {
                IDigest hash = (IDigest)hashes[curr];
                hash.Reset();
            }
        }

        public void SealHashAlgorithms()
        {
            CheckStopBuffering();
        }

        public TlsHandshakeHash StopTracking()
        {
            IDigest prfHash = DtlsHelper.CloneHash(prfHashAlgorithm.Value, (IDigest)hashes[prfHashAlgorithm.Value]);
            if (buf != null)
                buf.UpdateDigest(prfHash);

            DeferredHash result = new DeferredHash(prfHashAlgorithm.Value, prfHash);
            result.Init(context);
            return result;
        }

        public void TrackHashAlgorithm(byte hashAlgorithm)
        {
            if (buf == null)
            {
                throw new InvalidOperationException("Too late to track more hash algorithms");
            }

            CheckTrackingHash(hashAlgorithm);
        }

        public void Update(byte input)
        {
            if (buf != null)
            {
                buf.WriteByte(input);
                return;
            }

            foreach (Int16 curr in hashes.Keys)
            {
                IDigest hash = (IDigest)hashes[curr];
                hash.Update(input);
            }
        }

        protected void CheckStopBuffering()
        {
            if (buf != null && hashes.Count <= BUFFERING_HASH_LIMIT)
            {
                foreach (Int16 curr in hashes.Keys)
                {
                    IDigest hash = (IDigest)hashes[curr];
                    buf.UpdateDigest(hash);
                }

                this.buf = null;
            }
        }

        protected void CheckTrackingHash(Int16 hashAlgorithm)
        {
            if (!hashes.ContainsKey(hashAlgorithm))
            {
                IDigest hash = DtlsHelper.CreateHash(hashAlgorithm);
                hashes[hashAlgorithm]=hash;
            }
        }
    }
}
