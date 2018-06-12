

using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.constructors;
using com.mobius.software.windows.iotbroker.amqp.headerapi;
using com.mobius.software.windows.iotbroker.amqp.sections;
using com.mobius.software.windows.iotbroker.amqp.tlv.api;
using com.mobius.software.windows.iotbroker.amqp.tlv.compound;
using com.mobius.software.windows.iotbroker.amqp.tlv.described;
using com.mobius.software.windows.iotbroker.amqp.tlv.fixed_;
using com.mobius.software.windows.iotbroker.amqp.wrappers;
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
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.amqp.sections
{
    public class ApplicationProperties : AMQPSection
    {
        #region private fields

        private Dictionary<String, Object> _properties;

        #endregion

        #region constructors

        #endregion

        #region public fields

        public TLVAmqp getValue()
        {
            TLVMap map = new TLVMap();

            if (_properties != null)
                map = AMQPWrapper<String>.wrapMap(_properties);

            DescribedConstructor constructor = new DescribedConstructor(map.Code, new TLVFixed(AMQPType.SMALL_ULONG, new byte[] { 0x74 }));
            map.Constructor = constructor;

            return map;
        }

        public void fill(TLVAmqp map)
        {
            if (!map.isNull())
                _properties = AMQPUnwrapper<String>.unwrapMap(map);
        }

        public SectionCodes getCode()
        {
            return SectionCodes.APPLICATION_PROPERTIES;
        }

        public Dictionary<String, Object> Properties
        {
            get
            {
                return _properties;
            }
        }

        public void addProperties(String key, Object value)
        {
            if (key == null)
                throw new ArgumentException("Properties map cannot contain objects with null keys");

            if (_properties == null)
                _properties = new Dictionary<String, Object>();

            _properties[key] = value;
        }

        #endregion
    }
}
