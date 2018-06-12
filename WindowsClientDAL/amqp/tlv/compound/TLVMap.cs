

using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.constructors;
using com.mobius.software.windows.iotbroker.amqp.tlv.api;
using com.mobius.software.windows.iotbroker.amqp.tlv.array;
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
using com.mobius.software.windows.iotbroker.mqtt.avps;
using com.mobius.software.windows.iotbroker.mqtt.headers.api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.amqp.tlv.compound
{
    public class TLVMap : TLVAmqp
    {
        #region private fields

        private Int32 _width;
        private Int32 _size;
        private Int32 _count;

        private Dictionary<TLVAmqp, TLVAmqp> map = new Dictionary<TLVAmqp, TLVAmqp>();

        #endregion

        #region constructors

        public TLVMap():base(new SimpleConstructor(codes.AMQPType.MAP_8))
        {
            _width = 1;
            _size = 1;
            _count = 0;
        }

        public TLVMap(AMQPType code, Dictionary<TLVAmqp, TLVAmqp> map) :base(new SimpleConstructor(code))
        {
            this.map = map;
            _width = code == AMQPType.MAP_8 ? 1 : 4;
            _size += _width;
            foreach (KeyValuePair<TLVAmqp, TLVAmqp> entry in map)
            {
                _size += entry.Key.getLength();
                _size += entry.Value.getLength();
            }

            _count = map.Count;
        }

        #endregion

        #region public fields

        public override byte[] getValue()
        {
            return null;
        }

        public void putElement(TLVAmqp key, TLVAmqp value)
        {
            map[key] =  value;
            _size += key.getLength() + value.getLength();
            _count++;
            update();
        }

        public String toString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<TLVAmqp, TLVAmqp> entry in map)
            {
                sb.Append(entry.Key.ToString());
                sb.Append(" : ");
                sb.Append(entry.Value.ToString());
                sb.Append("\n");
            }
            return sb.ToString();
        }

        public override byte[] getBytes()
        {
            byte[] constructorBytes = Constructor.getBytes();
            byte[] sizeBytes = new byte[_width];
            switch (_width)
            {
                case 1:
                    sizeBytes[0] = (byte)_size;                    
                    break;
                default:
                    Array.Copy(BitConverter.GetBytes(_size), sizeBytes, 4);
                    break;
            }

            byte[] countBytes = new byte[_width];
            switch (_width)
            {
                case 1:
                    countBytes[0] = (byte)(_count * 2);
                    break;
                default:
                    Array.Copy(BitConverter.GetBytes(_count * 2), countBytes, 4);
                    break;
            }

            byte[] valueBytes = new byte[_size - _width];
            int pos = 0;
            byte[] keyBytes;
            byte[] valBytes;
            foreach (KeyValuePair<TLVAmqp, TLVAmqp> entry in map)
            {
                keyBytes = entry.Key.getBytes();
                valBytes = entry.Value.getBytes();
                Array.Copy(keyBytes, 0, valueBytes, pos, keyBytes.Length);
                pos += keyBytes.Length;
                Array.Copy(valBytes, 0, valueBytes, pos, valBytes.Length);
                pos += valBytes.Length;
            }

            byte[] bytes = new byte[constructorBytes.Length + sizeBytes.Length + countBytes.Length + valueBytes.Length];
            Array.Copy(constructorBytes, 0, bytes, 0, constructorBytes.Length);
            if (_size > 0)
            {
                Array.Copy(sizeBytes, 0, bytes, constructorBytes.Length, sizeBytes.Length);
                Array.Copy(countBytes, 0, bytes, constructorBytes.Length + sizeBytes.Length,countBytes.Length);
                Array.Copy(valueBytes, 0, bytes, constructorBytes.Length + sizeBytes.Length + countBytes.Length, valueBytes.Length);
            }
            return bytes;
        }

        protected void update()
        {
            if (_width == 1 && _size > 255)
            {
                Constructor.Code = AMQPType.MAP_32;
                _width = 4;
                _size += 3;
            }
        }

        public Dictionary<TLVAmqp, TLVAmqp> getMap()
        {
            return map;
        }

        public override int getLength()
        {
            return Constructor.getLength() + _width + _size;
        }

        #endregion
    }
}