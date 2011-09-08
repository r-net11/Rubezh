namespace Common
{
    public static class PasswordHashChecker
    {
        public static bool Check(string password, string hash)
        {
            var mD5CryptoServiceProvider = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            passwordBytes = mD5CryptoServiceProvider.ComputeHash(passwordBytes);
            var stringBuilder = new System.Text.StringBuilder();
            foreach (byte passwordByte in passwordBytes)
            {
                stringBuilder.Append(passwordByte.ToString("x2").ToLower());
            }
            string realHash = stringBuilder.ToString();

            return realHash.ToLower() == hash.ToLower();
        }
    }
}
