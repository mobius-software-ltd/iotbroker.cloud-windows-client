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
    public class Subscribe:CountableMessage,MQMessage
    {
        #region private fields

        private Topic[] _topics;

        #endregion

        #region constructors
        
        public Subscribe(Int32? packetID, Topic[] topics):base(packetID)
        {            
            this._topics = topics;
        }

        #endregion

        #region public fields

        public Int32 GetLength()
        {
            Int32 length = 2;
            for (int i = 0; i < _topics.Length; i++)
                length += _topics[i].Name.Length + 3;

            return length;
        }

        public void ProcessBy(MQDevice device)
        {
            device.ProcessSubscribe(PacketID.Value,_topics);
        }

        public MessageType MessageType
        {
            get
            {
                return MessageType.SUBSCRIBE;
            }            
        }

        public Topic[] Topics
        {
            get
            {
                return _topics;
            }

            set
            {
                this._topics = value;
            }
        }

        #endregion
    }
}
