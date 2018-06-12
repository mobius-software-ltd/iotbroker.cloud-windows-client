

using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.tlv.compound;
using com.mobius.software.windows.iotbroker.dal;
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

namespace com.mobius.software.windows.iotbroker.amqp.headerapi
{
    public abstract class AMQPHeader
    {
        #region private fields

        protected HeaderCodes? _code;

        protected Int32 _doff = 2;
        protected Int32 _headerType = 0;
        protected Int32 _channel = 0;

        #endregion

        #region constructors

        public AMQPHeader(HeaderCodes? code)
        {
            this._code = code;
        }

        #endregion

        #region public fields

        public Int32 Dott
        {
            get
            {
                return _doff;
            }

            set
            {
                _doff = value;
            }
        }

        public Int32 HeaderType
        {
            get
            {
                return _headerType;
            }

            set
            {
                _headerType = value;
            }
        }

        public Int32 Channel
        {
            get
            {
                return _channel;
            }

            set
            {
                _channel = value;
            }
        }

        public HeaderCodes? Code
        {
            get
            {
                return _code;
            }            
        }

        public Protocols getProtocol()
        {
            return Protocols.AMQP_PROTOCOL;
        }

        public abstract TLVList getArguments();

        public abstract void fillArguments(TLVList list);

        public abstract int getType();

        public abstract int getLength();

        public abstract void ProcessBy(AMQPDevice device);

        #endregion
    }
}
