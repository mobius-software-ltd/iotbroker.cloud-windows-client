

using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.constructors;
using com.mobius.software.windows.iotbroker.amqp.tlv.api;
using com.mobius.software.windows.iotbroker.amqp.tlv.array;
using com.mobius.software.windows.iotbroker.amqp.tlv.fixed_;
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
    public class TLVList : TLVAmqp
    {
        #region private fields

        private Int32 _width;
        private Int32 _size;
        private Int32 _count;

        private List<TLVAmqp> elements = new List<TLVAmqp>();

        #endregion

        #region constructors

        public TLVList():base(new SimpleConstructor(codes.AMQPType.LIST_0))
        {
            _width = 0;
            _size = 0;
            _count = 0;
        }

        public TLVList(AMQPType code, List<TLVAmqp> elements):base(new SimpleConstructor(code))
        {
            this.elements = elements;
            _width = code == AMQPType.LIST_8 ? 1 : 4;
            _size += _width;
            foreach (TLVAmqp tlv in elements)
                _size += tlv.getLength();
            _count = elements.Count;
        }

        #endregion

        #region public fields

        public void addElement(TLVAmqp element)
        {
            if (_size == 0)
            {
                Constructor.Code = AMQPType.LIST_8;
                _width = 1;
                _size += 1;
            }

            elements.Add(element);
            _count++;
            _size += element.getLength();
            update();
        }

        public void setElement(int index, TLVAmqp element)
        {
            _size -= elements[index].getLength();
            elements[index] = element;
            _size += element.getLength();
            update();
        }

        public void addElement(int index, TLVAmqp element)
        {
            int diff = index - elements.Count;
            do
            {
                addElement(new TLVNull());
            }
            while (diff-- > 0);
            setElement(index, element);
        }

        public void addToList(int index, int elemIndex, TLVAmqp element)
        {
            if (_count <= index)
                addElement(index, new TLVList());
            TLVAmqp list = this.elements[index];
            if (list.isNull())
                setElement(index, new TLVList());
            ((TLVList)this.elements[index]).addElement(elemIndex, element);
            _size += element.getLength();
            update();
        }

        public void addToMap(int index, TLVAmqp key, TLVAmqp value)
        {
            if (_count <= index)
                addElement(index, new TLVMap());
            TLVAmqp map = elements[index];
            if (map.isNull())
                setElement(index, new TLVMap());
            ((TLVMap)elements[index]).putElement(key, value);
            _size += key.getLength() + value.getLength();
            update();
        }

        public void addToArray(int index, TLVAmqp element)
        {
            if (_count <= index)
                addElement(index, new TLVArray());
            TLVAmqp array = elements[index];
            if (array.isNull())
                setElement(index, new TLVArray());
            ((TLVArray)elements[index]).addElement(element);
            _size += element.getLength();
            update();
        }

        public override byte[] getBytes()
        {
            byte[] constructorBytes = Constructor.getBytes();
            byte[] sizeBytes = new byte[_width];
            switch (_width)
            {
                case 0:
                    break;
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
                case 0:
                    break;
                case 1:
                    countBytes[0] = (byte)_count;
                    break;
                default:
                    Array.Copy(BitConverter.GetBytes(_count), countBytes, 4);
                    break;
            }
            byte[] valueBytes = new byte[_size - _width];
            int pos = 0;
            byte[] tlvBytes;
            foreach (TLVAmqp tlv in elements)
            {
                tlvBytes = tlv.getBytes();
                Array.Copy(tlvBytes, 0, valueBytes, pos, tlvBytes.Length);
                pos += tlvBytes.Length;
            }
            byte[] bytes = new byte[constructorBytes.Length + sizeBytes.Length + countBytes.Length + valueBytes.Length];
            Array.Copy(constructorBytes, 0, bytes, 0, constructorBytes.Length);
            if (_size > 0)
            {
                Array.Copy(sizeBytes, 0, bytes, constructorBytes.Length, sizeBytes.Length);
                Array.Copy(countBytes, 0, bytes, constructorBytes.Length + sizeBytes.Length,countBytes.Length);
                Array.Copy(valueBytes, 0, bytes, constructorBytes.Length + sizeBytes.Length
                        + countBytes.Length, valueBytes.Length);
            }
            return bytes;
        }

        public String toString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (TLVAmqp element in elements)
                sb.Append(element.ToString() + "\n");
            return sb.ToString();
        }

        protected void update()
        {
            if (_width == 1 && _size > 255)
            {
                Constructor.Code = AMQPType.LIST_32;
                _width = 4;
                _size += 3;
            }
        }

        public List<TLVAmqp> getList()
        {
            return elements;
        }

        public override byte[] getValue()
        {
            return null;
        }

        public override int getLength()
        {
            return Constructor.getLength() + _width + _size;
        }

        #endregion
    }
}