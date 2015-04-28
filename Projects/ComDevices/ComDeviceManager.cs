using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Common;

namespace ComDevices
{
    public static class ComDeviceManager
    {
        public static ComServer.CoreConfig.config coreConfig { get; set; }
        public static ComServer.Metadata.config metadataConfig { get; set; }
        public static List<ComDevice> Devices { get; set; }

        public static void Load()
        {
            metadataConfig = ComServer.ComServer.GetGetMetaData();
            coreConfig = ComServer.ComServer.GetCoreConfig();

            Devices = new List<ComDevice>();

            ComServer.CoreConfig.devType comDevice = coreConfig.dev[0];

            ComDevice device = new ComDevice(null, comDevice);
            Devices.Add(device);
            AddChild(comDevice, device);
        }

        static void AddChild(ComServer.CoreConfig.devType comParent, ComDevice parent)
        {
            if (comParent.dev != null)
                foreach (ComServer.CoreConfig.devType comChild in comParent.dev)
                {
                    ComDevice child = new ComDevice(parent, comChild);
                    Devices.Add(child);
                    AddChild(comChild, child);
                }
        }
    }
}
