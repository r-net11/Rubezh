using System;
using System.Collections.Generic;
using System.Threading;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FS2Api;
using ServerFS2.Service;

namespace ServerFS2.Journal
{
	public class ServerJournalHelper
	{
		public static Thread CurrentThread;

		public static List<FS2JournalItem> GetFilteredJournal(JournalFilter journalFilter)
		{
			return DatabaseHelper.GetFilteredJournal(journalFilter);
		}

		public static List<FS2JournalItem> GetFilteredArchive(ArchiveFilter archiveFilter)
		{
			return DatabaseHelper.OnGetFilteredArchive(archiveFilter, true);
		}
		public static void BeginGetFilteredArchive(ArchiveFilter archiveFilter)
		{
			if (CurrentThread != null)
			{
				DatabaseHelper.IsAbort = true;
				CurrentThread.Join(TimeSpan.FromMinutes(1));
				CurrentThread = null;
			}
			DatabaseHelper.IsAbort = false;
			var thread = new Thread(new ThreadStart((new Action(() =>
			{
				DatabaseHelper.ArchivePortionReady -= DatabaseHelper_ArchivePortionReady;
				DatabaseHelper.ArchivePortionReady += DatabaseHelper_ArchivePortionReady;
				DatabaseHelper.OnGetFilteredArchive(archiveFilter, false);
			}))));
			CurrentThread = thread;
			thread.Start();
		}

		static void DatabaseHelper_ArchivePortionReady(List<FS2JournalItem> journalItems)
		{
			var fs2Callbac = new FS2Callbac()
			{
				ArchiveJournalItems = journalItems
			};
			CallbackManager.Add(fs2Callbac);
		}

		public static List<JournalDescriptionItem> GetDistinctDescriptions()
		{
			return DatabaseHelper.GetDistinctDescriptions();
		}

		public static DateTime GetArchiveStartDate()
		{
			return DatabaseHelper.GetArchiveStartDate();
		}

		public static void AddJournalItems(List<FS2JournalItem> journalItems)
		{
			var operationResult = new OperationResult<bool>();
			try
			{
				DatabaseHelper.AddJournalItems(journalItems);
				operationResult.Result = true;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecService.AddJournalItems");
				operationResult.HasError = true;
				operationResult.Error = e.Message.ToString();
			}
		}

		static void AddInfoMessage(string userName, string mesage)
		{
			var journalItem = new FS2JournalItem()
			{
				DeviceTime = DateTime.Now,
				SystemTime = DateTime.Now,
				StateType = StateType.Info,
				Description = mesage,
				UserName = userName,
			};

			DatabaseHelper.AddJournalItems(new List<FS2JournalItem>() { journalItem });
			//NotifyNewJournal(new List<FS2JournalItem>() { journalItem });
		}
	}
}