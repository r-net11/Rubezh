using System;
using System.Collections.Generic;
using FiresecAPI.Models;

namespace FiresecClient
{
    public static class ItvManager
    {
        public static List<Driver> Drivers
        {
            get { return FiresecManager.FiresecConfiguration.Drivers; }
        }
        public static DeviceConfiguration DeviceConfiguration
        {
			get { return FiresecManager.FiresecConfiguration.DeviceConfiguration; }
        }
		[Obsolete]
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
			var result = FiresecManager.Connect(ClientType.Itv, serverAddress, login, password);
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
            FiresecManager.FiresecDriver.AddToIgnoreList(deviceUIDs);
        }

        public static void RemoveFromIgnoreList(List<Guid> deviceUIDs)
        {
            FiresecManager.FiresecDriver.RemoveFromIgnoreList(deviceUIDs);
        }

        public static void ExecuteCommand(Guid deviceUID, string methodName)
        {
            FiresecManager.FiresecDriver.ExecuteCommand(deviceUID, methodName);
        }

		public static void SetZoneGuard(Zone zone)
		{
			FiresecManager.SetZoneGuard(zone);
		}

		public static void UnSetZoneGuard(Zone zone)
		{
			FiresecManager.UnSetZoneGuard(zone);
		}

        public static void ResetAllStates()
        {
        }
    }
}