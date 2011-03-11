using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data
{
    public static class IdManager
    {
        static int id = 0;
        public static int Next
        {
            get
            {
                return id++;
            }
        }
    }
}
