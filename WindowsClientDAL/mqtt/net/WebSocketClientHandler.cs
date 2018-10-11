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

using com.mobius.software.windows.iotbroker.mqtt.headers.api;
using com.mobius.software.windows.iotbroker.network;
using DotNetty.Codecs.Http;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Common.Concurrency;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.mqtt.net
{
    public class WebSocketClientHandler : SimpleChannelInboundHandler<Object>
    {
        private WebSocketClientHandshaker handshaker;
        private TaskCompletionSource completionSource;

        private ConnectionListener<MQMessage> listener;
        private WSClient client;

        public WebSocketClientHandler(WSClient client, WebSocketClientHandshaker handshaker, ConnectionListener<MQMessage> listener)
        {
            this.client = client;
            this.handshaker = handshaker;
            this.listener = listener;
            this.completionSource = new TaskCompletionSource();
        }

        override
        protected void ChannelRead0(IChannelHandlerContext ctx, Object msg)
        {
            IChannel ch = ctx.Channel;

            if (!handshaker.IsHandshakeComplete)
            {
                handshaker.FinishHandshake(ch, (IFullHttpResponse)msg);
                return;
            }

            if (msg is IFullHttpResponse) 
            {
                IFullHttpResponse response = (IFullHttpResponse)msg;
                throw new Exception("Unexpected FullHttpResponse (content=" + response.Content.ToString(Encoding.UTF8) + ')');                
            }

            WebSocketFrame frame = (WebSocketFrame)msg;
            if (frame is TextWebSocketFrame || frame is BinaryWebSocketFrame) 
            {
                MQJsonParser parser = client.GetParser();
                try
                {
                    MQMessage message = parser.Decode(frame.Content.Array);
                    this.listener.PacketReceived(message);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    client.ReleaseParser(parser);
                }
            } 
            else if (frame is CloseWebSocketFrame)         
                ch.CloseAsync();         
        }

        public Task HandshakeCompletion => this.completionSource.Task;

        public override void ChannelActive(IChannelHandlerContext ctx) =>
            this.handshaker.HandshakeAsync(ctx.Channel).LinkOutcome(this.completionSource);

        override
        public void ChannelInactive(IChannelHandlerContext ctx)
        {
            listener.ConnectionLost();
        }

        override
        public void ChannelReadComplete(IChannelHandlerContext ctx)
        {
            ctx.Flush();
        }

        override
        public void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            this.completionSource.TrySetException(exception);
            context.CloseAsync();
        }
    }
}