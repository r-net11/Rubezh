using Common;
using RubezhAPI;
using RubezhAPI.AutomationCallback;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using System;
using System.Collections.Generic;
using System.Threading;

namespace RubezhClient
{
	public partial class SafeFiresecService
	{
		public static event Action<GKProgressCallback> GKProgressCallbackEvent;
		public static event Action<GKCallbackResult> GKCallbackResultEvent;
		public static event Action<GKPropertyChangedCallback> GKPropertyChangedEvent;
		public static event Action<CallbackOperationResult> CallbackOperationResultEvent;
		public static event Action<AutomationCallbackResult> AutomationEvent;
		public static event Action ConfigurationChangedEvent;
		public static event Action ReconnectionRequiredEvent;
		public static event Action<List<JournalItem>, bool> JournalItemsEvent;

		bool isConnected = true;
		public bool SuspendPoll = false;
		Thread PollThread;
		int CallbackIndex = -1;

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

					var pollResult = Poll(FiresecServiceFactory.UID, CallbackIndex);
					ProcessPollResult(pollResult);
				}
				catch (Exception e)
				{
					Logger.Error(e, "SafeFiresecService.OnPoll");
				}
			}
		}

		public static void ProcessAutomationCallback(AutomationCallbackResult callback, Guid? clientUid = null)
		{
			SafeContext.Execute(() =>
			{
				if (AutomationEvent != null)
					AutomationEvent(callback);
			});
		}

		void ProcessPollResult(PollResult pollResult)
		{
			if (pollResult == null)
				return;

			CallbackIndex = pollResult.CallbackIndex;

			//if (pollResult.IsDisconnecting) ;

			if (pollResult.IsReconnectionRequired)
			{
				SafeContext.Execute(() =>
				{
					if (ReconnectionRequiredEvent != null)
						ReconnectionRequiredEvent();
				});
				return;
			}

			foreach (var callbackResult in pollResult.CallbackResults)
			{
				switch (callbackResult.CallbackResultType)
				{
					case CallbackResultType.GKProgress:
						SafeContext.Execute(() =>
						{
							if (GKProgressCallbackEvent != null)
								GKProgressCallbackEvent(callbackResult.GKProgressCallback);
						});
						break;

					case CallbackResultType.GKObjectStateChanged:
						SafeContext.Execute(() =>
						{
							if (GKCallbackResultEvent != null)
								GKCallbackResultEvent(callbackResult.GKCallbackResult);
						});
						break;

					case CallbackResultType.GKPropertyChanged:
						SafeContext.Execute(() =>
						{
							if (GKPropertyChangedEvent != null)
								GKPropertyChangedEvent(callbackResult.GKPropertyChangedCallback);
						});
						break;

					case CallbackResultType.OperationResult:
						SafeContext.Execute(() =>
						{
							if (CallbackOperationResultEvent != null)
								CallbackOperationResultEvent(callbackResult.CallbackOperationResult);
						});
						break;

					case CallbackResultType.AutomationCallbackResult:
						ProcessAutomationCallback(callbackResult.AutomationCallbackResult);
						break;

					case CallbackResultType.NewEvents:
					case CallbackResultType.UpdateEvents:
						SafeContext.Execute(() =>
						{
							if (JournalItemsEvent != null)
								JournalItemsEvent(callbackResult.JournalItems, callbackResult.CallbackResultType == CallbackResultType.NewEvents);
						});
						break;

					case CallbackResultType.ConfigurationChanged:
						SafeContext.Execute(() =>
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