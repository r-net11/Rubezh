using System;
using System.Collections.Generic;
using System.Threading;
using Common;
using StrazhAPI;
using StrazhAPI.AutomationCallback;
using StrazhAPI.GK;
using StrazhAPI.Journal;
using StrazhAPI.SKD;
using StrazhAPI.SKD.Device;
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
		/// <summary>
		/// Событие прохода по "Гостевой" карте
		/// </summary>
		public static event Action<SKDCard> GuestCardPassedEvent;
		/// <summary>
		/// Событие деактивации карты
		/// </summary>
		public static event Action<SKDCard> CardDeactivatedEvent;
		/// <summary>
		/// Событие изменения лога загрузки Сервера
		/// </summary>
		public static event Action CoreLoadingLogChangedEvent;

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
					// Поступило уведомление о проходе по "Гостевой" карте
					case CallbackResultType.GuestCardPassed:
						SafeOperationCall(() =>
						{
							if (GuestCardPassedEvent != null)
								GuestCardPassedEvent(callbackResult.Card);
						});
						break;
					// Поступило уведомление о деактивации карты
					case CallbackResultType.CardDeactivated:
						SafeOperationCall(() =>
						{
							if (CardDeactivatedEvent != null)
								CardDeactivatedEvent(callbackResult.Card);
						});
						break;
					// Поступило уведомление об изменении лога загрузки Сервера
					case CallbackResultType.CoreLoadingLogChanged:
						Logger.Info("Поступило уведомление об изменении лога загрузки Сервера");
						SafeOperationCall(() =>
						{
							if (CoreLoadingLogChangedEvent != null)
								CoreLoadingLogChangedEvent();
						});
						break;
				}
			}
		}
	}
}