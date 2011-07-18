using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecClient
{
    public static class PasswordService
    {
        public static bool Check(string password, string hash)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(password);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            string realHash = s.ToString();

            if (realHash.ToLower() == hash.ToLower())
            {
                return true;
            }
            return false;
        }
    }
}
