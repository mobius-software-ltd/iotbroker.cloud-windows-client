using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.tlv.api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.amqp.sections
{
    public interface AMQPSection
    {
        void fill(TLVAmqp list);

        TLVAmqp getValue();

        SectionCodes getCode();
    }
}
