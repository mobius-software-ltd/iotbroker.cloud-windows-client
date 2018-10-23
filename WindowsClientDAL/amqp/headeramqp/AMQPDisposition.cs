

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
    public class AMQPDisposition : AMQPHeader
    {
        #region private fields

        private RoleCodes? _role;
        private Int64? _first;
        private Int64? _last;
        private Boolean? _settled;
        private AMQPState _state;
        private Boolean? _batchable;
        
        #endregion

        #region constructors

        public AMQPDisposition():base(HeaderCodes.DISPOSITION)
        { 
        }

        #endregion

        #region public fields

        public override TLVList getArguments()
        {
            TLVList list = new TLVList();

            if (_role == null)
                throw new MalformedMessageException("Disposition header's role can't be null");

            if (_role.Value == RoleCodes.RECEIVER)
                list.addElement(0, AMQPWrapper<AMQPSymbol>.wrap(true));
            else
                list.addElement(0, AMQPWrapper<AMQPSymbol>.wrap(false));

            if (_first == null)
                throw new MalformedMessageException("Transfer header's first can't be null");

            list.addElement(1, AMQPWrapper<AMQPSymbol>.wrap(_first));

            if (_last != null)
                list.addElement(2, AMQPWrapper<AMQPSymbol>.wrap(_last));

            if (_settled != null)
                list.addElement(3, AMQPWrapper<AMQPSymbol>.wrap(_settled));

            if (_state != null)
                list.addElement(4, _state.getList());

            if (_batchable != null)
                list.addElement(5, AMQPWrapper<AMQPSymbol>.wrap(_batchable));

            DescribedConstructor constructor = new DescribedConstructor(list.Code,new TLVFixed(AMQPType.SMALL_ULONG, new byte[] { (byte)_code }));
            list.Constructor = constructor;

            return list;
        }

        public override void fillArguments(TLVList list)
        {
            int size = list.getList().Count;

            if (size < 2)
                throw new MalformedMessageException("Received malformed Disposition header: role and first can't be null");

            if (size > 6)
                throw new MalformedMessageException("Received malformed Disposition header. Invalid number of arguments: " + size);

            if (size > 0)
            {
                TLVAmqp element = list.getList()[0];
                if (element.isNull())
                    throw new MalformedMessageException("Received malformed Disposition header: role can't be null");

                 Boolean role = AMQPUnwrapper<AMQPSymbol>.unwrapBool(element);
                if (role)
                    _role = RoleCodes.RECEIVER;
                else
                    _role = RoleCodes.SENDER;
            }

            if (size > 1)
            {
                TLVAmqp element = list.getList()[1];
                if (element.isNull())
                    throw new MalformedMessageException("Received malformed Disposition header: first can't be null");

                _first = AMQPUnwrapper<AMQPSymbol>.unwrapUInt(element);
            }

            if (size > 2)
            {
                TLVAmqp element = list.getList()[2];
                if (!element.isNull())
                    _last = AMQPUnwrapper<AMQPSymbol>.unwrapUInt(element);
            }

            if (size > 3)
            {
                TLVAmqp element = list.getList()[3];
                if (!element.isNull())
                    _settled = AMQPUnwrapper<AMQPSymbol>.unwrapBool(element);
            }

            if (size > 4)
            {
                TLVAmqp element = list.getList()[4];
                if (!element.isNull())
                {
                    AMQPType code = element.Code;
                    if (code != AMQPType.LIST_0 && code != AMQPType.LIST_8 && code != AMQPType.LIST_32)
                        throw new MalformedMessageException("Expected type 'STATE' - received: " + element.Code);

                    _state = AMQPFactory.getState((TLVList)element);
                    _state.fill((TLVList)element);
                }
            }

            if (size > 5)
            {
                TLVAmqp element = list.getList()[5];
                if (!element.isNull())
                    _batchable = AMQPUnwrapper<AMQPSymbol>.unwrapBool(element);
            }
        }

        public RoleCodes? Role
        {
            get
            {
                return _role;
            }

            set
            {
                if (value == null)
                    throw new ArgumentException("Role can't be assigned a null value");

                this._role = value;
            }
        }

        public Int64? First
        {
            get
            {
                return _first;
            }

            set
            {
                this._first = value;
            }
        }

        public Int64? Last
        {
            get
            {
                return _last;
            }

            set
            {
                this._last = value;
            }
        }

        public Boolean? Settled
        {
            get
            {
                return _settled;
            }

            set
            {
                this._settled = value;
            }
        }

        public AMQPState State
        {
            get
            {
                return _state;
            }

            set
            {
                this._state = value;
            }
        }

        public Boolean? Batchable
        {
            get
            {
                return _batchable;
            }

            set
            {
                this._batchable = value;
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
            return (int)HeaderCodes.DISPOSITION;
        }

        public override void ProcessBy(AMQPDevice device)
        {
            device.ProcessDisposition(First,Last);
        }

        #endregion
    }
}
