using System;

namespace Controls
{
    public static class DigitalPasswordHelper
    {
        public static bool Check(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            foreach (var passwordChar in password.ToCharArray())
            {
                if (Char.IsDigit(passwordChar) == false)
                    return false;
            }
            return true;
        }
    }
}