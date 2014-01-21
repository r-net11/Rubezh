using System;
using System.Linq;
using FiresecAPI.Models;
using FiresecAPI;
using FiresecClient;
using System.Collections.Generic;

namespace AssadProcessor
{
	public class Watcher
	{
		internal void Start()
		{
            FiresecManager.FiresecDriver.Watcher.DevicesStateChanged += new Action<List<DeviceState>>(OnDevicesStateChanged);
            FiresecManager.FiresecDriver.Watcher.ZonesStateChanged += new Action<List<ZoneState>>(OnZoneStateChanged);
            FiresecManager.FiresecDriver.Watcher.NewJournalRecords += new Action<List<JournalRecord>>(OnNewJournalRecords);
		}

        void OnDevicesStateChanged(List<DeviceState> deviceStates)
        {
            foreach (var deviceState in deviceStates)
            {
                var assadDevice = Configuration.Devices.FirstOrDefault(x => x.Id == deviceState.Device.PathId);
                if (assadDevice != null)
                {
                    assadDevice.FireEvent(null);
                }

                if (Configuration.Monitor != null)
                {
                    Configuration.Monitor.FireEvent(null);
                }
            }
        }

		void OnZoneStateChanged(List<ZoneState> zoneStates)
		{
            foreach (var zoneState in zoneStates)
            {
                var assadZone = Configuration.Zones.FirstOrDefault(x => x.ZoneNo == zoneState.Zone.No.ToString());
                if (assadZone != null)
                {
                    assadZone.FireEvent(null);
                }
            }
		}

        void OnNewJournalRecords(List<JournalRecord> journalRecords)
        {
            foreach (var journalRecord in journalRecords)
            {
                if (journalRecord.DeviceDatabaseId != null)
                {
                    SendEvent(journalRecord, journalRecord.DeviceDatabaseId);
                }
                if (journalRecord.PanelDatabaseId != null)
                {
                    SendEvent(journalRecord, journalRecord.PanelDatabaseId);
                }
            }
        }

		void SendEvent(JournalRecord journalRecord, string dataBaseId)
		{
			var device = FiresecManager.Devices.FirstOrDefault(x => x.DatabaseId == dataBaseId);
			if (device != null)
			{
				var assadDevice = Configuration.Devices.FirstOrDefault(x => x.Id == device.PathId);
				if (assadDevice != null)
				{
                    string eventName = journalRecord.StateType.ToDescription();
					assadDevice.FireEvent(eventName);
				}
			}
		}
	}
}