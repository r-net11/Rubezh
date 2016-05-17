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
	public partial class SafeRubezhService
	{
		public static event Action<GKProgressCallback> GKProgressCallbackEvent;
		public static event Action<GKCallbackResult> GKCallbackResultEvent;
		public static event Action<RviCallbackResult> RviCallbackResultEvent;
		public static event Action<GKPropertyChangedCallback> GKPropertyChangedEvent;
		public static event Action<CallbackOperationResult> CallbackOperationResultEvent;
		public static event Action<AutomationCallbackResult> AutomationEvent;
		public static event Action ConfigurationChangedEvent;
		public static event Action RestartEvent;
		public static event Action<List<JournalItem>, bool> JournalItemsEvent;

		public bool IsConnected { get; private set; }
		public bool SuspendPoll = false;
		Thread PollThread;
		int CallbackIndex = -1;

		public void StartPoll()
		{
			PollThread = new Thread(OnPoll);
			PollThread.Name = "SafeRubezhService Poll";
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

					var pollResult = Poll(RubezhServiceFactory.UID, CallbackIndex);
					ProcessPollResult(pollResult);
				}
				catch (Exception e)
				{
					Logger.Error(e, "SafeRubezhService.OnPoll");
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

			if (pollResult.IsReconnectionRequired)
			{
				Reconnect();
				return;
			}

			foreach (var callbackResult in pollResult.CallbackResults)
			{
				SafeContext.Execute(() =>
				{
					switch (callbackResult.CallbackResultType)
					{
						case CallbackResultType.GKProgress:
							if (GKProgressCallbackEvent != null)
								GKProgressCallbackEvent(callbackResult.GKProgressCallback);
							break;

						case CallbackResultType.GKObjectStateChanged:
							if (GKCallbackResultEvent != null)
								GKCallbackResultEvent(callbackResult.GKCallbackResult);
							break;

						case CallbackResultType.RviObjectStateChanged:
							if (RviCallbackResultEvent != null)
								RviCallbackResultEvent(callbackResult.RviCallbackResult);
							break;

						case CallbackResultType.GKPropertyChanged:
							if (GKPropertyChangedEvent != null)
								GKPropertyChangedEvent(callbackResult.GKPropertyChangedCallback);
							break;

						case CallbackResultType.OperationResult:
							if (CallbackOperationResultEvent != null)
								CallbackOperationResultEvent(callbackResult.CallbackOperationResult);
							break;

						case CallbackResultType.AutomationCallbackResult:
							ProcessAutomationCallback(callbackResult.AutomationCallbackResult);
							break;

						case CallbackResultType.NewEvents:
						case CallbackResultType.UpdateEvents:
							if (JournalItemsEvent != null)
								JournalItemsEvent(callbackResult.JournalItems, callbackResult.CallbackResultType == CallbackResultType.NewEvents);
							break;

						case CallbackResultType.ConfigurationChanged:
							if (ConfigurationChangedEvent != null)
								ConfigurationChangedEvent();
							break;
					}
				});
			}
		}

		void Reconnect()
		{
			try
			{
				SuspendPoll = true;
				var operationResult = ClientManager.RubezhService.Connect(ClientManager.ClientCredentials);
				if (operationResult.HasError && RestartEvent != null)
					RestartEvent();
			}
			catch (Exception e)
			{
				Logger.Error(e, "SafeRubezhService.Reconnect");
			}
			finally
			{
				SuspendPoll = false;
			}
		}
	}
}