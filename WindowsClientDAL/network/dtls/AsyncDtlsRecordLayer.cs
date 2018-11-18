using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Org.BouncyCastle.Crypto.Tls;
using System;
using System.Collections.Generic;
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
    public class AsyncDtlsRecordLayer
    {
        public static int RECORD_HEADER_LENGTH = 13;
        public static int MAX_FRAGMENT_LENGTH = 1400;
        public static long TCP_MSL = 1000L * 60 * 2;
        public static long RETRANSMIT_TIMEOUT = TCP_MSL * 2;

        private TlsPeer peer;
    
        private volatile Boolean closed = false;
        private volatile Boolean failed = false;
        private volatile ProtocolVersion readVersion = null, writeVersion = null;
        private volatile Boolean inHandshake;
        private volatile int plaintextLimit;
        private AsyncDtlsEpoch currentEpoch, pendingEpoch;
        private AsyncDtlsEpoch readEpoch, writeEpoch;

        private IChannel channel;
        
        private HandshakeHandler handshakeHandler;
        private TlsHandshakeHash handshakeHash;

        private Dictionary<Int16, PendingMessageData> pendingBuffers = new Dictionary<Int16, PendingMessageData>();

        public AsyncDtlsRecordLayer(TlsHandshakeHash handshakeHash, HandshakeHandler handshakeHandler, IChannel channel, TlsContext context, TlsPeer peer)
        {
            this.handshakeHash = handshakeHash;
            this.channel = channel;
            this.handshakeHandler = handshakeHandler;
            this.peer = peer;
            this.inHandshake = true;
            this.currentEpoch = new AsyncDtlsEpoch(0, new TlsNullCipher(context));
            this.pendingEpoch = null;
            this.readEpoch = currentEpoch;
            this.writeEpoch = currentEpoch;

            SetPlaintextLimit(MAX_FRAGMENT_LENGTH);
        }

        public void SetPlaintextLimit(int plaintextLimit)
        {
            this.plaintextLimit = plaintextLimit;
        }

        public int GetReadEpoch()
        {
            return readEpoch.Epoch;
        }

        public ProtocolVersion GetReadVersion()
        {
            return readVersion;
        }

        public void SetReadVersion(ProtocolVersion readVersion)
        {
            this.readVersion = readVersion;
        }

        public void SetWriteVersion(ProtocolVersion writeVersion)
        {
            this.writeVersion = writeVersion;
        }

        public void InitPendingEpoch(TlsCipher pendingCipher)
        {
            if (pendingEpoch != null)
                throw new InvalidOperationException();

            this.pendingEpoch = new AsyncDtlsEpoch(writeEpoch.Epoch + 1, pendingCipher);
        }

        public void HandshakeSuccessful()
        {
            if (readEpoch == currentEpoch || writeEpoch == currentEpoch)
                throw new InvalidOperationException();

            this.inHandshake = false;
            this.currentEpoch = pendingEpoch;
            this.pendingEpoch = null;
        }

        public int GetReceiveLimit()
        {
            return this.plaintextLimit;
        }

        public int GetSendLimit()
        {
            return this.plaintextLimit - RECORD_HEADER_LENGTH;
        }

        public List<IByteBuffer> Receive(IByteBuffer record)
        {
            List<IByteBuffer> outputList=new List<IByteBuffer>();
    	    while(record.ReadableBytes>RECORD_HEADER_LENGTH)
    	    {
    		    byte type = (byte)(record.ReadByte() & 0x00FF);
                ProtocolVersion version = ProtocolVersion.Get(record.ReadByte() & 0xFF, record.ReadByte() & 0xFF);
                int epoch = record.ReadUnsignedShort();
                long seq = DtlsHelper.ReadUint48(record);
                //just reading length,not using it
                short packetLength = record.ReadShort();
                byte[] realData = new byte[packetLength];
                record.ReadBytes(realData);
	        
	            AsyncDtlsEpoch recordEpoch = null;
	            if (epoch == readEpoch.Epoch)
	                recordEpoch = readEpoch;

	            if (recordEpoch == null)
	                continue;
	
	            if (recordEpoch.ReplayWindow.ShouldDiscard(seq))
	        	    continue;
	
	            if (!version.IsDtls)
	        	    continue;
	
	            if (readVersion != null && !readVersion.Equals(version))
	        	    continue;
	        
	            byte[] plaintext = recordEpoch.getCipher().DecodeCiphertext(GetMacSequenceNumber(recordEpoch.Epoch, seq), type, realData, 0, realData.Length);
                IByteBuffer output = Unpooled.WrappedBuffer(plaintext);

                recordEpoch.ReplayWindow.ReportAuthenticated(seq);
	            if (plaintext.Length > this.plaintextLimit)
	        	    continue;
	
	            if (readVersion == null)
	                readVersion = version;
	        
	            switch (type)
	            {
		            case ContentType.alert:
		                if (output.ReadableBytes == 2)
		                {
		                    byte alertLevel = (byte)(output.ReadByte() & 0x0FF);
                            byte alertDescription = (byte)(output.ReadByte() & 0x0FF);

                            peer.NotifyAlertReceived(alertLevel, alertDescription);
		
		                    if (alertLevel == AlertLevel.fatal)
		                    {
                                Failed();
		                        throw new TlsFatalAlert(alertDescription);
                            }
		
		                    if (alertDescription == AlertDescription.close_notify)
                                CloseTransport();
                        }
		
		                continue;
		            case ContentType.application_data:
		                if (inHandshake)
		            	    continue;
		                break;
		            case ContentType.change_cipher_spec:
		        	    while(output.ReadableBytes>0)
		                {
		            	
		                    short message = (short)(output.ReadByte() & 0x0FF);
		                    if (message != ChangeCipherSpec.change_cipher_spec)
		                	    continue;
		            
		                    if (pendingEpoch != null)
		                        readEpoch = pendingEpoch;	                
		                }
		
		        	    continue;
		            case ContentType.handshake:
		                if (!inHandshake)
		                    continue;
		            
		                HandshakeHeader handshakeHeader = DtlsHelper.ReadHandshakeHeader(output);
		            
		                if(handshakeHeader!=null)
		                {
		            	    if(!handshakeHeader.FragmentLength.Equals(handshakeHeader.TotalLength))
		            	    {
                                PendingMessageData data = null;
                                    if (pendingBuffers.ContainsKey(handshakeHeader.MessageSequence))
                                        data = pendingBuffers[handshakeHeader.MessageSequence];

		            		    if(data==null)
		            		    {
		            			    data=new PendingMessageData(Unpooled.Buffer(handshakeHeader.TotalLength));
		            			    pendingBuffers[handshakeHeader.MessageSequence]=data;
		            		    }
		            			
		            		    data.WriteBytes(output, handshakeHeader.FragmentOffset);
		            		    if(data.WrottenBytes.Equals(handshakeHeader.TotalLength))
		            		    {
		            			    data.Buffer.SetWriterIndex(handshakeHeader.TotalLength);
		            			    byte[] packetData = null;
                                    IByteBuffer copy = data.Buffer.Copy();
                                    packetData=new byte[copy.ReadableBytes];
		            			    copy.ReadBytes(packetData);	
		            			
		            			    if(handshakeHeader.MessageType.HasValue && handshakeHandler!=null)
		            				    handshakeHandler.HandleHandshake(handshakeHeader.MessageType.Value, data.Buffer);
		            			
		            			    byte[] pseudoHeader = new byte[DtlsHelper.HANDSHAKE_MESSAGE_HEADER_LENGTH];
                                    IByteBuffer headerBuffer = Unpooled.WrappedBuffer(pseudoHeader);
                                    headerBuffer.SetWriterIndex(0);    
	            				    DtlsHelper.WriteHandshakeHeader(handshakeHeader.MessageSequence, handshakeHeader.MessageType.Value, headerBuffer, handshakeHeader.TotalLength);
	            				    headerBuffer.SetReaderIndex(0);
	            				    handshakeHash.BlockUpdate(pseudoHeader, 0, pseudoHeader.Length); 
		            			    handshakeHash.BlockUpdate(packetData, 0, packetData.Length);
		            			
		            			    if(handshakeHeader.MessageType.HasValue && handshakeHandler!=null)
		            				    handshakeHandler.PostProcessHandshake(handshakeHeader.MessageType.Value, data.Buffer);
		            			
		            			    pendingBuffers.Remove(handshakeHeader.MessageSequence);
		            		    }		            				            		
		            	    }
		            	    else
		            	    {
		            		    byte[] packetData = null;
                                IByteBuffer copy = output.Copy();
                                packetData=new byte[copy.ReadableBytes];
	            			    copy.ReadBytes(packetData);
	            			
	            			    if(handshakeHeader.MessageType.HasValue && handshakeHandler!=null)
	            				    handshakeHandler.HandleHandshake(handshakeHeader.MessageType.Value, output);
	            			
	            			    byte[] pseudoHeader = new byte[DtlsHelper.HANDSHAKE_MESSAGE_HEADER_LENGTH];
                                IByteBuffer headerBuffer = Unpooled.WrappedBuffer(pseudoHeader);
                                headerBuffer.SetWriterIndex(0);
            				    DtlsHelper.WriteHandshakeHeader(handshakeHeader.MessageSequence, handshakeHeader.MessageType.Value, headerBuffer, handshakeHeader.TotalLength);
            				    headerBuffer.SetReaderIndex(0);
            				    handshakeHash.BlockUpdate(pseudoHeader, 0, pseudoHeader.Length); 
	            			    handshakeHash.BlockUpdate(packetData, 0, packetData.Length); 
	            			
	            			    if(handshakeHeader.MessageType.HasValue && handshakeHandler!=null)
	            				    handshakeHandler.PostProcessHandshake(handshakeHeader.MessageType.Value, output);	            		
		            	    }
		                }
		            
		                continue;
		            case ContentType.heartbeat:
		        	    continue;	       
	            }
	        
	            outputList.Add(output);
    	    }
    	
            return outputList;
        }
    
        public void SendAlert(byte alertLevel, byte alertDescription, String message, Exception cause)
        {
    	    if(closed)
    		    return;

            peer.NotifyAlertRaised(alertLevel, alertDescription, message, cause);
            IByteBuffer buf=Unpooled.Buffer(2);
            buf.WriteByte(alertLevel);
            buf.WriteByte(alertDescription);
            SendRecord(ContentType.alert, buf);
        }

        public void Send(IByteBuffer buffer)
        {
    	    if(closed)
    		    return;
    	
    	    if(this.inHandshake)
    		    return;

            SendRecord(ContentType.application_data,buffer);
        }

        public void Send(short sequence, MessageType messageType, IByteBuffer buffer)
        {
    	    if(closed)
    		    return;
    	
    	    if (messageType == MessageType.FINISHED)
            {
                AsyncDtlsEpoch nextEpoch = null;
                if (this.inHandshake)
                    nextEpoch = pendingEpoch;

                if (nextEpoch == null)
                    throw new InvalidOperationException();

                IByteBuffer cipherSpecBuf = Unpooled.Buffer(1);
                cipherSpecBuf.WriteByte(1);
                SendRecord(ContentType.change_cipher_spec, cipherSpecBuf);

                writeEpoch = nextEpoch;
            }

            IByteBuffer copy=buffer.Copy();
    	    byte[] realArray=new byte[copy.ReadableBytes];
    	    copy.ReadBytes(realArray);

    	    if(buffer.ReadableBytes<=GetSendLimit())
                SendRecord(ContentType.handshake, buffer);
    	    else
    	    {
    		    int fragmentOffset = 0;
                int totalLength = buffer.ReadableBytes - DtlsHelper.HANDSHAKE_MESSAGE_HEADER_LENGTH;

                IByteBuffer header = buffer.ReadBytes(DtlsHelper.HANDSHAKE_MESSAGE_HEADER_LENGTH);
                header.Release();
			
			    do
    	        {
				    int fragmentLength = Math.Min(buffer.ReadableBytes + DtlsHelper.HANDSHAKE_MESSAGE_HEADER_LENGTH, GetSendLimit());
                    IByteBuffer current = Unpooled.Buffer(fragmentLength);
                    DtlsHelper.WriteHandshakeHeader(fragmentOffset, fragmentLength-DtlsHelper.HANDSHAKE_MESSAGE_HEADER_LENGTH, sequence, messageType, current, totalLength);
    			    buffer.ReadBytes(current, fragmentLength-DtlsHelper.HANDSHAKE_MESSAGE_HEADER_LENGTH);

                    SendRecord(ContentType.handshake, current);
                    fragmentOffset+=fragmentLength-DtlsHelper.HANDSHAKE_MESSAGE_HEADER_LENGTH;     			
    	        }
    		    while (buffer.ReadableBytes>0);    	            			    	    
    	    }
    	
    	    handshakeHash.BlockUpdate(realArray, 0, DtlsHelper.HANDSHAKE_MESSAGE_HEADER_LENGTH);
    	    handshakeHash.BlockUpdate(realArray, DtlsHelper.HANDSHAKE_MESSAGE_HEADER_LENGTH, realArray.Length-DtlsHelper.HANDSHAKE_MESSAGE_HEADER_LENGTH);        
        }
    
        public void Close()
        {
    	    if (!closed)
            {
                if (inHandshake)
                    Warn(AlertDescription.user_canceled, "User canceled handshake");

                CloseTransport();
            }
        }

        void Fail(byte alertDescription)
        {
            if (!closed)
            {
                try
                {
                    RaiseAlert(AlertLevel.fatal, alertDescription, null, null);
                }
                catch (Exception)
                {
                    // Ignore
                }

                failed = true;

                CloseTransport();
            }
        }

        void Failed()
        {
            if (!closed)
            {
                failed = true;
                CloseTransport();
            }
        }

        void Warn(byte alertDescription, String message)
        {
            RaiseAlert(AlertLevel.warning, alertDescription, message, null);
        }

        private void CloseTransport()
        {
            if (!closed)
            {
                try
                {
                    if (!failed)
                        Warn(AlertDescription.close_notify, null);

                    channel.CloseAsync();
                }
                catch (Exception)
                {
                }

                closed = true;
            }
        }

        private void RaiseAlert(byte alertLevel, byte alertDescription, String message, Exception cause)
        {
            peer.NotifyAlertRaised(alertLevel, alertDescription, message, cause);
            IByteBuffer error = Unpooled.Buffer(2);
            error.WriteByte(alertLevel);
            error.WriteByte(alertDescription);

            SendRecord(ContentType.alert, error);
        }

        private void SendRecord(byte contentType, IByteBuffer buf)
        {
    	    if (writeVersion == null)
    		    return;
            
    	    int length=buf.ReadableBytes;
    	    if (length > this.plaintextLimit)
    		    throw new TlsFatalAlert(AlertDescription.internal_error);
        
    	    if (length< 1 && contentType != ContentType.application_data)
    		    throw new TlsFatalAlert(AlertDescription.internal_error);

            int recordEpoch = writeEpoch.Epoch;
            long recordSequenceNumber = writeEpoch.allocateSequenceNumber();

            byte[] plainData = new byte[length];
            buf.ReadBytes(plainData);
            byte[] ciphertext = writeEpoch.getCipher().EncodePlaintext(GetMacSequenceNumber(recordEpoch, recordSequenceNumber), contentType, plainData, 0, length);
            IByteBuffer buffer = Unpooled.Buffer(RECORD_HEADER_LENGTH + ciphertext.Length);
            buffer.WriteByte(contentType);
            buffer.WriteByte(writeVersion.MajorVersion);
            buffer.WriteByte(writeVersion.MinorVersion);
            buffer.WriteShort(recordEpoch);
            DtlsHelper.WriteUint48(recordSequenceNumber, buffer);
        
            buffer.WriteShort(ciphertext.Length);
            buffer.WriteBytes(ciphertext);
            channel.WriteAndFlushAsync(new DatagramPacket(buffer, channel.RemoteAddress));
        }

        private static long GetMacSequenceNumber(int epoch, long sequence_number)
        {
            return ((epoch & 0xFFFFFFFFL) << 48) | sequence_number;
        }
    }
}
