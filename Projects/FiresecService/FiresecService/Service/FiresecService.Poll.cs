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
			CallbackNewJournal(journalRecords);
			return "Test";
		}

        public IAsyncResult BeginPoll(int index, DateTime dateTime, AsyncCallback asyncCallback, object state)
        {
            PollAsyncResult pollAsyncResult = new PollAsyncResult(index, dateTime, asyncCallback, state);
            ThreadPool.QueueUserWorkItem(new WaitCallback(PollCallback), pollAsyncResult);
            return pollAsyncResult;
        }

        public List<CallbackResult> EndPoll(IAsyncResult asyncResult)
        {
            if (asyncResult != null)
            {
                using (PollAsyncResult pollAsyncResult = asyncResult as PollAsyncResult)
                {
                    if (pollAsyncResult == null)
                    {
                        Logger.Error("FiresecService.EndPoll PollAsyncResult = null");
                        return null;
                    }

                    pollAsyncResult.AsyncWait.WaitOne();
                    return pollAsyncResult.Result;
                }
            }
            return null;
        }

		public List<CallbackResult> ShortPoll(Guid uid)
		{
			lock (CallbackResultsLocker)
			{
				var clientInfo = ClientsManager.ClientInfos.FirstOrDefault(x => x.UID == uid);
				if (clientInfo != null)
				{
					var result = CallbackManager.Get(clientInfo.CallbackIndex);
					clientInfo.CallbackIndex = CallbackManager.GetLastIndex();
					return result;
				}
				return new List<CallbackResult>();

				var callbackResults = new List<CallbackResult>(CallbackResults);
				CallbackResults = new List<CallbackResult>();
				return callbackResults;
			}
		}

        private void PollCallback(object state)
        {
            PollAsyncResult asyncResult = state as PollAsyncResult;
            try
            {
                asyncResult.Result = InternalPoll(asyncResult.index, asyncResult.dateTime);
                StopPingEvent = null;
                CallbackResults = new List<CallbackResult>();
            }
            finally
            {
                asyncResult.Complete();
            }
        }

        private List<CallbackResult> InternalPoll(int index, DateTime dateTime)
        {
            if (CallbackResults.Count > 0)
            {
                return new List<CallbackResult>(CallbackResults);
            }
            CallbackResults = new List<CallbackResult>();
            StopPingEvent = new AutoResetEvent(false);
            if (StopPingEvent.WaitOne(500000))
            {
                return new List<CallbackResult>(CallbackResults);
            }
            return new List<CallbackResult>();
        }

        public void CallbackNewJournal(List<JournalRecord> journalRecords)
        {
            var callbackResult = new CallbackResult()
            {
                CallbackResultType = CallbackResultType.NewEvents,
                JournalRecords = journalRecords
            };
            AddCallback(callbackResult);
        }

        public void CallbackArchiveCompleted(List<JournalRecord> journalRecords)
        {
            var callbackResult = new CallbackResult()
            {
                CallbackResultType = CallbackResultType.ArchiveCompleted,
                JournalRecords = journalRecords
            };
            AddCallback(callbackResult);
        }

        public void CallbackConfigurationChanged()
        {
            var callbackResult = new CallbackResult()
            {
                CallbackResultType = CallbackResultType.ConfigurationChanged
            };
            AddCallback(callbackResult);
        }

		void AddCallback(CallbackResult callbackResult)
		{
			lock (CallbackResultsLocker)
			{
				CallbackManager.Add(callbackResult);
				return;

				if (CallbackResults.Count > 10)
					CallbackResults.RemoveAt(0);
				CallbackResults.Add(callbackResult);
				if (StopPingEvent != null)
				{
					StopPingEvent.Set();
				}
			}
		}

        List<CallbackResult> CallbackResults = new List<CallbackResult>();
        AutoResetEvent StopPingEvent;
		object CallbackResultsLocker = new object();
    }
}