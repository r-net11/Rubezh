using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.SKD;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public static Thread CurrentThread;

		#region Add
		public static void AddGKGlobalJournalItem(XJournalItem journalItem)
		{
			var globalJournalItem = new JournalItem();
			globalJournalItem.SystemDateTime = journalItem.SystemDateTime;
			globalJournalItem.DeviceDateTime = journalItem.DeviceDateTime;
			globalJournalItem.ObjectUID = journalItem.ObjectUID;
			globalJournalItem.ObjectName = journalItem.ObjectName;
			globalJournalItem.JournalEventNameType = journalItem.JournalEventNameType;
			globalJournalItem.NameText = journalItem.Name;
			globalJournalItem.DescriptionText = journalItem.Description;
			globalJournalItem.StateClass = journalItem.StateClass;
			AddGlobalJournalItem(globalJournalItem);
			System.Diagnostics.Trace.WriteLine("AddGlobalJournalItem");
		}

		public static void AddGlobalJournalItem(JournalItem journalItem)
		{
			DBHelper.Add(journalItem);
		}

		void AddSKDMessage(JournalEventNameType journalEventNameType, SKDDevice device, string userName)
		{
			AddSKDMessage(journalEventNameType, JournalEventDescriptionType.NULL, device, userName);
		}

		void AddSKDMessage(JournalEventNameType journalEventNameType, JournalEventDescriptionType description, SKDDevice device, string userName)
		{
			var journalItem = new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				DeviceDateTime = DateTime.Now,
				JournalEventNameType = journalEventNameType,
				JournalEventDescriptionType = description,
				ObjectName = device != null ? device.Name : null,
				ObjectUID = device != null ? device.UID : Guid.Empty,
				JournalObjectType = JournalObjectType.SKDDevice,
				StateClass = EventDescriptionAttributeHelper.ToStateClass(journalEventNameType),
				UserName = userName,
			};

			DBHelper.AddMessage(journalEventNameType, userName);
			FiresecService.NotifyNewJournalItems(new List<JournalItem>() { journalItem });
		}
		#endregion

		#region Get
		//public OperationResult<List<JournalRecord>> GetFilteredJournal(JournalFilter journalFilter)
		//{
		//    return FiresecDB.DatabaseHelper.GetFilteredJournal(journalFilter);
		//}

		public OperationResult<DateTime> GetMinDateTime()
		{
			using (var dataContext = new SqlConnection("Data Source=.\\SQLEXPRESS;Initial Catalog=SKD;Integrated Security=True;Language='English'"))
			{
				var query = "SELECT MIN(SystemDate) FROM Journal";
				var sqlCeCommand = new SqlCommand(query, dataContext);
				dataContext.Open();
				var reader = sqlCeCommand.ExecuteReader();
				var result = DateTime.Now;
				if (reader.Read())
				{
					if (!reader.IsDBNull(0))
					{
						result = reader.GetDateTime(0);
					}
				}
				dataContext.Close();
				return new OperationResult<DateTime>() { Result = result };
			}
		}

		public OperationResult<IEnumerable<JournalItem>> GetSKDJournalItems(JournalFilter filter)
		{
			var journalItems = DBHelper.GetFilteredJournalItems(filter);
			return new OperationResult<IEnumerable<JournalItem>>() { Result = journalItems };
			//return SKDDatabaseService.JournalItemTranslator.Get(filter);
		}

		public void BeginGetSKDFilteredArchive(ArchiveFilter archiveFilter, Guid archivePortionUID)
		{
			if (CurrentThread != null)
			{
				DBHelper.IsAbort = true;
				CurrentThread.Join(TimeSpan.FromMinutes(1));
				CurrentThread = null;
			}
			DBHelper.IsAbort = false;
			var thread = new Thread(new ThreadStart((new Action(() =>
			{
				DBHelper.ArchivePortionReady -= DatabaseHelper_ArchivePortionReady;
				DBHelper.ArchivePortionReady += DatabaseHelper_ArchivePortionReady;
				DBHelper.BeginGetSKDFilteredArchive(archiveFilter, archivePortionUID, false);

			}))));
			thread.Name = "FiresecService.GetFilteredArchive";
			thread.IsBackground = true;
			CurrentThread = thread;
			thread.Start();
		}

		void DatabaseHelper_ArchivePortionReady(List<JournalItem> journalItems, Guid archivePortionUID)
		{
			FiresecService.NotifySKDArchiveCompleted(journalItems, archivePortionUID);
		}

		public List<JournalEventDescriptionType> GetDistinctEventDescriptions()
		{
			return DBHelper.GetDistinctEventDescriptions();
		}

		public List<JournalEventNameType> GetDistinctEventNames()
		{
			return DBHelper.GetDistinctEventNames();
		}
		#endregion
	}
}