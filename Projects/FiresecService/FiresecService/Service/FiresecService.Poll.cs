using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using System.Diagnostics;

namespace FiresecService.Service
{
    public partial class FiresecService
    {
		public string Test(string arg)
		{
			var journalRecords = new List<JournalRecord>();
			var journalRecord = new JournalRecord()
			{
				DeviceTime = DateTime.Now,
				SystemTime = DateTime.Now,
				Description = arg
			};
			journalRecords.Add(journalRecord);
			NotifyNewJournal(journalRecords);
			return "Test";
		}

        public List<CallbackResult> Poll(Guid uid)
        {
            var clientInfo = ClientsManager.ClientInfos.FirstOrDefault(x => x.UID == uid);
            if (clientInfo != null)
            {
                var result = CallbackManager.Get(clientInfo);
                if (result.Count == 0)
                {
                    clientInfo.WaitEvent = new AutoResetEvent(false);
                    if (clientInfo.WaitEvent.WaitOne(TimeSpan.FromMinutes(5)))
                    {
                        result = CallbackManager.Get(clientInfo);
                    }
                }
                return result;
            }
            return new List<CallbackResult>();
        }

        public void NotifyNewJournal(List<JournalRecord> journalRecords)
        {
            var callbackResult = new CallbackResult()
            {
                CallbackResultType = CallbackResultType.NewEvents,
                JournalRecords = journalRecords
            };
            CallbackManager.Add(callbackResult);
        }

        public static void NotifyArchivePortionCompleted(List<JournalRecord> journalRecords)
        {
            var callbackResult = new CallbackResult()
            {
                CallbackResultType = CallbackResultType.ArchiveCompleted,
                JournalRecords = journalRecords
            };
            CallbackManager.Add(callbackResult);
        }

        public void NotifyArchiveCompleted(List<JournalRecord> journalRecords)
        {
            var callbackResult = new CallbackResult()
            {
                CallbackResultType = CallbackResultType.ArchiveCompleted,
                JournalRecords = journalRecords
            };
            CallbackManager.Add(callbackResult);
        }

        public void NotifyConfigurationChanged()
        {
            var callbackResult = new CallbackResult()
            {
                CallbackResultType = CallbackResultType.ConfigurationChanged
            };
            CallbackManager.Add(callbackResult);
        }
    }
}