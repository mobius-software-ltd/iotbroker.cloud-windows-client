

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
    public class AMQPData : AMQPSection
    {
        #region private fields

        private byte[] _data;

        #endregion

        #region constructors

        #endregion

        #region public fields

        public TLVAmqp getValue()
        {

            TLVAmqp bin = null;
            if (_data != null)
                bin = AMQPWrapper<AMQPSymbol>.wrap(_data);
            else
                bin = new TLVNull();

            DescribedConstructor constructor = new DescribedConstructor(bin.Code, new TLVFixed(AMQPType.SMALL_ULONG, new byte[] { 0x75 }));
            bin.Constructor = constructor;

            return bin;
        }

        public void fill(TLVAmqp value)
        {
            if (!value.isNull())
                _data = AMQPUnwrapper<AMQPSymbol>.unwrapBinary(value);
        }

        public SectionCodes getCode()
        {
            return SectionCodes.DATA;
        }

        public byte[] Data
        {
            get
            {
                return _data;
            }

            set
            {
                _data = value;
            }
        }

        #endregion
    }
}
