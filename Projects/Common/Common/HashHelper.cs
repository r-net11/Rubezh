using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    public static class HashHelper
    {
        //Refactored by Badaev Andrei. See how it was in file history
        public static Dictionary<string, string> GetDirectoryHash(string directory)
        {
            var hashTable = new Dictionary<string, string>();
            var stringBuilder = new StringBuilder();
            byte[] hash = null;
            if (Directory.Exists(Directory.GetCurrentDirectory() + @"\" + directory))
            {
                var directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory() + @"\" + directory);
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
                        for (int i = 0; i < fileInfo.Name.Length; ++i)
                        {
                            stringBuilder.Append(fileInfo.Name[i]);
                        }
                    }

                    if (hashTable.ContainsKey(stringBuilder.ToString()) == false)
                    {
                        hashTable.Add(stringBuilder.ToString(), fileInfo.Name);
                    }
                    stringBuilder.Clear();
                }
            }
            return hashTable;
        }
    }
}