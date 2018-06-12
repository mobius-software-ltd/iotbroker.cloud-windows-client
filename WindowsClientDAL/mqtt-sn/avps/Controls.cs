

using com.mobius.software.windows.iotbroker.mqtt.exceptions;
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
    public class Controls
    {
        #region private fields

        private Radius _radius;

        #endregion

        #region constructors

        public Controls(Radius radius)
        {
            this._radius = radius;
        }

        #endregion

        #region public fields

        public static Controls decode(byte ctrlByte)
        {
            if (ctrlByte > 3 || ctrlByte < 0)
                throw new MalformedMessageException("Invalid Encapsulated message control encoding:" + ctrlByte);

            return new Controls((Radius)ctrlByte);
        }

        public static byte encode(Radius radius)
        {
            int ctrlByte = 0;
            ctrlByte |= (int)radius;
            return (byte)ctrlByte;
        }

        public Radius Radius
        {
            get
            {
                return _radius;
            }
        }

        #endregion
    }
}
