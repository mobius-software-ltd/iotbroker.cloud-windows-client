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
 
using com.mobius.software.windows.iotbroker.mqtt.avps;
using com.mobius.software.windows.iotbroker.mqtt.headers.api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.mqtt.headers.impl
{
    public class Connect : MQMessage
    {
        public static Byte DEFAULT_PROTOCOL_LEVEL = (byte)4;
        public static String PROTOCOL_NAME = "MQTT";

        #region private fields

        private String _username;
        private String _password;
        private String _clientID;

        private Byte _protocolLevel = DEFAULT_PROTOCOL_LEVEL;
        private Boolean _cleanSession;
        private Int32 _keepalive;

        private Will _will;

        #endregion

        #region constructors

        public Connect(String username, String password, String clientID, Boolean isClean, Int32 keepalive, Will will)
        {
            this._username = username;
            this._password = password;
            this._clientID = clientID;
            this._cleanSession = isClean;
            this._keepalive = keepalive;
            this._will = will;
        }

        #endregion

        #region public fields

        public Int32 GetLength()
        {
            Int32 length = 10;
            length += _clientID.Length + 2;
            length += _will != null ? _will.RetrieveLenght() : 0;
            length += _username != null ? _username.Length + 2 : 0;
            length += _password != null ? _password.Length + 2 : 0;
            return length;
        }

        public void ProcessBy(MQDevice device)
        {
            device.ProcessConnect(_cleanSession, _keepalive, _will);
        }

        public MessageType MessageType
        {
            get
            {
                return MessageType.CONNECT;
            }
        }

        public Byte ProtocolLevel
        {
            get
            {
                return _protocolLevel;
            }

            set
            {
                this._protocolLevel = value;
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

        public Will Will
        {
            get
            {
                return _will;
            }

            set
            {
                _will = value;
            }
        }

        public Int32 Keepalive
        {
            get
            {
                return _keepalive;
            }

            set
            {
                _keepalive = value;
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

        public String Username
        {
            get
            {
                return _username;
            }

            set
            {
                _username = value;
            }
        }

        public String Password
        {
            get
            {
                return _password;
            }

            set
            {
                _password = value;
            }
        }

        public String Name
        {
            get
            {
                return PROTOCOL_NAME;
            }
        }

        #endregion
    }
}
