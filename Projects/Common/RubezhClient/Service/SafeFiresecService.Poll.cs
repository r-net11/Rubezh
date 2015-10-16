using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Common;
using RubezhAPI;
using RubezhAPI.AutomationCallback;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.SKD;
using Infrastructure.Common.Windows;

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
		public static event Action<List<JournalItem>> NewJournalItemsEvent;
		public static event Action<DbCallbackResult> DbCallbackResultEvent;

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
			Poll(FiresecServiceFactory.UID);
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
				}
				catch (Exception e)
				{
					Logger.Error(e, "SafeFiresecService.OnPoll");
				}
			}
		}

		public static void ProcessAutomationCallback(AutomationCallbackResult callback, Guid? clientUid = null)
		{
			SafeOperationCall(() =>
			{
				if (AutomationEvent != null)
					AutomationEvent(callback);
			});
		}
	}
}