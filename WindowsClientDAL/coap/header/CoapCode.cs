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

namespace com.mobius.software.windows.iotbroker.coap.avps
{
    public enum CoapCode : Int32 { GET = 1, POST = 2, PUT = 3, DELETE = 4, CREATED = 65, DELETED = 66, VALID = 67, CHANGED = 68, CONTENT = 69, BAD_REQUEST = 128, UNAUTHORIZED = 129, BAD_OPTION = 130, FORBIDDEN = 131, NOT_FOUND = 132, METHOD_NOT_ALLOWED = 133,NOT_ACCEPTABLE = 134, PRECONDITION_FAILED = 140, REQUEST_ENTITY_TOO_LARGE = 141,UNSUPPORTED_CONTENT_FORMAT = 143, INTERNAL_SERVER_ERROR = 160, NOT_IMPLEMENTED = 161, BAD_GATEWAY = 162, SERVICE_UNAWAILABLE = 163, GATEWAY_TIMEOUT = 164, PROXYING_NOT_SUPPORTED = 165 };    
}
