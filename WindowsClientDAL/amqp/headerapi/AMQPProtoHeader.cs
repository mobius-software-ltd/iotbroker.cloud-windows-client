

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
    public class AMQPProtoHeader: AMQPHeader
    {
        private static String protocol = "AMQP";
	
        #region private fields

        private Int32 _protocolId;
        private Int32 _versionMajor = 1;
        private Int32 _versionMinor = 0;
        private Int32 _versionRevision = 0;

        #endregion

        #region constructors

        public AMQPProtoHeader(Int32 protocolId):base(null)
        {
            if (protocolId != 0 && protocolId != 3)
                throw new ArgumentException();
            
            this._protocolId = protocolId;   
        }

        #endregion

        #region public fields

        public Int32 ProtocolId
        {
            get
            {
                return _protocolId;
            }
        }

        public Int32 VersionMajor
        {
            get
            {
                return _versionMajor;
            }
        }

        public Int32 VersionMinor
        {
            get
            {
                return _versionMinor;
            }
        }

        public Int32 VersionRevision
        {
            get
            {
                return _versionRevision;
            }
        }

        public byte[] getBytes()
        {
            byte[] bytes = new byte[8];
            byte[] protocolBytes = Encoding.UTF8.GetBytes(protocol);
            Array.Copy(protocolBytes, 0, bytes, 0, protocolBytes.Length);
            bytes[4] = (byte)_protocolId;
            bytes[5] = (byte)_versionMajor;
            bytes[6] = (byte)_versionMinor;
            bytes[7] = (byte)_versionRevision;
            return bytes;
        }

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
            return (int)HeaderCodes.PROTO;
        }

        public override void ProcessBy(AMQPDevice device)
        {
            device.ProcessProto(Channel,ProtocolId);
        }

        #endregion
    }
}