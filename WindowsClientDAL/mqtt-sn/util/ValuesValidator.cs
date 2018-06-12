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

using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.mqtt_sn.util
{
    public class ValuesValidator
    {
        private static HashSet<Int32> RESERVED_MESSAGE_IDS = new HashSet<Int32>(new Int32[] { 0x0000 });
        private static HashSet<Int32> RESERVED_TOPIC_IDS = new HashSet<Int32>(new Int32[] { 0x0000, 0xFFFF });

        public static Boolean validateMessageID(int messageID)
        {
            return messageID > 0 && !RESERVED_MESSAGE_IDS.Contains(messageID);
        }

        public static Boolean validateTopicID(int topicID)
        {
            return topicID > 0 && !RESERVED_TOPIC_IDS.Contains(topicID);
        }

        public static Boolean validateRegistrationTopicID(int topicID)
        {
            return topicID >= 0;
        }

        public static Boolean canRead(IByteBuffer buf, int bytesLeft)
        {
            return buf.IsReadable() && bytesLeft > 0;
        }

        public static Boolean validateClientID(String clientID)
        {
            return clientID != null && clientID.Length>0;
        }
    }
}
