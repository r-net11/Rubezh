using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.SKD;
using RubezhClient;
using GKProcessor;
using RubezhDAL;
using System.Diagnostics;
using System.Threading;

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
			GKProcessorManager.CancelGKProgress(progressCallbackUID, userName);
		}

		public OperationResult<bool> GKWriteConfiguration(Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				var progressCallback = new GKProgressCallback();
				ServerTaskRunner.Add(progressCallback, "Запись конфигурации ГК", new Action(() =>
				{
					var result = GKProcessorManager.GKWriteConfiguration(device, UserName, progressCallback);
					FiresecService.NotifyOperationResult_WriteConfiguration(result);
				}
				));
				return new OperationResult<bool>(true);
			}
			return OperationResult<bool>.FromError("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
		}

		public OperationResult<GKDeviceConfiguration> GKReadConfiguration(Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKReadConfiguration(device, UserName);
			}
			return OperationResult<GKDeviceConfiguration>.FromError("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
		}

		public OperationResult<bool> GKReadConfigurationFromGKFile(Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				var progressCallback = new GKProgressCallback();
				ServerTaskRunner.Add(progressCallback, "Чтение файла конфигурации ГК", new Action(() =>
				{
					var result = GKProcessorManager.GKReadConfigurationFromGKFile(device, UserName, progressCallback);
					FiresecService.NotifyOperationResult_ReadConfigurationFromGKFile(result);
				}
				));
				return new OperationResult<bool>(true);
			}
			return OperationResult<bool>.FromError("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
		}

		public OperationResult<GKDevice> GKAutoSearch(Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
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

		public OperationResult<bool> GKSetSingleParameter(Guid objectUID, List<byte> parameterBytes, List<GKProperty> deviceProperties)
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
			if (gkBase == null)
			{
				gkBase = GKManager.DeviceConfiguration.PumpStations.FirstOrDefault(x => x.UID == objectUID);
			}
			if (gkBase == null)
			{
				gkBase = GKManager.DeviceConfiguration.Doors.FirstOrDefault(x => x.UID == objectUID);
			}

			if (gkBase != null)
			{
				var result = GKProcessorManager.GKSetSingleParameter(gkBase, parameterBytes);
				if (deviceProperties != null)
				{
					FiresecService.NotifyGKParameterChanged(objectUID, deviceProperties);
				}
				return result;
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
			if (gkBase == null)
			{
				gkBase = GKManager.DeviceConfiguration.PumpStations.FirstOrDefault(x => x.UID == objectUID);
			}
			if (gkBase == null)
			{
				gkBase = GKManager.DeviceConfiguration.Doors.FirstOrDefault(x => x.UID == objectUID);
			}

			if (gkBase != null)
			{
				return GKProcessorManager.GKGetSingleParameter(gkBase);
			}
			return OperationResult<List<GKProperty>>.FromError("Не найден компонент в конфигурации");
		}

		public OperationResult<bool> GKRewriteAllSchedules(Guid deviceUID)
		{
			if (RubezhDAL.DataClasses.DbService.ConnectionOperationResult.HasError)
				return OperationResult<bool>.FromError("Отсутствует подключение к БД");
			var gkControllerDevice = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (gkControllerDevice != null)
			{
				return GKScheduleHelper.RewriteAllSchedules(gkControllerDevice);
			}
			return OperationResult<bool>.FromError("Не найден ГК в конфигурации");
		}

		public OperationResult<bool> GKSetSchedule(GKSchedule schedule)
		{
			var result = GKScheduleHelper.SetSchedule(schedule);
			return OperationResult<bool>.FromError(result.Error, true);
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
				case GKBaseObjectType.Device:
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
					return GKManager.Delays.FirstOrDefault(x => x.UID == uid);
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
		public OperationResult<bool> GKGetUsers(Guid gkDeviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == gkDeviceUID);
			if (device != null)
			{
				var progressCallback = new GKProgressCallback();
				ServerTaskRunner.Add(progressCallback, "Чтение пользователей прибора", new Action(() =>
					{
						try
						{
							var users = GKSKDHelper.GetAllUsers(device, progressCallback);
							FiresecService.NotifyOperationResult_GetAllUsers(users);
						}
						catch (Exception e)
						{
							FiresecService.NotifyOperationResult_GetAllUsers(OperationResult<List<GKUser>>.FromError(e.Message));
						}
					}
				));
				return new OperationResult<bool>(true);
			}
			return OperationResult<bool>.FromError("Не найден ГК в конфигурации");
		}

		public OperationResult<bool> GKRewriteUsers(Guid deviceUID)
		{
			if (RubezhDAL.DataClasses.DbService.ConnectionOperationResult.HasError)
				return OperationResult<bool>.FromError("Отсутствует подключение к БД");
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				var progressCallback = new GKProgressCallback();
				ServerTaskRunner.Add(progressCallback, "Перезапись пользователей прибора", new Action(() =>
					{
						progressCallback = GKProcessorManager.StartProgress("Удаление пользователей прибора " + device.PresentationName, "", 65535, false, GKProgressClientType.Administrator);

						using (var databaseService = new RubezhDAL.DataClasses.DbService())
						{
							databaseService.CardTranslator.DeleteAllPendingCards(device.UID);
						}

						try
						{
							var usersCount = GKSKDHelper.GetUsersCount(device);
							var removeResult = GKSKDHelper.RemoveAllUsers(device, usersCount, progressCallback);
							if (!removeResult)
							{
								GKProcessorManager.StopProgress(progressCallback);
								FiresecService.NotifyOperationResult_RewriteUsers(OperationResult<bool>.FromError("Ошибка при удалении пользователя из ГК"));
								return;
							}
							progressCallback.Title = "Добавление пользователей прибора " + device.PresentationName;

							int currentUserNo = 1;

							using (var databaseService = new RubezhDAL.DataClasses.DbService())
							{
								var cardsResult = databaseService.CardTranslator.Get(new CardFilter() { DeactivationType = LogicalDeletationType.Active });
								if (!cardsResult.HasError)
								{
									progressCallback.StepCount = cardsResult.Result.Count();
									progressCallback.CurrentStep = 0;
									foreach (var card in cardsResult.Result.OrderBy(x => x.Number))
									{
										var getAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(card.AccessTemplateUID);
										var accessTemplateDoors = !getAccessTemplateOperationResult.HasError && getAccessTemplateOperationResult.Result != null ? getAccessTemplateOperationResult.Result.CardDoors : new List<CardDoor>();

										var controllerCardSchedules = GKSKDHelper.GetGKControllerCardSchedules(card, accessTemplateDoors);
										var controllerCardSchedule = controllerCardSchedules.FirstOrDefault(x => x.ControllerDevice.UID == device.UID);
										if (controllerCardSchedule != null)
										{
											var employeeOperationResult = databaseService.EmployeeTranslator.GetSingle(card.EmployeeUID.Value);
											var employee = employeeOperationResult.Result;
											if (employee != null)
											{
												GKSKDHelper.AddOrEditCard(controllerCardSchedule, card, employee.FIO, currentUserNo, currentUserNo > usersCount);
												currentUserNo++;
											}
										}
										GKProcessorManager.DoProgress("Пользователь " + card.EmployeeName + ". Номер карты " + card.Number, progressCallback);
									}
								}
							}
							FiresecService.NotifyOperationResult_RewriteUsers(new OperationResult<bool>(true));
						}
						catch (Exception e)
						{
							FiresecService.NotifyOperationResult_RewriteUsers(OperationResult<bool>.FromError(e.Message));
						}
						finally
						{
							GKProcessorManager.StopProgress(progressCallback);
						}
					}
				));
				return new OperationResult<bool>(true);
			}
			return OperationResult<bool>.FromError("Не найден ГК в конфигурации");
		}
		#endregion

		public OperationResult<List<MirrorUser>> GKReadMirrorUsers(Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return OperationResult<List<MirrorUser>>.FromError("метод не реализован");
			}
			else
				return OperationResult<List<MirrorUser>>.FromError("Не найдено Отражение в конфигурации");
		}

		public OperationResult<bool> GKWriteMirrorUsers(Guid deviceUID, List<MirrorUser> mirrorUsers)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return OperationResult<bool>.FromError("метод не реализован");
			}
			else
				return OperationResult<bool>.FromError("Не найдено Отражение в конфигурации");
		}
	}
}