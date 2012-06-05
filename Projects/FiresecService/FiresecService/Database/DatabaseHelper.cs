using System;
using System.Linq;
using FiresecAPI.Models;
using FiresecService.Service;
using Common;
using FiresecService.Processor;

namespace FiresecService.Database
{
	public static class DatabaseHelper
	{
		public static bool AddJournalRecord(JournalRecord journalRecord)
		{
			try
			{
				using (var dataContext = ConnectionManager.CreateFiresecDataContext())
				{
					var query =
					"SELECT * FROM Journal WHERE " +
					"\n SystemTime = '" + journalRecord.SystemTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
					"\n AND OldId = " + journalRecord.OldId.ToString();

					var result = dataContext.ExecuteQuery<JournalRecord>(query);

					if (result.Count() == 0)
					{
						dataContext.JournalRecords.InsertOnSubmit(journalRecord);
						dataContext.SubmitChanges();
						return true;
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e);
			}
			return false;
		}

		public static int GetLastOldId()
		{
			try
			{
				using (var dataContext = ConnectionManager.CreateFiresecDataContext())
				{
					var query = "SELECT MAX(OldId) FROM Journal";
					var result = dataContext.ExecuteQuery<int>(query);
					return result.FirstOrDefault();
				}
			}
			catch (Exception e)
			{
				Logger.Info("Обработано исключение при вызове метода DatabaseHelper.GetLastOldId");
				Logger.Error(e);
				return -1;
			}
		}

		public static void AddInfoMessage(string userName, string mesage)
		{
			var journalRecord = new JournalRecord()
			{
				DeviceTime = DateTime.Now,
				SystemTime = DateTime.Now,
				StateType = StateType.Info,
				Description = mesage,
				User = userName,
				DeviceDatabaseId = "",
				DeviceName = "",
				PanelDatabaseId = "",
				PanelName = "",
				ZoneName = ""
			};

			AddJournalRecord(journalRecord);
			ClientsCash.OnNewJournalRecord(journalRecord);
		}
	}
}