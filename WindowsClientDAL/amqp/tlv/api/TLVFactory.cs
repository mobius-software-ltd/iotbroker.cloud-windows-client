

using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.constructors;
using com.mobius.software.windows.iotbroker.amqp.tlv.array;
using com.mobius.software.windows.iotbroker.amqp.tlv.compound;
using com.mobius.software.windows.iotbroker.amqp.tlv.fixed_;
using com.mobius.software.windows.iotbroker.amqp.tlv.variable;
using com.mobius.software.windows.iotbroker.mqtt.exceptions;
using DotNetty.Buffers;
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
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.amqp.tlv.api
{
    public class TLVFactory
    {
        #region private fields

        private static SimpleConstructor getConstructor(IByteBuffer buf)
        {
            AMQPType code = AMQPType.NULL;
            SimpleConstructor constructor = null;
            byte codeByte = buf.ReadByte();
            if (codeByte == 0)
            {
                TLVAmqp descriptor = getTlv(buf);
                code = (AMQPType)(buf.ReadByte() & 0x0ff);
                constructor = new DescribedConstructor(code, descriptor);
            }
            else
            {
                code = (AMQPType)(codeByte & 0x0ff);
                constructor = new SimpleConstructor(code);
            }
            return constructor;
        }

        #endregion

        #region constructors

        #endregion

        #region public fields

        public static TLVAmqp getTlv(IByteBuffer buf)
        {

            SimpleConstructor constructor = getConstructor(buf);

            TLVAmqp tlv = getElement(constructor, buf);
            return tlv;
        }

        private static TLVAmqp getElement(SimpleConstructor constructor, IByteBuffer buf)
        {

            TLVAmqp tlv = null;

            AMQPType code = constructor.Code;
            switch (code)
            {

                case AMQPType.NULL:
                    tlv = new TLVNull();
                    break;

                case AMQPType.BOOLEAN_TRUE:
                case AMQPType.BOOLEAN_FALSE:
                case AMQPType.UINT_0:
                case AMQPType.ULONG_0:
                    tlv = new TLVFixed(code, new byte[0]);
                    break;

                case AMQPType.BOOLEAN:
                case AMQPType.UBYTE:
                case AMQPType.BYTE:
                case AMQPType.SMALL_UINT:
                case AMQPType.SMALL_INT:
                case AMQPType.SMALL_ULONG:
                case AMQPType.SMALL_LONG:
                    byte valueOne = buf.ReadByte();
                    tlv = new TLVFixed(code, new byte[] { valueOne });
                    break;

                case AMQPType.SHORT:
                case AMQPType.USHORT:
                    byte[] valueTwo = new byte[2];
                    buf.ReadBytes(valueTwo);
                    tlv = new TLVFixed(code, valueTwo);
                    break;

                case AMQPType.UINT:
                case AMQPType.INT:
                case AMQPType.FLOAT:
                case AMQPType.DECIMAL_32:
                case AMQPType.CHAR:
                    byte[] valueFour = new byte[4];
                    buf.ReadBytes(valueFour);
                    tlv = new TLVFixed(code, valueFour);
                    break;

                case AMQPType.ULONG:
                case AMQPType.LONG:
                case AMQPType.DECIMAL_64:
                case AMQPType.DOUBLE:
                case AMQPType.TIMESTAMP:
                    byte[] valueEight = new byte[8];
                    buf.ReadBytes(valueEight);
                    tlv = new TLVFixed(code, valueEight);
                    break;

                case AMQPType.DECIMAL_128:
                case AMQPType.UUID:
                    byte[] valueSixteen = new byte[16];
                    buf.ReadBytes(valueSixteen);
                    tlv = new TLVFixed(code, valueSixteen);
                    break;

                case AMQPType.STRING_8:
                case AMQPType.SYMBOL_8:
                case AMQPType.BINARY_8:
                    int var8length = buf.ReadByte() & 0xff;
                    byte[] varValue8 = new byte[var8length];
                    buf.ReadBytes(varValue8, 0, varValue8.Length);
                    tlv = new TLVVariable(code, varValue8);
                    break;

                case AMQPType.STRING_32:
                case AMQPType.SYMBOL_32:
                case AMQPType.BINARY_32:
                    int var32length = buf.ReadInt();
                    byte[] varValue32 = new byte[var32length];
                    buf.ReadBytes(varValue32, 0, varValue32.Length);
                    tlv = new TLVVariable(code, varValue32);
                    break;

                case AMQPType.LIST_0:
                    tlv = new TLVList();
                    break;

                case AMQPType.LIST_8:
                    int list8size = buf.ReadByte() & 0xff;
                    int list8count = buf.ReadByte() & 0xff;
                    List<TLVAmqp> list8values = new List<TLVAmqp>();
                    for (int i = 0; i < list8count; i++)
                        list8values.Add(TLVFactory.getTlv(buf));
                    tlv = new TLVList(code, list8values);
                    break;

                case AMQPType.LIST_32:
                    int list32size = buf.ReadInt();
                    int list32count = buf.ReadInt();
                    List<TLVAmqp> list32values = new List<TLVAmqp>();
                    for (int i = 0; i < list32count; i++)
                        list32values.Add(TLVFactory.getTlv(buf));
                    tlv = new TLVList(code, list32values);
                    break;

                case AMQPType.MAP_8:
                    Dictionary<TLVAmqp, TLVAmqp> map8 = new Dictionary<TLVAmqp, TLVAmqp>();
                    int map8size = buf.ReadByte() & 0xff;
                    int map8count = buf.ReadByte() & 0xff;
                    int stop8 = buf.ReaderIndex + map8size - 1;
                    while (buf.ReaderIndex < stop8)
                        map8[TLVFactory.getTlv(buf)] = TLVFactory.getTlv(buf);
                    tlv = new TLVMap(code, map8);
                    break;

                case AMQPType.MAP_32:
                    Dictionary<TLVAmqp, TLVAmqp> map32 = new Dictionary<TLVAmqp, TLVAmqp>();
                    int map32size = buf.ReadInt();
                    int map32count = buf.ReadInt();
                    int stop32 = buf.ReaderIndex + map32size - 4;
                    while (buf.ReaderIndex < stop32)
                        map32[TLVFactory.getTlv(buf)] = TLVFactory.getTlv(buf);
                    tlv = new TLVMap(code, map32);
                    break;

                case AMQPType.ARRAY_8:
                    List<TLVAmqp> arr8 = new List<TLVAmqp>();
                    int array8size = buf.ReadByte() & 0xff;
                    int array8count = buf.ReadByte() & 0xff;
                    SimpleConstructor arr8constructor = getConstructor(buf);
                    for (int i = 0; i < array8count; i++)
                        arr8.Add(TLVFactory.getElement(arr8constructor, buf));
                    tlv = new TLVArray(code, arr8);
                    break;

                case AMQPType.ARRAY_32:
                    List<TLVAmqp> arr32 = new List<TLVAmqp>();
                    int array32size = buf.ReadInt();
                    int array32count = buf.ReadInt();
                    SimpleConstructor arr32constructor = getConstructor(buf);
                    for (int i = 0; i < array32count; i++)
                        arr32.Add(TLVFactory.getElement(arr32constructor, buf));
                    tlv = new TLVArray(code, arr32);
                    break;

                default:
                    break;
            }

            if (constructor is DescribedConstructor)
			    tlv.Constructor = constructor;

            return tlv;
        }

        #endregion
    }
}
