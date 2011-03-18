using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientApi
{
    public class Configuration
    {
        public List<FullDevice> Devices { get; set; }
        public List<FullZone> Zones { get; set; }
    }
}
