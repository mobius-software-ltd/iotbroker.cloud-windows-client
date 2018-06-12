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
    public class SNSuback: CountableMessage
    {
        #region private fields

        private Int32 _topicID;
        private ReturnCode _code;
        private SNQoS _allowedQos;

        #endregion

        #region constructors

        public SNSuback()
        {
        }

        public SNSuback(Int32 topicID, Int32 messageID, ReturnCode code, SNQoS allowedQos) :base(messageID)
        {
            this._topicID = topicID;
            this._code = code;
            this._allowedQos = allowedQos;
        }

        #endregion

        #region public fields

        public SNSuback reInit(Int32 topicID, Int32 messageID, ReturnCode code, SNQoS allowedQos)
        {
            this._topicID = topicID;
            this.MessageID = messageID;
            this._code = code;
            this._allowedQos = allowedQos;
            return this;
        }

        public new int getLength()
        {
            return 8;
        }

        public override SNType getType()
        {
            return SNType.SUBACK;
        }

        public Int32 topicID
        {
            get
            {
                return _topicID;
            }

            set
            {
                _topicID = value;
            }            
        }

        public ReturnCode ReturnCode
        {
            get
            {
                return _code;
            }

            set
            {
                _code = value;
            }
        }

        public SNQoS AllowedQos
        {
            get
            {
                return _allowedQos;
            }

            set
            {
                _allowedQos = value;
            }
        }

        public override void ProcessBy(SNDevice device)
        {
            device.ProcessSuback(MessageID.Value,topicID,ReturnCode,AllowedQos);
        }

        #endregion
    }
}