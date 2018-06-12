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

using com.mobius.software.windows.iotbroker.mqtt.headers.api;
using com.mobius.software.windows.iotbroker.mqtt.net;
using com.mobius.software.windows.iotbroker.network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace com.mobius.software.windows.iotbroker.amqp
{
    public class ServerConnectTimer<T>
    {
        #region private fields

        private TimersMapInterface<T> _timersMap;
        private Timer _timer;

        #endregion

        #region contructors

        public ServerConnectTimer(TimersMapInterface<T> timersMap)
        {
            this._timersMap = timersMap;
        }

        #endregion

        #region public fields

        public void Execute(Int64 period)
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }

            _timer = new System.Timers.Timer();
            _timer.AutoReset = false;
            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _timer.Interval = period;
            _timer.Enabled = true;
        }

        public void OnTimedEvent(Object sender,ElapsedEventArgs args)
        {
            _timer = null;
            _timersMap.CancelConnectTimer();
            return;
        }

        public void Stop()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }
        }

        #endregion
    }
}
