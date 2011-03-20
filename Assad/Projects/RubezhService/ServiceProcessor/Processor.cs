using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Common;
using ServiceApi;

namespace ServiseProcessor
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
            Firesec.CoreConfig.config config = comDeviceToNativeConverter.Convert(configuration);
            Firesec.ComServer.SetNewConfig(config);
        }

        public static void ExecuteCommand(Device device, string commandName)
        {
            commandName = commandName.Remove(0, "Сброс ".Length);
            State comState = device.States.First(x => x.Name == commandName);
            string id = comState.Id;

            Firesec.CoreState.config coreState = new Firesec.CoreState.config();
            coreState.dev = new Firesec.CoreState.devType[1];
            coreState.dev[0] = new Firesec.CoreState.devType();
            coreState.dev[0].name = device.PlaceInTree;
            coreState.dev[0].state = new Firesec.CoreState.stateType[1];
            coreState.dev[0].state[0] = new Firesec.CoreState.stateType();
            coreState.dev[0].state[0].id = id;

            Firesec.ComServer.ResetStates(coreState);
        }

        void BuildDeviceTree()
        {
            Services.Configuration = new Configuration();
            Services.Configuration.Metadata = Firesec.ComServer.GetMetaData();
            Services.Configuration.CoreConfig = Firesec.ComServer.GetCoreConfig();

            BuildZones();

            Services.Configuration.Devices = new List<Device>();

            Firesec.CoreConfig.devType rootInnerDevice = Services.Configuration.CoreConfig.dev[0];

            Device rootDevice = new Device();
            rootDevice.Parent = null;
            rootDevice.SetInnerDevice(rootInnerDevice);
            Services.Configuration.Devices.Add(rootDevice);
            AddDevice(rootInnerDevice, rootDevice);
        }

        void AddDevice(Firesec.CoreConfig.devType parentInnerDevice, Device parentDevice)
        {
            if (parentInnerDevice.dev != null)
            {
                foreach (Firesec.CoreConfig.devType innerDevice in parentInnerDevice.dev)
                {
                    Device device = new Device();
                    device.Parent = parentDevice;
                    device.SetInnerDevice(innerDevice);
                    Services.Configuration.Devices.Add(device);
                    AddDevice(innerDevice, device);
                }
            }
        }

        // заполить зоны конфигурацией из ком-сервера

        void BuildZones()
        {
            Services.Configuration.Zones = new List<Zone>();
            if (Services.Configuration.CoreConfig.zone != null)
            {
                foreach(Firesec.CoreConfig.zoneType innerZone in Services.Configuration.CoreConfig.zone)
                {
                    Zone zone = new Zone();
                    zone.Id = innerZone.idx;
                    zone.Name = innerZone.name;
                    zone.Devices = new List<Device>();
                    Services.Configuration.Zones.Add(zone);
                }
            }
        }
    }
}
