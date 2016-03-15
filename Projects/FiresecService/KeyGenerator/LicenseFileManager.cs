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
			using (var rdProvider = new RijndaelManaged())
			{
				var secretFile = new SecretFile(rdProvider, path);

				secretFile.SaveSensitiveData(productKey);

				using (var db = new SKDDatabaseService())
				{
					db.LicenseInfoTranslator.SetKey(secretFile.Key, secretFile.IV);
				}
			}
		}

		public string Load(string path)
		{
			if (string.IsNullOrEmpty(path)) return string.Empty;

			string result;

			using (var rdProvider = new RijndaelManaged())
			{
				var secretFile = new SecretFile(rdProvider, path);

				using (var db = new SKDDatabaseService())
				{
					var key = db.LicenseInfoTranslator.GetKey();

					if (key.Key == null || key.Value == null) return string.Empty;

					secretFile.Key = key.Key;
					secretFile.IV = key.Value;

					result = secretFile.ReadSensitiveData();
				}
			}

			return result;
		}
	}
}
