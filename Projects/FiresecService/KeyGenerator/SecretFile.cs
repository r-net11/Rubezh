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

		public SecretFile(SymmetricAlgorithm algorithm, string fileName)
		{
			_symmetricAlgorithm = algorithm;
			_path = fileName;
		}

		public void SaveSensitiveData(string sensitiveData)
		{
			//Encode data string to be stored in encrypted file.
			var encodedData = Encoding.Unicode.GetBytes(sensitiveData);

			try
			{
				//Create FileStream and crypto service provider objects.
				using (var fs = new FileStream(_path, FileMode.Create, FileAccess.Write))
				{
					//Generate and save secret key and init vector.
					GenerateSecretKey();
					GenerateSecretInitVector();

					//Create crypto transform and stream objects
					using (var transform = _symmetricAlgorithm.CreateEncryptor(Key, IV))
					{
						using (var cryptoStream = new CryptoStream(fs, transform, CryptoStreamMode.Write))
						{
							//Write encrypted data to the file
							cryptoStream.Write(encodedData, 0, encodedData.Length);
							cryptoStream.FlushFinalBlock();
							cryptoStream.Clear();
						}
					}
				}
			}
			catch (IOException)
			{

			}
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

		private void GenerateSecretKey()
		{
			var rdProvider = _symmetricAlgorithm as RijndaelManaged;

			if (rdProvider == null)
				throw new ArgumentException("Invalid algorithm");

			rdProvider.KeySize = 256; //MaximumKeySize
			rdProvider.GenerateKey();
			Key = rdProvider.Key;
		}

		private void GenerateSecretInitVector()
		{
			var rdProvider = _symmetricAlgorithm as RijndaelManaged;

			if (rdProvider == null)
				throw new ArgumentException("Invalid algorithm");

			rdProvider.GenerateIV();
			IV = rdProvider.IV;
		}
	}
}
