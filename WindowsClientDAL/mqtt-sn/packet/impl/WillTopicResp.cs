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
    public class WillTopicResp: ResponseMessage
    {
        #region private fields

        #endregion

        #region constructors

        public WillTopicResp()
        {
        }

        public WillTopicResp(ReturnCode code): base(code)
        {
        }

        #endregion

        #region public fields

        public WillTopicResp reinit(ReturnCode code)
        {
            this.ReturnCode = code;
            return this;
        }

        public new int getLength()
        {
            return 2;
        }

        public override SNType getType()
        {
            return SNType.WILL_TOPIC_RESP;
        }

        public override void ProcessBy(SNDevice device)
        {
            device.ProcessWillTopicResponse();
        }

        #endregion
    }
}
