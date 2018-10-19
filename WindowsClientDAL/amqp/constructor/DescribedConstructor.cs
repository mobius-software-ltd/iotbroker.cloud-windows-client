

using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.tlv.api;
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

namespace com.mobius.software.windows.iotbroker.amqp.constructors
{
    public class DescribedConstructor: SimpleConstructor
    {
        #region private fields

        private TLVAmqp _descriptor;
        
        #endregion

        #region constructors

        public DescribedConstructor(AMQPType code, TLVAmqp descriptor): base(code)
        {
            this._descriptor = descriptor;
        }

        #endregion

        #region public fields

        public override byte[] getBytes()
        {
            byte[] descriptorBytes = Descriptor.getBytes();
            byte[] bytes = new byte[descriptorBytes.Length + 2];
            bytes[0] = 0;
            Array.Copy(descriptorBytes, 0, bytes, 1, descriptorBytes.Length);
            bytes[bytes.Length - 1] = (byte)Code;
            return bytes;
        }
        
        public override int getLength()
        {
            return Descriptor.getLength() + 2;
        }

        public override Byte? getDescriptorCode()
        {
            return Descriptor.getBytes()[1];
        }

        public TLVAmqp Descriptor
        {
            get
            {
                return _descriptor;
            }
        }

        #endregion
    }
}
