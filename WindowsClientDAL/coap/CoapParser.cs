using com.mobius.software.windows.iotbroker.coap.avps;
using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

namespace com.mobius.software.windows.iotbroker.coap
{
    public class CoapParser
    {
        public static CoapMessage decode(IByteBuffer buf)
        {
            byte firstByte = buf.ReadByte();

            int version = firstByte >> 6;
            if (version != 1)
                throw new ArgumentException("Invalid version:" + version);
            
            int typeValue = (firstByte >> 4) & 2;
            CoapType type = (CoapType)typeValue;
            
            int tokenLength = firstByte & 0xf;
            if (tokenLength < 0 || tokenLength > 8)
                throw new ArgumentException("Invalid token length:" + tokenLength);

            int codeByte = buf.ReadByte();
            int codeValue = (codeByte >> 5) * 100;
            codeValue += codeByte & 0x1F;
            CoapCode code = (CoapCode)codeValue;
            
            int messageID = buf.ReadShort();
            if (messageID < 0 || messageID > 65535)
                throw new ArgumentException("Invalid messageID value:" + messageID);

            byte[] token = new byte[tokenLength];
            if (tokenLength > 0)
                buf.ReadBytes(token, 0, tokenLength);
            int number = 0;

            List<CoapOption> options = new List<CoapOption>();
            while (buf.IsReadable())
            {
                byte nextByte = buf.ReadByte();
                if (nextByte == 0xFF)
                    break;

                int delta = ((nextByte >> 4) & 15);
                if (delta == 13)
                    delta = (delta << 8 | buf.ReadByte()) - 13;
                else if (delta == 14)
                    delta = (delta << 16 | buf.ReadByte() << 8 | buf.ReadByte()) - 269;
                else if (delta < 0 || delta > 14)
                    throw new ArgumentException("invalid option delta value:" + delta);

                number += delta;
                if (number < 0)
                    throw new ArgumentException("invalid negative option number:" + number + ", delta:" + delta);

                int optionLength = nextByte & 15;
                if (optionLength == 13)
                    optionLength = (optionLength << 8 | buf.ReadByte()) - 13;
                else if (optionLength == 14)
                    optionLength = (optionLength << 16 | buf.ReadByte() << 8 | buf.ReadByte()) - 269;
                else if (optionLength < 0 || optionLength > 14)
                    throw new ArgumentException("invalid option length");

                byte[] optionValue = new byte[optionLength];
                if (optionLength > 0)
                    buf.ReadBytes(optionValue, 0, optionLength);

                options.Add(new CoapOption(number, optionLength, optionValue));                
            }

            byte[] payload = null;
            if (buf.IsReadable())
            {
                payload = new byte[buf.ReadableBytes];
                buf.ReadBytes(payload);                
            }

            return new CoapMessage(version, type, code, messageID, token, options, payload);            
        }

        public static IByteBuffer encode(CoapMessage message)
        {
            IByteBuffer buf = Unpooled.Buffer();
            byte firstByte = 0;
            firstByte += (byte) (message.Version << 6);
            firstByte += (byte) ((int)message.CoapType << 4);
            firstByte |= (byte) message.Token.Length;
            buf.WriteByte(firstByte);

            int coapCode = (int)message.CoapCode;
            int codeMsb = (coapCode / 100);
            int codeLsb = (byte)(coapCode % 100);
            int codeByte = ((codeMsb << 5) + codeLsb);

            buf.WriteByte(codeByte & 0x0FF);
            buf.WriteShort(message.MessageID);
            buf.WriteBytes(message.Token);

            Int32 previousDelta = 0;
            List<CoapOption> sortedOptions = message.Options.OrderBy(o => o.Number).ToList();
            foreach (CoapOption option in sortedOptions)
            {
                int delta = option.Number - previousDelta;
                int realDelta = delta;
                byte[] extraDeltaBytes = new byte[0];
                if (delta >= 269)
                {
                    realDelta = 14;
                    short remainingValue = (short)(delta - 269);
                    extraDeltaBytes = BitConverter.GetBytes(remainingValue);
                }
                else if (delta >= 13)
                {
                    realDelta = 13;
                    extraDeltaBytes = new byte[1];
                    extraDeltaBytes[0] = (byte)(delta - 13);
                }

                int valueLength = option.Value.Length;
                int realLength = valueLength;
                byte[] extraLengthBytes = new byte[0];
                if (valueLength >= 269)
                {
                    realLength = 14;
                    short remainingValue = (short)(valueLength - 269);
                    extraLengthBytes = BitConverter.GetBytes(remainingValue);
                }
                else if (valueLength >= 13)
                {
                    realLength = 13;
                    extraLengthBytes = new byte[1];
                    extraLengthBytes[0] = (byte)(valueLength - 13);
                }

                buf.WriteByte((byte)((realDelta << 4) | (realLength & 0x0F)));
                if (extraDeltaBytes.Length > 0)
                    buf.WriteBytes(extraDeltaBytes);

                if (extraLengthBytes.Length > 0)
                    buf.WriteBytes(extraLengthBytes);

                buf.WriteBytes(option.Value);
                previousDelta = option.Number;
            }

            buf.WriteByte(0xFF);
            if (message.Payload != null)
                buf.WriteBytes(message.Payload);

            return buf;
        }
    }
}
