using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientApi
{
    public class Configuration
    {
        public Firesec.Metadata.config Metadata { get; set; }
        public List<Device> Devices { get; set; }
        public List<Zone> Zones { get; set; }

        public event Action<Device> DeviceStateChanged;
        public void OnDeviceStateChanged(Device device)
        {
            if (DeviceStateChanged != null)
            {
                DeviceStateChanged(device);
            }
        }

        public event Action<Zone> ZoneStateChanged;
        public void OnZoneStateChanged(Zone zone)
        {
            if (ZoneStateChanged != null)
            {
                ZoneStateChanged(zone);
            }
        }
    }
}
