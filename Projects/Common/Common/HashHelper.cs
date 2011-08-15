using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Common
{
    public static class HashHelper
    {
        public static Dictionary<string, string> GetDirectoryHash(string directory)
        {
            var hashTable = new Dictionary<string, string>();
            string fullDirectory = Directory.GetCurrentDirectory() + @"\" + directory;
            var dirInfo = new DirectoryInfo(fullDirectory);
            var fileInfos = dirInfo.GetFiles();
            byte[] hash;
            var stringBuilder = new StringBuilder();
            foreach (var fileInfo in fileInfos)
            {
                stringBuilder.Clear();
                using (FileStream fileStream = fileInfo.Open(FileMode.Open))
                {
                    hash = MD5.Create().ComputeHash(fileStream);
                    for (int i = 0; i < hash.Length; i++)
                    {
                        stringBuilder.Append(hash[i].ToString());
                    }
                }
                hashTable.Add(stringBuilder.ToString(), fileInfo.Name);
            }
            return hashTable;
        }
    }
}
