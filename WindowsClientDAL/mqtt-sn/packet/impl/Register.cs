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
    public class Register: CountableMessage
    {
        #region private fields

        private Int32 _topicID;
        private String _topicName;

        #endregion

        #region constructors

        public Register()
        {
        }

        public Register(Int32 topicID, Int32? messageID, String topicName) :base(messageID)
        {
            this._topicID = topicID;
            this._topicName = topicName;
        }

        #endregion

        #region public fields

        public Register reInit(Int32 topicID, Int32 messageID, String topicName)
        {
            this._topicID = topicID;
            this.MessageID = messageID;
            this._topicName = topicName;
            return this;
        }

        public new int getLength()
        {
            if (this._topicName == null)
                throw new MalformedMessageException(this.GetType().Name + " must contain a valid topic name");

            int length = 6;
            length += _topicName.Length;
            if (_topicName.Length > 249)
                length += 2;
            return length;
        }

        public override SNType getType()
        {
            return SNType.REGISTER;
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

        public String TopicName
        {
            get
            {
                return _topicName;
            }

            set
            {
                _topicName = value;
            }
        }

        public override void ProcessBy(SNDevice device)
        {
            device.ProcessRegister(MessageID.Value, topicID, TopicName);
        }

        #endregion
    }
}