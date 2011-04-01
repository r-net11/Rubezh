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

        public static void SetNewConfig(CurrentConfiguration configuration)
        {
            Validator validator = new Validator();
            validator.Validate(configuration);

            //Services.Configuration.Devices = configuration.Devices;
            //Services.Configuration.Zones = configuration.Zones;

            ConfigToFiresec configToFiresec = new ConfigToFiresec();
            Firesec.CoreConfig.config config = configToFiresec.Convert(configuration);
            Firesec.FiresecClient.SetNewConfig(config);
        }

        public static void ExecuteCommand(DeviceState device, string commandName)
        {
            commandName = commandName.Remove(0, "Сброс ".Length);
            InnerState state = device.InnerStates.First(x => x.Name == commandName);
            string id = state.Id;

            Firesec.CoreState.config coreState = new Firesec.CoreState.config();
            coreState.dev = new Firesec.CoreState.devType[1];
            coreState.dev[0] = new Firesec.CoreState.devType();
            string placeInTree = Services.AllDevices.FirstOrDefault(x => x.Path == device.Path).PlaceInTree;
            coreState.dev[0].name = placeInTree;
            coreState.dev[0].state = new Firesec.CoreState.stateType[1];
            coreState.dev[0].state[0] = new Firesec.CoreState.stateType();
            coreState.dev[0].state[0].id = id;

            Firesec.FiresecClient.ResetStates(coreState);
        }

        void BuildDeviceTree()
        {
            Services.CurrentConfiguration = new CurrentConfiguration();
            Services.CurrentConfiguration.Metadata = Firesec.FiresecClient.GetMetaData();
            Services.CoreConfig = Firesec.FiresecClient.GetCoreConfig();

            FiresecToConfig firesecToConfig = new FiresecToConfig();
            CurrentConfiguration stateConfiguration = firesecToConfig.Convert(Services.CoreConfig);
            stateConfiguration.Metadata = Firesec.FiresecClient.GetMetaData();
            Services.CurrentConfiguration = stateConfiguration;
        }
    }
}
