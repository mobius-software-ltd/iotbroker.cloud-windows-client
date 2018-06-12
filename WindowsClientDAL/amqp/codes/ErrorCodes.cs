

using com.mobius.software.windows.iotbroker.dal;
/**
* Mobius Software LTD
* Copyright 2015-2018, Mobius Software LTD
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.amqp.codes
{
    public enum ErrorCodes : Int32
    {
        [StringValue("amqp:internal-error")]
        INTERNAL_ERROR = 1,
        [StringValue("amqp:not-found")]
        NOT_FOUND = 2,
        [StringValue("amqp:unauthorized-access")]
        UNAUTHORIZED_ACCESS = 3,
        [StringValue("amqp:decode-error")]
        DECODE_ERROR = 4,
        [StringValue("amqp:resource-limit-exceeded")]
        RESOURCE_LIMIT_EXCEEDED = 5,
        [StringValue("amqp:not-allowed")]
        NOT_ALLOWED = 6,
        [StringValue("amqp:invalid-field")]
        INVALID_FIELD = 7,
        [StringValue("amqp:not-implemented")]
        NOT_IMPLEMENTED = 8,
        [StringValue("amqp:resource-locked")]
        RESOURCE_LOCKED = 9,
        [StringValue("amqp:precondition-failed")]
        PRECONDITION_FAILED = 10,
        [StringValue("amqp:resource-deleted")]
        RESOURCE_DELETED = 11,
        [StringValue("amqp:illegal-state")]
        ILLEGAL_STATE = 12,
        [StringValue("amqp:frame-size-too-small")]
        FRAME_SIZE_TOO_SMALL = 13,
        [StringValue("amqp:connection-forced")]
        CONNECTION_FORCED = 14,
        [StringValue("amqp:framing-error")]
        FRAMING_ERROR = 15,
        [StringValue("amqp:redirected")]
        REDIRECTED = 16,
        [StringValue("amqp:window-violation")]
        WINDOW_VIOLATION = 17,
        [StringValue("amqp:errant-link")]
        ERRANT_LINK = 18,
        [StringValue("amqp:handle-in-use")]
        HANDLE_IN_USE = 19,
        [StringValue("amqp:unattached-handle")]
        UNATTACHED_HANDLE = 20,
        [StringValue("amqp:detach-forced")]
        DETACH_FORCED = 21,
        [StringValue("amqp:transfer-limit-exceeded")]
        TRANSFER_LIMIT_EXCEEDED = 22,
        [StringValue("amqp:message-size-exceeded")]
        MESSAGE_SIZE_EXCEEDED = 23,
        [StringValue("amqp:redirect")]
        REDIRECT = 24,
        [StringValue("amqp:stolen")]
        STOLEN = 25
    };    
}