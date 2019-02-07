using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace com.mobius.software.windows.iotbroker.dal
{
    public class CertificatesHelper
    {
        public static System.Security.Cryptography.X509Certificates.X509Certificate2 load(String value, String password)
        {
            PemReader reader;
            if (password.Length > 0)
                reader = new PemReader(new StringReader(value), new PasswordFinder(password));
            else
                reader = new PemReader(new StringReader(value));

            List<X509CertificateEntry> chain = new List<X509CertificateEntry>();
            AsymmetricKeyParameter privKey = null;

            object o;
            while ((o = reader.ReadObject()) != null)
            {
                if (o is X509Certificate)
                    chain.Add(new X509CertificateEntry((X509Certificate)o));
                else if (o is AsymmetricCipherKeyPair)
                    privKey = ((AsymmetricCipherKeyPair)o).Private;
                else if (o is AsymmetricKeyParameter)
                    privKey = (AsymmetricKeyParameter)o;
            }

            Pkcs12Store store = new Pkcs12StoreBuilder().Build();
            store.SetKeyEntry("certificate", new AsymmetricKeyEntry(privKey), chain.ToArray());
            MemoryStream ms = new MemoryStream();
            store.Save(ms, password.ToCharArray(), new SecureRandom());
            System.Security.Cryptography.X509Certificates.X509Certificate2 realCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(ms.ToArray(), password);
            return realCertificate;
        }

        public static Pkcs12Store loadBC(String value, String password)
        {
            PemReader reader;
            if (password.Length > 0)
                reader = new PemReader(new StringReader(value), new PasswordFinder(password));
            else
                reader = new PemReader(new StringReader(value));

            List<X509CertificateEntry> chain = new List<X509CertificateEntry>();
            AsymmetricKeyParameter privKey = null;

            object o;
            while ((o = reader.ReadObject()) != null)
            {
                if (o is X509Certificate)
                    chain.Add(new X509CertificateEntry((X509Certificate)o));
                else if (o is AsymmetricCipherKeyPair)
                    privKey = ((AsymmetricCipherKeyPair)o).Private;
                else if (o is AsymmetricKeyParameter)
                    privKey = (AsymmetricKeyParameter)o;
            }

            Pkcs12Store store = new Pkcs12StoreBuilder().Build();
            store.SetKeyEntry("certificate", new AsymmetricKeyEntry(privKey), chain.ToArray());
            return store;
        }
    }
}
