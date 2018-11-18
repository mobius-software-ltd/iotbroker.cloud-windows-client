using Org.BouncyCastle.Crypto.Tls;
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
    public class AsyncDtlsSessionImpl : TlsSession
    {
        byte[] sessionID;
        SessionParameters sessionParameters;

        public AsyncDtlsSessionImpl(byte[] sessionID, SessionParameters sessionParameters)
        {
            if (sessionID == null)
                throw new ArgumentException("'sessionID' cannot be null");

            if (sessionID.Length < 1 || sessionID.Length > 32)
                throw new ArgumentException("'sessionID' must have length between 1 and 32 bytes, inclusive");

            this.sessionID = new byte[sessionID.Length];
            Array.Copy(sessionID, 0, this.sessionID, 0, sessionID.Length);
            this.sessionParameters = sessionParameters;
        }

        public bool IsResumable
        {
            get
            {
                return this.sessionParameters != null;
            }
        }

        public byte[] SessionID
        {
            get
            {
                return sessionID;
            }
        }

        public SessionParameters ExportSessionParameters()
        {
            return this.sessionParameters == null ? null : this.sessionParameters.Copy();
        }

        public void Invalidate()
        {
            if (this.sessionParameters != null)
            {
                this.sessionParameters.Clear();
                this.sessionParameters = null;
            }
        }
    }
}
