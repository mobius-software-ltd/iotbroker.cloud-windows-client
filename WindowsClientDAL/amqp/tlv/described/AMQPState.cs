using com.mobius.software.windows.iotbroker.amqp.tlv.compound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.amqp.tlv.described
{
    public interface AMQPState
    {
        TLVList getList();

        void fill(TLVList list);
    }
}
