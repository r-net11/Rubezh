using Common;
using StrazhAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Infrastructure.Common
{
	public static class ControllersInfoHelper
	{
		static string FileName = AppDataFolderHelper.GetControllersInfoFileName();

		static List<ControllerInfo> Infos { get; set; }

		static ControllersInfoHelper()
		{
			Load();
		}

		public static void SetControllerInfo(Guid deviceUid, int journalItemsCount, int alarmJournalItemsCount, string macAddress)
		{
			var info = Infos.FirstOrDefault(x => x.DeviceUid == deviceUid);
			if (info != null)
			{
				info.JournalItemsCount = journalItemsCount;
				info.AlarmJournlItemsCount = alarmJournalItemsCount;
				info.MacAddress = macAddress;
			}
			else
			{
				Infos.Add(new ControllerInfo
				{
					DeviceUid = deviceUid,
					JournalItemsCount = journalItemsCount,
					AlarmJournlItemsCount = alarmJournalItemsCount,
					MacAddress = macAddress
				});
			}
			Save();
		}

		public static void ChangeControllerInfo(Guid deviceUid, int journalItemsCount, int alarmJournalItemsCount)
		{
			var info = Infos.FirstOrDefault(x => x.DeviceUid == deviceUid);
			if (info != null)
			{
				info.JournalItemsCount = journalItemsCount;
				info.AlarmJournlItemsCount = alarmJournalItemsCount;
				Save();
			}
		}

		public static string GetControllerMacAddress(Guid deviceUid)
		{
			var info = Infos.FirstOrDefault(x => x.DeviceUid == deviceUid);
			return info != null ? info.MacAddress : null;
		}

		public static int GetJournalItemsCount(Guid deviceUid)
		{
			var info = Infos.FirstOrDefault(x => x.DeviceUid == deviceUid);
			return info != null ? info.JournalItemsCount : 0;
		}

		public static int GetAlarmJournaItemsCount(Guid deviceUid)
		{
			var info = Infos.FirstOrDefault(x => x.DeviceUid == deviceUid);
			return info != null ? info.AlarmJournlItemsCount : 0;
		}

		static void Load()
		{
			try
			{

				Infos = new List<ControllerInfo>();
				if (File.Exists(FileName))
				{
					using (var fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					{
						var xmlSerializer = XmlSerializer.FromTypes(new[] { typeof(List<ControllerInfo>) })[0];
						Infos = (List<ControllerInfo>)xmlSerializer.Deserialize(fileStream);
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e);
			}
		}

		static void Save()
		{
			try
			{
				var xmlSerializer = XmlSerializer.FromTypes(new[] { typeof(List<ControllerInfo>) })[0];
				using (var fileStream = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
				{
					xmlSerializer.Serialize(fileStream, Infos);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e);
			}
		}
	}
}