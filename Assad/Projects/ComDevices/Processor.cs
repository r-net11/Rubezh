using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Common;
using ServiceApi;

namespace ComDevices
{
    public class Processor
    {
        public void Start()
        {
            BuildDeviceTree();

            ServiceManager.Open();

            Watcher watcher = new Watcher();
            watcher.Start();
        }

        public void Stop()
        {
            ServiceManager.Close();
        }

        public static void SetNewConfig(Configuration configuration)
        {
            Validator.Validate(configuration);

            Services.Configuration.Devices = configuration.Devices;
            Services.Configuration.Zones = configuration.Zones;

            Converter comDeviceToNativeConverter = new Converter();
            ComServer.CoreConfig.config config = comDeviceToNativeConverter.Convert(configuration);
            ComServer.ComServer.SetNewConfig(config);
        }

        void BuildDeviceTree()
        {
            Services.Configuration = new Configuration();
            Services.Configuration.Metadata = ComServer.ComServer.GetMetaData();
            Services.Configuration.CoreConfig = ComServer.ComServer.GetCoreConfig();

            Services.Configuration.Devices = new List<Device>();

            ComServer.CoreConfig.devType innerDevice = Services.Configuration.CoreConfig.dev[0];

            Device device = new Device();
            device.Parent = null;
            DeviceHelper.SetDeviceInnerDevice(device, innerDevice);
            Services.Configuration.Devices.Add(device);
            AddChild(innerDevice, device);
        }

        void AddChild(ComServer.CoreConfig.devType comParent, Device parent)
        {
            if (comParent.dev != null)
            {
                foreach (ComServer.CoreConfig.devType comChild in comParent.dev)
                {
                    Device child = new Device();
                    child.Parent = parent;
                    DeviceHelper.SetDeviceInnerDevice(child, comChild);
                    Services.Configuration.Devices.Add(child);
                    AddChild(comChild, child);
                }
            }
        }
    }
}
