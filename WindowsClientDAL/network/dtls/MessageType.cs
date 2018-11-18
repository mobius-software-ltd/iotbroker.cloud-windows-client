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

namespace com.mobius.software.windows.iotbroker.network.dtls
{
    public enum MessageType
    {
        HELLO_REQUEST = 0, CLIENT_HELLO = 1, SERVER_HELLO = 2, HELLO_VERIFY_REQUEST = 3, SESSION_TICKET = 4, CERTIFICATE = 11, SERVER_KEY_EXCHANGE = 12, CERTIFICATE_REQUEST = 13, SERVER_HELLO_DONE = 14, CERTIFICATE_VERIFY = 15, CLIENT_KEY_EXCHANGE = 16, FINISHED = 20, CERTIFICATE_URL = 21, CERTIFICATE_STATUS = 22,SUPPLEMENTAL_DATA = 23
    }
}
