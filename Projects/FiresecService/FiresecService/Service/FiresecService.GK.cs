using GKProcessor;
using Infrastructure.Common;
using Ionic.Zip;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public void CancelGKProgress(Guid clientUID, Guid progressCallbackUID, string userName)
		{
			GKProcessorManager.CancelGKProgress(progressCallbackUID, userName);
		}

		public OperationResult<bool> GKWriteConfiguration(Guid clientUID, Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				var progressCallback = new GKProgressCallback();
				ServerTaskRunner.Add(progressCallback, "Запись конфигурации ГК", (() =>
				{
					if (GKManager.DeviceConfiguration.OnlyGKDeviceConfiguration)
					{
						var deviceConfigFileName = AppDataFolderHelper.GetServerAppDataPath("Config\\GKDeviceConfiguration.xml");
						var zipDeviceConfigFileName = AppDataFolderHelper.GetServerAppDataPath("GKDeviceConfiguration.fscp");
						if (File.Exists(zipDeviceConfigFileName))
							File.Delete(zipDeviceConfigFileName);
						var zipFile = new ZipFile(zipDeviceConfigFileName);
						zipFile.AddFile(deviceConfigFileName, "");
						zipFile.Save(zipDeviceConfigFileName);
						zipFile.Dispose();
					}
					var result = GKProcessorManager.GKWriteConfiguration(device, GetUserName(clientUID), progressCallback, clientUID);
					NotifyOperationResult_WriteConfiguration(result, clientUID);
				}
				));
				return new OperationResult<bool>(true);
			}
			return OperationResult<bool>.FromError("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
		}

		public OperationResult<GKDeviceConfiguration> GKReadConfiguration(Guid clientUID, Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKReadConfiguration(device, GetUserName(clientUID), clientUID);
			}
			return OperationResult<GKDeviceConfiguration>.FromError("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
		}

		public OperationResult<bool> GKReadConfigurationFromGKFile(Guid clientUID, Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				var progressCallback = new GKProgressCallback();
				ServerTaskRunner.Add(progressCallback, "Чтение файла конфигурации ГК", new Action(() =>
				{
					var result = GKProcessorManager.GKReadConfigurationFromGKFile(device, GetUserName(clientUID), progressCallback, clientUID);
					FiresecService.NotifyOperationResult_ReadConfigurationFromGKFile(result, clientUID);
				}
				));
				return new OperationResult<bool>(true);
			}
			return OperationResult<bool>.FromError("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
		}

		public OperationResult<GKDevice> GKAutoSearch(Guid clientUID, Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKAutoSearch(device, GetUserName(clientUID), clientUID);
			}
			return OperationResult<GKDevice>.FromError("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
		}

		public OperationResult<bool> GKUpdateFirmware(Guid clientUID, Guid deviceUID, List<byte> firmwareBytes)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKUpdateFirmware(device, firmwareBytes, GetUserName(clientUID), clientUID);
			}
			return OperationResult<bool>.FromError("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
		}

		public OperationResult<bool> GKSyncronyseTime(Guid clientUID, Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				var result = GKProcessorManager.GKSyncronyseTime(device, GetUserName(clientUID));
				if (result)
					return new OperationResult<bool>(true);
				return OperationResult<bool>.FromError("Устройство недоступно или IP-адрес компьютера отсутствует в списке разрешенных адресов прибора", false);
			}
			return OperationResult<bool>.FromError("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
		}

		public OperationResult<string> GKGetDeviceInfo(Guid clientUID, Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return new OperationResult<string>(GKProcessorManager.GKGetDeviceInfo(device, GetUserName(clientUID)));
			}
			return OperationResult<string>.FromError("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
		}

		public OperationResult<int> GKGetJournalItemsCount(Guid clientUID, Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKGetJournalItemsCount(device);
			}
			return OperationResult<int>.FromError("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
		}

		public OperationResult<JournalItem> GKReadJournalItem(Guid clientUID, Guid deviceUID, int no)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKReadJournalItem(device, no);
			}
			return OperationResult<JournalItem>.FromError("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
		}

		public OperationResult<bool> GKSetSingleParameter(Guid clientUID, Guid objectUID, List<byte> parameterBytes, List<GKProperty> deviceProperties)
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
					FiresecService.NotifyGKParameterChanged(objectUID, deviceProperties, clientUID);
				}
				return result;
			}
			return OperationResult<bool>.FromError("Не найден компонент в конфигурации");
		}

		public OperationResult<List<GKProperty>> GKGetSingleParameter(Guid clientUID, Guid objectUID)
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

		public OperationResult<bool> GKRewriteAllSchedules(Guid clientUID, Guid deviceUID)
		{
			if (RubezhDAL.DataClasses.DbService.ConnectionOperationResult.HasError)
				return OperationResult<bool>.FromError("Отсутствует подключение к БД");
			var gkControllerDevice = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (gkControllerDevice != null)
			{
				return GKScheduleHelper.RewriteAllSchedules(gkControllerDevice, clientUID);
			}
			return OperationResult<bool>.FromError("Не найден ГК в конфигурации");
		}

		public OperationResult<bool> GKSetSchedule(Guid clientUID, GKSchedule schedule)
		{
			var result = GKScheduleHelper.SetSchedule(schedule);
			return OperationResult<bool>.FromError(result.Error, true);
		}

		public OperationResult<List<byte>> GKGKHash(Guid clientUID, Guid gkDeviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == gkDeviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKGKHash(device);
			}
			return OperationResult<List<byte>>.FromError("Не найдено устройство в конфигурации");
		}

		public GKStates GKGetStates(Guid clientUID)
		{
			return GKProcessorManager.GKGetStates();
		}

		public void GKExecuteDeviceCommand(Guid clientUID, string userName, Guid deviceUID, GKStateBit stateBit)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				GKProcessorManager.GKExecuteDeviceCommand(device, stateBit, userName == null ? GetUserName(clientUID) : userName);
			}
		}

		public void GKReset(Guid clientUID, string userName, Guid uid, GKBaseObjectType objectType)
		{
			var gkBase = GetGKBase(uid, objectType);
			if (gkBase != null)
			{
				GKProcessorManager.GKReset(gkBase, userName == null ? GetUserName(clientUID) : userName);
			}
		}

		public void GKResetFire1(Guid clientUID, Guid zoneUID)
		{
			var zone = GKManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
			if (zone != null)
			{
				GKProcessorManager.GKResetFire1(zone, GetUserName(clientUID));
			}
		}

		public void GKResetFire2(Guid clientUID, Guid zoneUID)
		{
			var zone = GKManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
			if (zone != null)
			{
				GKProcessorManager.GKResetFire2(zone, GetUserName(clientUID));
			}
		}

		public void GKSetAutomaticRegime(Guid clientUID, string userName, Guid uid, GKBaseObjectType objectType)
		{
			var gkBase = GetGKBase(uid, objectType);
			if (gkBase != null)
			{
				GKProcessorManager.GKSetAutomaticRegime(gkBase, userName == null ? GetUserName(clientUID) : userName);
			}
		}

		public void GKSetManualRegime(Guid clientUID, string userName, Guid uid, GKBaseObjectType objectType)
		{
			var gkBase = GetGKBase(uid, objectType);
			if (gkBase != null)
			{
				GKProcessorManager.GKSetManualRegime(gkBase, userName == null ? GetUserName(clientUID) : userName);
			}
		}

		public void GKSetIgnoreRegime(Guid clientUID, string userName, Guid uid, GKBaseObjectType objectType)
		{
			var gkBase = GetGKBase(uid, objectType);
			if (gkBase != null)
			{
				GKProcessorManager.GKSetIgnoreRegime(gkBase, userName == null ? GetUserName(clientUID) : userName);
			}
		}

		public void GKTurnOn(Guid clientUID, string userName, Guid uid, GKBaseObjectType objectType)
		{
			var gkBase = GetGKBase(uid, objectType);
			if (gkBase != null)
			{
				GKProcessorManager.GKTurnOn(gkBase,  userName == null ? GetUserName(clientUID) : userName);
			}
		}

		public void GKTurnOnNow(Guid clientUID, string userName, Guid uid, GKBaseObjectType objectType)
		{
			var xBase = GetGKBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKTurnOnNow(xBase, userName == null ? GetUserName(clientUID) : userName);
			}
		}

		public void GKTurnOnInAutomatic(Guid clientUID, Guid uid, GKBaseObjectType objectType)
		{
			var gkBase = GetGKBase(uid, objectType);
			if (gkBase != null)
			{
				GKProcessorManager.GKTurnOnInAutomatic(gkBase, GetUserName(clientUID));
			}
		}

		public void GKTurnOnNowInAutomatic(Guid clientUID, Guid uid, GKBaseObjectType objectType)
		{
			var xBase = GetGKBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKTurnOnNowInAutomatic(xBase, GetUserName(clientUID));
			}
		}

		public void GKTurnOff(Guid clientUID, string userName, Guid uid, GKBaseObjectType objectType)
		{
			var gkBase = GetGKBase(uid, objectType);
			if (gkBase != null)
			{
				GKProcessorManager.GKTurnOff(gkBase, userName == null ? GetUserName(clientUID) : userName);
			}
		}

		public void GKTurnOffNow(Guid clientUID, string userName, Guid uid, GKBaseObjectType objectType)
		{
			var gkBase = GetGKBase(uid, objectType);
			if (gkBase != null)
			{
				GKProcessorManager.GKTurnOffNow(gkBase, userName == null ? GetUserName(clientUID) : userName);
			}
		}

		public void GKTurnOffInAutomatic(Guid clientUID,string userName, Guid uid, GKBaseObjectType objectType)
		{
			var gkBase = GetGKBase(uid, objectType);
			if (gkBase != null)
			{
				GKProcessorManager.GKTurnOffInAutomatic(gkBase, userName == null ? GetUserName(clientUID) : userName);
			}
		}

		public void GKTurnOffNowInAutomatic(Guid clientUID, Guid uid, GKBaseObjectType objectType)
		{
			var gkBase = GetGKBase(uid, objectType);
			if (gkBase != null)
			{
				GKProcessorManager.GKTurnOffNowInAutomatic(gkBase, GetUserName(clientUID));
			}
		}

		public void GKTurnOnNowGlobalPimsInAutomatic(Guid clientUID)
		{
			GKProcessorManager.GKTurnOnNowGlobalPimsInAutomatic(GetUserName(clientUID));
		}

		public void GKTurnOffNowGlobalPimsInAutomatic(Guid clientUID)
		{
			GKProcessorManager.GKTurnOffNowGlobalPimsInAutomatic(GetUserName(clientUID));
		}

		public void GKStop(Guid clientUID, string userName, Guid uid, GKBaseObjectType objectType)
		{
			var gkBase = GetGKBase(uid, objectType);
			if (gkBase != null)
			{
				GKProcessorManager.GKStop(gkBase, userName == null ? GetUserName(clientUID) : userName);
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

		public void GKStartMeasureMonitoring(Guid clientUID, Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				GKProcessorManager.GKStartMeasureMonitoring(device);
			}
		}

		public void GKStopMeasureMonitoring(Guid clientUID, Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				GKProcessorManager.GKStopMeasureMonitoring(device);
			}
		}

		public OperationResult<uint> GKGetReaderCode(Guid clientUID, Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKGetReaderCode(device);
			}
			return OperationResult<uint>.FromError("Не найдено устройство в конфигурации");
		}

		public void GKOpenSKDZone(Guid clientUID, Guid zoneUID)
		{
			var zone = GKManager.SKDZones.FirstOrDefault(x => x.UID == zoneUID);
			if (zone != null)
			{
				GKProcessorManager.GKOpenSKDZone(zone);
			}
		}

		public void GKCloseSKDZone(Guid clientUID, Guid zoneUID)
		{
			var zone = GKManager.SKDZones.FirstOrDefault(x => x.UID == zoneUID);
			if (zone != null)
			{
				GKProcessorManager.GKCloseSKDZone(zone);
			}
		}

		public OperationResult<CurrentConsumption> GetAlsMeasure(Guid clientUID, Guid alsUid)
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
		public OperationResult<bool> GKGetUsers(Guid clientUID, Guid gkDeviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == gkDeviceUID);
			if (device != null)
			{
				var isGk = device.DriverType == GKDriverType.GK;
				var progressCallback = new GKProgressCallback();
				ServerTaskRunner.Add(progressCallback, "Чтение пользователей прибора", () =>
				{
					try
					{
						var users = GKSKDHelper.GetAllUsers(device, progressCallback, clientUID);
						NotifyOperationResult_GetAllUsers(users, isGk, clientUID, device.UID);
					}
					catch (Exception e)
					{
						NotifyOperationResult_GetAllUsers(OperationResult<List<GKUser>>.FromError(e.Message), isGk, clientUID, device.UID);
					}
				});
				return new OperationResult<bool>(true);
			}
			return OperationResult<bool>.FromError("Не найден ГК в конфигурации");
		}

		public OperationResult<bool> GKRewriteUsers(Guid clientUID, Guid deviceUID)
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
							var usersCountResult = GKSKDHelper.GetUsersCount(device);
							if (usersCountResult.HasError)
								return;
							var usersCount = usersCountResult.Result;
							var removeResult = GKSKDHelper.RemoveAllUsers(device, usersCount, progressCallback, clientUID);
							if (!removeResult)
							{
								GKProcessorManager.StopProgress(progressCallback);
								NotifyOperationResult_RewriteUsers(OperationResult<bool>.FromError("Ошибка при удалении пользователя из ГК"), clientUID);
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
							FiresecService.NotifyOperationResult_RewriteUsers(new OperationResult<bool>(true), clientUID);
						}
						catch (Exception e)
						{
							FiresecService.NotifyOperationResult_RewriteUsers(OperationResult<bool>.FromError(e.Message), clientUID);
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

		public OperationResult<List<GKUser>> GetGKUsers(Guid clientUID, Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return GKSKDHelper.GetAllUsers(device, new GKProgressCallback(), clientUID);
			}
			return OperationResult<List<GKUser>>.FromError("Прибор не найден в конфигурации");
		}

		public OperationResult<bool> RewritePmfUsers(Guid clientUID, Guid uid, List<GKUser> users)
		{
			var progressCallback = new GKProgressCallback();
			ServerTaskRunner.Add(progressCallback, "Чтение пользователей прибора", new Action(() =>
			{
				try
				{
					var result = GKSKDHelper.RewritePmfUsers(clientUID, uid, users, progressCallback);
					FiresecService.NotifyOperationResult_RewriteUsers(result, clientUID);
				}
				catch (Exception e)
				{
					NotifyOperationResult_RewriteUsers(OperationResult<bool>.FromError(e.Message), clientUID);
				}
			}
			));
			return new OperationResult<bool>(true);
		}

		#endregion
	}
}