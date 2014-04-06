using System;
using System.Collections.Generic;
using System.Threading;
using Common;
using Firesec;
using FiresecAPI;
using FiresecAPI.Models;
using XFiresecAPI;

namespace FiresecClient
{
	public partial class SafeFiresecService
	{
		public static event Action<GKProgressCallback> GKProgressCallbackEvent;
		public static event Action<GKCallbackResult> GKCallbackResultEvent;
		public static event Action<SKDCallbackResult> SKDCallbackResultEvent;
		public static event Action ConfigurationChangedEvent;
		public static event Action<JournalRecord> NewJournalRecordEvent;
		public static event Action<IEnumerable<JournalRecord>> GetFilteredArchiveCompletedEvent;
		public static event Action<IEnumerable<JournalItem>> GetFilteredGKArchiveCompletedEvent;
		public static event Action<IEnumerable<SKDJournalItem>> GetFilteredSKDArchiveCompletedEvent;

		bool isConnected = true;
		public bool SuspendPoll = false;
		Thread PollThread;
		bool MustReactOnCallback;

		public void StartPoll(bool mustReactOnCallback)
		{
			MustReactOnCallback = mustReactOnCallback;
			PollThread = new Thread(OnPoll);
			PollThread.Name = "SafeFiresecService Poll";
			PollThread.IsBackground = true;
			PollThread.Start();
		}

		public void StopPoll()
		{
			if (PollThread != null)
			{
				try
				{
					if (!PollThread.Join(TimeSpan.FromSeconds(2)))
					{
						PollThread.Abort();
					}
				}
				catch { }
			}
		}

		void OnPoll()
		{
			while (true)
			{
				try
				{
					if (IsDisconnecting)
						return;

					if (SuspendPoll)
					{
						Thread.Sleep(TimeSpan.FromSeconds(5));
						continue;
					}

					var callbackResults = Poll(FiresecServiceFactory.UID);
					//if (!MustReactOnCallback)
					{
						ProcessCallbackResult(callbackResults);
					}
				}
				catch (Exception e)
				{
					Logger.Error(e, "SafeFiresecService.OnPoll");
				}
			}
		}

		void ProcessCallbackResult(List<CallbackResult> callbackResults)
		{
			if (callbackResults == null || callbackResults.Count == 0)
				return;

			foreach (var callbackResult in callbackResults)
			{
				switch (callbackResult.CallbackResultType)
				{
					case CallbackResultType.GKProgress:
						SafeOperationCall(() =>
						{
							if (GKProgressCallbackEvent != null)
								GKProgressCallbackEvent(callbackResult.GKProgressCallback);
						});
						break;

					case CallbackResultType.GKObjectStateChanged:
						SafeOperationCall(() =>
						{
							if (GKCallbackResultEvent != null)
								GKCallbackResultEvent(callbackResult.GKCallbackResult);
						});
						break;

					case CallbackResultType.SKDObjectStateChanged:
						SafeOperationCall(() =>
						{
							if (SKDCallbackResultEvent != null)
								SKDCallbackResultEvent(callbackResult.SKDCallbackResult);
						});
						break;

					case CallbackResultType.NewEvents:
						foreach (var journalRecord in callbackResult.JournalRecords)
						{
							SafeOperationCall(() =>
							{
								if (NewJournalRecordEvent != null)
									NewJournalRecordEvent(journalRecord);
							});
						}
						break;

					case CallbackResultType.ArchiveCompleted:
						SafeOperationCall(() =>
						{
							callbackResult.JournalRecords.ForEach((x) => { JournalConverter.SetDeviceCatogoryAndDevieUID(x); });
							if (GetFilteredArchiveCompletedEvent != null)
								GetFilteredArchiveCompletedEvent(callbackResult.JournalRecords);
						});
						break;

					case CallbackResultType.GKArchiveCompleted:
						SafeOperationCall(() =>
						{
							if (GetFilteredGKArchiveCompletedEvent != null)
								GetFilteredGKArchiveCompletedEvent(callbackResult.JournalItems);
						});
						break;

					case CallbackResultType.SKDArchiveCompleted:
						SafeOperationCall(() =>
						{
							if (GetFilteredSKDArchiveCompletedEvent != null)
								GetFilteredSKDArchiveCompletedEvent(callbackResult.SKDCallbackResult.JournalItems);
						});
						break;

					case CallbackResultType.ConfigurationChanged:
						SafeOperationCall(() =>
						{
							if (ConfigurationChangedEvent != null)
								ConfigurationChangedEvent();
						});
						break;
				}
			}
		}
	}
}