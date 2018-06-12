

using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.constructors;
using com.mobius.software.windows.iotbroker.amqp.tlv.api;
using com.mobius.software.windows.iotbroker.amqp.tlv.array;
using com.mobius.software.windows.iotbroker.amqp.tlv.compound;
using com.mobius.software.windows.iotbroker.amqp.tlv.fixed_;
using com.mobius.software.windows.iotbroker.amqp.wrappers;
/**
* Mobius Software LTD
* Copyright 2015-2017, Mobius Software LTD
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
using com.mobius.software.windows.iotbroker.mqtt.avps;
using com.mobius.software.windows.iotbroker.mqtt.headers.api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.amqp.tlv.described
{
    public class AMQPReceived : AMQPState
    {
        #region private fields

        private Int64? _sectionNumber;
        private BigInteger _sectionOffset;

        #endregion

        #region constructors



        #endregion

        #region public fields

        public TLVList getList()
        {
            TLVList list = new TLVList();

            if (_sectionNumber != null)
                list.addElement(0, AMQPWrapper<AMQPSymbol>.wrap(_sectionNumber));
            if (_sectionOffset != null)
                list.addElement(1, AMQPWrapper<AMQPSymbol>.wrap(_sectionOffset));

            DescribedConstructor constructor = new DescribedConstructor(list.Code, new TLVFixed(AMQPType.SMALL_ULONG, new byte[] { 0x23 }));
            list.Constructor=constructor;

            return list;
        }

        public void fill(TLVList list)
        {
            if (list.getList().Count > 0)
            {
                TLVAmqp element = list.getList()[0];
                if (!element.isNull())
                    _sectionNumber = AMQPUnwrapper<AMQPSymbol>.unwrapUInt(element);
            }
            if (list.getList().Count > 1)
            {
                TLVAmqp element = list.getList()[1];
                if (!element.isNull())
                    _sectionOffset = AMQPUnwrapper<AMQPSymbol>.unwrapULong(element);
            }
        }

        public Int64? SectionNumber
        {
            get
            {
                return _sectionNumber;
            }

            set
            {
                _sectionNumber = value;
            }
        }

        public BigInteger SectionOffset
        {
            get
            {
                return _sectionOffset;
            }

            set
            {
                _sectionOffset = value;
            }
        }
        
        #endregion
    }
}