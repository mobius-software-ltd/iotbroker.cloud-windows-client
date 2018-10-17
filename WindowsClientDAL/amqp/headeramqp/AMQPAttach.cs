

using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.constructors;
using com.mobius.software.windows.iotbroker.amqp.headerapi;
using com.mobius.software.windows.iotbroker.amqp.terminus;
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
    public class AMQPAttach: AMQPHeader
    {
        #region private fields

        private String _name;
        private Int64? _handle;
        private RoleCodes? _role;
        private SendCodes? _sndSettleMode;
        private ReceiveCodes? _rcvSettleMode;
        private AMQPSource _source;
        private AMQPTarget _target;
        private Dictionary<AMQPSymbol, Object> _unsettled;
        private Boolean? _incompleteUnsettled;
        private Int64? _initialDeliveryCount;
        private BigInteger _maxMessageSize;
        private List<AMQPSymbol> _offeredCapabilities;
        private List<AMQPSymbol> _desiredCapabilities;
        private Dictionary<AMQPSymbol, Object> _properties;

        #endregion

        #region constructors

        public AMQPAttach():base(HeaderCodes.ATTACH)
        { 
        }

        #endregion

        #region public fields

        public override TLVList getArguments()
        {

            TLVList list = new TLVList();

            if (_name == null)
                throw new MalformedMessageException("Attach header's name can't be null");
            list.addElement(0, AMQPWrapper<AMQPSymbol>.wrapString(_name));

            if (_handle == null)
                throw new MalformedMessageException("Attach header's handle can't be null");

            list.addElement(1, AMQPWrapper<AMQPSymbol>.wrap(_handle));

            if (!_role.HasValue)
                throw new MalformedMessageException("Attach header's role can't be null");

            if(_role.Value==RoleCodes.RECEIVER)
                list.addElement(2, AMQPWrapper<AMQPSymbol>.wrap(true));
            else
                list.addElement(2, AMQPWrapper<AMQPSymbol>.wrap(false));

            if (!_sndSettleMode.HasValue)
                list.addElement(3, AMQPWrapper<AMQPSymbol>.wrap(_sndSettleMode));

            if (!_rcvSettleMode.HasValue)
                list.addElement(4, AMQPWrapper<AMQPSymbol>.wrap(_rcvSettleMode));

            if (_source != null)
                list.addElement(5, _source.getList());

            if (_target != null)
                list.addElement(6, _target.getList());

            if (_unsettled != null)
                list.addElement(7, AMQPWrapper<AMQPSymbol>.wrapMap(_unsettled));

            if (!_incompleteUnsettled.HasValue)
                list.addElement(8, AMQPWrapper<AMQPSymbol>.wrap(_incompleteUnsettled));

            if (!_initialDeliveryCount.HasValue)
                list.addElement(9, AMQPWrapper<AMQPSymbol>.wrap(_initialDeliveryCount));
            else if (_role.Value==RoleCodes.SENDER)
                throw new MalformedMessageException("Sender's attach header must contain a non-null initial-delivery-count value");

            if (_maxMessageSize != null)
                list.addElement(10, AMQPWrapper<AMQPSymbol>.wrap(_maxMessageSize));

            if (_offeredCapabilities != null)
                list.addElement(11, AMQPWrapper<AMQPSymbol>.wrapArray(_offeredCapabilities));

            if (_desiredCapabilities != null)
                list.addElement(12, AMQPWrapper<AMQPSymbol>.wrapArray(_desiredCapabilities));

            if (_properties != null)
                list.addElement(13, AMQPWrapper<AMQPSymbol>.wrapMap(_properties));

            DescribedConstructor constructor = new DescribedConstructor(list.Code,new TLVFixed(AMQPType.SMALL_ULONG, new byte[] { (byte)_code }));

            list.Constructor = constructor;

            return list;
        }

        public override void fillArguments(TLVList list)
        {

            int size = list.getList().Count;

            if (size < 3)
                throw new MalformedMessageException("Received malformed Attach header: mandatory " + "fields name, handle and role must not be null");

            if (size > 14)
                throw new MalformedMessageException("Received malformed Attach header. Invalid number of arguments: " + size);

            if (size > 0)
            {
                TLVAmqp element = list.getList()[0];
                if (element.isNull())
                    throw new MalformedMessageException("Received malformed Attach header: name can't be null");

                _name = AMQPUnwrapper<AMQPSymbol>.unwrapString(element);
            }

            if (size > 1)
            {
                TLVAmqp element = list.getList()[1];
                if (element.isNull())
                    throw new MalformedMessageException("Received malformed Attach header: handle can't be null");

                _handle = AMQPUnwrapper<AMQPSymbol>.unwrapUInt(element);
            }

            if (size > 2)
            {
                TLVAmqp element = list.getList()[2];
                if (element.isNull())
                    throw new MalformedMessageException("Received malformed Attach header: role can't be null");

                Boolean value = AMQPUnwrapper<AMQPSymbol>.unwrapBool(element);
                if (value)
                    _role = RoleCodes.RECEIVER;
                else
                    _role = RoleCodes.SENDER;
            }

            if (size > 3)
            {
                TLVAmqp element = list.getList()[3];
                if (!element.isNull())
                    _sndSettleMode = (SendCodes)AMQPUnwrapper<AMQPSymbol>.unwrapUByte(element);
            }

            if (size > 4)
            {
                TLVAmqp element = list.getList()[4];
                if (!element.isNull())
                    _rcvSettleMode = (ReceiveCodes)AMQPUnwrapper<AMQPSymbol>.unwrapUByte(element);
            }

            if (size > 5)
            {
                TLVAmqp element = list.getList()[5];
                if (!element.isNull())
                {
                    AMQPType code = element.Code;
                    if (code != AMQPType.LIST_0 && code != AMQPType.LIST_8 && code != AMQPType.LIST_32)
                        throw new MalformedMessageException("Expected type SOURCE - received: " + element.Code);

                    _source = new AMQPSource();
                    _source.fill((TLVList)element);
                }
            }

            if (size > 6)
            {
                TLVAmqp element = list.getList()[6];
                if (!element.isNull())
                {
                    AMQPType code = element.Code;
                    if (code != AMQPType.LIST_0 && code != AMQPType.LIST_8 && code != AMQPType.LIST_32)
                        throw new MalformedMessageException("Expected type TARGET - received: " + element.Code);

                    _target = new AMQPTarget();
                    _target.fill((TLVList)element);
                }
            }

            if (size > 7)
            {
                TLVAmqp unsettledMap = list.getList()[7];
                if (!unsettledMap.isNull())
                    _unsettled = AMQPUnwrapper<AMQPSymbol>.unwrapMap(unsettledMap);
            }

            if (size > 8)
            {
                TLVAmqp element = list.getList()[8];
                if (!element.isNull())
                    _incompleteUnsettled = AMQPUnwrapper<AMQPSymbol>.unwrapBool(element);
            }

            if (size > 9)
            {
                TLVAmqp element = list.getList()[9];
                if (!element.isNull())
                    _initialDeliveryCount = AMQPUnwrapper<AMQPSymbol>.unwrapUInt(element);
                else if (_role==RoleCodes.SENDER)
                    throw new MalformedMessageException("Received an attach header with a null initial-delivery-count");
            }

            if (size > 10)
            {
                TLVAmqp element = list.getList()[10];
                if (!element.isNull())
                    _maxMessageSize = AMQPUnwrapper<AMQPSymbol>.unwrapULong(element);
            }

            if (size > 11)
            {
                TLVAmqp element = list.getList()[11];
                if (!element.isNull())
                    _offeredCapabilities = AMQPUnwrapper<AMQPSymbol>.unwrapArray(element);
            }

            if (size > 12)
            {
                TLVAmqp element = list.getList()[12];
                if (!element.isNull())
                    _desiredCapabilities = AMQPUnwrapper<AMQPSymbol>.unwrapArray(element);
            }

            if (size > 13)
            {
                TLVAmqp element = list.getList()[13];
                if (!element.isNull())
                    _properties = AMQPUnwrapper<AMQPSymbol>.unwrapMap(element);
            }
        }

        public String Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (value == null)
                    throw new ArgumentException("Name can't be assigned a null value");

                this._name = value;
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

        public SendCodes? SndSettleMode
        {
            get
            {
                return _sndSettleMode;
            }

            set
            {
                this._sndSettleMode = value;
            }
        }

        public ReceiveCodes? RcvSettleMode
        {
            get
            {
                return _rcvSettleMode;
            }

            set
            {
                this._rcvSettleMode = value;
            }
        }

        public AMQPSource Source
        {
            get
            {
                return _source;
            }

            set
            {
                this._source = value;
            }
        }

        public AMQPTarget Target
        {
            get
            {
                return _target;
            }

            set
            {
                this._target = value;
            }
        }

        public Dictionary<AMQPSymbol, Object> Unsettled
        {
            get
            {
                return _unsettled;
            }
        }

        public void addUnsettled(String key, Object value)
        {
            if (key == null)
                throw new ArgumentException("Unsettled map cannot contain objects with null keys");

            if (_unsettled == null)
                _unsettled = new Dictionary<AMQPSymbol, Object>();

            _unsettled[new AMQPSymbol(key)] = value;
        }

        public Boolean? IncompleteUnsettled
        {
            get
            {
                return _incompleteUnsettled;
            }

            set
            {
                this._incompleteUnsettled = value;
            }
        }

        public Int64? InitialDeliveryCount
        {
            get
            {
                return _initialDeliveryCount;
            }

            set
            {
                this._initialDeliveryCount = value;
            }
        }

        public BigInteger MaxMessageSize
        {
            get
            {
                return _maxMessageSize;
            }

            set
            {
                this._maxMessageSize = value;
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
            return (int)HeaderCodes.ATTACH;
        }

        public override void ProcessBy(AMQPDevice device)
        {
            device.ProcessAttach(Name,Role,Handle);
        }

        #endregion
    }
}
