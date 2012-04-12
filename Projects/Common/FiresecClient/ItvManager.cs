using System.Collections.Generic;
using FiresecAPI.Models;
using System;

namespace FiresecClient
{
    public static class ItvManager
    {
        public static List<Driver> Drivers
        {
            get { return FiresecManager.Drivers; }
        }
        public static DeviceConfiguration DeviceConfiguration
        {
            get { return FiresecManager.DeviceConfiguration; }
        }
        public static DeviceConfigurationStates DeviceStates
        {
            get { return FiresecManager.DeviceStates; }
        }
        public static LibraryConfiguration LibraryConfiguration
        {
            get { return FiresecManager.LibraryConfiguration; }
        }

        public static string Connect(string serverAddress, string login, string password)
        {
            var result = FiresecManager.Connect("ITV", serverAddress, login, password);
            if (string.IsNullOrEmpty(result))
            {
                FiresecManager.GetConfiguration(true);
                FiresecManager.GetStates();
            }
            return result;
        }

        public static void Disconnect()
        {
            FiresecManager.Disconnect();
        }

        public static void AddToIgnoreList(List<Guid> deviceUIDs)
        {
            FiresecManager.FiresecService.AddToIgnoreList(deviceUIDs);
        }

        public static void RemoveFromIgnoreList(List<Guid> deviceUIDs)
        {
            FiresecManager.FiresecService.RemoveFromIgnoreList(deviceUIDs);
        }

        public static void ExecuteCommand(Guid deviceUID, string methodName)
        {
            FiresecManager.FiresecService.ExecuteCommand(deviceUID, methodName);
        }

        public static void ResetAllStates()
        {
        }
    }
}