using System;
using System.Collections.Generic;
using FiresecAPI.Models;
using Firesec;

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
			get { return FiresecManager.FiresecConfiguration.DeviceConfiguration; }
        }
        public static LibraryConfiguration LibraryConfiguration
        {
            get { return FiresecManager.LibraryConfiguration; }
        }

        public static string Connect(string serverAddress, string login="adm", string password="", string FS_Address="localhost", int FS_Port=211, string FS_Login="adm", string FS_Password="")
        {
			var result = FiresecManager.Connect(ClientType.Itv, serverAddress, login, password);
            if (string.IsNullOrEmpty(result))
            {
				SocketServerHelper.Stop();
				FiresecManager.GetConfiguration();
				FiresecManager.InitializeFiresecDriver(FS_Address, FS_Port, FS_Login, FS_Password);
				FiresecManager.Synchronyze();
				FiresecManager.StatrtWatcher(true);
                FiresecManager.StartPing();
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