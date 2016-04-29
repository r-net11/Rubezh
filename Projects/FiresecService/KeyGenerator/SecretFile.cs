using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace KeyGenerator
{
	internal sealed class SecretFile
	{
		private readonly SymmetricAlgorithm _symmetricAlgorithm;
		private readonly string _path;

		public byte[] Key { get; set; }
		public byte[] IV { get; set; }

		public SecretFile(SymmetricAlgorithm algorithm, string key, string iv, string fileName = null)
		{
			_symmetricAlgorithm = algorithm;
			_path = fileName;
			Key = Convert.FromBase64String(key);
			IV = Convert.FromBase64String(iv);
		}

		public string ReadSensitiveData(string encryptedStr)
		{
			var encryptedStrAsBytes = Convert.FromBase64String(encryptedStr);
			var initialText = new byte[encryptedStrAsBytes.Length];

			using (var memStream = new MemoryStream(encryptedStrAsBytes))
			{
				if(Key == null || IV == null)
					throw new NullReferenceException("Key and IV must be not null");

				using (var rdTransform = _symmetricAlgorithm.CreateDecryptor((byte[]) Key.Clone(), (byte[]) IV.Clone()))
				using (var cryptoStream = new CryptoStream(memStream, rdTransform, CryptoStreamMode.Read))
					cryptoStream.Read(initialText, 0, initialText.Length);
			}

			return Convert.ToBase64String(initialText);
		}

		public void SaveSensitiveData(string sensitiveData)
		{
			if (string.IsNullOrEmpty(_path))
				throw new ArgumentException("Path to file");

			File.WriteAllText(_path, Convert.ToBase64String(EncryptData(sensitiveData)));
		}

		public string GetEncryptString(string sensitiveData)
		{
			return Convert.ToBase64String(EncryptData(sensitiveData));
		}

		private byte[] EncryptData(string input)
		{
			byte[] resultBytes;

			using (var ms = new MemoryStream())
			{
				using (var transform = _symmetricAlgorithm.CreateEncryptor(Key, IV))
				using (var cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
				{
					var encryptedString = Encoding.Unicode.GetBytes(input);
					cs.Write(encryptedString, 0, encryptedString.Length);
					cs.FlushFinalBlock();
				}
				resultBytes = ms.ToArray();
			}

			return resultBytes;
		}
	}
}
