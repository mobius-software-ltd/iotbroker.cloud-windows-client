

using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.constructors;
using com.mobius.software.windows.iotbroker.amqp.tlv.api;
using com.mobius.software.windows.iotbroker.amqp.tlv.array;
using com.mobius.software.windows.iotbroker.amqp.tlv.compound;
using com.mobius.software.windows.iotbroker.amqp.tlv.fixed_;
using com.mobius.software.windows.iotbroker.amqp.wrappers;
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
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.amqp.tlv.described
{
    public class AMQPModified : AMQPOutcome, AMQPState
    {
        #region private fields

        private Boolean? _deliveryFailed;
        private Boolean? _undeliverableHere;
        private Dictionary<AMQPSymbol, Object> _messageAnnotations;

        #endregion

        #region constructors



        #endregion

        #region public fields

        public TLVList getList()
        {
            TLVList list = new TLVList();

            if (_deliveryFailed.HasValue)
                list.addElement(0, AMQPWrapper<AMQPSymbol>.wrap(_deliveryFailed));
            if (_undeliverableHere.HasValue)
                list.addElement(1, AMQPWrapper<AMQPSymbol>.wrap(_undeliverableHere));
            if (_messageAnnotations!=null)
                list.addElement(2, AMQPWrapper<AMQPSymbol>.wrapMap(_messageAnnotations));

            DescribedConstructor constructor = new DescribedConstructor(list.Code, new TLVFixed(AMQPType.SMALL_ULONG, new byte[] { 0x27 }));
            list.Constructor = constructor;

            return list;
        }

        public void fill(TLVList list)
        {
            if (list.getList().Count > 0)
            {
                TLVAmqp element = list.getList()[0];
                if (!element.isNull())
                    _deliveryFailed = AMQPUnwrapper<AMQPSymbol>.unwrapBool(element);
            }

            if (list.getList().Count > 1)
            {
                TLVAmqp element = list.getList()[1];
                if (!element.isNull())
                    _undeliverableHere = AMQPUnwrapper<AMQPSymbol>.unwrapBool(element);
            }

            if (list.getList().Count > 2)
            {
                TLVAmqp element = list.getList()[2];
                if (!element.isNull())
                    _messageAnnotations = AMQPUnwrapper<AMQPSymbol>.unwrapMap(element);
            }
        }

        public Boolean? DeliveryFailed
        {
            get
            {
                return _deliveryFailed;
            }

            set
            {
                _deliveryFailed = value;
            }
        }

        public Boolean? UndeliverableHere
        {
            get
            {
                return _undeliverableHere;
            }

            set
            {
                _undeliverableHere = value;
            }
        }

        public Dictionary<AMQPSymbol, Object> MessageAnnotations
        {
            get
            {
                return _messageAnnotations;
            }
        }

        public void addMessageAnnotation(String key, Object value)
        {
            if (!key.StartsWith("x-"))
                throw new ArgumentException();
            if (_messageAnnotations == null)
                _messageAnnotations = new Dictionary<AMQPSymbol, object>();

            _messageAnnotations[new AMQPSymbol(key)] = value;
        }

        #endregion
    }
}