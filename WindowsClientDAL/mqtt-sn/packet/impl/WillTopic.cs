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
    public class WillTopic: SNMessage
    {
        #region private fields

        private Boolean _retain;
        private FullTopic _topic;
        
        #endregion

        #region constructors

        public WillTopic()
        {
        }

        public WillTopic(Boolean retain, FullTopic topic)
        {
            this._topic = topic;
            this._retain = retain;
        }

        #endregion

        #region public fields

        public WillTopic reInit(Boolean retain, FullTopic topic)
        {
            this._retain = retain;
            this._topic = topic;
            return this;
        }

        public int getLength()
        {
            int length = 2;
            if (_topic != null)
            {
                length += _topic.length() + 1;
                if (_topic.length() > 252)
                    length += 2;
            }
            return length;
        }

        public SNType getType()
        {
            return SNType.WILL_TOPIC;
        }

        public void ProcessBy(SNDevice device)
        {
            device.ProcessWillTopic(Topic);
        }

        public FullTopic Topic
        {
            get
            {
                return _topic;
            }

            set
            {
                _topic = value;
            }            
        }

        public Boolean Retain
        {
            get
            {
                return _retain;
            }

            set
            {
                _retain = value;
            }
        }

        #endregion
    }
}