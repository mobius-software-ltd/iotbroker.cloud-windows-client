

using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.constructors;
using com.mobius.software.windows.iotbroker.amqp.headerapi;
using com.mobius.software.windows.iotbroker.amqp.tlv.api;
using com.mobius.software.windows.iotbroker.amqp.tlv.compound;
using com.mobius.software.windows.iotbroker.amqp.tlv.described;
using com.mobius.software.windows.iotbroker.amqp.tlv.fixed_;
using com.mobius.software.windows.iotbroker.amqp.wrappers;
using com.mobius.software.windows.iotbroker.dal;
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
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.amqp.headersasl
{
    public class SASLResponse : AMQPHeader
    {
        #region private fields

        private byte[] _response;

        #endregion

        #region constructors

        public SASLResponse():base(HeaderCodes.RESPONSE)
        { 
        }

        #endregion

        #region public fields

        public override TLVList getArguments()
        {
            TLVList list = new TLVList();

            if (_response == null)
                throw new MalformedMessageException("SASL-Response header's challenge can't be null");

            list.addElement(0, AMQPWrapper<AMQPSymbol>.wrap(_response));

            DescribedConstructor constructor = new DescribedConstructor(list.Code,new TLVFixed(AMQPType.SMALL_ULONG, new byte[] { 0x43 }));
            list.Constructor = constructor;

            return list;
        }

        public override void fillArguments(TLVList list)
        {
            int size = list.getList().Count;

            if (size == 0)
                throw new MalformedMessageException("Received malformed SASL-Response header: challenge can't be null");

            if (size > 1)
                throw new MalformedMessageException(
                        "Received malformed SASL-Response header. Invalid number of arguments: " + size);

            if (size > 0)
            {
                TLVAmqp element = list.getList()[0];
                if (element.isNull())
                    throw new MalformedMessageException("Received malformed SASL-Response header: challenge can't be null");

                _response = AMQPUnwrapper<AMQPSymbol>.unwrapBinary(element);
            }
        }

        public byte[] Response
        {
            get
            {
                return _response;
            }
        }

        public override int getLength()
        {
            int length = 8;
            TLVAmqp arguments = this.getArguments();
            length += arguments.getLength();
            return length;
        }

        public override int getType()
        {
            return (int)HeaderCodes.RESPONSE;
        }

        public void setCramMD5Response(byte[] challenge, String user)
        {
		    if (user == null)
			    throw new ArgumentException("CramMD5 response generator must be provided with a non-null username value");

            if (challenge == null)
			    throw new ArgumentException("CramMD5 response generator must be provided with a non-null challenge value");

            this._response = calcCramMD5(challenge, user);
        }

        private byte[] calcCramMD5(byte[] challenge, String user)
        {
		    if (challenge != null && challenge.Length != 0) 
            {
                try
                {
                    HMACMD5 md5 = new HMACMD5(Encoding.ASCII.GetBytes(user));
                    byte[] bytes = md5.ComputeHash(challenge);

                    StringBuilder hash = new StringBuilder(user);
                    hash.Append(' ');
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        String hex = (0xFF & bytes[i]).ToString("X2");
                        if (hex.Length == 1)
                            hash.Append('0');

                        hash.Append(hex);
                    }
                    return Encoding.ASCII.GetBytes(hash.ToString());
                }
                catch (Exception e)
                {
                    throw new Exception("calcCramMD5 error", e);
                }
            } 
            else 
            {
                return new byte[0];
            }
        }

        public override void ProcessBy(AMQPDevice device)
        {
            device.ProcessSASLResponse(Response);
        }

        #endregion
    }
}
