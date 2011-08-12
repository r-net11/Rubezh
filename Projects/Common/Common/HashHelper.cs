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
            string currentDirectory = Directory.GetCurrentDirectory() + @"\" + directory;
            var dir = new DirectoryInfo(currentDirectory);
            var files = dir.GetFiles();
            byte[] hash;
            var sBuilder = new StringBuilder();
            foreach (var fInfo in files)
            {
                sBuilder.Clear();
                using (FileStream fileStream = fInfo.Open(FileMode.Open))
                {
                    hash = MD5.Create().ComputeHash(fileStream);
                    for (int i = 0; i < hash.Length; i++)
                    {
                        sBuilder.Append(hash[i].ToString());
                    }
                }
                hashTable.Add(sBuilder.ToString(), fInfo.Name);
            }
            return hashTable;
        }
    }
}
