using System;
using System.Threading;

namespace Common
{
    public static class SingleLaunchHelper
    {
        static Mutex Mutex { get; set; }

        public static bool Check(string mutexName)
        {
            bool isNew;
            Mutex = new Mutex(true, mutexName, out isNew);
            return isNew;
        }

        public static void KeepAlive()
        {
            GC.KeepAlive(Mutex);
        }
    }
}