using DotNetty.Codecs;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using com.mobius.software.windows.iotbroker.network.dtls;
using DotNetty.Buffers;
using Org.BouncyCastle.Crypto.Tls;

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

namespace com.mobius.software.windows.iotbroker.mqtt_sn.net
{
    public class DtlsClientHandler : MessageToMessageDecoder<DatagramPacket>
    {
        private AsyncDtlsClientProtocol protocol;
        private DtlsStateHandler handler;
        public DtlsClientHandler(AsyncDtlsClientProtocol protocol, DtlsStateHandler handler)
        {
		    this.protocol=protocol;
		    this.handler=handler;
        }

        protected override void Decode(IChannelHandlerContext context, DatagramPacket message, List<object> output)
        {
            try
            {
                List<IByteBuffer> parsedPackets = protocol.ReceivePacket(message.Content);
                if (parsedPackets.Count > 0)
                {
                    foreach (IByteBuffer currBuffer in parsedPackets)
                    {
                        output.Add(new DatagramPacket(currBuffer, message.Recipient));
                    }					    
                }
            }
            catch (TlsFatalAlert ex)
            {
                try
                {
                    protocol.SendAlert(AlertLevel.fatal, ex.AlertDescription, ex.Message, ex.InnerException);
                }
                catch (Exception)
                {
                }

                if (handler != null)
                    handler.errorOccured(context.Channel);
            }
            catch (Exception ex)
            {
                try
                {
                    protocol.SendAlert(AlertLevel.fatal, AlertDescription.decode_error, ex.Message, ex.InnerException);
                }
                catch (Exception)
                {
                }

                if (handler != null)
                    handler.errorOccured(context.Channel);
            }
        }
    }
}