using System;
using System.Collections.Generic;
using System.Threading;
using Common;
using FiresecAPI;
using FiresecAPI.AutomationCallback;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using FiresecAPI.SKD.Device;
using Infrastructure.Common.Windows;

namespace FiresecClient
{
	public partial class SafeFiresecService
	{
		public static event Action<SKDProgressCallback> SKDProgressCallbackEvent;
		public static event Action<SKDCallbackResult> SKDCallbackResultEvent;
		public static event Action<SKDStates> SKDStatesEvent;
		public static event Action<AutomationCallbackResult> AutomationEvent;
		public static event Action ConfigurationChangedEvent;
		public static event Action<JournalItem> NewJournalItemEvent;
		public static event Action<IEnumerable<JournalItem>, Guid> GetFilteredArchiveCompletedEvent;
		public static event Action<SKDDeviceSearchInfo> NewSearchDeviceEvent;
		public static event Action DisconnectClientCommandEvent;
		public static event Action LicenseChangedEvent;

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
					case CallbackResultType.SKDProgress:
						SafeOperationCall(() =>
						{
							if (SKDProgressCallbackEvent != null)
								SKDProgressCallbackEvent(callbackResult.SKDProgressCallback);
						});
						break;

					case CallbackResultType.SKDObjectStateChanged:
						SafeOperationCall(() =>
						{
							if (SKDStatesEvent != null)
								SKDStatesEvent(callbackResult.SKDStates);
						});
						break;

					case CallbackResultType.AutomationCallbackResult:
						if (callbackResult.AutomationCallbackResult.Data == null || callbackResult.AutomationCallbackResult.Data.LayoutFilter == null || callbackResult.AutomationCallbackResult.Data.LayoutFilter.LayoutsUIDs.Count == 0 ||
							ApplicationService.Shell == null || ApplicationService.Shell.Layout == null || callbackResult.AutomationCallbackResult.Data.LayoutFilter.LayoutsUIDs.Contains(ApplicationService.Shell.Layout.UID))
							SafeOperationCall(() =>
							{
								if (AutomationEvent != null)
									AutomationEvent(callbackResult.AutomationCallbackResult);
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

					case CallbackResultType.ArchiveCompleted:
						SafeOperationCall(() =>
						{
							if (GetFilteredArchiveCompletedEvent != null)
								GetFilteredArchiveCompletedEvent(callbackResult.JournalItems, callbackResult.ArchivePortionUID);
						});
						break;

					case CallbackResultType.ConfigurationChanged:
						SafeOperationCall(() =>
						{
							if (ConfigurationChangedEvent != null)
								ConfigurationChangedEvent();
						});
						break;

					case CallbackResultType.NewSearchDevices:
						foreach (var searchDevice in callbackResult.SearchDevices)
						{
							SafeOperationCall(() =>
							{
								if (NewSearchDeviceEvent != null)
									NewSearchDeviceEvent(searchDevice);
							});
						}
						break;

					// Поступила команда на закрытие соединения с Сервером
					case CallbackResultType.DisconnectClientCommand:
						SafeOperationCall(() =>
						{
							if (DisconnectClientCommandEvent != null)
								DisconnectClientCommandEvent();
						});
						break;

					// Поступило уведомление о смене лицензии на Сервере
					case CallbackResultType.LicenseChanged:
						SafeOperationCall(() =>
						{
							if (LicenseChangedEvent != null)
								LicenseChangedEvent();
						});
						break;
				}
			}
		}
	}
}