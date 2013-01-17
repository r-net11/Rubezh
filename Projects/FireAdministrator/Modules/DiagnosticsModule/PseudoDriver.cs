using System;
using System.Collections.Generic;

namespace DiagnosticsModule
{
    public class PseudoDriver
    {
        int int1;
        double double1;

        public string ShortName { get; private set; }

        public PseudoDriver()
        {
            Random rnd = new Random();
            this.int1 = rnd.Next(100);
            this.double1 = rnd.NextDouble() * rnd.Next(100);
            this.ShortName = this.double1.ToString();
        }

        public static List<PseudoDriver> PseudoDrivers = new List<PseudoDriver>();
    }
}