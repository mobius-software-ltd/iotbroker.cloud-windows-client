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
using com.mobius.software.windows.iotbroker.mqtt_sn.avps;
using com.mobius.software.windows.iotbroker.mqtt_sn.headers.api;
using com.mobius.software.windows.iotbroker.mqtt_sn.net;
using com.mobius.software.windows.iotbroker.mqtt_sn.packet.impl;
using com.mobius.software.windows.iotbroker.network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.mqtt_sn
{
    public class SNClient : ConnectionListener<SNMessage>,SNDevice,NetworkClient
    {
        public static String MESSAGETYPE_PARAM = "MESSAGETYPE";
        private Int32 RESEND_PERIOND = 3000;
        private Int32 WORKER_THREADS = 4;
        
        private DnsEndPoint _address;
        private ConnectionState _connectionState;

        private TimersMap _timers;
        private UDPClient _client;
        private String _username;
        private String _password;
        private String _clientID;
        private Boolean _isClean;
        private Int32 _keepalive;
        private Will _will;
        private ClientListener _listener;
        private DBInterface _dbInterface;

        private Dictionary<Int32, String> mappedTopics = new Dictionary<Int32, String>();
        private Dictionary<String, Int32> reverseMappedTopics = new Dictionary<String, Int32>();
        private List<SNPublish> pendingMessages = new List<SNPublish>();

        public SNClient(DBInterface _interface, DnsEndPoint address, String username,String password, String clientID, Boolean isClean, int keepalive,Will will, ClientListener listener)
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
            _client = new UDPClient(address, WORKER_THREADS);
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

        public DnsEndPoint GetEndpoint()
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
            Boolean willPresent = false;
            if (_will != null)
                willPresent = true;

            SNConnect connect = new SNConnect(_isClean, _keepalive, _clientID, willPresent);

            if (_timers != null)
                _timers.StopAllTimers();

            _timers = new TimersMap(this, _client, RESEND_PERIOND, _keepalive * 1000);
            _timers.StoreConnectTimer(connect);

            if (_client.IsConnected())
                _client.Send(connect);            
        }

        public void Disconnect()
        {
            if (_client.IsConnected())
            {
                _client.Send(new SNDisconnect());
                _client.Close();
            }

            SetState(ConnectionState.NONE);
            return;
        }

        public void Subscribe(Topic[] topics)
        {
            for(int i=0;i<topics.Length;i++)
            {
                SNQoS realQos = SNQoS.AT_LEAST_ONCE;
                switch (topics[i].Qos)
                {
                    case QOS.AT_LEAST_ONCE:
                        realQos = SNQoS.AT_LEAST_ONCE;
                        break;
                    case QOS.AT_MOST_ONCE:
                        realQos = SNQoS.AT_MOST_ONCE;
                        break;
                    case QOS.EXACTLY_ONCE:
                        realQos = SNQoS.EXACTLY_ONCE;
                        break;
                }

                SNTopic topic;
                if (reverseMappedTopics.ContainsKey(topics[i].Name))
                    topic = new IdentifierTopic(reverseMappedTopics[topics[i].Name], realQos);
                else
                    topic = new FullTopic(topics[i].Name, realQos);

                SNSubscribe subscribe = new SNSubscribe(null, topic, false);
                _timers.Store(subscribe);
                _client.Send(subscribe);
            }            
        }

        public void Unsubscribe(String[] topics)
        {
            for (int i = 0; i < topics.Length; i++)
            {
                SNQoS realQos = SNQoS.AT_LEAST_ONCE;
                SNTopic topic;
                if (reverseMappedTopics.ContainsKey(topics[i]))
                    topic = new IdentifierTopic(reverseMappedTopics[topics[i]], realQos);
                else
                    topic = new FullTopic(topics[i], realQos);

                SNUnsubscribe subscribe = new SNUnsubscribe(null, topic);
                _timers.Store(subscribe);
                _client.Send(subscribe);
            }
        }

        public void Publish(Topic topic, byte[] content, Boolean retain, Boolean dup)
        {
            SNQoS realQos = SNQoS.AT_LEAST_ONCE;
            switch (topic.Qos)
            {
                case QOS.AT_LEAST_ONCE:
                    realQos = SNQoS.AT_LEAST_ONCE;
                    break;
                case QOS.AT_MOST_ONCE:
                    realQos = SNQoS.AT_MOST_ONCE;
                    break;
                case QOS.EXACTLY_ONCE:
                    realQos = SNQoS.EXACTLY_ONCE;
                    break;
            }

            if (reverseMappedTopics.ContainsKey(topic.Name))
            {
                IdentifierTopic idTopic = new IdentifierTopic(reverseMappedTopics[topic.Name], realQos);
                SNPublish publish = new SNPublish(null, idTopic, content, dup, retain);
                if (topic.Qos != QOS.AT_MOST_ONCE)
                    _timers.Store(publish);

                _client.Send(publish);
            }
            else
            {
                FullTopic fullTopic = new FullTopic(topic.Name, realQos);
                pendingMessages.Add(new SNPublish(null, fullTopic, content, dup, retain));
                Register register = new Register(0, null, topic.Name);
                _timers.Store(register);
                _client.Send(register);
            }            
        }

        public void Reinit()
        {
            SetState(ConnectionState.CHANNEL_CREATING);

            if (_client != null)
                _client.Shutdown();

            _client = new UDPClient(_address, WORKER_THREADS);            
        }

        public void CloseConnection()
        {
            if (_timers != null)
                _timers.StopAllTimers();

            if (_client != null)
            {
                UDPClient currClient = _client;
                _client = null;
                currClient.Shutdown();
            }
        }

        public void PacketReceived(SNMessage message)
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

        public void ProcessWillTopicRequest()
        {
            Boolean retain = false;
            String topicName = String.Empty;
            SNQoS topicQos = SNQoS.EXACTLY_ONCE;
            if(_will!=null && _will.Topic!=null)
            {
                retain = _will.Retain;
                if (_will.Topic.Name != null)
                    topicName = _will.Topic.Name;

                switch (_will.Topic.Qos)
                {
                    case QOS.AT_LEAST_ONCE:
                        topicQos = SNQoS.AT_LEAST_ONCE;
                        break;
                    case QOS.AT_MOST_ONCE:
                        topicQos = SNQoS.AT_MOST_ONCE;
                        break;
                    default:
                        break;
                }
            }

            FullTopic topic = new FullTopic(_will.Topic.Name, topicQos);
            WillTopic willTopic = new WillTopic(retain,topic);
            _client.Send(willTopic);
        }

        public void ProcessWillMessageRequest()
        {
            byte[] content = new byte[0];
            if(_will!=null && _will.Content!=null)
                content=_will.Content;

            WillMsg willMessage = new WillMsg(content);
            _client.Send(willMessage);
        }

        public void ProcessConnack(ReturnCode code)
        {
            // CANCEL CONNECT TIMER
            MessageResendTimer<SNMessage> timer = _timers.ConnectTimer;
            _timers.StopConnectTimer();

            // CHECK CODE , IF OK THEN MOVE TO CONNECTED AND NOTIFY NETWORK SESSION
            if (code == ReturnCode.ACCEPTED)
            {
                SetState(ConnectionState.CONNECTION_ESTABLISHED);

                if (timer != null)
                {
                    SNConnect connect = (SNConnect)timer.Message;
                    if (connect.CleanSession)
                        clearAccountTopics();     
                }

                _timers.StartPingTimer();
            }
            else
            {
                _timers.StopAllTimers();
                _client.Shutdown();
                SetState(ConnectionState.CONNECTION_FAILED);
            }
        }

        private void clearAccountTopics()
        {
            _dbInterface.DeleteAllTopics();
        }

        public void ProcessSuback(Int32 packetID, Int32 topicID, ReturnCode returnCode, SNQoS allowedQos)
        {

            SNMessage message = _timers.Remove(packetID);
            if (returnCode != ReturnCode.ACCEPTED)
                throw new CoreLogicException("received invalid message suback");
            else
            {
                SNSubscribe subscribe = (SNSubscribe)message;
                String topicName = String.Empty;
                if (subscribe.SnTopic is IdentifierTopic)
                    topicName = mappedTopics[((IdentifierTopic)subscribe.SnTopic).Value];
                else
                    topicName = ((FullTopic)subscribe.SnTopic).Value;

                SNQoS actualQos = allowedQos;
                QOS realQos = QOS.AT_LEAST_ONCE;
                switch (actualQos)
                {
                    case SNQoS.AT_MOST_ONCE:
                        realQos = QOS.AT_MOST_ONCE;
                        break;
                    case SNQoS.EXACTLY_ONCE:
                        realQos = QOS.EXACTLY_ONCE;
                        break;
                    case SNQoS.LEVEL_ONE:
                        realQos = QOS.AT_MOST_ONCE;
                        break;
                }

                _dbInterface.StoreTopic(topicName, realQos);

                if (_listener != null)
                    _listener.MessageReceived(MessageType.SUBACK);
            }
        }       

        public void ProcessUnsuback(Int32 packetID)
        {
            SNMessage message = _timers.Remove(packetID);
            if (message != null)
            {
                SNUnsubscribe unsubscribe = (SNUnsubscribe)message;
                String topicName = String.Empty;
                if (unsubscribe.SnTopic is IdentifierTopic)
                    topicName = mappedTopics[((IdentifierTopic)unsubscribe.SnTopic).Value];
                else
                    topicName = ((FullTopic)unsubscribe.SnTopic).Value;

                _dbInterface.DeleteTopic(topicName);
            }

            if (_listener != null)
                _listener.MessageReceived(MessageType.UNSUBACK);            
        }

        public void ProcessRegister(Int32 packetID, Int32 topicID, String topicName)
        {
            if (mappedTopics.ContainsKey(topicID))
                mappedTopics[topicID] = topicName;
            else
                mappedTopics.Add(topicID, topicName);

            if (reverseMappedTopics.ContainsKey(topicName))
                reverseMappedTopics[topicName] = topicID;
            else
                reverseMappedTopics.Add(topicName, topicID);

            SNMessage message = new Regack(topicID, packetID, ReturnCode.ACCEPTED);
            _client.Send(message);
        }

        public void ProcessRegack(Int32 packetID,Int32 topicID, ReturnCode returnCode)
        {
            SNMessage message=_timers.Remove(packetID);
            if (message != null)
            {
                Register register = (Register)message;
                if (returnCode == ReturnCode.ACCEPTED)
                {
                    if (mappedTopics.ContainsKey(topicID))
                        mappedTopics[topicID] = register.TopicName;
                    else
                        mappedTopics.Add(topicID, register.TopicName);

                    if (reverseMappedTopics.ContainsKey(register.TopicName))
                        reverseMappedTopics[register.TopicName] = topicID;
                    else
                        reverseMappedTopics.Add(register.TopicName, topicID);
                }

                for(int i=0;i<pendingMessages.Count;i++)                    
                {
                    SNPublish currMessage = pendingMessages[i];
                    if (((FullTopic)currMessage.SnTopic).Value == register.TopicName)
                    {
                        pendingMessages.RemoveAt(i);
                        i--;
                        currMessage.SnTopic = new IdentifierTopic(topicID, ((FullTopic)currMessage.SnTopic).Qos);
                        _timers.Store(currMessage);
                        _client.Send(currMessage);
                    }
                }
            }
        }

        public void ProcessPublish(Int32? packetID, SNTopic topic, Byte[] content, Boolean retain, Boolean isDup)
        {
            SNQoS publisherQos = topic.getQos();
            QOS realQos = QOS.AT_MOST_ONCE;
            switch (publisherQos)
            {
                case SNQoS.AT_LEAST_ONCE:
                    SNPuback puback = new SNPuback();
                    puback.MessageID = packetID;
                    puback.ReturnCode = ReturnCode.ACCEPTED;
                    int topicID = 0;
                    if (topic is IdentifierTopic)
					    topicID = ((IdentifierTopic)topic).Value;
				    else if (topic is ShortTopic)
					    topicID = Int32.Parse(((ShortTopic)topic).Value);

                    puback.topicID = topicID;
                    _client.Send(puback);
                    realQos = QOS.AT_LEAST_ONCE;
                    break;
                case SNQoS.EXACTLY_ONCE:
                    realQos = QOS.EXACTLY_ONCE;
                    SNPubrec pubrec = new SNPubrec(packetID.Value);
                    _client.Send(pubrec);
                    break;
                default:
                    break;
            }

            String topicName = String.Empty;
            if(topic is IdentifierTopic && mappedTopics.ContainsKey(((IdentifierTopic)topic).Value))
                topicName = mappedTopics[((IdentifierTopic)topic).Value];

            if (topicName.Length==0 || !_dbInterface.TopicExists(topicName))
                return;

            if (!(isDup && publisherQos == SNQoS.EXACTLY_ONCE))
                _dbInterface.StoreMessage(topicName, content, realQos);                

            if (_listener != null)
                _listener.MessageReceived(MessageType.PUBLISH);            
        }

        public void ProcessPuback(Int32 packetID)
        {
            _timers.Remove(packetID);
            if (_listener != null)
                _listener.MessageReceived(MessageType.PUBACK);
        }

        public void ProcessPubrec(Int32 packetID)
        {
            _timers.Remove(packetID);
            if (_listener != null)
                _listener.MessageReceived(MessageType.PUBREC);

            SNMessage message = new SNPubrel(packetID);
            _timers.Store(message);
            _client.Send(message);
        }

        public void ProcessPubrel(Int32 packetID)
        {
            _client.Send(new SNPubcomp(packetID));
        }

        public void ProcessPubcomp(Int32 packetID)
        {
            _timers.Remove(packetID);
            if (_listener != null)
                _listener.MessageReceived(MessageType.PUBCOMP);            
        }

        public void ProcessPingresp()
        {
            //DO NOTHING
        }

        public void ProcessSubscribe(Int32 packetID, SNTopic topics)
        {
            throw new CoreLogicException("received invalid message subscribe");
        }

        public void ProcessConnect(Boolean cleanSession, Int32 keepalive)
        {
            throw new CoreLogicException("received invalid message connect");
        }

        public void ProcessPingreq()
        {
            throw new CoreLogicException("received invalid message pingreq");
        }

        public void ProcessDisconnect()
        {
            CloseConnection();
            SetState(ConnectionState.CONNECTION_LOST);
        }

        public void ProcessUnsubscribe(Int32 packetID, SNTopic topics)
        {
            throw new CoreLogicException("received invalid message unsubscribe");
        }

        public void ProcessWillTopicUpdate(FullTopic willTopic)
        {
            throw new CoreLogicException("received invalid message will topic update");
        }

        public void ProcessWillMessageUpdate(byte[] _content)
        {
            throw new CoreLogicException("received invalid message will message update");
        }

        public void ProcessWillTopic(FullTopic topic)
        {
            throw new CoreLogicException("received invalid message will topic");
        }

        public void ProcessWillMessage(byte[] _content)
        {
            throw new CoreLogicException("received invalid message will message");
        }

        public void ProcessAdvertise(Int32 gatewayID, Int32 duration)
        {
            throw new CoreLogicException("received invalid message advertise");
        }

        public void ProcessGwInfo(Int32 gatewayID, String gatewayAddress)
        {
            throw new CoreLogicException("received invalid message gw info");
        }

        public void ProcessSearchGw(Radius radius)
        {
            throw new CoreLogicException("received invalid message search gw");
        }

        public void ProcessWillTopicResponse()
        {
            throw new CoreLogicException("received invalid message will topic response");
        }

        public void ProcessWillMessageResponse()
        {
            throw new CoreLogicException("received invalid message will message response");
        }

        public void Connected()
        {
            SetState(ConnectionState.CHANNEL_ESTABLISHED);           
        }

        public void ConnectFailed()
        {
            SetState(ConnectionState.CHANNEL_FAILED);
        }
    }
}