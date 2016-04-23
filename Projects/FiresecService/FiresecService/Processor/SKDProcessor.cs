using ChinaSKDDriver;
using Common;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using FiresecAPI.SKD.Device;
using SKDDriver;
using SKDDriver.Translators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiresecService
{
	public static class SKDProcessor
	{
		static SKDProcessor()
		{
#if DEBUG
			try
			{
				System.IO.File.Copy(@"..\..\..\ChinaController\CPPWrapper\Bin\CPPWrapper.dll", @"CPPWrapper.dll", true);
			}
			catch { }
#endif
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
					deviceProcessor.NewJournalItem -= new Action<JournalItem>(OnNewJournalItem);
					deviceProcessor.NewJournalItem += new Action<JournalItem>(OnNewJournalItem);

					deviceProcessor.ConnectionAppeared -= new Action<DeviceProcessor>(OnConnectionAppeared);
					deviceProcessor.ConnectionAppeared += new Action<DeviceProcessor>(OnConnectionAppeared);
				}

				Processor.NewJournalItem -= new Action<JournalItem>(OnNewJournalItem);
				Processor.NewJournalItem += new Action<JournalItem>(OnNewJournalItem);

				Processor.StatesChangedEvent -= new Action<SKDStates>(OnSKDStates);
				Processor.StatesChangedEvent += new Action<SKDStates>(OnSKDStates);

				Processor.SKDProgressCallbackEvent -= new Action<SKDProgressCallback>(OnSKDProgressCallbackEvent);
				Processor.SKDProgressCallbackEvent += new Action<SKDProgressCallback>(OnSKDProgressCallbackEvent);

				// События функционала автопоиска контроллеров в сети
				Processor.NewSearchDevice -= new Action<SKDDeviceSearchInfo>(OnNewSearchDevice);
				Processor.NewSearchDevice += new Action<SKDDeviceSearchInfo>(OnNewSearchDevice);
			}
			catch (Exception e)
			{
				Logger.Error(e, "SKDProcessor.Start");
			}
		}

		public static void Stop()
		{
			ChinaSKDDriver.Processor.Stop();
		}

		private static void OnNewJournalItem(JournalItem journalItem)
		{
			var cardNo = 0;
			var journalDetalisationItem = journalItem.JournalDetalisationItems.FirstOrDefault(x => x.Name == "Номер карты");
			if (journalDetalisationItem != null)
			{
				var cardNoString = journalDetalisationItem.Value;
				Int32.TryParse(cardNoString, System.Globalization.NumberStyles.HexNumber, null, out cardNo);
				//journalItem.JournalDetalisationItems.Remove(journalDetalisationItem);
			}

			if (cardNo > 0)
			{
				using (var databaseService = new SKDDatabaseService())
				{
					var operationResult = databaseService.CardTranslator.GetEmployeeByCardNo(cardNo);
					if (!operationResult.HasError)
					{
						var employeeUID = operationResult.Result;
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
						if (journalItem.JournalItemType == JournalItemType.Online && journalItem.JournalEventNameType == JournalEventNameType.Проход_разрешен)
						{
							var readerdevice = SKDManager.Devices.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
							if (readerdevice != null && readerdevice.Zone != null)
							{
								var zoneUID = readerdevice.Zone.UID;
								using (var passJournalTranslator = new PassJournalTranslator())
								{
									passJournalTranslator.AddPassJournal(employeeUID, zoneUID);
								}
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
			FiresecService.Service.FiresecService.NotifyNewSearchDevices(new List<SKDDeviceSearchInfo>() { skdDeviceSearchInfo });
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
						zone.State.StateClass = zone.State.StateClasses.Min();
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
						else
						{
							// TODO: Вроде ничего больше не нужно делать
						}
					}
				}
			}

			Service.FiresecService.NotifySKDObjectStateChanged(skdStates);
			ProcedureRunner.RunOnStateChanged();
		}

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
			foreach (var zone in SKDManager.Zones)
			{
				zone.State.StateClasses = GetZoneStateClasses(zone);
				zone.State.StateClass = zone.State.StateClasses.Min();
				skdStates.ZoneStates.Add(zone.State);
			}
			foreach (var door in SKDManager.Doors)
			{
				door.State.StateClasses = GetDoorStateClasses(door);
				door.State.StateClass = door.State.StateClasses.Min();
				skdStates.DoorStates.Add(door.State);
			}
			return skdStates;
		}

		private static void OnSKDProgressCallbackEvent(SKDProgressCallback SKDProgressCallback)
		{
			FiresecService.Service.FiresecService.NotifySKDProgress(SKDProgressCallback);
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
							cardWriter = ChinaSKDDriver.Processor.AddCard(card, getAccessTemplateOperationResult.Result);
						}
						if ((PendingCardAction)pendingCard.Action == PendingCardAction.Edit)
						{
							cardWriter = ChinaSKDDriver.Processor.DeleteCard(card, getAccessTemplateOperationResult.Result);
							cardWriter = ChinaSKDDriver.Processor.AddCard(card, getAccessTemplateOperationResult.Result);
						}
						if ((PendingCardAction)pendingCard.Action == PendingCardAction.Delete)
						{
							cardWriter = ChinaSKDDriver.Processor.DeleteCard(card, getAccessTemplateOperationResult.Result);
						}
						if ((PendingCardAction) pendingCard.Action == PendingCardAction.ResetRepeatEnter)
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