using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Windows.Forms;
using AppSoftware.LicenceEngine.Common;
using AppSoftware.LicenceEngine.KeyVerification;

namespace KeyGenerator
{
	public static class Generator
	{
		private static string RunQuery(string tableName, string methodName)
		{
			var mos = new ManagementObjectSearcher("Select * from Win32_" + tableName);
			foreach (ManagementObject mo in mos.Get())
			{
				try
				{
					return mo[methodName].ToString();
				}
				catch (Exception e)
				{
					MessageBox.Show(e.ToString());
				}
			}

			return string.Empty;
		}

		public static string GetSerialKey()
		{
			var resultKey = string.Empty;
			while (true)
			{
				resultKey += RunQuery("BaseBoard", "Product");
				resultKey += RunQuery("BIOS", "Version");
				resultKey += RunQuery("OperatingSystem", "SerialNumber");
				resultKey += RunQuery("Processor", "ProcessorId");

				resultKey = RemoveUseLess(resultKey);

				if (resultKey.Length >= 25)
				{
					return resultKey.Substring(0, 25).ToUpper();
				}
				if (!resultKey.Any())
					return string.Empty;
			}
		}

		private static string RemoveUseLess(string st)
		{
			for (var i = st.Length - 1; i >= 0; i--)
			{
				var ch = char.ToUpper(st[i]);

				if ((ch < 'A' || ch > 'Z')
					&& (ch < '0' || ch > '9'))
				{
					st = st.Remove(i, 1);
				}
			}
			return st;
		}



		public static bool VerifyProductKey(string productKey)
		{
			if (string.IsNullOrEmpty(productKey)) return false;

			var userEnteredLicenceKeyStr = productKey;

			var pkvKeyCheck = new PkvKeyCheck();

			var keyBytes = new[]
			{
				new KeyByteSet(5, 165, 15, 132),
				new KeyByteSet(6, 128, 175, 213)
			};

			var result = pkvKeyCheck.CheckKey(userEnteredLicenceKeyStr, keyBytes, 8, null);

			return result == PkvLicenceKeyResult.KeyGood;
		}

		public static void SaveToFile(string productKey, string userKey, string path)
		{
			if (string.IsNullOrEmpty(productKey) || string.IsNullOrEmpty(userKey) || string.IsNullOrEmpty(path)) return;
			Serialize(productKey, userKey, path);
		}

		private static void Serialize(string productKey, string userKey, string path)
		{
			var fs = new FileStream(path, FileMode.Create);

			var formatter = new BinaryFormatter();
			try
			{
				using (var myRijndael = Rijndael.Create())
				{
					var key = new Rfc2898DeriveBytes(userKey, new byte[8]); //Encoding.ASCII.GetBytes("path"));

					myRijndael.Key = key.GetBytes(myRijndael.KeySize / 8);
					myRijndael.IV = key.GetBytes(myRijndael.BlockSize / 8);
					myRijndael.Padding = PaddingMode.Zeros;

					formatter.Serialize(fs, EncryptStringToBytes(productKey, myRijndael.Key, myRijndael.IV));
				}
			}
			catch (SerializationException e)
			{
				Console.WriteLine("Failed to serialize. Reason: " + e.Message);
				throw;
			}
			finally
			{
				fs.Close();
			}
		}

		public static string Load(string userKey, string path)
		{
			if (string.IsNullOrEmpty(userKey) || string.IsNullOrEmpty(path)) return null;

			using (var myRijndael = Rijndael.Create())
			{
				//return Deserialize(DecryptStringFromBytes(LoadFromFile("LicData.dat"), myRijndael.Key, myRijndael.IV));
				var key = new Rfc2898DeriveBytes(userKey, new byte[8]); //Encoding.ASCII.GetBytes(path));
				myRijndael.Key = key.GetBytes(myRijndael.KeySize/8);
				myRijndael.IV = key.GetBytes(myRijndael.BlockSize / 8);
				return DecryptStringFromBytes(Deserialize(LoadFromFile(path)), myRijndael.Key, myRijndael.IV);
			}
		}

		public static byte[] Deserialize(byte[] userKey)
		{
			if (userKey == null) return null;
			byte[] productKey;

			try
			{
				using (var memoryStream = new MemoryStream(userKey)) //new MemoryStream(Encoding.Unicode.GetBytes(userKey)))
				{
					var formatter = new BinaryFormatter();
					productKey = (byte[])formatter.Deserialize(memoryStream);
				}
			}
			catch (SerializationException e)
			{
				Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
				throw;
			}

			return productKey;
		}

		private static byte[] LoadFromFile(string fileName)
		{
			try
			{
				byte[] loadedBytes;

				if (!File.Exists(fileName)) return null;

				using (var file = File.OpenRead(fileName))
				{
					loadedBytes = new byte[file.Length];
					file.Read(loadedBytes, 0, loadedBytes.Length);
				}
				return loadedBytes;
			}
			catch
			{
				return null;
			}
		}



		private static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
		{
			if (plainText == null || plainText.Length <= default(int))
				return null;
			if (key == null || key.Length <= default(int))
				return null;
			if (iv == null || iv.Length <= default(int))
				return null;

			using (var rijAlg = Rijndael.Create())
			{
				rijAlg.Key = key;
				rijAlg.IV = iv;

				var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

				using (var msEncrypt = new MemoryStream())
				{
					using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
					{
						using (var swEncrypt = new StreamWriter(csEncrypt))
						{
							swEncrypt.Write(plainText);
						}

						return msEncrypt.ToArray();
					}
				}
			}
		}

		private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
		{
			if (cipherText == null || cipherText.Length <= default(int))
				return null;
			if (key == null || key.Length <= default(int))
				return null;
			if (iv == null || iv.Length <= default(int))
				return null;

			string plaintext;

			using (var rijAlg = Rijndael.Create())
			{
				rijAlg.Key = key;
				rijAlg.IV = iv;
			//	rijAlg.Padding = PaddingMode.Zeros;

				var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

				using (var msDecrypt = new MemoryStream(cipherText))
				{
					using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
					{
						using (var srDecrypt = new StreamReader(csDecrypt))
						{
							plaintext = srDecrypt.ReadToEnd();
						}
					}
				}

			}

			return plaintext;
		}
	}
}
