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
 
using com.mobius.software.windows.iotbroker.mqtt.avps;
using com.mobius.software.windows.iotbroker.mqtt.exceptions;
using com.mobius.software.windows.iotbroker.mqtt.headers.api;
using com.mobius.software.windows.iotbroker.mqtt.headers.impl;
using com.mobius.software.windows.iotbroker.mqtt.utils;
using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.mqtt
{
    public class MQParser
    {
        /// <exception cref="MalformedMessageException">Exception is thrown in case of invalid packet format</exception>
        public static IByteBuffer Next(IByteBuffer buf)
        {
            buf.MarkReaderIndex();
            MessageType type = (MessageType)(((buf.ReadByte() >> 4) & 0xf));

            switch (type)
            {
                case MessageType.PINGREQ:
                case MessageType.PINGRESP:
                case MessageType.DISCONNECT:
                    buf.ResetReaderIndex();
                    return Unpooled.Buffer(2);
                default:
                    LengthDetails length = DecodeLength(buf);
                    buf.ResetReaderIndex();
                    if (length.Length == 0)
                        return null;
                    int result = length.Length + length.Size + 1;
                    return result <= buf.ReadableBytes ? Unpooled.Buffer(result) : null;
            }
        }

        /// <exception cref="MalformedMessageException">Exception is thrown in case of invalid packet format</exception>
        public static MQMessage Decode(IByteBuffer buf)
	    {
            MQMessage header = null;

            byte fixedHeader = buf.ReadByte();

            LengthDetails length = DecodeLength(buf);

            MessageType type = (MessageType)((fixedHeader >> 4) & 0xf);
            switch (type)
            {
                case MessageType.CONNECT:

                    Byte[] nameValue = new byte[buf.ReadUnsignedShort()];
                    buf.ReadBytes(nameValue, 0, nameValue.Length);
                    String name = Encoding.UTF8.GetString(nameValue);
                    if (!name.Equals(Connect.PROTOCOL_NAME))
                        throw new MalformedMessageException("CONNECT, protocol-name set to " + name);

                    Byte protocolLevel = buf.ReadByte();
                    Byte contentFlags = buf.ReadByte();

                    Boolean userNameFlag = (((contentFlags >> 7) & 1) == 1) ? true : false;
                    Boolean userPassFlag = (((contentFlags >> 6) & 1) == 1) ? true : false;
                    Boolean willRetain = (((contentFlags >> 5) & 1) == 1) ? true : false;

                    Int32 QOSValue = ((contentFlags & 0x1f) >> 3) & 3;
                    QOS willQos = (QOS)QOSValue;
                    if (!Enum.IsDefined(typeof(QOS), QOSValue))
                        throw new MalformedMessageException("CONNECT, will QoS set to " + willQos);
                    Boolean willFlag = (((contentFlags >> 2) & 1) == 1) ? true : false;

                    if (willQos != QOS.AT_MOST_ONCE && !willFlag)
                        throw new MalformedMessageException("CONNECT, will QoS set to " + willQos + ", willFlag not set");

                    if (willRetain && !willFlag)
                        throw new MalformedMessageException("CONNECT, will retain set, willFlag not set");

                    Boolean cleanSession = (((contentFlags >> 1) & 1) == 1) ? true : false;

                    Boolean reservedFlag = ((contentFlags & 1) == 1) ? true : false;
                    if (reservedFlag)
                        throw new MalformedMessageException("CONNECT, reserved flag set to true");

                    int keepalive = buf.ReadUnsignedShort();

                    Byte[] clientIdValue = new byte[buf.ReadUnsignedShort()];
                    buf.ReadBytes(clientIdValue, 0, clientIdValue.Length);
                    String clientID = Encoding.UTF8.GetString(clientIdValue);
                    if (!StringVerifier.verify(clientID))
                        throw new MalformedMessageException("ClientID contains restricted characters: U+0000, U+D000-U+DFFF");

                    String willTopic = null;
                    Byte[] willMessage = null;
                    String username = null;
                    String password = null;

                    Will will = null;
                    if (willFlag)
                    {
                        if (buf.ReadableBytes < 2)
                            throw new MalformedMessageException("Invalid encoding will/username/password");

                        byte[] willTopicValue = new byte[buf.ReadUnsignedShort()];
                        if (buf.ReadableBytes < willTopicValue.Length)
                            throw new MalformedMessageException("Invalid encoding will/username/password");

                        buf.ReadBytes(willTopicValue, 0, willTopicValue.Length);

                        willTopic = Encoding.UTF8.GetString(willTopicValue);
                        if (!StringVerifier.verify(willTopic))
                            throw new MalformedMessageException("WillTopic contains one or more restricted characters: U+0000, U+D000-U+DFFF");

                        if (buf.ReadableBytes < 2)
                            throw new MalformedMessageException("Invalid encoding will/username/password");

                        willMessage = new byte[buf.ReadUnsignedShort()];
                        if (buf.ReadableBytes < willMessage.Length)
                            throw new MalformedMessageException("Invalid encoding will/username/password");

                        buf.ReadBytes(willMessage, 0, willMessage.Length);
                        if (willTopic.Length == 0)
                            throw new MalformedMessageException("invalid will encoding");
                        will = new Will(new Topic(willTopic, willQos), willMessage, willRetain);
                        if (!will.IsValid())
                            throw new MalformedMessageException("invalid will encoding");
                    }

                    if (userNameFlag)
                    {
                        if (buf.ReadableBytes < 2)
                            throw new MalformedMessageException("Invalid encoding will/username/password");

                        byte[] userNameValue = new byte[buf.ReadUnsignedShort()];
                        if (buf.ReadableBytes < userNameValue.Length)
                            throw new MalformedMessageException("Invalid encoding will/username/password");

                        buf.ReadBytes(userNameValue, 0, userNameValue.Length);
                        username = Encoding.UTF8.GetString(userNameValue);
                        if (!StringVerifier.verify(username))
                            throw new MalformedMessageException("Username contains one or more restricted characters: U+0000, U+D000-U+DFFF");
                    }

                    if (userPassFlag)
                    {
                        if (buf.ReadableBytes < 2)
                            throw new MalformedMessageException("Invalid encoding will/username/password");

                        byte[] userPassValue = new byte[buf.ReadUnsignedShort()];
                        if (buf.ReadableBytes < userPassValue.Length)
                            throw new MalformedMessageException("Invalid encoding will/username/password");

                        buf.ReadBytes(userPassValue, 0, userPassValue.Length);
                        password = Encoding.UTF8.GetString(userPassValue);
                        if (!StringVerifier.verify(password))
                            throw new MalformedMessageException("Password contains one or more restricted characters: U+0000, U+D000-U+DFFF");
                    }

                    if (buf.ReadableBytes > 0)
                        throw new MalformedMessageException("Invalid encoding will/username/password");

                    Connect connect = new Connect(username, password, clientID, cleanSession, keepalive, will);
                    if (protocolLevel != 4)
                        connect.ProtocolLevel = protocolLevel;
                    header = connect;
                    break;

                case MessageType.CONNACK:
                    byte sessionPresentValue = buf.ReadByte();
                    if (sessionPresentValue != 0 && sessionPresentValue != 1)
                        throw new MalformedMessageException("CONNACK, session-present set to " + (sessionPresentValue & 0xff));

                    Boolean isPresent = sessionPresentValue == 1 ? true : false;

                    Int32 connackByte = ((Int32)buf.ReadByte()) & 0xFF;
                    ConnackCode connackCode = (ConnackCode)connackByte;
                    if (!Enum.IsDefined(typeof(ConnackCode), connackByte))
                        throw new MalformedMessageException("Invalid connack code: " + connackByte);
                    header = new Connack(isPresent, connackCode);
                    break;

                case MessageType.PUBLISH:
                    int dataLength = length.Length;
                    fixedHeader &= 0xf;

                    Boolean dup = (((fixedHeader >> 3) & 1) == 1) ? true : false;

                    QOSValue = (fixedHeader & 0x07) >> 1;
                    QOS qos = (QOS)QOSValue;
                    if (!Enum.IsDefined(typeof(QOS), QOSValue))
                        throw new MalformedMessageException("invalid QoS value");
                    if (dup && qos == QOS.AT_MOST_ONCE)
                        throw new MalformedMessageException("PUBLISH, QoS-0 dup flag present");

                    Boolean retain = ((fixedHeader & 1) == 1) ? true : false;

                    byte[] topicNameValue = new byte[buf.ReadUnsignedShort()];
                    buf.ReadBytes(topicNameValue, 0, topicNameValue.Length);
                    String topicName = Encoding.UTF8.GetString(topicNameValue);
                    if (!StringVerifier.verify(topicName))
                        throw new MalformedMessageException("Publish-topic contains one or more restricted characters: U+0000, U+D000-U+DFFF");
                    dataLength -= topicName.Length + 2;

                    Int32? packetID = null;
                    if (qos != QOS.AT_MOST_ONCE)
                    {
                        packetID = buf.ReadUnsignedShort();
                        if (packetID < 0 || packetID > 65535)
                            throw new MalformedMessageException("Invalid PUBLISH packetID encoding");
                        dataLength -= 2;
                    }
                    byte[] data = new byte[dataLength];
                    if (dataLength > 0)
                        buf.ReadBytes(data, 0, data.Length);
                    header = new Publish(packetID, new Topic(topicName, qos), data, retain, dup);
                    break;

                case MessageType.PUBACK:
                    header = new Puback(buf.ReadUnsignedShort());
                    break;

                case MessageType.PUBREC:
                    header = new Pubrec(buf.ReadUnsignedShort());
                    break;

                case MessageType.PUBREL:
                    header = new Pubrel(buf.ReadUnsignedShort());
                    break;

                case MessageType.PUBCOMP:
                    header = new Pubcomp(buf.ReadUnsignedShort());
                    break;

                case MessageType.SUBSCRIBE:
                    Int32 subID = buf.ReadUnsignedShort();
                    List<Topic> subscriptions = new List<Topic>();
                    while (buf.IsReadable())
                    {
                        byte[] value = new byte[buf.ReadUnsignedShort()];
                        buf.ReadBytes(value, 0, value.Length);
                        QOSValue = buf.ReadByte();
                        QOS requestedQos = (QOS)QOSValue;
                        if (!Enum.IsDefined(typeof(QOS), QOSValue))
                            throw new MalformedMessageException("Subscribe qos must be in range from 0 to 2: " + requestedQos);
                        String topic = Encoding.UTF8.GetString(value);
                        if (!StringVerifier.verify(topic))
                            throw new MalformedMessageException("Subscribe topic contains one or more restricted characters: U+0000, U+D000-U+DFFF");
                        Topic subscription = new Topic(topic, requestedQos);
                        subscriptions.Add(subscription);
                    }
                    if (subscriptions.Count == 0)
                        throw new MalformedMessageException("Subscribe with 0 topics");

                    header = new Subscribe(subID, subscriptions.ToArray());
                    break;

                case MessageType.SUBACK:
                    Int32 subackID = buf.ReadUnsignedShort();
                    List<SubackCode> subackCodes = new List<SubackCode>();
                    while (buf.IsReadable())
                    {
                        Int32 subackByte = ((Int32)buf.ReadByte()) & 0xFF;
                        SubackCode subackCode = (SubackCode)subackByte;
                        if (!Enum.IsDefined(typeof(SubackCode), subackByte))
                            throw new MalformedMessageException("Invalid suback code: " + subackByte);
                        subackCodes.Add(subackCode);
                    }
                    if (subackCodes.Count == 0)
                        throw new MalformedMessageException("Suback with 0 return-codes");

                    header = new Suback(subackID, subackCodes);
                    break;

                case MessageType.UNSUBSCRIBE:
                    Int32 unsubID = buf.ReadUnsignedShort();
                    List<String> unsubscribeTopics = new List<String>();
                    while (buf.IsReadable())
                    {
                        byte[] value = new byte[buf.ReadUnsignedShort()];
                        buf.ReadBytes(value, 0, value.Length);
                        String topic = Encoding.UTF8.GetString(value);
                        if (!StringVerifier.verify(topic))
                            throw new MalformedMessageException("Unsubscribe topic contains one or more restricted characters: U+0000, U+D000-U+DFFF");
                        unsubscribeTopics.Add(topic);
                    }
                    if (unsubscribeTopics.Count == 0)
                        throw new MalformedMessageException("Unsubscribe with 0 topics");
                    header = new Unsubscribe(unsubID, unsubscribeTopics.ToArray());
                    break;

                case MessageType.UNSUBACK:
                    header = new Unsuback(buf.ReadUnsignedShort());
                    break;

                case MessageType.PINGREQ:
                    header = new Pingreq();
                    break;
                case MessageType.PINGRESP:
                    header = new Pingresp();
                    break;
                case MessageType.DISCONNECT:
                    header = new Disconnect();
                    break;

                default:
                    throw new MalformedMessageException("Invalid header type: " + type);
            }

            if (buf.IsReadable())
                throw new MalformedMessageException("unexpected bytes in content");

            if (length.Length != header.GetLength())
                throw new MalformedMessageException("Invalid length. Encoded: " + length.Length + ", actual: " + header.GetLength());

            return header;
        }

	    /// <exception cref="MalformedMessageException">Exception is thrown in case of invalid packet format</exception>
        public static IByteBuffer encode(MQMessage header)
	    {
            int length = header.GetLength();
            IByteBuffer buf = GetBuffer(length);
            MessageType type = header.MessageType;

            switch (type)
            {
                case MessageType.CONNECT:
                    Connect connect = (Connect)header;
                    if (connect.Will != null && !connect.Will.IsValid())
                        throw new MalformedMessageException("invalid will encoding");

                    buf.SetByte(0, (byte)(((Int32)type) << 4));
                    buf.WriteShort(Connect.DEFAULT_PROTOCOL_LEVEL);
                    buf.WriteBytes(Encoding.UTF8.GetBytes(connect.Name));
                    buf.WriteByte(connect.ProtocolLevel);
                    byte contentFlags = 0;
                    if (connect.CleanSession)
                        contentFlags += 0x02;

                    if (connect.Will != null)
                    {
                        contentFlags += 0x04;
                        contentFlags += (byte)(((Int32)connect.Will.Topic.Qos) << 3);
                        if (connect.Will.Retain)
                            contentFlags += 0x20;

                    }

                    if (connect.Username != null)
                        contentFlags += 0x40;

                    if (connect.Password != null)
                        contentFlags += 0x80;

                    buf.WriteByte(contentFlags);
                    buf.WriteShort(connect.Keepalive);
                    buf.WriteShort(connect.ClientID.Length);
                    buf.WriteBytes(Encoding.UTF8.GetBytes(connect.ClientID));

                    if (connect.Will != null)
                    {
                        String willTopic = connect.Will.Topic.Name;
                        if (willTopic != null)
                        {
                            buf.WriteShort(willTopic.Length);
                            buf.WriteBytes(Encoding.UTF8.GetBytes(willTopic));
                        }

                        byte[] willMessage = connect.Will.Content;
                        if (willMessage != null)
                        {
                            buf.WriteShort(willMessage.Length);
                            buf.WriteBytes(willMessage);
                        }
                    }

                    String username = connect.Username;
                    if (username != null)
                    {
                        buf.WriteShort(username.Length);
                        buf.WriteBytes(Encoding.UTF8.GetBytes(username));
                    }

                    String password = connect.Password;
                    if (password != null)
                    {
                        buf.WriteShort(password.Length);
                        buf.WriteBytes(Encoding.UTF8.GetBytes(password));
                    }
                    break;

                case MessageType.CONNACK:
                    Connack connack = (Connack)header;
                    buf.SetByte(0, (byte)(((Int32)type) << 4));
                    buf.WriteBoolean(connack.SessionPresent);
                    buf.WriteByte((Int32)connack.ReturnCode);
                    break;

                case MessageType.PUBLISH:
                    Publish publish = (Publish)header;
                    byte firstByte = (byte)(((Int32)type) << 4);
                    if (publish.Dup)
                        firstByte += 0x08;

                    firstByte += (byte)(((Int32)publish.Topic.Qos) << 1);

                    if (publish.Retain)
                        firstByte += 0x01;

                    buf.SetByte(0, firstByte);
                    buf.WriteShort(publish.Topic.Name.Length);
                    buf.WriteBytes(Encoding.UTF8.GetBytes(publish.Topic.Name));
                    if (publish.PacketID.HasValue)
                        buf.WriteShort(publish.PacketID.Value);
                    buf.WriteBytes(publish.Content);
                    break;

                case MessageType.PUBACK:
                    Puback puback = (Puback)header;
                    buf.SetByte(0, (byte)(((Int32)type) << 4));
                    buf.WriteShort(puback.PacketID.Value);
                    break;

                case MessageType.PUBREC:
                    Pubrec pubrec = (Pubrec)header;
                    buf.SetByte(0, (byte)(((Int32)type) << 4));
                    buf.WriteShort(pubrec.PacketID.Value);
                    break;

                case MessageType.PUBREL:
                    Pubrel pubrel = (Pubrel)header;
                    buf.SetByte(0, (byte)((((Int32)type) << 4) | 0x2));
                    buf.WriteShort(pubrel.PacketID.Value);
                    break;

                case MessageType.PUBCOMP:
                    Pubcomp pubcomp = (Pubcomp)header;
                    buf.SetByte(0, (byte)(((Int32)type) << 4));
                    buf.WriteShort(pubcomp.PacketID.Value);
                    break;

                case MessageType.SUBSCRIBE:
                    Subscribe sub = (Subscribe)header;
                    buf.SetByte(0, (byte)((((Int32)type) << 4) | 0x2));
                    buf.WriteShort(sub.PacketID.Value);
                    foreach (Topic subscription in sub.Topics)
                    {
                        buf.WriteShort(subscription.Name.Length);
                        buf.WriteBytes(Encoding.UTF8.GetBytes(subscription.Name));
                        buf.WriteByte((Int32)subscription.Qos);
                    }
                    break;

                case MessageType.SUBACK:
                    Suback suback = (Suback)header;
                    buf.SetByte(0, (byte)(((Int32)type) << 4));
                    buf.WriteShort(suback.PacketID.Value);
                    foreach (SubackCode code in suback.ReturnCodes)
                        buf.WriteByte((Int32)code);
                    break;

                case MessageType.UNSUBSCRIBE:
                    Unsubscribe unsub = (Unsubscribe)header;
                    buf.SetByte(0, (byte)((((Int32)type) << 4) | 0x2));
                    buf.WriteShort(unsub.PacketID.Value);
                    foreach (String topic in unsub.Topics)
                    {
                        buf.WriteShort(topic.Length);
                        buf.WriteBytes(Encoding.UTF8.GetBytes(topic));
                    }
                    break;

                case MessageType.UNSUBACK:
                    Unsuback unsuback = (Unsuback)header;
                    buf.SetByte(0, (byte)(((Int32)type) << 4));
                    buf.WriteShort(unsuback.PacketID.Value);
                    break;

                case MessageType.DISCONNECT:
                case MessageType.PINGREQ:
                case MessageType.PINGRESP:
                    buf.SetByte(0, (byte)(((Int32)type) << 4));
                    break;

                default:
                    throw new MalformedMessageException("Invalid header type: " + type);
            }

            return buf;
        }

        /// <exception cref="MalformedMessageException">Exception is thrown in case of invalid packet length</exception>
        private static IByteBuffer GetBuffer(Int32 length)
        {
            Byte[] lengthBytes;

            if (length <= 127)
                lengthBytes = new Byte[1];
            else if (length <= 16383)
                lengthBytes = new Byte[2];
            else if (length <= 2097151)
                lengthBytes = new Byte[3];
            else if (length <= 26843545)
                lengthBytes = new Byte[4];
            else
                throw new MalformedMessageException("header length exceeds maximum of 26843545 bytes");

            Byte encByte;
            Int32 pos = 0, l = length;
            do
            {
                encByte = (byte)(l % 128);
                l /= 128;
                if (l > 0)
                    lengthBytes[pos++] = (byte)(encByte | 128);
                else
                    lengthBytes[pos++] = encByte;
            }
            while (l > 0);

            int bufferSize = 1 + lengthBytes.Length + length;
            IByteBuffer buf = Unpooled.Buffer(bufferSize);

            buf.WriteByte(0);
            buf.WriteBytes(lengthBytes);

            return buf;
        }

        /// <exception cref="MalformedMessageException">Exception is thrown in case of invalid packet length</exception>
        private static LengthDetails DecodeLength(IByteBuffer buf)
        {
            Int32 length = 0, multiplier = 1;
            Int32 bytesUsed = 0;
            Byte enc = 0;
            do
            {
                if (multiplier > 128 * 128 * 128)
                    throw new MalformedMessageException("Encoded length exceeds maximum of 268435455 bytes");

                if (!buf.IsReadable())
                    return new LengthDetails(0, 0);

                enc = buf.ReadByte();
                length += (enc & 0x7f) * multiplier;
                multiplier *= 128;
                bytesUsed++;
            }
            while ((enc & 0x80) != 0);

            return new LengthDetails(length, bytesUsed);
        }
    }
}