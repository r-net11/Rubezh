using System;
using System.Collections.Generic;
using System.Linq;
using ChinaSKDDriverAPI;
using FiresecAPI.SKD;
using FiresecAPI;

namespace ChinaSKDDriver
{
	public class CardWriter
	{
		public List<ControllerCardItem> ControllerCardItems { get; private set; }

		public void AddCard(SKDCard skdCard, AccessTemplate accessTemplate)
		{
			ControllerCardItems = Create_ControllerCardItems_ToAdd(skdCard, accessTemplate);
			ProcessControllerCardItems(ControllerCardItems, false);
		}

		List<ControllerCardItem> Create_ControllerCardItems_ToAdd(SKDCard skdCard, AccessTemplate accessTemplate)
		{
			var controllerCardItems = new List<ControllerCardItem>();
			var cardDoors = new List<CardDoor>();
			if (accessTemplate != null)
			{
				foreach (var cardDoor in accessTemplate.CardDoors)
				{
					cardDoors.Add(cardDoor);
				}
			}
			foreach (var cardDoor in skdCard.CardDoors)
			{
				cardDoors.Add(cardDoor);
			}

			foreach (var cardDoor in cardDoors)
			{
				var door = SKDManager.SKDConfiguration.Doors.FirstOrDefault(x => x.UID == cardDoor.DoorUID);
				if (door != null)
				{
					Add(skdCard, controllerCardItems, door.InDeviceUID, cardDoor.EnterIntervalID);
					if (door.OutDevice != null && door.OutDevice.DriverType == SKDDriverType.Reader)
					{
						Add(skdCard, controllerCardItems, door.OutDeviceUID, cardDoor.ExitIntervalID);
					}
				}
			}
			return controllerCardItems;
		}

		void Add(SKDCard card, List<ControllerCardItem> controllerCardItems, Guid readerUID, int intervalID)
		{
			var readerDevice = SKDManager.SKDConfiguration.Devices.FirstOrDefault(x => x.UID == readerUID);
			if (readerDevice != null && readerDevice.Parent != null)
			{
				var controllerDevice = readerDevice.Parent;
				var controllerCardItem = controllerCardItems.FirstOrDefault(x => x.ControllerDevice.UID == controllerDevice.UID);
				if (controllerCardItem == null)
				{
					controllerCardItem = new ControllerCardItem();
					controllerCardItem.Card = card;
					controllerCardItem.ControllerDevice = controllerDevice;
					controllerCardItem.ActionType = ControllerCardItem.ActionTypeEnum.Add;
					controllerCardItems.Add(controllerCardItem);
				}
				var readerIntervalItem = controllerCardItem.ReaderIntervalItems.FirstOrDefault(x => x.ReaderUID == readerUID);
				if (readerIntervalItem == null)
				{
					readerIntervalItem = new ReaderIntervalItem();
					readerIntervalItem.ReaderUID = readerUID;
					controllerCardItem.ReaderIntervalItems.Add(readerIntervalItem);
				}
				readerIntervalItem.WeeklyIntervalID = intervalID;
			}
		}

		public void DeleteCard(SKDCard skdCard, AccessTemplate accessTemplate)
		{
			ControllerCardItems = Create_ControllerCardItems_ToDelete(skdCard, accessTemplate);
			ProcessControllerCardItems(ControllerCardItems, false);
		}

		List<ControllerCardItem> Create_ControllerCardItems_ToDelete(SKDCard skdCard, AccessTemplate accessTemplate)
		{
			var controllerCardItems = new List<ControllerCardItem>();
			var cardDoors = new List<CardDoor>();
			if (accessTemplate != null)
			{
				foreach (var cardDoor in accessTemplate.CardDoors)
				{
					cardDoors.Add(cardDoor);
				}
			}
			foreach (var cardDoor in skdCard.CardDoors)
			{
				cardDoors.Add(cardDoor);
			}

			var controllerDevices = new HashSet<SKDDevice>();
			foreach (var cardDoor in cardDoors)
			{
				var door = SKDManager.SKDConfiguration.Doors.FirstOrDefault(x => x.UID == cardDoor.DoorUID);
				if (door != null)
				{
					var readerDevice = SKDManager.SKDConfiguration.Devices.FirstOrDefault(x => x.UID == door.InDeviceUID);
					if (readerDevice != null && readerDevice.Parent != null)
					{
						if (controllerDevices.Add(readerDevice.Parent))
						{
							var controllerCardItem = new ControllerCardItem();
							controllerCardItem.Card = skdCard;
							controllerCardItem.ControllerDevice = readerDevice.Parent;
							controllerCardItem.ActionType = ControllerCardItem.ActionTypeEnum.Delete;
							controllerCardItems.Add(controllerCardItem);
						}
					}
				}
			}
			return controllerCardItems;
		}

		public void EditCard(SKDCard oldCard, AccessTemplate oldAccessTemplate, SKDCard newCard, AccessTemplate newAccessTemplate)
		{
			var controllerCardItems_ToDelete = Create_ControllerCardItems_ToDelete(oldCard, oldAccessTemplate);
			var controllerCardItems_ToEdit = Create_ControllerCardItems_ToAdd(newCard, newAccessTemplate);
			foreach (var controllerCardItem_ToEdit in controllerCardItems_ToEdit)
			{
				controllerCardItem_ToEdit.ActionType = ControllerCardItem.ActionTypeEnum.Edit;
				controllerCardItems_ToDelete.RemoveAll(x => x.ControllerDevice.UID == controllerCardItem_ToEdit.ControllerDevice.UID);
			}
			ControllerCardItems = controllerCardItems_ToDelete;
			ControllerCardItems.AddRange(controllerCardItems_ToEdit);
			ProcessControllerCardItems(ControllerCardItems, false);
		}

		public bool RewriteAllCards(SKDDevice device, IEnumerable<SKDCard> cards, IEnumerable<AccessTemplate> accessTemplates)
		{
			var progressCallback = Processor.StartProgress("Запись всех карт в контроллер " + device.Name, "", cards.Count(), true, GKProgressClientType.Administrator);

			foreach (var card in cards)
			{
				AccessTemplate accessTemplate = null;
				if (card.AccessTemplateUID != null)
				{
					accessTemplate = accessTemplates.FirstOrDefault(x => x.UID == card.AccessTemplateUID);
				}

				ControllerCardItems = new List<ControllerCardItem>();
				var controllerCardItems = Create_ControllerCardItems_ToAdd(card, accessTemplate);
				var controllerCardItem = controllerCardItems.FirstOrDefault(x => x.ControllerDevice.UID == device.UID);
				if (controllerCardItem != null)
				{
					ControllerCardItems.Add(controllerCardItem);
				}

					if (progressCallback.IsCanceled)
						return false;
					Processor.DoProgress("Запись карты " + card.Number + " в контроллер " + device.Name, progressCallback);
				ProcessControllerCardItems(ControllerCardItems, true);
			}

			Processor.StopProgress(progressCallback);
			return true;
		}

		void ProcessControllerCardItems(List<ControllerCardItem> controllerCardItems, bool showProgress)
		{

			foreach (var controllerCardItem in controllerCardItems)
			{
				var deviceProcessor = Processor.DeviceProcessors.FirstOrDefault(x => x.Device.UID == controllerCardItem.ControllerDevice.UID);
				if (deviceProcessor != null)
				{
					var card = new Card();
					card.CardNo = controllerCardItem.Card.Number.ToString();
					card.ValidStartDateTime = controllerCardItem.Card.StartDate;
					card.ValidEndDateTime = controllerCardItem.Card.EndDate;
					switch (controllerCardItem.Card.CardType)
					{
						case FiresecAPI.SKD.CardType.Constant:
							card.CardType = ChinaSKDDriverAPI.CardType.NET_ACCESSCTLCARD_TYPE_GENERAL;
							card.CardStatus = CardStatus.NET_ACCESSCTLCARD_STATE_NORMAL;
							card.ValidEndDateTime = controllerCardItem.Card.StartDate.AddYears(100);
							break;

						case FiresecAPI.SKD.CardType.Temporary:
							card.CardType = ChinaSKDDriverAPI.CardType.NET_ACCESSCTLCARD_TYPE_GENERAL;
							card.CardStatus = CardStatus.NET_ACCESSCTLCARD_STATE_NORMAL;
							break;

						case FiresecAPI.SKD.CardType.OneTime:
							card.CardType = ChinaSKDDriverAPI.CardType.NET_ACCESSCTLCARD_TYPE_GENERAL;
							card.CardStatus = CardStatus.NET_ACCESSCTLCARD_STATE_NORMAL;
							card.ValidEndDateTime = controllerCardItem.Card.StartDate.AddDays(1);
							break;

						case FiresecAPI.SKD.CardType.Duress:
							card.CardType = ChinaSKDDriverAPI.CardType.NET_ACCESSCTLCARD_TYPE_CORCE;
							card.CardStatus = CardStatus.NET_ACCESSCTLCARD_STATE_NORMAL;
							break;

						case FiresecAPI.SKD.CardType.Blocked:
							card.CardType = ChinaSKDDriverAPI.CardType.NET_ACCESSCTLCARD_TYPE_GENERAL;
							card.CardStatus = CardStatus.NET_ACCESSCTLCARD_STATE_LOGOFF;
							break;
					}

					foreach (var readerIntervalItem in controllerCardItem.ReaderIntervalItems)
					{
						var readerDevice = SKDManager.SKDConfiguration.Devices.FirstOrDefault(x => x.UID == readerIntervalItem.ReaderUID);
						if (readerDevice != null)
						{
							card.Doors.Add(readerDevice.IntAddress);
							card.TimeSections.Add(readerIntervalItem.WeeklyIntervalID);
						}
					}

					var result = false;
					switch (controllerCardItem.ActionType)
					{
						case ControllerCardItem.ActionTypeEnum.Add:
							var cardRecordNo = deviceProcessor.Wrapper.AddCard(card);
							result = cardRecordNo >= controllerCardItem.Card.Number;
							break;

						case ControllerCardItem.ActionTypeEnum.Edit:
							result = deviceProcessor.Wrapper.EditCard(card);
							break;

						case ControllerCardItem.ActionTypeEnum.Delete:
							result = deviceProcessor.Wrapper.RemoveCard(controllerCardItem.Card.Number);
							break;
					}

					if (!result)
					{
						controllerCardItem.Error = "Ошибка при выполнении операции в приборе";
					}
				}
				else
				{
					controllerCardItem.Error = "Не найден контроллер в конфигурации";
				}
			}
		}

		public class ControllerCardItem
		{
			public ControllerCardItem()
			{
				ReaderIntervalItems = new List<ReaderIntervalItem>();
				Error = null;
			}

			public SKDCard Card { get; set; }
			public SKDDevice ControllerDevice { get; set; }
			public List<ReaderIntervalItem> ReaderIntervalItems { get; set; }
			public ActionTypeEnum ActionType { get; set; }
			
			public string Error { get; set; }
			public bool HasError
			{
				get { return Error != null; }
			}

			public enum ActionTypeEnum
			{
				Add,
				Edit,
				Delete
			}
		}

		public class ReaderIntervalItem
		{
			public Guid ReaderUID { get; set; }
			public int WeeklyIntervalID { get; set; }
		}
	}
}