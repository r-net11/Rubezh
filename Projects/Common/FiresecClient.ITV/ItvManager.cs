using System;
using System.Collections.Generic;
using Firesec;
using Firesec.Imitator;
using FiresecAPI.Models;

namespace FiresecClient.Itv
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
				FiresecManager.StartWatcher(true, false);
                FiresecManager.FiresecDriver.Watcher.DevicesStateChanged += new Action<List<DeviceState>>(Watcher_DevicesStateChanged);
                FiresecManager.FiresecDriver.Watcher.ZonesStateChanged += new Action<List<ZoneState>>(Watcher_ZonesStateChanged);
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
            FiresecManager.ResetAllStates();
        }

        public static void ShowImitator()
        {
            ImitatorService.Show();
        }

        static void Watcher_DevicesStateChanged(List<DeviceState> deviceStates)
        {
            foreach (var deviceState in deviceStates)
            {
                if (DeviceStateChanged != null)
                    DeviceStateChanged(deviceState);
            }
        }
        static void Watcher_ZonesStateChanged(List<ZoneState> zoneStates)
        {
            foreach (var zoneState in zoneStates)
            {
                if (ZoneStateChanged != null)
                    ZoneStateChanged(zoneState);
            }
        }
        public static event Action<DeviceState> DeviceStateChanged;
        public static event Action<ZoneState> ZoneStateChanged;
    }
}