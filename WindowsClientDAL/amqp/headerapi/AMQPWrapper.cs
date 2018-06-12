

using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.exceptions;
using com.mobius.software.windows.iotbroker.amqp.tlv.api;
using com.mobius.software.windows.iotbroker.amqp.tlv.array;
using com.mobius.software.windows.iotbroker.amqp.tlv.compound;
using com.mobius.software.windows.iotbroker.amqp.tlv.fixed_;
using com.mobius.software.windows.iotbroker.amqp.tlv.variable;
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
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.amqp.wrappers
{
    public class AMQPWrapper<T>
    {
        #region private fields

        #endregion

        #region constructors

        #endregion

        #region public fields

        public static TLVAmqp wrap(Object o)
        {

            TLVAmqp result = null;

            if (o == null)
                return new TLVNull();

            if (o is Byte)
			result = wrapByte((Byte)o);
		else if (o is Int16)
			result = (Int16)o >= 0 ? wrapUByte((Int16)o) : wrapShort((Int16)o);
		else if (o is Int32)
			result = (Int32)o >= 0 ? wrapUShort((Int32)o) : wrapInt((Int32)o);
		else if (o is Int64)
			result = (Int64)o >= 0 ? wrapUInt((Int64)o) : wrapLong((Int64)o);
		else if (o is BigInteger)
			result = wrapULong((BigInteger)o);
		else if (o is String)
			result = wrapString((String)o);
		else if (o is AMQPSymbol)
			result = wrapSymbol((AMQPSymbol)o);
		else if (o is byte[])
			result = wrapBinary((byte[])o);
		else if (o is Boolean)
			result = wrapBool((Boolean)o);
		else if (o is Char)
			result = wrapChar((Char)o);
		else if (o is Double)
			result = wrapDouble((Double)o);
		else if (o is Single)
			result = wrapFloat((Single)o);
		else if (o is Guid)
			result = wrapUuid((Guid)o);
		else if (o is DateTime)
			result = wrapTimestamp((DateTime)o);
		else if (o is AMQPDecimal) {
                byte[] val = ((AMQPDecimal)o).getValue();
                if (val.Length == 4)
                    result = wrapDecimal32((AMQPDecimal)o);
                else if (val.Length == 8)
                    result = wrapDecimal64((AMQPDecimal)o);
                else if (val.Length == 16)
                    result = wrapDecimal128((AMQPDecimal)o);
            } else
			throw new ArgumentException("Wrapper received unrecognized type");

            return result;
        }

        public static TLVFixed wrapBool(Boolean b)
        {
            byte[] value = new byte[0];
            AMQPType code = b ? AMQPType.BOOLEAN_TRUE : AMQPType.BOOLEAN_FALSE;
            return new TLVFixed(code, value);
        }

        public static TLVFixed wrapUByte(short b)
        {
            if (b < 0)
                throw new ArgumentException("negative value of " + b + " cannot be assignet to UBYTE type");
            return new TLVFixed(AMQPType.UBYTE, new byte[] { (byte)b });
        }

        public static TLVFixed wrapByte(byte b)
        {
            return new TLVFixed(AMQPType.BYTE, new byte[] { b });
        }

        public static TLVFixed wrapUInt(long i)
        {
            if (i < 0)
                throw new ArgumentException("negative value of " + i + " cannot be assignet to UINT type");
            byte[] value = convertUInt(i);
            AMQPType code = AMQPType.NULL;
            if (value.Length == 0)
                code = AMQPType.UINT_0;
            else if (value.Length == 1)
                code = AMQPType.SMALL_UINT;
            else if (value.Length > 1)
                code = AMQPType.UINT;

            return new TLVFixed(code, value);
        }

        public static TLVFixed wrapInt(int i)
        {
            byte[] value = convertInt(i);
            AMQPType code = value.Length > 1 ? AMQPType.INT : AMQPType.SMALL_INT;
            return new TLVFixed(code, value);
        }

        public static TLVFixed wrapULong(BigInteger l)
        {
            if (l == null)
                throw new ArgumentException("Wrapper cannot wrap ulong null");
            if (l<BigInteger.Zero)
                throw new ArgumentException("negative value of " + l + " cannot be assignet to ULONG type");
            byte[] value = convertULong(l);
            AMQPType code = AMQPType.NULL;
            if (value.Length == 0)
                code = AMQPType.ULONG_0;
            else if (value.Length == 1)
                code = AMQPType.SMALL_ULONG;
            else
                code = AMQPType.ULONG;
            return new TLVFixed(code, value);
        }

        public static TLVFixed wrapLong(long l)
        {
            byte[] value = convertLong(l);
            AMQPType code = value.Length > 1 ? AMQPType.LONG : AMQPType.SMALL_LONG;
            return new TLVFixed(code, value);
        }

        public static TLVVariable wrapBinary(byte[] b)
        {
            if (b == null)
                throw new ArgumentException("Wrapper cannot wrap binary null");

            AMQPType code = b.Length > 255 ? AMQPType.BINARY_32 : AMQPType.BINARY_8;
            return new TLVVariable(code, b);
        }

        public static TLVFixed wrapUuid(Guid u)
        {
            if (u == null)
                throw new ArgumentException("Wrapper cannot wrap uuid null");

            return new TLVFixed(AMQPType.UUID, Encoding.UTF8.GetBytes(u.ToString()));
        }

        public static TLVFixed wrapUShort(int s)
        {
            if (s < 0)
                throw new ArgumentException("negative value of " + s + " cannot be assignet to USHORT type");
                        
            return new TLVFixed(AMQPType.USHORT, BitConverter.GetBytes((short)s));
        }

        public static TLVFixed wrapShort(short s)
        {
            return new TLVFixed(AMQPType.SHORT, BitConverter.GetBytes((short)s));
        }

        public static TLVFixed wrapDouble(double d)
        {            
            return new TLVFixed(AMQPType.DOUBLE, BitConverter.GetBytes(d));
        }

        public static TLVFixed wrapFloat(float f)
        {
            return new TLVFixed(AMQPType.FLOAT, BitConverter.GetBytes(f));
        }

        public static TLVFixed wrapChar(int c)
        {
            return new TLVFixed(AMQPType.CHAR, BitConverter.GetBytes(c));
        }

        public static TLVFixed wrapTimestamp(DateTime date)
        {
            if (date == null)
                throw new ArgumentException("Wrapper cannot wrap null Timestamp");

            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan ts = date.Subtract(origin);            
            return new TLVFixed(AMQPType.TIMESTAMP, BitConverter.GetBytes(ts.TotalMilliseconds));
        }

        public static TLVFixed wrapDecimal32(AMQPDecimal d)
        {
            if (d == null)
                throw new ArgumentException("Wrapper cannot wrap null decimal32");

            return new TLVFixed(AMQPType.DECIMAL_32, d.getValue());
        }

        public static TLVFixed wrapDecimal64(AMQPDecimal d)
        {
            if (d == null)
                throw new ArgumentException("Wrapper cannot wrap null decimal64");

            return new TLVFixed(AMQPType.DECIMAL_64, d.getValue());
        }

        public static TLVFixed wrapDecimal128(AMQPDecimal d)
        {
            if (d == null)
                throw new ArgumentException("Wrapper cannot wrap null decimal128");

            return new TLVFixed(AMQPType.DECIMAL_128, d.getValue());
        }

        public static TLVVariable wrapString(String s)
        {
            if (s == null)
                throw new ArgumentException("Wrapper cannot wrap null String");

            byte[] value = Encoding.UTF8.GetBytes(s);
            AMQPType code = value.Length > 255 ? AMQPType.STRING_32 : AMQPType.STRING_8;
            return new TLVVariable(code, value);
        }

        public static TLVVariable wrapSymbol(AMQPSymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentException("Wrapper cannot wrap null symbol");

            byte[] value = Encoding.ASCII.GetBytes(symbol.Value);
            AMQPType code = value.Length > 255 ? AMQPType.SYMBOL_32 : AMQPType.SYMBOL_8;
            return new TLVVariable(code, value);
        }

        public static TLVList wrapList(List<T> input)
        {
            if (input == null)
                throw new ArgumentException("Wrapper cannot wrap null List");

            TLVList list = new TLVList();
            foreach (T o in input)
                list.addElement(wrap(o));

            return list;
        }

        public static TLVMap wrapMap(Dictionary<T, Object> input)
        {
            if (input == null)
                throw new ArgumentException("Wrapper cannot wrap null Map");

            TLVMap map = new TLVMap();
            TLVAmqp key, value;

            foreach (KeyValuePair<T, Object> entry in input)
            {
                key = wrap(entry.Key);
                value = wrap(entry.Value);
                map.putElement(key, value);
            }
            return map;
        }

        public static TLVArray wrapArray(List<T> input)
        {
            if (input == null)
                throw new ArgumentException("Wrapper cannot wrap null array");

            TLVArray array = new TLVArray();
            foreach (T s in input)
                array.addElement(wrap(s));

            return array;
        }

        private static byte[] convertUInt(long i)
        {            
            if (i == 0)
                return new byte[0];
            else if (i > 0 && i <= 255)
                return new byte[] { (byte)i };
            else
                return BitConverter.GetBytes((UInt32)i);
        }

        private static byte[] convertInt(int i)
        {
            if (i == 0)
                return new byte[0];
            else if (i >= -128 && i <= 127)
                return new byte[] { (byte)i };
            else
                return BitConverter.GetBytes(i);
        }

        private static byte[] convertULong(BigInteger l)
        {
            if (l == 0)
                return new byte[0];
            else if (l > 0 && l <= 255)
                return new byte[] { (byte)l };
            else                                
                return BitConverter.GetBytes((UInt64)l);
        }

        private static byte[] convertLong(long l)
        {
            if (l == 0)
                return new byte[0];
            else if (l >= -128 && l <= 127)
                return new byte[] { (byte)l };
            else
                return BitConverter.GetBytes(l);
        }

        #endregion
    }
}