

using com.mobius.software.windows.iotbroker.mqtt_sn.avps;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.mqtt_sn.headers.api
{
    public interface SNDevice
    {
        void ProcessConnect(Boolean cleanSession, Int32 keepalive);

        void ProcessConnack(ReturnCode code);

        void ProcessSubscribe(Int32 packetID, SNTopic topics);

        void ProcessSuback(Int32 packetID, Int32 topicID, ReturnCode returnCode, SNQoS allowedQos);

        void ProcessUnsubscribe(Int32 packetID, SNTopic topics);

        void ProcessUnsuback(Int32 packetID);

        void ProcessPublish(Int32? packetID, SNTopic topic, Byte[] content, Boolean retain, Boolean isDup);

        void ProcessPuback(Int32 packetID);

        void ProcessPubrec(Int32 packetID);

        void ProcessPubrel(Int32 packetID);

        void ProcessPubcomp(Int32 packetID);

        void ProcessPingreq();

        void ProcessPingresp();

        void ProcessDisconnect();

        void ProcessWillTopicRequest();

        void ProcessWillMessageRequest();

        void ProcessWillTopic(FullTopic topic);

        void ProcessWillMessage(byte[] _content);

        void ProcessWillTopicUpdate(FullTopic willTopic);

        void ProcessWillMessageUpdate(byte[] _content);

        void ProcessWillTopicResponse();

        void ProcessWillMessageResponse();

        void ProcessAdvertise(Int32 gatewayID, Int32 duration);

        void ProcessGwInfo(Int32 gatewayID, String gatewayAddress);

        void ProcessSearchGw(Radius radius);

        void ProcessRegister(Int32 packetID, Int32 topicID,String topicName);

        void ProcessRegack(Int32 packetID, Int32 topicID, ReturnCode returnCode);
    }
}
