using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common;
using FiresecAPI;
using FiresecAPI.Models;

namespace FiresecClient
{
	public class FiresecCallbackService : IFiresecCallbackService
	{
        public static FiresecCallbackService Current;
        public FiresecCallbackService()
        {
            Current = this;
        }

		public void NewJournalRecords(List<JournalRecord> journalRecords)
		{
			SafeOperationCall(() =>
			{
				foreach (var journalRecord in journalRecords)
				{
					if (NewJournalRecordEvent != null)
						NewJournalRecordEvent(journalRecord);
				}
			});
		}

		public void ConfigurationChanged()
		{
			SafeOperationCall(() =>
			{
				if (ConfigurationChangedEvent != null)
					ConfigurationChangedEvent();
			});
		}

		public Guid Ping()
		{
			try
			{
				return FiresecManager.ClientCredentials.ClientUID;
			}
			catch(Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecCallbackService.Ping");
				return Guid.Empty;
			}
		}

		public void GetFilteredArchiveCompleted(IEnumerable<JournalRecord> journalRecords)
		{
			SafeOperationCall(() =>
			{
				if (GetFilteredArchiveCompletedEvent != null)
					GetFilteredArchiveCompletedEvent(journalRecords);
			});
		}

		public void Notify(string message)
		{
			SafeOperationCall(() =>
			{
				if (NotifyEvent != null)
					NotifyEvent(message);
			});
		}

		public static event Action<JournalRecord> NewJournalRecordEvent;
		public static event Action ConfigurationChangedEvent;
		public static event Action<IEnumerable<JournalRecord>> GetFilteredArchiveCompletedEvent;
		public static event Action<string> NotifyEvent;

		static void SafeOperationCall(Action action)
		{
			try
			{
				action();
				//var thread = new Thread(new ThreadStart(action));
				//thread.Start();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecCallbackService.SafeOperationCall");
			}
		}
	}
}