

using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.constructors;
using com.mobius.software.windows.iotbroker.amqp.tlv.api;
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

namespace com.mobius.software.windows.iotbroker.amqp.tlv.array
{
    public class TLVArray: TLVAmqp
    {
        #region private fields

        private Int32 _width;
        private Int32 _size;
        private Int32 _count;

        private List<TLVAmqp> elements = new List<TLVAmqp>();

        private SimpleConstructor elementConstructor;

        #endregion

        #region constructors

        public TLVArray():base(new SimpleConstructor(codes.AMQPType.ARRAY_8))
        {
            _width = 1;
            _size = 0;
            _count = 0;
        }

        public TLVArray(AMQPType code, List<TLVAmqp> elements):base(new SimpleConstructor(code))
        {
            this.elements = elements;
            _width = code==AMQPType.ARRAY_8 ? 1 : 4;
            _size += _width;
            foreach (TLVAmqp element in elements)
            {
                _size += element.getLength() - element.Constructor.getLength();
                if (elementConstructor == null && element != null)
                    elementConstructor = element.Constructor;
            }

            _size += elementConstructor.getLength();
            _count = elements.Count;
        }

        #endregion

        #region public fields

        public SimpleConstructor getElementConstructor()
        {
            return elementConstructor;
        }

        public AMQPType getElemetsCode()
        {
            return elementConstructor.Code;
        }

        public void addElement(TLVAmqp element)
        {
            if (elements.Count==0)
            {
                elementConstructor = element.Constructor;
                _size += _width;
                _size += elementConstructor.getLength();
            }

            elements.Add(element);
            _count++;
            _size += element.getLength() - elementConstructor.getLength();
            if (_width == 1 && _size > 255)
            {
                Constructor.Code = AMQPType.ARRAY_32;
                _width = 4;
                _size += 3;
            }
        }

        public String toString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (TLVAmqp element in elements)
                    sb.Append(element.ToString() + "\n");

            return sb.ToString();
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
                case 4:
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
                case 4:
                    Array.Copy(BitConverter.GetBytes(_count), countBytes, 4);
                    break;
            }
            byte[] elemetConstructorBytes = elementConstructor.getBytes();
            byte[] valueBytes = new byte[_size - _width - elemetConstructorBytes.Length];
            int pos = 0;
            byte[] tlvBytes;
            foreach (TLVAmqp tlv in elements)
            {
                tlvBytes = tlv.getBytes();
                Array.Copy(tlvBytes, elemetConstructorBytes.Length, valueBytes, pos,
                        tlvBytes.Length - elemetConstructorBytes.Length);                
                pos += tlvBytes.Length - elemetConstructorBytes.Length;
            }

            byte[] bytes = new byte[constructorBytes.Length + sizeBytes.Length + countBytes.Length
                    + elemetConstructorBytes.Length + valueBytes.Length];
            Array.Copy(constructorBytes, 0, bytes, 0, constructorBytes.Length);
            if (_size > 0)
            {
                Array.Copy(sizeBytes, 0, bytes, constructorBytes.Length, sizeBytes.Length);
                Array.Copy(countBytes, 0, bytes, constructorBytes.Length + sizeBytes.Length,countBytes.Length);
                Array.Copy(elemetConstructorBytes, 0, bytes, constructorBytes.Length + sizeBytes.Length + countBytes.Length, elemetConstructorBytes.Length);
                Array.Copy(valueBytes, 0, bytes, constructorBytes.Length + sizeBytes.Length + countBytes.Length + elemetConstructorBytes.Length, valueBytes.Length);
            }

            return bytes;
        }

        public List<TLVAmqp> getElements()
        {
            return elements;
        }

        public override int getLength()
        {
            return Constructor.getLength() + _size + _width;
        }

        public override byte[] getValue()
        {
            return null;
        }

        public new Boolean isNull()
        {
            AMQPType code = Constructor.Code;
            if (code == AMQPType.NULL)
                return true;
            if (code == AMQPType.ARRAY_8 || code == AMQPType.ARRAY_32)
            {
                if (elements.Count == 0)
                    return true;
            }
            return false;
        }

        #endregion
    }
}