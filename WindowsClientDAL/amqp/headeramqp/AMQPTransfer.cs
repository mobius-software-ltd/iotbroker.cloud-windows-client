


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
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.amqp.headeramqp
{
    public class AMQPTransfer : AMQPHeader
    {
        #region private fields

        private Int64? _handle;
        private Int64? _deliveryId;
        private byte[] _deliveryTag;
        private AMQPMessageFormat _messageFormat;
        private Boolean? _settled;
        private Boolean? _more;
        private ReceiveCodes? _rcvSettleMode;
        private AMQPState _state;
        private Boolean? _resume;
        private Boolean? _aborted;
        private Boolean? _batchable;
        private Dictionary<SectionCodes, AMQPSection> _sections;

        #endregion

        #region constructors

        public AMQPTransfer():base(HeaderCodes.TRANSFER)
        { 
        }

        #endregion

        #region public fields

        public override TLVList getArguments()
        {
            TLVList list = new TLVList();

            if (_handle == null)
                throw new MalformedMessageException("Transfer header's handle can't be null");

            list.addElement(0, AMQPWrapper<SectionCodes>.wrap(_handle));

            if (_deliveryId != null)
                list.addElement(1, AMQPWrapper<SectionCodes>.wrap(_deliveryId));

            if (_deliveryTag != null)
                list.addElement(2, AMQPWrapper<SectionCodes>.wrap(_deliveryTag));

            if (_messageFormat != null)
                list.addElement(3, AMQPWrapper<SectionCodes>.wrap(_messageFormat.encode()));

            if (_settled != null)
                list.addElement(4, AMQPWrapper<SectionCodes>.wrap(_settled));

            if (_more != null)
                list.addElement(5, AMQPWrapper<SectionCodes>.wrap(_more));

            if (_rcvSettleMode != null)
                list.addElement(6, AMQPWrapper<SectionCodes>.wrap((Int32)_rcvSettleMode.Value));

            if (_state != null)
                list.addElement(7, _state.getList());

            if (_resume != null)
                list.addElement(8, AMQPWrapper<SectionCodes>.wrap(_resume));

            if (_aborted != null)
                list.addElement(9, AMQPWrapper<SectionCodes>.wrap(_aborted));

            if (_batchable != null)
                list.addElement(10, AMQPWrapper<SectionCodes>.wrap(_batchable));

            DescribedConstructor constructor = new DescribedConstructor(list.Code, new TLVFixed(AMQPType.SMALL_ULONG, new byte[] { (byte)_code }));
            list.Constructor = constructor;

            return list;
        }

        public override void fillArguments(TLVList list)
        {
            int size = list.getList().Count;

            if (size == 0)
                throw new MalformedMessageException("Received malformed Transfer header: handle can't be null");

            if (size > 11)
                throw new MalformedMessageException("Received malformed Transfer header. Invalid number of arguments: " + size);

            if (size > 0)
            {
                TLVAmqp element = list.getList()[0];
                if (element.isNull())
                    throw new MalformedMessageException("Received malformed Transfer header: handle can't be null");

                _handle = AMQPUnwrapper<SectionCodes>.unwrapUInt(element);
            }

            if (size > 1)
            {
                TLVAmqp element = list.getList()[1];
                if (!element.isNull())
                    _deliveryId = AMQPUnwrapper<SectionCodes>.unwrapUInt(element);
            }

            if (size > 2)
            {
                TLVAmqp element = list.getList()[2];
                if (!element.isNull())
                    _deliveryTag = AMQPUnwrapper<SectionCodes>.unwrapBinary(element);
            }

            if (size > 3)
            {
                TLVAmqp element = list.getList()[3];
                if (!element.isNull())
                    _messageFormat = new AMQPMessageFormat(AMQPUnwrapper<SectionCodes>.unwrapUInt(element));
            }
            if (size > 4)
            {
                TLVAmqp element = list.getList()[4];
                if (!element.isNull())
                    _settled = AMQPUnwrapper<SectionCodes>.unwrapBool(element);
            }

            if (size > 5)
            {
                TLVAmqp element = list.getList()[5];
                if (!element.isNull())
                    _more = AMQPUnwrapper<SectionCodes>.unwrapBool(element);
            }

            if (size > 6)
            {
                TLVAmqp element = list.getList()[6];
                if (!element.isNull())
                    _rcvSettleMode = (ReceiveCodes)(AMQPUnwrapper<SectionCodes>.unwrapUByte(element));
            }

            if (size > 7)
            {
                TLVAmqp element = list.getList()[7];
                if (!element.isNull())
                {
                    AMQPType code = element.Code;
                    if (code != AMQPType.LIST_0 && code != AMQPType.LIST_8 && code != AMQPType.LIST_32)
                        throw new MalformedMessageException("Expected type 'STATE' - received: " + element.Code);

                    _state = AMQPFactory.getState((TLVList)element);
                    _state.fill((TLVList)element);
                }
            }

            if (size > 8)
            {
                TLVAmqp element = list.getList()[8];
                if (!element.isNull())
                    _resume = AMQPUnwrapper<SectionCodes>.unwrapBool(element);
            }

            if (size > 9)
            {
                TLVAmqp element = list.getList()[9];
                if (!element.isNull())
                    _aborted = AMQPUnwrapper<SectionCodes>.unwrapBool(element);
            }
            if (size > 10)
            {
                TLVAmqp element = list.getList()[10];
                if (!element.isNull())
                    _batchable = AMQPUnwrapper<SectionCodes>.unwrapBool(element);
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

        public Int64? DeliveryId
        {
            get
            {
                return _deliveryId;
            }

            set
            {
                this._deliveryId = value;
            }
        }

        public byte[] DeliveryTag
        {
            get
            {
                return _deliveryTag;
            }

            set
            {
                this._deliveryTag = value;
            }
        }

        public AMQPMessageFormat MessageFormat
        {
            get
            {
                return _messageFormat;
            }

            set
            {
                this._messageFormat = value;
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

        public Boolean? More
        {
            get
            {
                return _more;
            }

            set
            {
                this._more = value;
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

        public Boolean? Resume
        {
            get
            {
                return _resume;
            }

            set
            {
                this._resume = value;
            }
        }

        public Boolean? Aborted
        {
            get
            {
                return _aborted;
            }

            set
            {
                this._aborted = value;
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

        public AMQPSection getHeader()
        {
            return _sections[SectionCodes.HEADER];
        }

        public AMQPSection getDeliveryAnnotations()
        {
            return _sections[SectionCodes.DELIVERY_ANNOTATIONS];
        }

        public AMQPSection getMessageAnnotations()
        {
            return _sections[SectionCodes.MESSAGE_ANNOTATIONS];
        }

        public AMQPSection getProperties()
        {
            return _sections[SectionCodes.PROPERTIES];
        }

        public AMQPSection getApplicationProperties()
        {
            return _sections[SectionCodes.APPLICATION_PROPERTIES];
        }

        public AMQPSection getData()
        {
            return _sections[SectionCodes.DATA];
        }

        public AMQPSection getSequence()
        {
            return _sections[SectionCodes.SEQUENCE];
        }

        public AMQPSection getValue()
        {
            return _sections[SectionCodes.VALUE];
        }

        public AMQPSection getFooter()
        {
            return _sections[SectionCodes.FOOTER];
        }

        public Dictionary<SectionCodes, AMQPSection> getSections()
        {
            return _sections;
        }

        public void addSections(AMQPSection[] values)
        {
            if (_sections == null)
                _sections = new Dictionary<SectionCodes, AMQPSection>();

            foreach (AMQPSection value in values)
                _sections[value.getCode()] = value;
        }

        public override int getLength()
        {
            int length = 8;
            TLVAmqp arguments = this.getArguments();
            length += arguments.getLength();

            foreach (KeyValuePair<SectionCodes,AMQPSection> item in _sections)
                length += item.Value.getValue().getLength();
            
            return length;
        }

        public override int getType()
        {
            return (int)HeaderCodes.TRANSFER;
        }

        public override void ProcessBy(AMQPDevice device)
        {
            device.ProcessTransfer((AMQPData)getData(),Handle,Settled,DeliveryId);
        }

        #endregion
    }
}
