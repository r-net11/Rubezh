using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using FiresecClient;
using GKProcessor;
using SKDDriver;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		string UserName
		{
			get
			{
				if (CurrentClientCredentials != null)
					return CurrentClientCredentials.FriendlyUserName;
				return "<Нет>";
			}
		}

		public void CancelGKProgress(Guid progressCallbackUID, string userName)
		{
			ChinaSKDDriver.Processor.CancelProgress(progressCallbackUID, userName);
			GKProcessorManager.CancelGKProgress(progressCallbackUID, userName);
		}

		public OperationResult<GKDeviceConfiguration> GKReadConfiguration(Guid deviceUID)
		{
			return OperationResult<GKDeviceConfiguration>.FromError("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
		}

		public OperationResult<bool> GKUpdateFirmware(Guid deviceUID, string fileName)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKUpdateFirmware(device, fileName, UserName);
			}
			return OperationResult<bool>.FromError("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
		}

		public OperationResult<bool> GKUpdateFirmwareFSCS(HexFileCollectionInfo hxcFileInfo, string userName, List<Guid> deviceUIDs)
		{
			var devices = new List<GKDevice>();
			foreach (var deviceUID in deviceUIDs)
			{
				var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
				if (device == null)
				{
					return OperationResult<bool>.FromError("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
				}
				devices.Add(device);
			}
			return GKProcessorManager.GKUpdateFirmwareFSCS(hxcFileInfo, userName, devices);
		}

		public OperationResult<bool> GKSetSingleParameter(Guid objectUID, List<byte> parameterBytes)
		{
			return OperationResult<bool>.FromError("Не найден компонент в конфигурации");
		}

		public OperationResult<List<GKProperty>> GKGetSingleParameter(Guid objectUID)
		{
			return OperationResult<List<GKProperty>>.FromError("Не найден компонент в конфигурации");
		}

		public OperationResult<bool> GKRewriteAllSchedules(Guid deviceUID)
		{
			var gkControllerDevice = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (gkControllerDevice != null)
			{
				return GKScheduleHelper.GKRewriteAllSchedules(gkControllerDevice);
			}
			return OperationResult<bool>.FromError("Не найден ГК в конфигурации");
		}

		public OperationResult<List<byte>> GKGKHash(Guid gkDeviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == gkDeviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKGKHash(device);
			}
			return OperationResult<List<byte>>.FromError("Не найдено устройство в конфигурации");
		}

		GKBase GetGKBase(Guid uid, GKBaseObjectType objectType)
		{
			switch (objectType)
			{
				case GKBaseObjectType.Deivce:
					return GKManager.Devices.FirstOrDefault(x => x.UID == uid);
			}
			return null;
		}

		//public void GKOpenSKDZone(Guid zoneUID) //TODO: Change to SKD
		//{
		//	var zone = GKManager.SKDZones.FirstOrDefault(x => x.UID == zoneUID);
		//	if (zone != null)
		//	{
		//		GKProcessorManager.GKOpenSKDZone(zone);
		//	}
		//}

		//public void GKCloseSKDZone(Guid zoneUID)
		//{
		//	var zone = GKManager.SKDZones.FirstOrDefault(x => x.UID == zoneUID);
		//	if (zone != null)
		//	{
		//		GKProcessorManager.GKCloseSKDZone(zone);
		//	}
		//}

		#region Users
		//public OperationResult<List<GKUser>> GKGetUsers(Guid gkDeviceUID)
		//{
		//	var gkControllerDevice = GKManager.Devices.FirstOrDefault(x => x.UID == gkDeviceUID);
		//	if (gkControllerDevice != null)
		//	{
		//		try
		//		{
		//			return GKSKDHelper.GetAllUsers(gkControllerDevice);
		//		}
		//		catch (Exception e)
		//		{
		//			return OperationResult<List<GKUser>>.FromError(e.Message);
		//		}
		//	}
		//	return OperationResult<List<GKUser>>.FromError("Не найден ГК в конфигурации");
		//}

		public OperationResult<bool> GKRewriteUsers(Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				var progressCallback = GKProcessorManager.StartProgress("Удаление пользователей прибора " + device.PresentationName, "", 65535, false, GKProgressClientType.Administrator);

				using (var databaseService = new SKDDatabaseService())
				{
					databaseService.CardTranslator.DeleteAllPendingCards(device.UID);
				}

				try
				{
					var removeResult = GKSKDHelper.RemoveAllUsers(device, progressCallback);
					if (!removeResult)
					{
						GKProcessorManager.StopProgress(progressCallback);
						return OperationResult<bool>.FromError("Ошибка при удалении пользователя из ГК");
					}

					using (var databaseService = new SKDDatabaseService())
					{
						var cardsResult = databaseService.CardTranslator.Get(new CardFilter());
						if (!cardsResult.HasError)
						{
							progressCallback.StepCount = cardsResult.Result.Count();
							progressCallback.CurrentStep = 0;
							foreach (var card in cardsResult.Result)
							{
								GKProcessorManager.DoProgress("Пользователь " + card.Number, progressCallback);
							}
						}
					}

					return new OperationResult<bool>(true);
				}
				catch (Exception e)
				{
					return OperationResult<bool>.FromError(e.Message);
				}
				finally
				{
					GKProcessorManager.StopProgress(progressCallback);
				}
			}
			return OperationResult<bool>.FromError("Не найден ГК в конфигурации");
		}
		#endregion
	}
}