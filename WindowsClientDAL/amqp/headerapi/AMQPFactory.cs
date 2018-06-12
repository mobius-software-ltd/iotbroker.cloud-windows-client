

using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.headeramqp;
using com.mobius.software.windows.iotbroker.amqp.headersasl;
using com.mobius.software.windows.iotbroker.amqp.sections;
using com.mobius.software.windows.iotbroker.amqp.tlv.api;
using com.mobius.software.windows.iotbroker.amqp.tlv.compound;
using com.mobius.software.windows.iotbroker.amqp.tlv.described;
using com.mobius.software.windows.iotbroker.dal;
using com.mobius.software.windows.iotbroker.mqtt.exceptions;
using DotNetty.Buffers;
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
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.amqp.headerapi
{
    public class AMQPFactory
    {
        #region private fields

        #endregion

        #region constructors

        #endregion

        #region public fields

        public static AMQPHeader getAMQP(IByteBuffer buf)
        {
            TLVAmqp list = TLVFactory.getTlv(buf);
            if (list.Code != AMQPType.LIST_0 && list.Code != AMQPType.LIST_8 && list.Code != AMQPType.LIST_32)
                throw new MalformedMessageException("Received amqp-header with malformed arguments");

            AMQPHeader header = null;

            Byte byteCode = list.Constructor.getDescriptorCode().Value;
            HeaderCodes code = (HeaderCodes)byteCode;
            switch (code)
            {
                case HeaderCodes.ATTACH:
                    header = new AMQPAttach();
                    break;
                case HeaderCodes.BEGIN:
                    header = new AMQPBegin();
                    break;
                case HeaderCodes.CLOSE:
                    header = new AMQPClose();
                    break;
                case HeaderCodes.DETACH:
                    header = new AMQPDetach();
                    break;
                case HeaderCodes.DISPOSITION:
                    header = new AMQPDisposition();
                    break;
                case HeaderCodes.END:
                    header = new AMQPEnd();
                    break;
                case HeaderCodes.FLOW:
                    header = new AMQPFlow();
                    break;
                case HeaderCodes.OPEN:
                    header = new AMQPOpen();
                    break;
                case HeaderCodes.TRANSFER:
                    header = new AMQPTransfer();
                    break;
                default:
                    throw new MalformedMessageException("Received amqp-header with unrecognized performative");
            }

            header.fillArguments((TLVList)list);

            return header;
        }

        public static AMQPHeader getSASL(IByteBuffer buf)
        {

            TLVAmqp list = TLVFactory.getTlv(buf);
            if (list.Code != AMQPType.LIST_0 && list.Code != AMQPType.LIST_8 && list.Code != AMQPType.LIST_32)
                throw new MalformedMessageException("Received amqp-header with malformed arguments");

            AMQPHeader header = null;

            Byte byteCode = list.Constructor.getDescriptorCode().Value;
            HeaderCodes code = (HeaderCodes)byteCode;
            switch (code)
            {
                case HeaderCodes.CHALLENGE:
                    header = new SASLChallenge();
                    break;
                case HeaderCodes.INIT:
                    header = new SASLInit();
                    break;
                case HeaderCodes.MECHANISMS:
                    header = new SASLMechanisms();
                    break;
                case HeaderCodes.OUTCOME:
                    header = new SASLOutcome();
                    break;
                case HeaderCodes.RESPONSE:
                    header = new SASLResponse();
                    break;
                default:
                    throw new MalformedMessageException("Received sasl-header with unrecognized arguments code");
            }

            header.fillArguments((TLVList)list);

            return header;

        }

        public static AMQPSection getSection(IByteBuffer buf)
        {

            TLVAmqp value = TLVFactory.getTlv(buf);

            AMQPSection section = null;

            Byte byteCode = value.Constructor.getDescriptorCode().Value;
            SectionCodes code = (SectionCodes)byteCode;
            switch (code)
            {
                case SectionCodes.APPLICATION_PROPERTIES:
                    section = new ApplicationProperties();
                    break;
                case SectionCodes.DATA:
                    section = new AMQPData();
                    break;
                case SectionCodes.DELIVERY_ANNOTATIONS:
                    section = new DeliveryAnnotations();
                    break;
                case SectionCodes.FOOTER:
                    section = new AMQPFooter();
                    break;
                case SectionCodes.HEADER:
                    section = new MessageHeader();
                    break;
                case SectionCodes.MESSAGE_ANNOTATIONS:
                    section = new MessageAnnotations();
                    break;
                case SectionCodes.PROPERTIES:
                    section = new AMQPProperties();
                    break;
                case SectionCodes.SEQUENCE:
                    section = new AMQPSequence();
                    break;
                case SectionCodes.VALUE:
                    section = new AMQPValue();
                    break;
                default:
                    throw new MalformedMessageException("Received header with unrecognized message section code");
            }

            section.fill(value);

            return section;
        }

        public static AMQPState getState(TLVList list)
        {

            AMQPState state = null;

            Byte byteCode = list.Constructor.getDescriptorCode().Value;
            StateCodes code = (StateCodes)byteCode;
            switch (code)
            {
                case StateCodes.ACCEPTED:
                    state = new AMQPAccepted();
                    break;
                case StateCodes.MODIFIED:
                    state = new AMQPModified();
                    break;
                case StateCodes.RECEIVED:
                    state = new AMQPReceived();
                    break;
                case StateCodes.REJECTED:
                    state = new AMQPRejected();
                    break;
                case StateCodes.RELEASED:
                    state = new AMQPReleased();
                    break;
                default:
                    throw new MalformedMessageException("Received header with unrecognized state code");
            }

            return state;
        }

        public static AMQPOutcome getOutcome(TLVList list)
        {

            AMQPOutcome outcome = null;

            Byte byteCode = list.Constructor.getDescriptorCode().Value;
            StateCodes code = (StateCodes)byteCode;
            switch (code)
            {
                case StateCodes.ACCEPTED:
                    outcome = new AMQPAccepted();
                    break;
                case StateCodes.MODIFIED:
                    outcome = new AMQPModified();
                    break;
                case StateCodes.REJECTED:
                    outcome = new AMQPRejected();
                    break;
                case StateCodes.RELEASED:
                    outcome = new AMQPReleased();
                    break;
                default:
                    throw new MalformedMessageException("Received header with unrecognized outcome code");
            }
            return outcome;
        }

        #endregion
    }
}