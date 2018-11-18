using Org.BouncyCastle.Crypto.Tls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class AsyncDtlsSecurityParameters: SecurityParameters
    {
        #region private fields

        int entity = -1;
        int cipherSuite = -1;
        byte compressionAlgorithm = CompressionMethod.cls_null;
        int prfAlgorithm = -1;
        int verifyDataLength = -1;
        byte[] masterSecret = null;
        byte[] clientRandom = null;
        byte[] serverRandom = null;
        byte[] sessionHash = null;
        byte[] pskIdentity = null;
        byte[] srpIdentity = null;

        short maxFragmentLength = -1;
        Boolean truncatedHMac = false;
        Boolean encryptThenMAC = false;
        Boolean extendedMasterSecret = false;

        byte[] cookie = null;

        #endregion

        #region public fields

        override
        public int Entity
        {
            get
            {
                return entity;
            }
        }

        public void SetEntity(int entity)
        {
            this.entity = entity;
        }

        override
        public int CipherSuite
        {
            get
            {
                return cipherSuite;
            }         
        }

        public void SetCipherSuite(int cipherSuite)
        {
            this.cipherSuite = cipherSuite;
        }

        public new byte CompressionAlgorithm
        {
            get
            {
                return compressionAlgorithm;
            }            
        }

        public void SetCompressionAlgorithm(byte compressionAlgorithm)
        {
            this.compressionAlgorithm = compressionAlgorithm;
        }

        override
        public int PrfAlgorithm
        {
            get
            {
                return prfAlgorithm;
            }            
        }

        public void SetPrfAlgorithm(int prfAlgorithm)
        {
            this.prfAlgorithm = prfAlgorithm;
        }

        override
        public int VerifyDataLength
        {
            get
            {
                return verifyDataLength;
            }            
        }

        public void SetVerifyDataLength(int verifyDataLength)
        {
            this.verifyDataLength = verifyDataLength;
        }

        override
        public byte[] MasterSecret
        {
            get
            {
                return masterSecret;
            }            
        }

        public void SetMasterSecret(byte[] masterSecret)
        {
            this.masterSecret = masterSecret;
        }

        override
        public byte[] ClientRandom
        {
            get
            {
                return clientRandom;
            }
        }

        public void SetClientRandom(byte[] clientRandom)
        {
            this.clientRandom = clientRandom;
        }

        override
        public byte[] ServerRandom
        {
            get
            {
                return serverRandom;
            }            
        }

        public void SetServerRandom(byte[] serverRandom)
        {
            this.serverRandom = serverRandom;
        }

        override
        public byte[] SessionHash
        {
            get
            {
                return sessionHash;
            }            
        }

        public void SetSessionHash(byte[] sessionHash)
        {
            this.sessionHash = sessionHash;
        }

        override
        public byte[] PskIdentity
        {
            get
            {
                return pskIdentity;
            }            
        }

        public void SetPskIdentity(byte[] pskIdentity)
        {
            this.pskIdentity = pskIdentity;
        }

        override
        public byte[] SrpIdentity
        {
            get
            {
                return srpIdentity;
            }            
        }

        public void SetSrpIdentity(byte[] srpIdentity)
        {
            this.srpIdentity = srpIdentity;
        }

        public short GetMaxFragmentLength()
        {
            return maxFragmentLength;
        }

        public void SetMaxFragmentLength(short maxFragmentLength)
        {
            this.maxFragmentLength = maxFragmentLength;
        }

        public Boolean IsTruncatedHMac()
        {
            return truncatedHMac;
        }

        public void SetTruncatedHMac(Boolean truncatedHMac)
        {
            this.truncatedHMac = truncatedHMac;
        }

        public Boolean IsEncryptThenMAC()
        {
            return encryptThenMAC;
        }

        public void SetEncryptThenMAC(Boolean encryptThenMAC)
        {
            this.encryptThenMAC = encryptThenMAC;
        }

        public Boolean IsExtendedMasterSecret()
        {
            return extendedMasterSecret;
        }

        public void SetExtendedMasterSecret(Boolean extendedMasterSecret)
        {
            this.extendedMasterSecret = extendedMasterSecret;
        }

        public byte[] GetCookie()
        {
            return cookie;
        }

        public void SetCookie(byte[] cookie)
        {
            this.cookie = cookie;
        }
        #endregion
    }
}
