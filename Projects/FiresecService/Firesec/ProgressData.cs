using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Firesec
{
    public class ProgressData
    {
        public int Stage { get; set; }
        public string Comment { get; set; }
        public int PercentComplete { get; set; }
        public int BytesRW { get; set; }
    }
}