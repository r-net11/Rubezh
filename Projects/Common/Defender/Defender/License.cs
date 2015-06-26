using System;
using System.Collections.Generic;

namespace Defender
{
    public class License
    {
        public byte[] Key { get; set; }
        public string HexKey { get { return Key == null || Key.Length == 0 ? "-" : "0x" + BitConverter.ToString(Key).Replace("-", String.Empty); } }
        public List<LicenseParameter> Parameters { get; set; }

        public License()
        {
            this.Parameters = new List<LicenseParameter>();
        }

        public License(byte[] key)
        {
            this.Key = key;
            this.Parameters = new List<LicenseParameter>();
        }
    }
}
