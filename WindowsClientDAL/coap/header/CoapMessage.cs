

using com.mobius.software.windows.iotbroker.mqtt.exceptions;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.coap.avps
{
    public class CoapMessage
    {
        #region private fields

        private Int32 _version;
        private CoapType _type;
        private CoapCode _code;
        private Int32 _messageID;
        private byte[] _token;
        private List<CoapOption> _options;
        private byte[] _payload;

        #endregion

        #region constructors

        public CoapMessage(Int32 version, CoapType type, CoapCode code, Int32 messageID, byte[] token, List<CoapOption> options, byte[] payload)
        {
            this._version = version;
            this._type = type;
            this._code = code;
            this._messageID = messageID;
            this._token = token;
            this._options = options;
            this._payload = payload;
        }

        #endregion

        #region public fields

        public Int32 Version
        {
            get
            {
                return _version;
            }

            set
            {
                this._version = value;
            }
        }

        public CoapType CoapType
        {
            get
            {
                return _type;
            }

            set
            {
                this._type = value;
            }
        }

        public CoapCode CoapCode
        {
            get
            {
                return _code;
            }

            set
            {
                this._code = value;
            }
        }

        public Int32 MessageID
        {
            get
            {
                return _messageID;
            }

            set
            {
                this._messageID = value;
            }
        }

        public byte[] Token
        {
            get
            {
                return _token;
            }

            set
            {
                this._token = value;
            }
        }

        public List<CoapOption> Options
        {
            get
            {
                return _options;
            }

            set
            {
                this._options = value;
            }
        }

        public byte[] Payload
        {
            get
            {
                return _payload;
            }

            set
            {
                this._payload = value;
            }
        }

        #endregion
    }
}
