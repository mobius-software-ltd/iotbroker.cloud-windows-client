using com.mobius.software.windows.iotbroker.coap.avps;
using com.mobius.software.windows.iotbroker.mqtt_sn;
using com.mobius.software.windows.iotbroker.mqtt_sn.headers.api;
using com.mobius.software.windows.iotbroker.mqtt_sn.packet.api;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
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

namespace com.mobius.software.windows.iotbroker.mqtt_sn.net
{
    public class SNEncoder : MessageToMessageEncoder<SNMessage>
    {
        private IChannel _channel;

        public SNEncoder(IChannel channel)
        {
            _channel = channel;
        }

        protected override void Encode(IChannelHandlerContext context, SNMessage message, List<object> output)
        {
            IByteBuffer buffer = SNParser.encode(message);
            output.Add(new DatagramPacket(buffer, _channel.LocalAddress, _channel.RemoteAddress));
        }
    }
}
