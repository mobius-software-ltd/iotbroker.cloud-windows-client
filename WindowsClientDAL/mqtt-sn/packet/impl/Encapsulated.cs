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

using com.mobius.software.windows.iotbroker.mqtt_sn.packet.api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mobius.software.windows.iotbroker.mqtt_sn.avps;
using com.mobius.software.windows.iotbroker.mqtt_sn.headers.api;

namespace com.mobius.software.windows.iotbroker.mqtt_sn.packet.impl
{
    public class Encapsulated : SNMessage
    {
        #region private fields

        private Radius _radius;
        private String _wirelessNodeID;
        private SNMessage _message;

        #endregion

        #region constructors

        public Encapsulated()
        {
        }

        public Encapsulated(Radius radius, String wirelessNodeID, SNMessage message)
        {
            this._radius = radius;
            this._wirelessNodeID = wirelessNodeID;
            this._message = message;
        }

        #endregion

        #region public fields

        public Encapsulated reInit(Radius radius, String wirelessNodeID, SNMessage message)
        {
            this._radius = radius;
            this._wirelessNodeID = wirelessNodeID;
            this._message = message;
            return this;
        }

        public int getLength()
        {
            int length = 3;
            if (_wirelessNodeID != null)
                length += _wirelessNodeID.Length;

            if (_message != null)
                length += _message.getLength();

            return length;
        }

        public SNType getType()
        {
            return SNType.ENCAPSULATED;
        }

        public void ProcessBy(SNDevice device)
        {
            
        }

        public Radius radius
        {
            get
            {
                return _radius;
            }

            set
            {
                _radius = value;
            }            
        }

        public String wirelessNodeID
        {
            get
            {
                return _wirelessNodeID;
            }

            set
            {
                _wirelessNodeID = value;
            }
        }

        public SNMessage message
        {
            get
            {
                return _message;
            }

            set
            {
                _message = value;
            }
        }

        #endregion
    }
}