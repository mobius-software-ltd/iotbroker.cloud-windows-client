using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.exceptions;
using com.mobius.software.windows.iotbroker.amqp.headeramqp;
using com.mobius.software.windows.iotbroker.amqp.headerapi;
using com.mobius.software.windows.iotbroker.amqp.sections;
using com.mobius.software.windows.iotbroker.amqp.tlv.compound;
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

namespace com.mobius.software.windows.iotbroker.amqp
{
    public class AMQPParser
    {
        private static int getNext(IByteBuffer buf)
        {
            buf.MarkReaderIndex();
            int length = buf.ReadInt();
            if (length == 1095586128)
            {
                int protocolId = buf.ReadByte();
                int versionMajor = buf.ReadByte();
                int versionMinor = buf.ReadByte();
                int versionRevision = buf.ReadByte();
                if ((protocolId == 0 || protocolId == 3) && versionMajor == 1 && versionMinor == 0
                        && versionRevision == 0)
                {
                    buf.ResetReaderIndex();
                    return 8;
                }
            }
            buf.ResetReaderIndex();
            return length;
        }

        public static IByteBuffer Next(IByteBuffer buf)
        {
            int size = getNext(buf);
            return Unpooled.Buffer(size);
        }

        public static AMQPHeader Decode(IByteBuffer buf)
        {

            long length = buf.ReadInt() & 0xffffffffL;
            int doff = buf.ReadByte() & 0xff;
            int type = buf.ReadByte() & 0xff;
            int channel = buf.ReadShort() & 0xffff;

            // TODO check condition
            if (length == 8 && doff == 2 && (type == 0 || type == 1) && channel == 0)
                if (buf.ReadableBytes == 0)
                    return new AMQPPing();
                else
                    throw new MalformedMessageException("Received malformed ping-header with invalid length");

            // PTOROCOL-HEADER
            if (length == 1095586128 && (doff == 3 || doff == 0) && type == 1 && channel == 0)
                if (buf.ReadableBytes == 0)
                    return new AMQPProtoHeader(doff);
                else
                    throw new MalformedMessageException("Received malformed protocol-header with invalid length");

            if (length != buf.ReadableBytes + 8)
                throw new MalformedMessageException("Received malformed header with invalid length");

            AMQPHeader header = null;
            if (type == 0)
                header = AMQPFactory.getAMQP(buf);
            else if (type == 1)
                header = AMQPFactory.getSASL(buf);
            else
                throw new MalformedMessageException("Received malformed header with invalid type: " + type);

            header.Dott = doff;
            header.HeaderType = type;
            header.Channel = channel;

            if (header.Code.HasValue && header.Code.Value == HeaderCodes.TRANSFER)
            {
                List<AMQPSection> sections = new List<AMQPSection>();
                while (buf.ReadableBytes > 0)
                    sections.Add(AMQPFactory.getSection(buf));

                ((AMQPTransfer)header).addSections(sections.ToArray());
            }                                    

            return header;
        }

        public static IByteBuffer Encode(AMQPHeader header)
        {

            IByteBuffer buf = null;

            if (header is AMQPProtoHeader) {
                buf = Unpooled.Buffer(8);                
                buf.WriteBytes(ASCIIEncoding.ASCII.GetBytes("AMQP"));
                buf.WriteByte(((AMQPProtoHeader)header).ProtocolId);
                buf.WriteByte(((AMQPProtoHeader)header).VersionMajor);
                buf.WriteByte(((AMQPProtoHeader)header).VersionMinor);
                buf.WriteByte(((AMQPProtoHeader)header).VersionRevision);
                return buf;
            }

            if (header is AMQPPing) {
                buf = Unpooled.Buffer(8);
                buf.WriteInt(8);
                buf.WriteByte(header.Dott);
                buf.WriteByte(header.HeaderType);
                buf.WriteShort(header.Channel);
                return buf;
            }

            int length = 8;

            TLVList arguments = header.getArguments();
            length += arguments.getLength();

            Dictionary<SectionCodes, AMQPSection> sections = null;
            if (header.Code.HasValue && header.Code.Value ==HeaderCodes.TRANSFER)
            {
                sections = ((AMQPTransfer)header).getSections();
                foreach (KeyValuePair<SectionCodes, AMQPSection> entry in sections)
                {
                    length += entry.Value.getValue().getLength();
                }
            }

            buf = Unpooled.Buffer(length);

            buf.WriteInt(length);

            int doff = header.Dott;
            buf.WriteByte(doff);

            int type = header.HeaderType;
            buf.WriteByte(type);

            int channel = header.Channel;
            buf.WriteShort(channel);

            buf.WriteBytes(arguments.getBytes());

            if (sections != null)
            {
                foreach (KeyValuePair<SectionCodes, AMQPSection> entry in sections)
                    buf.WriteBytes(entry.Value.getValue().getBytes());
            }

            return buf;
        }
    }
}
