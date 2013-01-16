using System;
using System.Collections.Generic;
using System.Linq;
using Firesec.Imitator;
using FiresecAPI;
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

        public static string Connect(string serverAddress, string login="adm", string password="", string FS_Address="localhost")
        {
			var result = FiresecManager.Connect(ClientType.Itv, serverAddress, login, password);
            if (string.IsNullOrEmpty(result))
            {
                try
                {
					FiresecManager.GetConfiguration("ITV/Configuration");
                    var initializeFiresecDriverResult = FiresecManager.InitializeFiresecDriver(true);
                    if (initializeFiresecDriverResult.HasError)
                    {
                        return initializeFiresecDriverResult.Error;
                    }
                    FiresecManager.FiresecDriver.Synchronyze();
					FiresecManager.FiresecDriver.StartWatcher(true, true);
                    FiresecManager.FiresecDriver.Watcher.DevicesStateChanged += new Action<List<DeviceState>>(Watcher_DevicesStateChanged);
                    FiresecManager.FiresecDriver.Watcher.ZonesStateChanged += new Action<List<ZoneState>>(Watcher_ZonesStateChanged);
                    FiresecManager.FiresecDriver.Watcher.NewJournalRecords += new Action<List<JournalRecord>>(Watcher_NewJournalRecords);
					FiresecManager.FiresecDriver.StartWatcher(true, true);
					FiresecManager.FSAgent.Start();
                }
                catch (FiresecException e)
                {
                    return e.Message;
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
            return result;
        }

        public static void Disconnect()
        {
            FiresecManager.Disconnect();
        }

        public static void ExecuteCommand(Guid deviceUID, string methodName)
        {
            var device = FiresecManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
            if (device != null)
            {
                FiresecManager.FiresecDriver.ExecuteCommand(device, methodName);
            }
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