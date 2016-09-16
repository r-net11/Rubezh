using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Licensing.Services
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

		public void SaveSensitiveData(string sensitiveData)
		{
			if(string.IsNullOrEmpty(_path))
				throw new ArgumentException("Path to file");

			File.WriteAllText(_path, EncryptData(sensitiveData));
		}

		public string ReadSensitiveData()
		{
			if (Key == null || IV == null) return string.Empty;

			string decrypted = string.Empty;

			try
			{
				//Create file stream to read encrypted file back.
				using (var fs = new FileStream(_path, FileMode.Open, FileAccess.Read))
				{
					//Create decryptor.
					using (var transform = _symmetricAlgorithm.CreateDecryptor(Key, IV))
					{
						using (var cryptoStream = new CryptoStream(fs, transform, CryptoStreamMode.Read))
						{
							//Print out the contents of the decrypted file
							using (var srDecrypted = new StreamReader(cryptoStream, new UnicodeEncoding()))
							{
								decrypted = srDecrypted.ReadToEnd(); //TODO: catch cryptographic exception
							}
						}
					}
				}
			}
			catch (FileNotFoundException)
			{
			}

			return decrypted;
		}

		public string GetEncryptString(string sensitiveData)
		{
			return EncryptData(sensitiveData);
		}

		private string EncryptData(string input)
		{
			var originalStrAsBytes = Convert.FromBase64String(input);
			byte[] originalBytes;

			// Create MemoryStream to contain output.
			using (var memStream = new MemoryStream(originalStrAsBytes.Length))
			{
				// Create encryptor and stream objects.
				using (ICryptoTransform rdTransform = _symmetricAlgorithm.CreateEncryptor((byte[])Key.Clone(),(byte[])IV.Clone()))
				{
					using (var cryptoStream = new CryptoStream(memStream, rdTransform, CryptoStreamMode.Write))
					{
						// Write encrypted data to the MemoryStream.
						cryptoStream.Write(originalStrAsBytes, 0, originalStrAsBytes.Length);
						cryptoStream.FlushFinalBlock();
						originalBytes = memStream.ToArray();
					}
				}
			}
			// Convert encrypted string.
			return Convert.ToBase64String(originalBytes);
		}
	}
}
