using System;
using System.Collections.Generic;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using System.Threading;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
	    public static Thread CurrentThread;
		public OperationResult<int> GetJournalLastId()
		{
			return new OperationResult<int>()
				{
                    Result = FiresecDB.DatabaseHelper.GetLastOldId()
				};
		}

		public OperationResult<List<JournalRecord>> GetFilteredJournal(JournalFilter journalFilter)
		{
			return FiresecDB.DatabaseHelper.GetFilteredJournal(journalFilter);
		}

		public OperationResult<List<JournalRecord>> GetFilteredArchive(ArchiveFilter archiveFilter)
		{
            
            return FiresecDB.DatabaseHelper.OnGetFilteredArchive(archiveFilter,true);
		}
		public void BeginGetFilteredArchive(ArchiveFilter archiveFilter)
		{
            if (CurrentThread != null)
            {
                FiresecDB.DatabaseHelper.IsAbort = true;
                CurrentThread.Join();
                CurrentThread = null;
            }
            FiresecDB.DatabaseHelper.IsAbort = false;
            var thread = new Thread(new ThreadStart((new Action(() =>
            {
                FiresecDB.DatabaseHelper.ArchivePortionReady -= DatabaseHelper_ArchivePortionReady;
                FiresecDB.DatabaseHelper.ArchivePortionReady += DatabaseHelper_ArchivePortionReady;
                FiresecDB.DatabaseHelper.OnGetFilteredArchive(archiveFilter, false);
            }))));
            CurrentThread = thread;
            thread.Start();
        }

        void DatabaseHelper_ArchivePortionReady(List<JournalRecord> obj)
        {
            FiresecService.NotifyArchivePortionCompleted(obj);
        }

		public OperationResult<List<JournalDescriptionItem>> GetDistinctDescriptions()
		{
		    return FiresecDB.DatabaseHelper.GetDistinctDescriptions();
		}

		public OperationResult<DateTime> GetArchiveStartDate()
		{
		    return FiresecDB.DatabaseHelper.GetArchiveStartDate();
		}

		public void AddJournalRecords(List<JournalRecord> journalRecords)
		{
			var operationResult = new OperationResult<bool>();
			try
			{
				FiresecDB.DatabaseHelper.AddJournalRecords(journalRecords);
				operationResult.Result = true;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecService.AddJournalRecords");
				operationResult.HasError = true;
				operationResult.Error = e.Message.ToString();
			}
		}

		public void SetJournal(List<JournalRecord> journalRecords)
		{
            FiresecDB.DatabaseHelper.SetJournal(journalRecords);
		}

		void AddInfoMessage(string userName, string mesage)
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

			FiresecDB.DatabaseHelper.AddJournalRecord(journalRecord);
			NotifyNewJournal(new List<JournalRecord>() { journalRecord });
		}
	}
}