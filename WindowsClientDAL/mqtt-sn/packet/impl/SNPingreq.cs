using com.mobius.software.windows.iotbroker.mqtt_sn.avps;
using com.mobius.software.windows.iotbroker.mqtt_sn.headers.api;
using com.mobius.software.windows.iotbroker.mqtt_sn.packet.api;
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

namespace com.mobius.software.windows.iotbroker.mqtt_sn.packet.impl
{
    public class SNPingreq : SNMessage
    {
        #region private fields

        private String _clientID;

        #endregion

        #region constructors

        public SNPingreq()
        {
        }

        public SNPingreq(String clientID)
        {
            this._clientID = clientID;
        }

        #endregion

        #region public fields

        public SNPingreq reInit(String clientID)
        {
            this._clientID = clientID;
            return this;
        }

        public int getLength()
        {
            int length = 2;

            if (_clientID != null)
                length += _clientID.Length;

            return length;
        }

        public SNType getType()
        {
            return SNType.PINGREQ;
        }

        public void ProcessBy(SNDevice device)
        {
            device.ProcessPingreq();
        }

        public String ClientID
        {
            get
            {
                return _clientID;
            }

            set
            {
                _clientID = value;
            }
        }

        #endregion
    }
}
