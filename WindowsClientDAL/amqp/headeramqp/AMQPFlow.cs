

using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.constructors;
using com.mobius.software.windows.iotbroker.amqp.headerapi;
using com.mobius.software.windows.iotbroker.amqp.tlv.api;
using com.mobius.software.windows.iotbroker.amqp.tlv.compound;
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
    public class AMQPFlow : AMQPHeader
    {
        #region private fields

        private Int64? _nextIncomingId;
        private Int64? _incomingWindow;
        private Int64? _nextOutgoingId;
        private Int64? _outgoingWindow;
        private Int64? _handle;
        private Int64? _deliveryCount;
        private Int64? _linkCredit;
        private Int64? _avaliable;
        private Boolean? _drain;
        private Boolean? _echo;
        private Dictionary<AMQPSymbol, Object> _properties;

        #endregion

        #region constructors

        public AMQPFlow():base(HeaderCodes.FLOW)
        { 
        }

        #endregion

        #region public fields

        public override TLVList getArguments()
        {
            TLVList list = new TLVList();

            if (_nextIncomingId != null)
                list.addElement(0, AMQPWrapper<AMQPSymbol>.wrap(_nextIncomingId));

            if (_incomingWindow == null)
                throw new MalformedMessageException("Flow header's incoming-window can't be null");
            list.addElement(1, AMQPWrapper<AMQPSymbol>.wrap(_incomingWindow));

            if (_nextOutgoingId == null)
                throw new MalformedMessageException("Flow header's next-outgoing-id can't be null");
            list.addElement(2, AMQPWrapper<AMQPSymbol>.wrap(_nextOutgoingId));

            if (_outgoingWindow == null)
                throw new MalformedMessageException("Flow header's outgoing-window can't be null");
            list.addElement(3, AMQPWrapper<AMQPSymbol>.wrap(_outgoingWindow));

            if (_handle != null)
                list.addElement(4, AMQPWrapper<AMQPSymbol>.wrap(_handle));

            if (_deliveryCount != null)
                if (_handle != null)
                    list.addElement(5, AMQPWrapper<AMQPSymbol>.wrap(_deliveryCount));
                else
                    throw new MalformedMessageException("Flow headers delivery-count can't be assigned when handle is not specified");

            if (_linkCredit != null)
                if (_handle != null)
                    list.addElement(6, AMQPWrapper<AMQPSymbol>.wrap(_linkCredit));
                else
                    throw new MalformedMessageException(
                            "Flow headers link-credit can't be assigned when handle is not specified");

            if (_avaliable != null)
                if (_handle != null)
                    list.addElement(7, AMQPWrapper<AMQPSymbol>.wrap(_avaliable));
                else
                    throw new MalformedMessageException(
                            "Flow headers avaliable can't be assigned when handle is not specified");

            if (_drain != null)
                if (_handle != null)
                    list.addElement(8, AMQPWrapper<AMQPSymbol>.wrap(_drain));
                else
                    throw new MalformedMessageException("Flow headers drain can't be assigned when handle is not specified");

            if (_echo != null)
                list.addElement(9, AMQPWrapper<AMQPSymbol>.wrap(_echo));

            if (_properties != null)
                list.addElement(10, AMQPWrapper<AMQPSymbol>.wrapMap(_properties));

            DescribedConstructor constructor = new DescribedConstructor(list.Code,new TLVFixed(AMQPType.SMALL_ULONG, new byte[] { (byte)_code }));
            list.Constructor = constructor;

            return list;
        }

        public override void fillArguments(TLVList list)
        {
            int size = list.getList().Count;

            if (size < 4)
                throw new MalformedMessageException("Received malformed Flow header: mandatory " + "fields incoming-window, next-outgoing-id and " + "outgoing-window must not be null");

            if (size > 11)
                throw new MalformedMessageException("Received malformed Flow header. Invalid arguments size: " + size);

            if (size > 0)
            {
                TLVAmqp element = list.getList()[0];
                if (!element.isNull())
                    _nextIncomingId = AMQPUnwrapper<AMQPSymbol>.unwrapUInt(element);
            }

            if (size > 1)
            {
                TLVAmqp element = list.getList()[1];
                if (element.isNull())
                    throw new MalformedMessageException("Received malformed Flow header: incoming-window can't be null");

                _incomingWindow = AMQPUnwrapper<AMQPSymbol>.unwrapUInt(element);
            }

            if (size > 2)
            {
                TLVAmqp element = list.getList()[2];
                if (element.isNull())
                    throw new MalformedMessageException("Received malformed Flow header: next-outgoing-id can't be null");

                _nextOutgoingId = AMQPUnwrapper<AMQPSymbol>.unwrapUInt(element);
            }

            if (size > 3)
            {
                TLVAmqp element = list.getList()[3];
                if (element.isNull())
                    throw new MalformedMessageException("Received malformed Flow header: outgoing-window can't be null");
                _outgoingWindow = AMQPUnwrapper<AMQPSymbol>.unwrapUInt(element);
            }
            if (size > 4)
            {
                TLVAmqp element = list.getList()[4];
                if (!element.isNull())
                    _handle = AMQPUnwrapper<AMQPSymbol>.unwrapUInt(element);
            }

            if (size > 5)
            {
                TLVAmqp element = list.getList()[5];
                if (!element.isNull())
                    if (_handle != null)
                        _deliveryCount = AMQPUnwrapper<AMQPSymbol>.unwrapUInt(element);
                    else
                        throw new MalformedMessageException("Received malformed Flow header: delivery-count can't be present when handle is null");
            }

            if (size > 6)
            {
                TLVAmqp element = list.getList()[6];
                if (!element.isNull())
                    if (_handle != null)
                        _linkCredit = AMQPUnwrapper<AMQPSymbol>.unwrapUInt(element);
                    else
                        throw new MalformedMessageException("Received malformed Flow header: link-credit can't be present when handle is null");

            }

            if (size > 7)
            {
                TLVAmqp element = list.getList()[7];
                if (!element.isNull())
                    if (_handle != null)
                        _avaliable = AMQPUnwrapper<AMQPSymbol>.unwrapUInt(element);
                    else
                        throw new MalformedMessageException(
                                "Received malformed Flow header: avaliable can't be present when handle is null");
            }

            if (size > 8)
            {
                TLVAmqp element = list.getList()[8];
                if (!element.isNull())
                    if (_handle != null)
                        _drain = AMQPUnwrapper<AMQPSymbol>.unwrapBool(element);
                    else
                        throw new MalformedMessageException(
                                "Received malformed Flow header: drain can't be present when handle is null");

            }

            if (size > 9)
            {
                TLVAmqp element = list.getList()[9];
                if (!element.isNull())
                    _echo = AMQPUnwrapper<AMQPSymbol>.unwrapBool(element);
            }

            if (size > 10)
            {
                TLVAmqp element = list.getList()[10];
                if (!element.isNull())
                    _properties = AMQPUnwrapper<AMQPSymbol>.unwrapMap(element);
            }
        }

        public Int64? NextIncomingId
        {
            get
            {
                return _nextIncomingId;
            }

            set
            {
                this._nextIncomingId = value;
            }
        }

        public Int64? IncomingWindow
        {
            get
            {
                return _incomingWindow;
            }

            set
            {
                if (value == null)
                    throw new ArgumentException("Incoming-window can't be assigned a null value");

                this._incomingWindow = value;
            }
        }

        public Int64? NextOutgoingId
        {
            get
            {
                return _nextOutgoingId;
            }

            set
            {
                if (value == null)
                    throw new ArgumentException("Next-outgoing-id can't be assigned a null value");

                this._nextOutgoingId = value;
            }
        }

        public Int64? OutgoingWindow
        {
            get
            {
                return _outgoingWindow;
            }

            set
            {
                if (value == null)
                    throw new ArgumentException("Outgoing-window can't be assigned a null value");

                this._outgoingWindow = value;
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

        public Int64? DeliveryCount
        {
            get
            {
                return _deliveryCount;
            }

            set
            {
                this._deliveryCount = value;
            }
        }

        public Int64? LinkCredit
        {
            get
            {
                return _linkCredit;
            }

            set
            {
                this._linkCredit = value;
            }
        }

        public Int64? Avaliable
        {
            get
            {
                return _avaliable;
            }

            set
            {
                this._avaliable = value;
            }
        }

        public Boolean? Drain
        {
            get
            {
                return _drain;
            }

            set
            {
                this._drain = value;
            }
        }

        public Boolean? Echo
        {
            get
            {
                return _echo;
            }

            set
            {
                this._echo = value;
            }
        }

        public Dictionary<AMQPSymbol, Object> Properties
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
                _properties = new Dictionary<AMQPSymbol, Object>();

            _properties[new AMQPSymbol(key)] = value;
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
            return (int)HeaderCodes.FLOW;
        }

        public override void ProcessBy(AMQPDevice device)
        {
            device.ProcessFlow(Channel);
        }

        #endregion
    }
}
