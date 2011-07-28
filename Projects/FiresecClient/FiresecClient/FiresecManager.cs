using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using FiresecAPI.Models;
using FiresecAPI;

namespace FiresecClient
{
    public class EventClient : IFiresecCallback
    {
        public void StateChanged(string deviceId)
        {
        }
    }

    public class FiresecManager
    {
        public static CurrentConfiguration Configuration { get; set; }
        public static CurrentStates States { get; set; }

        static DuplexChannelFactory<IFiresecService> _duplexChannelFactory;
        public static IFiresecService FiresecService;
        static EventClient _eventClient;

        public static bool Connect(string login, string password)
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.MaxBufferSize = Int32.MaxValue;
            binding.MaxReceivedMessageSize = Int32.MaxValue;
            binding.MaxBufferPoolSize = Int32.MaxValue;
            binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
            EndpointAddress endpointAddress = new EndpointAddress("net.tcp://localhost:8000/FiresecService");
            _eventClient = new EventClient();
            _duplexChannelFactory = new DuplexChannelFactory<IFiresecService>(new InstanceContext(_eventClient), binding, endpointAddress);
            FiresecService = _duplexChannelFactory.CreateChannel();


            States = FiresecService.GetCurrentStates();

            Configuration = FiresecService.GetCoreConfig();

            Update();

            _loggedInUserName = login;

            return true;
        }

        static void Update()
        {
            Configuration.Update();
            Configuration.UpdateDrivers();

            foreach (var deviceState in States.DeviceStates)
            {
                deviceState.Device = FiresecManager.Configuration.Devices.FirstOrDefault(x => x.Id == deviceState.Id);
            }
        }

        static string _loggedInUserName;
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
            //FiresecInternalClient.Disconnect();
        }

        static void BuildDeviceTree()
        {
            //CoreConfig = FiresecInternalClient.GetCoreConfig();
            //Configuration = new CurrentConfiguration();
            //var metadata = FiresecInternalClient.GetMetaData();
            //Configuration.FillDrivrs(metadata);
            //Convert();
        }

        public static void SetNewConfig()
        {
            //Validator validator = new Validator();
            //validator.Validate(Configuration);
            //ConvertBack();
            //FiresecInternalClient.SetNewConfig(CoreConfig);
        }

        static void Convert()
        {
            //FiresecManager.States = new CurrentStates();
            //ZoneConverter.Convert(CoreConfig);
            //DirectionConverter.Convert(CoreConfig);
            //SecurityConverter.Convert(CoreConfig);
            //DeviceConverter.Convert(CoreConfig);
        }

        static void ConvertBack()
        {
            //ZoneConverter.ConvertBack(Configuration);
            //DeviceConverter.ConvertBack(Configuration);
            //DirectionConverter.ConvertBack(Configuration);
            //SecurityConverter.ConvertBack(Configuration);
        }

        public static void LoadFromFile(string fileName)
        {
            //CoreConfig = FiresecInternalClient.LoadConfigFromFile(fileName);
            //FiresecManager.States = new CurrentStates();
            //ZoneConverter.Convert(CoreConfig);
            //DirectionConverter.Convert(CoreConfig);
            //DeviceConverter.Convert(CoreConfig);
        }

        public static void SaveToFile(string fileName)
        {
            //ZoneConverter.ConvertBack(Configuration);
            //DeviceConverter.ConvertBack(Configuration);
            //DirectionConverter.ConvertBack(Configuration);
            //FiresecInternalClient.SaveConfigToFile(CoreConfig, fileName);
        }

        public static void AddToIgnoreList(List<string> devicePaths)
        {
            //FiresecInternalClient.AddToIgnoreList(devicePaths);
        }

        public static void RemoveFromIgnoreList(List<string> devicePaths)
        {
            //FiresecInternalClient.RemoveFromIgnoreList(devicePaths);
        }

        public static void ResetOne(string deviceId, string stateName)
        {
            //FiresecResetHelper.ResetOne(deviceId, stateName);
        }

        public static void ResetMany(List<ResetItem> resetItems)
        {
            //FiresecResetHelper.ResetMany(resetItems);
        }

        public static void AddUserMessage(string message)
        {
            //FiresecInternalClient.AddUserMessage(message);
        }

        public static void ExecuteCommand(string devicePath, string methodName)
        {
            //FiresecInternalClient.ExecuteCommand(devicePath, methodName);
        }

        public static List<JournalItem> ReadJournal(int startIndex, int count)
        {
            return FiresecService.ReadJournal(startIndex, count);
        }
    }
}
