using com.mobius.software.windows.iotbroker.mqtt_sn.avps;
using com.mobius.software.windows.iotbroker.mqtt_sn.headers.api;
using com.mobius.software.windows.iotbroker.mqtt_sn.packet.api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.mqtt_sn.packet.impl
{
    public class SNPingresp: ResponseMessage
    {
        #region private fields

        #endregion

        #region constructors

        public SNPingresp()
        {
        }

        #endregion

        #region public fields

        override
        public int getLength()
        {
            return 2;
        }

        public override SNType getType()
        {
            return SNType.PINGRESP;
        }

        public override void ProcessBy(SNDevice device)
        {
            device.ProcessPingresp();
        }

        #endregion
    }
}
