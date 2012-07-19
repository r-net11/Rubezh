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
			FiresecCallbackService.DeviceStateChangedEvent += new Action<Guid>(OnDeviceStateChangedEvent);
			FiresecCallbackService.ZoneStateChangedEvent += new Action<ulong>(OnZoneStateChangedEvent);
			FiresecCallbackService.NewJournalRecordEvent += new Action<JournalRecord>(OnNewJournalItemEvent);
		}

		void OnDeviceStateChangedEvent(Guid deviceUID)
		{
			var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
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

		void OnZoneStateChangedEvent(ulong zoneNo)
		{
			var assadZone = Configuration.Zones.FirstOrDefault(x => x.ZoneNo == zoneNo.ToString());
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
			var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.DatabaseId == dataBaseId);
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