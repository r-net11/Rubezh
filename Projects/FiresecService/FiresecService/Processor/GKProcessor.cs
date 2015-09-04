﻿using System;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using FiresecClient;
using GKProcessor;
using SKDDriver;
using System.Collections.Generic;

namespace FiresecService
{
	public static class GKProcessor
	{
		public static void Create()
		{
			GKProcessorManager.GKProgressCallbackEvent -= new Action<GKProgressCallback>(OnGKProgressCallbackEvent);
			GKProcessorManager.GKProgressCallbackEvent += new Action<GKProgressCallback>(OnGKProgressCallbackEvent);
			GKProcessorManager.GKCallbackResultEvent -= new Action<GKCallbackResult>(OnGKCallbackResultEvent);
			GKProcessorManager.GKCallbackResultEvent += new Action<GKCallbackResult>(OnGKCallbackResultEvent);
		}

		public static void Start()
		{
			GKProcessorManager.Start();
		}

		public static void Stop()
		{
			GKProcessorManager.Stop();
		}

		public static void SetNewConfig()
		{
			Stop();
			Create();
			Start();
		}

		static void OnGKProgressCallbackEvent(GKProgressCallback gkProgressCallback)
		{
			FiresecService.Service.FiresecService.NotifyGKProgress(gkProgressCallback);
		}

		static void OnGKCallbackResultEvent(GKCallbackResult gkCallbackResult)
		{
			CheckPendingCards(gkCallbackResult);

			if (gkCallbackResult.JournalItems.Count > 0)
			{
				foreach (var journalItem in gkCallbackResult.JournalItems)
				{
					ProcessPassJournal(journalItem);
					FiresecService.Service.FiresecService.AddCommonJournalItem(journalItem);
				}
			}

			foreach (var doorState in gkCallbackResult.GKStates.DoorStates)
			{
				if (doorState.Door != null)
				{
					var enterZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == doorState.Door.EnterZoneUID);
					if (enterZone != null)
					{
						GKProcessorManager.CalculateSKDZone(enterZone);
						if (!gkCallbackResult.GKStates.SKDZoneStates.Any(x => x.UID == enterZone.UID))
						{
							gkCallbackResult.GKStates.SKDZoneStates.Add(enterZone.State);
						}
					}
					var exitZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == doorState.Door.ExitZoneUID);
					if (exitZone != null)
					{
						GKProcessorManager.CalculateSKDZone(exitZone);
						if (!gkCallbackResult.GKStates.SKDZoneStates.Any(x => x.UID == exitZone.UID))
						{
							gkCallbackResult.GKStates.SKDZoneStates.Add(exitZone.State);
						}
					}
				}
			}
			FiresecService.Service.FiresecService.NotifyGKObjectStateChanged(gkCallbackResult);

			ProcedureRunner.RunOnStateChanged();
		}

		static void ProcessPassJournal(JournalItem journalItem)
		{
			if (journalItem.JournalEventNameType == JournalEventNameType.Проход_пользователя_разрешен)
			{
				Guid? zoneUID = null;
				var door = GKManager.Doors.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
				if (door != null)
				{
					if (journalItem.JournalEventDescriptionType == JournalEventDescriptionType.Вход_Глобал)
					{
						zoneUID = door.EnterZoneUID;
					}
					else if (journalItem.JournalEventDescriptionType == JournalEventDescriptionType.Выход_Глобал)
					{
						zoneUID = door.ExitZoneUID;
					}
				}

				if (zoneUID.HasValue)
				{
					using (var dbService = new SKDDriver.DataClasses.DbService())
					{
						dbService.PassJournalTranslator.AddPassJournal(journalItem.EmployeeUID, zoneUID.Value);
					}
				}
			}
		}

		static void CheckPendingCards(GKCallbackResult gkCallbackResult)
		{
			foreach (var journalItem in gkCallbackResult.JournalItems)
			{
				if (journalItem.JournalEventNameType == JournalEventNameType.Восстановление_связи_с_прибором || journalItem.JournalEventNameType == JournalEventNameType.Начало_мониторинга)
				{
					var device = GKManager.Devices.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
					{
						using (var databaseService = new SKDDriver.DataClasses.DbService())
						{
							if (device != null)
							{
								var pendingCards = databaseService.CardTranslator.GetAllPendingCards(device.UID);
								if (pendingCards == null)
									return;
								foreach (var pendingCard in pendingCards)
								{
									var operationResult = databaseService.CardTranslator.GetSingle(pendingCard.CardUID);
									if (!operationResult.HasError && operationResult.Result != null)
									{
										var card = operationResult.Result;
										var getAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(card.AccessTemplateUID);
										var employeeOperationResult = databaseService.EmployeeTranslator.GetSingle(card.EmployeeUID);
										var employeeName = employeeOperationResult.Result != null ? employeeOperationResult.Result.FIO : "";

										if ((PendingCardAction)pendingCard.Action == PendingCardAction.Delete)
										{
											var result = GKSKDHelper.RemoveCard(device, card, databaseService);
											if (!result.HasError)
											{
												databaseService.CardTranslator.DeleteAllPendingCards(pendingCard.CardUID, device.UID);
											}
										}
										if (!getAccessTemplateOperationResult.HasError)
										{
											var accessTemplateDoors = getAccessTemplateOperationResult.Result != null ? getAccessTemplateOperationResult.Result.CardDoors : new List<CardDoor>();
											var controllerCardSchedules = GKSKDHelper.GetGKControllerCardSchedules(card, accessTemplateDoors);
											foreach (var controllerCardSchedule in controllerCardSchedules)
											{
												switch ((PendingCardAction)pendingCard.Action)
												{
													case PendingCardAction.Add:
													case PendingCardAction.Edit:
														var result = GKSKDHelper.AddOrEditCard(controllerCardSchedule, card, employeeName, dbService: databaseService);
														if (!result.HasError)
														{
															databaseService.CardTranslator.DeleteAllPendingCards(pendingCard.CardUID, device.UID);
														}
														break;
												}
											}
										}
									}
									else
									{
										databaseService.CardTranslator.DeleteAllPendingCards(pendingCard.CardUID, device.UID);
									}
								}
							}
						}
					}
				}
			}
		}
	}
}