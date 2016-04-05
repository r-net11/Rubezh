using FiresecAPI;
using FiresecAPI.AutomationCallback;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FiresecAPI.SKD.Device;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public List<CallbackResult> Poll(Guid uid)
		{
			var clientInfo = ClientsManager.ClientInfos.FirstOrDefault(x => x.UID == uid);
			if (clientInfo != null)
			{
				var result = CallbackManager.Get(clientInfo);
				if (result.Count == 0)
				{
					clientInfo.WaitEvent = new AutoResetEvent(false);
					if (clientInfo.WaitEvent.WaitOne(TimeSpan.FromMinutes(5)))
					{
						result = CallbackManager.Get(clientInfo);
					}
				}
				return result;
			}
			return new List<CallbackResult>();
		}

		public static void NotifySKDProgress(SKDProgressCallback SKDProgressCallback)
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = CallbackResultType.SKDProgress,
				SKDProgressCallback = SKDProgressCallback
			};
			CallbackManager.Add(callbackResult);
		}

		public static void NotifySKDObjectStateChanged(SKDStates skdStates)
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = CallbackResultType.SKDObjectStateChanged,
				SKDStates = skdStates
			};
			CallbackManager.Add(callbackResult);
		}

		public static void NotifyAutomation(AutomationCallbackResult automationCallbackResult, Guid? clientUID = null)
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = CallbackResultType.AutomationCallbackResult,
				AutomationCallbackResult = automationCallbackResult,
			};
			CallbackManager.Add(callbackResult, clientUID);
		}

		public static void NotifyNewJournalItems(List<JournalItem> journalItems)
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = CallbackResultType.NewEvents,
				JournalItems = journalItems
			};
			CallbackManager.Add(callbackResult);
		}

		public static void NotifyArchiveCompleted(List<JournalItem> journallItems, Guid archivePortionUID)
		{
			var callbackResult = new CallbackResult()
			{
				ArchivePortionUID = archivePortionUID,
				CallbackResultType = CallbackResultType.ArchiveCompleted,
				JournalItems = journallItems,
			};
			CallbackManager.Add(callbackResult);
		}

		public void NotifyConfigurationChanged()
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = CallbackResultType.ConfigurationChanged
			};
			CallbackManager.Add(callbackResult);
		}
		
		public static void NotifyNewSearchDevices(List<SKDDeviceSearchInfo> searchDevices)
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = CallbackResultType.NewSearchDevices,
				SearchDevices = searchDevices
			};
			CallbackManager.Add(callbackResult);
		}

		/// <summary>
		/// Посылает команду Клиенту на закрытие соединения с Сервером
		/// </summary>
		/// <param name="clientUid">Идентификатор клиента, которому посылается команда</param>
		public void SendDisconnectClientCommand(Guid clientUid)
		{
			CallbackManager.Add(new CallbackResult { CallbackResultType = CallbackResultType.DisconnectClientCommand }, clientUid);
		}

		/// <summary>
		/// Посылает Клиентам уведомление о смене лицензии на Сервере
		/// </summary>
		public void NotifyLicenseChanged()
		{
			// Если к Серверу не подключено ни одного Клиента, то выходим
			if (!ClientsManager.ClientInfos.Any())
				return;
			
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = CallbackResultType.LicenseChanged
			};
			CallbackManager.Add(callbackResult);
		}
	}
}