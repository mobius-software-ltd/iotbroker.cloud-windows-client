

using com.mobius.software.windows.iotbroker.coap.avps;
using com.mobius.software.windows.iotbroker.coap.net;
using com.mobius.software.windows.iotbroker.network;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.coap
{
    public class TimersMap: TimersMapInterface<CoapMessage>
    {
        #region private fields

        private static Int32 MAX_VALUE = 65535;
        private static Int32 MIN_VALUE = 1;

        private UDPClient _listener;
        private Int64 _resendPeriod;
        private Int64 _keepalivePeriod;
        private CoapClient _client;

        private Dictionary<byte[],MessageResendTimer<CoapMessage>> _timersMap = new Dictionary<byte[], MessageResendTimer<CoapMessage>>();
        private Int32 _packetIDCounter = MIN_VALUE;

        private MessageResendTimer<CoapMessage> _pingTimer;
        private MessageResendTimer<CoapMessage> _connectTimer;

        private void ExecuteTimer(MessageResendTimer<CoapMessage> timer)
        {

        }

        #endregion

        #region constructors

        public TimersMap(CoapClient client, UDPClient listener, Int64 resendPeriod,Int64 keepalivePeriod)
        {
            this._listener = listener;
            this._resendPeriod = resendPeriod;
            this._keepalivePeriod = keepalivePeriod;
            this._client = client;
        }

        #endregion

        #region public fields
        public void Store(CoapMessage message)
        {
            MessageResendTimer<CoapMessage> timer = new MessageResendTimer<CoapMessage>(message,_listener, this, false);
            Boolean added = false;
            if (message.Token==null)
            {
                Int32 packetID = _packetIDCounter;
                byte[] token=BitConverter.GetBytes(packetID);
                while (!added)
                {
                    packetID = Interlocked.Increment(ref _packetIDCounter) % MAX_VALUE;
                    token = Encoding.UTF8.GetBytes(packetID.ToString());
                    try
                    {                        
                        _timersMap.Add(token, timer);
                        added = true;
                    }
                    catch (ArgumentException)
                    {
                        //already exists
                    }
                }


                message.Token = token;
            }
            else
                _timersMap.Add(message.Token, timer);

            timer.Execute(_resendPeriod);
        }

        public void Store(byte[] token, CoapMessage message)
        {
            MessageResendTimer<CoapMessage> timer = new MessageResendTimer<CoapMessage>(message, _listener, this, false);
            _timersMap.Add(token, timer);
            timer.Execute(_resendPeriod);
        }

        public void RefreshTimer(MessageResendTimer<CoapMessage> timer)
        {
            if(timer.Message.Token!=null)
                timer.Execute(_keepalivePeriod);
            else
                timer.Execute(_resendPeriod);                
        }

        public CoapMessage Remove(byte[] token)
        {
            MessageResendTimer<CoapMessage> timer =_timersMap[token];
            _timersMap.Remove(token);
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

            KeyValuePair<byte[], MessageResendTimer<CoapMessage>>[] list = _timersMap.ToArray();
            for (int i = 0; i < list.Length; i++)
                list[i].Value.Stop();

            _timersMap.Clear();
        }

        public MessageResendTimer<CoapMessage> ConnectTimer
        {
            get
            {
                return _connectTimer;
            }
        }

        public void StoreConnectTimer(CoapMessage message)
        {
            if (_connectTimer != null)
                _connectTimer.Stop();

            _connectTimer = new MessageResendTimer<CoapMessage>(message,_listener, this, true);
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

            _pingTimer = new MessageResendTimer<CoapMessage>(getPingreqMessage(),_listener, this, false);
            _pingTimer.Execute(_keepalivePeriod);
        }

        private CoapMessage getPingreqMessage()
        {
            List<CoapOption> options = new List<CoapOption>();
            byte[] token = BitConverter.GetBytes(0);
            byte[] nodeIdBytes = Encoding.UTF8.GetBytes(_client.ClientID);
            options.Add(new CoapOption((int)CoapOptionType.NODE_ID, nodeIdBytes.Length, nodeIdBytes));
            CoapMessage coapMessage = new CoapMessage(CoapClient.VERSION, CoapType.CONFIRMABLE, CoapCode.PUT, 0, token, options, new byte[0]);
            return coapMessage;
        }

        #endregion
    }
}
