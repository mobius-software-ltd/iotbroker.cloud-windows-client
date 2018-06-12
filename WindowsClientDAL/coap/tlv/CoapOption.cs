

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

namespace com.mobius.software.windows.iotbroker.coap.avps
{
    public class CoapOption
    {
        #region private fields

        private Int32 _number;
        private Int32 _length;
        private byte[] _value;
        
        #endregion

        #region constructors

        public CoapOption(Int32 number, Int32 length, byte[] value)
        {
            this._number = number;
            this._length = length;
            this._value = value;
        }

        #endregion

        #region public fields

        public Int32 Number
        {
            get
            {
                return _number;
            }

            set
            {
                this._number = value;
            }
        }

        public Int32 Length
        {
            get
            {
                return _length;
            }

            set
            {
                this._length = value;
            }
        }

        public byte[] Value
        {
            get
            {
                return _value;
            }

            set
            {
                this._value = value;
            }
        }

        #endregion
    }
}
