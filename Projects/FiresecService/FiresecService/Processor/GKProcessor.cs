﻿using System;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecClient;
using GKProcessor;
using SKDDriver;
using ChinaSKDDriver;
using FiresecAPI.SKD;
using SKDDriver.Translators;

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
			// DeviceLibraryConfigurationPatchHelper.Patch();
			GKProcessorManager.MustMonitor = true;
			GKProcessorManager.Start();
			GKLicenseProcessor.Start();
		}

		public static void Stop()
		{
			GKProcessorManager.Stop();
		}

		public static void SetNewConfig()
		{
			var allHashesAreEqual = true;
			if (GKManager.DeviceConfiguration.RootDevice.Children.Count == GKManager.DeviceConfiguration.RootDevice.Children.Count)
			{
				for (int i = 0; i < GKManager.DeviceConfiguration.RootDevice.Children.Count; i++)
				{
					var hash1 = GKFileInfo.CreateHash1(GKManager.DeviceConfiguration, GKManager.DeviceConfiguration.RootDevice.Children[i]);
					var hash2 = GKFileInfo.CreateHash1(GKManager.DeviceConfiguration, GKManager.DeviceConfiguration.RootDevice.Children[i]);
					if (!GKFileInfo.CompareHashes(hash1, hash2))
					{
						allHashesAreEqual = false;
					}
				}
			}
			else
			{
				allHashesAreEqual = false;
			}

			//if (!allHashesAreEqual)
			{
				Stop();
				Create();
				Start();
			}
		}

		static void OnGKProgressCallbackEvent(GKProgressCallback gkProgressCallback)
		{
			FiresecService.Service.FiresecService.NotifyGKProgress(gkProgressCallback);
		}

		static void OnGKCallbackResultEvent(GKCallbackResult gkCallbackResult)
		{
			ChackPendingCards(gkCallbackResult);

			if (gkCallbackResult.JournalItems.Count > 0)
			{
				foreach (var journalItem in gkCallbackResult.JournalItems)
				{
					FiresecService.Service.FiresecService.AddCommonJournalItem(journalItem);

					if (journalItem.JournalEventNameType == JournalEventNameType.Проход_пользователя_разрешен)
					{
						var door = GKManager.Doors.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
						if (door != null)
						{
							if (door.EnterZoneUID != Guid.Empty)
							{
								using (var passJournalTranslator = new PassJournalTranslator())
								{
									passJournalTranslator.AddPassJournal(journalItem.EmployeeUID, door.EnterZoneUID);
								}
							}
						}
					}
				}
			}
			FiresecService.Service.FiresecService.NotifyGKObjectStateChanged(gkCallbackResult);

			ProcedureRunner.RunOnStateChanged();
		}

		static void ChackPendingCards(GKCallbackResult gkCallbackResult)
		{
			foreach (var deviceState in gkCallbackResult.GKStates.DeviceStates)
			{
				if (deviceState.Device.DriverType == GKDriverType.GK && !deviceState.StateClasses.Contains(XStateClass.Unknown) && !deviceState.StateClasses.Contains(XStateClass.ConnectionLost))
				{
					var gkSKDHelper = new GKSKDHelper();

					using (var databaseService = new SKDDatabaseService())
					{
						var pendingCards = databaseService.CardTranslator.GetAllPendingCards(deviceState.Device.UID);
						foreach (var pendingCard in pendingCards)
						{
							var operationResult = databaseService.CardTranslator.GetSingle(pendingCard.CardUID);
							if (!operationResult.HasError && operationResult.Result != null)
							{
								var card = operationResult.Result;
								var getAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(card.AccessTemplateUID);
								var employeeOperationResult = databaseService.EmployeeTranslator.GetSingle(card.HolderUID);
								var employeeName = employeeOperationResult.Result != null ? employeeOperationResult.Result.Name : "";
								if ((PendingCardAction)pendingCard.Action == PendingCardAction.Add)
								{
									var addGKResult = gkSKDHelper.AddOneCard(deviceState.Device, card, getAccessTemplateOperationResult.Result, employeeName);
									if (!addGKResult.HasError)
									{
										databaseService.CardTranslator.DeleteAllPendingCards(pendingCard.CardUID, deviceState.Device.UID);
									}
								}
								if ((PendingCardAction)pendingCard.Action == PendingCardAction.Edit)
								{
									var editGKResult = gkSKDHelper.AddOneCard(deviceState.Device, card, getAccessTemplateOperationResult.Result, employeeName);
									if (!editGKResult.HasError)
									{
										var removeGKCardResult = gkSKDHelper.RemoveOneCard(deviceState.Device, card);
										if (!removeGKCardResult.HasError)
										{
											databaseService.CardTranslator.DeleteAllPendingCards(pendingCard.CardUID, deviceState.Device.UID);
										}
									}
								}
								if ((PendingCardAction)pendingCard.Action == PendingCardAction.Add)
								{
									var removeGKCardResult = gkSKDHelper.RemoveOneCard(deviceState.Device, card);
									if (!removeGKCardResult.HasError)
									{
										databaseService.CardTranslator.DeleteAllPendingCards(pendingCard.CardUID, deviceState.Device.UID);
									}
								}
							}
							else
							{
								databaseService.CardTranslator.DeleteAllPendingCards(pendingCard.CardUID, deviceState.Device.UID);
							}
						}
					}
				}
			}
		}
	}
}