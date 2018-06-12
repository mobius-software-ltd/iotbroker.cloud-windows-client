

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
    public class SASLMechanisms : AMQPHeader
    {
        #region private fields

        private List<AMQPSymbol> _mechanisms;

        #endregion

        #region constructors

        public SASLMechanisms():base(HeaderCodes.MECHANISMS)
        { 
        }

        #endregion

        #region public fields

        public override TLVList getArguments()
        {
            TLVList list = new TLVList();

            if (_mechanisms == null)
                throw new MalformedMessageException("At least one SASL Mechanism must be specified");

            list.addElement(0, AMQPWrapper<AMQPSymbol>.wrapArray(_mechanisms));

            DescribedConstructor constructor = new DescribedConstructor(list.Code, new TLVFixed(AMQPType.SMALL_ULONG, new byte[] { 0x40 }));
            list.Constructor = constructor;

            return list;
        }

        public override void fillArguments(TLVList list)
        {
            if (list.getList().Count > 0)
            {
                TLVAmqp element = list.getList()[0];
                if (!element.isNull())
                    _mechanisms = AMQPUnwrapper<AMQPSymbol>.unwrapArray(element);
            }
        }

        public List<AMQPSymbol> Mechanisms
        {
            get
            {
                return _mechanisms;
            }
        }

        public void addMechanism(String value)
        {
            if (_mechanisms == null)
                _mechanisms = new List<AMQPSymbol>();

            if (value == null)
                throw new ArgumentException("Cannot add a null value to SASL Mechanisms");

            _mechanisms.Add(new AMQPSymbol(value));
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
            return (int)HeaderCodes.MECHANISMS;
        }

        public override void ProcessBy(AMQPDevice device)
        {
            device.ProcessSASLMechanism(Mechanisms,Channel,HeaderType);
        }

        #endregion
    }
}