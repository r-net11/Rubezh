using System;
using System.IO;
using Common;
using SKDDriver;
using System.Security.Cryptography;

namespace KeyGenerator
{
	public sealed class LicenseFileManager
	{
		public void SaveToFile(string productKey, string path, string key, string iv)
		{
			if (string.IsNullOrEmpty(productKey) || string.IsNullOrEmpty(path)) return;
			Save(productKey, path, key, iv);
		}

		private static void Save(string productKey, string path, string key, string iv)
		{
			using (var rdProvider = new RijndaelManaged())
			{
				var secretFile = new SecretFile(rdProvider, path, key, iv);

				secretFile.SaveSensitiveData(productKey);
			}
		}

		public string Load(string path, string key, string iv)
		{
			if (string.IsNullOrEmpty(path) || !File.Exists(path))
			{
				Logger.Error("License file is not exists");
				return string.Empty;
			}

			using (var rdProvider = new RijndaelManaged())
			{
				var secretFile = new SecretFile(rdProvider, path);

				secretFile.Key = Convert.FromBase64String(key);
				secretFile.IV = Convert.FromBase64String(iv);

				return secretFile.ReadSensitiveData();
			}

		}
	}
}
