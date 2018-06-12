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
    public class GWInfo : SNMessage
    {
        #region private fields

        private Int32 _gwID;
        private String _gwAddress;

        #endregion

        #region constructors

        public GWInfo()
        {
        }

        public GWInfo(Int32 gwID, String gwAddress)
        {
            this._gwID = gwID;
            this._gwAddress = gwAddress;
        }

        #endregion

        #region public fields

        public GWInfo reInit(Int32 gwID, String gwAddress)
        {
            this._gwID = gwID;
            this._gwAddress = gwAddress;
            return this;
        }

        public int getLength()
        {
            int length = 3;
            if (_gwAddress != null)
                length += _gwAddress.Length;

            return length;
        }

        public SNType getType()
        {
            return SNType.GWINFO;
        }

        public void ProcessBy(SNDevice device)
        {
            device.ProcessGwInfo(gwID, gwAddress);
        }

        public Int32 gwID
        {
            get
            {
                return _gwID;
            }

            set
            {
                _gwID = value;
            }            
        }

        public String gwAddress
        {
            get
            {
                return _gwAddress;
            }

            set
            {
                _gwAddress = value;
            }
        }

        #endregion
    }
}