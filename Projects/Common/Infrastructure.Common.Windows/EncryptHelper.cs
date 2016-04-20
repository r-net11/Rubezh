using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Common.Windows
{
	public static class EncryptHelper
	{
		static string FormatPassword(string password)
		{
			if (password == null)
				password = "";
			if (password.Length > 8)
				return password.Substring(0, 8);
			if (password.Length < 8)
				password += new string(' ', 8 - password.Length);
			return password;
		}

		public static void EncryptFile(string inputFile, string outputFile, string password)
		{
			try
			{
				var unicodeEncoding = new UnicodeEncoding();
				byte[] key = unicodeEncoding.GetBytes(FormatPassword(password));

				string cryptFile = outputFile;
				FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);
				var rijndaelManaged = new RijndaelManaged();
				var cryptoStream = new CryptoStream(fsCrypt, rijndaelManaged.CreateEncryptor(key, key), CryptoStreamMode.Write);
				FileStream fsIn = new FileStream(inputFile, FileMode.Open);

				int data;
				while ((data = fsIn.ReadByte()) != -1)
					cryptoStream.WriteByte((byte)data);

				fsIn.Close();
				cryptoStream.Close();
				fsCrypt.Close();
			}
			catch { }
		}

		public static void DecryptFile(string inputFile, string outputFile, string password)
		{
			{
				var unicodeEncoding = new UnicodeEncoding();
				byte[] key = unicodeEncoding.GetBytes(FormatPassword(password));

				if (!File.Exists(inputFile))
				{
					File.Create(inputFile);
				}
				FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);
				var rijndaelManaged = new RijndaelManaged();
				var cryptoStream = new CryptoStream(fsCrypt, rijndaelManaged.CreateDecryptor(key, key), CryptoStreamMode.Read);
				var fileStream = new FileStream(outputFile, FileMode.Create);

				try
				{
					int data;
					while ((data = cryptoStream.ReadByte()) != -1)
						fileStream.WriteByte((byte)data);
				}
				catch { throw; }
				finally
				{
					fsCrypt.Close();
					fileStream.Close();
					cryptoStream.Close();
				}
			}
		}
	}
}