

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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.amqp.headersasl
{
    public class SASLOutcome : AMQPHeader
    {
        #region private fields

        private byte[] _additionalData;
        private OutcomeCodes? _outcomeCode;

        #endregion

        #region constructors

        public SASLOutcome():base(HeaderCodes.OUTCOME)
        { 
        }

        #endregion

        #region public fields

        public override TLVList getArguments()
        {
            TLVList list = new TLVList();

            if (_outcomeCode == null)
                throw new MalformedMessageException("SASL-Outcome header's code can't be null");

            list.addElement(0, AMQPWrapper<AMQPSymbol>.wrap((Int32)_outcomeCode.Value));

            if (_additionalData != null)
                list.addElement(1, AMQPWrapper<AMQPSymbol>.wrap(_additionalData));

            DescribedConstructor constructor = new DescribedConstructor(list.Code,new TLVFixed(AMQPType.SMALL_ULONG, new byte[] { 0x44 }));
            list.Constructor = constructor;

            return list;
        }

        public override void fillArguments(TLVList list)
        {
            int size = list.getList().Count;

            if (size == 0)
                throw new MalformedMessageException("Received malformed SASL-Outcome header: code can't be null");

            if (size > 2)
                throw new MalformedMessageException("Received malformed SASL-Outcome header. Invalid number of arguments: " + size);

            if (size > 0)
            {
                TLVAmqp element = list.getList()[0];
                if (element.isNull())
                    throw new MalformedMessageException("Received malformed SASL-Outcome header: code can't be null");

                _outcomeCode = (OutcomeCodes)AMQPUnwrapper<AMQPSymbol>.unwrapUByte(element);
            }

            if (list.getList().Count > 1)
            {
                TLVAmqp element = list.getList()[1];
                if (!element.isNull())
                    _additionalData = AMQPUnwrapper<AMQPSymbol>.unwrapBinary(element);
            }
        }

        public OutcomeCodes? OutcomeCode
        {
            get
            {
                return _outcomeCode;
            }

            set
            {
                if (value == null)
                    throw new ArgumentException("Outcome-code can't be assigned a null value");

                _outcomeCode = value;
            }
        }

        public byte[] AdditionalData
        {
            get
            {
                return _additionalData;
            }

            set
            {
                _additionalData = value;
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
            return (int)HeaderCodes.OUTCOME;
        }

        public override void ProcessBy(AMQPDevice device)
        {
            device.ProcessSASLOutcome(AdditionalData,OutcomeCode);
        }

        #endregion
    }
}
