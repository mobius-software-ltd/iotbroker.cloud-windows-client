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
    public class Publish:CountableMessage,MQMessage
    {
        #region private fields

        private Topic _topic;
        private Byte[] _content;
        private Boolean _retain;
        private Boolean _dup;

        #endregion

        #region constructors

        public Publish(Int32? packetID, Topic topic, Byte[] content, Boolean retain, Boolean dup):base(packetID)
        {
            this._topic = topic;
            this._content = content;
            this._retain = retain;
            this._dup = dup;
        }

        #endregion

        #region public fields

        public Int32 GetLength()
        {
            Int32 length = PacketID != null ? 2 : 0;
            length += _topic.Name.Length + 2;
            length += _content.Length;
            return length;
        }

        public void ProcessBy(MQDevice device)
        {
            device.ProcessPublish(PacketID, _topic, _content, _retain, _dup);
        }

        public MessageType MessageType
        {
            get
            {
                return MessageType.PUBLISH;
            }
        }

        public Topic Topic
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

        public Byte[] Content
        {
            get
            {
                return _content;
            }

            set
            {
                _content = value;
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

        #endregion
    }
}
