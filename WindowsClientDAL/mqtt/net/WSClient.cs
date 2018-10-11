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
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.mobius.software.windows.iotbroker.mqtt.net
{
    public class WSClient: NetworkChannel<MQMessage>
    {
        private DnsEndPoint address;
        private Int32 workerThreads;

        private Bootstrap bootstrap;
        private MultithreadEventLoopGroup loopGroup;
        private IChannel channel;

        private Queue<MQJsonParser> queue = new Queue<MQJsonParser>();

        // handlers for client connections
        public WSClient(DnsEndPoint address, Int32 workerThreads)
        {
            this.address = address;
            this.workerThreads = workerThreads;
        }

        public void Shutdown()
        {
            if (channel != null)
            {
                channel.CloseAsync();
                channel = null;
            }

            if (loopGroup != null)
                loopGroup.ShutdownGracefullyAsync();
        }

        public void Close()
        {
            if (channel != null)
            {
                channel.CloseAsync();
                channel = null;
            }
            if (loopGroup != null)
            {
                loopGroup.ShutdownGracefullyAsync();
                loopGroup = null;
            }
        }

        public Boolean Init(ConnectionListener<MQMessage> listener)
        {
            if (channel == null)
            {
                bootstrap = new Bootstrap();
                loopGroup = new MultithreadEventLoopGroup(workerThreads);
                bootstrap.Group(loopGroup);
                bootstrap.Channel<TcpSocketChannel>();
			    bootstrap.Option(ChannelOption.TcpNodelay, true);
                bootstrap.Option(ChannelOption.SoKeepalive, true);

                WebSocketClientHandshaker handshaker = WebSocketClientHandshakerFactory.NewHandshaker(this.getUri(), WebSocketVersion.V13, null, false, EmptyHttpHeaders.Default, 1280000);
                WebSocketClientHandler handler = new WebSocketClientHandler(this, handshaker, listener);

                bootstrap.Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    pipeline.AddLast("http - codec",new HttpClientCodec());
                    pipeline.AddLast("aggregator", new HttpObjectAggregator(65536));                    
                    pipeline.AddLast("handler", new MQHandler(listener));
                    pipeline.AddLast(new MQEncoder());
                    pipeline.AddLast(new ExceptionHandler());
                }));
			
                bootstrap.RemoteAddress(address);

                try
                {
                    Task<IChannel> task = bootstrap.ConnectAsync();
                    task.GetAwaiter().OnCompleted(() => 
                    {
                        try
                        {
                            channel = task.Result;
                        }
                        catch (Exception)
                        {
                            listener.ConnectFailed();
                            return;
                        }

                        if (channel != null)
                            listener.Connected();
                        else
                            listener.ConnectFailed();
                    });
			    }
                catch (Exception)
                {
                    return false;
			    } 
		    }

    		return true;
	    }

        public Boolean IsConnected()
        {
            return channel != null;
        }

	    public void Send(MQMessage message)
        {
            if (channel != null && channel.Open)
            {
                String value = null;
                MQJsonParser parser = GetParser();
                try
                {
                    value = parser.JsonString(message);
                }
                catch (Exception)
                {
                    Close();
                }

                ReleaseParser(parser);
                if(value!=null)
                    channel.WriteAndFlushAsync(new TextWebSocketFrame(value));
            }
        }

        public MQJsonParser GetParser()
        {
            if (queue.Count > 0)
                return queue.Dequeue();

            return new MQJsonParser();
        }

        public void ReleaseParser(MQJsonParser parser)
        {
            queue.Enqueue(parser);
        }

        private Uri getUri()
        {
            String type = "ws";
            String url = type + "//:" + this.address.Host + ":" + this.address.Port;
            Uri uri;
            try
            {
                uri = new Uri(url);
            }
            catch (Exception)
            {
                return null;
            }
            return uri;
        }
    }
}