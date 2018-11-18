using DotNetty.Buffers;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Utilities;
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
    public class DtlsHelper
    {
        public static byte[] EMPTY_BYTES = new byte[0];
        public static byte[] EMPTY_BYTES_WITH_LENGTH = new byte[] { 0x00 };

        public static Int32 EXT_RenegotiationInfo = ExtensionType.renegotiation_info;
	    public static Int32 EXT_SessionTicket = ExtensionType.session_ticket;

	    public static Int32 EXT_encrypt_then_mac = ExtensionType.encrypt_then_mac;
        public static Int32 EXT_extended_master_secret = ExtensionType.extended_master_secret;
        public static Int32 EXT_heartbeat = ExtensionType.heartbeat;
        public static Int32 EXT_max_fragment_length = ExtensionType.max_fragment_length;
        public static Int32 EXT_padding = ExtensionType.padding;
        public static Int32 EXT_server_name = ExtensionType.server_name;
        public static Int32 EXT_status_request = ExtensionType.status_request;
        public static Int32 EXT_truncated_hmac = ExtensionType.truncated_hmac;

        public static Int32 HANDSHAKE_MESSAGE_HEADER_LENGTH = 12;
        public static byte IPAD_BYTE = (byte)0x36;
        public static byte OPAD_BYTE = (byte)0x5C;

        public static byte[] IPAD = GenPad(IPAD_BYTE, 48);
        public static byte[] OPAD = GenPad(OPAD_BYTE, 48);
        public static byte[][] SSL3_CONST = GenSSL3Const();

        public static byte[][] GenSSL3Const()
        {
            int n = 10;
            byte[][] arr = new byte[n][];
            for (int i = 0; i < n; i++)
            {
                byte[] b = new byte[i + 1];
                Arrays.Fill(b, (byte)('A' + i));
                arr[i] = b;
            }
            return arr;
        }

        public static short EvaluateMaxFragmentLengthExtension(Boolean resumedSession, IDictionary clientExtensions, IDictionary serverExtensions, short alertDescription)
        {
            short maxFragmentLength = TlsExtensionsUtilities.GetMaxFragmentLengthExtension(serverExtensions);
		    if (maxFragmentLength >= 0)
		    {
			    if (!MaxFragmentLength.IsValid((byte)maxFragmentLength) || (!resumedSession && maxFragmentLength != TlsExtensionsUtilities.GetMaxFragmentLengthExtension(clientExtensions)))
				    throw new TlsFatalAlert((byte)alertDescription);
            }
	        
		    return maxFragmentLength;
	    }

        public static Int32 CalculateExtensionsLength(IDictionary extensions)
        {
            Int32 length=0;
            length+=CalculateSelectedExtensionsLength(extensions, true);
            length+=CalculateSelectedExtensionsLength(extensions, false);
		
		    if((length & 0xFFFF) != length)
			    throw new TlsFatalAlert(AlertDescription.internal_error);
		
		    return length+2;
	    }

        public static int GetCipherType(int ciphersuite)
        {
            switch (GetEncryptionAlgorithm(ciphersuite))
            {
                case EncryptionAlgorithm.AES_128_CCM:
                case EncryptionAlgorithm.AES_128_CCM_8:
                case EncryptionAlgorithm.AES_128_GCM:
                case EncryptionAlgorithm.AES_128_OCB_TAGLEN96:
                case EncryptionAlgorithm.AES_256_CCM:
                case EncryptionAlgorithm.AES_256_CCM_8:
                case EncryptionAlgorithm.AES_256_GCM:
                case EncryptionAlgorithm.AES_256_OCB_TAGLEN96:
                case EncryptionAlgorithm.CAMELLIA_128_GCM:
                case EncryptionAlgorithm.CAMELLIA_256_GCM:
                case EncryptionAlgorithm.CHACHA20_POLY1305:
                    return CipherType.aead;

                case EncryptionAlgorithm.RC2_CBC_40:
                case EncryptionAlgorithm.IDEA_CBC:
                case EncryptionAlgorithm.DES40_CBC:
                case EncryptionAlgorithm.DES_CBC:
                case EncryptionAlgorithm.cls_3DES_EDE_CBC:
                case EncryptionAlgorithm.AES_128_CBC:
                case EncryptionAlgorithm.AES_256_CBC:
                case EncryptionAlgorithm.CAMELLIA_128_CBC:
                case EncryptionAlgorithm.CAMELLIA_256_CBC:
                case EncryptionAlgorithm.SEED_CBC:
                    return CipherType.block;

                case EncryptionAlgorithm.NULL:
                case EncryptionAlgorithm.RC4_40:
                case EncryptionAlgorithm.RC4_128:
                    return CipherType.stream;

                default:
                    throw new TlsFatalAlert(AlertDescription.internal_error);
            }
        }

        public static int GetEncryptionAlgorithm(int ciphersuite)
        {
            switch (ciphersuite)
            {
	            case CipherSuite.TLS_DH_anon_WITH_3DES_EDE_CBC_SHA:
	            case CipherSuite.TLS_DH_DSS_WITH_3DES_EDE_CBC_SHA:
	            case CipherSuite.TLS_DH_RSA_WITH_3DES_EDE_CBC_SHA:
	            case CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA:
	            case CipherSuite.TLS_DHE_PSK_WITH_3DES_EDE_CBC_SHA:
	            case CipherSuite.TLS_DHE_RSA_WITH_3DES_EDE_CBC_SHA:
	            case CipherSuite.TLS_ECDH_anon_WITH_3DES_EDE_CBC_SHA:
	            case CipherSuite.TLS_ECDH_ECDSA_WITH_3DES_EDE_CBC_SHA:
	            case CipherSuite.TLS_ECDH_RSA_WITH_3DES_EDE_CBC_SHA:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_3DES_EDE_CBC_SHA:
	            case CipherSuite.TLS_ECDHE_PSK_WITH_3DES_EDE_CBC_SHA:
	            case CipherSuite.TLS_ECDHE_RSA_WITH_3DES_EDE_CBC_SHA:
	            case CipherSuite.TLS_PSK_WITH_3DES_EDE_CBC_SHA:
	            case CipherSuite.TLS_RSA_PSK_WITH_3DES_EDE_CBC_SHA:
	            case CipherSuite.TLS_RSA_WITH_3DES_EDE_CBC_SHA:
	            case CipherSuite.TLS_SRP_SHA_DSS_WITH_3DES_EDE_CBC_SHA:
	            case CipherSuite.TLS_SRP_SHA_RSA_WITH_3DES_EDE_CBC_SHA:
	            case CipherSuite.TLS_SRP_SHA_WITH_3DES_EDE_CBC_SHA:
	                return EncryptionAlgorithm.cls_3DES_EDE_CBC;
	
	            case CipherSuite.TLS_DH_anon_WITH_AES_128_CBC_SHA:
	            case CipherSuite.TLS_DH_anon_WITH_AES_128_CBC_SHA256:
	            case CipherSuite.TLS_DH_DSS_WITH_AES_128_CBC_SHA:
	            case CipherSuite.TLS_DH_DSS_WITH_AES_128_CBC_SHA256:
	            case CipherSuite.TLS_DH_RSA_WITH_AES_128_CBC_SHA:
	            case CipherSuite.TLS_DH_RSA_WITH_AES_128_CBC_SHA256:
	            case CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA:
	            case CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA256:
	            case CipherSuite.TLS_DHE_PSK_WITH_AES_128_CBC_SHA:
	            case CipherSuite.TLS_DHE_PSK_WITH_AES_128_CBC_SHA256:
	            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA:
	            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA256:
	            case CipherSuite.TLS_ECDH_anon_WITH_AES_128_CBC_SHA:
	            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_CBC_SHA:
	            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_CBC_SHA256:
	            case CipherSuite.TLS_ECDH_RSA_WITH_AES_128_CBC_SHA:
	            case CipherSuite.TLS_ECDH_RSA_WITH_AES_128_CBC_SHA256:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256:
	            case CipherSuite.TLS_ECDHE_PSK_WITH_AES_128_CBC_SHA:
	            case CipherSuite.TLS_ECDHE_PSK_WITH_AES_128_CBC_SHA256:
	            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA:
	            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256:
	            case CipherSuite.TLS_PSK_WITH_AES_128_CBC_SHA:
	            case CipherSuite.TLS_PSK_WITH_AES_128_CBC_SHA256:
	            case CipherSuite.TLS_RSA_PSK_WITH_AES_128_CBC_SHA:
	            case CipherSuite.TLS_RSA_PSK_WITH_AES_128_CBC_SHA256:
	            case CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA:
	            case CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA256:
	            case CipherSuite.TLS_SRP_SHA_DSS_WITH_AES_128_CBC_SHA:
	            case CipherSuite.TLS_SRP_SHA_RSA_WITH_AES_128_CBC_SHA:
	            case CipherSuite.TLS_SRP_SHA_WITH_AES_128_CBC_SHA:
	                return EncryptionAlgorithm.AES_128_CBC;
	
	            case CipherSuite.TLS_DHE_PSK_WITH_AES_128_CCM:
	            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM:
	            case CipherSuite.TLS_PSK_WITH_AES_128_CCM:
	            case CipherSuite.TLS_RSA_WITH_AES_128_CCM:
	                return EncryptionAlgorithm.AES_128_CCM;
	
	            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM_8:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM_8:
	            case CipherSuite.TLS_PSK_DHE_WITH_AES_128_CCM_8:
	            case CipherSuite.TLS_PSK_WITH_AES_128_CCM_8:
	            case CipherSuite.TLS_RSA_WITH_AES_128_CCM_8:
	                return EncryptionAlgorithm.AES_128_CCM_8;
	
	            case CipherSuite.TLS_DH_anon_WITH_AES_128_GCM_SHA256:
	            case CipherSuite.TLS_DH_DSS_WITH_AES_128_GCM_SHA256:
	            case CipherSuite.TLS_DH_RSA_WITH_AES_128_GCM_SHA256:
	            case CipherSuite.TLS_DHE_DSS_WITH_AES_128_GCM_SHA256:
	            case CipherSuite.TLS_DHE_PSK_WITH_AES_128_GCM_SHA256:
	            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_GCM_SHA256:
	            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_GCM_SHA256:
	            case CipherSuite.TLS_ECDH_RSA_WITH_AES_128_GCM_SHA256:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256:
	            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256:
	            case CipherSuite.TLS_PSK_WITH_AES_128_GCM_SHA256:
	            case CipherSuite.TLS_RSA_PSK_WITH_AES_128_GCM_SHA256:
	            case CipherSuite.TLS_RSA_WITH_AES_128_GCM_SHA256:
	                return EncryptionAlgorithm.AES_128_GCM;
	
	            case CipherSuite.DRAFT_TLS_DHE_PSK_WITH_AES_128_OCB:
	            case CipherSuite.DRAFT_TLS_DHE_RSA_WITH_AES_128_OCB:
	            case CipherSuite.DRAFT_TLS_ECDHE_ECDSA_WITH_AES_128_OCB:
	            case CipherSuite.DRAFT_TLS_ECDHE_PSK_WITH_AES_128_OCB:
	            case CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_AES_128_OCB:
	            case CipherSuite.DRAFT_TLS_PSK_WITH_AES_128_OCB:
	                return EncryptionAlgorithm.AES_128_OCB_TAGLEN96;
	
	            case CipherSuite.TLS_DH_anon_WITH_AES_256_CBC_SHA:
	            case CipherSuite.TLS_DH_anon_WITH_AES_256_CBC_SHA256:
	            case CipherSuite.TLS_DH_DSS_WITH_AES_256_CBC_SHA:
	            case CipherSuite.TLS_DH_DSS_WITH_AES_256_CBC_SHA256:
	            case CipherSuite.TLS_DH_RSA_WITH_AES_256_CBC_SHA:
	            case CipherSuite.TLS_DH_RSA_WITH_AES_256_CBC_SHA256:
	            case CipherSuite.TLS_DHE_DSS_WITH_AES_256_CBC_SHA:
	            case CipherSuite.TLS_DHE_DSS_WITH_AES_256_CBC_SHA256:
	            case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CBC_SHA:
	            case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CBC_SHA384:
	            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA:
	            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA256:
	            case CipherSuite.TLS_ECDH_anon_WITH_AES_256_CBC_SHA:
	            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_CBC_SHA:
	            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_CBC_SHA384:
	            case CipherSuite.TLS_ECDH_RSA_WITH_AES_256_CBC_SHA:
	            case CipherSuite.TLS_ECDH_RSA_WITH_AES_256_CBC_SHA384:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA384:
	            case CipherSuite.TLS_ECDHE_PSK_WITH_AES_256_CBC_SHA:
	            case CipherSuite.TLS_ECDHE_PSK_WITH_AES_256_CBC_SHA384:
	            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA:
	            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384:
	            case CipherSuite.TLS_PSK_WITH_AES_256_CBC_SHA:
	            case CipherSuite.TLS_PSK_WITH_AES_256_CBC_SHA384:
	            case CipherSuite.TLS_RSA_PSK_WITH_AES_256_CBC_SHA:
	            case CipherSuite.TLS_RSA_PSK_WITH_AES_256_CBC_SHA384:
	            case CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA:
	            case CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA256:
	            case CipherSuite.TLS_SRP_SHA_DSS_WITH_AES_256_CBC_SHA:
	            case CipherSuite.TLS_SRP_SHA_RSA_WITH_AES_256_CBC_SHA:
	            case CipherSuite.TLS_SRP_SHA_WITH_AES_256_CBC_SHA:
	                return EncryptionAlgorithm.AES_256_CBC;
	
	            case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CCM:
	            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM:
	            case CipherSuite.TLS_PSK_WITH_AES_256_CCM:
	            case CipherSuite.TLS_RSA_WITH_AES_256_CCM:
	                return EncryptionAlgorithm.AES_256_CCM;
	
	            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM_8:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM_8:
	            case CipherSuite.TLS_PSK_DHE_WITH_AES_256_CCM_8:
	            case CipherSuite.TLS_PSK_WITH_AES_256_CCM_8:
	            case CipherSuite.TLS_RSA_WITH_AES_256_CCM_8:
	                return EncryptionAlgorithm.AES_256_CCM_8;
	
	            case CipherSuite.TLS_DH_anon_WITH_AES_256_GCM_SHA384:
	            case CipherSuite.TLS_DH_DSS_WITH_AES_256_GCM_SHA384:
	            case CipherSuite.TLS_DH_RSA_WITH_AES_256_GCM_SHA384:
	            case CipherSuite.TLS_DHE_DSS_WITH_AES_256_GCM_SHA384:
	            case CipherSuite.TLS_DHE_PSK_WITH_AES_256_GCM_SHA384:
	            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384:
	            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_GCM_SHA384:
	            case CipherSuite.TLS_ECDH_RSA_WITH_AES_256_GCM_SHA384:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384:
	            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384:
	            case CipherSuite.TLS_PSK_WITH_AES_256_GCM_SHA384:
	            case CipherSuite.TLS_RSA_PSK_WITH_AES_256_GCM_SHA384:
	            case CipherSuite.TLS_RSA_WITH_AES_256_GCM_SHA384:
	                return EncryptionAlgorithm.AES_256_GCM;
	
	            case CipherSuite.DRAFT_TLS_DHE_PSK_WITH_AES_256_OCB:
	            case CipherSuite.DRAFT_TLS_DHE_RSA_WITH_AES_256_OCB:
	            case CipherSuite.DRAFT_TLS_ECDHE_ECDSA_WITH_AES_256_OCB:
	            case CipherSuite.DRAFT_TLS_ECDHE_PSK_WITH_AES_256_OCB:
	            case CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_AES_256_OCB:
	            case CipherSuite.DRAFT_TLS_PSK_WITH_AES_256_OCB:
	                return EncryptionAlgorithm.AES_256_OCB_TAGLEN96;
	
	            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_128_CBC_SHA:
	            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_128_CBC_SHA256:
	            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_128_CBC_SHA:
	            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_128_CBC_SHA256:
	            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_128_CBC_SHA:
	            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_128_CBC_SHA256:
	            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_128_CBC_SHA:
	            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_128_CBC_SHA256:
	            case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_128_CBC_SHA256:
	            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_CBC_SHA:
	            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_CBC_SHA256:
	            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_128_CBC_SHA256:
	            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_128_CBC_SHA256:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_128_CBC_SHA256:
	            case CipherSuite.TLS_ECDHE_PSK_WITH_CAMELLIA_128_CBC_SHA256:
	            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_128_CBC_SHA256:
	            case CipherSuite.TLS_PSK_WITH_CAMELLIA_128_CBC_SHA256:
	            case CipherSuite.TLS_RSA_WITH_CAMELLIA_128_CBC_SHA:
	            case CipherSuite.TLS_RSA_WITH_CAMELLIA_128_CBC_SHA256:
	            case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_128_CBC_SHA256:
	                return EncryptionAlgorithm.CAMELLIA_128_CBC;
	
	            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_128_GCM_SHA256:
	            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_128_GCM_SHA256:
	            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_128_GCM_SHA256:
	            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_128_GCM_SHA256:
	            case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_128_GCM_SHA256:
	            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_GCM_SHA256:
	            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_128_GCM_SHA256:
	            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_128_GCM_SHA256:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_128_GCM_SHA256:
	            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_128_GCM_SHA256:
	            case CipherSuite.TLS_PSK_WITH_CAMELLIA_128_GCM_SHA256:
	            case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_128_GCM_SHA256:
	            case CipherSuite.TLS_RSA_WITH_CAMELLIA_128_GCM_SHA256:
	                return EncryptionAlgorithm.CAMELLIA_128_GCM;
	
	            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_256_CBC_SHA:
	            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_256_CBC_SHA256:
	            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_256_CBC_SHA:
	            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_256_CBC_SHA256:
	            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_256_CBC_SHA:
	            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_256_CBC_SHA256:
	            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_CBC_SHA:
	            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_CBC_SHA256:
	            case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_256_CBC_SHA384:
	            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_CBC_SHA:
	            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_CBC_SHA256:
	            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_256_CBC_SHA384:
	            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_256_CBC_SHA384:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_256_CBC_SHA384:
	            case CipherSuite.TLS_ECDHE_PSK_WITH_CAMELLIA_256_CBC_SHA384:
	            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_256_CBC_SHA384:
	            case CipherSuite.TLS_PSK_WITH_CAMELLIA_256_CBC_SHA384:
	            case CipherSuite.TLS_RSA_WITH_CAMELLIA_256_CBC_SHA:
	            case CipherSuite.TLS_RSA_WITH_CAMELLIA_256_CBC_SHA256:
	            case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_256_CBC_SHA384:
	                return EncryptionAlgorithm.CAMELLIA_256_CBC;
	
	            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_256_GCM_SHA384:
	            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_256_GCM_SHA384:
	            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_256_GCM_SHA384:
	            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_GCM_SHA384:
	            case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_256_GCM_SHA384:
	            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_GCM_SHA384:
	            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_256_GCM_SHA384:
	            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_256_GCM_SHA384:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_256_GCM_SHA384:
	            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_256_GCM_SHA384:
	            case CipherSuite.TLS_PSK_WITH_CAMELLIA_256_GCM_SHA384:
	            case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_256_GCM_SHA384:
	            case CipherSuite.TLS_RSA_WITH_CAMELLIA_256_GCM_SHA384:
	                return EncryptionAlgorithm.CAMELLIA_256_GCM;
	
	            case CipherSuite.DRAFT_TLS_DHE_PSK_WITH_CHACHA20_POLY1305_SHA256:
	            case CipherSuite.DRAFT_TLS_DHE_RSA_WITH_CHACHA20_POLY1305_SHA256:
	            case CipherSuite.DRAFT_TLS_ECDHE_ECDSA_WITH_CHACHA20_POLY1305_SHA256:
	            case CipherSuite.DRAFT_TLS_ECDHE_PSK_WITH_CHACHA20_POLY1305_SHA256:
	            case CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_CHACHA20_POLY1305_SHA256:
	            case CipherSuite.DRAFT_TLS_PSK_WITH_CHACHA20_POLY1305_SHA256:
	            case CipherSuite.DRAFT_TLS_RSA_PSK_WITH_CHACHA20_POLY1305_SHA256:
	                return EncryptionAlgorithm.CHACHA20_POLY1305;
	
	            case CipherSuite.TLS_RSA_WITH_NULL_MD5:
	                return EncryptionAlgorithm.NULL;
	
	            case CipherSuite.TLS_DHE_PSK_WITH_NULL_SHA:
	            case CipherSuite.TLS_ECDH_anon_WITH_NULL_SHA:
	            case CipherSuite.TLS_ECDH_ECDSA_WITH_NULL_SHA:
	            case CipherSuite.TLS_ECDH_RSA_WITH_NULL_SHA:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_NULL_SHA:
	            case CipherSuite.TLS_ECDHE_PSK_WITH_NULL_SHA:
	            case CipherSuite.TLS_ECDHE_RSA_WITH_NULL_SHA:
	            case CipherSuite.TLS_PSK_WITH_NULL_SHA:
	            case CipherSuite.TLS_RSA_PSK_WITH_NULL_SHA:
	            case CipherSuite.TLS_RSA_WITH_NULL_SHA:
	                return EncryptionAlgorithm.NULL;
	
	            case CipherSuite.TLS_DHE_PSK_WITH_NULL_SHA256:
	            case CipherSuite.TLS_ECDHE_PSK_WITH_NULL_SHA256:
	            case CipherSuite.TLS_PSK_WITH_NULL_SHA256:
	            case CipherSuite.TLS_RSA_PSK_WITH_NULL_SHA256:
	            case CipherSuite.TLS_RSA_WITH_NULL_SHA256:
	                return EncryptionAlgorithm.NULL;
	
	            case CipherSuite.TLS_DHE_PSK_WITH_NULL_SHA384:
	            case CipherSuite.TLS_ECDHE_PSK_WITH_NULL_SHA384:
	            case CipherSuite.TLS_PSK_WITH_NULL_SHA384:
	            case CipherSuite.TLS_RSA_PSK_WITH_NULL_SHA384:
	                return EncryptionAlgorithm.NULL;
	
	            case CipherSuite.TLS_DH_anon_WITH_RC4_128_MD5:
	            case CipherSuite.TLS_RSA_WITH_RC4_128_MD5:
	                return EncryptionAlgorithm.RC4_128;
	
	            case CipherSuite.TLS_DHE_PSK_WITH_RC4_128_SHA:
	            case CipherSuite.TLS_ECDH_anon_WITH_RC4_128_SHA:
	            case CipherSuite.TLS_ECDH_ECDSA_WITH_RC4_128_SHA:
	            case CipherSuite.TLS_ECDH_RSA_WITH_RC4_128_SHA:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_RC4_128_SHA:
	            case CipherSuite.TLS_ECDHE_PSK_WITH_RC4_128_SHA:
	            case CipherSuite.TLS_ECDHE_RSA_WITH_RC4_128_SHA:
	            case CipherSuite.TLS_PSK_WITH_RC4_128_SHA:
	            case CipherSuite.TLS_RSA_WITH_RC4_128_SHA:
	            case CipherSuite.TLS_RSA_PSK_WITH_RC4_128_SHA:
	                return EncryptionAlgorithm.RC4_128;
	
	            case CipherSuite.TLS_DH_anon_WITH_SEED_CBC_SHA:
	            case CipherSuite.TLS_DH_DSS_WITH_SEED_CBC_SHA:
	            case CipherSuite.TLS_DH_RSA_WITH_SEED_CBC_SHA:
	            case CipherSuite.TLS_DHE_DSS_WITH_SEED_CBC_SHA:
	            case CipherSuite.TLS_DHE_RSA_WITH_SEED_CBC_SHA:
	            case CipherSuite.TLS_RSA_WITH_SEED_CBC_SHA:
	                return EncryptionAlgorithm.SEED_CBC;

                default:
	                throw new TlsFatalAlert(AlertDescription.internal_error);
            }
        }

        public static int GetPRFAlgorithm(ProtocolVersion version, int ciphersuite)
        {
            Boolean isTLSv12 = ProtocolVersion.TLSv12.IsEqualOrEarlierVersionOf(version.GetEquivalentTLSVersion());

            switch (ciphersuite)
            {
	            case CipherSuite.TLS_DH_anon_WITH_AES_128_CBC_SHA256:
	            case CipherSuite.TLS_DH_anon_WITH_AES_128_GCM_SHA256:
	            case CipherSuite.TLS_DH_anon_WITH_AES_256_CBC_SHA256:
	            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_128_CBC_SHA256:
	            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_128_GCM_SHA256:
	            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_256_CBC_SHA256:
	            case CipherSuite.TLS_DH_DSS_WITH_AES_128_CBC_SHA256:
	            case CipherSuite.TLS_DH_DSS_WITH_AES_128_GCM_SHA256:
	            case CipherSuite.TLS_DH_DSS_WITH_AES_256_CBC_SHA256:
	            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_128_CBC_SHA256:
	            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_128_GCM_SHA256:
	            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_256_CBC_SHA256:
	            case CipherSuite.TLS_DH_RSA_WITH_AES_128_CBC_SHA256:
	            case CipherSuite.TLS_DH_RSA_WITH_AES_128_GCM_SHA256:
	            case CipherSuite.TLS_DH_RSA_WITH_AES_256_CBC_SHA256:
	            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_128_CBC_SHA256:
	            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_128_GCM_SHA256:
	            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_256_CBC_SHA256:
	            case CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA256:
	            case CipherSuite.TLS_DHE_DSS_WITH_AES_128_GCM_SHA256:
	            case CipherSuite.TLS_DHE_DSS_WITH_AES_256_CBC_SHA256:
	            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_128_CBC_SHA256:
	            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_128_GCM_SHA256:
	            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_CBC_SHA256:
	            case CipherSuite.TLS_DHE_PSK_WITH_AES_128_CCM:
	            case CipherSuite.TLS_DHE_PSK_WITH_AES_128_GCM_SHA256:
	            case CipherSuite.DRAFT_TLS_DHE_PSK_WITH_AES_128_OCB:
	            case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CCM:
	            case CipherSuite.DRAFT_TLS_DHE_PSK_WITH_AES_256_OCB:
	            case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_128_GCM_SHA256:
	            case CipherSuite.DRAFT_TLS_DHE_PSK_WITH_CHACHA20_POLY1305_SHA256:
	            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA256:
	            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM:
	            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM_8:
	            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_GCM_SHA256:
	            case CipherSuite.DRAFT_TLS_DHE_RSA_WITH_AES_128_OCB:
	            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA256:
	            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM:
	            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM_8:
	            case CipherSuite.DRAFT_TLS_DHE_RSA_WITH_AES_256_OCB:
	            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_CBC_SHA256:
	            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_GCM_SHA256:
	            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_CBC_SHA256:
	            case CipherSuite.DRAFT_TLS_DHE_RSA_WITH_CHACHA20_POLY1305_SHA256:
	            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_CBC_SHA256:
	            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_GCM_SHA256:
	            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_128_CBC_SHA256:
	            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_128_GCM_SHA256:
	            case CipherSuite.TLS_ECDH_RSA_WITH_AES_128_CBC_SHA256:
	            case CipherSuite.TLS_ECDH_RSA_WITH_AES_128_GCM_SHA256:
	            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_128_CBC_SHA256:
	            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_128_GCM_SHA256:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM_8:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256:
	            case CipherSuite.DRAFT_TLS_ECDHE_ECDSA_WITH_AES_128_OCB:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM_8:
	            case CipherSuite.DRAFT_TLS_ECDHE_ECDSA_WITH_AES_256_OCB:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_128_CBC_SHA256:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_128_GCM_SHA256:
	            case CipherSuite.DRAFT_TLS_ECDHE_ECDSA_WITH_CHACHA20_POLY1305_SHA256:
	            case CipherSuite.DRAFT_TLS_ECDHE_PSK_WITH_AES_128_OCB:
	            case CipherSuite.DRAFT_TLS_ECDHE_PSK_WITH_AES_256_OCB:
	            case CipherSuite.DRAFT_TLS_ECDHE_PSK_WITH_CHACHA20_POLY1305_SHA256:
	            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256:
	            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256:
	            case CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_AES_128_OCB:
	            case CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_AES_256_OCB:
	            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_128_CBC_SHA256:
	            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_128_GCM_SHA256:
	            case CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_CHACHA20_POLY1305_SHA256:
	            case CipherSuite.TLS_PSK_DHE_WITH_AES_128_CCM_8:
	            case CipherSuite.TLS_PSK_DHE_WITH_AES_256_CCM_8:
	            case CipherSuite.TLS_PSK_WITH_AES_128_CCM:
	            case CipherSuite.TLS_PSK_WITH_AES_128_CCM_8:
	            case CipherSuite.TLS_PSK_WITH_AES_128_GCM_SHA256:
	            case CipherSuite.DRAFT_TLS_PSK_WITH_CHACHA20_POLY1305_SHA256:
	            case CipherSuite.DRAFT_TLS_PSK_WITH_AES_128_OCB:
	            case CipherSuite.TLS_PSK_WITH_AES_256_CCM:
	            case CipherSuite.TLS_PSK_WITH_AES_256_CCM_8:
	            case CipherSuite.DRAFT_TLS_PSK_WITH_AES_256_OCB:
	            case CipherSuite.TLS_PSK_WITH_CAMELLIA_128_GCM_SHA256:
	            case CipherSuite.TLS_RSA_PSK_WITH_AES_128_GCM_SHA256:
	            case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_128_GCM_SHA256:
	            case CipherSuite.DRAFT_TLS_RSA_PSK_WITH_CHACHA20_POLY1305_SHA256:
	            case CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA256:
	            case CipherSuite.TLS_RSA_WITH_AES_128_CCM:
	            case CipherSuite.TLS_RSA_WITH_AES_128_CCM_8:
	            case CipherSuite.TLS_RSA_WITH_AES_128_GCM_SHA256:
	            case CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA256:
	            case CipherSuite.TLS_RSA_WITH_AES_256_CCM:
	            case CipherSuite.TLS_RSA_WITH_AES_256_CCM_8:
	            case CipherSuite.TLS_RSA_WITH_CAMELLIA_128_CBC_SHA256:
	            case CipherSuite.TLS_RSA_WITH_CAMELLIA_128_GCM_SHA256:
	            case CipherSuite.TLS_RSA_WITH_CAMELLIA_256_CBC_SHA256:
	            case CipherSuite.TLS_RSA_WITH_NULL_SHA256:
	                if (isTLSv12)
                        return PrfAlgorithm.tls_prf_sha256;

                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);
	            case CipherSuite.TLS_DH_anon_WITH_AES_256_GCM_SHA384:
	            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_256_GCM_SHA384:
	            case CipherSuite.TLS_DH_DSS_WITH_AES_256_GCM_SHA384:
	            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_256_GCM_SHA384:
	            case CipherSuite.TLS_DH_RSA_WITH_AES_256_GCM_SHA384:
	            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_256_GCM_SHA384:
	            case CipherSuite.TLS_DHE_DSS_WITH_AES_256_GCM_SHA384:
	            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_GCM_SHA384:
	            case CipherSuite.TLS_DHE_PSK_WITH_AES_256_GCM_SHA384:
	            case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_256_GCM_SHA384:
	            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384:
	            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_GCM_SHA384:
	            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_CBC_SHA384:
	            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_GCM_SHA384:
	            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_256_CBC_SHA384:
	            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_256_GCM_SHA384:
	            case CipherSuite.TLS_ECDH_RSA_WITH_AES_256_CBC_SHA384:
	            case CipherSuite.TLS_ECDH_RSA_WITH_AES_256_GCM_SHA384:
	            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_256_CBC_SHA384:
	            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_256_GCM_SHA384:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA384:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_256_CBC_SHA384:
	            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_256_GCM_SHA384:
	            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384:
	            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384:
	            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_256_CBC_SHA384:
	            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_256_GCM_SHA384:
	            case CipherSuite.TLS_PSK_WITH_AES_256_GCM_SHA384:
	            case CipherSuite.TLS_PSK_WITH_CAMELLIA_256_GCM_SHA384:
	            case CipherSuite.TLS_RSA_PSK_WITH_AES_256_GCM_SHA384:
	            case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_256_GCM_SHA384:
	            case CipherSuite.TLS_RSA_WITH_AES_256_GCM_SHA384:
	            case CipherSuite.TLS_RSA_WITH_CAMELLIA_256_GCM_SHA384:
	                if (isTLSv12)
                        return PrfAlgorithm.tls_prf_sha384;

                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);
	        
	            case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CBC_SHA384:
	            case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_256_CBC_SHA384:
	            case CipherSuite.TLS_DHE_PSK_WITH_NULL_SHA384:
	            case CipherSuite.TLS_ECDHE_PSK_WITH_AES_256_CBC_SHA384:
	            case CipherSuite.TLS_ECDHE_PSK_WITH_CAMELLIA_256_CBC_SHA384:
	            case CipherSuite.TLS_ECDHE_PSK_WITH_NULL_SHA384:
	            case CipherSuite.TLS_PSK_WITH_AES_256_CBC_SHA384:
	            case CipherSuite.TLS_PSK_WITH_CAMELLIA_256_CBC_SHA384:
	            case CipherSuite.TLS_PSK_WITH_NULL_SHA384:
	            case CipherSuite.TLS_RSA_PSK_WITH_AES_256_CBC_SHA384:
	            case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_256_CBC_SHA384:
	            case CipherSuite.TLS_RSA_PSK_WITH_NULL_SHA384:
	                if (isTLSv12)
                        return PrfAlgorithm.tls_prf_sha384;

                    return PrfAlgorithm.tls_prf_legacy;
                default:
	                if (isTLSv12)
                        return PrfAlgorithm.tls_prf_sha256;

                    return PrfAlgorithm.tls_prf_legacy;
            }
        }

        public static ProtocolVersion GetMinimumVersion(int ciphersuite)
        {
            switch (ciphersuite)
            {
                case CipherSuite.TLS_DH_anon_WITH_AES_128_CBC_SHA256:
                case CipherSuite.TLS_DH_anon_WITH_AES_128_GCM_SHA256:
                case CipherSuite.TLS_DH_anon_WITH_AES_256_CBC_SHA256:
                case CipherSuite.TLS_DH_anon_WITH_AES_256_GCM_SHA384:
                case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_128_CBC_SHA256:
                case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_128_GCM_SHA256:
                case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_256_CBC_SHA256:
                case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_256_GCM_SHA384:
                case CipherSuite.TLS_DH_DSS_WITH_AES_128_CBC_SHA256:
                case CipherSuite.TLS_DH_DSS_WITH_AES_128_GCM_SHA256:
                case CipherSuite.TLS_DH_DSS_WITH_AES_256_CBC_SHA256:
                case CipherSuite.TLS_DH_DSS_WITH_AES_256_GCM_SHA384:
                case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_128_CBC_SHA256:
                case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_128_GCM_SHA256:
                case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_256_CBC_SHA256:
                case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_256_GCM_SHA384:
                case CipherSuite.TLS_DH_RSA_WITH_AES_128_CBC_SHA256:
                case CipherSuite.TLS_DH_RSA_WITH_AES_128_GCM_SHA256:
                case CipherSuite.TLS_DH_RSA_WITH_AES_256_CBC_SHA256:
                case CipherSuite.TLS_DH_RSA_WITH_AES_256_GCM_SHA384:
                case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_128_CBC_SHA256:
                case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_128_GCM_SHA256:
                case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_256_CBC_SHA256:
                case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_256_GCM_SHA384:
                case CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA256:
                case CipherSuite.TLS_DHE_DSS_WITH_AES_128_GCM_SHA256:
                case CipherSuite.TLS_DHE_DSS_WITH_AES_256_CBC_SHA256:
                case CipherSuite.TLS_DHE_DSS_WITH_AES_256_GCM_SHA384:
                case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_128_CBC_SHA256:
                case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_128_GCM_SHA256:
                case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_CBC_SHA256:
                case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_GCM_SHA384:
                case CipherSuite.TLS_DHE_PSK_WITH_AES_128_CCM:
                case CipherSuite.TLS_DHE_PSK_WITH_AES_128_GCM_SHA256:
                case CipherSuite.DRAFT_TLS_DHE_PSK_WITH_AES_128_OCB:
                case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CCM:
                case CipherSuite.TLS_DHE_PSK_WITH_AES_256_GCM_SHA384:
                case CipherSuite.DRAFT_TLS_DHE_PSK_WITH_AES_256_OCB:
                case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_128_GCM_SHA256:
                case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_256_GCM_SHA384:
                case CipherSuite.DRAFT_TLS_DHE_PSK_WITH_CHACHA20_POLY1305_SHA256:
                case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA256:
                case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM:
                case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM_8:
                case CipherSuite.TLS_DHE_RSA_WITH_AES_128_GCM_SHA256:
                case CipherSuite.DRAFT_TLS_DHE_RSA_WITH_AES_128_OCB:
                case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA256:
                case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM:
                case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM_8:
                case CipherSuite.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384:
                case CipherSuite.DRAFT_TLS_DHE_RSA_WITH_AES_256_OCB:
                case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_CBC_SHA256:
                case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_GCM_SHA256:
                case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_CBC_SHA256:
                case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_GCM_SHA384:
                case CipherSuite.DRAFT_TLS_DHE_RSA_WITH_CHACHA20_POLY1305_SHA256:
                case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_CBC_SHA256:
                case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_GCM_SHA256:
                case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_CBC_SHA384:
                case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_GCM_SHA384:
                case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_128_CBC_SHA256:
                case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_128_GCM_SHA256:
                case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_256_CBC_SHA384:
                case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_256_GCM_SHA384:
                case CipherSuite.TLS_ECDH_RSA_WITH_AES_128_CBC_SHA256:
                case CipherSuite.TLS_ECDH_RSA_WITH_AES_128_GCM_SHA256:
                case CipherSuite.TLS_ECDH_RSA_WITH_AES_256_CBC_SHA384:
                case CipherSuite.TLS_ECDH_RSA_WITH_AES_256_GCM_SHA384:
                case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_128_CBC_SHA256:
                case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_128_GCM_SHA256:
                case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_256_CBC_SHA384:
                case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_256_GCM_SHA384:
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256:
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM:
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM_8:
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256:
                case CipherSuite.DRAFT_TLS_ECDHE_ECDSA_WITH_AES_128_OCB:
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA384:
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM:
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM_8:
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384:
                case CipherSuite.DRAFT_TLS_ECDHE_ECDSA_WITH_AES_256_OCB:
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_128_CBC_SHA256:
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_128_GCM_SHA256:
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_256_CBC_SHA384:
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_256_GCM_SHA384:
                case CipherSuite.DRAFT_TLS_ECDHE_ECDSA_WITH_CHACHA20_POLY1305_SHA256:
                case CipherSuite.DRAFT_TLS_ECDHE_PSK_WITH_AES_128_OCB:
                case CipherSuite.DRAFT_TLS_ECDHE_PSK_WITH_AES_256_OCB:
                case CipherSuite.DRAFT_TLS_ECDHE_PSK_WITH_CHACHA20_POLY1305_SHA256:
                case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256:
                case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256:
                case CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_AES_128_OCB:
                case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384:
                case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384:
                case CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_AES_256_OCB:
                case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_128_CBC_SHA256:
                case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_128_GCM_SHA256:
                case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_256_CBC_SHA384:
                case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_256_GCM_SHA384:
                case CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_CHACHA20_POLY1305_SHA256:
                case CipherSuite.TLS_PSK_DHE_WITH_AES_128_CCM_8:
                case CipherSuite.TLS_PSK_DHE_WITH_AES_256_CCM_8:
                case CipherSuite.TLS_PSK_WITH_AES_128_CCM:
                case CipherSuite.TLS_PSK_WITH_AES_128_CCM_8:
                case CipherSuite.TLS_PSK_WITH_AES_128_GCM_SHA256:
                case CipherSuite.DRAFT_TLS_PSK_WITH_AES_128_OCB:
                case CipherSuite.TLS_PSK_WITH_AES_256_CCM:
                case CipherSuite.TLS_PSK_WITH_AES_256_CCM_8:
                case CipherSuite.TLS_PSK_WITH_AES_256_GCM_SHA384:
                case CipherSuite.DRAFT_TLS_PSK_WITH_AES_256_OCB:
                case CipherSuite.TLS_PSK_WITH_CAMELLIA_128_GCM_SHA256:
                case CipherSuite.TLS_PSK_WITH_CAMELLIA_256_GCM_SHA384:
                case CipherSuite.DRAFT_TLS_PSK_WITH_CHACHA20_POLY1305_SHA256:
                case CipherSuite.TLS_RSA_PSK_WITH_AES_128_GCM_SHA256:
                case CipherSuite.TLS_RSA_PSK_WITH_AES_256_GCM_SHA384:
                case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_128_GCM_SHA256:
                case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_256_GCM_SHA384:
                case CipherSuite.DRAFT_TLS_RSA_PSK_WITH_CHACHA20_POLY1305_SHA256:
                case CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA256:
                case CipherSuite.TLS_RSA_WITH_AES_128_CCM:
                case CipherSuite.TLS_RSA_WITH_AES_128_CCM_8:
                case CipherSuite.TLS_RSA_WITH_AES_128_GCM_SHA256:
                case CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA256:
                case CipherSuite.TLS_RSA_WITH_AES_256_CCM:
                case CipherSuite.TLS_RSA_WITH_AES_256_CCM_8:
                case CipherSuite.TLS_RSA_WITH_AES_256_GCM_SHA384:
                case CipherSuite.TLS_RSA_WITH_CAMELLIA_128_CBC_SHA256:
                case CipherSuite.TLS_RSA_WITH_CAMELLIA_128_GCM_SHA256:
                case CipherSuite.TLS_RSA_WITH_CAMELLIA_256_CBC_SHA256:
                case CipherSuite.TLS_RSA_WITH_CAMELLIA_256_GCM_SHA384:
                case CipherSuite.TLS_RSA_WITH_NULL_SHA256:
                    return ProtocolVersion.TLSv12;

                default:
                    return ProtocolVersion.SSLv3;
            }
        }

        public static void WriteExtensions(IByteBuffer buffer, IDictionary extensions)
        {
            Int32 length=0;
            length+=CalculateSelectedExtensionsLength(extensions, true);
            length+=CalculateSelectedExtensionsLength(extensions, false);
		
		    if((length & 0xFFFF) != length)
			    throw new TlsFatalAlert(AlertDescription.internal_error);

            buffer.WriteShort(length);

            WriteSelectedExtensions(buffer, extensions, true);
            WriteSelectedExtensions(buffer, extensions, false);
        }

        public static Int32 CalculateSelectedExtensionsLength(IDictionary extensions, Boolean selectEmpty)
        {
            Int32 length=0;
            foreach(Int32 key in extensions.Keys)
            {
                int extension_type = key;
                byte[] extension_data = (byte[])extensions[key];

                if (selectEmpty == (extension_data.Length == 0))
                {
                    if ((extension_type & 0xFFFF) != extension_type)
                        throw new TlsFatalAlert(AlertDescription.internal_error);

                    length += 4 + extension_data.Length;
                }
            }
		
		    return length;
        }

        public static void WriteSelectedExtensions(IByteBuffer output, IDictionary extensions, Boolean selectEmpty)
        {
            foreach(Int32 key in extensions.Keys)
		    {
                byte[] extension_data = (byte[])extensions[key];

                if (selectEmpty == (extension_data.Length == 0))
                {
                    output.WriteShort(key);
                    output.WriteShort(extension_data.Length);
                    output.WriteBytes(extension_data);
                }
            }
        }

        public static Dictionary<Int32, byte[]> ReadSelectedExtensions(IByteBuffer output)
        {
            int extentionsLength = output.ReadUnsignedShort();
            Dictionary<Int32, byte[]> extensions = new Dictionary<Int32, byte[]>();
            while (extentionsLength > 0)
            {
                Int32 extension_type = output.ReadUnsignedShort();
                byte[] extension_data = new byte[output.ReadUnsignedShort()];
                output.ReadBytes(extension_data);
            
                if(extensions.ContainsKey(extension_type))
                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);

                extensions[extension_type] = extension_data;
                    
                extentionsLength-=4+extension_data.Length;
            }

            return extensions;
	    }

        public static Int32 CalculateSupplementalDataLength(IList<SupplementalDataEntry> supplementalData)
        {
            Int32 length=0;
		    for (int i = 0; i<supplementalData.Count; ++i)
            {
                SupplementalDataEntry entry = supplementalData[i];
                length+=4+entry.Data.Length;
            }
		
		    return length;
	    }

        public static void WriteSupplementalData(IByteBuffer output, IList<SupplementalDataEntry> supplementalData)
        {
		    for (int i = 0; i<supplementalData.Count; ++i)
            {
                SupplementalDataEntry entry = supplementalData[i];
                output.WriteShort(entry.DataType);
			    output.WriteShort(entry.Data.Length);
			    output.WriteBytes(entry.Data);		
            }
        }

        public static IList ReadSupplementalData(IByteBuffer output)
        {
		    int suppDataLength=ReadUint24(output);
            IList result=new List<SupplementalDataEntry>();
		    while(suppDataLength>0)
		    {
			    int supp_data_type = output.ReadUnsignedShort();
                byte[] data = new byte[output.ReadUnsignedShort()];
                output.ReadBytes(data);
                result.Add(new SupplementalDataEntry(supp_data_type, data));
		    }
		
		    return result;
	    }

        public static IByteBuffer WriteCertificate(short messageSequence, Certificate certificate)
        {
            int totalLength = HANDSHAKE_MESSAGE_HEADER_LENGTH;
            List<byte[]> derEncodings = new List<byte[]>();
		    for (int i = 0; i<certificate.GetCertificateList().Length; ++i)
		    {
			    byte[] derEncoding = certificate.GetCertificateList()[i].GetEncoded(Asn1Encodable.Der);
                derEncodings.Add(derEncoding);
			    totalLength += derEncoding.Length + 3;
		    }

            totalLength+=3;
		    IByteBuffer output = Unpooled.Buffer(totalLength);
            totalLength-=HANDSHAKE_MESSAGE_HEADER_LENGTH;
		    DtlsHelper.WriteHandshakeHeader(messageSequence,MessageType.CERTIFICATE,output,totalLength);
		    totalLength-=3;
		    output.WriteByte((byte)(totalLength >> 16));
            output.WriteByte((byte)(totalLength >> 8));
            output.WriteByte((byte)totalLength);
        
		    for (int i = 0; i<derEncodings.Count; ++i)
		    {
			    byte[] curr = derEncodings[i];
                output.WriteByte((byte)(curr.Length >> 16));
	            output.WriteByte((byte)(curr.Length >> 8));
	            output.WriteByte((byte)curr.Length);
	            output.WriteBytes(curr);
		    }
		
		    return output;
	    }

        public static byte[] GenPad(byte b, int count)
        {
            byte[] padding = new byte[count];
            Arrays.Fill(padding, b);
            return padding;
        }

        public static byte[] CreateRandomBlock(Boolean useGMTUnixTime, IRandomGenerator randomGenerator)
        {
            byte[] result = new byte[32];
            randomGenerator.NextBytes(result);

            if (useGMTUnixTime)
                WriteGMTUnixTime(result, 0);

            return result;
        }

        public static void WriteGMTUnixTime(byte[] buf, int offset)
        {
            int t = (int)(DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            buf[offset] = (byte)(t >> 24);
            buf[offset + 1] = (byte)(t >> 16);
            buf[offset + 2] = (byte)(t >> 8);
            buf[offset + 3] = (byte)t;
        }

        public static int ReadUint24(IByteBuffer buf)
        {
            int result = (buf.ReadShort() << 8);
            result |= (buf.ReadByte() & 0x0FF);
            return result;
        }

        public static byte[] PRF(TlsContext context, byte[] secret, String asciiLabel, byte[] seed, int size)
        {
            ProtocolVersion version = context.ServerVersion;

            if (version.IsSsl)
                throw new InvalidOperationException("No PRF available for SSLv3 session");
           
            byte[] label = Encoding.UTF8.GetBytes(asciiLabel);
            byte[] labelSeed = new byte[label.Length + seed.Length];
            Array.Copy(label, 0, labelSeed, 0, label.Length);
            Array.Copy(seed, 0, labelSeed, label.Length, seed.Length);

            int prfAlgorithm = context.SecurityParameters.PrfAlgorithm;

            if (prfAlgorithm == PrfAlgorithm.tls_prf_legacy)
                return PRF_legacy(secret, label, labelSeed, size);

            IDigest prfDigest = null;
            switch (prfAlgorithm)
            {
                case PrfAlgorithm.tls_prf_legacy:
                    prfDigest = new CombinedHash();
                    break;
                case PrfAlgorithm.tls_prf_sha256:
                    prfDigest = new Sha256Digest();
                    break;
                case PrfAlgorithm.tls_prf_sha384:
                    prfDigest = new Sha384Digest();
                    break;
            }

            byte[] buf = new byte[size];
            hmac_hash(prfDigest, secret, labelSeed, buf);
            return buf;
        }

        public static void hmac_hash(IDigest digest, byte[] secret, byte[] seed, byte[] _out)
        {
            HMac mac = new HMac(digest);
            mac.Init(new KeyParameter(secret));
            byte[] a = seed;
            int size = digest.GetDigestSize();
            int iterations = (_out.Length + size - 1) / size;
            byte[] buf = new byte[mac.GetMacSize()];
            byte[] buf2 = new byte[mac.GetMacSize()];
            for (int i = 0; i < iterations; i++)
            {
                mac.BlockUpdate(a, 0, a.Length);
                mac.DoFinal(buf, 0);
                a = buf;
                mac.BlockUpdate(a, 0, a.Length);
                mac.BlockUpdate(seed, 0, seed.Length);
                mac.DoFinal(buf2, 0);
                Array.Copy(buf2, 0, _out, (size * i), Math.Min(size, _out.Length - (size * i)));
            }
        }

        public static byte[] CalculateVerifyData(TlsContext context, String asciiLabel, byte[] handshakeHash)
        {
            if (context.ServerVersion.IsSsl)
                return handshakeHash;

            SecurityParameters securityParameters = context.SecurityParameters;
            byte[] master_secret = securityParameters.MasterSecret;
            int verify_data_length = securityParameters.VerifyDataLength;

            return PRF(context, master_secret, asciiLabel, handshakeHash, verify_data_length);
        }

        public static byte[] PRF_legacy(byte[] secret, byte[] label, byte[] labelSeed, int size)
        {
            int s_half = (secret.Length + 1) / 2;
            byte[] s1 = new byte[s_half];
            byte[] s2 = new byte[s_half];
            Array.Copy(secret, 0, s1, 0, s_half);
            Array.Copy(secret, secret.Length - s_half, s2, 0, s_half);

            byte[] b1 = new byte[size];
            byte[] b2 = new byte[size];
            hmac_hash(CreateHash(HashAlgorithm.md5), s1, labelSeed, b1);
            hmac_hash(CreateHash(HashAlgorithm.sha1), s2, labelSeed, b2);
            for (int i = 0; i < size; i++)
            {
                b1[i] ^= b2[i];
            }
            return b1;
        }

        public static byte[] GetCurrentPRFHash(TlsContext context, TlsHandshakeHash handshakeHash, byte[] sslSender)
        {
            IDigest d = handshakeHash.ForkPrfHash();
            if (sslSender != null && context.ServerVersion.IsSsl)
                d.BlockUpdate(sslSender, 0, sslSender.Length);

            byte[] bs = new byte[d.GetDigestSize()];
            d.DoFinal(bs, 0);
            return bs;
        }

        public static void EstablishMasterSecret(AsyncDtlsSecurityParameters securityParameters, TlsContext context, TlsKeyExchange keyExchange)
        {
            byte[] pre_master_secret = keyExchange.GeneratePremasterSecret();

	        try
	        {
	    	    securityParameters.SetMasterSecret(CalculateMasterSecret(securityParameters, context, pre_master_secret));	    	
	        }
	        finally
	        {
	    	    if (pre_master_secret != null)
	        		Arrays.Fill(pre_master_secret, (byte)0);
	        }
    	}

        public static byte[] CalculateMasterSecret(AsyncDtlsSecurityParameters securityParameters, TlsContext context, byte[] pre_master_secret)
        {
            byte[] seed;
            if (securityParameters.IsExtendedMasterSecret())
                seed = securityParameters.SessionHash;
            else
            {
                seed = new byte[securityParameters.ClientRandom.Length + securityParameters.ServerRandom.Length];
                Array.Copy(securityParameters.ClientRandom, 0, seed, 0, securityParameters.ClientRandom.Length);
                Array.Copy(securityParameters.ServerRandom, 0, seed, securityParameters.ClientRandom.Length, securityParameters.ServerRandom.Length);
            }

            if (context.ServerVersion.IsSsl)
                return CalculateMasterSecret_SSL(pre_master_secret, seed);

            String asciiLabel = securityParameters.IsExtendedMasterSecret() ? ExporterLabel.extended_master_secret : ExporterLabel.master_secret;

            return PRF(context, pre_master_secret, asciiLabel, seed, 48);
        }

        public static byte[] CalculateMasterSecret_SSL(byte[] pre_master_secret, byte[] random)
        {
            IDigest md5 = CreateHash(HashAlgorithm.md5);
            IDigest sha1 = CreateHash(HashAlgorithm.sha1);
            int md5Size = md5.GetDigestSize();
            byte[] shatmp = new byte[sha1.GetDigestSize()];

            byte[] rval = new byte[md5Size * 3];
            int pos = 0;

            for (int i = 0; i < 3; ++i)
            {
                byte[] ssl3Const = SSL3_CONST[i];

                sha1.BlockUpdate(ssl3Const, 0, ssl3Const.Length);
                sha1.BlockUpdate(pre_master_secret, 0, pre_master_secret.Length);
                sha1.BlockUpdate(random, 0, random.Length);
                sha1.DoFinal(shatmp, 0);

                md5.BlockUpdate(pre_master_secret, 0, pre_master_secret.Length);
                md5.BlockUpdate(shatmp, 0, shatmp.Length);
                md5.DoFinal(rval, pos);

                pos += md5Size;
            }

            return rval;
        }

        public static SignatureAndHashAlgorithm GetSignatureAndHashAlgorithm(TlsContext context, TlsSignerCredentials signerCredentials)
        {
            SignatureAndHashAlgorithm signatureAndHashAlgorithm = null;
    	    if (ProtocolVersion.TLSv12.IsEqualOrEarlierVersionOf(context.ServerVersion.GetEquivalentTLSVersion()))
            {
                signatureAndHashAlgorithm = signerCredentials.SignatureAndHashAlgorithm;
                if (signatureAndHashAlgorithm == null)
                    throw new TlsFatalAlert(AlertDescription.internal_error);
            }

            return signatureAndHashAlgorithm;
        }

        public static Asn1Object ReadASN1Object(byte[] encoding)
        {
            Asn1InputStream asn1 = new Asn1InputStream(encoding);
            Asn1Object result = asn1.ReadObject();
            asn1.Close();
        
            if (null == result)
            {
                throw new TlsFatalAlert(AlertDescription.decode_error);
            }

            return result;
        }

        public static SignatureAndHashAlgorithm ParseSignatureAndHashAlgorithm(IByteBuffer data)
        {
            byte hash = data.ReadByte();
            byte signature = data.ReadByte();
    	    return new SignatureAndHashAlgorithm(hash, signature);
        }

        public static IList ParseSupportedSignatureAlgorithms(Boolean allowAnonymous, IByteBuffer data)
        {
            int length = data.ReadUnsignedShort();
    	    if (length< 2 || (length & 1) != 0)
    		    throw new TlsFatalAlert(AlertDescription.decode_error);

            int count = length / 2;
            IList supportedSignatureAlgorithms = new List<SignatureAndHashAlgorithm>(count);
            for (int i = 0; i<count; ++i)
            {
        	    SignatureAndHashAlgorithm entry = ParseSignatureAndHashAlgorithm(data);
        	    if (!allowAnonymous && entry.Signature == SignatureAlgorithm.anonymous)
                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);

                supportedSignatureAlgorithms.Add(entry);
            }

            return supportedSignatureAlgorithms;
        }

        public static Certificate ParseCertificate(IByteBuffer data)
        {
            int totalLength = ReadUint24(data);
    	    if (totalLength == 0)
    		    return Certificate.EmptyChain;
        
            IList<X509CertificateStructure> certificate_list = new List<X509CertificateStructure>();
    	    while (data.ReadableBytes > 0)
    	    {
    		    byte[] berEncoding = new byte[ReadUint24(data)];
                data.ReadBytes(berEncoding);
                Asn1Object asn1Cert = ReadASN1Object(berEncoding);
                certificate_list.Add(X509CertificateStructure.GetInstance(asn1Cert));
    	    }

            X509CertificateStructure[] certificateList = new X509CertificateStructure[certificate_list.Count];
    	    for (int i = 0; i<certificate_list.Count; i++)
    		    certificateList[i] = certificate_list[i];
    	    
    	    return new Certificate(certificateList);
        }

        public static CertificateStatus ParseCertificateStatus(IByteBuffer data)
        {
    	    byte status_type = data.ReadByte();
            Object response;

            switch (status_type)
            {
	            case CertificateStatusType.ocsp:
	            {
                    byte[] derEncoding = new byte[ReadUint24(data)];
                    data.ReadBytes(derEncoding);

                    Asn1InputStream asn1 = new Asn1InputStream(derEncoding);
                    Asn1Object result = asn1.ReadObject();
                    asn1.Close();
                    if (null == result)
                        throw new TlsFatalAlert(AlertDescription.decode_error);

                    if (null != asn1.ReadObject())
                        throw new TlsFatalAlert(AlertDescription.decode_error);

                    response = OcspResponse.GetInstance(result);
                    break;
                }
                default:
	                throw new TlsFatalAlert(AlertDescription.decode_error);
            }

            return new CertificateStatus(status_type, response);
        }

        public static long ReadUint48(IByteBuffer buf)
        {
            long result = ((long)buf.ReadInt()) << 16;
            result |= (long)(buf.ReadShort() & 0x0FFFF);
            return result;
        }

        public static void WriteUint48(Int64 value, IByteBuffer buf)
        {
            buf.WriteInt((int)((value >> 16) & 0x0FFFFFFFF));
            buf.WriteShort((short)(value & 0x0FFFF));
        }

        public static void WriteHandshakeHeader(short messageSequence, MessageType messageType, IByteBuffer buffer, int totalLength)
        {
            //message type
            buffer.WriteByte((Int32)messageType);
            //length
            buffer.WriteByte((byte)(totalLength >> 16));
            buffer.WriteByte((byte)(totalLength >> 8));
            buffer.WriteByte((byte)totalLength);
            //message sequence
            buffer.WriteShort(messageSequence);
            //fragment offset
            buffer.WriteByte(0);
            buffer.WriteByte(0);
            buffer.WriteByte(0);
            //fragment length
            buffer.WriteByte((byte)(totalLength >> 16));
            buffer.WriteByte((byte)(totalLength >> 8));
            buffer.WriteByte((byte)totalLength);
        }

        public static void WriteHandshakeHeader(int fragmentOffset, int fragmentLength, short messageSequence, MessageType messageType, IByteBuffer buffer, int totalLength)
        {
            //message type
            buffer.WriteByte((Int32)messageType);
            //length
            buffer.WriteByte((byte)(totalLength >> 16));
            buffer.WriteByte((byte)(totalLength >> 8));
            buffer.WriteByte((byte)totalLength);
            //message sequence
            buffer.WriteShort(messageSequence);
            //fragment offset
            buffer.WriteByte((byte)(fragmentOffset >> 16));
            buffer.WriteByte((byte)(fragmentOffset >> 8));
            buffer.WriteByte((byte)fragmentOffset);
            //fragment length
            buffer.WriteByte((byte)(fragmentLength >> 16));
            buffer.WriteByte((byte)(fragmentLength >> 8));
            buffer.WriteByte((byte)fragmentLength);
        }

        public static HandshakeHeader ReadHandshakeHeader(IByteBuffer data)
        {
            MessageType messageType = (MessageType)data.ReadByte();
            Int32 totalLength = ReadUint24(data);
            Int16 messageSequence = data.ReadShort();
            Int32 fragmentOffset = ReadUint24(data);
            Int32 fragmentLength = ReadUint24(data);
            return new HandshakeHeader(fragmentOffset, fragmentLength, totalLength, messageType, messageSequence);
        }

        public static IDigest CreateHash(short hashAlgorithm)
        {
            switch (hashAlgorithm)
            {
                case HashAlgorithm.md5:
                    return new MD5Digest();
                case HashAlgorithm.sha1:
                    return new Sha1Digest();
                case HashAlgorithm.sha224:
                    return new Sha224Digest();
                case HashAlgorithm.sha256:
                    return new Sha256Digest();
                case HashAlgorithm.sha384:
                    return new Sha384Digest();
                case HashAlgorithm.sha512:
                    return new Sha512Digest();
                default:
                    throw new ArgumentException("unknown HashAlgorithm");
            }
        }

        public static IDigest CloneHash(short hashAlgorithm, IDigest hash)
        {            
            switch (hashAlgorithm)
            {
                case HashAlgorithm.md5:
                    return new MD5Digest((MD5Digest)hash);
                case HashAlgorithm.sha1:
                    return new Sha1Digest((Sha1Digest)hash);
                case HashAlgorithm.sha224:
                    return new Sha224Digest((Sha224Digest)hash);
                case HashAlgorithm.sha256:
                    return new Sha256Digest((Sha256Digest)hash);
                case HashAlgorithm.sha384:
                    return new Sha384Digest((Sha384Digest)hash);
                case HashAlgorithm.sha512:
                    return new Sha512Digest((Sha512Digest)hash);
                default:
                    throw new ArgumentException("unknown HashAlgorithm");
            }
        }
    }
}
