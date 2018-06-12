

using com.mobius.software.windows.iotbroker.mqtt.exceptions;
using com.mobius.software.windows.iotbroker.mqtt_sn.avps;
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
    public class Flags
    {
        #region private fields

        private Boolean _dup;
        private SNQoS? _qos;
        private Boolean _retain;
        private Boolean _will;
        private Boolean _cleanSession;
        private TopicType? _topicType;

        #endregion

        #region constructors

        public Flags(Boolean dup, SNQoS? qos, Boolean retain, Boolean will, Boolean cleanSession, TopicType? topicType)
        {
            this._dup = dup;
            this._qos = qos;
            this._retain = retain;
            this._will = will;
            this._cleanSession = cleanSession;
            this._topicType = topicType;
        }

        #endregion

        #region public fields

        public static Flags decode(byte flagsByte, SNType type)
        {
            List<Flag> bitMask = new List<Flag>();
            foreach (var flag in Enum.GetValues(typeof(Flag)))
		    {
			    if ((flagsByte & (int)flag) == (int)flag)
				    bitMask.Add((Flag)flag);
		    }

		    return Flags.validateAndCreate(bitMask, type);
	    }

        private static Flags validateAndCreate(List<Flag> bitMask, SNType type)
        {            
            if (bitMask.Contains(Flag.RESERVED_TOPIC))
                throw new MalformedMessageException("Invalid topic type encoding. TopicType reserved bit must not be encoded");

            Boolean dup = bitMask.Contains(Flag.DUPLICATE);
            Boolean retain = bitMask.Contains(Flag.RETAIN);
            Boolean will = bitMask.Contains(Flag.WILL);
            Boolean cleanSession = bitMask.Contains(Flag.CLEAN_SESSION);

            SNQoS? qos = null;
            if (bitMask.Contains(Flag.QOS_LEVEL_ONE))
                qos = SNQoS.LEVEL_ONE;
            else if (bitMask.Contains(Flag.QOS_2))
                qos = SNQoS.EXACTLY_ONCE;
            else if (bitMask.Contains(Flag.QOS_1))
                qos = SNQoS.AT_LEAST_ONCE;
            else
                qos = SNQoS.AT_MOST_ONCE;

            TopicType? topicType = null;
            if (bitMask.Contains(Flag.SHORT_TOPIC))
                topicType = avps.TopicType.SHORT;
            else if (bitMask.Contains(Flag.ID_TOPIC))
                topicType = avps.TopicType.ID;
            else
                topicType = avps.TopicType.NAMED;

            switch (type)
            {
                case SNType.CONNECT:
                    if (dup)
                        throw new MalformedMessageException(type + " invalid encoding: dup flag");
                    if (qos != SNQoS.AT_MOST_ONCE)
                        throw new MalformedMessageException(type + " invalid encoding: qos flag - " + qos);
                    if (retain)
                        throw new MalformedMessageException(type + " invalid encoding: retain flag");
                    if (!topicType.HasValue || topicType != avps.TopicType.NAMED)
                        throw new MalformedMessageException(type + " invalid encoding: topicIdType flag - " + topicType);
                    break;
                case SNType.WILL_TOPIC:
                    if (dup)
                        throw new MalformedMessageException(type + " invalid encoding: dup flag");
                    if (qos == null)
                        throw new MalformedMessageException(type + " invalid encoding: qos flag");
                    if (will)
                        throw new MalformedMessageException(type + " invalid encoding: will flag");
                    if (cleanSession)
                        throw new MalformedMessageException(type + " invalid encoding: cleanSession flag");
                    if (!topicType.HasValue || topicType != avps.TopicType.NAMED)
                        throw new MalformedMessageException(type + " invalid encoding: topicIdType flag - " + topicType);
                    break;

                case SNType.PUBLISH:
                    if (qos == null)
                        throw new MalformedMessageException(type + " invalid encoding: qos flag");
                    if (topicType == null)
                        throw new MalformedMessageException(type + " invalid encoding: topicIdType flag");
                    if (will)
                        throw new MalformedMessageException(type + " invalid encoding: will flag");
                    if (cleanSession)
                        throw new MalformedMessageException(type + " invalid encoding: cleanSession flag");
                    if (dup && (qos == SNQoS.AT_MOST_ONCE || qos == SNQoS.LEVEL_ONE))
                        throw new MalformedMessageException(type + " invalid encoding: dup flag with invalid qos:" + qos);
                    break;

                case SNType.SUBSCRIBE:
                    if (qos == null)
                        throw new MalformedMessageException(type + " invalid encoding: qos flag");
                    if (qos == SNQoS.LEVEL_ONE)
                        throw new MalformedMessageException(type + "invalid encoding: qos " + qos);
                    if (retain)
                        throw new MalformedMessageException(type + " invalid encoding: retain flag");
                    if (will)
                        throw new MalformedMessageException(type + " invalid encoding: will flag");
                    if (cleanSession)
                        throw new MalformedMessageException(type + " invalid encoding: cleanSession flag");
                    if (!topicType.HasValue)
                        throw new MalformedMessageException(type + " invalid encoding: retain flag");
                    break;

                case SNType.SUBACK:
                    if (dup)
                        throw new MalformedMessageException(type + " invalid encoding: dup flag");
                    if (qos == null)
                        throw new MalformedMessageException(type + " invalid encoding: qos flag");
                    if (retain)
                        throw new MalformedMessageException(type + " invalid encoding: retain flag");
                    if (will)
                        throw new MalformedMessageException(type + " invalid encoding: will flag");
                    if (cleanSession)
                        throw new MalformedMessageException(type + " invalid encoding: cleanSession flag");
                    if (!topicType.HasValue || topicType != avps.TopicType.NAMED)
                        throw new MalformedMessageException(type + " invalid encoding: topicIdType flag");
                    break;

                case SNType.UNSUBSCRIBE:
                    if (dup)
                        throw new MalformedMessageException(type + " invalid encoding: dup flag");
                    if (qos != SNQoS.AT_MOST_ONCE)
                        throw new MalformedMessageException(type + " invalid encoding: qos flag");
                    if (retain)
                        throw new MalformedMessageException(type + " invalid encoding: retain flag");
                    if (will)
                        throw new MalformedMessageException(type + " invalid encoding: will flag");
                    if (cleanSession)
                        throw new MalformedMessageException(type + " invalid encoding: cleanSession flag");
                    if (topicType == null)
                        throw new MalformedMessageException(type + " invalid encoding: topicIdType flag");
                    break;

                case SNType.WILL_TOPIC_UPD:
                    if (dup)
                        throw new MalformedMessageException(type + " invalid encoding: dup flag");
                    if (qos == null)
                        throw new MalformedMessageException(type + " invalid encoding: qos flag");
                    if (will)
                        throw new MalformedMessageException(type + " invalid encoding: will flag");
                    if (cleanSession)
                        throw new MalformedMessageException(type + " invalid encoding: cleanSession flag");
                    if (!topicType.HasValue || topicType.Value != avps.TopicType.NAMED)
                        throw new MalformedMessageException(type + " invalid encoding: topicIdType flag");
                    break;

                default:
                    break;
            }
            return new Flags(dup, qos, retain, will, cleanSession, topicType);
        }

        public static byte encode(Boolean dup, SNQoS? qos, Boolean retain, Boolean will, Boolean cleanSession, TopicType? topicType)
        {
            int flagsByte = 0;
            if (dup)
                flagsByte += (int)Flag.DUPLICATE;

            if (qos.HasValue)
                flagsByte += ((int)qos) << 5;

            if (retain)
                flagsByte += (int)Flag.RETAIN;

            if (will)
                flagsByte += (int)Flag.WILL;

            if (cleanSession)
                flagsByte += (int)Flag.CLEAN_SESSION;

            if (topicType.HasValue)
                flagsByte += (int)topicType.Value;

            return (byte)flagsByte;
        }

        public Boolean Dup
        {
            get
            {
                return _dup;
            }
        }

        public SNQoS? Qos
        {
            get
            {
                return _qos;
            }
        }

        public Boolean Retain
        {
            get
            {
                return _retain;
            }
        }

        public Boolean Will
        {
            get
            {
                return _will;
            }
        }

        public Boolean CleanSession
        {
            get
            {
                return _cleanSession;
            }
        }

        public TopicType? TopicType
        {
            get
            {
                return _topicType;
            }
        }

        #endregion
    }
}
