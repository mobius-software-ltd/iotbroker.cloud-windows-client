

using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.headeramqp;
using com.mobius.software.windows.iotbroker.amqp.tlv.compound;
using com.mobius.software.windows.iotbroker.dal;
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.amqp.headerapi
{
    public class AMQPPing : AMQPHeader
    {
        #region private fields

        #endregion

        #region constructors

        public AMQPPing():base(HeaderCodes.PING)
        {

        }

        #endregion

        #region public fields

        public override TLVList getArguments()
        {
            return null;
        }

        public override void fillArguments(TLVList list)
        {
        }

        public override int getLength()
        {
            int length = 8;
            return length;
        }

        public override int getType()
        {
            return (int)HeaderCodes.PING;
        }

        public override void ProcessBy(AMQPDevice device)
        {
            device.ProcessPing();
        }

        #endregion
    }
}