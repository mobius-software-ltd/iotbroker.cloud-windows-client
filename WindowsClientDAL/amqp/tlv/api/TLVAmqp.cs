

using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.constructors;
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

namespace com.mobius.software.windows.iotbroker.amqp.tlv.api
{
    public abstract class TLVAmqp
    {
        #region private fields

        private SimpleConstructor _constructor;
        
        #endregion

        #region constructors

        public TLVAmqp(SimpleConstructor constructor)
        {
            this._constructor = constructor;
        }

        #endregion

        #region public fields

        public SimpleConstructor Constructor
        {
            get
            {
                return _constructor;
            }

            set
            {
                this._constructor = value;
            }
        }

        public AMQPType Code
        {
            get
            {
                return _constructor.Code;
            }

            set
            {
                _constructor.Code = value;
            }
        }

        public abstract byte[] getBytes();

        public abstract int getLength();

        public abstract byte[] getValue();

        public Boolean isNull()
        {
            return Constructor.Code==AMQPType.NULL;
        }

        public override bool Equals(object obj)
        {
            TLVAmqp otherItem = obj as TLVAmqp;

            return Array.Equals(otherItem.getBytes(), this.getBytes());
        }

        public override int GetHashCode()
        {
            byte[] bytes=this.getBytes();
            const int p = 16777619;
            int hash = unchecked((int)2166136261);

            for (int i = 0; i < bytes.Length; i++)
                hash = (hash ^ bytes[i]) * p;

            hash += hash << 13;
            hash ^= hash >> 7;
            hash += hash << 3;
            hash ^= hash >> 17;
            hash += hash << 5;
            return hash;
        }

        #endregion
    }
}
