using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Security.Cryptography;

namespace Defender
{
    public static class LicenseProcessor
    {
        public static List<Exception> Exceptions = new List<Exception>();

        public static License ProcessLoad(string fileName, InitialKey key)
        {
            return ProcessLoad(fileName, key.BinaryValue);
        }

        public static License ProcessLoad(string fileName, byte[] key)
        {
            Exceptions.Clear();
            return Deserialize(Decrypt(LoadFromFile(fileName), key));
        }

        public static License ProcessLoad(string fileName, string key)
        {
            Exceptions.Clear();
            return Deserialize(Decrypt(LoadFromFile(fileName), InitialKey.FromHexString(key).BinaryValue));
        }

        public static bool ProcessSave(string fileName, License license, InitialKey key)
        {
            return ProcessSave(fileName, license, key.BinaryValue);
        }

        public static bool ProcessSave(string fileName, License license, byte[] key)
        {
            Exceptions.Clear();
            return SaveToFile(Encrypt(Serialize(license), key), fileName);
        }

        public static bool ProcessSave(string fileName, License license, string key)
        {
            Exceptions.Clear();
            return SaveToFile(Encrypt(Serialize(license), InitialKey.FromHexString(key).BinaryValue), fileName);
        }

        static string Serialize(License license)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    new XmlSerializer(typeof(License)).Serialize(memoryStream, license);
                    memoryStream.Position = 0;
                    return new StreamReader(memoryStream).ReadToEnd();
                }
            }
            catch (Exception ex) 
            {
                Exceptions.Add(ex);
                return null;
            }
        }

        static License Deserialize(string data)
        {
            if (data == null)
                return null;
            
            try
            {
                using (MemoryStream memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(data)))
                {
                    var xmlSerializer = new XmlSerializer(typeof(License));
                    return (License)xmlSerializer.Deserialize(memoryStream);
                }
            }
            catch (Exception ex)
            {
                Exceptions.Add(ex);
                return null;
            }
        }

        static byte[] Encrypt(string data, byte[] key)
        {
            if (data == null)
                return null;

            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = FillBytes(key, aesAlg.KeySize);
                    aesAlg.IV = FillBytes(key, aesAlg.BlockSize); 

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(data);
                            }
                            return msEncrypt.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.Add(ex);
                return null;
            }
        }

        static string Decrypt(byte[] data, byte[] key)
        {
            if (data == null)
                return null;

            try
            {
                string result = null;

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = FillBytes(key, aesAlg.KeySize);
                    aesAlg.IV = FillBytes(key, aesAlg.BlockSize); 

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msDecrypt = new MemoryStream(data))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                result = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Exceptions.Add(ex);
                return null;
            }
        }

        static byte[] LoadFromFile(string fileName)
        {
            try
            {
                byte[] loadedBytes;
                using (var file = File.OpenRead(fileName))
                {
                    loadedBytes = new byte[file.Length];
                    file.Read(loadedBytes, 0, loadedBytes.Length);
                }
                
                return loadedBytes;
            }
            catch (Exception ex)
            {
                Exceptions.Add(ex);
                return null;
            }
        }

        static bool SaveToFile(byte[] data, string fileName)
        {
            if (data == null)
                return false;

            try
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);

                using (var file = File.OpenWrite(fileName))
                {
                    file.Write(data, 0, data.Length);
                }
                return true;
            }
            catch (Exception ex)
            {
                Exceptions.Add(ex);
                return false;
            }
        }

        static byte[] FillBytes(byte[] input, int bits)
        {
            if (input == null || input.Length == 0 || bits <= 0)
                return null;

            byte[] result = new byte[bits / 8];
            for (int i = 0; i < result.Length; i++)
                result[i] = input[i % input.Length];

            return result;
        }
    }
}
