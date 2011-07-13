using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssadProcessor.Devices;

namespace AssadProcessor
{
    public static class Configuration
    {
        static Configuration()
        {
            BaseDevices = new List<AssadBase>();
            Devices = new List<AssadDevice>();
            Zones = new List<AssadZone>();
        }

        public static List<AssadBase> BaseDevices { get; set; }
        public static List<AssadDevice> Devices { get; set; }
        public static List<AssadZone> Zones { get; set; }
        public static AssadMonitor Monitor { get; set; }
    }
}
