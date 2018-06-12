

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
    public class AMQPSequence : AMQPSection
    {
        #region private fields

        private List<Object> _sequence;

        #endregion

        #region constructors

        #endregion

        #region public fields

        public TLVAmqp getValue()
        {
            TLVList list = new TLVList();

            if (_sequence != null)
                list = AMQPWrapper<Object>.wrapList(_sequence);

            DescribedConstructor constructor = new DescribedConstructor(list.Code, new TLVFixed(AMQPType.SMALL_ULONG, new byte[] { 0x76 }));
            list.Constructor=constructor;

            return list;
        }

        public void fill(TLVAmqp list)
        {
            if (!list.isNull())
                _sequence = AMQPUnwrapper<Object>.unwrapList(list);
        }

        public SectionCodes getCode()
        {
            return SectionCodes.SEQUENCE;
        }

        public List<Object> Sequence
        {
            get
            {
                return _sequence;
            }

            set
            {
                _sequence = value;
            }
        }

        #endregion
    }
}
