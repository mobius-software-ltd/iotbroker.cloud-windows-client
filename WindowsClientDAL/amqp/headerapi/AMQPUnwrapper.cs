

using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.exceptions;
using com.mobius.software.windows.iotbroker.amqp.tlv.api;
using com.mobius.software.windows.iotbroker.amqp.tlv.array;
using com.mobius.software.windows.iotbroker.amqp.tlv.compound;
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
    public class AMQPUnwrapper<T>
    {
        #region private fields

        #endregion

        #region constructors

        #endregion

        #region public fields

        public static short unwrapUByte(TLVAmqp tlv)
        {
            if (tlv.Code != AMQPType.UBYTE)
                throw new ArgumentException(new DateTime() + ": " + "Error trying to parse UBYTE: received " + tlv.Code);

            return (short)(tlv.getValue()[0] & 0xff);
        }

        public static byte unwrapByte(TLVAmqp tlv)
        {
            if (tlv.Code != AMQPType.BYTE)
                throw new ArgumentException(new DateTime() + ": " + "Error trying to parse BYTE: received " + tlv.Code);

            return tlv.getValue()[0];
        }

        public static int unwrapUShort(TLVAmqp tlv)
        {
            if (tlv.Code != AMQPType.USHORT)
                throw new ArgumentException(new DateTime() + ": " + "Error trying to parse USHORT: received " + tlv.Code);
            
            return BitConverter.ToUInt16(tlv.getValue(), 0) & 0x00ffff;
        }

        public static short unwrapShort(TLVAmqp tlv)
        {
            if (tlv.Code != AMQPType.SHORT)
                throw new ArgumentException(new DateTime() + ": " + "Error trying to parse SHORT: received " + tlv.Code);

            return BitConverter.ToInt16(tlv.getValue(),0);
        }

        public static long unwrapUInt(TLVAmqp tlv)
        {
            AMQPType code = tlv.Code;
            if (code != AMQPType.UINT && code != AMQPType.SMALL_UINT && code != AMQPType.UINT_0)
                throw new ArgumentException(new DateTime() + ": " + "Error trying to parse UINT: received " + code);

            byte[] value = tlv.getValue();
            if (value.Length == 0)
                return 0;

            if (value.Length == 1)
                return tlv.getValue()[0] & 0x00ff;

            return BitConverter.ToUInt32(tlv.getValue(), 0) & 0x00ffffffffL;
        }

        public static int unwrapInt(TLVAmqp tlv)
        {
            AMQPType code = tlv.Code;
            if (code != AMQPType.INT && code != AMQPType.SMALL_INT)
                throw new ArgumentException(new DateTime() + ": " + "Error trying to parse INT: received " + code);

            byte[] value = tlv.getValue();
            if (value.Length == 0)
                return 0;

            if (value.Length == 1)
                return tlv.getValue()[0];
            
            return BitConverter.ToInt32(tlv.getValue(),0);
        }

        public static BigInteger unwrapULong(TLVAmqp tlv)
        {
            AMQPType code = tlv.Code;
            if (code != AMQPType.ULONG && code != AMQPType.SMALL_ULONG && code != AMQPType.ULONG_0)
                throw new ArgumentException(new DateTime() + ": " + "Error trying to parse ULONG: received " + code);

            byte[] value = tlv.getValue();

            if (value.Length == 0)
                return new BigInteger(0);

            if (value.Length == 1)
                return new BigInteger(tlv.getValue()[0] & 0xff);
            
            return new BigInteger(BitConverter.ToUInt64(tlv.getValue(),0));            
        }

        public static Int64 unwrapLong(TLVAmqp tlv)
        {
            AMQPType code = tlv.Code;
            if (code != AMQPType.LONG && code != AMQPType.SMALL_LONG)
                throw new ArgumentException(new DateTime() + ": " + "Error trying to parse LONG: received " + code);
            byte[] value = tlv.getValue();

            if (value.Length == 0)
                return 0L;

            if (value.Length == 1)
                return (long)tlv.getValue()[0];
            
            return BitConverter.ToInt64(tlv.getValue(),0);
        }

        public static Boolean unwrapBool(TLVAmqp tlv)
        {
            switch (tlv.Code)
            {
                case AMQPType.BOOLEAN:
                    byte val = tlv.getValue()[0];
                    if (val == 0)
                        return false;
                    else if (val == 1)
                        return true;
                    else
                        throw new MalformedMessageException("Invalid Boolean type value: " + val);
                case AMQPType.BOOLEAN_TRUE:
                    return true;
                case AMQPType.BOOLEAN_FALSE:
                    return false;
                default:
                    throw new ArgumentException(new DateTime() + ": " + "Error trying to parse BOOLEAN: received " + tlv.Code);
            }
        }

        public static Double unwrapDouble(TLVAmqp tlv)
        {
            if (tlv.Code != AMQPType.DOUBLE)
                throw new ArgumentException(new DateTime() + ": " + "Error trying to parse DOUBLE: received " + tlv.Code);

            return BitConverter.ToDouble(tlv.getValue(),0);            
        }

        public static float unwrapFloat(TLVAmqp tlv)
        {
            if (tlv.Code != AMQPType.FLOAT)
                throw new ArgumentException(new DateTime() + ": " + "Error trying to parse FLOAT: received " + tlv.Code);

            return BitConverter.ToSingle(tlv.getValue(), 0);
        }

        public static DateTime unwrapTimestamp(TLVAmqp tlv)
        {
            if (tlv.Code != AMQPType.TIMESTAMP)
                throw new ArgumentException(new DateTime() + ": " + "Error trying to parse TIMESTAMP: received " + tlv.Code);

            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            origin.AddMilliseconds(BitConverter.ToInt64(tlv.getValue(), 0));
            return origin;
        }

        public static AMQPDecimal unwrapDecimal(TLVAmqp tlv)
        {
            AMQPType code = tlv.Code;
            if (code != AMQPType.DECIMAL_32 && code != AMQPType.DECIMAL_64 && code != AMQPType.DECIMAL_128)
                throw new ArgumentException(new DateTime() + ": " + "Error trying to parse DECIMAL: received " + tlv.Code);

            return new AMQPDecimal(tlv.getValue());
        }

        public static int unwrapChar(TLVAmqp tlv)
        {
            if (tlv.Code != AMQPType.CHAR)
                throw new ArgumentException(new DateTime() + ": " + "Error trying to parse CHAR: received " + tlv.Code);
            
            return BitConverter.ToInt32(tlv.getValue(), 0);
        }

        public static String unwrapString(TLVAmqp tlv)
        {
            AMQPType code = tlv.Code;
            if (code != AMQPType.STRING_8 && code != AMQPType.STRING_32)
                throw new ArgumentException(new DateTime() + ": " + "Error trying to parse STRING: received " + code);
            return Encoding.UTF8.GetString(tlv.getValue());
        }

        public static AMQPSymbol unwrapSymbol(TLVAmqp tlv)
        {
            AMQPType code = tlv.Code;
            if (code != AMQPType.SYMBOL_8 && code != AMQPType.SYMBOL_32)
                throw new ArgumentException(new DateTime() + ": " + "Error trying to parse SYMBOL: received " + code);

            return new AMQPSymbol(Encoding.UTF8.GetString(tlv.getValue()));
        }

        public static byte[] unwrapBinary(TLVAmqp tlv)
        {
            AMQPType code = tlv.Code;
            if (code != AMQPType.BINARY_8 && code != AMQPType.BINARY_32)
                throw new ArgumentException(new DateTime() + ": " + "Error trying to parse BINARY: received " + code);

            return tlv.getValue();
        }

        public static Guid unwrapUuid(TLVAmqp tlv)
        {
            if (tlv.Code != AMQPType.UUID)
                throw new ArgumentException(
                        new DateTime() + ": " + "Error trying to parse UUID: received " + tlv.Code);

            return Guid.Parse(Encoding.UTF8.GetString(tlv.getValue()));
        }

        public static List<Object> unwrapList(TLVAmqp tlv)
        {
            AMQPType code = tlv.Code;
            if (code != AMQPType.LIST_0 && code != AMQPType.LIST_8 && code != AMQPType.LIST_32)
                throw new ArgumentException(new DateTime() + ": " + "Error trying to parse LIST: received " + tlv.Code);

            List<Object> result = new List<Object>();
            foreach (TLVAmqp value in ((TLVList)tlv).getList())
                result.Add(unwrap(value));

            return result;
        }


        public static Dictionary<T, Object> unwrapMap(TLVAmqp tlv)
        {
            AMQPType code = tlv.Code;
            if (code != AMQPType.MAP_8 && code != AMQPType.MAP_32)
                throw new ArgumentException(new DateTime() + ": " + "Error trying to parse MAP: received " + tlv.Code);
            Dictionary<T, Object> result = new Dictionary<T, Object>();
            foreach (KeyValuePair<TLVAmqp, TLVAmqp> entry in ((TLVMap)tlv).getMap())
            {
                T key = (T)unwrap(entry.Key);
                Object value = unwrap(entry.Value);
                result[key] = value;
            }
            return result;
        }


        public static List<T> unwrapArray(TLVAmqp tlv)
        {
            AMQPType code = tlv.Code;
            if (code != AMQPType.ARRAY_8 && code != AMQPType.ARRAY_32)
                throw new ArgumentException(new DateTime() + ": " + "Error trying to parse ARRAY: received " + tlv.Code);

            List<T> result = new List<T>();
            foreach (TLVAmqp element in ((TLVArray)tlv).getElements())
                result.Add((T)unwrap(element));
            return result;
        }

        public static Object unwrap(TLVAmqp value)
        {

            switch (value.Code)
            {

                case AMQPType.NULL:
                    return null;

                case AMQPType.ARRAY_32:
                case AMQPType.ARRAY_8:
                    return unwrapArray(value);

                case AMQPType.BINARY_32:
                case AMQPType.BINARY_8:
                    return unwrapBinary(value);

                case AMQPType.UBYTE:
                    return unwrapUByte(value);

                case AMQPType.BOOLEAN:
                case AMQPType.BOOLEAN_FALSE:
                case AMQPType.BOOLEAN_TRUE:
                    return unwrapBool(value);

                case AMQPType.BYTE:
                    return unwrapByte(value);

                case AMQPType.CHAR:
                    return unwrapChar(value);

                case AMQPType.DOUBLE:
                    return unwrapDouble(value);

                case AMQPType.FLOAT:
                    return unwrapFloat(value);

                case AMQPType.INT:
                case AMQPType.SMALL_INT:
                    return unwrapInt(value);

                case AMQPType.LIST_0:
                case AMQPType.LIST_32:
                case AMQPType.LIST_8:
                    return unwrapList(value);

                case AMQPType.LONG:
                case AMQPType.SMALL_LONG:
                    return unwrapLong(value);

                case AMQPType.MAP_32:
                case AMQPType.MAP_8:
                    return unwrapMap(value);

                case AMQPType.SHORT:
                    return unwrapShort(value);

                case AMQPType.STRING_32:
                case AMQPType.STRING_8:
                    return unwrapString(value);

                case AMQPType.SYMBOL_32:
                case AMQPType.SYMBOL_8:
                    return unwrapSymbol(value);

                case AMQPType.TIMESTAMP:
                    DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    origin.AddMilliseconds(BitConverter.ToInt64(value.getValue(), 0));
                    return origin;

                case AMQPType.UINT:
                case AMQPType.SMALL_UINT:
                case AMQPType.UINT_0:
                    return unwrapUInt(value);

                case AMQPType.ULONG:
                case AMQPType.SMALL_ULONG:
                case AMQPType.ULONG_0:
                    return unwrapULong(value);

                case AMQPType.USHORT:
                    return unwrapUShort(value);

                case AMQPType.UUID:
                    return unwrapUuid(value);

                case AMQPType.DECIMAL_128:
                case AMQPType.DECIMAL_32:
                case AMQPType.DECIMAL_64:
                    return unwrapDecimal(value);

                default:
                    throw new ArgumentException(new DateTime() + ": " + "received unrecognized type");
            }
        }

        #endregion
    }
}