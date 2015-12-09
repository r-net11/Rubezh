﻿using System;
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
using RubezhAPI.Models.Layouts;

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
		public static event Action<List<JournalItem>, bool> JournalItemsEvent;
		
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
					ProcessCallbackResult(callbackResults);
				}
				catch (Exception e)
				{
					Logger.Error(e, "SafeFiresecService.OnPoll");
				}
			}
		}

		public static void ProcessAutomationCallback(AutomationCallbackResult callback, Guid? clientUid = null)
		{
			if (callback.Data == null || callback.Data.LayoutFilter == null || ApplicationService.Shell == null)
				return;
			var layoutUID = ApplicationService.Shell.Layout == null ? 
				Layout.NoLayoutUID : 
				ApplicationService.Shell.Layout.UID;
			if (callback.Data.LayoutFilter.LayoutsUIDs.Contains(layoutUID))
				SafeOperationCall(() =>
				{
					if (AutomationEvent != null)
						AutomationEvent(callback);
				});
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

					case CallbackResultType.GKPropertyChanged:
						SafeOperationCall(() =>
						{
							if (GKPropertyChangedEvent != null)
								GKPropertyChangedEvent(callbackResult.GKPropertyChangedCallback);
						});
						break;

					case CallbackResultType.OperationResult:
						SafeOperationCall(() =>
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
						SafeOperationCall(() =>
						{
							if (JournalItemsEvent != null)
								JournalItemsEvent(callbackResult.JournalItems, callbackResult.CallbackResultType == CallbackResultType.NewEvents);
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