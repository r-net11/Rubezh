using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Security.Cryptography;

namespace RubezhLicense
{
    public class RubezhLicenseManager<T> where T : new()
    {
        public T TryLoad(string fileName, InitialKey key)
        {
			return Deserialize(Decrypt(LoadFromFile(fileName), key.BinaryValue));
        }

		public bool TrySave(string fileName, T licenseInfo, InitialKey key)
        {
			return SaveToFile(Encrypt(Serialize(licenseInfo), key.BinaryValue), fileName);
        }

		#region Private members

		string Serialize(T licenseInfo)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
					new XmlSerializer(typeof(T)).Serialize(memoryStream, licenseInfo);
                    memoryStream.Position = 0;
                    return new StreamReader(memoryStream).ReadToEnd();
                }
            }
            catch
            {
                return null;
            }
        }

		T Deserialize(string data)
        {
            if (data == null)
                return default(T);
            
            try
            {
                using (MemoryStream memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(data)))
                {
					var xmlSerializer = new XmlSerializer(typeof(T));
					return (T)xmlSerializer.Deserialize(memoryStream);
                }
            }
            catch
            {
				return default(T);
            }
        }

        byte[] Encrypt(string data, byte[] key)
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
            catch
            {
                return null;
            }
        }

        string Decrypt(byte[] data, byte[] key)
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
            catch
            {
                return null;
            }
        }

        byte[] LoadFromFile(string fileName)
        {
            try
            {
				byte[] loadedBytes = null;
				if (File.Exists(fileName))
				{
					using (var file = File.OpenRead(fileName))
					{
						loadedBytes = new byte[file.Length];
						file.Read(loadedBytes, 0, loadedBytes.Length);
					}
				}
                return loadedBytes;
            }
            catch
            {
                return null;
            }
        }

        bool SaveToFile(byte[] data, string fileName)
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
            catch
            {
                return false;
            }
        }

        byte[] FillBytes(byte[] input, int bits)
        {
            if (input == null || input.Length == 0 || bits <= 0)
                return null;

            byte[] result = new byte[bits / 8];
            for (int i = 0; i < result.Length; i++)
                result[i] = input[i % input.Length];

            return result;
		}
		#endregion
	}
}
