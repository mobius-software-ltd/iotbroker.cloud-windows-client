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
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.mqtt_sn.avps
{
    public enum SNType : Int32 { ADVERTISE = 0x00, SEARCHGW = 0x01, GWINFO = 0x02, CONNECT = 0x04, CONNACK = 0x05, WILL_TOPIC_REQ = 0x06, WILL_TOPIC = 0x07, WILL_MSG_REQ = 0x08, WILL_MSG = 0x09, REGISTER = 0x0A, REGACK = 0x0B, PUBLISH = 0x0C, PUBACK = 0x0D, PUBCOMP = 0x0E, PUBREC = 0x0F, PUBREL = 0x10, SUBSCRIBE = 0x12, SUBACK = 0x13, UNSUBSCRIBE = 0x14, UNSUBACK = 0x15, PINGREQ = 0x16, PINGRESP = 0x17, DISCONNECT = 0x18, WILL_TOPIC_UPD = 0x1A, WILL_TOPIC_RESP = 0x1B, WILL_MSG_UPD = 0x1C, WILL_MSG_RESP = 0x1D, ENCAPSULATED = 0xFE };
}
