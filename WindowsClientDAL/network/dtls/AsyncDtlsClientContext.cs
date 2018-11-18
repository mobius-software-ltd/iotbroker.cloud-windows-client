using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
    public class AsyncDtlsClientContext: TlsClientContext
    {
        private IRandomGenerator nonceRandom;
        private SecureRandom secureRandom;
        private AsyncDtlsSecurityParameters securityParameters;

        private ProtocolVersion clientVersion = null;
        private ProtocolVersion serverVersion = null;
        private TlsSession session = null;
        private Object userObject = null;

        private static long counter = Times.NanoTime();

        public IRandomGenerator NonceRandomGenerator
        {
            get
            {
                return nonceRandom;
            }
        }

        public SecureRandom SecureRandom
        {
            get
            {
                return secureRandom;
            }
        }

        public SecurityParameters SecurityParameters
        {
            get
            {
                return securityParameters;
            }
        }

        public bool IsServer
        {
            get
            {
                return false;
            }
        }

        public ProtocolVersion ClientVersion
        {
            get
            {
                return clientVersion;
            }

            set
            {
                clientVersion = value;
            }
        }

        public ProtocolVersion ServerVersion
        {
            get
            {
                return serverVersion;
            }

            set
            {
                serverVersion = value;
            }
        }

        public TlsSession ResumableSession
        {
            get
            {
                return session;
            }

            set
            {
                session = value;
            }
        }

        public object UserObject
        {
            get
            {
                return userObject;
            }

            set
            {
                userObject = value;
            }
        }

        public AsyncDtlsClientContext(SecureRandom secureRandom, AsyncDtlsSecurityParameters securityParameters)
        {
            IDigest d = TlsUtilities.CreateHash(HashAlgorithm.sha256);
            byte[] seed = new byte[d.GetDigestSize()];
            secureRandom.NextBytes(seed);

            this.nonceRandom = new DigestRandomGenerator(d);
            nonceRandom.AddSeedMaterial(NextCounterValue());
            nonceRandom.AddSeedMaterial(Times.NanoTime());
            nonceRandom.AddSeedMaterial(seed);

            this.secureRandom = secureRandom;
            this.securityParameters = securityParameters;
        }

        #if NETCF_1_0
                private static object counterLock = new object();
                private static long NextCounterValue()
                {
                    lock (counterLock)
                    {
                        return ++counter;
                    }
                }
        #else
                private static long NextCounterValue()
                {
                    return Interlocked.Increment(ref counter);
                }
        #endif        

        public byte[] ExportKeyingMaterial(string asciiLabel, byte[] context_value, int length)
        {
            if (context_value != null && !TlsUtilities.IsValidUint16(context_value.Length))
                throw new ArgumentException("'context_value' must have length less than 2^16 (or be null)");

            SecurityParameters sp = this.securityParameters;
            byte[] cr = sp.ClientRandom, sr = sp.ServerRandom;

            int seedLength = cr.Length + sr.Length;
            if (context_value != null)
                seedLength += (2 + context_value.Length);

            byte[] seed = new byte[seedLength];
            int seedPos = 0;

            Array.Copy(cr, 0, seed, seedPos, cr.Length);
            seedPos += cr.Length;
            Array.Copy(sr, 0, seed, seedPos, sr.Length);
            seedPos += sr.Length;
            if (context_value != null)
            {
                TlsUtilities.WriteUint16(context_value.Length, seed, seedPos);
                seedPos += 2;
                Array.Copy(context_value, 0, seed, seedPos, context_value.Length);
                seedPos += context_value.Length;
            }

            if (seedPos != seedLength)
            {
                throw new InvalidOperationException("error in calculation of seed for export");
            }

            return TlsUtilities.PRF(this, sp.MasterSecret, asciiLabel, seed, length);
        }
    }
}
