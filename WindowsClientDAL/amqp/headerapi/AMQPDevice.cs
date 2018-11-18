using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.sections;
using com.mobius.software.windows.iotbroker.amqp.wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

namespace com.mobius.software.windows.iotbroker.amqp.headerapi
{
    public interface AMQPDevice
    {
        void ProcessProto(Int32 channel, Int32 protocolId);

        void ProcessOpen(Int64? idleTimeout);

        void ProcessBegin();

        void ProcessAttach(String name,RoleCodes? role,Int64? handle);

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
