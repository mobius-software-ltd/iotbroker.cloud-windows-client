

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
    public class AMQPBegin : AMQPHeader
    {
        #region private fields

        private Int32? _remoteChannel;
        private Int64? _nextOutgoingId;
        private Int64? _incomingWindow;
        private Int64? _outgoingWindow;
        private Int64? _handleMax;
        private List<AMQPSymbol> _offeredCapabilities;
        private List<AMQPSymbol> _desiredCapabilities;
        private Dictionary<AMQPSymbol, Object> _properties;

        #endregion

        #region constructors

        public AMQPBegin():base(HeaderCodes.BEGIN)
        { 
        }

        #endregion

        #region public fields

        public override TLVList getArguments()
        {

            TLVList list = new TLVList();

            if (_remoteChannel != null)
                list.addElement(0, AMQPWrapper<AMQPSymbol>.wrap(_remoteChannel));

            if (_nextOutgoingId == null)
                throw new MalformedMessageException("Begin header's next-outgoing-id can't be null");
            list.addElement(1, AMQPWrapper<AMQPSymbol>.wrap(_nextOutgoingId));

            if (_incomingWindow == null)
                throw new MalformedMessageException("Begin header's incoming-window can't be null");
            list.addElement(2, AMQPWrapper<AMQPSymbol>.wrap(_incomingWindow));

            if (_outgoingWindow == null)
                throw new MalformedMessageException("Begin header's incoming-window can't be null");
            list.addElement(3, AMQPWrapper<AMQPSymbol>.wrap(_outgoingWindow));

            if (_handleMax != null)
                list.addElement(4, AMQPWrapper<AMQPSymbol>.wrap(_handleMax));

            if (_offeredCapabilities != null)
                list.addElement(5, AMQPWrapper<AMQPSymbol>.wrapArray(_offeredCapabilities));

            if (_desiredCapabilities != null)
                list.addElement(6, AMQPWrapper<AMQPSymbol>.wrapArray(_desiredCapabilities));

            if (_properties != null)
                list.addElement(7, AMQPWrapper<AMQPSymbol>.wrapMap(_properties));

            DescribedConstructor constructor = new DescribedConstructor(list.Code, new TLVFixed(AMQPType.SMALL_ULONG, new byte[] { (byte)_code.Value }));
            list.Constructor = constructor;

            return list;
        }

        public override void fillArguments(TLVList list)
        {
            int size = list.getList().Count;

            if (size < 4)
                throw new MalformedMessageException("Received malformed Begin header: mandatory " + "fields next-outgoing-id, incoming-window and " + "outgoing-window must not be null");

            if (size > 8)
                throw new MalformedMessageException("Received malformed Begin header. Invalid number of arguments: " + size);

            if (size > 0)
            {
                TLVAmqp element = list.getList()[0];
                if (!element.isNull())
                    _remoteChannel = AMQPUnwrapper<AMQPSymbol>.unwrapUShort(element);
            }

            if (size > 1)
            {
                TLVAmqp element = list.getList()[1];
                if (element.isNull())
                    throw new MalformedMessageException("Received malformed Begin header: next-outgoing-id can't be null");
                _nextOutgoingId = AMQPUnwrapper<AMQPSymbol>.unwrapUInt(element);
            }

            if (size > 2)
            {
                TLVAmqp element = list.getList()[2];
                if (element.isNull())
                    throw new MalformedMessageException("Received malformed Begin header: incoming-window can't be null");
                _incomingWindow = AMQPUnwrapper<AMQPSymbol>.unwrapUInt(element);
            }

            if (size > 3)
            {
                TLVAmqp element = list.getList()[3];
                if (element.isNull())
                    throw new MalformedMessageException("Received malformed Begin header: outgoing-window can't be null");
                _outgoingWindow = AMQPUnwrapper<AMQPSymbol>.unwrapUInt(element);
            }

            if (size > 4)
            {
                TLVAmqp element = list.getList()[4];
                if (!element.isNull())
                    _handleMax = AMQPUnwrapper<AMQPSymbol>.unwrapUInt(element);
            }

            if (size > 5)
            {
                TLVAmqp element = list.getList()[5];
                if (!element.isNull())
                    _offeredCapabilities = AMQPUnwrapper<AMQPSymbol>.unwrapArray(element);
            }

            if (size > 6)
            {
                TLVAmqp element = list.getList()[6];
                if (!element.isNull())
                    _desiredCapabilities = AMQPUnwrapper<AMQPSymbol>.unwrapArray(element);
            }

            if (size > 7)
            {
                TLVAmqp element = list.getList()[7];
                if (!element.isNull())
                    _properties = AMQPUnwrapper<AMQPSymbol>.unwrapMap(element);
            }
        }

        public Int32? RemoteChannel
        {
            get
            {
                return _remoteChannel;
            }

            set
            {
                this._remoteChannel = value;
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
                    throw new ArgumentException("Next-outgoing-id id can't be assigned a null value");

                this._nextOutgoingId = value;
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
                    throw new ArgumentException("Incoming-window id can't be assigned a null value");

                this._incomingWindow = value;
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
                    throw new ArgumentException("Outgoing-window id can't be assigned a null value");

                this._outgoingWindow = value;
            }
        }

        public Int64? HandleMax
        {
            get
            {
                return _handleMax;
            }

            set
            {
                this._handleMax = value;
            }
        }

        public List<AMQPSymbol> OfferedCapabilities
        {
            get
            {
                return _offeredCapabilities;
            }
        }

        public void addOfferedCapability(String[] capabilities)
        {
            if (_offeredCapabilities == null)
                _offeredCapabilities = new List<AMQPSymbol>();

            foreach (String capability in capabilities)
                _offeredCapabilities.Add(new AMQPSymbol(capability));
        }

        public List<AMQPSymbol> DesiredCapabilities
        {
            get
            {
                return _desiredCapabilities;
            }
        }

        public void addDesiredCapability(String[] capabilities)
        {
            if (_desiredCapabilities == null)
                _desiredCapabilities = new List<AMQPSymbol>();

            foreach (String capability in capabilities)
                _desiredCapabilities.Add(new AMQPSymbol(capability));
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
            return (int)HeaderCodes.BEGIN;
        }

        public override void ProcessBy(AMQPDevice device)
        {
            device.ProcessBegin();
        }

        #endregion
    }
}
