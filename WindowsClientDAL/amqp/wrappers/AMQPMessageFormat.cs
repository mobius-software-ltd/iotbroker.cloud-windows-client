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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.amqp.wrappers
{
    public class AMQPMessageFormat
    {
        #region private fields

        private Int32 _messageFormat;
        private Int32 _version;

        #endregion

        #region constructors

        public AMQPMessageFormat(long value)
        {
            byte[] arr = BitConverter.GetBytes((int)value);
            byte[] mf = new byte[4];
            Array.Copy(arr, 0, mf, 1, 3);            
            _messageFormat = BitConverter.ToInt32(mf,0);
            _version = arr[3] & 0x00ff;
        }

        public AMQPMessageFormat(int messageFormat, int version)
        {
            this._messageFormat = messageFormat;
            this._version = version;
        }

        #endregion

        #region public fields

        public Int32 MessageFormat
        {
            get
            {
                return _messageFormat;
            }            
        }

        public Int32 Version
        {
            get
            {
                return _version;
            }
        }

        public Int64 encode()
        {
            byte[] arr = new byte[4];
            byte[] mf = BitConverter.GetBytes(_messageFormat);
            Array.Copy(mf, 1, arr, 0, 3);
            arr[3] = (byte)_version;
            return (long)BitConverter.ToUInt32(arr,0);
        }

        #endregion
    }
}