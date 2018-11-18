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
    public class CombinedHash : TlsHandshakeHash
    {
        protected TlsContext context;
        protected IDigest md5;
        protected IDigest sha1;

        public CombinedHash()
        {
            this.md5 = DtlsHelper.CreateHash(HashAlgorithm.md5);
            this.sha1 = DtlsHelper.CreateHash(HashAlgorithm.sha1);
        }

        public CombinedHash(CombinedHash t)
        {
            this.context = t.context;
            this.md5 = DtlsHelper.CloneHash(HashAlgorithm.md5, t.md5);
            this.sha1 = DtlsHelper.CloneHash(HashAlgorithm.sha1, t.sha1);
        }

        public string AlgorithmName
        {
            get
            {
                return md5.AlgorithmName + " and " + sha1.AlgorithmName;
            }
        }

        public void BlockUpdate(byte[] input, int inOff, int length)
        {
            md5.BlockUpdate(input, inOff, length);
            sha1.BlockUpdate(input, inOff, length);
        }

        public int DoFinal(byte[] output, int outOff)
        {
            if (context != null && context.ServerVersion.IsSsl)
            {
                SSL3Complete(md5, DtlsHelper.IPAD, DtlsHelper.OPAD, 48);
                SSL3Complete(sha1, DtlsHelper.IPAD, DtlsHelper.OPAD, 40);
            }

            int i1 = md5.DoFinal(output, outOff);
            int i2 = sha1.DoFinal(output, outOff + i1);
            return i1 + i2;
        }

        public IDigest ForkPrfHash()
        {
            return new CombinedHash(this);
        }

        public int GetByteLength()
        {
            return System.Math.Max(md5.GetByteLength(), sha1.GetByteLength());
        }

        public int GetDigestSize()
        {
            return md5.GetDigestSize() + sha1.GetDigestSize();
        }

        public byte[] GetFinalHash(byte hashAlgorithm)
        {
            throw new InvalidOperationException("CombinedHash doesn't support multiple hashes");
        }

        public void Init(TlsContext context)
        {
            this.context = context;
        }

        public TlsHandshakeHash NotifyPrfDetermined()
        {
            return this;
        }

        public void Reset()
        {
            md5.Reset();
            sha1.Reset();
        }

        public void SealHashAlgorithms()
        {

        }

        public TlsHandshakeHash StopTracking()
        {
            return new CombinedHash(this);
        }

        public void TrackHashAlgorithm(byte hashAlgorithm)
        {
            throw new InvalidOperationException("CombinedHash only supports calculating the legacy PRF for handshake hash");
        }

        public void Update(byte input)
        {
            md5.Update(input);
            sha1.Update(input);
        }

        protected void SSL3Complete(IDigest d, byte[] ipad, byte[] opad, int padLength)
        {
            byte[] master_secret = context.SecurityParameters.MasterSecret;

            d.BlockUpdate(master_secret, 0, master_secret.Length);
            d.BlockUpdate(ipad, 0, padLength);

            byte[] tmp = new byte[d.GetDigestSize()];
            d.DoFinal(tmp, 0);

            d.BlockUpdate(master_secret, 0, master_secret.Length);
            d.BlockUpdate(opad, 0, padLength);
            d.BlockUpdate(tmp, 0, tmp.Length);
        }
    }
}
