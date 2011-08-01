using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using FiresecAPI;
using FiresecAPI.Models;
using System.ServiceModel.Description;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;

namespace FiresecClient
{
    public class FiresecManager
    {
        public static DeviceConfiguration DeviceConfiguration { get; set; }
        public static DeviceConfigurationStates DeviceStates { get; set; }
        public static SystemConfiguration SystemConfiguration { get; set; }

        static IFiresecService _firesecService;

        public static bool Connect(string login, string password)
        {
            _firesecService = FiresecServiceFactory.Create();

            _firesecService.Connect();
            SystemConfiguration = _firesecService.GetSystemConfiguration();
            DeviceConfiguration = _firesecService.GetDeviceConfiguration();
            DeviceStates = _firesecService.GetStates();
            Update();

            _loggedInUserName = login;
            return true;
        }

        static void Update()
        {
            DeviceConfiguration.Update();
            DeviceConfiguration.UpdateDrivers();

            foreach (var deviceState in DeviceStates.DeviceStates)
            {
                deviceState.Device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == deviceState.Id);
            }
        }

        static string _loggedInUserName;
        public static User CurrentUser
        {
            get
            {
                return SystemConfiguration.Users.FirstOrDefault(x => x.Name == _loggedInUserName);
            }
        }

        public static List<Perimission> CurrentPermissions
        {
            get
            {
                List<string> permissionIds = new List<string>();

                foreach (var groupId in CurrentUser.Groups)
                {
                    var group = SystemConfiguration.UserGroups.FirstOrDefault(x => x.Id == groupId);

                    permissionIds.AddRange(group.Permissions);
                }
                permissionIds.AddRange(CurrentUser.Permissions);

                foreach (var permissionId in CurrentUser.RemovedPermissions)
                {
                    permissionIds.Remove(permissionId);
                }

                var permissions = new List<Perimission>(from permission in SystemConfiguration.Perimissions
                                                        where permissionIds.Contains(permission.Id)
                                                        select permission);
                return permissions;
            }
        }

        public static void Disconnect()
        {
            _firesecService.Disconnect();
        }

        public static void SetConfiguration()
        {
            _firesecService.SetSystemConfiguration(FiresecManager.SystemConfiguration);
            return;
            _firesecService.SetDeviceConfiguration(DeviceConfiguration);
        }

        public static void AddToIgnoreList(List<string> devicePaths)
        {
            _firesecService.AddToIgnoreList(devicePaths);
        }

        public static void RemoveFromIgnoreList(List<string> devicePaths)
        {
            _firesecService.RemoveFromIgnoreList(devicePaths);
        }

        public static void ResetStates(List<ResetItem> resetItems)
        {
            _firesecService.ResetStates(resetItems);
        }

        public static void AddUserMessage(string message)
        {
            _firesecService.AddUserMessage(message);
        }

        public static void ExecuteCommand(string devicePath, string methodName)
        {
            _firesecService.ExecuteCommand(devicePath, methodName);
        }

        public static List<JournalItem> ReadJournal(int startIndex, int count)
        {
            return _firesecService.ReadJournal(startIndex, count);
        }

        public static List<string> GetSoundsFileName()
        {
            return _firesecService.GetSoundsFileName();
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
    }
}
