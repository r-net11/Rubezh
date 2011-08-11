using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using FiresecAPI;
using FiresecAPI.Models;

namespace FiresecClient
{
    public class FiresecManager
    {
        public static List<Driver> Drivers { get; set; }
        public static DeviceConfiguration DeviceConfiguration { get; set; }
        public static DeviceConfigurationStates DeviceStates { get; set; }
        public static SystemConfiguration SystemConfiguration { get; set; }
        public static SecurityConfiguration SecurityConfiguration { get; set; }
        public static FileHelper FileHelper { get; private set; }

        static SafeFiresecService _firesecService;

        static Thread _pingThread;

        public static bool Connect(string login, string password)
        {
            IFiresecService firesecService = FiresecServiceFactory.Create();
            _firesecService = new SafeFiresecService(firesecService);

            _firesecService.Connect();
            Drivers = _firesecService.GetDrivers();
            SystemConfiguration = _firesecService.GetSystemConfiguration();
            SecurityConfiguration = _firesecService.GetSecurityConfiguration();
            DeviceConfiguration = _firesecService.GetDeviceConfiguration();
            DeviceStates = _firesecService.GetStates();
            Update();

            FileHelper = new FileHelper();
            FileHelper.Synchronize();

            _loggedInUserName = login;

            _pingThread = new Thread(PingWork);
            _pingThread.Start();

            return true;
        }

        static void Update()
        {
            DeviceConfiguration.Update();

            foreach (var device in DeviceConfiguration.Devices)
            {
                device.Driver = FiresecManager.Drivers.FirstOrDefault(x => x.Id == device.DriverId);
            }

            foreach (var deviceState in DeviceStates.DeviceStates)
            {
                deviceState.Device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == deviceState.Id);

                foreach (var state in deviceState.States)
                {
                    var driverState = deviceState.Device.Driver.States.FirstOrDefault(x => x.Code == state.Code);
                    state.DriverState = driverState;
                }

                foreach (var parentState in deviceState.ParentStates)
                {
                    parentState.ParentDevice = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == parentState.ParentDeviceId);
                    var driverState = parentState.ParentDevice.Driver.States.FirstOrDefault(x => x.Code == parentState.Code);
                    parentState.DriverState = driverState;
                }
            }
        }

        static string _loggedInUserName;
        public static User CurrentUser
        {
            get
            {
                return SecurityConfiguration.Users.FirstOrDefault(x => x.Name == _loggedInUserName);
            }
        }

        public static List<Perimission> CurrentPermissions
        {
            get
            {
                List<string> permissionIds = new List<string>();

                foreach (var groupId in CurrentUser.Groups)
                {
                    var group = SecurityConfiguration.UserGroups.FirstOrDefault(x => x.Id == groupId);

                    permissionIds.AddRange(group.Permissions);
                }
                permissionIds.AddRange(CurrentUser.Permissions);

                foreach (var permissionId in CurrentUser.RemovedPermissions)
                {
                    permissionIds.Remove(permissionId);
                }

                var permissions = new List<Perimission>(from permission in SecurityConfiguration.Perimissions
                                                        where permissionIds.Contains(permission.Id)
                                                        select permission);
                return permissions;
            }
        }

        public static void Disconnect()
        {
            _firesecService.Disconnect();
            _pingThread.Abort();
        }

        public static void SetConfiguration()
        {
            _firesecService.SetSystemConfiguration(FiresecManager.SystemConfiguration);
            return;
            _firesecService.SetDeviceConfiguration(DeviceConfiguration);
        }

        public static void AddToIgnoreList(List<string> deviceIds)
        {
            _firesecService.AddToIgnoreList(deviceIds);
        }

        public static void RemoveFromIgnoreList(List<string> deviceIds)
        {
            _firesecService.RemoveFromIgnoreList(deviceIds);
        }

        public static void ResetStates(List<ResetItem> resetItems)
        {
            _firesecService.ResetStates(resetItems);
        }

        public static void AddUserMessage(string message)
        {
            _firesecService.AddUserMessage(message);
        }

        public static void ExecuteCommand(string deviceId, string methodName)
        {
            _firesecService.ExecuteCommand(deviceId, methodName);
        }

        public static List<JournalRecord> ReadJournal(int startIndex, int count)
        {
            return _firesecService.ReadJournal(startIndex, count);
        }

        public static Dictionary<string, string> GetHashAndNameFiles(string directory)
        {
            return _firesecService.GetHashAndNameFiles(directory);
        }

        public static Stream GetFile(string filepath)
        {
            return _firesecService.GetFile(filepath);
        }

        public static void LoadFromFile(string fileName)
        {
            DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(DeviceConfiguration));
            FileStream fileStream = new FileStream(fileName, FileMode.Open);
            FiresecManager.DeviceConfiguration = (DeviceConfiguration)dataContractSerializer.ReadObject(fileStream);
            fileStream.Close();

            Update();
        }

        public static void SaveToFile(string fileName)
        {
            DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(DeviceConfiguration));
            FileStream fileStream = new FileStream(fileName, FileMode.Create);
            dataContractSerializer.WriteObject(fileStream, FiresecManager.DeviceConfiguration);
            fileStream.Close();
        }

        static void PingWork()
        {
            while (true)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                _firesecService.Ping();
                Trace.WriteLine("Ping " + pingCount.ToString());
                pingCount++;
            }
        }

        static int pingCount = 0;

        public static void Reconnect(string userName)
        {
            _loggedInUserName = userName;
        }
    }
}