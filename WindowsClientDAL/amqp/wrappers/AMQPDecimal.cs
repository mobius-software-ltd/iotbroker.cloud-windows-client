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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.amqp.wrappers
{
    public class AMQPDecimal
    {
        #region private fields

        private byte[] _value;

        #endregion

        #region constructors

        public AMQPDecimal(byte[] value)
        {
            this._value = value;   
        }

        public AMQPDecimal(byte b)
        {            
            this._value = BitConverter.GetBytes(b);
        }

        public AMQPDecimal(short s)
        {
            this._value = BitConverter.GetBytes(s);
        }

        public AMQPDecimal(int i)
        {
            this._value = BitConverter.GetBytes(i);
        }

        public AMQPDecimal(long l)
        {
            this._value = BitConverter.GetBytes(l);
        }

        public AMQPDecimal(float f)
        {
            this._value = BitConverter.GetBytes(f);
        }

        public AMQPDecimal(double d)
        {
            this._value = BitConverter.GetBytes(d);
        }

        #endregion

        #region public fields

        public double getDouble()
        {
            return BitConverter.ToDouble(_value,0);
        }

        public long getLong()
        {
            return BitConverter.ToInt64(_value, 0);
        }

        public int getInt()
        {
            return BitConverter.ToInt32(_value, 0);
        }

        public float getFloat()
        {
            return BitConverter.ToSingle(_value, 0);
        }

        public short getShort()
        {
            return BitConverter.ToInt16(_value, 0);
        }

        public byte getByte()
        {
            return _value[0];
        }

        public byte[] getValue()
        {
            return _value;
        }

        #endregion
    }
}