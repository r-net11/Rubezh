using System;
using System.Linq;
using System.Collections.Generic;
using Common;
using FiresecAPI;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using FiresecAPI.GK;
using ChinaSKDDriver;
using SKDDriver;

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
		}

		public static void Start()
		{
			try
			{
				if (SKDManager.SKDConfiguration != null)
				{
					SKDManager.CreateDrivers();
					SKDManager.UpdateConfiguration();
				}
				ChinaSKDDriver.Processor.Start();
				foreach (var deviceProcessor in ChinaSKDDriver.Processor.DeviceProcessors)
				{
					deviceProcessor.NewJournalItem -= new Action<JournalItem>(OnNewJournalItem);
					deviceProcessor.NewJournalItem += new Action<JournalItem>(OnNewJournalItem);

					deviceProcessor.ConnectionAppeared -= new Action<DeviceProcessor>(OnConnectionAppeared);
					deviceProcessor.ConnectionAppeared += new Action<DeviceProcessor>(OnConnectionAppeared);
				}

				ChinaSKDDriver.Processor.NewJournalItem -= new Action<JournalItem>(OnNewJournalItem);
				ChinaSKDDriver.Processor.NewJournalItem += new Action<JournalItem>(OnNewJournalItem);

				ChinaSKDDriver.Processor.StatesChangedEvent -= new Action<SKDStates>(OnSKDStates);
				ChinaSKDDriver.Processor.StatesChangedEvent += new Action<SKDStates>(OnSKDStates);

				ChinaSKDDriver.Processor.GKProgressCallbackEvent -= new Action<GKProgressCallback>(OnGKProgressCallbackEvent);
				ChinaSKDDriver.Processor.GKProgressCallbackEvent += new Action<GKProgressCallback>(OnGKProgressCallbackEvent);
			}
			catch (Exception e)
			{
				Logger.Error(e, "SKDProcessor.Create");
			}
		}

		public static void Stop()
		{
			ChinaSKDDriver.Processor.Stop();
		}

		static void OnNewJournalItem(JournalItem journalItem)
		{
			if (journalItem.CardNo > 0)
			{
				var operationResult = SKDDatabaseService.CardTranslator.GetEmployeeByCardNo(journalItem.CardNo);
				if (!operationResult.HasError)
				{
					var employeeUID = operationResult.Result;
					journalItem.EmployeeUID = employeeUID;

					if (journalItem.JournalEventNameType == JournalEventNameType.Проход_разрешен)
					{
						var readerdevice = SKDManager.Devices.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
						if (readerdevice != null && readerdevice.Zone != null)
						{
							var zoneUID = readerdevice.Zone.UID;
							SKDDatabaseService.EmployeeTranslator.AddPassJournal(employeeUID, zoneUID);
						}
					}
				}
			}

			journalItem.StateClass = EventDescriptionAttributeHelper.ToStateClass(journalItem.JournalEventNameType);
			journalItem.JournalSubsystemType = EventDescriptionAttributeHelper.ToSubsystem(journalItem.JournalEventNameType);
			FiresecService.Service.FiresecService.AddCommonJournalItem(journalItem);
		}

		static void OnSKDStates(SKDStates skdStates)
		{
			if (skdStates.DeviceStates.Count > 0)
			{
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
						skdStates.DoorStates.Add(door.State);
					}
				}
			}

			FiresecService.Service.FiresecService.NotifySKDObjectStateChanged(skdStates);
		}

		static List<XStateClass> GetZoneStateClasses(SKDZone zone)
		{
			var stateClasses = new List<XStateClass>();
			foreach (var readerDevice in zone.Devices)
			{
				var lockDevice = readerDevice.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == readerDevice.IntAddress / 2);
				if (lockDevice != null)
				{
					if (!stateClasses.Contains(lockDevice.State.StateClass))
						stateClasses.Add(lockDevice.State.StateClass);
				}
			}
			stateClasses.Sort();
			if (stateClasses.Count == 0)
				stateClasses.Add(XStateClass.Norm);
			return stateClasses;
		}

		static List<XStateClass> GetDoorStateClasses(SKDDoor door)
		{
			var stateClasses = new List<XStateClass>();

			if (door.InDevice != null)
			{
				var lockDevice = door.InDevice.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == door.InDevice.IntAddress / 2);
				if (lockDevice != null)
				{
					if (!stateClasses.Contains(lockDevice.State.StateClass))
						stateClasses.Add(lockDevice.State.StateClass);
				}
			}

			stateClasses.Sort();
			if (stateClasses.Count == 0)
				stateClasses.Add(XStateClass.Norm);
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

		static void OnGKProgressCallbackEvent(GKProgressCallback gkProgressCallback)
		{
			FiresecService.Service.FiresecService.NotifyGKProgress(gkProgressCallback);
		}

		static void OnConnectionAppeared(DeviceProcessor deviceProcessor)
		{
			var pendingCards = SKDDatabaseService.CardTranslator.GetAllPendingCards(deviceProcessor.Device.UID);
			foreach (var pendingCard in pendingCards)
			{
				var operationResult = SKDDatabaseService.CardTranslator.GetByUID(pendingCard.CardUID);
				if (!operationResult.HasError)
				{
					var card = operationResult.Result;
					var accessTemplate = GetAccessTemplate(card.AccessTemplateUID);
					var cardWriter = ChinaSKDDriver.Processor.AddCard(card, accessTemplate);
					foreach (var controllerCardItem in cardWriter.ControllerCardItems)
					{
						if (!controllerCardItem.HasError)
						{
							SKDDatabaseService.CardTranslator.DeleteAllPendingCards(pendingCard.CardUID, deviceProcessor.Device.UID);
						}
					}
				}
				else
				{
					SKDDatabaseService.CardTranslator.DeleteAllPendingCards(pendingCard.CardUID, deviceProcessor.Device.UID);
				}
			}
		}

		static AccessTemplate GetAccessTemplate(Guid? uid)
		{
			var accessTemplateOperationResult = SKDDatabaseService.AccessTemplateTranslator.GetSingle(uid);
			if (!accessTemplateOperationResult.HasError)
				return accessTemplateOperationResult.Result;
			return null;
		}

		public static void SetNewConfig()
		{
			Stop();
			Start();
		}
	}
}