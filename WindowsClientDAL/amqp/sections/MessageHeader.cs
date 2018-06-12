

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
    public class MessageHeader : AMQPSection
    {
        #region private fields

        private Boolean? _durable;
        private Int16? _priority;
        private Int64? _milliseconds;
        private Boolean? _firstAquirer;
        private Int64? _deliveryCount;

        #endregion

        #region constructors

        #endregion

        #region public fields

        public TLVAmqp getValue()
        {
            TLVList list = new TLVList();

            if (_durable != null)
                list.addElement(0, AMQPWrapper<AMQPSymbol>.wrap(_durable));
            if (_priority != null)
                list.addElement(1, AMQPWrapper<AMQPSymbol>.wrap(_priority));
            if (_milliseconds != null)
                list.addElement(2, AMQPWrapper<AMQPSymbol>.wrap(_milliseconds));
            if (_firstAquirer != null)
                list.addElement(3, AMQPWrapper<AMQPSymbol>.wrap(_firstAquirer));
            if (_deliveryCount != null)
                list.addElement(4, AMQPWrapper<AMQPSymbol>.wrap(_deliveryCount));

            DescribedConstructor constructor = new DescribedConstructor(list.Code, new TLVFixed(AMQPType.SMALL_ULONG, new byte[] { 0x70 }));
            list.Constructor = constructor;

            return list;
        }

        public void fill(TLVAmqp value)
        {
            TLVList list = (TLVList)value;
            if (list.getList().Count > 0)
            {
                TLVAmqp element = list.getList()[0];
                if (!element.isNull())
                    _durable = AMQPUnwrapper<AMQPSymbol>.unwrapBool(element);
            }

            if (list.getList().Count > 1)
            {
                TLVAmqp element = list.getList()[1];
                if (!element.isNull())
                    _priority = AMQPUnwrapper<AMQPSymbol>.unwrapUByte(element);
            }
            if (list.getList().Count > 2)
            {
                TLVAmqp element = list.getList()[2];
                if (!element.isNull())
                    _milliseconds = AMQPUnwrapper<AMQPSymbol>.unwrapUInt(element);
            }
            if (list.getList().Count > 3)
            {
                TLVAmqp element = list.getList()[3];
                if (!element.isNull())
                    _firstAquirer = AMQPUnwrapper<AMQPSymbol>.unwrapBool(element);
            }
            if (list.getList().Count > 4)
            {
                TLVAmqp element = list.getList()[4];
                if (!element.isNull())
                    _deliveryCount = AMQPUnwrapper<AMQPSymbol>.unwrapUInt(element);
            }
        }

        public SectionCodes getCode()
        {
            return SectionCodes.HEADER;
        }

        public Boolean? Durable
        {
            get
            {
                return _durable;
            }

            set
            {
                _durable = value;
            }
        }

        public Int16? Priority
        {
            get
            {
                return _priority;
            }

            set
            {
                _priority = value;
            }
        }

        public Int64? Milliseconds
        {
            get
            {
                return _milliseconds;
            }

            set
            {
                _milliseconds = value;
            }
        }

        public Boolean? FirstAquirer
        {
            get
            {
                return _firstAquirer;
            }

            set
            {
                _firstAquirer = value;
            }
        }

        public Int64? DeliveryCount
        {
            get
            {
                return _deliveryCount;
            }

            set
            {
                _deliveryCount = value;
            }
        }

        #endregion
    }
}
