

using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.headeramqp;
using com.mobius.software.windows.iotbroker.amqp.headerapi;
using com.mobius.software.windows.iotbroker.amqp.net;
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
using com.mobius.software.windows.iotbroker.network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.amqp
{
    public class TimersMap:TimersMapInterface<AMQPHeader>
    {
        #region private fields

        private static Int32 MAX_VALUE = 65535;
        private static Int32 MIN_VALUE = 1;

        private TCPClient _listener;
        private Int64 _resendPeriod;
        private Int64 _keepalivePeriod;
        private AmqpClient _client;

        private Dictionary<Int32,MessageResendTimer<AMQPHeader>> _timersMap = new Dictionary<Int32,MessageResendTimer<AMQPHeader>>();
        private Int32 _packetIDCounter = MIN_VALUE;

        private MessageResendTimer<AMQPHeader> _pingTimer;
        private ServerConnectTimer<AMQPHeader> _connectTimer;

        private void ExecuteTimer(MessageResendTimer<AMQPHeader> timer)
        {

        }

        #endregion

        #region constructors

        public TimersMap(AmqpClient client, TCPClient listener, Int64 resendPeriod,Int64 keepalivePeriod)
        {
            this._listener = listener;
            this._resendPeriod = resendPeriod;
            this._keepalivePeriod = keepalivePeriod;
            this._client = client;
        }

        #endregion

        #region public fields
        public void Store(AMQPHeader message)
        {
            if(message.Code.Value == HeaderCodes.TRANSFER)
            {
                MessageResendTimer<AMQPHeader> timer = new MessageResendTimer<AMQPHeader>(message, _listener, this, false);
                Boolean added = false;
                if (!((AMQPTransfer)message).DeliveryId.HasValue)
                {
                    Int32 packetID = _packetIDCounter;
                    while (!added)
                    {
                        packetID = Interlocked.Increment(ref _packetIDCounter) % MAX_VALUE;
                        try
                        {
                            _timersMap.Add(packetID, timer);
                            added = true;
                        }
                        catch (ArgumentException)
                        {
                            //already exists
                        }
                    }


                    AMQPTransfer countable = (AMQPTransfer)message;
                    countable.DeliveryId = packetID;
                }
                else
                    _timersMap.Add((int)((AMQPTransfer)message).DeliveryId.Value, timer);

                timer.Execute(_resendPeriod);
            }
        }

        public void Store(Int32 packetID,AMQPHeader message)
        {
            Boolean isConnect = false;
            if (message.Code.Value == HeaderCodes.OPEN)
                isConnect = true;

            MessageResendTimer<AMQPHeader> timer = new MessageResendTimer<AMQPHeader>(message, _listener, this, isConnect);
            _timersMap.Add(packetID,timer);
            timer.Execute(_resendPeriod);
        }

        public void RefreshTimer(MessageResendTimer<AMQPHeader> timer)
        {            
            switch (timer.Message.Code.Value)
            {
                case HeaderCodes.PING:
                    timer.Execute(_keepalivePeriod);                    
                    break;
                default:
                    timer.Execute(_resendPeriod);
                    break;        
            }
        }

        public AMQPHeader Remove(Int32 packetID)
        {
            MessageResendTimer<AMQPHeader> timer =_timersMap[packetID];
            _timersMap.Remove(packetID);
            if (timer != null)
            {
                timer.Stop();
                return timer.Message;
            }

            return null;
        }

        public void StopAllTimers()
        {
            if (_connectTimer != null)
                _connectTimer.Stop();

            if (_pingTimer != null)
                _pingTimer.Stop();

            KeyValuePair<Int32, MessageResendTimer<AMQPHeader>>[] list = _timersMap.ToArray();
            for (int i = 0; i < list.Length; i++)
                list[i].Value.Stop();

            _timersMap.Clear();
        }

        public ServerConnectTimer<AMQPHeader> ConnectTimer
        {
            get
            {
                return _connectTimer;
            }
        }

        public void StoreConnectTimer()
        {
            if (_connectTimer != null)
                _connectTimer.Stop();

            _connectTimer = new ServerConnectTimer<AMQPHeader>(this);
            _connectTimer.Execute(_resendPeriod);
        }

        public void StopConnectTimer()
        {
            if (_connectTimer != null)
                _connectTimer.Stop();            
        }

        public void CancelConnectTimer()
        {
            if (_connectTimer != null)
            { 
                _connectTimer.Stop();
                _client.CancelConnection();
            }
        }

        public void StartPingTimer()
        {
            if (_pingTimer != null)
                _pingTimer.Stop();

            _pingTimer = new MessageResendTimer<AMQPHeader>(new AMQPPing(),_listener, this, false);
            _pingTimer.Execute(_keepalivePeriod);
        }
        #endregion
    }
}
