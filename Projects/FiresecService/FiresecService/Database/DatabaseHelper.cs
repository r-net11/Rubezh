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
		public static void AddJournalRecord(JournalRecord journalRecord)
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
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e);
			}
		}

		public static int GetLastOldId()
		{
			try
			{
				using (var dataContext = ConnectionManager.CreateFiresecDataContext())
				{
					var query =
					"SELECT MIN(OldId) FROM Journal";

					var result = dataContext.ExecuteQuery<JournalRecord>(query);
					if (result.Count() > 0)
					{
						return result.First().OldId;
					}
					return 0;
				}
			}
			catch (Exception e)
			{
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
			CallbackManager.OnNewJournalRecord(journalRecord);
		}
	}
}