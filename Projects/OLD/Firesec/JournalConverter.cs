using System;
using System.Linq;
using Common;
using Firesec.Models.Journals;
using FiresecAPI;
using FiresecAPI.Models;

namespace Firesec
{
	public static class JournalConverter
	{
		public static JournalRecord Convert(journalType innerJournalRecord)
		{
			var journalRecord = new JournalRecord()
			{
				OldId = int.Parse(innerJournalRecord.IDEvents),
				DeviceTime = ConvertTime(innerJournalRecord.Dt),
				SystemTime = ConvertTime(innerJournalRecord.SysDt),
				ZoneName = innerJournalRecord.ZoneName,
				Description = innerJournalRecord.EventDesc,
				DeviceName = innerJournalRecord.CLC_Device,
				PanelName = innerJournalRecord.CLC_DeviceSource,
				DeviceDatabaseId = innerJournalRecord.IDDevices,
				PanelDatabaseId = innerJournalRecord.IDDevicesSource,
				User = innerJournalRecord.UserInfo,
				Detalization = innerJournalRecord.CLC_Detalization,
				SubsystemType = EnumsConverter.StringToFS1SubsystemType(innerJournalRecord.IDSubSystem),
				StateType = (StateType)int.Parse(innerJournalRecord.IDTypeEvents),
			};

			SetDeviceCatogoryAndDevieUID(journalRecord);

			return journalRecord;
		}

		public static void SetDeviceCatogoryAndDevieUID(JournalRecord journalRecord)
		{
			Guid deviceDatabaseUID = Guid.Empty;
			Guid.TryParse(journalRecord.DeviceDatabaseId, out deviceDatabaseUID);
			journalRecord.DeviceDatabaseUID = deviceDatabaseUID;

			Guid panelDatabaseUID = Guid.Empty;
			Guid.TryParse(journalRecord.PanelDatabaseId, out panelDatabaseUID);
			journalRecord.PanelDatabaseUID = panelDatabaseUID;

			Device device = null;
			if (ConfigurationCash.DeviceConfiguration.Devices != null)
			{
				if (journalRecord.DeviceDatabaseUID != Guid.Empty)
				{
					device = ConfigurationCash.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == journalRecord.DeviceDatabaseUID);
				}
				else
				{
					device = ConfigurationCash.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == journalRecord.PanelDatabaseUID);
				}
			}
			if (device != null)
			{
				journalRecord.DeviceCategory = (int)device.Driver.Category;
			}
			else
			{
				journalRecord.DeviceCategory = (int)DeviceCategoryType.None;
			}
		}

		public static DateTime ConvertTime(string firesecTime)
		{
			if (string.IsNullOrEmpty(firesecTime) || firesecTime.Length < 18)
				return DateTime.MinValue;

			if (firesecTime.Length < 18)
			{
				Logger.Error("JournalConverter.ConvertTime firesecTime = " + firesecTime);
				return DateTime.MinValue;
			}

			return new DateTime(
				int.Parse(firesecTime.Substring(0, 4)),
				int.Parse(firesecTime.Substring(4, 2)),
				int.Parse(firesecTime.Substring(6, 2)),
				int.Parse(firesecTime.Substring(9, 2)),
				int.Parse(firesecTime.Substring(12, 2)),
				int.Parse(firesecTime.Substring(15, 2))
			);
		}
	}
}