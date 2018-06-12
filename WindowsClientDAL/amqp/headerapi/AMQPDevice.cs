using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.sections;
using com.mobius.software.windows.iotbroker.amqp.wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.amqp.headerapi
{
    public interface AMQPDevice
    {
        void ProcessProto(Int32 channel, Int32 protocolId);

        void ProcessOpen(Int64 idleTimeout);

        void ProcessBegin();

        void ProcessAttach(RoleCodes? role,Int64? handle);

        void ProcessFlow(Int32 channel);

        void ProcessTransfer(AMQPData data, Int64? handle,Boolean? settled, Int64? _deliveryId);

        void ProcessDisposition(Int64? first,Int64? last);

        void ProcessDetach(Int32 channel,Int64? handle);

        void ProcessEnd(Int32 channel);

        void ProcessClose();

        void ProcessSASLInit(String _mechanism,byte[] _initialResponse,String _hostName);

        void ProcessSASLChallenge(byte[] challenge);

        void ProcessSASLMechanism(List<AMQPSymbol> _mechanisms,Int32 channel, Int32 headerType);

        void ProcessSASLOutcome(byte[] additionalData,OutcomeCodes? outcomeCode);

        void ProcessSASLResponse(byte[] response);

        void ProcessPing();

    }
}
