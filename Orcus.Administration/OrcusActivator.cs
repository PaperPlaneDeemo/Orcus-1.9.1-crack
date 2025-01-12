﻿using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Orcus.Administration.Core.Utilities;
using Orcus.Administration.Licensing;

namespace Orcus.Administration
{
    public class OrcusActivator
    {
        private const string PublicKey =
            @"<RSAKeyValue><Modulus>0kOe1lvzBhDWD7k2rj9GeMyJt7drGCJByVl++DIX8oTd4eDXpBEsnmXYwheFk9DLPDBZzrq0GiHzaSsCYxcz2vwT+eM4UXMZux+lRx2fvgLv5ga1r+FC+DgF7CkKGZ2AeIGJYgN+cjMn0qBLp83QRJ69tzUDMfaOvA5VDDN5NzM=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        protected OrcusActivator()
        {
            HardwareId = HardwareIdGenerator.HardwareId;
            IsValid = true;
            IsRegistered = true;
        }

        public static bool IsRegistered { get; set; }
        public bool IsValid { get; }
        public string HardwareId { get; }

        private bool ValidateLicense(byte[] signature)
        {
            var dataStr = GeneralizeDataString(HardwareId); // "ERIKAMUSTERMANN"

            var dataBuffer = Encoding.UTF8.GetBytes(dataStr);

            using (var provider = new RSACryptoServiceProvider())
            {
                provider.FromXmlString(PublicKey);
                provider.PersistKeyInCsp = false;

                return provider.VerifyData(dataBuffer, new SHA1CryptoServiceProvider(), signature);
            }
        }

        public static OrcusActivator Parse(string licenseData)
        {
            return new OrcusActivator();
        }

        private static byte[] DecodeDataFromString(string value)
        {
            if (value == null)
                return new byte[0];

            if ((value.Length & 1) != 0)
                throw new FormatException();

            if (string.IsNullOrWhiteSpace(value))
                return new byte[0];

            value = value.ToUpperInvariant();

            byte[] ab = new byte[value.Length >> 1];
            for (int i = 0; i < value.Length; i++)
            {
                int b = value[i];
                b = b - '0' + ((('9' - b) >> 31) & -7);
                ab[i >> 1] |= (byte)(b << 4 * ((i & 1) ^ 1));
            }
            return ab;
        }
        private static string GeneralizeDataString(string someString)
        {
            return someString.StripWhiteSpace().ToUpperInvariant();
        }
    }
}