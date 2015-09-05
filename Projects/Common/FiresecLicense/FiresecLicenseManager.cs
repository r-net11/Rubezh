using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Security.Cryptography;

namespace FiresecLicense
{
    public static class FiresecLicenseManager
    {
		public static event Action LicenseChanged;

		static InitialKey _initialKey;
		public static InitialKey InitialKey 
		{ 
			get
			{
				if (_initialKey == null)
					_initialKey = InitialKey.Generate();
				return _initialKey;
			}
		}

		static FiresecLicenseInfo _currentLicenseInfo = new FiresecLicenseInfo();
		public static FiresecLicenseInfo CurrentLicenseInfo 
		{
			get { return _currentLicenseInfo; } 
			set
			{
				if (!TheSame(_currentLicenseInfo, value))
				{
					PreviousLicenseInfo = _currentLicenseInfo;
					_currentLicenseInfo = value;
					if (LicenseChanged != null)
						LicenseChanged();
				}
			}
		}

		public static FiresecLicenseInfo PreviousLicenseInfo { get; private set; }

        public static List<Exception> Exceptions = new List<Exception>();

		public static FiresecLicenseInfo TryLoad(string fileName)
		{
			return TryLoad(fileName, InitialKey);
		}
        public static FiresecLicenseInfo TryLoad(string fileName, InitialKey key)
        {
			Exceptions.Clear();
			var result = Deserialize(Decrypt(LoadFromFile(fileName), key.BinaryValue));
			if (result != null)
				result.LicenseMode = LicenseMode.HasLicense;
			return result;
        }
		public static bool TrySave(string fileName, FiresecLicenseInfo licenseInfo)
		{
			return TrySave(fileName, licenseInfo, InitialKey);
		}
		public static bool TrySave(string fileName, FiresecLicenseInfo licenseInfo, InitialKey key)
        {
			Exceptions.Clear();
			return SaveToFile(Encrypt(Serialize(licenseInfo), key.BinaryValue), fileName);
        }

		public static bool CheckLicense(string path)
		{
			return TryLoad(path) != null;
		}

		#region Private members

		static bool TheSame(FiresecLicenseInfo licenseInfo1, FiresecLicenseInfo licenseInfo2)
		{
			if (licenseInfo1 == licenseInfo2)
				return true;
			if (licenseInfo1 == null || licenseInfo2 == null)
				return false;
			return licenseInfo1.RemoteWorkplacesCount == licenseInfo2.RemoteWorkplacesCount
					&& licenseInfo1.Fire == licenseInfo2.Fire
					&& licenseInfo1.Security == licenseInfo2.Security
					&& licenseInfo1.Access == licenseInfo2.Access
					&& licenseInfo1.Video == licenseInfo2.Video
					&& licenseInfo2.OpcServer == licenseInfo2.OpcServer;
		}
		static string Serialize(FiresecLicenseInfo licenseInfo)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
					new XmlSerializer(typeof(FiresecLicenseInfo)).Serialize(memoryStream, licenseInfo);
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

		static FiresecLicenseInfo Deserialize(string data)
        {
            if (data == null)
                return null;
            
            try
            {
                using (MemoryStream memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(data)))
                {
					var xmlSerializer = new XmlSerializer(typeof(FiresecLicenseInfo));
					return (FiresecLicenseInfo)xmlSerializer.Deserialize(memoryStream);
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
		#endregion
	}
}
