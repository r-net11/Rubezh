using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Defender
{
    public class InitialKey
    {
        public string Value { get; private set; }
        public byte[] HashCode { get; private set; }

        public InitialKey(string value)
        {
            this.Value = value;
            if (value != null)
                this.HashCode = GenerateHashCode(value);
        }

        byte[] GenerateHashCode(string key)
        {
            byte[] keyBytes = new byte[key.Length * sizeof(char)];
            Buffer.BlockCopy(key.ToCharArray(), 0, keyBytes, 0, keyBytes.Length);
            return new SHA256Managed().ComputeHash(keyBytes);
        }

        public bool SaveToFile(string fileName)
        {
            try
            {
                using (var file = File.OpenWrite(fileName))
                {
                    file.Write(HashCode, 0, HashCode.Length);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static InitialKey Generate()
        {
            string value = HardwareInfo.CpuId + HardwareInfo.HardDiskId;
            return new InitialKey(value);
        }

        public static InitialKey FromFile(string fileName)
        {
            try
            {
                InitialKey loadedKey = new InitialKey(null);

                using (var file = File.OpenRead(fileName))
                {
                    loadedKey.HashCode = new byte[file.Length];
                    file.Read(loadedKey.HashCode, 0, loadedKey.HashCode.Length);
                }
                return loadedKey;
            }
            catch
            {
                return null;
            }
        }

        public static bool operator ==(InitialKey key1, InitialKey key2)
        {
            if (Equals(key1, null) && Equals(key2, null))
                return true;

            if (Equals(key1, null) || Equals(key2, null) || key1.HashCode == null || key2.HashCode == null || key1.HashCode.Length != key2.HashCode.Length)
                return false;

            for (int i = 0; i < key1.HashCode.Length; i++)
                if (key1.HashCode[i] != key2.HashCode[i])
                    return false;

            return true;
        }

        public static bool operator !=(InitialKey key1, InitialKey key2)
        {
            return !(key1 == key2);
        }

        public override bool Equals(object obj)
        {
            if (obj is InitialKey)
                return this == (InitialKey)obj;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return HashCode == null ? base.GetHashCode() : HashCode.Sum(x => (int)x);
        }

        public override string ToString()
        {
            return HashCode == null || HashCode.Length == 0 ? "-" : "0x" + BitConverter.ToString(HashCode).Replace("-", String.Empty);
        }
    }
}
