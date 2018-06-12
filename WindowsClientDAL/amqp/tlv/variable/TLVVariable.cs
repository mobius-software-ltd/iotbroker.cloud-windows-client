

using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.constructors;
using com.mobius.software.windows.iotbroker.amqp.tlv.api;
using com.mobius.software.windows.iotbroker.amqp.tlv.array;
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
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.amqp.tlv.variable
{
    public class TLVVariable : TLVAmqp
    {
        #region private fields

        private byte[] _value;
        private Int32 _width;

        #endregion

        #region constructors

        public TLVVariable(AMQPType code, byte[] value): base(new SimpleConstructor(code))
        {
            this._value = value;
            _width = _value.Length > 255 ? 4 : 1;
        }

        #endregion

        #region public fields

        public override byte[] getBytes()
        {
            byte[] constructorBytes = Constructor.getBytes();
            byte[] widthBytes = new byte[_width];
            if (_width == 1)
                widthBytes[0] = (byte)_value.Length;
            else if (_width == 4)
                Array.Copy(BitConverter.GetBytes(_value.Length), widthBytes, 4);
                
            byte[] bytes = new byte[constructorBytes.Length + _width + _value.Length];

            Array.Copy(constructorBytes, 0, bytes, 0, constructorBytes.Length);
            Array.Copy(widthBytes, 0, bytes, constructorBytes.Length, _width);

            if (_value.Length > 0)
                Array.Copy(_value, 0, bytes, constructorBytes.Length + _width, _value.Length);
            return bytes;
        }

        public override int getLength()
        {
            return _value.Length + Constructor.getLength() + _width;
        }

        public override byte[] getValue()
        {
            return _value;
        }

        #endregion
    }
}