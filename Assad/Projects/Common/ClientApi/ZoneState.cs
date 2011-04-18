using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ClientApi
{
    public class ZoneState
    {
        public string No { get; set; }
        public string State { get; set; }
        public bool ZoneChanged { get; set; }
    }
}
