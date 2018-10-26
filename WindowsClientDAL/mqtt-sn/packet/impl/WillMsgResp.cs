using com.mobius.software.windows.iotbroker.mqtt_sn.avps;
using com.mobius.software.windows.iotbroker.mqtt_sn.packet.api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mobius.software.windows.iotbroker.mqtt_sn.headers.api;

namespace com.mobius.software.windows.iotbroker.mqtt_sn.packet.impl
{
    public class WillMsgResp: ResponseMessage
    {
        #region private fields

        #endregion

        #region constructors

        public WillMsgResp()
        {
        }

        public WillMsgResp(ReturnCode code): base(code)
        {
        }

        #endregion

        #region public fields

        public WillMsgResp reinit(ReturnCode code)
        {
            this.ReturnCode = code;
            return this;
        }

        override
        public int getLength()
        {
            return 2;
        }

        public override SNType getType()
        {
            return SNType.WILL_MSG_RESP;
        }

        public override void ProcessBy(SNDevice device)
        {
            device.ProcessWillMessageResponse();
        }

        #endregion
    }
}
