using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
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

        public static void SetNewConfig(StateConfiguration configuration)
        {
            Validator.Validate(configuration);

            //Services.Configuration.Devices = configuration.Devices;
            //Services.Configuration.Zones = configuration.Zones;

            //Converter deviceToNativeConverter = new Converter();
            ConfigToFiresec configToFiresec = new ConfigToFiresec();
            Firesec.CoreConfig.config config = configToFiresec.Convert(configuration);
            Firesec.FiresecClient.SetNewConfig(config);
        }

        public static void ExecuteCommand(ShortDeviceState device, string commandName)
        {
            commandName = commandName.Remove(0, "Сброс ".Length);
            State state = device.InnerStates.First(x => x.Name == commandName);
            string id = state.Id;

            Firesec.CoreState.config coreState = new Firesec.CoreState.config();
            coreState.dev = new Firesec.CoreState.devType[1];
            coreState.dev[0] = new Firesec.CoreState.devType();
            string placeInTree = Services.Configuration.ShortDevices.FirstOrDefault(x => x.Path == device.Path).PlaceInTree;
            coreState.dev[0].name = placeInTree;
            coreState.dev[0].state = new Firesec.CoreState.stateType[1];
            coreState.dev[0].state[0] = new Firesec.CoreState.stateType();
            coreState.dev[0].state[0].id = id;

            Firesec.FiresecClient.ResetStates(coreState);
        }

        void BuildDeviceTree()
        {
            Services.Configuration = new Configuration();
            Services.Configuration.Metadata = Firesec.FiresecClient.GetMetaData();
            Services.Configuration.CoreConfig = Firesec.FiresecClient.GetCoreConfig();

            FiresecToConfig firesecToConfig = new FiresecToConfig();
            StateConfiguration stateConfiguration = firesecToConfig.Convert(Services.Configuration.CoreConfig);
            stateConfiguration.Metadata = Firesec.FiresecClient.GetMetaData();
            Services.Configuration.StateConfiguration = stateConfiguration;

            //BuildZones();

            //Services.Configuration.Devices = new List<Device>();

            //Firesec.CoreConfig.devType rootInnerDevice = Services.Configuration.CoreConfig.dev[0];

            //Device rootDevice = new Device();
            //rootDevice.Parent = null;
            //rootDevice.SetInnerDevice(rootInnerDevice);
            //Services.Configuration.Devices.Add(rootDevice);
            //AddDevice(rootInnerDevice, rootDevice);
        }

        //void AddDevice(Firesec.CoreConfig.devType parentInnerDevice, Device parentDevice)
        //{
        //    if (parentInnerDevice.dev != null)
        //    {
        //        foreach (Firesec.CoreConfig.devType innerDevice in parentInnerDevice.dev)
        //        {
        //            Device device = new Device();
        //            device.Parent = parentDevice;
        //            device.SetInnerDevice(innerDevice);
        //            Services.Configuration.Devices.Add(device);
        //            AddDevice(innerDevice, device);
        //        }
        //    }
        //}

        //// заполить зоны конфигурацией из ком-сервера

        //void BuildZones()
        //{
        //    Services.Configuration.Zones = new List<Zone>();
        //    if (Services.Configuration.CoreConfig.zone != null)
        //    {
        //        foreach(Firesec.CoreConfig.zoneType innerZone in Services.Configuration.CoreConfig.zone)
        //        {
        //            Zone zone = new Zone();
        //            zone.Name = innerZone.name;
        //            zone.No = innerZone.no;
        //            zone.Description = innerZone.desc;
        //            if (innerZone.param != null)
        //            {
        //                if (innerZone.param.Any(x => x.name == "ExitTime"))
        //                    zone.EvacuationTime = innerZone.param.FirstOrDefault(x => x.name == "ExitTime").value;
        //                if (innerZone.param.Any(x => x.name == "FireDeviceCount"))
        //                    zone.DetectorCount = innerZone.param.FirstOrDefault(x => x.name == "FireDeviceCount").value;
        //            }
        //            zone.Devices = new List<Device>();
        //            Services.Configuration.Zones.Add(zone);
        //        }
        //    }
        //}
    }
}
