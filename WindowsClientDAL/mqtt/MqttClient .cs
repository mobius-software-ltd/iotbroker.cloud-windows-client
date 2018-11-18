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
using com.mobius.software.windows.iotbroker.mqtt.headers.api;
using com.mobius.software.windows.iotbroker.mqtt.headers.impl;
using com.mobius.software.windows.iotbroker.mqtt.net;
using com.mobius.software.windows.iotbroker.network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.mqtt
{
    public class MqttClient: ConnectionListener<MQMessage>,MQDevice,NetworkClient
    {
        public static String MESSAGETYPE_PARAM = "MESSAGETYPE";
        private Int32 RESEND_PERIOND = 3000;
        private Int32 WORKER_THREADS = 4;
        
        private DnsEndPoint _address;
        private ConnectionState _connectionState;

        private TimersMap _timers;
        private NetworkChannel<MQMessage> _client;
        private String _username;
        private String _password;
        private String _clientID;
        private Boolean _isClean;
        private Int32 _keepalive;
        private Will _will;
        private Boolean isSecured;
        private String certificate;
        private String certificatePassword;

        private ClientListener _listener;
        private DBInterface _dbInterface;
        private Boolean _isWs;

        public MqttClient(DBInterface _interface, DnsEndPoint address, Boolean isWS, String username,String password, String clientID, Boolean isClean, int keepalive,Will will, Boolean isSecured,String certificate,String certificatePassword,ClientListener listener)
        {

            this._dbInterface = _interface;
            this._address = address;
            this._isWs = isWS;
            this._username = username;
            this._password = password;
            this._clientID = clientID;
            this._isClean = isClean;
            this._keepalive = keepalive;
            this._will = will;
            this.isSecured = isSecured;
            this.certificate = certificate;
            this.certificatePassword = certificatePassword;

            this._listener = listener;
            if(this._isWs)
                _client = new WSClient(address, isSecured, certificate, certificatePassword, WORKER_THREADS);
            else
                _client = new TCPClient(address, isSecured, certificate, certificatePassword, WORKER_THREADS);
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
            Connect connect = new Connect(_username, _password, _clientID, _isClean,_keepalive, _will);

            if (_timers != null)
                _timers.StopAllTimers();

            _timers = new TimersMap(this, _client, RESEND_PERIOND, _keepalive * 1000);
            _timers.StoreConnectTimer(connect);

            if (_client.IsConnected())
                _client.Send(connect);            
        }

        public Boolean Disconnect()
        {
            if (_client.IsConnected())
            {
                _client.Send(new Disconnect());
                _client.Close();
            }

            SetState(ConnectionState.NONE);
            return true;
        }

        public void Subscribe(Topic[] topics)
        {
            Subscribe subscribe = new Subscribe(null, topics);
            _timers.Store(subscribe);
            _client.Send(subscribe);
        }

        public void Unsubscribe(String[] topics)
        {
            Unsubscribe uunsubscribe = new Unsubscribe(null, topics);
            _timers.Store(uunsubscribe);
            _client.Send(uunsubscribe);
        }

        public void Publish(Topic topic, byte[] content, Boolean retain, Boolean dup)
        {
            Publish publish = new Publish(null, topic, content, retain, dup);
            if (topic.Qos != QOS.AT_MOST_ONCE)
                _timers.Store(publish);

            _client.Send(publish);
        }

        public void Reinit()
        {
            SetState(ConnectionState.CHANNEL_CREATING);

            if (_client != null)
                _client.Shutdown();

            if (this._isWs)
                _client = new WSClient(_address, isSecured, certificate, certificatePassword, WORKER_THREADS);
            else
                _client = new TCPClient(_address, isSecured, certificate, certificatePassword, WORKER_THREADS);            
        }

        public void CloseConnection()
        {
            if (_timers != null)
                _timers.StopAllTimers();

            if (_client != null)
            {
                NetworkChannel<MQMessage> currClient = _client;
                _client = null;
                currClient.Shutdown();
            }
        }

        public void PacketReceived(MQMessage message)
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

        public void ProcessConnack(ConnackCode code, Boolean sessionPresent)
        {
            // CANCEL CONNECT TIMER
            MessageResendTimer<MQMessage> timer = _timers.ConnectTimer;
            _timers.StopConnectTimer();

            // CHECK CODE , IF OK THEN MOVE TO CONNECTED AND NOTIFY NETWORK SESSION
            if (code == ConnackCode.ACCEPTED)
            {
                SetState(ConnectionState.CONNECTION_ESTABLISHED);

                if (timer != null)
                {
                    Connect connect = (Connect)timer.Message;
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

        public void ProcessSuback(Int32 packetID, List<SubackCode> codes)
        {

            MQMessage message = _timers.Remove(packetID);
            foreach (SubackCode code in codes)
            {
                if (code == SubackCode.FAILURE)
                    throw new CoreLogicException("received invalid message suback");
                else
                {
                    Subscribe subscribe = (Subscribe)message;
                    Topic topic = subscribe.Topics[0];
                    QOS expectedQos = topic.Qos;
                    QOS actualQos = (QOS)((int)code);
                    if (expectedQos == actualQos)
                        _dbInterface.StoreTopic(topic.Name, expectedQos);                    
                    else
                        _dbInterface.StoreTopic(topic.Name, actualQos);

                    if (_listener != null)
                        _listener.MessageReceived(MessageType.SUBACK);
                }
            }
        }       

        public void ProcessUnsuback(Int32 packetID)
        {
            MQMessage message = _timers.Remove(packetID);
            if (message != null)
            {
                Unsubscribe unsubscribe = (Unsubscribe)message;
                String[] topics = unsubscribe.Topics;
                foreach (String topic in topics)
                    _dbInterface.DeleteTopic(topic);
            }

            if (_listener != null)
                _listener.MessageReceived(MessageType.UNSUBACK);            
        }

        public void ProcessPublish(Int32? packetID, Topic topic, byte[] content,Boolean retain, Boolean isDup)
        {
            QOS publisherQos = topic.Qos;
            switch (publisherQos)
            {
                case QOS.AT_LEAST_ONCE:
                    Puback puback = new Puback(packetID.Value);
                    _client.Send(puback);
                    break;
                case QOS.EXACTLY_ONCE:
                    Pubrec pubrec = new Pubrec(packetID.Value);
                    _client.Send(pubrec);
                    break;
                default:
                    break;
            }

            String topicName = topic.Name;
            
            if (!(isDup && publisherQos == QOS.EXACTLY_ONCE))
                _dbInterface.StoreMessage(topicName, content, publisherQos);                

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
            MQMessage message = new Pubrel(packetID);
            _timers.Store(message);
            _client.Send(message);
        }

        public void ProcessPubrel(Int32 packetID)
        {
            _client.Send(new Pubcomp(packetID));
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

        public void ProcessSubscribe(Int32 packetID, Topic[] topics)
        {
            throw new CoreLogicException("received invalid message subscribe");
        }

        public void ProcessConnect(Boolean cleanSession, int keepalive, Will will)
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

        public void ProcessUnsubscribe(Int32 packetID, String[] topics)
        {
            throw new CoreLogicException("received invalid message unsubscribe");
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