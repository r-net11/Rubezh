using System;
using System.Linq;
using Firesec.Models.Journals;
using FiresecAPI;
using FiresecAPI.Models;
using Common;

namespace FSAgentServer
{
	public static class JournalConverter
	{
		public static JournalRecord Convert(journalType innerJournalRecord)
		{
			NormalizeJournalType(innerJournalRecord);
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

			return journalRecord;
		}

		static void NormalizeJournalType(journalType innerJournalRecord)
		{
				innerJournalRecord.IDEvents = NormalizeString(innerJournalRecord.IDEvents);
				innerJournalRecord.Dt = NormalizeString(innerJournalRecord.Dt);
				innerJournalRecord.SysDt = NormalizeString(innerJournalRecord.SysDt);
				innerJournalRecord.ZoneName = NormalizeString(innerJournalRecord.ZoneName);
				innerJournalRecord.EventDesc = NormalizeString(innerJournalRecord.EventDesc);
				innerJournalRecord.CLC_Device = NormalizeString(innerJournalRecord.CLC_Device);
				innerJournalRecord.CLC_DeviceSource = NormalizeString(innerJournalRecord.CLC_DeviceSource);
				innerJournalRecord.IDDevices = NormalizeString(innerJournalRecord.IDDevices);
				innerJournalRecord.IDDevicesSource = NormalizeString(innerJournalRecord.IDDevicesSource);
				innerJournalRecord.UserInfo = NormalizeString(innerJournalRecord.UserInfo);
				innerJournalRecord.CLC_Detalization = NormalizeString(innerJournalRecord.CLC_Detalization);
				innerJournalRecord.IDSubSystem = NormalizeString(innerJournalRecord.IDSubSystem);
				innerJournalRecord.IDTypeEvents = NormalizeString(innerJournalRecord.IDTypeEvents);
		}

		static string NormalizeString(string value)
		{
			if (value != null)
			{
				value = value.Replace("\n", "");
				value = value.Replace("\r", "");
				value = value.TrimStart(' ');
				value = value.TrimEnd(' ');
			}
			return value;
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

			firesecTime = firesecTime.Replace("\n    ", "");
			firesecTime = firesecTime.Replace("\n", "");
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