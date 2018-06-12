

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
    public class MessageAnnotations : AMQPSection
    {
        #region private fields

        private Dictionary<Object, Object> _annotations;

        #endregion

        #region constructors

        #endregion

        #region public fields

        public TLVAmqp getValue()
        {
            TLVMap map = new TLVMap();

            if (_annotations != null)
                map = AMQPWrapper<Object>.wrapMap(_annotations);

            DescribedConstructor constructor = new DescribedConstructor(map.Code, new TLVFixed(AMQPType.SMALL_ULONG, new byte[] { 0x72 }));
            map.Constructor = constructor;

            return map;
        }

        public void fill(TLVAmqp map)
        {
            if (!map.isNull())
                _annotations = AMQPUnwrapper<Object>.unwrapMap(map);
        }

        public SectionCodes getCode()
        {
            return SectionCodes.MESSAGE_ANNOTATIONS;
        }

        public Dictionary<Object, Object> Annotations
        {
            get
            {
                return _annotations;
            }
        }

        public void addAnnotations(Object key, Object value)
        {
            if (key == null)
                throw new ArgumentException("Annotations map cannot contain objects with null keys");

            if (_annotations == null)
                _annotations = new Dictionary<Object, Object>();

            if (key is String)
                _annotations[new AMQPSymbol((String)key)] = value;
            else if (key is BigInteger)
			    _annotations[key] = value;
		    else
			    throw new ArgumentException("MessageAnnotations keys are restricted to types Symbol and ULong. Received key type: " + key.GetType());
        }

        #endregion
    }
}
