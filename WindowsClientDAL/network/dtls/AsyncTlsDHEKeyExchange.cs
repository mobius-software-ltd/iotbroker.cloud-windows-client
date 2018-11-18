using com.mobius.software.windows.iotbroker.mqtt.net;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Security;
using System;
using System.Collections;
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
    public class AsyncTlsDHEKeyExchange : TlsDheKeyExchange
    {

        public AsyncTlsDHEKeyExchange(int keyExchange, IList supportedSignatureAlgorithms, DHParameters dhParameters): base(keyExchange, supportedSignatureAlgorithms, dhParameters)
        {            
        }

        override
        public byte[] GenerateServerKeyExchange()
        {
		    if (this.mDHParameters == null)
			    throw new TlsFatalAlert(AlertDescription.internal_error);

            DigestInputBuffer buf = new DigestInputBuffer();

            this.mDHAgreePrivateKey = TlsDHUtilities.GenerateEphemeralServerKeyExchange(mContext.SecureRandom, this.mDHParameters, buf);

            /*
             * RFC 5246 4.7. digitally-signed element needs SignatureAndHashAlgorithm from TLS 1.2
             */
            SignatureAndHashAlgorithm signatureAndHashAlgorithm = TlsUtilities.GetSignatureAndHashAlgorithm(
                mContext, mServerCredentials);

            IDigest d = TlsUtilities.CreateHash(signatureAndHashAlgorithm);

            SecurityParameters securityParameters = mContext.SecurityParameters;
            d.BlockUpdate(securityParameters.ClientRandom, 0, securityParameters.ClientRandom.Length);
            d.BlockUpdate(securityParameters.ServerRandom, 0, securityParameters.ServerRandom.Length);
            buf.UpdateDigest(d);

            byte[] hash = DigestUtilities.DoFinal(d);

            byte[] signature = mServerCredentials.GenerateCertificateSignature(hash);

            DigitallySigned signed_params = new DigitallySigned(signatureAndHashAlgorithm, signature);
            signed_params.Encode(buf);

            return buf.ToArray();
        }

        override
        protected ISigner InitVerifyer(TlsSigner tlsSigner, SignatureAndHashAlgorithm algorithm, SecurityParameters securityParameters)
        {
            ISigner signer = tlsSigner.CreateVerifyer(algorithm, this.mServerPublicKey);
            signer.BlockUpdate(securityParameters.ClientRandom, 0, securityParameters.ClientRandom.Length);
            signer.BlockUpdate(securityParameters.ServerRandom, 0, securityParameters.ServerRandom.Length);
            return signer;
        }
    }
}
