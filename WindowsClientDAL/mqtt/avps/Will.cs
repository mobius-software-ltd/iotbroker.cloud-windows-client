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

namespace com.mobius.software.windows.iotbroker.mqtt.avps
{
    public class Will
    {
        #region private fields

        private Topic _topic;
        private byte[] _content;
        private Boolean _retain;

        #endregion

        #region constructors
       
        public Will(Topic topic, byte[] content, Boolean retain)
        {
            this._topic = topic;
            this._content = content;
            this._retain = retain;
        }

        #endregion

        #region public fields

        public Int32 GetLength()
        {
            return _topic.Name.Length + _content.Length + 4;
        }

        public Topic Topic
        {
            get
            {
                return _topic;
            }
        }

        public byte[] Content
        {
            get
            {
                return _content;
            }
        }

        public Boolean Retain
        {
            get
            {
                return _retain;
            }
        }

        public Int32 RetrieveLenght()
        {
            return _topic.Name.Length + _content.Length + 4;
        }
        
        public Boolean IsValid()
        {
            return this._topic != null && this._topic.Name.Length > 0 && this._content != null;
        }

        #endregion
    }
}
