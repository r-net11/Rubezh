using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    public static class HashHelper
    {
        public static Dictionary<string, string> GetDirectoryHash(string directory)
        {
            var hashTable = new Dictionary<string, string>();
            var stringBuilder = new StringBuilder();
            byte[] hash = null;

            var directoryInfo = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration", directory));
            if (directoryInfo.Exists)
            {
                foreach (var fileInfo in directoryInfo.EnumerateFiles())
                {
                    using (var fileStream = fileInfo.Open(FileMode.Open))
                    using (var md5Hash = MD5.Create())
                    {
                        hash = md5Hash.ComputeHash(fileStream);
                        for (int i = 0; i < hash.Length; ++i)
                        {
                            stringBuilder.Append(hash[i].ToString("x2"));
                        }
                        stringBuilder.Append(fileInfo.Name);
                    }

                    if (hashTable.ContainsKey(stringBuilder.ToString()) == false)
                    {
                        hashTable.Add(stringBuilder.ToString(), fileInfo.Name);
                    }

                    stringBuilder.Length = 0;
                }
            }

            return hashTable;
        }

        public static List<string> GetFileNamesList(string directory)
        {
            var fileNames = new List<string>();
            var directoryInfo = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration", directory));
            if (directoryInfo.Exists)
            {
                foreach (var fileInfo in directoryInfo.EnumerateFiles())
                {
                    fileNames.Add(fileInfo.Name);
                }
            }

            return fileNames;
        }
    }
}