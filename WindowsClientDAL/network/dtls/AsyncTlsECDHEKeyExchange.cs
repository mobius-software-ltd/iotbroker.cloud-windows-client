using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.network.dtls
{
    public class AsyncTlsECDHEKeyExchange : TlsECDheKeyExchange
    {
        public AsyncTlsECDHEKeyExchange(int keyExchange, IList supportedSignatureAlgorithms, int[] namedCurves, byte[] clientECPointFormats, byte[] serverECPointFormats) : base(keyExchange, supportedSignatureAlgorithms, namedCurves, clientECPointFormats, serverECPointFormats)
        {
            ;
        }

        public override byte[] GenerateServerKeyExchange()
        {
            DigestInputBuffer buf = new DigestInputBuffer();

            this.mECAgreePrivateKey = GenerateEphemeralServerKeyExchange(mContext.SecureRandom, mNamedCurves,
                mClientECPointFormats, buf);

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

        private ECPrivateKeyParameters GenerateEphemeralServerKeyExchange(SecureRandom random, int[] namedCurves, byte[] ecPointFormats, Stream output)
        {
            /* First we try to find a supported named curve from the client's list. */
            int namedCurve = -1;
            if (namedCurves == null)
            {
                // TODO Let the peer choose the default named curve
                namedCurve = NamedCurve.secp256r1;
            }
            else
            {
                for (int i = 0; i < namedCurves.Length; ++i)
                {
                    int entry = namedCurves[i];
                    if (NamedCurve.IsValid(entry) && TlsEccUtilities.IsSupportedNamedCurve(entry))
                    {
                        namedCurve = entry;
                        break;
                    }
                }
            }

            ECDomainParameters ecParams = null;
            if (namedCurve >= 0)
            {
                ecParams = TlsEccUtilities.GetParametersForNamedCurve(namedCurve);
            }
            else
            {
                /* If no named curves are suitable, check if the client supports explicit curves. */
                if (Arrays.Contains(namedCurves, NamedCurve.arbitrary_explicit_prime_curves))
                {
                    ecParams = TlsEccUtilities.GetParametersForNamedCurve(NamedCurve.secp256r1);
                }
                else if (Arrays.Contains(namedCurves, NamedCurve.arbitrary_explicit_char2_curves))
                {
                    ecParams = TlsEccUtilities.GetParametersForNamedCurve(NamedCurve.sect283r1);
                }
            }

            if (ecParams == null)
            {
                /*
                 * NOTE: We shouldn't have negotiated ECDHE key exchange since we apparently can't find
                 * a suitable curve.
                 */
                throw new TlsFatalAlert(AlertDescription.internal_error);
            }

            if (namedCurve < 0)
            {
                TlsEccUtilities.WriteExplicitECParameters(ecPointFormats, ecParams, output);
            }
            else
            {
                TlsEccUtilities.WriteNamedECParameters(namedCurve, output);
            }

            return TlsEccUtilities.GenerateEphemeralClientKeyExchange(random, ecPointFormats, ecParams, output);
        }
    }
}
