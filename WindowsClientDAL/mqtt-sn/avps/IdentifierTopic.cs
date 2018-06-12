using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.mqtt_sn.avps
{
    public class IdentifierTopic : SNTopic
    {
        #region private fields

        private Int32 _value;
        private SNQoS _qos;

        #endregion

        #region constructors

        public IdentifierTopic()
        {

        }

        public IdentifierTopic(Int32 value, SNQoS qos)
        {
            this._value = value;
            this._qos = qos;
        }

        #endregion

        #region public fields

        public IdentifierTopic reInit(Int32 value, SNQoS qos)
        {
            this._value = value;
            this._qos = qos;
            return this;
        }

        public TopicType getType()
        {
            return TopicType.ID;
        }

        public byte[] encode()
        {
            return BitConverter.GetBytes((short)_value);
        }

        public SNQoS getQos()
        {
            return _qos;
        }

        public int length()
        {
            return 2;
        }

        public Int32 Value
        {
            get
            {
                return _value;
            }

            set
            {
                this._value = value;
            }
        }

        public SNQoS Qos
        {
            get
            {
                return _qos;
            }

            set
            {
                this._qos = value;
            }
        }

        #endregion
    }
}
