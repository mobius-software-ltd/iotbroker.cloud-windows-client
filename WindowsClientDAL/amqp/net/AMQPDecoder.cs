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

using DotNetty.Codecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using com.mobius.software.windows.iotbroker.mqtt.headers.api;
using com.mobius.software.windows.iotbroker.mqtt.exceptions;
using com.mobius.software.windows.iotbroker.amqp.headerapi;

namespace com.mobius.software.windows.iotbroker.amqp.net
{
    public class AMQPDecoder : ByteToMessageDecoder
    {
        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            IByteBuffer nextHeader = null;
            do
            {
                if (input.ReadableBytes > 1)
                {
                    try
                    {
                        nextHeader = AMQPParser.Next(input);
                    }
                    catch (Exception ex)
                    {
                        if (ex is MalformedMessageException || ex is IndexOutOfRangeException)
                        {
                            input.ResetReaderIndex();
                            if (nextHeader != null)
                            {
                                nextHeader.Release();
                                nextHeader = null;
                            }
                        }
                        else
                            throw ex;
                    }
                }

                if (nextHeader != null)
                {
                    input.ReadBytes(nextHeader, nextHeader.Capacity);
                    try
                    {
                        AMQPHeader header = AMQPParser.Decode(nextHeader);
                        output.Add(header);
                    }
                    catch (Exception e)
                    {
                        input.ResetReaderIndex();
                        context.Channel.Pipeline.Remove(this);
                        throw e;
                    }
                    finally
                    {
                        nextHeader.Release();
                    }
                }
            }
            while (input.ReadableBytes > 1 && nextHeader != null);
        }
    }
}
