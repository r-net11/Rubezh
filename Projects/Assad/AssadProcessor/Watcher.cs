using System;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;

namespace AssadProcessor
{
	public class Watcher
	{
		internal void Start()
		{
            FiresecManager.DeviceStateChangedEvent += new Action<DeviceState>(OnDeviceStateChangedEvent);
            FiresecManager.ZoneStateChangedEvent += new Action<ZoneState>(OnZoneStateChangedEvent);
            FiresecManager.NewJournalRecordEvent += new Action<JournalRecord>(OnNewJournalItemEvent);
		}

        void OnDeviceStateChangedEvent(DeviceState deviceState)
		{
            var device = deviceState.Device;
			var assadDevice = Configuration.Devices.FirstOrDefault(x => x.Id == device.PathId);
			if (assadDevice != null)
			{
				assadDevice.FireEvent(null);
			}

			if (Configuration.Monitor != null)
			{
				Configuration.Monitor.FireEvent(null);
			}
		}

		void OnZoneStateChangedEvent(ZoneState zoneState)
		{
            var assadZone = Configuration.Zones.FirstOrDefault(x => x.ZoneNo == zoneState.Zone.No.ToString());
			if (assadZone != null)
			{
				assadZone.FireEvent(null);
			}
		}

		void OnNewJournalItemEvent(JournalRecord journalRecord)
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

		void SendEvent(JournalRecord journalRecord, string dataBaseId)
		{
			var device = FiresecManager.Devices.FirstOrDefault(x => x.DatabaseId == dataBaseId);
			if (device != null)
			{
				var assadDevice = Configuration.Devices.FirstOrDefault(x => x.Id == device.PathId);
				if (assadDevice != null)
				{
					string eventName = EnumsConverter.StateTypeToClassName(journalRecord.StateType);
					assadDevice.FireEvent(eventName);
				}
			}
		}
	}
}