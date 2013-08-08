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
			return ServerFS2Database.GetFilteredJournal(journalFilter);
		}

		public static List<FS2JournalItem> GetFilteredArchive(ArchiveFilter archiveFilter)
		{
			return ServerFS2Database.OnGetFilteredArchive(archiveFilter, true);
		}
		public static void BeginGetFilteredArchive(ArchiveFilter archiveFilter)
		{
			if (CurrentThread != null)
			{
				ServerFS2Database.IsAbort = true;
				CurrentThread.Join(TimeSpan.FromMinutes(1));
				CurrentThread = null;
			}
			ServerFS2Database.IsAbort = false;
			var thread = new Thread(new ThreadStart((new Action(() =>
			{
				ServerFS2Database.ArchivePortionReady -= DatabaseHelper_ArchivePortionReady;
				ServerFS2Database.ArchivePortionReady += DatabaseHelper_ArchivePortionReady;
				try
				{
					ServerFS2Database.OnGetFilteredArchive(archiveFilter, false);
				}
				catch { }
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
			return ServerFS2Database.GetDistinctDescriptions();
		}

		public static DateTime GetArchiveStartDate()
		{
			return ServerFS2Database.GetArchiveStartDate();
		}

		public static void AddJournalItems(List<FS2JournalItem> journalItems)
		{
			var operationResult = new OperationResult<bool>();
			try
			{
				ServerFS2Database.AddJournalItems(journalItems);
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

			ServerFS2Database.AddJournalItems(new List<FS2JournalItem>() { journalItem });
			//NotifyNewJournal(new List<FS2JournalItem>() { journalItem });
		}
	}
}