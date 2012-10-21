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
        public static DeviceLibraryConfiguration DeviceLibraryConfiguration
        {
            get { return FiresecManager.DeviceLibraryConfiguration; }
        }

        public static string Connect(string serverAddress, string login="adm", string password="", string FS_Address="localhost", int FS_Port=211, string FS_Login="adm", string FS_Password="")
        {
			var result = FiresecManager.Connect(ClientType.Itv, serverAddress, login, password);
            if (string.IsNullOrEmpty(result))
            {
				FiresecManager.GetConfiguration();
				FiresecManager.InitializeFiresecDriver(FS_Address, FS_Port, FS_Login, FS_Password, true);
                FiresecManager.FiresecDriver.Synchronyze();
				FiresecManager.FiresecDriver.StartWatcher(true, true);
                FiresecManager.FiresecDriver.Watcher.DevicesStateChanged += new Action<List<DeviceState>>(Watcher_DevicesStateChanged);
                FiresecManager.FiresecDriver.Watcher.ZonesStateChanged += new Action<List<ZoneState>>(Watcher_ZonesStateChanged);
                FiresecManager.FiresecDriver.Watcher.NewJournalRecords += new Action<List<JournalRecord>>(Watcher_NewJournalRecords);
            }
            return result;
        }

        public static void Disconnect()
        {
            FiresecManager.Disconnect();
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

        public static void AddToIgnoreList(List<Device> devices)
        {
            FiresecManager.FiresecDriver.AddToIgnoreList(devices);
        }

        public static void RemoveFromIgnoreList(List<Device> devices)
        {
            FiresecManager.FiresecDriver.RemoveFromIgnoreList(devices);
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
        static void Watcher_NewJournalRecords(List<JournalRecord> journalRecords)
        {
            foreach (var journalRecord in journalRecords)
            {
                if (NewJournalRecord != null)
                    NewJournalRecord(journalRecord);
            }
        }
        public static event Action<DeviceState> DeviceStateChanged;
        public static event Action<ZoneState> ZoneStateChanged;
        public static event Action<JournalRecord> NewJournalRecord;
    }
}