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
    public class SNPingreq : SNMessage
    {
        #region private fields

        private String _clientID;

        #endregion

        #region constructors

        public SNPingreq()
        {
        }

        public SNPingreq(String clientID)
        {
            this._clientID = clientID;
        }

        #endregion

        #region public fields

        public SNPingreq reInit(String clientID)
        {
            this._clientID = clientID;
            return this;
        }

        public int getLength()
        {
            int length = 2;

            if (_clientID != null)
                length += _clientID.Length;

            return length;
        }

        public SNType getType()
        {
            return SNType.PINGREQ;
        }

        public void ProcessBy(SNDevice device)
        {
            device.ProcessPingreq();
        }

        public String ClientID
        {
            get
            {
                return _clientID;
            }

            set
            {
                _clientID = value;
            }
        }

        #endregion
    }
}
