

using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.headeramqp;
using com.mobius.software.windows.iotbroker.amqp.headerapi;
using com.mobius.software.windows.iotbroker.amqp.headersasl;
using com.mobius.software.windows.iotbroker.amqp.net;
using com.mobius.software.windows.iotbroker.amqp.sections;
using com.mobius.software.windows.iotbroker.amqp.terminus;
using com.mobius.software.windows.iotbroker.amqp.tlv.described;
using com.mobius.software.windows.iotbroker.amqp.wrappers;
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
using com.mobius.software.windows.iotbroker.dal;
using com.mobius.software.windows.iotbroker.mqtt.avps;
using com.mobius.software.windows.iotbroker.network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.amqp
{
    public class AmqpClient: ConnectionListener<AMQPHeader>,AMQPDevice,NetworkClient
    {
        public static String MESSAGETYPE_PARAM = "MESSAGETYPE";
        private Int32 RESEND_PERIOND = 3000;
        private Int32 WORKER_THREADS = 4;
        
        private EndPoint _address;
        private ConnectionState _connectionState;

        private TimersMap _timers;
        private TCPClient _client;
        private String _username;
        private String _password;
        private String _clientID;
        private Boolean _isClean;
        private Int32 _keepalive;
        private Will _will;
        private ClientListener _listener;
        private DBInterface _dbInterface;

        private Boolean _isSASLСonfirm = false;
        private int _channel;
        private Int64 _nextHandle = 0;
        private Dictionary<String, Int64> _usedIncomingMappings = new Dictionary<String, Int64>();
        private Dictionary<String, Int64> _usedOutgoingMappings = new Dictionary<String, Int64>();
        private Dictionary<Int64, String> _usedMappings = new Dictionary<Int64, String>();
        private List<AMQPTransfer> pendingMessages = new List<AMQPTransfer>();

        public AmqpClient(DBInterface _interface,EndPoint address, String username,String password, String clientID, Boolean isClean, int keepalive,Will will, ClientListener listener)
        {

            this._dbInterface = _interface;
            this._address = address;
            this._username = username;
            this._password = password;
            this._clientID = clientID;
            this._isClean = isClean;
            this._keepalive = keepalive;
            this._will = will;
            this._listener = listener;
            _client = new TCPClient(address, WORKER_THREADS);
        }

        public void SetListener(ClientListener listener)
        {
            this._listener = listener;
        }

        public void SetState(ConnectionState state)
        {
            this._connectionState = state;
            if (this._listener != null)
                _listener.StateChanged(state);
        }

        public Boolean createChannel()
        {
            SetState(ConnectionState.CHANNEL_CREATING);
            Boolean isSuccess = _client.Init(this);
            if (!isSuccess)
                SetState(ConnectionState.CHANNEL_FAILED);            
            else
                _timers.StoreConnectTimer();
            
            return isSuccess;
        }
        
        public Boolean isConnected()
        {
            return _connectionState == ConnectionState.CONNECTION_ESTABLISHED;
        }

        public ConnectionState GetConnectionState()
        {
            return _connectionState;
        }

        public EndPoint GetEndpoint()
        {
            return _address;
        }

        public void CloseChannel()
        {
            if(_client!=null)
                _client.Shutdown();
        }

        public void Connect()
        {
            SetState(ConnectionState.CONNECTING);

            if (_timers != null)
                _timers.StopAllTimers();

            AMQPProtoHeader header = new AMQPProtoHeader(3);
            _client.Send(header);
        }

        public void Disconnect()
        {
            if (_client.IsConnected())
            {
                AMQPEnd end = new AMQPEnd();
                end.Channel = _channel;
                _client.Send(end);

                if (_timers != null)
                    _timers.StopAllTimers();

                _timers = null;
            }

            SetState(ConnectionState.NONE);
            return;
        }

        public void Subscribe(Topic[] topics)
        {
            for(int i=0;i<topics.Length;i++)
            {
                Int64 currentHandler;
                if (_usedIncomingMappings.ContainsKey(topics[i].Name))
                    currentHandler = _usedIncomingMappings[topics[i].Name];
                else
                {
                    currentHandler = _nextHandle++;
                    _usedIncomingMappings[topics[i].Name] = currentHandler;
                    _usedMappings[currentHandler] = topics[i].Name;
                }

                AMQPAttach attach = new AMQPAttach();
                attach.Channel = _channel;
                attach.Name = topics[i].Name;
                attach.Handle = currentHandler;
                attach.Role = RoleCodes.RECEIVER;
                attach.SndSettleMode = SendCodes.MIXED;
                AMQPTarget target = new AMQPTarget();
                target.Address = topics[i].Name;
                target.Durable = TerminusDurability.NONE;
                target.Timeout = 0;
                target.Dynamic = false;
                attach.Target = target;
                _client.Send(attach);
            }
        }

        public void Unsubscribe(String[] topics)
        {
            foreach (String topic in topics)
            {
                if (_usedIncomingMappings.ContainsKey(topic))
                {
                    AMQPDetach detach = new AMQPDetach();
                    detach.Channel = _channel;
                    detach.Closed = true;
                    detach.Handle = _usedIncomingMappings[topic];
                    _client.Send(detach);
                }
                else
                {
                    _dbInterface.DeleteTopic(topic);
                    if (_listener != null)
                        _listener.MessageReceived(MessageType.UNSUBACK);
                }
            }
        }

        public void Publish(Topic topic, byte[] content, Boolean retain, Boolean dup)
        {
            AMQPTransfer transfer = new AMQPTransfer();
            transfer.Channel = _channel;
            transfer.DeliveryId = 0;
            transfer.Settled = false;
            transfer.More = false;
            transfer.MessageFormat = new AMQPMessageFormat(0);

            MessageHeader messageHeader = new MessageHeader();
            messageHeader.Durable = true;
            messageHeader.Priority = 3;
            messageHeader.Milliseconds = 1000;

            AMQPData data = new AMQPData();
            data.Data = content;

            AMQPSection[] sections = new AMQPSection[1];
            sections[0] = data;
            transfer.addSections(sections);

            if (_usedOutgoingMappings.ContainsKey(topic.Name))
            {
                Int64 handle = _usedOutgoingMappings[topic.Name];
                transfer.Handle = handle;
                _timers.Store(transfer);
                _client.Send(transfer);
            }
            else
            {
                Int64 currentHandler = _nextHandle++;
                _usedOutgoingMappings[topic.Name] = currentHandler;
                _usedMappings[currentHandler] = topic.Name;

                transfer.Handle = currentHandler;
                pendingMessages.Add(transfer);

                AMQPAttach attach = new AMQPAttach();
                attach.Channel = _channel;
                attach.Name = topic.Name;
                attach.Handle = currentHandler;
                attach.Role = RoleCodes.SENDER;
                attach.SndSettleMode = SendCodes.MIXED;
                AMQPSource source = new AMQPSource();
                source.Address = topic.Name;
                source.Durable = TerminusDurability.NONE;
                source.Timeout = 0;
                source.Dynamic = false;
                attach.Source = source;
                _client.Send(attach);
            }
        }

        public void Reinit()
        {
            SetState(ConnectionState.CHANNEL_CREATING);

            if (_client != null)
                _client.Shutdown();

            _client = new TCPClient(_address, WORKER_THREADS);            
        }

        public void CloseConnection()
        {
            if (_timers != null)
                _timers.StopAllTimers();

            if (_client != null)
            {
                TCPClient currClient = _client;
                _client = null;
                currClient.Shutdown();
            }
        }

        public void PacketReceived(AMQPHeader message)
        {
            //try
            //{
            message.ProcessBy(this);
            //}
            //catch (Exception)
            //{
            //_client.Shutdown();
            //}
        }

        public void CancelConnection()
        {
            _client.Shutdown();            
        }

        public void ConnectionLost()
        {
            if (_isClean)
                clearAccountTopics();

            if(_timers!=null)
                _timers.StopAllTimers();

            if (_client != null)
            {
                _client.Shutdown();
                SetState(ConnectionState.CONNECTION_LOST);
            }
        }

        private void clearAccountTopics()
        {
            _dbInterface.DeleteAllTopics();
        }
        
        public void Connected()
        {
            SetState(ConnectionState.CHANNEL_ESTABLISHED);           
        }

        public void ConnectFailed()
        {
            SetState(ConnectionState.CHANNEL_FAILED);
        }

        public void ProcessProto(int channel, int protocolId)
        {
            if (_isSASLСonfirm && protocolId == 0)
            {
                _channel = channel;
                AMQPOpen open = new AMQPOpen();
                open.ContainerId = Guid.NewGuid().ToString();
                open.Channel = _channel;
                _client.Send(open);
            }
            else
            {
                _timers.StopAllTimers();
                _client.Shutdown();
                SetState(ConnectionState.CONNECTION_FAILED);
            }
        }

        public void ProcessOpen(long idleTimeout)
        {
            _timers = new TimersMap(this, _client, RESEND_PERIOND, idleTimeout*1000);
            _timers.StartPingTimer();
            
            AMQPBegin begin = new AMQPBegin();
            begin.Channel = _channel;
            begin.NextOutgoingId =0;
            begin.IncomingWindow = 2147483647;
            begin.OutgoingWindow = 0;
            _client.Send(begin);
        }

        public void ProcessBegin()
        {
            SetState(ConnectionState.CONNECTION_ESTABLISHED);

            if (_isClean)
            {
                clearAccountTopics();
            }            
        }

        public void ProcessAttach(RoleCodes? role,Int64? handle)
        {
            if (role.HasValue)
            {
                if (role.Value == RoleCodes.SENDER)
                {
                    //publish
                    if (handle.HasValue)
                    {
                        for (int i = 0; i < pendingMessages.Count; i++)
                        {
                            AMQPTransfer currMessage = pendingMessages[i];
                            if (currMessage.Handle == handle.Value)
                            {
                                pendingMessages.RemoveAt(i);
                                i--;

                                _timers.Store(currMessage);
                                _client.Send(currMessage);
                            }
                        }
                    }
                }
                else
                {
                    //subscribe
                }
            }
        }

        public void ProcessFlow(int channel)
        {
            //not implemented for now
        }

        public void ProcessTransfer(AMQPData data, long? handle,bool? settled, long? _deliveryId)
        {
            QOS qos=QOS.AT_LEAST_ONCE;
            if (settled.HasValue && settled.Value)
                qos = QOS.AT_MOST_ONCE;
            else
            {
                AMQPDisposition disposition = new AMQPDisposition();
                disposition.Channel = _channel;
                disposition.Role = RoleCodes.RECEIVER;
                disposition.First = _deliveryId.Value;
                disposition.Last = _deliveryId.Value;
                disposition.Settled = true;
                disposition.State = new AMQPAccepted();
                _client.Send(disposition);
            }

            String topicName=null;
            if (!handle.HasValue || !_usedMappings.ContainsKey(handle.Value))
                return;

            topicName  = _usedMappings[handle.Value];
            if (!_dbInterface.TopicExists(topicName))
                return;

            _dbInterface.StoreMessage(topicName, data.Data, qos);

            if (_listener != null)
                _listener.MessageReceived(MessageType.PUBLISH);
        }

        public void ProcessDisposition(long? first, long? last)
        {
            if(first.HasValue && last.HasValue)
            {
                for (Int64 i = first.Value; i < last.Value; i++)
                    _timers.Remove((Int32)i);                
            }
        }

        public void ProcessDetach(int channel,Int64? handle)
        {
            if (handle.HasValue && _usedMappings.ContainsKey(handle.Value))
            {
                String topicName=_usedMappings[handle.Value];
                _usedMappings.Remove(handle.Value);
                if (_usedOutgoingMappings.ContainsKey(topicName))
                    _usedOutgoingMappings.Remove(topicName);
            }
        }

        public void ProcessEnd(int channel)
        {
            AMQPClose close = new AMQPClose();
            close.Channel = _channel;
            _client.Send(close);
        }

        public void ProcessClose()
        {
            if (_client.IsConnected())
                _client.Close();
            
            SetState(ConnectionState.NONE);
            return;
        }

        public void ProcessSASLInit(string _mechanism, byte[] _initialResponse, string _hostName)
        {
            throw new CoreLogicException("received invalid message init");
        }

        public void ProcessSASLChallenge(byte[] challenge)
        {
            throw new CoreLogicException("received invalid message challenge");
        }

        public void ProcessSASLMechanism(List<AMQPSymbol> _mechanisms, int channel, int headerType)
        {
            AMQPSymbol plainMechanism = null;
            foreach (AMQPSymbol mechanism in _mechanisms)
            {
                if (mechanism.Value.ToLower() == "plain")
                {
                    plainMechanism = mechanism;
                    break;
                }
            }

            //currently supporting only plain
            if (plainMechanism==null)
            {
                _timers.StopAllTimers();
                _client.Shutdown();
                SetState(ConnectionState.CONNECTION_FAILED);
                return;
            }

            SASLInit saslInit = new SASLInit();
            saslInit.HeaderType = headerType;
            saslInit.Channel = channel;
            saslInit.Mechanism = plainMechanism.Value;

            byte[] userBytes = Encoding.UTF8.GetBytes(_username);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(_password);
            byte[] challenge = new byte[userBytes.Length + 1 + userBytes.Length + 1 + passwordBytes.Length];
            Array.Copy(userBytes, 0, challenge, 0, userBytes.Length);
            challenge[userBytes.Length] = 0x00;
            Array.Copy(userBytes, 0, challenge, userBytes.Length + 1, userBytes.Length);
            challenge[userBytes.Length + 1 + userBytes.Length] = 0x00;
            Array.Copy(passwordBytes, 0, challenge, userBytes.Length + 1 + userBytes.Length + 1, passwordBytes.Length);            

            saslInit.InitialResponse = challenge;
            _client.Send(saslInit);
        }

        public void ProcessSASLOutcome(byte[] additionalData, OutcomeCodes? outcomeCode)
        {
            if (outcomeCode.HasValue)
                if(outcomeCode.Value == OutcomeCodes.OK)
                {
                    _isSASLСonfirm = true;
                    AMQPProtoHeader header = new AMQPProtoHeader(0);
                    _client.Send(header);
                }
            }

        public void ProcessSASLResponse(byte[] response)
        {
            throw new CoreLogicException("received invalid message response");
        }

        public void ProcessPing()
        {
           //nothing to be done here
        }
    }
}