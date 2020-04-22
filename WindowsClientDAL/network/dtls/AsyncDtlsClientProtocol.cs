using com.mobius.software.windows.iotbroker.dal;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
    public class AsyncDtlsClientProtocol: HandshakeHandler
    {
        enum State
        {
            INIT, CLIENT_HELLO_SENT, SERVER_HELLO_RECEIVED, SUPP_DATA_RECEIVED, CERTIFICATE_RECEIVED, CERTIFICATE_STATUS_RECEIVED, SERVER_KEY_EXCHANGE_RECEIVED, CERTIFICATE_REQUEST_RECEIVED, SERVER_HELLO_DONE, FINISH_SENT, SESSION_TICKET_RECEIVED, ENDED
        }

        private AsyncDtlsClientState clientState;
        private AsyncDtlsRecordLayer recordLayer;

        private State handshakeState = State.INIT;
        private Certificate serverCertificate;

        private short sequence = 0;
        private HandshakeHandler parentHandler;
        private DtlsStateHandler handler;

        private IChannel channel;
        private ProtocolVersion protocolVersion;

        public AsyncDtlsClientProtocol(AsyncDtlsClient client, SecureRandom secureRandom, IChannel channel, HandshakeHandler parentHandler, DtlsStateHandler handler, Boolean useExtendedMasterSecret, ProtocolVersion initialVersion)
	    {
		    this.parentHandler=parentHandler;
		    this.handler=handler;
	
		    this.channel=channel;
		    this.protocolVersion=initialVersion;
		
		    AsyncDtlsSecurityParameters securityParameters = new AsyncDtlsSecurityParameters();
            securityParameters.SetEntity(ConnectionEnd.client);

            clientState = new AsyncDtlsClientState();
            clientState.Client = client;
            clientState.ClientContext = new AsyncDtlsClientContext(secureRandom, securityParameters);

            securityParameters.SetExtendedMasterSecret(useExtendedMasterSecret);
            securityParameters.SetClientRandom(DtlsHelper.CreateRandomBlock(client.ShouldUseGmtUnixTime(),clientState.ClientContext.NonceRandomGenerator));
            client.InitClient(clientState.ClientContext);

            clientState.HandshakeHash = new DeferredHash();
    	    clientState.HandshakeHash.Init(clientState.ClientContext);
        
            recordLayer = new AsyncDtlsRecordLayer(clientState.HandshakeHash, this, channel,clientState.ClientContext, client);
	    }

        public Certificate GetServerCertificate()
        {
            return this.serverCertificate;
        }

        public void SendAlert(byte alertLevel, byte alertDescription, String message, Exception cause)
        {
            recordLayer.SendAlert(alertLevel, alertDescription, message, cause);
        }

        public void sendPacket(IByteBuffer data)
        {
            recordLayer.Send(data);
        }

        public List<IByteBuffer> ReceivePacket(IByteBuffer data)
        {
		    return recordLayer.Receive(data);
        }

        public void InitHandshake(byte[] cookie)
        {
            AsyncDtlsSecurityParameters securityParameters = (AsyncDtlsSecurityParameters)clientState.ClientContext.SecurityParameters;

            ProtocolVersion client_version = clientState.Client.ClientVersion;
            if (!client_version.IsDtls)
                throw new TlsFatalAlert(AlertDescription.internal_error);

            AsyncDtlsClientContext context = clientState.ClientContext;
            context.ClientVersion = client_version;
        
            Boolean fallback = clientState.Client.IsFallback;

            //Cipher suites
            clientState.OfferedCipherSuites = clientState.Client.GetCipherSuites();

            // Integer -> byte[]
            clientState.ClientExtensions = clientState.Client.GetClientExtensions();
            if(securityParameters.IsExtendedMasterSecret())
            {
                if(clientState.ClientExtensions == null)
            	    clientState.ClientExtensions = new Dictionary<int, byte[]>();
            
                clientState.ClientExtensions[DtlsHelper.EXT_extended_master_secret] =  DtlsHelper.EMPTY_BYTES;
            }

            byte[] renegExtData = null;
            if(clientState.ClientExtensions.Contains(DtlsHelper.EXT_RenegotiationInfo))
                renegExtData= (byte[])clientState.ClientExtensions[DtlsHelper.EXT_RenegotiationInfo];

            Boolean noRenegExt = (null == renegExtData);
            Boolean noRenegSCSV = true;
            for(int i = 0; i<clientState.OfferedCipherSuites.Length;i++)
        	if(clientState.OfferedCipherSuites[i]==CipherSuite.TLS_EMPTY_RENEGOTIATION_INFO_SCSV)
        	{
        		noRenegSCSV=false;
        		break;
        	}
        
            Boolean tlsFallbackFound = false;
            for(int i = 0; i<clientState.OfferedCipherSuites.Length; i++)
        	    if(clientState.OfferedCipherSuites[i]==CipherSuite.TLS_FALLBACK_SCSV)
        	    {
        		    tlsFallbackFound=true;
        		    break;
        	    }
        
            int additionalCount = 0;
            if (noRenegExt && noRenegSCSV)
                additionalCount++;
        
            if (fallback && !tlsFallbackFound)
                additionalCount++;
        
            int[] offeredCipherSuites = clientState.OfferedCipherSuites;
            if(additionalCount>0)
            {
        	    offeredCipherSuites=new int[clientState.OfferedCipherSuites.Length + additionalCount];
        	    Array.Copy(clientState.OfferedCipherSuites, 0, offeredCipherSuites, 0, clientState.OfferedCipherSuites.Length);
        	    if (noRenegExt && noRenegSCSV)
        		    offeredCipherSuites[clientState.OfferedCipherSuites.Length]=CipherSuite.TLS_EMPTY_RENEGOTIATION_INFO_SCSV;        		
        	
        	    if (fallback && !tlsFallbackFound)
        		    offeredCipherSuites[offeredCipherSuites.Length - 1]=CipherSuite.TLS_FALLBACK_SCSV;
            }
        
            clientState.OfferedCompressionMethods = new short[]{ CompressionMethod.cls_null };
        
            byte[] session_id = DtlsHelper.EMPTY_BYTES;
            if (clientState.TlsSession != null)
            {
                session_id = clientState.TlsSession.SessionID;
                if (session_id == null || session_id.Length > 32)
                    session_id = DtlsHelper.EMPTY_BYTES;
            }
        
            int totalLength = 2;
            totalLength += securityParameters.ClientRandom.Length;
            totalLength += 1 + session_id.Length;
        
            if(cookie!=null)
        	    totalLength+=cookie.Length+1;
            else
        	    totalLength+=1;
        
            totalLength += 2 + 2* offeredCipherSuites.Length;
            totalLength += 1 + clientState.OfferedCompressionMethods.Length; 
            totalLength += DtlsHelper.CalculateExtensionsLength(clientState.ClientExtensions);
        
            int capacity = DtlsHelper.HANDSHAKE_MESSAGE_HEADER_LENGTH + totalLength;
            IByteBuffer data = Unpooled.Buffer(capacity);
            short currSequence = sequence++;
            DtlsHelper.WriteHandshakeHeader(currSequence,MessageType.CLIENT_HELLO,data,totalLength);
            data.WriteByte(client_version.MajorVersion);
            data.WriteByte(client_version.MinorVersion);
        
            data.WriteBytes(securityParameters.ClientRandom);

            // Session ID
            data.WriteByte(session_id.Length);
            data.WriteBytes(session_id);
        
            //Cookie
            if(cookie!=null)
            {
        	    data.WriteByte(cookie.Length);
                data.WriteBytes(cookie);            
            }	
            else
        	    data.WriteBytes(DtlsHelper.EMPTY_BYTES_WITH_LENGTH);
        
            data.WriteShort(2* offeredCipherSuites.Length);
            for(int i = 0; i<offeredCipherSuites.Length;i++)
        	    data.WriteShort(offeredCipherSuites[i]);            
        
            data.WriteByte(clientState.OfferedCompressionMethods.Length);
            for(int i = 0; i<clientState.OfferedCompressionMethods.Length;i++)
        	    data.WriteByte(clientState.OfferedCompressionMethods[i]);
        
            // Extensions
            if (clientState.ClientExtensions != null)
        	    DtlsHelper.WriteExtensions(data, clientState.ClientExtensions);
        
            if(protocolVersion==null)
        	    recordLayer.SetWriteVersion(ProtocolVersion.DTLSv10);
            else
        	    recordLayer.SetWriteVersion(protocolVersion);
        
            recordLayer.Send(currSequence, MessageType.CLIENT_HELLO, data);
            handshakeState=State.CLIENT_HELLO_SENT;
        
            if(handler!=null)
        	    handler.handshakeStarted(channel);
	    }


        public void PostProcessServerHelloDone()
        {
            IList<SupplementalDataEntry> clientSupplementalData = (IList<SupplementalDataEntry>)clientState.Client.GetClientSupplementalData();
            if (clientSupplementalData != null)
            {
                int totalLength = 3 + DtlsHelper.CalculateSupplementalDataLength(clientSupplementalData);
                IByteBuffer supplementalDataOutput = Unpooled.Buffer(DtlsHelper.HANDSHAKE_MESSAGE_HEADER_LENGTH + totalLength);
                short sdataSequence = sequence++;
                DtlsHelper.WriteHandshakeHeader(sdataSequence, MessageType.SUPPLEMENTAL_DATA, supplementalDataOutput, totalLength);
                DtlsHelper.WriteSupplementalData(supplementalDataOutput, clientSupplementalData);
                recordLayer.Send(sdataSequence, MessageType.SUPPLEMENTAL_DATA, supplementalDataOutput);
            }

            if (clientState.CertificateRequest != null)
            {
                clientState.ClientCredentials = clientState.Authentication.GetClientCredentials(clientState.CertificateRequest);
                Certificate clientCertificate = null;
                if (clientState.ClientCredentials != null)
                    clientCertificate = clientState.ClientCredentials.Certificate;

                if (clientCertificate == null)
                    clientCertificate = Certificate.EmptyChain;

                short certificateSequence = sequence++;
                IByteBuffer certificateOutput = DtlsHelper.WriteCertificate(certificateSequence, clientCertificate);
                recordLayer.Send(certificateSequence, MessageType.CERTIFICATE, certificateOutput);
            }

            if (clientState.ClientCredentials != null)
        	    clientState.KeyExchange.ProcessClientCredentials(clientState.ClientCredentials);
            else
        	    clientState.KeyExchange.SkipClientCredentials();

            MemoryStream buf = new MemoryStream();
            clientState.KeyExchange.GenerateClientKeyExchange(buf);
            byte[] clientKeyExchange = buf.GetBuffer();
            Array.Resize(ref clientKeyExchange, clientKeyExchange[0] + 1);
            IByteBuffer keyExchangeOutput = Unpooled.Buffer(DtlsHelper.HANDSHAKE_MESSAGE_HEADER_LENGTH + clientKeyExchange.Length);
            short currSequence = sequence++;
            DtlsHelper.WriteHandshakeHeader(currSequence,MessageType.CLIENT_KEY_EXCHANGE,keyExchangeOutput,clientKeyExchange.Length);
            keyExchangeOutput.WriteBytes(clientKeyExchange);
            recordLayer.Send(currSequence,MessageType.CLIENT_KEY_EXCHANGE, keyExchangeOutput);
        
            TlsHandshakeHash prepareFinishHash = clientState.HandshakeHash;
            //clientState.setHandshakeHash(clientState.getHandshakeHash().stopTracking());

            ((AsyncDtlsSecurityParameters)clientState.ClientContext.SecurityParameters).SetSessionHash(DtlsHelper.GetCurrentPRFHash(clientState.ClientContext, prepareFinishHash, null));
        
            DtlsHelper.EstablishMasterSecret((AsyncDtlsSecurityParameters)clientState.ClientContext.SecurityParameters, clientState.ClientContext, clientState.KeyExchange);
            recordLayer.InitPendingEpoch(clientState.Client.GetCipher());

            if (clientState.ClientCredentials != null && clientState.ClientCredentials is TlsSignerCredentials)
            {
                TlsSignerCredentials signerCredentials = (TlsSignerCredentials)clientState.ClientCredentials;

                SignatureAndHashAlgorithm signatureAndHashAlgorithm = DtlsHelper.GetSignatureAndHashAlgorithm(clientState.ClientContext, signerCredentials);

                byte[] hash;
                if (signatureAndHashAlgorithm == null)
                    hash = ((AsyncDtlsSecurityParameters)clientState.ClientContext.SecurityParameters).SessionHash;
                else
                    hash = prepareFinishHash.GetFinalHash(signatureAndHashAlgorithm.Hash);
                  
                byte[] signature = signerCredentials.GenerateCertificateSignature(hash);
                int addon = 0;
                if (signatureAndHashAlgorithm != null)
            	    addon=2;
            
                IByteBuffer certificateVerifyBody = Unpooled.Buffer(DtlsHelper.HANDSHAKE_MESSAGE_HEADER_LENGTH + addon + 2 + signature.Length);
                currSequence=sequence++;
                DtlsHelper.WriteHandshakeHeader(currSequence,MessageType.CERTIFICATE_VERIFY,certificateVerifyBody,addon + 2 + signature.Length);
                if (signatureAndHashAlgorithm != null)
                {
            	    certificateVerifyBody.WriteByte(signatureAndHashAlgorithm.Hash);
            	    certificateVerifyBody.WriteByte(signatureAndHashAlgorithm.Signature);            	
                }
            
                certificateVerifyBody.WriteShort(signature.Length);
                certificateVerifyBody.WriteBytes(signature);            
                recordLayer.Send(currSequence,MessageType.CERTIFICATE_VERIFY, certificateVerifyBody);            
            }

            byte[] clientVerifyData = DtlsHelper.CalculateVerifyData(clientState.ClientContext, ExporterLabel.client_finished, DtlsHelper.GetCurrentPRFHash(clientState.ClientContext, clientState.HandshakeHash, null));

            IByteBuffer serverVerifyBuffer = Unpooled.Buffer(DtlsHelper.HANDSHAKE_MESSAGE_HEADER_LENGTH + clientVerifyData.Length);
            currSequence=sequence++;
            DtlsHelper.WriteHandshakeHeader(currSequence,MessageType.FINISHED,serverVerifyBuffer,clientVerifyData.Length);
            serverVerifyBuffer.WriteBytes(clientVerifyData);
            recordLayer.Send(currSequence,MessageType.FINISHED, serverVerifyBuffer);
        
            clientVerifyData = DtlsHelper.CalculateVerifyData(clientState.ClientContext, ExporterLabel.client_finished, DtlsHelper.GetCurrentPRFHash(clientState.ClientContext, clientState.HandshakeHash, null));        
	    }
	
	    public void PostProcessFinished()
        {
		    if(handshakeState==State.SERVER_HELLO_RECEIVED)
            {
                byte[] clientVerifyData = DtlsHelper.CalculateVerifyData(clientState.ClientContext, ExporterLabel.client_finished, DtlsHelper.GetCurrentPRFHash(clientState.ClientContext, clientState.HandshakeHash, null));
                IByteBuffer serverVerifyBuffer = Unpooled.Buffer(DtlsHelper.HANDSHAKE_MESSAGE_HEADER_LENGTH + clientVerifyData.Length);
                short currSequence = sequence++;
                DtlsHelper.WriteHandshakeHeader(currSequence, MessageType.FINISHED, serverVerifyBuffer, clientVerifyData.Length);
                serverVerifyBuffer.WriteBytes(clientVerifyData);
                recordLayer.Send(currSequence, MessageType.FINISHED, serverVerifyBuffer);
            }

            recordLayer.HandshakeSuccessful();
        
		    if(handshakeState==State.SERVER_HELLO_RECEIVED)
        	    clientState.ClientContext.ResumableSession = clientState.TlsSession;
            else
            {
                if (clientState.TlsSession != null)
                {
                    AsyncDtlsSecurityParameters parameters=(AsyncDtlsSecurityParameters)clientState.ClientContext.SecurityParameters;
                    clientState.SessionParameters = new SessionParameters.Builder()
                        .SetCipherSuite(parameters.CipherSuite)
                        .SetCompressionAlgorithm(parameters.CompressionAlgorithm)
                        .SetMasterSecret(parameters.MasterSecret)
                        .SetPeerCertificate(serverCertificate)
                        .SetPskIdentity(parameters.PskIdentity)
                        .SetSrpIdentity(parameters.SrpIdentity)
                        .SetServerExtensions(clientState.ServerExtensions)
                        .Build();

                    clientState.TlsSession = new AsyncDtlsSessionImpl(clientState.TlsSession.SessionID, clientState.SessionParameters);
                    clientState.ClientContext.ResumableSession = clientState.TlsSession;
                }
            }

            clientState.Client.NotifyHandshakeComplete();
        }

        public void PostProcessHelloVerifyRequest()
        {
            clientState.HandshakeHash.Reset();
            InitHandshake(((AsyncDtlsSecurityParameters)clientState.ClientContext.SecurityParameters).GetCookie());
        }

        public void HandleHandshake(MessageType messageType, IByteBuffer data)
        {
		    if(parentHandler!=null)
			    parentHandler.HandleHandshake(messageType, data);
		
		    switch(messageType)
            {
			    case MessageType.HELLO_VERIFY_REQUEST:
                    if (handshakeState != State.CLIENT_HELLO_SENT)
                        throw new TlsFatalAlert(AlertDescription.unexpected_message);

                    ProcessHelloVerifyRequest(data);
                    break;
			    case MessageType.SERVER_HELLO:
                    if (handshakeState != State.CLIENT_HELLO_SENT)
                        throw new TlsFatalAlert(AlertDescription.unexpected_message);

                    ProcessServerHello(data);
                    clientState.HandshakeHash = clientState.HandshakeHash.NotifyPrfDetermined();

                    short maxFragmentLength = ((AsyncDtlsSecurityParameters)clientState.ClientContext.SecurityParameters).GetMaxFragmentLength();
                    if (maxFragmentLength >= 0)
                    {
                        if (!MaxFragmentLength.IsValid((byte)maxFragmentLength))
                            throw new TlsFatalAlert(AlertDescription.internal_error);

                        int plainTextLimit = 1 << (8 + maxFragmentLength);
                        recordLayer.SetPlaintextLimit(plainTextLimit);
                    }

                    if (clientState.ResumedSession)
                    {
                        byte[] masterSecret = new byte[clientState.SessionParameters.MasterSecret.Length];
                        Array.Copy(clientState.SessionParameters.MasterSecret, 0, masterSecret, 0, masterSecret.Length);
                        ((AsyncDtlsSecurityParameters)clientState.ClientContext.SecurityParameters).SetMasterSecret(masterSecret);
                        recordLayer.InitPendingEpoch(clientState.Client.GetCipher());
                    }
                    else
                    {
                        if (clientState.SessionParameters != null)
                        {
                            clientState.SessionParameters.Clear();
                            clientState.SessionParameters=null;
                        }

                        if (clientState.TlsSession != null)
                        {
                            clientState.TlsSession.Invalidate();
                            clientState.TlsSession = null;
                        }

                        if (clientState.SelectedSessionID.Length > 0)
                            clientState.TlsSession = new AsyncDtlsSessionImpl(clientState.SelectedSessionID, null);
                    }
                    handshakeState = State.SERVER_HELLO_RECEIVED;
                    break;
			    case MessageType.SUPPLEMENTAL_DATA:
                    if (handshakeState != State.SERVER_HELLO_RECEIVED)
                        throw new TlsFatalAlert(AlertDescription.unexpected_message);

                    ProcessServerSupplementalData(data);
                    handshakeState = State.SUPP_DATA_RECEIVED;
                    break;
			    case MessageType.CERTIFICATE:
                    if (handshakeState == State.SERVER_HELLO_RECEIVED)
                    {
                        clientState.Client.ProcessServerSupplementalData(null);
                        handshakeState = State.SUPP_DATA_RECEIVED;
                    }

                    if (handshakeState != State.SUPP_DATA_RECEIVED)
                        throw new TlsFatalAlert(AlertDescription.unexpected_message);

                    clientState.KeyExchange = clientState.Client.GetKeyExchange();
                    clientState.KeyExchange.Init(clientState.ClientContext);

                    ProcessServerCertificate(data);
                    handshakeState = State.CERTIFICATE_RECEIVED;
                    break;
			    case MessageType.CERTIFICATE_STATUS:
                    if (handshakeState == State.SERVER_HELLO_RECEIVED)
                    {
                        clientState.Client.ProcessServerSupplementalData(null);
                        handshakeState = State.SUPP_DATA_RECEIVED;
                    }

                    if (handshakeState == State.SUPP_DATA_RECEIVED)
                    {
                        clientState.KeyExchange = clientState.Client.GetKeyExchange();
                        clientState.KeyExchange.Init(clientState.ClientContext);
                        clientState.KeyExchange.SkipServerCredentials();
                        handshakeState = State.CERTIFICATE_RECEIVED;
                    }

                    if (handshakeState != State.CERTIFICATE_RECEIVED)
                        throw new TlsFatalAlert(AlertDescription.unexpected_message);

                    ProcessCertificateStatus(data);
                    handshakeState = State.CERTIFICATE_STATUS_RECEIVED;
                    break;
			    case MessageType.SERVER_KEY_EXCHANGE:
                    if (handshakeState == State.SERVER_HELLO_RECEIVED)
                    {
                        clientState.Client.ProcessServerSupplementalData(null);
                        handshakeState = State.SUPP_DATA_RECEIVED;
                    }

                    if (handshakeState == State.SUPP_DATA_RECEIVED)
                    {
                        clientState.KeyExchange = clientState.Client.GetKeyExchange();
                        clientState.KeyExchange.Init(clientState.ClientContext);
                        clientState.KeyExchange.SkipServerCredentials();
                        handshakeState = State.CERTIFICATE_RECEIVED;
                    }

                    if (handshakeState == State.CERTIFICATE_RECEIVED)
                        handshakeState = State.CERTIFICATE_STATUS_RECEIVED;

                    if (handshakeState != State.CERTIFICATE_STATUS_RECEIVED)
                        throw new TlsFatalAlert(AlertDescription.unexpected_message);

                    ProcessServerKeyExchange(data);
                    handshakeState = State.SERVER_KEY_EXCHANGE_RECEIVED;
                    break;
			    case MessageType.CERTIFICATE_REQUEST:
                    if (handshakeState == State.SERVER_HELLO_RECEIVED)
                    {
                        clientState.Client.ProcessServerSupplementalData(null);
                        handshakeState = State.SUPP_DATA_RECEIVED;
                    }

                    if (handshakeState == State.SUPP_DATA_RECEIVED)
                    {
                        clientState.KeyExchange = clientState.Client.GetKeyExchange();
                        clientState.KeyExchange.Init(clientState.ClientContext);
                        clientState.KeyExchange.SkipServerCredentials();
                        handshakeState = State.CERTIFICATE_RECEIVED;
                    }

                    if (handshakeState == State.CERTIFICATE_RECEIVED)
                        handshakeState = State.CERTIFICATE_STATUS_RECEIVED;

                    if (handshakeState == State.CERTIFICATE_STATUS_RECEIVED)
                    {
                        clientState.KeyExchange.SkipServerKeyExchange();
                        handshakeState = State.SERVER_KEY_EXCHANGE_RECEIVED;
                    }

                    if (handshakeState != State.SERVER_KEY_EXCHANGE_RECEIVED)
                        throw new TlsFatalAlert(AlertDescription.unexpected_message);

                    ProcessCertificateRequest(data);
                    handshakeState = State.CERTIFICATE_REQUEST_RECEIVED;
                    break;
			    case MessageType.SERVER_HELLO_DONE:
                    if (handshakeState == State.SERVER_HELLO_RECEIVED)
                    {
                        clientState.Client.ProcessServerSupplementalData(null);
                        handshakeState = State.SUPP_DATA_RECEIVED;
                    }

                    if (handshakeState == State.SUPP_DATA_RECEIVED)
                    {
                        clientState.KeyExchange = clientState.Client.GetKeyExchange();
                        clientState.KeyExchange.Init(clientState.ClientContext);
                        clientState.KeyExchange.SkipServerCredentials();
                        handshakeState = State.CERTIFICATE_RECEIVED;
                    }

                    if (handshakeState == State.CERTIFICATE_RECEIVED)
                        handshakeState = State.CERTIFICATE_STATUS_RECEIVED;

                    if (handshakeState == State.CERTIFICATE_STATUS_RECEIVED)
                    {
                        clientState.KeyExchange.SkipServerKeyExchange();
                        handshakeState = State.SERVER_KEY_EXCHANGE_RECEIVED;
                    }

                    if (handshakeState == State.SERVER_KEY_EXCHANGE_RECEIVED)
                        handshakeState = State.CERTIFICATE_REQUEST_RECEIVED;

                    if (handshakeState != State.CERTIFICATE_REQUEST_RECEIVED)
                        throw new TlsFatalAlert(AlertDescription.unexpected_message);

                    ProcessServerHelloDone(data);
                    handshakeState = State.SERVER_HELLO_DONE;
                    break;
			    case MessageType.SESSION_TICKET:
                    if (handshakeState != State.FINISH_SENT || !clientState.ExpectSessionTicket)
                        throw new TlsFatalAlert(AlertDescription.unexpected_message);

                    ProcessNewSessionTicket(data);
                    handshakeState = State.SESSION_TICKET_RECEIVED;
                    break;
			    case MessageType.FINISHED:
                    if (handshakeState != State.FINISH_SENT && handshakeState != State.SESSION_TICKET_RECEIVED && handshakeState != State.SERVER_HELLO_RECEIVED)
                        throw new TlsFatalAlert(AlertDescription.unexpected_message);

                    if (handshakeState == State.FINISH_SENT && clientState.ExpectSessionTicket)
                        throw new TlsFatalAlert(AlertDescription.unexpected_message);

                    if (handshakeState == State.SERVER_HELLO_RECEIVED && clientState.ResumedSession)
                        throw new TlsFatalAlert(AlertDescription.unexpected_message);

                    ProcessFinished(data);
                    break;
                default:
				        throw new TlsFatalAlert(AlertDescription.unexpected_message);
            }
        }

        public void PostProcessHandshake(MessageType messageType, IByteBuffer data)
        {
		    //not throwing exception since already handled in handleHandshake
		    if(parentHandler!=null)
			    parentHandler.PostProcessHandshake(messageType, data);
		
		    switch(messageType)
            {
			    case MessageType.HELLO_VERIFY_REQUEST:
                    PostProcessHelloVerifyRequest();
                    break;
			    case MessageType.SERVER_HELLO_DONE:
                    PostProcessServerHelloDone();
                    handshakeState = State.FINISH_SENT;
                    break;
			    case MessageType.FINISHED:
                    PostProcessFinished();
                    handshakeState = State.ENDED;

                    if (handler != null)
                        handler.handshakeCompleted(channel);
                    break;
                default:
				    break;
            }
        }

        private void ProcessHelloVerifyRequest(IByteBuffer body)
        {
            ProtocolVersion recordLayerVersion = recordLayer.GetReadVersion();
            ProtocolVersion client_version = clientState.ClientContext.ClientVersion;

            if (!recordLayerVersion.IsEqualOrEarlierVersionOf(client_version))
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);

            recordLayer.SetReadVersion(null);

            ProtocolVersion server_version = ProtocolVersion.Get(body.ReadByte() & 0xFF, body.ReadByte() & 0xFF);
            byte[] cookie = new byte[body.ReadByte()];
            body.ReadBytes(cookie);
         
            if (!server_version.IsEqualOrEarlierVersionOf(clientState.ClientContext.ClientVersion))
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);
         
            if (!ProtocolVersion.DTLSv12.IsEqualOrEarlierVersionOf(server_version) && cookie.Length > 32)
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);

            ((AsyncDtlsSecurityParameters)clientState.ClientContext.SecurityParameters).SetCookie(cookie);
	    }

        private void ProcessServerHello(IByteBuffer body)
        {
            ProtocolVersion recordLayerVersion = recordLayer.GetReadVersion();
            ReportServerVersion(recordLayerVersion);
            recordLayer.SetWriteVersion(recordLayerVersion);

            AsyncDtlsSecurityParameters securityParameters = (AsyncDtlsSecurityParameters)clientState.ClientContext.SecurityParameters;

            ProtocolVersion server_version = ProtocolVersion.Get(body.ReadByte() & 0xFF, body.ReadByte() & 0xFF);
            ReportServerVersion(server_version);

            byte[] serverRandom = new byte[32];
            body.ReadBytes(serverRandom);
            securityParameters.SetServerRandom(serverRandom);

            byte[] selectedSessionID = new byte[body.ReadByte() & 0x0FF];
            if (selectedSessionID.Length > 0)
                body.ReadBytes(selectedSessionID);

            clientState.SelectedSessionID = selectedSessionID;
            if (selectedSessionID.Length > 32)
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);

            clientState.Client.NotifySessionID(selectedSessionID);
            clientState.ResumedSession = selectedSessionID.Length > 0 && clientState.TlsSession != null && ArrayUtils.Equals(clientState.SelectedSessionID, clientState.TlsSession.SessionID);

            int selectedCipherSuite = body.ReadUnsignedShort();
            Boolean inOfferedCipherSuites = false;
            for (int i = 0; i < clientState.OfferedCipherSuites.Length; i++)
            {
                if (selectedCipherSuite == clientState.OfferedCipherSuites[i])
                {
                    inOfferedCipherSuites = true;
                    break;
                }
            }

            if (!inOfferedCipherSuites || selectedCipherSuite == CipherSuite.TLS_NULL_WITH_NULL_NULL || CipherSuite.IsScsv(selectedCipherSuite) || !DtlsHelper.GetMinimumVersion(selectedCipherSuite).IsEqualOrEarlierVersionOf(clientState.ClientContext.ServerVersion.GetEquivalentTLSVersion()))
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);

            switch (DtlsHelper.GetEncryptionAlgorithm(selectedCipherSuite))
            {
                case EncryptionAlgorithm.RC4_40:
                case EncryptionAlgorithm.RC4_128:
                    throw new TlsFatalAlert(AlertDescription.internal_error);
            }

            clientState.Client.NotifySelectedCipherSuite(selectedCipherSuite);

            byte selectedCompressionMethod = body.ReadByte();
            Boolean inOfferedCompressionMethods = false;
            for (int i = 0; i < clientState.OfferedCompressionMethods.Length; i++)
            {
                if (selectedCompressionMethod == clientState.OfferedCompressionMethods[i])
                {
                    inOfferedCompressionMethods = true;
                    break;
                }
            }

            if (!inOfferedCompressionMethods)
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);

            clientState.Client.NotifySelectedCompressionMethod(selectedCompressionMethod);
            clientState.ServerExtensions = DtlsHelper.ReadSelectedExtensions(body);

            if (clientState.ServerExtensions != null)
            {
                foreach(Int32 extType in clientState.ServerExtensions.Keys)
                {
                    if (extType.Equals(DtlsHelper.EXT_RenegotiationInfo))
                        continue;

                    if (!clientState.ClientExtensions.Contains(extType))
                        throw new TlsFatalAlert(AlertDescription.unsupported_extension);
                }
            }

            byte[] renegExtData = null;
            if (clientState.ServerExtensions.Contains(DtlsHelper.EXT_RenegotiationInfo))
                renegExtData = (byte[])clientState.ServerExtensions[DtlsHelper.EXT_RenegotiationInfo];

            if (renegExtData != null)
            {
                clientState.SecureRenegotiation=true;

                if (!ArrayUtils.Equals(renegExtData, DtlsHelper.EMPTY_BYTES_WITH_LENGTH))
                    throw new TlsFatalAlert(AlertDescription.handshake_failure);
            }

            if (clientState.SecureRenegotiation)
                clientState.Client.NotifySecureRenegotiation(clientState.SecureRenegotiation);

            IDictionary sessionClientExtensions = clientState.ClientExtensions;
            IDictionary sessionServerExtensions = clientState.ServerExtensions;
            if (clientState.ResumedSession)
            {
                if (selectedCipherSuite != clientState.SessionParameters.CipherSuite || selectedCompressionMethod != clientState.SessionParameters.CompressionAlgorithm)
                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);

                sessionClientExtensions = null;
                sessionServerExtensions = (Dictionary < Int32, byte[]> )clientState.SessionParameters.ReadServerExtensions();
            }

            securityParameters.SetCipherSuite(selectedCipherSuite);
            securityParameters.SetCompressionAlgorithm(selectedCompressionMethod);

            if (sessionServerExtensions != null)
            {
                byte[] encryptThenMac = null;
                if(sessionServerExtensions.Contains(DtlsHelper.EXT_encrypt_then_mac))
                    encryptThenMac = (byte[])sessionServerExtensions[DtlsHelper.EXT_encrypt_then_mac];

                if (encryptThenMac != null && encryptThenMac.Length > 0)
                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);

                Boolean serverSentEncryptThenMAC = encryptThenMac != null;
                if (serverSentEncryptThenMAC && DtlsHelper.GetCipherType(securityParameters.CipherSuite) != CipherType.block)
                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);

                securityParameters.SetEncryptThenMAC(serverSentEncryptThenMAC);

                byte[] extendedMacSecret = null;
                if(sessionServerExtensions.Contains(DtlsHelper.EXT_extended_master_secret))
                    extendedMacSecret = (byte[])sessionServerExtensions[DtlsHelper.EXT_extended_master_secret];

                if (extendedMacSecret != null && extendedMacSecret.Length > 0)
                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);

                securityParameters.SetExtendedMasterSecret(extendedMacSecret != null);

                securityParameters.SetMaxFragmentLength(DtlsHelper.EvaluateMaxFragmentLengthExtension(clientState.ResumedSession, sessionClientExtensions, sessionServerExtensions, AlertDescription.illegal_parameter));

                byte[] truncatedHMAC = null;
                if(sessionServerExtensions.Contains(DtlsHelper.EXT_truncated_hmac))
                    truncatedHMAC = (byte[])sessionServerExtensions[DtlsHelper.EXT_truncated_hmac];

                if (truncatedHMAC != null && truncatedHMAC.Length > 0)
                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);

                securityParameters.SetTruncatedHMac(truncatedHMAC != null);

                byte[] statusRequest = null;
                if(sessionServerExtensions.Contains(DtlsHelper.EXT_status_request))
                    statusRequest = (byte[])sessionServerExtensions[DtlsHelper.EXT_status_request];

                if (statusRequest != null && statusRequest.Length > 0)
                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);

                clientState.AllowCertificateStatus = (!clientState.ResumedSession && statusRequest != null);

                byte[] sessionTicket = null;
                if(sessionServerExtensions.Contains(DtlsHelper.EXT_SessionTicket))
                    sessionTicket = (byte[])sessionServerExtensions[DtlsHelper.EXT_SessionTicket];

                if (sessionTicket != null && sessionTicket.Length > 0)
                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);

                clientState.ExpectSessionTicket = (!clientState.ResumedSession && sessionTicket != null);
            }

            if (sessionClientExtensions != null)
                clientState.Client.ProcessServerExtensions(sessionServerExtensions);

            securityParameters.SetPrfAlgorithm(DtlsHelper.GetPRFAlgorithm(clientState.ClientContext.ServerVersion, securityParameters.CipherSuite));
            securityParameters.SetVerifyDataLength(12);
        }


        private void ProcessServerSupplementalData(IByteBuffer body)
        {
            IList serverSupplementalData = DtlsHelper.ReadSupplementalData(body);
            clientState.Client.ProcessServerSupplementalData(serverSupplementalData);
        }

        private void ProcessServerCertificate(IByteBuffer data)
        {
            serverCertificate = DtlsHelper.ParseCertificate(data);
            clientState.KeyExchange.ProcessServerCertificate(serverCertificate);
            clientState.Authentication = clientState.Client.GetAuthentication();
            clientState.Authentication.NotifyServerCertificate(serverCertificate);
        }

        private void ProcessCertificateStatus(IByteBuffer data)
        {
            if (!clientState.AllowCertificateStatus)
                throw new TlsFatalAlert(AlertDescription.unexpected_message);

            clientState.CertificateStatus = DtlsHelper.ParseCertificateStatus(data);
    	}
	
	    private void ProcessServerKeyExchange(IByteBuffer body)
        {	
            //can not parse with byte buffer , needs input stream
            byte[] backedData=new byte[body.ReadableBytes];
            body.ReadBytes(backedData);
            MemoryStream buf = new MemoryStream(backedData);
            clientState.KeyExchange.ProcessServerKeyExchange(buf);        
	    }
	
	    private void ProcessCertificateRequest(IByteBuffer body)
        {
		    if (clientState.Authentication == null)
		    	throw new TlsFatalAlert(AlertDescription.handshake_failure);

            clientState.CertificateRequest = AsyncCertificateRequest.Parse(clientState.ClientContext.ServerVersion, body);
		
		    clientState.KeyExchange.ValidateCertificateRequest(clientState.CertificateRequest);
		    if (clientState.CertificateRequest.SupportedSignatureAlgorithms != null)
            {
                for (int i = 0; i<clientState.CertificateRequest.SupportedSignatureAlgorithms.Count; ++i)
                {
                    SignatureAndHashAlgorithm signatureAndHashAlgorithm = (SignatureAndHashAlgorithm)clientState.CertificateRequest.SupportedSignatureAlgorithms[i];
                    byte hashAlgorithm = signatureAndHashAlgorithm.Hash;
                    if (!HashAlgorithm.IsPrivate(hashAlgorithm))
                	    clientState.HandshakeHash.TrackHashAlgorithm(hashAlgorithm);                	                 
                }
            }
	    }
	
	    private void ProcessServerHelloDone(IByteBuffer body)
        {
		    if (body.ReadableBytes != 0)
                throw new TlsFatalAlert(AlertDescription.decode_error);

            clientState.HandshakeHash.SealHashAlgorithms();
	    }
	
	    private void ProcessNewSessionTicket(IByteBuffer body)
        {
		    long ticketLifetimeHint = body.ReadUnsignedInt();
            byte[] ticket = new byte[body.ReadUnsignedShort()];
            body.ReadBytes(body);
            NewSessionTicket newSessionTicket = new NewSessionTicket(ticketLifetimeHint, ticket);
            clientState.Client.NotifyNewSessionTicket(newSessionTicket);
	    }
	
	    private void ProcessFinished(IByteBuffer body)
        {
		    byte[] expectedClientVerifyData = DtlsHelper.CalculateVerifyData(clientState.ClientContext, ExporterLabel.server_finished,DtlsHelper.GetCurrentPRFHash(clientState.ClientContext, clientState.HandshakeHash, null));
            if(body.ReadableBytes!=expectedClientVerifyData.Length)
			    throw new TlsFatalAlert(AlertDescription.handshake_failure);

            byte[] serverVerifyData = new byte[body.ReadableBytes];
            body.ReadBytes(serverVerifyData);
		
		    if(!ArrayUtils.Equals(serverVerifyData, expectedClientVerifyData))
			    throw new TlsFatalAlert(AlertDescription.handshake_failure);		
	    }
	
	    private void ReportServerVersion(ProtocolVersion server_version)
        {
            ProtocolVersion currentServerVersion = clientState.ClientContext.ServerVersion;
		    if (null == currentServerVersion)
            {
                clientState.ClientContext.ServerVersion = server_version;
                clientState.Client.NotifyServerVersion(server_version);
            }
		    else if (!currentServerVersion.Equals(server_version))
			    throw new TlsFatalAlert(AlertDescription.illegal_parameter);		
	    }
    }
}