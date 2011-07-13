using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using FiresecClient.Models;
using FiresecClient.Converters;

namespace FiresecClient
{
    public class FiresecManager
    {
        public static CurrentConfiguration Configuration { get; set; }
        public static CurrentStates States { get; set; }
        public static Firesec.CoreConfig.config CoreConfig { get; set; }

        public static bool Connect(string login, string password)
        {
            bool result = FiresecInternalClient.Connect(login, password);
            if (result)
            {
                _loggedInUserName = login;
                BuildDeviceTree();
                Configuration.Update();
                Watcher watcher = new Watcher();
                watcher.Start();
            }
            return result;
        }

        public static string _loggedInUserName { get; set; }
        public static User CurrentUser
        {
            get
            {
                return Configuration.Users.FirstOrDefault(x => x.Name == _loggedInUserName);
            }
        }

        public static List<Perimission> CurrentPermissions
        {
            get
            {
                List<string> permissionIds = new List<string>();

                foreach (var groupId in CurrentUser.Groups)
                {
                    var group = Configuration.UserGroups.FirstOrDefault(x => x.Id == groupId);

                    permissionIds.AddRange(group.Permissions);
                }
                permissionIds.AddRange(CurrentUser.Permissions);

                foreach (var permissionId in CurrentUser.RemovedPermissions)
                {
                    permissionIds.Remove(permissionId);
                }

                var permissions = new List<Perimission>(from permission in Configuration.Perimissions
                               where permissionIds.Contains(permission.Id)
                               select permission);
                return permissions;
            }
        }

        public static void Disconnect()
        {
            FiresecInternalClient.Disconnect();
        }

        static void BuildDeviceTree()
        {
            CoreConfig = FiresecInternalClient.GetCoreConfig();
            Configuration = new CurrentConfiguration();
            Configuration.Metadata = FiresecInternalClient.GetMetaData();
            Configuration.FillDrivrs(Configuration.Metadata);
            Convert();
        }

        public static void SetNewConfig()
        {
            Validator validator = new Validator();
            validator.Validate(Configuration);
            ConvertBack();
            FiresecInternalClient.SetNewConfig(CoreConfig);
        }

        static void Convert()
        {
            FiresecManager.States = new CurrentStates();
            ZoneConverter.Convert(CoreConfig);
            DirectionConverter.Convert(CoreConfig);
            SecurityConverter.Convert(CoreConfig);
            DeviceConverter.Convert(CoreConfig);
        }

        static void ConvertBack()
        {
            ZoneConverter.ConvertBack(Configuration);
            DeviceConverter.ConvertBack(Configuration);
            DirectionConverter.ConvertBack(Configuration);
            SecurityConverter.ConvertBack(Configuration);
        }

        public static void LoadFromFile(string fileName)
        {
            CoreConfig = FiresecInternalClient.LoadConfigFromFile(fileName);
            FiresecManager.States = new CurrentStates();
            ZoneConverter.Convert(CoreConfig);
            DirectionConverter.Convert(CoreConfig);
            //SecurityConverter.Convert(CoreConfig);
            DeviceConverter.Convert(CoreConfig);
        }

        public static void SaveToFile(string fileName)
        {
            ZoneConverter.ConvertBack(Configuration);
            DeviceConverter.ConvertBack(Configuration);
            DirectionConverter.ConvertBack(Configuration);
            //SecurityConverter.ConvertBack(Configuration);
            FiresecInternalClient.SaveConfigToFile(CoreConfig, fileName);
        }

        public static void ResetState(string deviceId, string stateName)
        {
            DeviceState deviceState = FiresecManager.States.DeviceStates.FirstOrDefault(x => x.Id == deviceId);
            InnerState state = deviceState.InnerStates.First(x => x.Name == stateName);

            Firesec.CoreState.config coreState = new Firesec.CoreState.config();
            coreState.dev = new Firesec.CoreState.devType[1];
            coreState.dev[0] = new Firesec.CoreState.devType();
            coreState.dev[0].name = deviceState.PlaceInTree;
            coreState.dev[0].state = new Firesec.CoreState.stateType[1];
            coreState.dev[0].state[0] = new Firesec.CoreState.stateType();
            coreState.dev[0].state[0].id = state.Id;

            FiresecInternalClient.ResetStates(coreState);
        }

        public static void Reset(List<ResetItem> resetItems)
        {
            List<Firesec.CoreState.devType> innerDevices = new List<Firesec.CoreState.devType>();

            foreach (var resetItem in resetItems)
            {
                if (resetItem == null)
                    continue;

                var deviceState = FiresecManager.States.DeviceStates.FirstOrDefault(x => x.Id == resetItem.DeviceId);

                Firesec.CoreState.devType innerDevice = new Firesec.CoreState.devType();
                innerDevice.name = deviceState.PlaceInTree;

                List<Firesec.CoreState.stateType> innerStates = new List<Firesec.CoreState.stateType>();

                foreach (var state in resetItem.States)
                {
                    var innerState = deviceState.InnerStates.First(x => x.Name == state);
                    innerStates.Add(new Firesec.CoreState.stateType() { id = innerState.Id });
                }
                innerDevice.state = innerStates.ToArray();
                innerDevices.Add(innerDevice);
            }

            Firesec.CoreState.config coreState = new Firesec.CoreState.config();
            coreState.dev = innerDevices.ToArray();

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
