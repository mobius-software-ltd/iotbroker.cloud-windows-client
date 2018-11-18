using com.mobius.software.windows.iotbroker.mqtt.exceptions;
using com.mobius.software.windows.iotbroker.mqtt_sn.avps;
using com.mobius.software.windows.iotbroker.mqtt_sn.headers.api;
using com.mobius.software.windows.iotbroker.mqtt_sn.packet.api;
using com.mobius.software.windows.iotbroker.mqtt_sn.packet.impl;
using com.mobius.software.windows.iotbroker.mqtt_sn.util;
using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

namespace com.mobius.software.windows.iotbroker.mqtt_sn
{
    public class SNParser
    {
        private static byte THREE_OCTET_LENGTH_SUFFIX = 0x01;

        public static WillTopicReq WILL_TOPIC_REQ = new WillTopicReq();
        public static WillMsgReq WILL_MSG_REQ = new WillMsgReq();
        public static SNPingresp PING_RESP = new SNPingresp();
        public static SNDisconnect DISCONNECT = new SNDisconnect();

        public static SNMessage decode(IByteBuffer buf)
        {
            SNMessage message = null;
            int currIndex = buf.ReaderIndex;
            int messageLength = decodeContentLength(buf);
            int bytesLeft = messageLength - (buf.ReaderIndex - currIndex);

            short typeByte = (short)(0x00FF & (short)buf.ReadByte());
            SNType? type = null;
            try
            {
                type = (SNType)(int)(typeByte);
            }
            catch (Exception)
            {
                throw new MalformedMessageException("invalid packet type encoding:" + typeByte);
            }

            bytesLeft--;

            switch (type.Value)
            {
                case SNType.ADVERTISE:
                    int advertiseGwID = 0x00FF & (short)buf.ReadByte();
                    int advertiseDuration = buf.ReadUnsignedShort();
                    message = new Advertise(advertiseGwID, advertiseDuration);
                    break;

                case SNType.SEARCHGW:
                    int radius = 0x00FF & (short)buf.ReadByte();
                    message = new SearchGW((Radius)radius);
                    break;

                case SNType.GWINFO:
                    int gwInfoGwID = 0x00FF & (short)buf.ReadByte();
                    bytesLeft--;
                    String gwInfoGwAddress = null;
                    if (bytesLeft > 0)
                    {
                        byte[] gwInfoGwAddressBytes = new byte[bytesLeft];
                        buf.ReadBytes(gwInfoGwAddressBytes);
                        gwInfoGwAddress = Encoding.UTF8.GetString(gwInfoGwAddressBytes);
                    }
                    message = new GWInfo(gwInfoGwID, gwInfoGwAddress);
                    break;

                case SNType.CONNECT:
                    Flags connectFlags = Flags.decode(buf.ReadByte(), type.Value);
                    bytesLeft--;
                    int protocolID = buf.ReadByte();
                    bytesLeft--;
                    if (protocolID != SNConnect.MQTT_SN_PROTOCOL_ID)
                        throw new MalformedMessageException("Invalid protocolID " + protocolID);
                    int connectDuration = buf.ReadUnsignedShort();
                    bytesLeft -= 2;
                    if (!ValuesValidator.canRead(buf, bytesLeft))
                        throw new MalformedMessageException(type + ", clientID can't be empty");
                    byte[] connectClientIDBytes = new byte[bytesLeft];
                    buf.ReadBytes(connectClientIDBytes);
                    String connectClientID = Encoding.UTF8.GetString(connectClientIDBytes);
                    message = new SNConnect(connectFlags.CleanSession, connectDuration, connectClientID, connectFlags.Will);
                    break;

                case SNType.CONNACK:
                    ReturnCode connackCode = (ReturnCode)(int)(buf.ReadByte());
                    message = new SNConnack(connackCode);
                    break;

                case SNType.WILL_TOPIC_REQ:
                    message = WILL_TOPIC_REQ;
                    break;

                case SNType.WILL_TOPIC:
                    Boolean willTopicRetain = false;
                    FullTopic willTopic = null;
                    if (bytesLeft > 0)
                    {
                        Flags willTopicFlags = Flags.decode(buf.ReadByte(), type.Value);
                        bytesLeft--;
                        willTopicRetain = willTopicFlags.Retain;
                        if (!ValuesValidator.canRead(buf, bytesLeft))
                            throw new MalformedMessageException(type + " invalid topic encoding");
                        byte[] willTopicBytes = new byte[bytesLeft];
                        buf.ReadBytes(willTopicBytes);
                        String willTopicValue = Encoding.UTF8.GetString(willTopicBytes);
                        willTopic = new FullTopic(willTopicValue, willTopicFlags.Qos.Value);
                    }
                    message = new WillTopic(willTopicRetain, willTopic);
                    break;

                case SNType.WILL_MSG_REQ:
                    message = WILL_MSG_REQ;
                    break;

                case SNType.WILL_MSG:
                    if (!ValuesValidator.canRead(buf, bytesLeft))
                        throw new MalformedMessageException(type + " content must not be empty");
                    byte[] willMessageContent = new byte[bytesLeft];
                    buf.ReadBytes(willMessageContent);
                    message = new WillMsg(willMessageContent);
                    break;

                case SNType.REGISTER:
                    int registerTopicID = buf.ReadUnsignedShort();
                    if (!ValuesValidator.validateRegistrationTopicID(registerTopicID))
                        throw new MalformedMessageException(type + " invalid topicID value " + registerTopicID);
                    bytesLeft -= 2;
                    int registerMessageID = buf.ReadUnsignedShort();
                    if (!ValuesValidator.validateMessageID(registerMessageID))
                        throw new MalformedMessageException(type + " invalid messageID " + registerMessageID);
                    bytesLeft -= 2;
                    if (!ValuesValidator.canRead(buf, bytesLeft))
                        throw new MalformedMessageException(type + " must contain a valid topic");
                    byte[] registerTopicBytes = new byte[bytesLeft];
                    buf.ReadBytes(registerTopicBytes);
                    String registerTopicName = Encoding.UTF8.GetString(registerTopicBytes);
                    message = new Register(registerTopicID, registerMessageID, registerTopicName);
                    break;

                case SNType.REGACK:
                    int regackTopicID = buf.ReadUnsignedShort();
                    if (!ValuesValidator.validateRegistrationTopicID(regackTopicID))
                        throw new MalformedMessageException(type + " invalid topicID value " + regackTopicID);
                    int regackMessageID = buf.ReadUnsignedShort();
                    if (!ValuesValidator.validateMessageID(regackMessageID))
                        throw new MalformedMessageException(type + " invalid messageID " + regackMessageID);
                    ReturnCode regackCode = (ReturnCode)(int)(buf.ReadByte());
                    message = new Regack(regackTopicID, regackMessageID, regackCode);
                    break;

                case SNType.PUBLISH:
                    Flags publishFlags = Flags.decode(buf.ReadByte(), type.Value);
                    bytesLeft--;
                    int publishTopicID = buf.ReadUnsignedShort();
                    bytesLeft -= 2;
                    int publishMessageID = buf.ReadUnsignedShort();
                    bytesLeft -= 2;
                    if (publishFlags.Qos.Value != SNQoS.AT_MOST_ONCE && publishMessageID == 0)
                        throw new MalformedMessageException("invalid PUBLISH QoS-0 messageID:" + publishMessageID);
                    SNTopic publishTopic = null;
                    if (publishFlags.TopicType == TopicType.SHORT)
                        publishTopic = new ShortTopic(publishTopicID.ToString(), publishFlags.Qos.Value);
                    else
                    {
                        if (!ValuesValidator.validateTopicID(publishTopicID))
                            throw new MalformedMessageException(type + " invalid topicID value " + publishTopicID);
                        publishTopic = new IdentifierTopic(publishTopicID, publishFlags.Qos.Value);
                    }

                    byte[] data = new byte[bytesLeft];
                    if (bytesLeft > 0)
                        buf.ReadBytes(data);
                    
                    message = new SNPublish(publishMessageID, publishTopic, data, publishFlags.Dup, publishFlags.Retain);
                    break;

                case SNType.PUBACK:
                    int pubackTopicID = buf.ReadUnsignedShort();
                    if (!ValuesValidator.validateTopicID(pubackTopicID))
                        throw new MalformedMessageException(type + " invalid topicID value " + pubackTopicID);
                    int pubackMessageID = buf.ReadUnsignedShort();
                    if (!ValuesValidator.validateMessageID(pubackMessageID))
                        throw new MalformedMessageException(type + " invalid messageID " + pubackMessageID);
                    ReturnCode pubackCode = (ReturnCode)(int)(buf.ReadByte());
                    message = new SNPuback(pubackTopicID, pubackMessageID, pubackCode);
                    break;

                case SNType.PUBREC:
                    int pubrecMessageID = buf.ReadUnsignedShort();
                    if (!ValuesValidator.validateMessageID(pubrecMessageID))
                        throw new MalformedMessageException(type + " invalid messageID " + pubrecMessageID);
                    message = new SNPubrec(pubrecMessageID);
                    break;

                case SNType.PUBREL:
                    int pubrelMessageID = buf.ReadUnsignedShort();
                    if (!ValuesValidator.validateMessageID(pubrelMessageID))
                        throw new MalformedMessageException(type + " invalid messageID " + pubrelMessageID);
                    message = new SNPubrel(pubrelMessageID);
                    break;

                case SNType.PUBCOMP:
                    int pubcompMessageID = buf.ReadUnsignedShort();
                    if (!ValuesValidator.validateMessageID(pubcompMessageID))
                        throw new MalformedMessageException(type + " invalid messageID " + pubcompMessageID);
                    message = new SNPubcomp(pubcompMessageID);
                    break;

                case SNType.SUBSCRIBE:
                    Flags subscribeFlags = Flags.decode(buf.ReadByte(), type.Value);
                    bytesLeft--;
                    int subscribeMessageID = buf.ReadUnsignedShort();
                    if (subscribeMessageID == 0)
                        throw new MalformedMessageException(type + " invalid messageID " + subscribeMessageID);
                    bytesLeft -= 2;
                    if (!ValuesValidator.canRead(buf, bytesLeft) || bytesLeft < 2)
                        throw new MalformedMessageException(type + " invalid topic encoding");
                    byte[] subscribeTopicBytes = new byte[bytesLeft];
                    buf.ReadBytes(subscribeTopicBytes);
                    SNTopic subscribeTopic = null;
                    switch (subscribeFlags.TopicType)
                    {
                        case TopicType.NAMED:
                            String subscribeTopicName = Encoding.UTF8.GetString(subscribeTopicBytes);
                            subscribeTopic = new FullTopic(subscribeTopicName, subscribeFlags.Qos.Value);
                            break;
                        case TopicType.ID:
                            int subscribeTopicID = BitConverter.ToInt16(subscribeTopicBytes, 0);
                            if (!ValuesValidator.validateTopicID(subscribeTopicID))
                                throw new MalformedMessageException(type + " invalid topicID value " + subscribeTopicID);
                            subscribeTopic = new IdentifierTopic(subscribeTopicID, subscribeFlags.Qos.Value);
                            break;
                        case TopicType.SHORT:
                            String subscribeTopicShortName = Encoding.UTF8.GetString(subscribeTopicBytes);
                            subscribeTopic = new ShortTopic(subscribeTopicShortName, subscribeFlags.Qos.Value);
                            break;
                    }
                    message = new SNSubscribe(subscribeMessageID, subscribeTopic, subscribeFlags.Dup);
                    break;

                case SNType.SUBACK:
                    Flags subackFlags = Flags.decode(buf.ReadByte(), type.Value);
                    int subackTopicID = buf.ReadUnsignedShort();
                    if (!ValuesValidator.validateRegistrationTopicID(subackTopicID))
                        throw new MalformedMessageException(type + " invalid topicID value " + subackTopicID);
                    int subackMessageID = buf.ReadUnsignedShort();
                    if (!ValuesValidator.validateMessageID(subackMessageID))
                        throw new MalformedMessageException(type + " invalid messageID " + subackMessageID);
                    ReturnCode subackCode = (ReturnCode)(int)(buf.ReadByte());
                    message = new SNSuback(subackTopicID, subackMessageID, subackCode, subackFlags.Qos.Value);
                    break;

                case SNType.UNSUBSCRIBE:
                    Flags unsubscribeFlags = Flags.decode(buf.ReadByte(), type.Value);
                    bytesLeft--;
                    int unsubscribeMessageID = buf.ReadUnsignedShort();
                    if (!ValuesValidator.validateMessageID(unsubscribeMessageID))
                        throw new MalformedMessageException(type + " invalid messageID " + unsubscribeMessageID);
                    bytesLeft -= 2;
                    byte[] unsubscribeTopicBytes = new byte[bytesLeft];
                    buf.ReadBytes(unsubscribeTopicBytes);
                    SNTopic unsubscribeTopic = null;
                    switch (unsubscribeFlags.TopicType)
                    {
                        case TopicType.NAMED:
                            String unsubscribeTopicName = Encoding.UTF8.GetString(unsubscribeTopicBytes);
                            unsubscribeTopic = new FullTopic(unsubscribeTopicName, unsubscribeFlags.Qos.Value);
                            break;
                        case TopicType.ID:
                            int unsubscribeTopicID = BitConverter.ToInt16(unsubscribeTopicBytes, 0);
                            if (!ValuesValidator.validateTopicID(unsubscribeTopicID))
                                throw new MalformedMessageException(type + " invalid topicID value " + unsubscribeTopicID);
                            unsubscribeTopic = new IdentifierTopic(unsubscribeTopicID, unsubscribeFlags.Qos.Value);
                            break;
                        case TopicType.SHORT:
                            String unsubscribeTopicShortName = Encoding.UTF8.GetString(unsubscribeTopicBytes);
                            unsubscribeTopic = new ShortTopic(unsubscribeTopicShortName, unsubscribeFlags.Qos.Value);
                            break;
                    }
                    message = new SNUnsubscribe(unsubscribeMessageID, unsubscribeTopic);
                    break;

                case SNType.UNSUBACK:
                    int unsubackMessageID = buf.ReadUnsignedShort();
                    if (!ValuesValidator.validateMessageID(unsubackMessageID))
                        throw new MalformedMessageException(type + " invalid messageID " + unsubackMessageID);
                    message = new SNUnsuback(unsubackMessageID);
                    break;

                case SNType.PINGREQ:
                    String pingreqClientID = null;
                    if (bytesLeft > 0)
                    {
                        byte[] pingreqClientIDValue = new byte[bytesLeft];
                        buf.ReadBytes(pingreqClientIDValue);
                        pingreqClientID = Encoding.UTF8.GetString(pingreqClientIDValue);
                    }
                    message = new SNPingreq(pingreqClientID);
                    break;

                case SNType.PINGRESP:
                    message = PING_RESP;
                    break;

                case SNType.DISCONNECT:
                    int duration = 0;
                    if (bytesLeft > 0)
                        duration = buf.ReadUnsignedShort();
                    message = new SNDisconnect(duration);
                    break;

                case SNType.WILL_TOPIC_UPD:
                    FullTopic willTopicUpdTopic = null;
                    Boolean willTopicUpdateRetain = false;
                    if (bytesLeft > 0)
                    {
                        Flags willTopicUpdFlags = Flags.decode(buf.ReadByte(), type.Value);
                        willTopicUpdateRetain = willTopicUpdFlags.Retain;
                        bytesLeft--;
                        byte[] willTopicUpdTopicBytes = new byte[bytesLeft];
                        buf.ReadBytes(willTopicUpdTopicBytes);
                        String willTopicUpdTopicValue = Encoding.UTF8.GetString(willTopicUpdTopicBytes);
                        willTopicUpdTopic = new FullTopic(willTopicUpdTopicValue, willTopicUpdFlags.Qos.Value);
                    }
                    message = new WillTopicUpd(willTopicUpdateRetain, willTopicUpdTopic);
                    break;

                case SNType.WILL_MSG_UPD:
                    if (!ValuesValidator.canRead(buf, bytesLeft))
                        throw new MalformedMessageException(type + " must contain content data");
                    byte[] willMsgUpdContent = new byte[bytesLeft];
                    buf.ReadBytes(willMsgUpdContent);
                    message = new WillMsgUpd(willMsgUpdContent);
                    break;

                case SNType.WILL_TOPIC_RESP:
                    ReturnCode willTopicRespCode = (ReturnCode)(int)buf.ReadByte();
                    message = new WillTopicResp(willTopicRespCode);
                    break;

                case SNType.WILL_MSG_RESP:
                    ReturnCode willMsgRespCode = (ReturnCode)(int)buf.ReadByte();
                    message = new WillMsgResp(willMsgRespCode);
                    break;

                case SNType.ENCAPSULATED:
                    Controls control = Controls.decode(buf.ReadByte());
                    bytesLeft--;
                    byte[] wirelessNodeIDBytes = new byte[bytesLeft];
                    buf.ReadBytes(wirelessNodeIDBytes);
                    String wirelessNodeID = Encoding.UTF8.GetString(wirelessNodeIDBytes);
                    SNMessage encapsulatedMessage = SNParser.decode(buf);
                    message = new Encapsulated(control.Radius, wirelessNodeID, encapsulatedMessage);
                    break;
            }

            if (buf.IsReadable())
                throw new MalformedMessageException("not all bytes have been read from buffer:" + buf.ReadableBytes);

            if (messageLength != message.getLength())
                throw new MalformedMessageException(String.Format("Invalid length. Encoded: %d, actual: %d", messageLength, message.getLength()));

            return message;
        }

        private static int decodeContentLength(IByteBuffer buf)
        {
            int length = 0;
            short firstLengthByte = (short)(0x00FF & (short)buf.ReadByte());
            if (firstLengthByte == THREE_OCTET_LENGTH_SUFFIX)
                length = buf.ReadUnsignedShort();
            else
                length = firstLengthByte;
            return length;
        }

        public static IByteBuffer encode(SNMessage message)
        {
            int length = message.getLength();
            IByteBuffer buf = Unpooled.Buffer(length);
            if (length <= 255)
                buf.WriteByte(length);
            else
            {
                buf.WriteByte(THREE_OCTET_LENGTH_SUFFIX);
                buf.WriteShort(length);
            }
            SNType type = message.getType();
            buf.WriteByte((byte)(int)type);

            switch (type)
            {
                case SNType.ADVERTISE:
                    Advertise advertise = (Advertise)message;
                    buf.WriteByte(advertise.gwID);
                    buf.WriteShort(advertise.duration);
                    break;

                case SNType.SEARCHGW:
                    SearchGW searchGw = (SearchGW)message;
                    buf.WriteByte((byte)(int)searchGw.Radius);
                    break;

                case SNType.GWINFO:
                    GWInfo gwInfo = (GWInfo)message;
                    buf.WriteByte(gwInfo.gwID);
                    if (gwInfo.gwAddress != null)
                        buf.WriteBytes(Encoding.UTF8.GetBytes(gwInfo.gwAddress));
                    break;

                case SNType.CONNECT:
                    SNConnect connect = (SNConnect)message;
                    byte connectFlagsByte = Flags.encode(false, null, false, connect.WillPresent, connect.CleanSession, null);
                    buf.WriteByte(connectFlagsByte);
                    buf.WriteByte(connect.ProtocolID);
                    buf.WriteShort(connect.Duration);
                    buf.WriteBytes(Encoding.UTF8.GetBytes(connect.ClientID));
                    break;

                case SNType.CONNACK:
                case SNType.WILL_TOPIC_RESP:
                case SNType.WILL_MSG_RESP:
                    ResponseMessage responseMessage = (ResponseMessage)message;
                    buf.WriteByte((byte)(int)responseMessage.ReturnCode);
                    break;

                case SNType.WILL_TOPIC:
                    WillTopic willTopic = (WillTopic)message;
                    if (willTopic.Topic != null)
                    {
                        byte willTopicFlagsByte = Flags.encode(false, willTopic.Topic.getQos(), willTopic.Retain, false, false, willTopic.Topic.getType());
                        buf.WriteByte(willTopicFlagsByte);
                        buf.WriteBytes(Encoding.UTF8.GetBytes(willTopic.Topic.Value));
                    }
                    break;

                case SNType.WILL_MSG:
                    WillMsg willMsg = (WillMsg)message;
                    buf.WriteBytes(willMsg.Content);
                    break;

                case SNType.REGISTER:
                    Register register = (Register)message;
                    buf.WriteShort(register.topicID);
                    buf.WriteShort(register.MessageID.Value);
                    buf.WriteBytes(Encoding.UTF8.GetBytes(register.TopicName));
                    break;

                case SNType.REGACK:
                    Regack regack = (Regack)message;
                    buf.WriteShort(regack.topicID);
                    buf.WriteShort(regack.MessageID.Value);
                    buf.WriteByte((byte)(int)regack.code);
                    break;

                case SNType.PUBLISH:
                    SNPublish publish = (SNPublish)message;
                    byte publishFlagsByte = Flags.encode(publish.Dup, publish.SnTopic.getQos(), publish.Retain, false, false, publish.SnTopic.getType());
                    buf.WriteByte(publishFlagsByte);
                    buf.WriteBytes(publish.SnTopic.encode());
                    buf.WriteShort(publish.MessageID.Value);
                    buf.WriteBytes(publish.Content);
                    break;

                case SNType.PUBACK:
                    SNPuback puback = (SNPuback)message;
                    buf.WriteShort(puback.topicID);
                    buf.WriteShort(puback.MessageID.Value);
                    buf.WriteByte((byte)(int)puback.ReturnCode);
                    break;

                case SNType.PUBREC:
                case SNType.PUBREL:
                case SNType.PUBCOMP:
                case SNType.UNSUBACK:
                    CountableMessage contableMessage = (CountableMessage)message;
                    buf.WriteShort(contableMessage.MessageID.Value);
                    break;

                case SNType.SUBSCRIBE:
                    SNSubscribe subscribe = (SNSubscribe)message;
                    byte subscribeFlags = Flags.encode(subscribe.Dup, subscribe.SnTopic.getQos(), false, false, false, subscribe.SnTopic.getType());
                    buf.WriteByte(subscribeFlags);
                    buf.WriteShort(subscribe.MessageID.Value);
                    buf.WriteBytes(subscribe.SnTopic.encode());
                    break;

                case SNType.SUBACK:
                    SNSuback suback = (SNSuback)message;
                    byte subackByte = Flags.encode(false, suback.AllowedQos, false, false, false, null);
                    buf.WriteByte(subackByte);
                    buf.WriteShort(suback.topicID);
                    buf.WriteShort(suback.MessageID.Value);
                    buf.WriteByte((byte)(int)suback.ReturnCode);
                    break;

                case SNType.UNSUBSCRIBE:
                    SNUnsubscribe unsubscribe = (SNUnsubscribe)message;
                    byte unsubscribeFlags = Flags.encode(false, null, false, false, false, unsubscribe.SnTopic.getType());
                    buf.WriteByte(unsubscribeFlags);
                    buf.WriteShort(unsubscribe.MessageID.Value);
                    buf.WriteBytes(unsubscribe.SnTopic.encode());
                    break;

                case SNType.PINGREQ:
                    if (length > 2)
                    {
                        SNPingreq pingreq = (SNPingreq)message;
                        buf.WriteBytes(Encoding.UTF8.GetBytes(pingreq.ClientID));
                    }
                    break;

                case SNType.DISCONNECT:
                    if (length > 2)
                    {
                        SNDisconnect disconnect = (SNDisconnect)message;
                        buf.WriteShort(disconnect.Duration);
                    }
                    break;

                case SNType.WILL_TOPIC_UPD:
                    WillTopicUpd willTopicUpd = (WillTopicUpd)message;
                    if (willTopicUpd.Topic != null)
                    {
                        byte willTopicUpdByte = Flags.encode(false, willTopicUpd.Topic.getQos(), willTopicUpd.Retain, false, false, null);
                        buf.WriteByte(willTopicUpdByte);
                        buf.WriteBytes(Encoding.UTF8.GetBytes(willTopicUpd.Topic.Value));
                    }
                    break;

                case SNType.WILL_MSG_UPD:
                    WillMsgUpd willMsgUpd = (WillMsgUpd)message;
                    buf.WriteBytes(willMsgUpd.Content);
                    break;

                case SNType.WILL_TOPIC_REQ:
                case SNType.WILL_MSG_REQ:
                case SNType.PINGRESP:
                    break;

                case SNType.ENCAPSULATED:
                    Encapsulated encapsulated = (Encapsulated)message;
                    
                    buf.WriteByte(Controls.encode(encapsulated.radius));
                    buf.WriteBytes(Encoding.UTF8.GetBytes(encapsulated.wirelessNodeID));
                    buf.WriteBytes(SNParser.encode(encapsulated.message));
                    break;

                default:
                    break;
            }

            if (type != SNType.ENCAPSULATED && message.getLength() != buf.ReadableBytes)
                throw new MalformedMessageException("invalid message encoding: expected length-" + message.getLength() + ",actual-" + buf.ReadableBytes);

            return buf;
        }
    }
}
