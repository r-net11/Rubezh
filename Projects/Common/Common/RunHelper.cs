using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Common
{
    public static class RunHelper
    {
        static Mutex Mutex { get; set; }

        public static bool Run(string mutexName)
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
