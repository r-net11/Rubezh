namespace Firesec
{
    internal class FiresecExceptionHelper
    {
        public static bool IsWellKnownException(string name)
        {
            switch (name)
            {
                case "Операция прервана":
                    return true;
            }
            return false;
        }
    }
}