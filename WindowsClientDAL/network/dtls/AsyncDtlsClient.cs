using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Pkcs;
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
    public class AsyncDtlsClient:DefaultTlsClient
    {
        private CertificateData certificateData;
        private X509CertificateStructure[] serverCertificates;

        private Pkcs12Store keystore;
        private String keystorePassword;
        private int[] definedCipherSuities = null;

        public AsyncDtlsClient(Pkcs12Store keystore, String keystorePassword, int[] definedCipherSuities)
        {
            this.keystore = keystore;
            this.keystorePassword = keystorePassword;
            this.definedCipherSuities = definedCipherSuities;
        }

        public void InitClient(TlsClientContext context)
	    {
            base.Init(context);
            this.certificateData=new CertificateData(keystore, keystorePassword, context, true, null);
        }

        override
        public ProtocolVersion ClientVersion
        {
            get
            {
                return ProtocolVersion.DTLSv12;
            }
        }

        override
        public ProtocolVersion MinimumVersion
        {
            get
            {
                return ProtocolVersion.DTLSv10;
            }            
        }

        public X509CertificateStructure[] getServerCertificates()
        {
            return this.serverCertificates;
        }

        override
        public int[] GetCipherSuites()
        {
            if (definedCipherSuities != null)
                return definedCipherSuities;

            return new int[]
            {
                CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256,
                CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA384,
                CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256,
                CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA,
                CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384,
                CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256,
                CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384,
                CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256,
                CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA,
                CipherSuite.TLS_DHE_DSS_WITH_AES_128_GCM_SHA256,
                CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA256,
                CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA,
                CipherSuite.TLS_DHE_RSA_WITH_AES_128_GCM_SHA256,
                CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA256,
                CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA,
                CipherSuite.TLS_RSA_WITH_AES_128_GCM_SHA256,
                CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA256,
                CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA,
            };
        }

        override
        public TlsAuthentication GetAuthentication()
        {
            return new AsyncDtlsClientAuthentication(this);
        }
	
	    override
        protected TlsKeyExchange CreateDHKeyExchange(int keyExchange)
        {            
            return new AsyncTlsDHEKeyExchange(keyExchange, mSupportedSignatureAlgorithms, null);
        }

        override
        protected TlsKeyExchange CreateECDheKeyExchange(int keyExchange)
        {
            return new AsyncTlsECDHEKeyExchange(keyExchange, mSupportedSignatureAlgorithms, mNamedCurves, mClientECPointFormats, mServerECPointFormats);
        }

        public CertificateData CertificateData
        {
            get
            {
                return certificateData;
            }

            set
            {
                certificateData = value;
            }
        }

        public X509CertificateStructure[] ServerCertificates
        {
            get
            {
                return serverCertificates;
            }

            set
            {
                serverCertificates = value;
            }
        }
    }
}