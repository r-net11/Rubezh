using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace MuliclientAPI
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
			catch
			{
			}
		}

        public static void DecryptFile(string inputFile, string outputFile, string password)
		{
			{
				var unicodeEncoding = new UnicodeEncoding();
                byte[] key = unicodeEncoding.GetBytes(FormatPassword(password));

				FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);

				var rijndaelManaged = new RijndaelManaged();

				var cryptoStream = new CryptoStream(fsCrypt, rijndaelManaged.CreateDecryptor(key, key), CryptoStreamMode.Read);

				FileStream fsOut = new FileStream(outputFile, FileMode.Create);

				int data;
				while ((data = cryptoStream.ReadByte()) != -1)
					fsOut.WriteByte((byte)data);

				fsOut.Close();
				cryptoStream.Close();
				fsCrypt.Close();
			}
		}
	}
}