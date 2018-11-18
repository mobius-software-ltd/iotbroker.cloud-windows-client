using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public class CertificateData
    {
        private X509Certificate certificate;
        private AsymmetricKeyParameter keyParameter;
        private Certificate chain;
        private TlsAgreementCredentials agreementCredentials;
        private TlsEncryptionCredentials encryptionCredentials;
        private TlsContext tlsContext;

        public CertificateData(Pkcs12Store keystore, String keystorePassword, TlsContext tlsContext, Boolean isClient, String certificateAlias)
	    {
		    this.tlsContext=tlsContext;
		    List<X509Certificate> allCertificates = new List<X509Certificate>();
		
		    if(keystore!=null)
		    {	
			    IEnumerator aliasesEnum = keystore.Aliases.GetEnumerator();
			    while(aliasesEnum.MoveNext())
			    {
                    String alias = (String)aliasesEnum.Current;
                    X509Certificate currCertificate = keystore.GetCertificate(alias).Certificate;
                    AsymmetricKeyParameter currParameter = null;
				    if(certificateAlias==null || certificateAlias.Equals(alias))
                        currParameter = keystore.GetKey(alias).Key;				
				
				    if(currParameter != null)
				    {
					    certificate=currCertificate;
					    keyParameter = currParameter;
				    }
				
				    if(currCertificate!=null)
				    {
					    if(keyParameter != null)
						    allCertificates.Insert(0, currCertificate);
					    else
						    allCertificates.Add(currCertificate);
				    }
			    }
		    }
		
		    if(!isClient && keyParameter == null)
			    throw new InvalidOperationException("No private key found");
		
		    if(!isClient && allCertificates.Count==0)
			    throw new InvalidOperationException("No certificate found");
            
            X509CertificateStructure[] certificateChain = new X509CertificateStructure[allCertificates.Count];
            for (int i = 0; i<allCertificates.Count; ++i) 
        	    certificateChain[i] = allCertificates[i].CertificateStructure;
        
            chain=new Certificate(certificateChain);
            if(!chain.IsEmpty)
            {
        	    try
        	    {
                    if (keyParameter != null)
                        encryptionCredentials = new DefaultTlsEncryptionCredentials(tlsContext, chain, keyParameter);

                    agreementCredentials = new DefaultTlsAgreementCredentials(chain, keyParameter);
        	    }
        	    catch(Exception)
        	    {
        		    //may be invalid key format
        	    }
            }
	    }

	    public X509Certificate getCertificate()
        {
            return certificate;
        }

        public AsymmetricKeyParameter getKeyParameter()
        {
            return keyParameter;
        }

        public Certificate getChain()
        {
            return chain;
        }

        public TlsAgreementCredentials getAgreementCredentials()
        {
            return agreementCredentials;
        }

        public TlsEncryptionCredentials getEncryptionCredentials()
        {
            return encryptionCredentials;
        }

        public TlsSignerCredentials getSignerCredentials(SignatureAndHashAlgorithm signatureAndHashAlgorithm)
        {
            if (chain.IsEmpty)
                return null;

            return new DefaultTlsSignerCredentials(tlsContext, chain, keyParameter, signatureAndHashAlgorithm);
        }
    }
}
