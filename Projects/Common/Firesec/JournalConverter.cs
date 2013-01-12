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
				SubsystemType = EnumsConverter.StringToSubsystemType(innerJournalRecord.IDSubSystem),
				StateType = (StateType)int.Parse(innerJournalRecord.IDTypeEvents),
			};
			SetDeviceCatogory(journalRecord);

			return journalRecord;
		}

		public static void SetDeviceCatogory(JournalRecord journalRecord)
		{
			Device device = null;
			if (ConfigurationCash.DeviceConfiguration.Devices != null)
			{
				if (string.IsNullOrWhiteSpace(journalRecord.DeviceDatabaseId) == false)
				{
					device = ConfigurationCash.DeviceConfiguration.Devices.FirstOrDefault(
						 x => x.DatabaseId == journalRecord.DeviceDatabaseId);
				}
				else
				{
					device = ConfigurationCash.DeviceConfiguration.Devices.FirstOrDefault(
						   x => x.DatabaseId == journalRecord.PanelDatabaseId);
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