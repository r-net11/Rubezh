using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Defender
{
    public class InitialKey
    {
        byte[] _binaryValue;
        public byte[] BinaryValue 
        {
            get { return _binaryValue; }
            set
            {
                _binaryValue = value;
                _stringValue = GetStringFromBinary(value);
            }
        }

        string _stringValue;
        public string StringValue 
        {
            get { return _stringValue; }
            set
            {
                _binaryValue = GetBinaryFromString(value);
                _stringValue = _binaryValue == null ? null : value;
            }
        }

        public static InitialKey Generate()
        {
            return new InitialKey() { BinaryValue = GenerateHashCode(HardwareInfo.CpuId + HardwareInfo.HardDiskId) };
        }

        public static InitialKey FromBinary(byte[] value)
        {
            return new InitialKey() { BinaryValue = value };
        }

        public static InitialKey FromHexString(string value)
        {
            return new InitialKey() { StringValue = value };
        }
        
        public override string ToString()
        {
            return StringValue == null ? String.Empty : StringValue;
        }

        static byte[] GenerateHashCode(string data)
        {
            if (data == null)
                return null;
            byte[] keyBytes = new byte[data.Length * sizeof(char)];
            Buffer.BlockCopy(data.ToCharArray(), 0, keyBytes, 0, keyBytes.Length);
            return new SHA256Managed().ComputeHash(keyBytes);
        }

        string GetStringFromBinary(byte[] value)
        {
            return value == null ? null : BitConverter.ToString(BinaryValue).Replace("-", String.Empty);
        }

        byte[] GetBinaryFromString(string value)
        {
            if (value == null || value.Length == 0 || value.Length % 2 != 0 || !value.All(x => String.Copy("0123456789ABCDEFabcdef").Contains(x)))
                return null;

            return Enumerable.Range(0, value.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(value.Substring(x, 2), 16)).ToArray();
        }

        public static bool operator ==(InitialKey initialKey1, InitialKey initialKey2)
        {
            if (Object.ReferenceEquals(initialKey1, initialKey2))
                return true;
            if (((object)initialKey1 == null) || ((object)initialKey1 == null))
                return false;
            return initialKey1._stringValue == initialKey2._stringValue;
        }

        public static bool operator !=(InitialKey initialKey1, InitialKey initialKey2)
        {
            return !(initialKey1 == initialKey2);
        }
    }
}
