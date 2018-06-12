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

namespace com.mobius.software.windows.iotbroker.mqtt_sn.avps
{
    public class FullTopic: SNTopic
    {
        #region private fields

        private String _value;
        private SNQoS _qos;

        #endregion

        #region constructors

        public FullTopic()
        {
            
        }

        public FullTopic(String value, SNQoS qos)
        {
            this._value = value;
            this._qos = qos;
        }

        #endregion

        #region public fields

        public FullTopic reInit(String value, SNQoS qos)
        {
            this._value = value;
            this._qos = qos;
            return this;
        }

        public TopicType getType()
        {
            return TopicType.NAMED;
        }

        public byte[] encode()
        {
            return Encoding.UTF8.GetBytes(_value);
        }

        public SNQoS getQos()
        {
            return _qos;
        }

        public int length()
        {
            return _value.Length;
        }

        public String Value
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
