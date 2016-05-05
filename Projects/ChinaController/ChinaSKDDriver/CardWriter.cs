using ChinaSKDDriverAPI;
using FiresecAPI;
using FiresecAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChinaSKDDriver
{
	public class CardWriter
	{
		public List<ControllerCardItem> ControllerCardItems { get; private set; }

		public CardWriter()
		{
			ControllerCardItems = new List<ControllerCardItem>();
		}

		public void AddCard(SKDCard skdCard, AccessTemplate accessTemplate)
		{
			ControllerCardItems = Create_ControllerCardItems_ToAdd(skdCard, accessTemplate);
			ProcessControllerCardItems(ControllerCardItems, false);
		}

		public void ResetRepeatEnter(SKDCard card, List<Guid> doorGuids)
		{
			ControllerCardItems = Create_ControllerCardItems_ToResetRepeatEnter(card, doorGuids);
			ProcessControllerCardItems(ControllerCardItems, false);
		}

		private List<CardWriter.ControllerCardItem> Create_ControllerCardItems_ToResetRepeatEnter(SKDCard card, List<Guid> doorGuids)
		{
			var controllerCardItems = new List<ControllerCardItem>();

			var controllerDevices = new HashSet<SKDDevice>();
			foreach (var doorGuid in doorGuids)
			{
				var door = SKDManager.SKDConfiguration.Doors.FirstOrDefault(x => x.UID == doorGuid);
				if (door == null) continue;

				var readerDevice = SKDManager.SKDConfiguration.Devices.FirstOrDefault(x => x.UID == door.InDeviceUID);

				if (readerDevice == null || readerDevice.Parent == null) continue;
				if (!controllerDevices.Add(readerDevice.Parent)) continue;

				var controllerCardItem = new ControllerCardItem
				{
					Card = card,
					ControllerDevice = readerDevice.Parent,
					ActionType = ControllerCardItem.ActionTypeEnum.ResetRepeatEnter
				};
				controllerCardItems.Add(controllerCardItem);
			}
			return controllerCardItems;
		}

		private List<CardWriter.ControllerCardItem> Create_ControllerCardItems_ToAdd(SKDCard skdCard, AccessTemplate accessTemplate)
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
					Add(skdCard, controllerCardItems, door.InDeviceUID, cardDoor.EnterScheduleNo);
				}
			}
			return controllerCardItems;
		}

		private void Add(SKDCard card, List<ControllerCardItem> controllerCardItems, Guid readerUID, int intervalID)
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

		private List<ControllerCardItem> Create_ControllerCardItems_ToDelete(SKDCard skdCard, AccessTemplate accessTemplate)
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

		/// <summary>
		/// Перезаписывает на контроллер все пропуска
		/// </summary>
		/// <param name="device">Контроллер, на который перезаписываются пропуска</param>
		/// <param name="cards">Перезаписываемые пропуска</param>
		/// <param name="accessTemplates">Шаблоны доступа к перезаписываемым пропускам</param>
		/// <returns>Список ошибок при выполнении операции</returns>
		public List<string> RewriteAllCards(SKDDevice device, IEnumerable<SKDCard> cards, IEnumerable<AccessTemplate> accessTemplates, bool doProgress = true)
		{
			SKDProgressCallback progressCallback = null;

			// Показываем индикатор выполнения операции
			if (doProgress)
                progressCallback = Processor.StartProgress(String.Format(Resources.Language.CardWriter.RewriteAllCards_progressCallback, device.Name), "", cards.Count(), true, SKDProgressClientType.Administrator);

			var errors = new List<string>();
			foreach (var card in cards)
			{
				AccessTemplate accessTemplate = null;
				if (card.AccessTemplateUID != null)
				{
					accessTemplate = accessTemplates.FirstOrDefault(x => x.UID == card.AccessTemplateUID);
				}

				ControllerCardItems = new List<ControllerCardItem>();
				var controllerCardItems = Create_ControllerCardItems_ToAdd(card, accessTemplate);
				foreach (var controllerCardItem in controllerCardItems)
				{
					if (controllerCardItem.ControllerDevice.UID == device.UID)
					{
						ControllerCardItems.Add(controllerCardItem);
					}
				}

				// Пользователь отменил операцию
				if (progressCallback != null && progressCallback.IsCanceled)
					return new List<string> { Resources.Language.CardWriter.RewriteAllCards_progressCallback_Cancel };

				// Обновляем индикатор выполнения операции
				if (progressCallback != null)
					Processor.DoProgress(null, progressCallback);

				ProcessControllerCardItems(ControllerCardItems, true);

				foreach (var controllerCardItem in ControllerCardItems)
				{
					if (controllerCardItem.HasError)
					{
						errors.Add(string.Format(Resources.Language.CardWriter.RewriteAllCards_progressCallback_Error,controllerCardItem.Card.Number,device.Name));
					}
				}
			}

			// Останавливаем индикатор выполнения операции
			if (progressCallback != null)
				Processor.StopProgress(progressCallback);

			return errors;
		}

		private void ProcessControllerCardItems(List<ControllerCardItem> controllerCardItems, bool showProgress)
		{
			foreach (var controllerCardItem in controllerCardItems)
			{
				var deviceProcessor = Processor.DeviceProcessors.FirstOrDefault(x => x.Device.UID == controllerCardItem.ControllerDevice.UID);
				if (deviceProcessor != null)
				{
					var card = new Card
					{
						CardNo = controllerCardItem.Card.Number.GetValueOrDefault().ToString("X"),
						ValidStartDateTime = controllerCardItem.Card.StartDate,
						ValidEndDateTime = controllerCardItem.Card.EndDate,
						UserTime = controllerCardItem.Card.UserTime,
						Password = controllerCardItem.Card.Password,
						IsHandicappedCard = controllerCardItem.Card.IsHandicappedCard
					};

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

						case FiresecAPI.SKD.CardType.Guest:
							card.CardType = ChinaSKDDriverAPI.CardType.NET_ACCESSCTLCARD_TYPE_GENERAL;
							card.CardStatus = CardStatus.NET_ACCESSCTLCARD_STATE_NORMAL;
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
						if (readerDevice != null || readerDevice.Parent != null)
						{
							if (readerDevice.Parent.DoorType == DoorType.OneWay)
							{
								card.Doors.Add(readerDevice.IntAddress);
							}
							else
							{
								card.Doors.Add(readerDevice.IntAddress / 2);
							}
							card.TimeSections.Add(readerIntervalItem.WeeklyIntervalID);
						}
					}

					var result = false;

					switch (controllerCardItem.ActionType)
					{
						case ControllerCardItem.ActionTypeEnum.Add:
							var cardRecordNo = deviceProcessor.Wrapper.AddCard(card);
							result = cardRecordNo == controllerCardItem.Card.Number.ToString();
							if (!result)
							{
								result = deviceProcessor.Wrapper.RemoveCard((int)controllerCardItem.Card.Number);
								if (result)
								{
									cardRecordNo = deviceProcessor.Wrapper.AddCard(card);
									result = cardRecordNo == controllerCardItem.Card.Number.ToString();
								}
							}
							break;

						case ControllerCardItem.ActionTypeEnum.Edit:
							result = deviceProcessor.Wrapper.EditCard(card);
							if (!result)
							{
								cardRecordNo = deviceProcessor.Wrapper.AddCard(card);
								result = cardRecordNo == controllerCardItem.Card.Number.ToString();
							}
							break;

						case ControllerCardItem.ActionTypeEnum.Delete:
							result = deviceProcessor.Wrapper.RemoveCard((int)controllerCardItem.Card.Number);
							break;
						case ControllerCardItem.ActionTypeEnum.ResetRepeatEnter:
							var cardInfo = deviceProcessor.Wrapper.GetCardInfo((int) controllerCardItem.Card.Number);

							result = deviceProcessor.Wrapper.ResetRepeatEnter(controllerCardItem.Card.Number.GetValueOrDefault().ToString("X"));
							if (cardInfo == null)
							{
								controllerCardItem.Error = string.Format(Resources.Language.CardWriter.ProcessControllerCardItems_controllerCardItemActionType_ResetRepeatEnter_Error, deviceProcessor.Device.Name);
							}
							break;
					}

					if (!result && controllerCardItem.ActionType != ControllerCardItem.ActionTypeEnum.ResetRepeatEnter)
					{
						var operationName = "";
						if (controllerCardItem.ActionType == ControllerCardItem.ActionTypeEnum.Add)
							operationName = Resources.Language.CardWriter.ProcessControllerCardItems_controllerCardItemActionType_Add;
						if (controllerCardItem.ActionType == ControllerCardItem.ActionTypeEnum.Edit)
                            operationName = Resources.Language.CardWriter.ProcessControllerCardItems_controllerCardItemActionType_Edit;
						if (controllerCardItem.ActionType == ControllerCardItem.ActionTypeEnum.Delete)
                            operationName = Resources.Language.CardWriter.ProcessControllerCardItems_controllerCardItemActionType_Delete;
						//if (controllerCardItem.ActionType == ControllerCardItem.ActionTypeEnum.ResetRepeatEnter)
						//	operationName = "сброса ограничения на повторный проход";

						controllerCardItem.Error = string.Format(Resources.Language.CardWriter.ProcessControllerCardItems_controllerCardItemActionType_Error,
                                                                    operationName, controllerCardItem.Card.Number, deviceProcessor.Device.Name);
					}
				}
				else
				{
					controllerCardItem.Error = Resources.Language.CardWriter.ProcessControllerCardItems_controllerCardItem_Error;
				}
			}
		}

		public string GetError()
		{
			var stringBuilder = new StringBuilder();
			foreach (var controllerCardItem in ControllerCardItems)
			{
				if (controllerCardItem.HasError)
				{
					stringBuilder.AppendLine(controllerCardItem.Error);
				}
			}
			return stringBuilder.ToString();
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
				Delete,
				ResetRepeatEnter
			}
		}

		public class ReaderIntervalItem
		{
			public Guid ReaderUID { get; set; }

			public int WeeklyIntervalID { get; set; }
		}
	}
}