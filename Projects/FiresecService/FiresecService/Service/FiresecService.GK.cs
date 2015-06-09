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

		public OperationResult<bool> GKWriteConfiguration(Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKWriteConfiguration(device, UserName);
			}
			return OperationResult<bool>.FromError("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
		}

		public OperationResult<GKDeviceConfiguration> GKReadConfiguration(Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				DescriptorsManager.Create();
				return GKProcessorManager.GKReadConfiguration(device, UserName);
			}
			return OperationResult<GKDeviceConfiguration>.FromError("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
		}

		public Stream GKReadConfigurationFromGKFile(Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				DescriptorsManager.Create();
				return GKProcessorManager.GKReadConfigurationFromGKFile(device, UserName);
			}
			return Stream.Null;
			//return new OperationResult<Stream("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
		}

		public OperationResult<GKDevice> GKAutoSearch(Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				DescriptorsManager.Create();
				return GKProcessorManager.GKAutoSearch(device, UserName);
			}
			return OperationResult<GKDevice>.FromError("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
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

		public OperationResult<bool> GKSyncronyseTime(Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				var result = GKProcessorManager.GKSyncronyseTime(device, UserName);
				if (result)
					return new OperationResult<bool>(true);
				return OperationResult<bool>.FromError("Устройство недоступно", false);
			}
			return OperationResult<bool>.FromError("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
		}

		public OperationResult<string> GKGetDeviceInfo(Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return new OperationResult<string>(GKProcessorManager.GKGetDeviceInfo(device, UserName));
			}
			return OperationResult<string>.FromError("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
		}

		public OperationResult<int> GKGetJournalItemsCount(Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKGetJournalItemsCount(device);
			}
			return OperationResult<int>.FromError("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
		}

		public OperationResult<JournalItem> GKReadJournalItem(Guid deviceUID, int no)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKReadJournalItem(device, no);
			}
			return OperationResult<JournalItem>.FromError("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
		}

		public OperationResult<bool> GKSetSingleParameter(Guid objectUID, List<byte> parameterBytes)
		{
			GKBase gkBase = null;
			gkBase = GKManager.Devices.FirstOrDefault(x => x.UID == objectUID);
			if (gkBase == null)
			{
				gkBase = GKManager.Directions.FirstOrDefault(x => x.UID == objectUID);
			}
			if (gkBase == null)
			{
				gkBase = GKManager.Delays.FirstOrDefault(x => x.UID == objectUID);
			}
			if (gkBase == null)
			{
				gkBase = GKManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == objectUID);
			}
			if (gkBase == null)
			{
				gkBase = GKManager.DeviceConfiguration.GuardZones.FirstOrDefault(x => x.UID == objectUID);
			}

			if (gkBase != null)
			{
				return GKProcessorManager.GKSetSingleParameter(gkBase, parameterBytes);
			}
			return OperationResult<bool>.FromError("Не найден компонент в конфигурации");
		}

		public OperationResult<List<GKProperty>> GKGetSingleParameter(Guid objectUID)
		{
			GKBase gkBase = null;
			gkBase = GKManager.Devices.FirstOrDefault(x => x.UID == objectUID);
			if (gkBase == null)
			{
				gkBase = GKManager.Directions.FirstOrDefault(x => x.UID == objectUID);
			}
			if (gkBase == null)
			{
				gkBase = GKManager.Delays.FirstOrDefault(x => x.UID == objectUID);
			}
			if (gkBase == null)
			{
				gkBase = GKManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == objectUID);
			}
			if (gkBase == null)
			{
				gkBase = GKManager.DeviceConfiguration.GuardZones.FirstOrDefault(x => x.UID == objectUID);
			}

			if (gkBase != null)
			{
				return GKProcessorManager.GKGetSingleParameter(gkBase);
			}
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

		public OperationResult<bool> GKSetSchedule(GKSchedule schedule)
		{
			var gkControllerDevice = GKManager.Devices.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
			if (gkControllerDevice != null)
			{
				return GKScheduleHelper.GKSetSchedule(gkControllerDevice, schedule);
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

		public GKStates GKGetStates()
		{
			return GKProcessorManager.GKGetStates();
		}

		public void GKExecuteDeviceCommand(Guid deviceUID, GKStateBit stateBit)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				GKProcessorManager.GKExecuteDeviceCommand(device, stateBit, UserName);
			}
		}

		public void GKReset(Guid uid, GKBaseObjectType objectType)
		{
			var gkBase = GetGKBase(uid, objectType);
			if (gkBase != null)
			{
				GKProcessorManager.GKReset(gkBase, UserName);
			}
		}

		public void GKResetFire1(Guid zoneUID)
		{
			var zone = GKManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
			if (zone != null)
			{
				GKProcessorManager.GKResetFire1(zone, UserName);
			}
		}

		public void GKResetFire2(Guid zoneUID)
		{
			var zone = GKManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
			if (zone != null)
			{
				GKProcessorManager.GKResetFire2(zone, UserName);
			}
		}

		public void GKSetAutomaticRegime(Guid uid, GKBaseObjectType objectType)
		{
			var gkBase = GetGKBase(uid, objectType);
			if (gkBase != null)
			{
				GKProcessorManager.GKSetAutomaticRegime(gkBase, UserName);
			}
		}

		public void GKSetManualRegime(Guid uid, GKBaseObjectType objectType)
		{
			var gkBase = GetGKBase(uid, objectType);
			if (gkBase != null)
			{
				GKProcessorManager.GKSetManualRegime(gkBase, UserName);
			}
		}

		public void GKSetIgnoreRegime(Guid uid, GKBaseObjectType objectType)
		{
			var gkBase = GetGKBase(uid, objectType);
			if (gkBase != null)
			{
				GKProcessorManager.GKSetIgnoreRegime(gkBase, UserName);
			}
		}

		public void GKTurnOn(Guid uid, GKBaseObjectType objectType)
		{
			var gkBase = GetGKBase(uid, objectType);
			if (gkBase != null)
			{
				GKProcessorManager.GKTurnOn(gkBase, UserName);
			}
		}

		public void GKTurnOnNow(Guid uid, GKBaseObjectType objectType)
		{
			var xBase = GetGKBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKTurnOnNow(xBase, UserName);
			}
		}

		public void GKTurnOnInAutomatic(Guid uid, GKBaseObjectType objectType)
		{
			var gkBase = GetGKBase(uid, objectType);
			if (gkBase != null)
			{
				GKProcessorManager.GKTurnOnInAutomatic(gkBase, UserName);
			}
		}

		public void GKTurnOnNowInAutomatic(Guid uid, GKBaseObjectType objectType)
		{
			var xBase = GetGKBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKTurnOnNowInAutomatic(xBase, UserName);
			}
		}

		public void GKTurnOff(Guid uid, GKBaseObjectType objectType)
		{
			var gkBase = GetGKBase(uid, objectType);
			if (gkBase != null)
			{
				GKProcessorManager.GKTurnOff(gkBase, UserName);
			}
		}

		public void GKTurnOffNow(Guid uid, GKBaseObjectType objectType)
		{
			var gkBase = GetGKBase(uid, objectType);
			if (gkBase != null)
			{
				GKProcessorManager.GKTurnOffNow(gkBase, UserName);
			}
		}

		public void GKTurnOffInAutomatic(Guid uid, GKBaseObjectType objectType)
		{
			var gkBase = GetGKBase(uid, objectType);
			if (gkBase != null)
			{
				GKProcessorManager.GKTurnOffInAutomatic(gkBase, UserName);
			}
		}

		public void GKTurnOffNowInAutomatic(Guid uid, GKBaseObjectType objectType)
		{
			var gkBase = GetGKBase(uid, objectType);
			if (gkBase != null)
			{
				GKProcessorManager.GKTurnOffNowInAutomatic(gkBase, UserName);
			}
		}

		public void GKStop(Guid uid, GKBaseObjectType objectType)
		{
			var gkBase = GetGKBase(uid, objectType);
			if (gkBase != null)
			{
				GKProcessorManager.GKStop(gkBase, UserName);
			}
		}

		GKBase GetGKBase(Guid uid, GKBaseObjectType objectType)
		{
			switch (objectType)
			{
				case GKBaseObjectType.Deivce:
					return GKManager.Devices.FirstOrDefault(x => x.UID == uid);
				case GKBaseObjectType.Direction:
					return GKManager.Directions.FirstOrDefault(x => x.UID == uid);
				case GKBaseObjectType.Zone:
					return GKManager.Zones.FirstOrDefault(x => x.UID == uid);
				case GKBaseObjectType.PumpStation:
					return GKManager.PumpStations.FirstOrDefault(x => x.UID == uid);
				case GKBaseObjectType.MPT:
					return GKManager.MPTs.FirstOrDefault(x => x.UID == uid);
				case GKBaseObjectType.Pim:
					return GKManager.AutoGeneratedPims.FirstOrDefault(x => x.UID == uid);
				case GKBaseObjectType.Delay:
					return GKManager.AutoGeneratedDelays.FirstOrDefault(x => x.UID == uid);
				case GKBaseObjectType.GuardZone:
					return GKManager.GuardZones.FirstOrDefault(x => x.UID == uid);
				case GKBaseObjectType.Door:
					return GKManager.Doors.FirstOrDefault(x => x.UID == uid);
			}
			return null;
		}

		public void GKStartMeasureMonitoring(Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				GKProcessorManager.GKStartMeasureMonitoring(device);
			}
		}

		public void GKStopMeasureMonitoring(Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				GKProcessorManager.GKStopMeasureMonitoring(device);
			}
		}

		public OperationResult<uint> GKGetReaderCode(Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKGetReaderCode(device);
			}
			return OperationResult<uint>.FromError("Не найдено устройство в конфигурации");
		}

		public void GKOpenSKDZone(Guid zoneUID)
		{
			var zone = GKManager.SKDZones.FirstOrDefault(x => x.UID == zoneUID);
			if (zone != null)
			{
				GKProcessorManager.GKOpenSKDZone(zone);
			}
		}

		public void GKCloseSKDZone(Guid zoneUID)
		{
			var zone = GKManager.SKDZones.FirstOrDefault(x => x.UID == zoneUID);
			if (zone != null)
			{
				GKProcessorManager.GKCloseSKDZone(zone);
			}
		}

		public OperationResult<CurrentConsumption> GetAlsMeasure(Guid alsUid)
		{
			try
			{
				return GKProcessorManager.GetAlsMeasure(alsUid);
			}
			catch (Exception e)
			{
				return OperationResult<CurrentConsumption>.FromError(e.Message);
			}
		}

		#region Users
		public OperationResult<List<GKUser>> GKGetUsers(Guid gkDeviceUID)
		{
			var gkControllerDevice = GKManager.Devices.FirstOrDefault(x => x.UID == gkDeviceUID);
			if (gkControllerDevice != null)
			{
				try
				{
					return GKSKDHelper.GetAllUsers(gkControllerDevice);
				}
				catch (Exception e)
				{
					return OperationResult<List<GKUser>>.FromError(e.Message);
				}
			}
			return OperationResult<List<GKUser>>.FromError("Не найден ГК в конфигурации");
		}

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
								var getAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(card.AccessTemplateUID);
								var accessTemplate = getAccessTemplateOperationResult.Result;

								var controllerCardSchedules = GKSKDHelper.GetGKControllerCardSchedules(card, accessTemplate);
								var controllerCardSchedule = controllerCardSchedules.FirstOrDefault(x => x.ControllerDevice.UID == deviceUID);
								if (controllerCardSchedule != null)
								{
									var employeeOperationResult = databaseService.EmployeeTranslator.GetSingle(card.HolderUID);
									var employee = employeeOperationResult.Result;
									if (employee != null)
									{
										GKSKDHelper.AddOrEditCard(controllerCardSchedule, card, employee.FIO);
									}
								}
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