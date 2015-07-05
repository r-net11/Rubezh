using System;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using GKProcessor;
using SKDDriver;
using SKDDriver.Translators;

namespace FiresecService
{
	public static class GKProcessor
	{
		//public static void Create()
		//{
		//	GKProcessorManager.SKDProgressCallbackEvent -= new Action<SKDProgressCallback>(OnSKDProgressCallbackEvent);
		//	GKProcessorManager.SKDProgressCallbackEvent += new Action<SKDProgressCallback>(OnSKDProgressCallbackEvent);
		//	GKProcessorManager.SKDCallbackResultEvent -= new Action<SKDCallbackResult>(OnSKDCallbackResultEvent);
		//	GKProcessorManager.SKDCallbackResultEvent += new Action<SKDCallbackResult>(OnSKDCallbackResultEvent);
		//}

		//public static void Start()
		//{
			// DeviceLibraryConfigurationPatchHelper.Patch();
		//	GKProcessorManager.MustMonitor = true;
		//}
		//public static void SetNewConfig()
	//	{
		//	Create();
			//Start();
	//	}

		//static void OnSKDProgressCallbackEvent(SKDProgressCallback SKDProgressCallback)
		//{
		//	FiresecService.Service.FiresecService.NotifyGKProgress(SKDProgressCallback);
		//}

		//static void OnSKDCallbackResultEvent(SKDCallbackResult SKDCallbackResult)
		//{
		//	CheckPendingCards(SKDCallbackResult);

		//	if (SKDCallbackResult.JournalItems.Count > 0)
		//	{
		//		foreach (var journalItem in SKDCallbackResult.JournalItems)
		//		{
		//			ProcessPassJournal(journalItem);
		//			FiresecService.Service.FiresecService.AddCommonJournalItem(journalItem);
		//		}
		//	}
		//	FiresecService.Service.FiresecService.NotifyGKObjectStateChanged(SKDCallbackResult);

		//	ProcedureRunner.RunOnStateChanged();
		//}

		//static void ProcessPassJournal(JournalItem journalItem)
		//{
		//	if (journalItem.JournalEventNameType == JournalEventNameType.Проход_пользователя_разрешен)
		//	{
		//		Guid? zoneUID = null;

		//		if (zoneUID.HasValue)
		//		{
		//			using (var passJournalTranslator = new PassJournalTranslator())
		//			{
		//				passJournalTranslator.AddPassJournal(journalItem.EmployeeUID, zoneUID.Value);
		//			}
		//		}
		//	}
		//}

		//static void CheckPendingCards(SKDCallbackResult SKDCallbackResult)
		//{
		//	foreach (var deviceState in SKDCallbackResult.GKStates.DeviceStates)
		//	{
		//		if (deviceState.Device.DriverType == GKDriverType.GK && !deviceState.StateClasses.Contains(XStateClass.Unknown) && !deviceState.StateClasses.Contains(XStateClass.ConnectionLost))
		//		{
		//			using (var databaseService = new SKDDatabaseService())
		//			{
		//				var pendingCards = databaseService.CardTranslator.GetAllPendingCards(deviceState.Device.UID);
		//				foreach (var pendingCard in pendingCards)
		//				{
		//					var operationResult = databaseService.CardTranslator.GetSingle(pendingCard.CardUID);
		//					if (!operationResult.HasError && operationResult.Result != null)
		//					{
		//						var card = operationResult.Result;
		//						var getAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(card.AccessTemplateUID);
		//						var employeeOperationResult = databaseService.EmployeeTranslator.GetSingle(card.HolderUID);
		//						var employeeName = employeeOperationResult.Result != null ? employeeOperationResult.Result.FIO : "";

		//						if ((PendingCardAction)pendingCard.Action == PendingCardAction.Delete)
		//						{
		//							var result = GKSKDHelper.RemoveCard(deviceState.Device, card);
		//							if (!result.HasError)
		//							{
		//								databaseService.CardTranslator.DeleteAllPendingCards(pendingCard.CardUID, deviceState.Device.UID);
		//							}
		//						}
		//						var controllerCardSchedules = GKSKDHelper.GetGKControllerCardSchedules(card, getAccessTemplateOperationResult.Result);
		//						foreach (var controllerCardSchedule in controllerCardSchedules)
		//						{
		//							var result = new OperationResult<bool>(false);
		//							switch ((PendingCardAction)pendingCard.Action)
		//							{
		//								case PendingCardAction.Add:
		//									result = GKSKDHelper.AddOrEditCard(controllerCardSchedule, card, employeeName);
		//									break;

		//								case PendingCardAction.Edit:
		//									result = GKSKDHelper.AddOrEditCard(controllerCardSchedule, card, employeeName);
		//									if (!result.HasError)
		//									{
		//										result = GKSKDHelper.RemoveCard(deviceState.Device, card);
		//									}
		//									break;
		//							}
		//							if (!result.HasError)
		//							{
		//								databaseService.CardTranslator.DeleteAllPendingCards(pendingCard.CardUID, deviceState.Device.UID);
		//							}
		//						}
		//					}
		//					else
		//					{
		//						databaseService.CardTranslator.DeleteAllPendingCards(pendingCard.CardUID, deviceState.Device.UID);
		//					}
		//				}
		//			}
		//		}
		//	}
		//}
	}
}