using System;
using System.Collections.Generic;
using System.Threading;
using Common;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.SKD;

namespace FiresecClient
{
	public partial class SafeFiresecService
	{
		public static event Action<GKProgressCallback> GKProgressCallbackEvent;
		public static event Action<GKCallbackResult> GKCallbackResultEvent;
		public static event Action<SKDStates> SKDStatesEvent;
		public static event Action ConfigurationChangedEvent;
		public static event Action<JournalItem> NewJournalItemEvent;
		public static event Action<IEnumerable<XJournalItem>, Guid> GetFilteredGKArchiveCompletedEvent;
		public static event Action<IEnumerable<JournalItem>, Guid> GetFilteredSKDArchiveCompletedEvent;

		bool isConnected = true;
		public bool SuspendPoll = false;
		Thread PollThread;

		public void StartPoll()
		{
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
					ProcessCallbackResult(callbackResults);
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
							if (SKDStatesEvent != null)
								SKDStatesEvent(callbackResult.SKDStates);
						});
						break;

					case CallbackResultType.NewEvents:
						foreach (var journalItem in callbackResult.JournalItems)
						{
							SafeOperationCall(() =>
							{
								if (NewJournalItemEvent != null)
									NewJournalItemEvent(journalItem);

							});
						}
						break;

					case CallbackResultType.SKDArchiveCompleted:
						SafeOperationCall(() =>
						{
							if (GetFilteredSKDArchiveCompletedEvent != null)
								GetFilteredSKDArchiveCompletedEvent(callbackResult.JournalItems, callbackResult.ArchivePortionUID);
						});
						break;

					case CallbackResultType.GKArchiveCompleted:
						SafeOperationCall(() =>
						{
							if (GetFilteredGKArchiveCompletedEvent != null)
								GetFilteredGKArchiveCompletedEvent(callbackResult.GKJournalItemsArchiveCompleted, callbackResult.ArchivePortionUID);
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