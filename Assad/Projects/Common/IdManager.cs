using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public static class IdManager
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
