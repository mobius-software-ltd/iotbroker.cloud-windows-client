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

using com.mobius.software.windows.iotbroker.mqtt_sn.packet.api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mobius.software.windows.iotbroker.mqtt_sn.avps;
using com.mobius.software.windows.iotbroker.mqtt.exceptions;
using com.mobius.software.windows.iotbroker.mqtt_sn.headers.api;

namespace com.mobius.software.windows.iotbroker.mqtt_sn.packet.impl
{
    public class SNConnect : SNMessage
    {
        public static Int32 MQTT_SN_PROTOCOL_ID = 1;

        #region private fields

        private Boolean _willPresent;
        private Boolean _cleanSession;
        private Int32 _protocolID = MQTT_SN_PROTOCOL_ID;
        private Int32 _duration;
        private String _clientID;

        #endregion

        #region constructors

        public SNConnect()
        {
        }

        public SNConnect(Boolean cleanSession, Int32 duration, String clientID, Boolean willPresent)
        {
            this._cleanSession = cleanSession;
            this._duration = duration;
            this._clientID = clientID;
            this._willPresent = willPresent;
        }

        #endregion

        #region public fields

        public SNConnect reInit(Boolean cleanSession, Int32 duration, String clientID, Boolean willPresent)
        {
            this._cleanSession = cleanSession;
            this._duration = duration;
            this._clientID = clientID;
            this._willPresent = willPresent;
            return this;
        }

        public int getLength()
        {
            if (_clientID == null || _clientID.Length==0)
                throw new MalformedMessageException("connect must contain a valid clientID");
            return 6 + _clientID.Length;
        }

        public SNType getType()
        {
            return SNType.CONNECT;
        }

        public void ProcessBy(SNDevice device)
        {
            device.ProcessConnect(CleanSession, Duration);
        }

        public Boolean WillPresent
        {
            get
            {
                return _willPresent;
            }

            set
            {
                _willPresent = value;
            }            
        }

        public Boolean CleanSession
        {
            get
            {
                return _cleanSession;
            }

            set
            {
                _cleanSession = value;
            }
        }

        public Int32 ProtocolID
        {
            get
            {
                return _protocolID;
            }

            set
            {
                _protocolID = value;
            }
        }

        public Int32 Duration
        {
            get
            {
                return _duration;
            }

            set
            {
                _duration = value;
            }
        }

        public String ClientID
        {
            get
            {
                return _clientID;
            }

            set
            {
                _clientID = value;
            }
        }

        #endregion
    }
}