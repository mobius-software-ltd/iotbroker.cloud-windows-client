

using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.constructors;
using com.mobius.software.windows.iotbroker.amqp.headerapi;
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
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.amqp.headeramqp
{
    public class AMQPDetach : AMQPHeader
    {
        #region private fields

        private Int64? _handle;
        private Boolean? _closed;
        private AMQPError _error;

        #endregion

        #region constructors

        public AMQPDetach():base(HeaderCodes.DETACH)
        { 
        }

        #endregion

        #region public fields

        public override TLVList getArguments()
        {
            TLVList list = new TLVList();

            if (_handle == null)
                throw new MalformedMessageException("Detach header's handle can't be null");

            list.addElement(0, AMQPWrapper<AMQPSymbol>.wrap(_handle));

            if (_closed != null)
                list.addElement(1, AMQPWrapper<AMQPSymbol>.wrap(_closed));

            if (_error != null)
                list.addElement(2, _error.getList());

            DescribedConstructor constructor = new DescribedConstructor(list.Code,new TLVFixed(AMQPType.SMALL_ULONG, new byte[] { (byte)_code }));
            list.Constructor = constructor;

            return list;
        }

        public override void fillArguments(TLVList list)
        {
            int size = list.getList().Count;

            if (size == 0)
                throw new MalformedMessageException("Received malformed Detach header: handle can't be null");

            if (size > 3)
                throw new MalformedMessageException("Received malformed Detach header. Invalid number of arguments: " + size);

            if (size > 0)
            {
                TLVAmqp element = list.getList()[0];
                if (element.isNull())
                    throw new MalformedMessageException("Received malformed Detach header: handle can't be null");

                _handle = AMQPUnwrapper<AMQPSymbol>.unwrapUInt(element);
            }

            if (size > 1)
            {
                TLVAmqp element = list.getList()[1];
                if (!element.isNull())
                    _closed = AMQPUnwrapper<AMQPSymbol>.unwrapBool(element);
            }

            if (size > 2)
            {
                TLVAmqp element = list.getList()[2];
                if (!element.isNull())
                {
                    AMQPType code = element.Code;
                    if (code != AMQPType.LIST_0 && code != AMQPType.LIST_8 && code != AMQPType.LIST_32)
                        throw new MalformedMessageException("Expected type 'ERROR' - received: " + element.Code);

                    _error = new AMQPError();
                    _error.fill((TLVList)element);
                }
            }
        }

        public Int64? Handle
        {
            get
            {
                return _handle;
            }

            set
            {
                if (value == null)
                    throw new ArgumentException("Handle can't be assigned a null value");

                this._handle = value;
            }
        }

        public Boolean? Closed
        {
            get
            {
                return _closed;
            }

            set
            {
                this._closed = value;
            }
        }

        public AMQPError Error
        {
            get
            {
                return _error;
            }

            set
            {
                this._error = value;
            }
        }

        public override int getLength()
        {
            int length = 8;
            TLVAmqp arguments = this.getArguments();
            length += arguments.getLength();
            return length;
        }

        public override int getType()
        {
            return (int)HeaderCodes.DETACH;
        }

        public override void ProcessBy(AMQPDevice device)
        {
            device.ProcessDetach(Channel,Handle);
        }

        #endregion
    }
}