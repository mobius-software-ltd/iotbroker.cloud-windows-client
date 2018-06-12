

using com.mobius.software.windows.iotbroker.amqp.headeramqp;
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

namespace com.mobius.software.windows.iotbroker.amqp
{
    public class AmqpTransferMap
    {
        #region private fields

        private int _index;
        private Dictionary<Int32, AMQPTransfer> _map;

        #endregion

        #region constructors

        public AmqpTransferMap()
        {
            _index = 0;
            _map = new Dictionary<Int32, AMQPTransfer>();
        }

        #endregion

        #region public fields

        public AMQPTransfer addTransfer(AMQPTransfer item)
        {
            int num = _index;
            _map[num] = item;

            newIndex();
            item.DeliveryId = (long)num;

            return item;
        }

        public AMQPTransfer removeTransfer(int key)
        {
            AMQPTransfer oldItem = _map[key];
            _map.Remove(key);
            return oldItem;
        }

        private void newIndex()
        {
            _index += 1;

            if (_index == 65535)
                _index = 0;           
        }

        #endregion
    }
}
