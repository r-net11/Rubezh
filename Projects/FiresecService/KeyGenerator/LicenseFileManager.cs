using System;
using System.IO;
using Common;
using SKDDriver;
using System.Security.Cryptography;

namespace KeyGenerator
{
	public sealed class LicenseFileManager
	{
		public void SaveToFile(string productKey, string path)
		{
			if (string.IsNullOrEmpty(productKey) || string.IsNullOrEmpty(path)) return;
			Save(productKey, path);
		}

		private static void Save(string productKey, string path)
		{
			try
			{
				File.WriteAllText(path, productKey);
			}
			catch (Exception e)
			{
				Logger.Error(e);
				Logger.Error(string.Format("Path to license: {0}", path));
				throw;
			}

			//using (var rdProvider = new RijndaelManaged())
			//{
			//	var secretFile = new SecretFile(rdProvider, path);

			//	secretFile.SaveSensitiveData(productKey);

			//	using (var db = new SKDDatabaseService())
			//	{
			//		db.LicenseInfoTranslator.SetKey(secretFile.Key, secretFile.IV);
			//	}
			//}
		}

		public string Load(string path)
		{
			if (string.IsNullOrEmpty(path) || !File.Exists(path))
			{
				Logger.Error("License file is not exists");
				return string.Empty;
			}

			string result;

			try
			{
				result = File.ReadAllText(path);
			}
			catch (Exception e)
			{
				Logger.Error(e);
				Logger.Error(string.Format("Path to license: {0}", path));
				throw;
			}
			return result;

			//using (var rdProvider = new RijndaelManaged())
			//{
			//	var secretFile = new SecretFile(rdProvider, path);

			//	using (var db = new SKDDatabaseService())
			//	{
			//		var key = db.LicenseInfoTranslator.GetKey();

			//		if (key.Key == null || key.Value == null) return string.Empty;

			//		secretFile.Key = key.Key;
			//		secretFile.IV = key.Value;

			//		result = secretFile.ReadSensitiveData();
			//	}
			//}
		}
	}
}
