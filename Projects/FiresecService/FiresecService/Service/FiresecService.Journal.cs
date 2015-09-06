using System;
using System.Collections.Generic;
using System.Threading;
using FiresecAPI;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using SKDDriver.DataClasses;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public static Thread CurrentThread;

		#region Add
		void AddJournalMessage(JournalEventNameType journalEventNameType, string objectName, JournalEventDescriptionType journalEventDescriptionType = JournalEventDescriptionType.NULL, string userName = null, Guid? uid = null)
		{
			var journalItem = new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				JournalEventNameType = journalEventNameType,
				JournalEventDescriptionType = journalEventDescriptionType,
				JournalSubsystemType = EventDescriptionAttributeHelper.ToSubsystem(journalEventNameType),
				JournalObjectType = JournalObjectType.None,
				ObjectUID = uid != null ? uid.Value : Guid.Empty,
				ObjectName = objectName,
			};
			if (userName != null)
			{
				journalItem.UserName = userName;
			}
			else
			{
				journalItem.UserName = UserName;
			}

			AddCommonJournalItems(new List<JournalItem>() { journalItem });
		}

		public static void AddCommonJournalItems(List<JournalItem> journalItems)
		{
			using (var dbService = new SKDDriver.DataClasses.DbService())
			{
				foreach (var journalItem in journalItems)
				{
					dbService.JournalTranslator.Add(journalItem);
				}
			}
			FiresecService.NotifyNewJournalItems(journalItems);
			foreach (var journalItem in journalItems)
			{
				ProcedureRunner.RunOnJournal(journalItem);
			}
		}

		public OperationResult<bool> AddJournalItem(JournalItem journalItem)
		{
			try
			{
				journalItem.UserName = UserName;
				journalItem.JournalSubsystemType = EventDescriptionAttributeHelper.ToSubsystem(journalItem.JournalEventNameType);
				AddCommonJournalItems(new List<JournalItem>() { journalItem });
			}
			catch (Exception e)
			{
				return OperationResult<bool>.FromError(e.Message);
			}
			return new OperationResult<bool>(true);
		}
		#endregion

		#region Get
		public OperationResult<DateTime> GetMinJournalDateTime()
		{
			using (var dbService = new DbService())
			{
				return dbService.JournalTranslator.GetMinDate();
			}
		}

		public OperationResult<List<JournalItem>> GetFilteredJournalItems(JournalFilter filter)
		{
			using (var dbService = new SKDDriver.DataClasses.DbService())
			{
				return dbService.JournalTranslator.GetFilteredJournalItems(filter);
			}
		}

		public OperationResult BeginGetFilteredArchive(ArchiveFilter archiveFilter, Guid archivePortionUID)
		{
			try
			{
				if (CurrentThread != null)
				{
					DbService.IsAbort = true;
					CurrentThread.Join(TimeSpan.FromMinutes(1));
					CurrentThread = null;
				}
				DbService.IsAbort = false;
				var thread = new Thread(new ThreadStart((new Action(() =>
				{
					using (var dbService = new SKDDriver.DataClasses.DbService())
					{
						dbService.JournalTranslator.ArchivePortionReady -= DatabaseHelper_ArchivePortionReady;
						dbService.JournalTranslator.ArchivePortionReady += DatabaseHelper_ArchivePortionReady;
						dbService.JournalTranslator.BeginGetFilteredArchive(archiveFilter, archivePortionUID);
					}
				}))));
				thread.Name = "FiresecService.GetFilteredArchive";
				thread.IsBackground = true;
				CurrentThread = thread;
				thread.Start();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		void DatabaseHelper_ArchivePortionReady(List<JournalItem> journalItems, Guid archivePortionUID)
		{
			FiresecService.NotifyArchiveCompleted(journalItems, archivePortionUID);
		}

		public OperationResult<List<JournalItem>> GetArchivePage(ArchiveFilter filter, int page)
		{
			using(var dbService = new SKDDriver.DataClasses.DbService())
			{
				return dbService.JournalTranslator.GetArchivePage(filter, page);
			}
		}

		public OperationResult<int> GetArchiveCount(ArchiveFilter filter)
		{
			using (var dbService = new SKDDriver.DataClasses.DbService())
			{
				return dbService.JournalTranslator.GetArchiveCount(filter);
			}
		}
		#endregion
	}
}