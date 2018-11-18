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
    public class AsyncDtlsClientState
    {
        #region private fields

        private TlsClient client = null;
        private AsyncDtlsClientContext clientContext = null;
        private TlsSession tlsSession = null;
        private SessionParameters sessionParameters = null;
        private SessionParameters.Builder sessionParametersBuilder = null;
        private int[] offeredCipherSuites = null;
        private short[] offeredCompressionMethods = null;
        private IDictionary clientExtensions = null;
        private IDictionary serverExtensions = null;
        private byte[] selectedSessionID = null;
        private Boolean resumedSession = false;
        private Boolean secure_renegotiation = false;
        private Boolean allowCertificateStatus = false;
        private Boolean expectSessionTicket = false;
        private TlsKeyExchange keyExchange = null;
        private TlsAuthentication authentication = null;
        private CertificateStatus certificateStatus = null;
        private CertificateRequest certificateRequest = null;
        private TlsCredentials clientCredentials = null;
        private TlsHandshakeHash handshakeHash = null;

        #endregion

        #region public fields

        public TlsClient Client
        {
            get
            {
                return client;
            }

            set
            {
                this.client = value;
            }
        }

        public AsyncDtlsClientContext ClientContext
        {
            get
            {
                return clientContext;
            }

            set
            {
                this.clientContext = value;
            }
        }

        public TlsSession TlsSession
        {
            get
            {
                return tlsSession;
            }

            set
            {
                this.tlsSession = value;
            }
        }

        public SessionParameters SessionParameters
        {
            get
            {
                return sessionParameters;
            }

            set
            {
                this.sessionParameters = value;
            }
        }

        public SessionParameters.Builder SessionParametersBuilder
        {
            get
            {
                return sessionParametersBuilder;
            }

            set
            {
                this.sessionParametersBuilder = value;
            }
        }

        public int[] OfferedCipherSuites
        {
            get
            {
                return offeredCipherSuites;
            }

            set
            {
                this.offeredCipherSuites = value;
            }
        }

        public short[] OfferedCompressionMethods
        {
            get
            {
                return offeredCompressionMethods;
            }

            set
            {
                this.offeredCompressionMethods = value;
            }
        }

        public IDictionary ClientExtensions
        {
            get
            {
                return clientExtensions;
            }

            set
            {
                this.clientExtensions = value;
            }
        }

        public IDictionary ServerExtensions
        {
            get
            {
                return serverExtensions;
            }

            set
            {
                this.serverExtensions = value;
            }
        }

        public byte[] SelectedSessionID
        {
            get
            {
                return selectedSessionID;
            }

            set
            {
                this.selectedSessionID = value;
            }
        }

        public Boolean ResumedSession
        {
            get
            {
                return resumedSession;
            }

            set
            {
                this.resumedSession = value;
            }
        }

        public Boolean SecureRenegotiation
        {
            get
            {
                return secure_renegotiation;
            }

            set
            {
                this.secure_renegotiation = value;
            }
        }

        public Boolean AllowCertificateStatus
        {
            get
            {
                return allowCertificateStatus;
            }

            set
            {
                this.allowCertificateStatus = value;
            }
        }

        public Boolean ExpectSessionTicket
        {
            get
            {
                return expectSessionTicket;
            }

            set
            {
                this.expectSessionTicket = value;
            }
        }

        public TlsKeyExchange KeyExchange
        {
            get
            {
                return keyExchange;
            }

            set
            {
                this.keyExchange = value;
            }
        }

        public TlsAuthentication Authentication
        {
            get
            {
                return authentication;
            }

            set
            {
                this.authentication = value;
            }
        }

        public CertificateStatus CertificateStatus
        {
            get
            {
                return certificateStatus;
            }

            set
            {
                this.certificateStatus = value;
            }
        }

        public CertificateRequest CertificateRequest
        {
            get
            {
                return certificateRequest;
            }

            set
            {
                this.certificateRequest = value;
            }
        }

        public TlsCredentials ClientCredentials
        {
            get
            {
                return clientCredentials;
            }

            set
            {
                this.clientCredentials = value;
            }
        }

        public TlsHandshakeHash HandshakeHash
        {
            get
            {
                return handshakeHash;
            }

            set
            {
                this.handshakeHash = value;
            }
        }

        #endregion
    }
}
