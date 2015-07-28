using System.Collections.Generic;

namespace Defender
{
    public class License
    {
        public InitialKey InitialKey { get; set; }
        
        public List<LicenseParameter> Parameters { get; set; }

        public License()
        {
            this.InitialKey = new InitialKey();
            this.Parameters = new List<LicenseParameter>();
        }

        public static License Create(InitialKey initialKey)
        {
            return new License() { InitialKey = initialKey };
        }

        public static License Create(byte[] binaryValue)
        {
            return new License() { InitialKey = InitialKey.FromBinary(binaryValue) };
        }

        public static License Create(string hexStringValue)
        {
            return new License() { InitialKey = InitialKey.FromHexString(hexStringValue) };
        }
    }
}
