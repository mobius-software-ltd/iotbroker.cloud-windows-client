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
    public class SNPublish : CountableMessage
    {
        #region private fields

        private SNTopic _snTopic;
        private Byte[] _content;
        private Boolean _dup;
        private Boolean _retain;

        #endregion

        #region constructors

        public SNPublish()
        {
        }

        public SNPublish(Int32? messageID, SNTopic snTopic, Byte[] content, Boolean dup, Boolean retain):base(messageID)
        {
            this._snTopic = snTopic;
            this._content = content;
            this._dup = dup;
            this._retain = retain;
        }

        #endregion

        #region public fields

        public SNPublish reInit(Int32? messageID, SNTopic snTopic, Byte[] content, Boolean dup, Boolean retain)
        {
            this.MessageID = messageID;
            this._snTopic = snTopic;
            this._content = content;
            this._dup = dup;
            this._retain = retain;
            return this;
        }

        override
        public int getLength()
        {
            int length = 7;
            length += _content.Length;
            if (_content.Length > 248)
                length += 2;

            return length;
        }

        public override SNType getType()
        {
            return SNType.PUBLISH;
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

        public override void ProcessBy(SNDevice device)
        {
            device.ProcessPublish(MessageID.Value, SnTopic, Content, Retain, Dup);
        }

        #endregion
    }
}