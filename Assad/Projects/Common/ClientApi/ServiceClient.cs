using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ServiceApi;

namespace ClientApi
{
    public class ServiceClient
    {
        public static FiresecClient firesecClient;

        public static CurrentConfiguration CurrentConfiguration { get; set; }
        public static CurrentStates CurrentStates { get; set; }
        public static Firesec.CoreConfig.config CoreConfig { get; set; }

        public void Start()
        {
            firesecClient = new FiresecClient();
            firesecClient.Start();

            BuildDeviceTree();

            CurrentConfiguration.FillAllDevices();
            CurrentConfiguration.SetUnderlyingZones();

            Watcher watcher = new Watcher();
            watcher.Start();

            firesecClient.Subscribe();
        }

        public void Stop()
        {
            //duplexChannelFactory.Close();
        }

        void BuildDeviceTree()
        {
            CurrentConfiguration = new CurrentConfiguration();
            CurrentConfiguration.Metadata = FiresecClient.GetMetaData();
            CoreConfig = FiresecClient.GetCoreConfig();

            FiresecToConfig firesecToConfig = new FiresecToConfig();
            CurrentConfiguration stateConfiguration = firesecToConfig.Convert(CoreConfig);
            stateConfiguration.Metadata = FiresecClient.GetMetaData();
            CurrentConfiguration = stateConfiguration;
        }

        public static void SetNewConfig(CurrentConfiguration configuration)
        {
            Validator validator = new Validator();
            validator.Validate(configuration);

            //Services.Configuration.Devices = configuration.Devices;
            //Services.Configuration.Zones = configuration.Zones;

            ConfigToFiresec configToFiresec = new ConfigToFiresec();
            Firesec.CoreConfig.config config = configToFiresec.Convert(configuration);
            FiresecClient.SetNewConfig(config);
        }

        public static void ResetState(Device device, string stateName)
        {
            DeviceState deviceState = ServiceClient.CurrentStates.DeviceStates.FirstOrDefault(x => x.Path == device.Path);
            //stateName = stateName.Remove(0, "Сброс ".Length);
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

            FiresecClient.ResetStates(coreState);
        }

        public List<Firesec.ReadEvents.journalType> ReadJournal(int startIndex, int count)
        {
            Firesec.ReadEvents.document journal = FiresecClient.ReadEvents(startIndex, count);
            if (journal.Journal != null)
            {
                if (journal.Journal.Count() > 0)
                {
                    return journal.Journal.ToList();
                }
            }
            return new List<Firesec.ReadEvents.journalType>();
        }

        //public static void StateChanged(CurrentStates currentStates)
        //{
        //    foreach (DeviceState deviceState in currentStates.DeviceStates)
        //    {
        //        DeviceState localDeviceState = CurrentStates.DeviceStates.FirstOrDefault(x => x.Path == deviceState.Path);
        //        localDeviceState = deviceState;
        //        CurrentStates.OnDeviceStateChanged(deviceState);
        //    }
        //    foreach (ZoneState zoneState in currentStates.ZoneStates)
        //    {
        //        ZoneState localZoneState = CurrentStates.ZoneStates.FirstOrDefault(x => x.No == zoneState.No);
        //        localZoneState = zoneState;
        //        CurrentStates.OnZoneStateChanged(zoneState);
        //    }
        //}

        public static void NewJournalEvent(Firesec.ReadEvents.journalType journalItem)
        {
            CurrentStates.OnNewJournalEvent(journalItem);
        }
    }
}
