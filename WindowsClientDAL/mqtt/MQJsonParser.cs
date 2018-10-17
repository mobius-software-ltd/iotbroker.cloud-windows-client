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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace com.mobius.software.windows.iotbroker.mqtt
{
    public class MQJsonParser
    {
        public const String JSON_MESSAGE_TYPE_PROPERTY_NAME = "packet";
        JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
        
        public MQJsonParser()
        {
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        public byte[] Encode(MQMessage message)
        {
            String json = JsonConvert.SerializeObject(message, serializerSettings);
		    return Encoding.UTF8.GetBytes(json);
        }

        public String JsonString(MQMessage message)
        {
            String json = JsonConvert.SerializeObject(message, serializerSettings);
            return json;
        }

        public MQMessage Decode(byte[] data)
        {
            String json = Encoding.UTF8.GetString(data);
            JObject parsed = JObject.Parse(json);            
            if (parsed.ContainsKey(JSON_MESSAGE_TYPE_PROPERTY_NAME)) 
            {
                JToken packetProperty = parsed.GetValue(JSON_MESSAGE_TYPE_PROPERTY_NAME);
                MessageType packet = (MessageType)(packetProperty.ToObject<Int32>());
                switch (packet) 
                {
			        case MessageType.CONNECT:
				        return JsonConvert.DeserializeObject<Connect>(json, serializerSettings);
			        case MessageType.CONNACK:
                        return JsonConvert.DeserializeObject<Connack>(json, serializerSettings);
                    case MessageType.PUBLISH:
                        return JsonConvert.DeserializeObject<Publish>(json, serializerSettings);
                    case MessageType.PUBACK:
                        return JsonConvert.DeserializeObject<Puback>(json, serializerSettings);
                    case MessageType.PUBREC:
                        return JsonConvert.DeserializeObject<Pubrec>(json, serializerSettings);
                    case MessageType.PUBREL:
                        return JsonConvert.DeserializeObject<Pubrel>(json, serializerSettings);
                    case MessageType.PUBCOMP:
                        return JsonConvert.DeserializeObject<Pubcomp>(json, serializerSettings);
                    case MessageType.SUBSCRIBE:
                        return JsonConvert.DeserializeObject<Subscribe>(json, serializerSettings);
                    case MessageType.SUBACK:
                        return JsonConvert.DeserializeObject<Suback>(json, serializerSettings);
                    case MessageType.UNSUBSCRIBE:
                        return JsonConvert.DeserializeObject<Unsubscribe>(json, serializerSettings);
                    case MessageType.UNSUBACK:
                        return JsonConvert.DeserializeObject<Unsuback>(json, serializerSettings);
                    case MessageType.PINGREQ:
                        return JsonConvert.DeserializeObject<Pingreq>(json, serializerSettings);
                    case MessageType.PINGRESP:
                        return JsonConvert.DeserializeObject<Pingresp>(json, serializerSettings);
                    case MessageType.DISCONNECT:
                        return JsonConvert.DeserializeObject<Disconnect>(json, serializerSettings);
                    default:
				        throw new MalformedMessageException("Wrong packet type while decoding message from json.");
                }
		    }

		    return null;
	    }
	
	    public MQMessage MessageObject(String json)
        {
		return this.Decode(Encoding.UTF8.GetBytes(json));
        }
    }
}