using System;
using System.Collections.Generic;
using System.Threading;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using SKDDriver;
using FiresecAPI.SKD;
using FiresecAPI.Events;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public static Thread CurrentThread;
		//public OperationResult<int> GetJournalLastId()
		//{
		//    return new OperationResult<int>()
		//        {
		//            Result = FiresecDB.DatabaseHelper.GetLastOldId()
		//        };
		//}

		//public OperationResult<List<JournalRecord>> GetFilteredJournal(JournalFilter journalFilter)
		//{
		//    return FiresecDB.DatabaseHelper.GetFilteredJournal(journalFilter);
		//}

		//public OperationResult<List<JournalRecord>> GetFilteredArchive(ArchiveFilter archiveFilter)
		//{
		//    return FiresecDB.DatabaseHelper.OnGetFilteredArchive(archiveFilter, true);
		//}
		//public void BeginGetFilteredArchive(ArchiveFilter archiveFilter)
		//{
		//    if (CurrentThread != null)
		//    {
		//        FiresecDB.DatabaseHelper.IsAbort = true;
		//        CurrentThread.Join(TimeSpan.FromMinutes(1));
		//        CurrentThread = null;
		//    }
		//    FiresecDB.DatabaseHelper.IsAbort = false;
		//    var thread = new Thread(new ThreadStart((new Action(() =>
		//    {
		//        FiresecDB.DatabaseHelper.ArchivePortionReady -= DatabaseHelper_ArchivePortionReady;
		//        FiresecDB.DatabaseHelper.ArchivePortionReady += DatabaseHelper_ArchivePortionReady;
		//        FiresecDB.DatabaseHelper.OnGetFilteredArchive(archiveFilter, false);
		//    }))));
		//    thread.Name = "FS1 GetFilteredArchive";
		//    CurrentThread = thread;
		//    thread.Start();
		//}

		//void DatabaseHelper_ArchivePortionReady(List<JournalRecord> journalRecords)
		//{
		//    FiresecService.NotifyArchivePortionCompleted(journalRecords);
		//}

		public static void AddGKGlobalJournalItem(FiresecAPI.GK.JournalItem journalItem)
		{
			var globalJournalItem = new FiresecAPI.SKD.JournalItem();
			globalJournalItem.SystemDateTime = journalItem.SystemDateTime;
			globalJournalItem.DeviceDateTime = journalItem.DeviceDateTime;
			globalJournalItem.ObjectUID = journalItem.ObjectUID;
			globalJournalItem.ObjectName = journalItem.ObjectName;
			globalJournalItem.Name = journalItem.GlobalEventNameType;
			globalJournalItem.NameText = journalItem.Name;
			globalJournalItem.DescriptionText = journalItem.Description;
			AddGlobalJournalItem(globalJournalItem);
			System.Diagnostics.Trace.WriteLine("AddGlobalJournalItem");
		}

		public static void AddGlobalJournalItem(JournalItem journalItem)
		{
			int xxx = (int)journalItem.Name;
			SKDDBHelper.Add(journalItem);
		}

		void AddSKDMessage(GlobalEventNameEnum globalEventNameEnum, SKDDevice device, string userName)
		{
			AddSKDMessage(globalEventNameEnum, FiresecAPI.GK.EventDescription.Нет, device, userName);
		}

		void AddSKDMessage(GlobalEventNameEnum globalEventNameEnum, FiresecAPI.GK.EventDescription description, SKDDevice device, string userName)
		{
			var journalItem = new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				DeviceDateTime = DateTime.Now,
				Name = globalEventNameEnum,
				Description = description,
				ObjectName = device != null ? device.Name : null,
				ObjectUID = device != null ? device.UID : Guid.Empty,
				ObjectType = ObjectType.Устройство_СКД,
				UserName = userName,
			};

			SKDDBHelper.AddMessage(globalEventNameEnum, userName);
			FiresecService.NotifyNewJournalItems(new List<JournalItem>() { journalItem });
		}

		public void BeginGetSKDFilteredArchive(SKDArchiveFilter archiveFilter, Guid archivePortionUID)
		{
			if (CurrentThread != null)
			{
				SKDDBHelper.IsAbort = true;
				CurrentThread.Join(TimeSpan.FromMinutes(1));
				CurrentThread = null;
			}
			SKDDBHelper.IsAbort = false;
			var thread = new Thread(new ThreadStart((new Action(() =>
			{
				SKDDBHelper.ArchivePortionReady -= DatabaseHelper_ArchivePortionReady;
				SKDDBHelper.ArchivePortionReady += DatabaseHelper_ArchivePortionReady;
				SKDDBHelper.BeginGetSKDFilteredArchive(archiveFilter, archivePortionUID, false);

			}))));
			thread.Name = "SKD GetFilteredArchive";
			thread.IsBackground = true;
			CurrentThread = thread;
			thread.Start();
		}

		void DatabaseHelper_ArchivePortionReady(List<JournalItem> journalItems, Guid archivePortionUID)
		{
			FiresecService.NotifySKDArchiveCompleted(journalItems, archivePortionUID);
		}
	}
}