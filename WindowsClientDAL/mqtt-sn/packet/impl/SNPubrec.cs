﻿/**
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

using com.mobius.software.windows.iotbroker.mqtt_sn.packet.api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mobius.software.windows.iotbroker.mqtt_sn.avps;
using com.mobius.software.windows.iotbroker.mqtt.exceptions;
using com.mobius.software.windows.iotbroker.mqtt_sn.headers.api;

namespace com.mobius.software.windows.iotbroker.mqtt_sn.packet.impl
{
    public class SNPubrec: CountableMessage
    {
        #region private fields

        #endregion

        #region constructors

        public SNPubrec()
        {
        }

        public SNPubrec(Int32 messageID) :base(messageID)
        {            
        }

        #endregion

        #region public fields

        public SNPubrec reInit(Int32 messageID)
        {
            this.MessageID = messageID;
            return this;
        }

        public override SNType getType()
        {
            return SNType.PUBREC;
        }

        public override void ProcessBy(SNDevice device)
        {
            device.ProcessPubrec(MessageID.Value);
        }

        #endregion
    }
}