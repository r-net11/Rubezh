using System;
using System.Collections.Generic;

namespace Defender
{
    public class License
    {
        InitialKey _initialKey;
        public InitialKey InitialKey 
        {
            get { return _initialKey; }
        }

        public List<LicenseParameter> Parameters { get; set; }

        public License()
        {
            this.Parameters = new List<LicenseParameter>();
        }

        public static License Create(InitialKey initialKey)
        {
            return new License() { _initialKey = initialKey };
        }

        public static License Create(byte[] binaryValue)
        {
            return new License() { _initialKey = InitialKey.FromBinary(binaryValue) };
        }

        public static License Create(string hexStringValue)
        {
            return new License() { _initialKey = InitialKey.FromHexString(hexStringValue) };
        }
    }
}
