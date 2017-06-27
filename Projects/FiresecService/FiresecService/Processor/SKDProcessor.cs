using Common;
using FiresecService.Service;
using Localization.StrazhService.Core.Common;
using Localization.StrazhService.Core.Errors;
using StrazhAPI;
using StrazhAPI.GK;
using StrazhAPI.Journal;
using StrazhAPI.SKD;
using StrazhAPI.SKD.Device;
using StrazhDAL;
using StrazhDeviceSDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiresecService
{
	public static class SKDProcessor
	{
		static SKDProcessor()
		{
			Logger.Info("Запускаем службу синхронизации времени на контроллерах");
			ControllersTimeSynchronizer.Start();
		}

		public static void Start()
		{
			try
			{
				if (SKDManager.SKDConfiguration != null)
				{
					SKDManager.CreateDrivers();
					SKDManager.UpdateConfiguration();
					SKDManager.CreateStates();
				}
				Processor.Start();
				foreach (var deviceProcessor in Processor.DeviceProcessors)
				{
					deviceProcessor.NewJournalItem -= OnNewJournalItem;
					deviceProcessor.NewJournalItem += OnNewJournalItem;

					deviceProcessor.ConnectionAppeared -= OnConnectionAppeared;
					deviceProcessor.ConnectionAppeared += OnConnectionAppeared;
				}

				Processor.NewJournalItem -= OnNewJournalItem;
				Processor.NewJournalItem += OnNewJournalItem;

				Processor.StatesChangedEvent -= OnSKDStates;
				Processor.StatesChangedEvent += OnSKDStates;

				Processor.SKDProgressCallbackEvent -= OnSKDProgressCallbackEvent;
				Processor.SKDProgressCallbackEvent += OnSKDProgressCallbackEvent;

				// События функционала автопоиска контроллеров в сети
				Processor.NewSearchDevice -= OnNewSearchDevice;
				Processor.NewSearchDevice += OnNewSearchDevice;
			}
			catch (Exception e)
			{
				Logger.Error(e, "SKDProcessor.Start");
			}
		}

		public static void Stop()
		{
			Processor.Stop();
		}

		private static void OnNewJournalItem(JournalItem journalItem)
		{
			var cardNo = journalItem.CardNo;

			if (cardNo > 0)
			{
				using (var databaseService = new SKDDatabaseService())
				{
					var getEmployeeByCardNoOperationResult = databaseService.CardTranslator.GetEmployeeByCardNo(cardNo);
					if (!getEmployeeByCardNoOperationResult.HasError)
					{
						var employeeUID = getEmployeeByCardNoOperationResult.Result;
						journalItem.EmployeeUID = employeeUID;
						if (employeeUID != Guid.Empty)
						{
							var employee = databaseService.EmployeeTranslator.GetSingle(employeeUID);
							if (employee != null && employee.Result != null)
							{
								journalItem.UserName = employee.Result.Name;
							}
						}

						// Для онлайн событий прохода выполняем фиксацию факта прохода в БД журнала проходов
						if (journalItem.JournalEventNameType == JournalEventNameType.Проход_разрешен)
						{
							var readerDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
							if (readerDevice != null && readerDevice.Zone != null)
							{
								var zoneUID = readerDevice.Zone.UID;
								using (var passJournalTranslator = new PassJournalTranslator())
								{
									passJournalTranslator.AddPassJournal(employeeUID, zoneUID, journalItem.SystemDateTime);
								}
							}
						}
					}

					// Бизнес-логика для приложенной к считывателю "Гостевой" карты, если контроллер разрешает проход
					if (journalItem.JournalEventNameType == JournalEventNameType.Проход_разрешен &&
						journalItem.ObjectUID != Guid.Empty)
					{
						var getCardOperationResult = databaseService.CardTranslator.Get(cardNo);
						if (!getCardOperationResult.HasError)
						{
							var card = getCardOperationResult.Result;
							if (card != null &&
								card.CardType == CardType.Guest &&
								databaseService.AccessTemplateDeactivatingReaderTranslator.HasReader(journalItem.ObjectUID))
							{
								card.AllowedPassCount--;
								databaseService.CardTranslator.Save(card);
								// Если разрешенное число проходов равно нулю, деактивируем "Гостевую" карту
								if (card.AllowedPassCount == 0)
									FiresecServiceManager.SafeFiresecService.DeleteCardFromEmployee(card, journalItem.UserName, CommonErrors.PermittedPassNumberZero_Error);
								// Уведомляем подключенных Клиентов о том, что был осуществлен проход по "Гостевой" карте
								FiresecServiceManager.SafeFiresecService.FiresecService.NotifyGuestCardPassed(card);
							}
						}
					}
				}
			}

			journalItem.JournalSubsystemType = EventDescriptionAttributeHelper.ToSubsystem(journalItem.JournalEventNameType);

			// Фиксируем информацию о событии в БД журнала событий
			Service.FiresecService.AddCommonJournalItem(journalItem);
		}

		static void OnNewSearchDevice(SKDDeviceSearchInfo skdDeviceSearchInfo)
		{
			Service.FiresecService.NotifyNewSearchDevices(new List<SKDDeviceSearchInfo> { skdDeviceSearchInfo });
		}

		static void OnSKDStates(SKDStates skdStates)
		{
			if (skdStates.DeviceStates.Count > 0)
			{
				// Синхронизируем режим точки доступа согласно режиму замка
				foreach (var deviceState in skdStates.DeviceStates)
				{
					var door = SKDManager.SynchronizeDoorAccessStateForLock(deviceState.Device);
					if (door != null)
					{
						skdStates.DoorStates.Add(door.State);
					}
				}

				foreach (var zone in SKDManager.Zones)
				{
					var stateClasses = GetZoneStateClasses(zone);

					var hasChanges = stateClasses.Count != zone.State.StateClasses.Count;
					if (!hasChanges)
					{
						foreach (var stateClass in stateClasses)
						{
							if (!zone.State.StateClasses.Contains(stateClass))
								hasChanges = true;
						}
					}

					if (hasChanges)
					{
						zone.State.StateClasses = stateClasses;
						zone.State.StateClass = GetZoneStateClass(zone.State.StateClasses);
						skdStates.ZoneStates.Add(zone.State);
					}
				}
				foreach (var door in SKDManager.Doors)
				{
					var stateClasses = GetDoorStateClasses(door);

					var hasChanges = stateClasses.Count != door.State.StateClasses.Count;
					if (!hasChanges)
					{
						foreach (var stateClass in stateClasses)
						{
							if (!door.State.StateClasses.Contains(stateClass))
								hasChanges = true;
						}
					}

					if (hasChanges)
					{
						door.State.StateClasses = stateClasses;
						door.State.StateClass = door.State.StateClasses.Min();
						if (!skdStates.DoorStates.Contains(door.State))
							skdStates.DoorStates.Add(door.State);
					}
				}
			}

			Service.FiresecService.NotifySKDObjectStateChanged(skdStates);
			ProcedureRunner.RunOnStateChanged();
		}

		/// <summary>
		/// Возвращает состояние зоны в зависимости от состояний точек доступа (замков), ведущих в эту зону
		/// </summary>
		/// <param name="stateClasses">Состояния точек доступа (замков), ведущих в эту зону</param>
		/// <returns>Состояние зоны</returns>
		private static XStateClass GetZoneStateClass(IEnumerable<XStateClass> stateClasses)
		{
			var hasOpened = stateClasses.Any(x => x == XStateClass.On);
			var hasClosed = stateClasses.Any(x => x == XStateClass.Off);
			var hasBreaked = stateClasses.Any(x => x == XStateClass.Attention);
			var hasConnectionLost = stateClasses.Any(x => x == XStateClass.ConnectionLost);

			if (!hasOpened &&
				!hasClosed &&
				!hasBreaked &&
				!hasConnectionLost)
				return XStateClass.Unknown;

			if (hasOpened &&
				!hasClosed &&
				!hasBreaked &&
				!hasConnectionLost)
				return XStateClass.On;

			if (!hasOpened &&
				hasClosed &&
				!hasBreaked &&
				!hasConnectionLost)
				return XStateClass.Off;

			if (!hasOpened &&
				!hasClosed &&
				hasBreaked &&
				!hasConnectionLost)
				return XStateClass.Attention;

			if (!hasOpened &&
				!hasClosed &&
				!hasBreaked &&
				hasConnectionLost)
				return XStateClass.ConnectionLost;

			if (hasOpened &&
				hasClosed &&
				!hasBreaked &&
				!hasConnectionLost)
				return XStateClass.On;

			if (hasOpened &&
				!hasClosed &&
				hasBreaked &&
				!hasConnectionLost)
				return XStateClass.Attention;

			if (hasOpened &&
				!hasClosed &&
				!hasBreaked &&
				hasConnectionLost)
				return XStateClass.On;

			if (hasOpened &&
				hasClosed &&
				hasBreaked &&
				!hasConnectionLost)
				return XStateClass.Attention;

			if (hasOpened &&
				hasClosed &&
				!hasBreaked &&
				hasConnectionLost)
				return XStateClass.On;

			if (hasOpened &&
				hasClosed &&
				hasBreaked &&
				hasConnectionLost)
				return XStateClass.Attention;

			if (!hasOpened &&
				hasClosed &&
				hasBreaked &&
				hasConnectionLost)
				return XStateClass.Attention;

			if (!hasOpened &&
				hasClosed &&
				!hasBreaked &&
				hasConnectionLost)
				return XStateClass.Off;

			if (!hasOpened &&
				!hasClosed &&
				hasBreaked &&
				hasConnectionLost)
				return XStateClass.Attention;

			if (!hasOpened &&
				hasClosed &&
				hasBreaked &&
				!hasConnectionLost)
				return XStateClass.Attention;

			if (hasOpened &&
				!hasClosed &&
				hasBreaked &&
				hasConnectionLost)
				return XStateClass.Attention;

			return XStateClass.Unknown;
		}

		/// <summary>
		/// Возвращает список состояний точек доступа (замков), ведущих в эту зону
		/// </summary>
		/// <param name="zone">Зона</param>
		/// <returns>Список состояний точек доступа (замков), ведущих в эту зону</returns>
		private static List<XStateClass> GetZoneStateClasses(SKDZone zone)
		{
			var stateClasses = new List<XStateClass>();
			foreach (var readerDevice in zone.Devices)
			{
				var lockAddress = readerDevice.IntAddress;
				if (readerDevice.Parent != null && readerDevice.Parent.DoorType == DoorType.TwoWay)
				{
					lockAddress = readerDevice.IntAddress / 2;
				}
				var lockDevice = readerDevice.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == lockAddress);
				if (lockDevice != null)
				{
					if (!stateClasses.Contains(lockDevice.State.StateClass))
						stateClasses.Add(lockDevice.State.StateClass);
				}
			}
			stateClasses.Sort();
			if (stateClasses.Count == 0)
				stateClasses.Add(XStateClass.Unknown);
			return stateClasses;
		}

		private static List<XStateClass> GetDoorStateClasses(SKDDoor door)
		{
			var stateClasses = new List<XStateClass>();

			if (door.InDevice != null)
			{
				var lockAddress = door.InDevice.IntAddress;
				if (door.DoorType == DoorType.TwoWay)
				{
					lockAddress = door.InDevice.IntAddress / 2;
				}
				var lockDevice = door.InDevice.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == lockAddress);
				if (lockDevice != null)
				{
					if (!stateClasses.Contains(lockDevice.State.StateClass))
						stateClasses.Add(lockDevice.State.StateClass);
				}
			}

			stateClasses.Sort();
			if (stateClasses.Count == 0)
				stateClasses.Add(XStateClass.Unknown);
			return stateClasses;
		}

		public static SKDStates SKDGetStates()
		{
			var skdStates = new SKDStates();
			foreach (var device in SKDManager.Devices)
			{
				skdStates.DeviceStates.Add(device.State);
			}
			foreach (var zone in SKDManager.Zones.Where(x => x.State != null))
			{
				zone.State.StateClasses = GetZoneStateClasses(zone);
				zone.State.StateClass = zone.State.StateClasses.Min();
				skdStates.ZoneStates.Add(zone.State);
			}
			foreach (var door in SKDManager.Doors.Where(x => x.State != null))
			{
				door.State.StateClasses = GetDoorStateClasses(door);
				door.State.StateClass = door.State.StateClasses.Min();
				skdStates.DoorStates.Add(door.State);
			}
			return skdStates;
		}

		private static void OnSKDProgressCallbackEvent(SKDProgressCallback skdProgressCallback)
		{
			Service.FiresecService.NotifySKDProgress(skdProgressCallback);
		}

		private static void OnConnectionAppeared(DeviceProcessor deviceProcessor)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				var pendingCards = databaseService.CardTranslator.GetAllPendingCards(deviceProcessor.Device.UID);
				foreach (var pendingCard in pendingCards)
				{
					var operationResult = databaseService.CardTranslator.GetSingle(pendingCard.CardUID);
					if (!operationResult.HasError && operationResult.Result != null)
					{
						var card = operationResult.Result;
						var getAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(card.AccessTemplateUID);
						var cardWriter = new CardWriter();
						if ((PendingCardAction)pendingCard.Action == PendingCardAction.Add)
						{
							cardWriter = Processor.AddCard(card, getAccessTemplateOperationResult.Result);
						}
						if ((PendingCardAction)pendingCard.Action == PendingCardAction.Edit)
						{
							Processor.DeleteCard(card, getAccessTemplateOperationResult.Result);
							cardWriter = Processor.AddCard(card, getAccessTemplateOperationResult.Result);
						}
						if ((PendingCardAction)pendingCard.Action == PendingCardAction.Delete)
						{
							cardWriter = Processor.DeleteCard(card, getAccessTemplateOperationResult.Result);
						}
						if ((PendingCardAction)pendingCard.Action == PendingCardAction.ResetRepeatEnter)
						{
							List<Guid> doorsGuids = deviceProcessor.Device.Children.Where(x => x.DriverType == SKDDriverType.Reader && x.Door != null).Select(x => x.Door.UID).ToList();
							cardWriter = Processor.ResetRepeatEnter(card, doorsGuids);
						}

						foreach (var controllerCardItem in cardWriter.ControllerCardItems)
						{
							if (!controllerCardItem.HasError)
							{
								databaseService.CardTranslator.DeleteAllPendingCards(pendingCard.CardUID, deviceProcessor.Device.UID);
							}
						}
					}
					else
					{
						databaseService.CardTranslator.DeleteAllPendingCards(pendingCard.CardUID, deviceProcessor.Device.UID);
					}
				}
			}
		}

		public static void SetNewConfig()
		{
			Stop();
			Start();
		}
	}
}