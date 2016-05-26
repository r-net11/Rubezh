using System.Threading.Tasks;
using Common;
using FiresecService.Service.Validators;
using StrazhAPI;
using StrazhAPI.AutomationCallback;
using StrazhAPI.Journal;
using StrazhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using StrazhAPI.SKD.Device;

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
			
			// После посылки Клиенту команды на разрыв соединения ждем 2 секунды и сами имитируем вызов от имени Клиента на разрыв соединения,
			// т.к. "мертвый" Клиент этого не сделает
			Task.Factory.StartNew(() =>
			{
				Thread.Sleep(TimeSpan.FromSeconds(2));
				Disconnect(clientUid);
			});
		}

		/// <summary>
		/// Монитор Сервера уведомляет Сервер о смене лицензии
		/// </summary>
		public void NotifyLicenseChanged()
		{
			Logger.Info("Получено сообщение об изменении лицензии");
			// Сервер обновляет лицензию у себя
			if (!_licenseManager.IsValidExistingKey())
				Logger.Warn("Отсутствует лицензия");
			else
				Logger.Info("Лицензия проверена");

			Logger.Info("Инициализируем валидатор конфигурации");
			ConfigurationElementsAgainstLicenseDataValidator.Instance.Validate();

			Logger.Info("Уведомляем подключенных Клиентов об изменении лицензии");
			// и уведомляет об это своих Клиентов
			foreach (var clientInfo in ClientsManager.ClientInfos)
			{
				var callbackResult = new CallbackResult()
				{
					CallbackResultType = CallbackResultType.LicenseChanged
				};
				CallbackManager.Add(callbackResult, clientInfo.ClientCredentials.ClientUID);
			}
		}

		/// <summary>
		/// Посылает подключенным Клиентам уведомление о том, что был осуществлен проход по "Гостевой" карте
		/// </summary>
		/// <param name="card">Гостевая карта</param>
		public void NotifyGuestCardPassed(SKDCard card)
		{
			foreach (var clientInfo in ClientsManager.ClientInfos)
			{
				var callbackResult = new CallbackResult()
				{
					CallbackResultType = CallbackResultType.GuestCardPassed,
					Card = card
				};
				CallbackManager.Add(callbackResult, clientInfo.ClientCredentials.ClientUID);
			}
		}

		/// <summary>
		/// Посылает подключенным Клиентам уведомление о деактивации карты
		/// </summary>
		/// <param name="card">Деактивированная карта</param>
		public void NotifyCardDeactivated(SKDCard card)
		{
			foreach (var clientInfo in ClientsManager.ClientInfos)
			{
				var callbackResult = new CallbackResult()
				{
					CallbackResultType = CallbackResultType.CardDeactivated,
					Card = card
				};
				CallbackManager.Add(callbackResult, clientInfo.ClientCredentials.ClientUID);
			}
		}
	}
}