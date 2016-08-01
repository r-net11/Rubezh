using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
	public static class HashHelper
	{
		public static Dictionary<string, string> GetDirectoryHash(string directory)
		{
			var hashTable = new Dictionary<string, string>();

			var directoryInfo = new DirectoryInfo(directory);

			if (!directoryInfo.Exists) return hashTable;

			foreach (var fileInfo in directoryInfo.EnumerateFiles())
			{
				string hash;
				if ((hash = GetHashFromFile(fileInfo)) != null)
				{
					hash += fileInfo.Name;
					if (hashTable.ContainsKey(hash) == false)
						hashTable.Add(hash, fileInfo.Name);
				}
			}

			return hashTable;
		}

		public static List<string> GetFileNamesList(string directory)
		{
			var directoryInfo = new DirectoryInfo(directory);
			if (directoryInfo.Exists)
			{
				return new List<string>(
					directoryInfo.EnumerateFiles().Select(fileInfo => fileInfo.Name)
				);
			}

			return new List<string>();
		}

		public static bool CheckPass(string password, string hash)
		{
			return hash.Equals(GetHashFromString(password), System.StringComparison.OrdinalIgnoreCase);
		}

		public static string GetHashFromString(string str)
		{
			var mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			var hash = new StringBuilder();

			foreach (byte passByte in mD5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(str)))
			{
				hash.Append(passByte.ToString("x2"));
			}

			return hash.ToString();
		}

		public static string GetHashFromFile(FileInfo fileInfo)
		{
			if (fileInfo.Exists == false)
				return null;

			var mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			var hash = new StringBuilder();

			using (var fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				foreach (byte passByte in mD5CryptoServiceProvider.ComputeHash(fileStream))
				{
					hash.Append(passByte.ToString("x2"));
				}
			}

			return hash.ToString();
		}
	}
}