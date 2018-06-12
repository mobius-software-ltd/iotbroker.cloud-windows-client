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
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.amqp.wrappers
{
    public class StringID: MessageID
    {
        #region private fields

        private String _id;

        #endregion

        #region constructors

        public StringID(String id)
        {
            this._id = id;   
        }

        #endregion

        #region public fields

        public String getString()
        {
            return _id;
        }

        public byte[] getBinary()
        {
            return null;
        }

        public BigInteger? getLong()
        {
            return null;
        }

        public Guid? getUuid()
        {
            return null;
        }

        #endregion
    }
}