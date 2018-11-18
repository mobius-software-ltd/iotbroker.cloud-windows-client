

using com.mobius.software.windows.iotbroker.dal;
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
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.mobius.software.windows.iotbroker.mqtt.net
{
    public class TCPClient : NetworkChannel<MQMessage>
    {
        private DnsEndPoint address;
        private Int32 workerThreads;

        private Bootstrap bootstrap;
        private MultithreadEventLoopGroup loopGroup;
        private IChannel channel;

        private Boolean isSecured;
        private String certificate;
        private String certificatePassword;

        X509Certificate2 cert = null;

        // handlers for client connections
        public TCPClient(DnsEndPoint address, Boolean isSecured, String certificate, String certificatePassword, Int32 workerThreads)
        {
            this.address = address;
            this.workerThreads = workerThreads;
            this.isSecured = isSecured;
            this.certificate = certificate;
            this.certificatePassword = certificatePassword;
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

                if (certificate != null && certificate.Length>0)
                    cert = CertificatesHelper.load(certificate, certificatePassword);

                TlsHandler tlsHandler=null;
                if (isSecured)
                {
                    if (cert != null)
                        tlsHandler = new TlsHandler(stream => new SslStream(stream, true, (sender, certificate, chain, errors) => true), new ClientTlsSettings(address.Host, new List<X509Certificate>() { cert }));
                    else
                        tlsHandler = TlsHandler.Client(address.Host);
                }

                bootstrap.Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    if (isSecured)
                        pipeline.AddLast("ssl", tlsHandler);                        
                    
                    pipeline.AddLast(new MQDecoder());
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
                channel.WriteAndFlushAsync(message);
        }
    }
}
    