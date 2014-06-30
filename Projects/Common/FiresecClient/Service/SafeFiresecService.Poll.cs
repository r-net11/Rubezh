using System;
using System.Collections.Generic;
using System.Threading;
using Common;
using Firesec;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecAPI.SKD;

namespace FiresecClient
{
	public partial class SafeFiresecService
	{
		public static event Action<GKProgressCallback> GKProgressCallbackEvent;
		public static event Action<GKCallbackResult> GKCallbackResultEvent;
		public static event Action<SKDCallbackResult> SKDCallbackResultEvent;
		public static event Action ConfigurationChangedEvent;
		public static event Action<FiresecAPI.SKD.JournalItem> NewJournalItemEvent;
		public static event Action<IEnumerable<FiresecAPI.SKD.JournalItem>> GetFilteredArchiveCompletedEvent;
		public static event Action<IEnumerable<FiresecAPI.GK.JournalItem>, Guid> GetFilteredGKArchiveCompletedEvent;
		public static event Action<IEnumerable<FiresecAPI.SKD.JournalItem>> GetFilteredSKDArchiveCompletedEvent;

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
						foreach (var journalItem in callbackResult.GlobalJournalItems)
						{
							SafeOperationCall(() =>
							{
								if (NewJournalItemEvent != null)
									NewJournalItemEvent(journalItem);

							});
						}
						break;

					case CallbackResultType.ArchiveCompleted:
						SafeOperationCall(() =>
						{
							if (GetFilteredArchiveCompletedEvent != null)
								GetFilteredArchiveCompletedEvent(callbackResult.GlobalJournalItems);
						});
						break;

					case CallbackResultType.GKArchiveCompleted:
						SafeOperationCall(() =>
						{
							if (GetFilteredGKArchiveCompletedEvent != null)
								GetFilteredGKArchiveCompletedEvent(callbackResult.JournalItems, callbackResult.ArchivePortionUID);
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