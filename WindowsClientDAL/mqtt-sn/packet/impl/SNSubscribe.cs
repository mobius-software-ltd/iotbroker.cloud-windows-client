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
using DotNetty.Buffers;
using com.mobius.software.windows.iotbroker.mqtt_sn.headers.api;

namespace com.mobius.software.windows.iotbroker.mqtt_sn.packet.impl
{
    public class SNSubscribe: CountableMessage
    {
        #region private fields

        private SNTopic _snTopic;
        private Boolean _dup;
        
        #endregion

        #region constructors

        public SNSubscribe()
        {
        }

        public SNSubscribe(Int32? messageID, SNTopic snTopic, Boolean dup):base(messageID)
        {
            this._snTopic = snTopic;
            this._dup = dup;            
        }

        #endregion

        #region public fields

        public SNSubscribe reInit(Int32? messageID, SNTopic snTopic, Boolean dup)
        {
            this.MessageID = messageID;
            this._snTopic = snTopic;
            this._dup = dup;
            return this;
        }

        override
        public int getLength()
        {
            int length = 5;
            length += _snTopic.length();
            if (_snTopic.length() > 250)
                length += 2;
            return length;
        }

        public override SNType getType()
        {
            return SNType.SUBSCRIBE;
        }

        public SNTopic SnTopic
        {
            get
            {
                return _snTopic;
            }

            set
            {
                _snTopic = value;
            }            
        }

        public Boolean Dup
        {
            get
            {
                return _dup;
            }

            set
            {
                _dup = value;
            }
        }

        public override void ProcessBy(SNDevice device)
        {
            device.ProcessSubscribe(MessageID.Value, SnTopic);
        }

        #endregion
    }
}