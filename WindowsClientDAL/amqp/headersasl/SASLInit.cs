

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
    public class SASLInit : AMQPHeader
    {
        #region private fields

        private AMQPSymbol _mechanism;
        private byte[] _initialResponse;
        private String _hostName;

        #endregion

        #region constructors

        public SASLInit():base(HeaderCodes.INIT)
        { 
        }

        #endregion

        #region public fields

        public override TLVList getArguments()
        {
            TLVList list = new TLVList();

            if (_mechanism == null)
                throw new MalformedMessageException("SASL-Init header's mechanism can't be null");

            list.addElement(0, AMQPWrapper<AMQPSymbol>.wrap(_mechanism));

            if (_initialResponse != null)
                list.addElement(1, AMQPWrapper<AMQPSymbol>.wrap(_initialResponse));

            if (_hostName != null)
                list.addElement(2, AMQPWrapper<AMQPSymbol>.wrap(_hostName));

            DescribedConstructor constructor = new DescribedConstructor(list.Code,new TLVFixed(AMQPType.SMALL_ULONG, new byte[] { 0x41 }));
            list.Constructor = constructor;

            return list;
        }

        public override void fillArguments(TLVList list)
        {
            int size = list.getList().Count;

            if (size == 0)
                throw new MalformedMessageException("Received malformed SASL-Init header: mechanism can't be null");

            if (size > 3)
                throw new MalformedMessageException("Received malformed SASL-Init header. Invalid number of arguments: " + size);

            if (size > 0)
            {
                TLVAmqp element = list.getList()[0];
                if (element.isNull())
                    throw new MalformedMessageException("Received malformed SASL-Init header: mechanism can't be null");

                _mechanism = AMQPUnwrapper<AMQPSymbol>.unwrapSymbol(element);
            }

            if (size > 1)
            {
                TLVAmqp element = list.getList()[1];
                if (!element.isNull())
                    _initialResponse = AMQPUnwrapper<AMQPSymbol>.unwrapBinary(element);
            }

            if (size > 2)
            {
                TLVAmqp element = list.getList()[2];
                if (!element.isNull())
                    _hostName = AMQPUnwrapper<AMQPSymbol>.unwrapString(element);
            }
        }

        public String Mechanism
        {
            get
            {
                return _mechanism.Value;
            }

            set
            {
                if (value == null)
                    throw new ArgumentException("Mechanism can't be assigned a null value");

                this._mechanism = new AMQPSymbol(value);
            }
        }

        public byte[] InitialResponse
        {
            get
            {
                return _initialResponse;
            }

            set
            {
                _initialResponse = value;
            }
        }

        public String HostName
        {
            get
            {
                return _hostName;
            }

            set
            {
                _hostName = value;
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
            return (int)HeaderCodes.INIT;
        }

        public override void ProcessBy(AMQPDevice device)
        {
            device.ProcessSASLInit(Mechanism, InitialResponse, HostName);
        }

        #endregion
    }
}
