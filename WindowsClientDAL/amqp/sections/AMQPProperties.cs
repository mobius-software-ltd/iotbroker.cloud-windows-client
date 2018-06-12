

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
    public class AMQPProperties : AMQPSection
    {
        #region private fields

        private MessageID _messageId;
        private byte[] _userId;
        private String _to;
        private String _subject;
        private String _replyTo;
        private byte[] _correlationId;
        private String _contentType;
        private String _contentEncoding;
        private DateTime? _absoluteExpiryTime;
        private DateTime? _creationTime;
        private String _groupId;
        private Int64? _groupSequence;
        private String _replyToGroupId;

        #endregion

        #region constructors

        #endregion

        #region public fields

        public TLVAmqp getValue()
        {
            TLVList list = new TLVList();

            if (_messageId != null)
            {
                Object value = null;
                if (_messageId.getBinary() != null)
                    value = _messageId.getBinary();
                else if (_messageId.getLong() != null)
                    value = _messageId.getLong();
                else if (_messageId.getString() != null)
                    value = _messageId.getString();
                else if (_messageId.getUuid() != null)
                    value = _messageId.getUuid();
                list.addElement(0, AMQPWrapper<AMQPSymbol>.wrap(value));
            }

            if (_userId != null)
                list.addElement(1, AMQPWrapper<AMQPSymbol>.wrap(_userId));

            if (_to != null)
                list.addElement(2, AMQPWrapper<AMQPSymbol>.wrap(_to));

            if (_subject != null)
                list.addElement(3, AMQPWrapper<AMQPSymbol>.wrap(_subject));

            if (_replyTo != null)
                list.addElement(4, AMQPWrapper<AMQPSymbol>.wrap(_replyTo));

            if (_correlationId != null)
                list.addElement(5, AMQPWrapper<AMQPSymbol>.wrap(_correlationId));

            if (_contentType != null)
                list.addElement(6, AMQPWrapper<AMQPSymbol>.wrap(_contentType));

            if (_contentEncoding != null)
                list.addElement(7, AMQPWrapper<AMQPSymbol>.wrap(_contentEncoding));

            if (_absoluteExpiryTime != null)
                list.addElement(8, AMQPWrapper<AMQPSymbol>.wrap(_absoluteExpiryTime));

            if (_creationTime != null)
                list.addElement(9, AMQPWrapper<AMQPSymbol>.wrap(_creationTime));

            if (_groupId != null)
                list.addElement(10, AMQPWrapper<AMQPSymbol>.wrap(_groupId));

            if (_groupSequence != null)
                list.addElement(11, AMQPWrapper<AMQPSymbol>.wrap(_groupSequence));

            if (_replyToGroupId != null)
                list.addElement(12, AMQPWrapper<AMQPSymbol>.wrap(_replyToGroupId));

            DescribedConstructor constructor = new DescribedConstructor(list.Code, new TLVFixed(AMQPType.SMALL_ULONG, new byte[] { 0x73 }));
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
                {
                    switch (element.Code)
                    {
                        case AMQPType.ULONG_0:
                        case AMQPType.SMALL_ULONG:
                        case AMQPType.ULONG:
                            _messageId = new LongID(AMQPUnwrapper<AMQPSymbol>.unwrapULong(element));
                            break;
                        case AMQPType.STRING_8:
                        case AMQPType.STRING_32:
                            _messageId = new StringID(AMQPUnwrapper<AMQPSymbol>.unwrapString(element));
                            break;
                        case AMQPType.BINARY_8:
                        case AMQPType.BINARY_32:
                            _messageId = new BinaryID(AMQPUnwrapper<AMQPSymbol>.unwrapBinary(element));
                            break;
                        case AMQPType.UUID:
                            _messageId = new UuidID(AMQPUnwrapper<AMQPSymbol>.unwrapUuid(element));
                            break;
                        default:
                            throw new ArgumentException("Expected type 'MessageID' - received: " + element.Code);
                    }
                }
            }
            if (list.getList().Count > 1)
            {
                TLVAmqp element = list.getList()[1];
                if (!element.isNull())
                    _userId = AMQPUnwrapper<AMQPSymbol>.unwrapBinary(element);
            }
            if (list.getList().Count > 2)
            {
                TLVAmqp element = list.getList()[2];
                if (!element.isNull())
                    _to = AMQPUnwrapper<AMQPSymbol>.unwrapString(element);
            }
            if (list.getList().Count > 3)
            {
                TLVAmqp element = list.getList()[3];
                if (!element.isNull())
                    _subject = AMQPUnwrapper<AMQPSymbol>.unwrapString(element);
            }
            if (list.getList().Count > 4)
            {
                TLVAmqp element = list.getList()[4];
                if (!element.isNull())
                    _replyTo = AMQPUnwrapper<AMQPSymbol>.unwrapString(element);
            }
            if (list.getList().Count > 5)
            {
                TLVAmqp element = list.getList()[5];
                if (!element.isNull())
                    _correlationId = AMQPUnwrapper<AMQPSymbol>.unwrapBinary(element);
            }
            if (list.getList().Count > 6)
            {
                TLVAmqp element = list.getList()[6];
                if (!element.isNull())
                    _contentType = AMQPUnwrapper<AMQPSymbol>.unwrapString(element);
            }
            if (list.getList().Count > 7)
            {
                TLVAmqp element = list.getList()[7];
                if (!element.isNull())
                    _contentEncoding = AMQPUnwrapper<AMQPSymbol>.unwrapString(element);
            }
            if (list.getList().Count > 8)
            {
                TLVAmqp element = list.getList()[8];
                if (!element.isNull())
                    _absoluteExpiryTime = AMQPUnwrapper<AMQPSymbol>.unwrapTimestamp(element);
            }
            if (list.getList().Count > 9)
            {
                TLVAmqp element = list.getList()[9];
                if (!element.isNull())
                    _creationTime = AMQPUnwrapper<AMQPSymbol>.unwrapTimestamp(element);
            }
            if (list.getList().Count > 10)
            {
                TLVAmqp element = list.getList()[10];
                if (!element.isNull())
                    _groupId = AMQPUnwrapper<AMQPSymbol>.unwrapString(element);
            }
            if (list.getList().Count > 11)
            {
                TLVAmqp element = list.getList()[11];
                if (!element.isNull())
                    _groupSequence = AMQPUnwrapper<AMQPSymbol>.unwrapUInt(element);
            }
            if (list.getList().Count > 12)
            {
                TLVAmqp element = list.getList()[12];
                if (!element.isNull())
                    _replyToGroupId = AMQPUnwrapper<AMQPSymbol>.unwrapString(element);
            }
        }

        public SectionCodes getCode()
        {
            return SectionCodes.PROPERTIES;
        }

        public MessageID MessageId
        {
            get
            {
                return _messageId;
            }

            set
            {
                _messageId = value;
            }
        }

        public byte[] UserId
        {
            get
            {
                return _userId;
            }

            set
            {
                _userId = value;
            }
        }

        public String To
        {
            get
            {
                return _to;
            }

            set
            {
                _to = value;
            }
        }

        public String Subject
        {
            get
            {
                return _subject;
            }

            set
            {
                _subject = value;
            }
        }

        public String ReplyTo
        {
            get
            {
                return _replyTo;
            }

            set
            {
                _replyTo = value;
            }
        }

        public byte[] CorrelationId
        {
            get
            {
                return _correlationId;
            }

            set
            {
                _correlationId = value;
            }
        }

        public String ContentType
        {
            get
            {
                return _contentType;
            }

            set
            {
                _contentType = value;
            }
        }

        public String ContentEncoding
        {
            get
            {
                return _contentEncoding;
            }

            set
            {
                _contentEncoding = value;
            }
        }

        public DateTime? AbsoluteExpiryTime
        {
            get
            {
                return _absoluteExpiryTime;
            }

            set
            {
                _absoluteExpiryTime = value;
            }
        }

        public DateTime? CreationTime
        {
            get
            {
                return _creationTime;
            }

            set
            {
                _creationTime = value;
            }
        }

        public String GroupId
        {
            get
            {
                return _groupId;
            }

            set
            {
                _groupId = value;
            }
        }

        public Int64? GroupSequence
        {
            get
            {
                return _groupSequence;
            }

            set
            {
                _groupSequence = value;
            }
        }

        public String ReplyToGroupId
        {
            get
            {
                return _replyToGroupId;
            }

            set
            {
                _replyToGroupId = value;
            }
        }
                
        #endregion
    }
}
