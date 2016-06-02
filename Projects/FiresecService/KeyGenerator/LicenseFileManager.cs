using System;
using System.IO;
using Common;
using SKDDriver;
using System.Security.Cryptography;

namespace KeyGenerator
{
	//public sealed class LicenseFileManager
	//{
	//	public void SaveToFile(string productKey, string path, string key, string iv)
	//	{
	//		if (string.IsNullOrEmpty(productKey) || string.IsNullOrEmpty(path)) return;
	//		Save(productKey, path, key, iv);
	//	}

	//	private static void Save(string productKey, string path, string key, string iv)
	//	{
	//		using (var rdProvider = new RijndaelManaged())
	//		{
	//			var secretFile = new SecretFile(rdProvider, key, iv, path);

	//			secretFile.SaveSensitiveData(productKey);
	//		}
	//	}

	//	public string Load(string path, string key, string iv)
	//	{
	//		if (string.IsNullOrEmpty(path) || !File.Exists(path))
	//		{
	//			Logger.Error("License file is not exists");
	//			return string.Empty;
	//		}

	//		string license;

	//		try
	//		{
	//			license = File.ReadAllText(path);
	//		}
	//		catch (Exception e)
	//		{
	//			Logger.Error(e);
	//			throw;
	//		}

	//		try
	//		{
	//			using (var rdProvider = new RijndaelManaged())
	//			{
	//				var secretFile = new SecretFile(rdProvider, key, iv, path);

	//				return secretFile.ReadSensitiveData(license);
	//			}
	//		}
	//		catch (Exception)
	//		{
	//			Logger.Info("License file is not valid.");
	//			return string.Empty;
	//		}
	//	}
	//}
}
