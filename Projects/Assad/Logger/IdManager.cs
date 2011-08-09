namespace Logger
{
    internal static class IdManager
    {
        static int id = 1;
        public static int Next
        {
            get
            {
                return id++;
            }
        }
    }
}