using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace FiresecClient
{
    public class FiresecManager
    {
        static FiresecInternalClient firesecInternalClient;

        public static CurrentConfiguration CurrentConfiguration { get; set; }
        public static CurrentStates CurrentStates { get; set; }
        public static Firesec.CoreConfig.config CoreConfig { get; set; }

        static FiresecManager()
        {
            Start();
        }

        public static void Start()
        {
            if (firesecInternalClient != null)
                return;

            firesecInternalClient = new FiresecInternalClient();
            firesecInternalClient.Start();

            BuildDeviceTree();

            CurrentConfiguration.FillAllDevices();
            CurrentConfiguration.SetUnderlyingZones();

            Watcher watcher = new Watcher();
            watcher.Start();

            firesecInternalClient.Subscribe();
        }

        public static void Stop()
        {
            firesecInternalClient.Stop();
        }

        static void BuildDeviceTree()
        {
            CurrentConfiguration = new CurrentConfiguration();
            CurrentConfiguration.Metadata = FiresecInternalClient.GetMetaData();
            CoreConfig = FiresecInternalClient.GetCoreConfig();

            FiresecToConfig firesecToConfig = new FiresecToConfig();
            CurrentConfiguration stateConfiguration = firesecToConfig.Convert(CoreConfig);
            stateConfiguration.Metadata = FiresecInternalClient.GetMetaData();
            CurrentConfiguration = stateConfiguration;
        }

        public static void SetNewConfig(CurrentConfiguration configuration)
        {
            Validator validator = new Validator();
            validator.Validate(configuration);

            ConfigToFiresec configToFiresec = new ConfigToFiresec();
            Firesec.CoreConfig.config config = configToFiresec.Convert(configuration);
            FiresecInternalClient.SetNewConfig(config);
        }

        public static void ResetState(Device device, string stateName)
        {
            DeviceState deviceState = FiresecManager.CurrentStates.DeviceStates.FirstOrDefault(x => x.Path == device.Path);
            InnerState state = deviceState.InnerStates.First(x => x.Name == stateName);
            string id = state.Id;

            Firesec.CoreState.config coreState = new Firesec.CoreState.config();
            coreState.dev = new Firesec.CoreState.devType[1];
            coreState.dev[0] = new Firesec.CoreState.devType();
            string placeInTree = CurrentConfiguration.AllDevices.FirstOrDefault(x => x.Path == deviceState.Path).PlaceInTree;
            coreState.dev[0].name = placeInTree;
            coreState.dev[0].state = new Firesec.CoreState.stateType[1];
            coreState.dev[0].state[0] = new Firesec.CoreState.stateType();
            coreState.dev[0].state[0].id = id;

            FiresecInternalClient.ResetStates(coreState);
        }

        public static List<Firesec.ReadEvents.journalType> ReadJournal(int startIndex, int count)
        {
            Firesec.ReadEvents.document journal = FiresecInternalClient.ReadEvents(startIndex, count);
            if (journal.Journal != null)
            {
                if (journal.Journal.Count() > 0)
                {
                    return journal.Journal.ToList();
                }
            }
            return new List<Firesec.ReadEvents.journalType>();
        }
    }
}
