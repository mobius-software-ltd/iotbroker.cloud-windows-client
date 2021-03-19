using DotNetty.Buffers;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Tls;
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
    public class AsyncCertificateRequest: CertificateRequest
    {
        public AsyncCertificateRequest(byte[] certificateTypes, IList supportedSignatureAlgorithms, IList certificateAuthorities):base(certificateTypes, supportedSignatureAlgorithms, certificateAuthorities)
        {
        }

        public IByteBuffer Encode(short sequence)
        {
            int length = DtlsHelper.HANDSHAKE_MESSAGE_HEADER_LENGTH;
		    if (CertificateTypes == null || CertificateTypes.Length == 0)
			    length++;
            else
        	    length+=1+CertificateTypes.Length;
		
		    if (SupportedSignatureAlgorithms != null)
		    {
			    for (int i = 0; i<SupportedSignatureAlgorithms.Count; ++i)
	            {
	                SignatureAndHashAlgorithm entry = (SignatureAndHashAlgorithm)SupportedSignatureAlgorithms[i];
	                if (entry.Signature == SignatureAlgorithm.anonymous)
	            	    throw new ArgumentException("SignatureAlgorithm.anonymous MUST NOT appear in the signature_algorithms extension");
                }

                length += 2* SupportedSignatureAlgorithms.Count + 2;
		    }

            IList<byte[]> derEncodings = new List<byte[]>(CertificateAuthorities.Count);
            int totalLength = 0;
        
		    if (CertificateAuthorities == null || CertificateAuthorities.Count==0)
        	    length+=2;
            else
            {
                length+=2;
                for (int i = 0; i<CertificateAuthorities.Count; ++i)
                {
                    X509Name certificateAuthority = (X509Name)CertificateAuthorities[i];
                    byte[] derEncoding = certificateAuthority.GetEncoded(Asn1Encodable.Der);
                    derEncodings.Add(derEncoding);
                    length += derEncoding.Length + 2;
                    totalLength+=derEncoding.Length + 2;
                }
            }
		
		    IByteBuffer buffer = Unpooled.Buffer(length);
            DtlsHelper.WriteHandshakeHeader(sequence,MessageType.CERTIFICATE_REQUEST,buffer,length-DtlsHelper.HANDSHAKE_MESSAGE_HEADER_LENGTH);
    	
		    if (CertificateTypes == null || CertificateTypes.Length == 0)
        	    buffer.WriteByte(0);
            else
            {
        	    buffer.WriteByte(CertificateTypes.Length);
        	    foreach(byte curr in CertificateTypes)
        		    buffer.WriteByte(curr);        		             
            }

            if (SupportedSignatureAlgorithms != null)
            {
        	    buffer.WriteShort(2 * SupportedSignatureAlgorithms.Count);
                for (int i = 0; i<SupportedSignatureAlgorithms.Count; ++i)
                {
                    SignatureAndHashAlgorithm entry = (SignatureAndHashAlgorithm)SupportedSignatureAlgorithms[i];
                    buffer.WriteByte(entry.Hash);
                    buffer.WriteByte(entry.Signature);                
                }
            }

            if (CertificateAuthorities == null || CertificateAuthorities.Count==0)
            	buffer.WriteShort(0);
            else
            {
                buffer.WriteShort(totalLength);
                for (int i = 0; i<derEncodings.Count; ++i)
                {
                    byte[] derEncoding = (byte[])derEncodings[i];
                    buffer.WriteShort(derEncoding.Length);
                    buffer.WriteBytes(derEncoding);
                }
            }
        
            return buffer;
	    }

        public static AsyncCertificateRequest Parse(ProtocolVersion version, IByteBuffer data)
        {
		    int numTypes = data.ReadByte() & 0x0FF;
		    byte[] certificateTypes = new byte[numTypes];
		    for (int i = 0; i<numTypes; ++i)
			    certificateTypes[i] = data.ReadByte();
	     
		    IList supportedSignatureAlgorithms = null;
		    if (ProtocolVersion.TLSv12.IsEqualOrEarlierVersionOf(version.GetEquivalentTLSVersion()))
			    supportedSignatureAlgorithms = DtlsHelper.ParseSupportedSignatureAlgorithms(false, data);
	        
		    IList certificateAuthorities = new List<X509Name>();
            int remainingBytes = data.ReadUnsignedShort();
		    while (remainingBytes>0)
		    {
			    byte[] derEncoding = new byte[data.ReadUnsignedShort()];
                data.ReadBytes(derEncoding);
                Asn1InputStream asn1 = new Asn1InputStream(derEncoding);
                Asn1Object result = asn1.ReadObject();
                asn1.Close();

                if (null == result)
				    throw new TlsFatalAlert(AlertDescription.decode_error);
		        
                certificateAuthorities.Add(X509Name.GetInstance(result));
			    remainingBytes-=2+derEncoding.Length;
		    }

		    return new AsyncCertificateRequest(certificateTypes, supportedSignatureAlgorithms, certificateAuthorities);
	    }
    }
}
