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
using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.constructors;
using com.mobius.software.windows.iotbroker.amqp.tlv.api;
using com.mobius.software.windows.iotbroker.amqp.tlv.array;

using com.mobius.software.windows.iotbroker.mqtt.avps;
using com.mobius.software.windows.iotbroker.mqtt.headers.api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.amqp.tlv.fixed_
{
    public class TLVFixed : TLVAmqp
    {
        #region private fields

        private byte[] _value;

        #endregion

        #region constructors

        public TLVFixed(AMQPType code, byte[] value): base(new SimpleConstructor(code))
        {
            this._value = value;
        }

        #endregion

        #region public fields

        public override byte[] getBytes()
        {
            byte[] constructorBytes = Constructor.getBytes();
            byte[] bytes = new byte[constructorBytes.Length + _value.Length];
            Array.Copy(constructorBytes, 0, bytes, 0, constructorBytes.Length);
            if (_value.Length > 0)
                Array.Copy(_value, 0, bytes, constructorBytes.Length, _value.Length);
            return bytes;
        }

        public override int getLength()
        {
            return _value.Length + Constructor.getLength();
        }

        public override byte[] getValue()
        {
            return _value;
        }

        public String toString()
        {
            String s = null;

            switch (Constructor.Code)
            {

                case AMQPType.BOOLEAN_TRUE:
                    s = "1";
                    break;

                case AMQPType.BOOLEAN_FALSE:
                case AMQPType.UINT_0:
                case AMQPType.ULONG_0:
                    s = "0";
                    break;

                case AMQPType.BOOLEAN:
                case AMQPType.BYTE:
                case AMQPType.UBYTE:
                case AMQPType.SMALL_INT:
                case AMQPType.SMALL_LONG:
                case AMQPType.SMALL_UINT:
                case AMQPType.SMALL_ULONG:
                    s = BitConverter.ToString(_value,0,1);
                    break;

                case AMQPType.SHORT:
                case AMQPType.USHORT:
                    s = 
                    s = BitConverter.ToString(_value,0,2);
                    break;

                case AMQPType.CHAR:
                case AMQPType.DECIMAL_32:
                case AMQPType.FLOAT:
                case AMQPType.INT:
                case AMQPType.UINT:
                    s = BitConverter.ToString(_value,0,4);
                    break;

                case AMQPType.DECIMAL_64:
                case AMQPType.DOUBLE:
                case AMQPType.LONG:
                case AMQPType.ULONG:
                case AMQPType.TIMESTAMP:
                    s = BitConverter.ToString(_value, 0, 8);
                    break;

                case AMQPType.DECIMAL_128:
                    s = "decimal-128";
                    break;

                case AMQPType.UUID:
                    s = Encoding.UTF8.GetString(_value);
                    break;

                default:
                    break;
            }

            return s;
        }

        #endregion
    }
}