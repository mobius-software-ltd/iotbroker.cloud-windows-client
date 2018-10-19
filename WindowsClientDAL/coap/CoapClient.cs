using com.mobius.software.windows.iotbroker.network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mobius.software.windows.iotbroker.mqtt;
using com.mobius.software.windows.iotbroker.mqtt.avps;
using com.mobius.software.windows.iotbroker.mqtt.headers.api;
using com.mobius.software.windows.iotbroker.dal;
using System.Net;
using com.mobius.software.windows.iotbroker.coap.net;
using com.mobius.software.windows.iotbroker.coap.avps;

/**
* Mobius Software LTD
* Copyright 2015-2018, Mobius Software LTD
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

namespace com.mobius.software.windows.iotbroker.coap
{
    public class CoapClient : NetworkClient, ConnectionListener<CoapMessage>
    {
        public static Int32 VERSION = 1;
        private Int32 WORKER_THREADS = 4;
        private Int32 RESEND_PERIOND = 3000;

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

        public CoapClient(DBInterface _interface, DnsEndPoint address, String username, String password, String clientID, Boolean isClean, int keepalive, Will will, ClientListener listener)
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
            if (_client != null)
                _client.Shutdown();
        }

        public void CancelConnection()
        {
            _client.Shutdown();
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

        public void Connect()
        {
            SetState(ConnectionState.CONNECTION_ESTABLISHED);

            if (_timers != null)
                _timers.StopAllTimers();

            _timers = new TimersMap(this, _client, RESEND_PERIOND, _keepalive * 1000);

        }

        public Boolean Disconnect()
        {
            if (_client.IsConnected())
                _client.Close();
            
            SetState(ConnectionState.NONE);
            return true;
        }

        public void Publish(Topic topic, byte[] content, bool retain, bool dup)
        {
            List<CoapOption> options = new List<CoapOption>();
            byte[] nameBytes = Encoding.UTF8.GetBytes(topic.Name);
            options.Add(new CoapOption((int)CoapOptionType.URI_PATH, nameBytes.Length, nameBytes));
            byte[] nodeIdBytes = Encoding.UTF8.GetBytes(_clientID);
            options.Add(new CoapOption((int)CoapOptionType.NODE_ID, nodeIdBytes.Length, nodeIdBytes));

            byte[] qosValue=new byte[2];
            qosValue[0] = 0x00;
            switch (topic.Qos)
            {
                case QOS.AT_LEAST_ONCE:
                    qosValue[1] = 0x00;
                    break;
                case QOS.AT_MOST_ONCE:
                    qosValue[1] = 0x01;
                    break;
                case QOS.EXACTLY_ONCE:
                    qosValue[1] = 0x02;
                    break;
            }

            options.Add(new CoapOption((int)CoapOptionType.ACCEPT, 2,qosValue));
            CoapMessage coapMessage = new CoapMessage(VERSION, CoapType.CONFIRMABLE, CoapCode.PUT, 0, null, options, content);
            _timers.Store(coapMessage);
            //set message id = token id
            coapMessage.MessageID = Int32.Parse(Encoding.UTF8.GetString(coapMessage.Token));
            _client.Send(coapMessage);
        }

        public void Subscribe(Topic[] topics)
        {
            for (int i = 0; i < topics.Length; i++)
            {
                List<CoapOption> options = new List<CoapOption>();
                options.Add(new CoapOption((int)CoapOptionType.OBSERVE, 4, new byte[] { 0x00, 0x00, 0x00, 0x00 }));

                byte[] nameBytes = Encoding.UTF8.GetBytes(topics[i].Name);
                options.Add(new CoapOption((int)CoapOptionType.URI_PATH, nameBytes.Length, nameBytes));

                byte[] qosValue = new byte[2];
                qosValue[0] = 0x00;
                switch (topics[i].Qos)
                {
                    case QOS.AT_LEAST_ONCE:
                        qosValue[1] = 0x00;
                        break;
                    case QOS.AT_MOST_ONCE:
                        qosValue[1] = 0x01;
                        break;
                    case QOS.EXACTLY_ONCE:
                        qosValue[1] = 0x02;
                        break;
                }

                options.Add(new CoapOption((int)CoapOptionType.ACCEPT, 2, qosValue));
                byte[] nodeIdBytes = Encoding.UTF8.GetBytes(_clientID);
                options.Add(new CoapOption((int)CoapOptionType.NODE_ID, nodeIdBytes.Length, nodeIdBytes));
                CoapMessage coapMessage = new CoapMessage(VERSION, CoapType.CONFIRMABLE, CoapCode.GET, 0, null, options, new byte[0]);
                _timers.Store(coapMessage);
                //set message id = token id
                coapMessage.MessageID = Int32.Parse(Encoding.UTF8.GetString(coapMessage.Token));
                _client.Send(coapMessage);
            }
        }

        public void Unsubscribe(string[] topics)
        {
            List<CoapOption> options = new List<CoapOption>();
            options.Add(new CoapOption((int)CoapOptionType.OBSERVE, 4, new byte[] { 0x00, 0x00, 0x00, 0x01 }));
            for (int i = 0; i < topics.Length; i++)
            {
                byte[] nameBytes = Encoding.UTF8.GetBytes(topics[i]);
                options.Add(new CoapOption((int)CoapOptionType.URI_PATH, nameBytes.Length, nameBytes));
            }

            byte[] nodeIdBytes = Encoding.UTF8.GetBytes(_clientID);
            options.Add(new CoapOption((int)CoapOptionType.NODE_ID, nodeIdBytes.Length, nodeIdBytes));
            CoapMessage coapMessage = new CoapMessage(VERSION, CoapType.CONFIRMABLE, CoapCode.GET, 0, null, options, new byte[0]);
            _timers.Store(coapMessage);
            //set message id = token id
            coapMessage.MessageID = Int32.Parse(Encoding.UTF8.GetString(coapMessage.Token));
            _client.Send(coapMessage);
        }

        public void PacketReceived(CoapMessage message)
        {            
            CoapType type = message.CoapType;
            if (message.CoapCode == CoapCode.POST || message.CoapCode == CoapCode.PUT)
            {
                String topic = null;
                foreach (CoapOption option in message.Options)
                    if (option.Number == (int)CoapOptionType.URI_PATH)
                    {
                        topic = Encoding.UTF8.GetString(option.Value);
                        break;
                    }

                byte[] content = message.Payload;
                if (!_dbInterface.TopicExists(topic))
                {
                    List<CoapOption> options = new List<CoapOption>();
                    byte[] textBytes = Encoding.UTF8.GetBytes("text/plain");
                    options.Add(new CoapOption((int)CoapOptionType.CONTENT_FORMAT, textBytes.Length, textBytes));
                    byte[] nodeIdBytes = Encoding.UTF8.GetBytes(_clientID);
                    options.Add(new CoapOption((int)CoapOptionType.NODE_ID, nodeIdBytes.Length, nodeIdBytes));
                    CoapMessage ack = new CoapMessage(VERSION, CoapType.ACKNOWLEDGEMENT, CoapCode.BAD_OPTION, message.MessageID, message.Token, options, new byte[0]);
                    _client.Send(ack);
                }

                _dbInterface.StoreMessage(topic, content, 0);

                if (_listener != null)
                    _listener.MessageReceived(MessageType.PUBLISH);
            }

            switch (type)
            {
                case CoapType.CONFIRMABLE:
                    {
                        byte[] nodeIdBytes = Encoding.UTF8.GetBytes(_clientID);
                        message.Options.Add(new CoapOption((int)CoapOptionType.NODE_ID, nodeIdBytes.Length, nodeIdBytes));
                        CoapMessage ack = new CoapMessage(message.Version, CoapType.ACKNOWLEDGEMENT, message.CoapCode, message.MessageID, message.Token, message.Options, new byte[0]);
                        _client.Send(ack);
                    }
                    break;
                case CoapType.NON_CONFIRMABLE:
                    {
                        _timers.Remove(message.Token);
                    }
                    break;
                case CoapType.ACKNOWLEDGEMENT:
                    {
                        if (message.CoapCode == CoapCode.GET)
                        {
                            Boolean? observe = null;
                            QOS qos = QOS.AT_MOST_ONCE;
                            foreach (CoapOption option in message.Options)
                            {
                                if (option.Number == (int)CoapOptionType.OBSERVE && option.Value.Length>0)
                                {
                                    if (option.Value[option.Value.Length-1] == 0x00)
                                        observe = false;
                                    else
                                        observe = true;

                                    break;
                                }
                                else if (option.Number == (int)CoapOptionType.ACCEPT)
                                    qos = (QOS)option.Value[option.Value.Length - 1];
                            }

                            if (observe.HasValue)
                            {
                                if (!observe.Value)
                                {
                                    CoapMessage originalMessage = _timers.Remove(message.Token);
                                    if (originalMessage != null)
                                    {
                                        List<String> topics = new List<String>();
                                        foreach (CoapOption option in originalMessage.Options)
                                        {
                                            if (option.Number == (int)CoapOptionType.URI_PATH)
                                                topics.Add(Encoding.UTF8.GetString(option.Value));
                                        }

                                        for (int i = 0; i < topics.Count; i++)
                                            _dbInterface.StoreTopic(topics[i], qos);

                                        if (_listener != null)
                                            _listener.MessageReceived(MessageType.SUBACK);
                                    }
                                }
                                else
                                {
                                    CoapMessage originalMessage = _timers.Remove(message.Token);
                                    if (originalMessage != null)
                                    {
                                        List<String> topics = new List<String>();
                                        foreach (CoapOption option in originalMessage.Options)
                                        {
                                            if (option.Number == (int)CoapOptionType.URI_PATH)
                                                topics.Add(Encoding.UTF8.GetString(option.Value));
                                        }

                                        for (int i = 0; i < topics.Count; i++)
                                            _dbInterface.DeleteTopic(topics[i]);
                                    }

                                    if (_listener != null)
                                        _listener.MessageReceived(MessageType.UNSUBACK);
                                }
                            }
                        }
                        else
                        {
                            _timers.Remove(message.Token);
                            if (_listener != null)
                                _listener.MessageReceived(MessageType.PUBACK);
                        }
                    }
                    break;
                case CoapType.RESET:
                    {
                        _timers.Remove(message.Token);
                    }
                    break;
            }
        }

        public void ConnectionLost()
        {
            if (_isClean)
                clearAccountTopics();

            if (_timers != null)
                _timers.StopAllTimers();

            if (_client != null)
            {
                _client.Shutdown();
                SetState(ConnectionState.CONNECTION_LOST);
            }
        }

        public void Connected()
        {
            SetState(ConnectionState.CHANNEL_ESTABLISHED);
        }

        public void ConnectFailed()
        {
            SetState(ConnectionState.CHANNEL_FAILED);
        }

        private void clearAccountTopics()
        {
            _dbInterface.DeleteAllTopics();
        }

        public String ClientID
        {
            get
            {
                return _clientID;
            }
        }
    }
}
