namespace Common
{
    public static class PasswordHashChecker
    {
        public static bool Check(string password, string hash)
        {
            var mD5CryptoServiceProvider = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            passwordBytes = mD5CryptoServiceProvider.ComputeHash(passwordBytes);

            var realHash = new System.Text.StringBuilder();
            foreach (byte passwordByte in passwordBytes)
            {
                realHash.Append(passwordByte.ToString("x2"));
            }

            return hash.Equals(realHash.ToString(), System.StringComparison.OrdinalIgnoreCase);
        }
    }
}