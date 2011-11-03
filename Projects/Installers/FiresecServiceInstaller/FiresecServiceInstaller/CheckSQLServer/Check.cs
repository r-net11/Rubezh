using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheckingSQLServer
{
    public static class Test
    {
        public static List<string> TestMethod()
        {
            var commandLineArgs = Environment.GetCommandLineArgs();
            return new List<string>(commandLineArgs);
        }
    }
}
