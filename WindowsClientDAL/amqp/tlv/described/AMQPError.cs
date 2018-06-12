

using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.constructors;
using com.mobius.software.windows.iotbroker.amqp.tlv.api;
using com.mobius.software.windows.iotbroker.amqp.tlv.array;
using com.mobius.software.windows.iotbroker.amqp.tlv.compound;
using com.mobius.software.windows.iotbroker.amqp.tlv.fixed_;
using com.mobius.software.windows.iotbroker.amqp.wrappers;
using com.mobius.software.windows.iotbroker.dal;
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
    public class AMQPError
    {
        #region private fields

        private ErrorCodes? _condition;
        private String _description;
        private Dictionary<AMQPSymbol, Object> _info;

        #endregion

        #region constructors



        #endregion

        #region public fields

        public TLVList getList()
        {
            TLVList list = new TLVList();

            if (_condition != null)
                list.addElement(0, AMQPWrapper<AMQPSymbol>.wrap(new AMQPSymbol(StringEnum.GetStringValue(_condition))));

            if (_description != null)
                list.addElement(1, AMQPWrapper<AMQPSymbol>.wrap(_description));

            if (_info != null)
                list.addElement(2, AMQPWrapper<AMQPSymbol>.wrapMap(_info));

            DescribedConstructor constructor = new DescribedConstructor(list.Code, new TLVFixed(AMQPType.SMALL_ULONG, new byte[] { 0x1D }));

            list.Constructor = constructor;

            return list;
        }

        public void fill(TLVList list)
        {
            if (list.getList().Count > 0)
            {
                TLVAmqp element = list.getList()[0];
                if (!element.isNull())
                    _condition = (ErrorCodes)StringEnum.Parse(typeof(ErrorCodes),AMQPUnwrapper<AMQPSymbol>.unwrapSymbol(element).Value);
            }

            if (list.getList().Count > 1)
            {
                TLVAmqp element = list.getList()[1];
                if (!element.isNull())
                    _description = AMQPUnwrapper<AMQPSymbol>.unwrapString(element);
            }

            if (list.getList().Count > 2)
            {
                TLVAmqp element = list.getList()[2];
                if (!element.isNull())
                    _info = AMQPUnwrapper<AMQPSymbol>.unwrapMap(element);
            }
        }

        public ErrorCodes? Condition
        {
            get
            {
                return _condition;
            }

            set
            {
                _condition = value;
            }
        }

        public String Description
        {
            get
            {
                return _description;
            }

            set
            {
                _description = value;
            }
        }

        public Dictionary<AMQPSymbol, Object> Info
        {
            get
            {
                return _info;
            }
        }

        public void addInfo(String key, Object value)
        {
            if (!key.StartsWith("x-"))
                throw new ArgumentException();
            if (_info == null)
                _info = new Dictionary<AMQPSymbol, object>();

            _info[new AMQPSymbol(key)] = value;
        }

        #endregion
    }
}