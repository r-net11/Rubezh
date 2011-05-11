using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssadProcessor.Devices;

namespace AssadProcessor
{
    public static class Configuration
    {
        public static List<AssadBase> BaseDevices { get; set; }
        public static List<AssadDevice> Devices { get; set; }
        public static List<AssadZone> Zones { get; set; }
        public static AssadMonitor Monitor { get; set; }
    }
}
