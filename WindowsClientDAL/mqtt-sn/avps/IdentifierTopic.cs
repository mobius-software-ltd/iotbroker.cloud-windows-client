using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

namespace com.mobius.software.windows.iotbroker.mqtt_sn.avps
{
    public class IdentifierTopic : SNTopic
    {
        #region private fields

        private Int32 _value;
        private SNQoS _qos;

        #endregion

        #region constructors

        public IdentifierTopic()
        {

        }

        public IdentifierTopic(Int32 value, SNQoS qos)
        {
            this._value = value;
            this._qos = qos;
        }

        #endregion

        #region public fields

        public IdentifierTopic reInit(Int32 value, SNQoS qos)
        {
            this._value = value;
            this._qos = qos;
            return this;
        }

        public TopicType getType()
        {
            return TopicType.ID;
        }

        public byte[] encode()
        {
            byte[] output = new byte[2];
            output[0] = (byte)((_value >> 8) & 0x0FF);
            output[1] = (byte)(_value & 0x0FF);
            return output;
        }

        public SNQoS getQos()
        {
            return _qos;
        }

        public int length()
        {
            return 2;
        }

        public Int32 Value
        {
            get
            {
                return _value;
            }

            set
            {
                this._value = value;
            }
        }

        public SNQoS Qos
        {
            get
            {
                return _qos;
            }

            set
            {
                this._qos = value;
            }
        }

        #endregion
    }
}
