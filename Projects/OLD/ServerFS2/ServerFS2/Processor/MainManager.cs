using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using FiresecAPI.Models;
using FS2Api;
using ServerFS2.Journal;
using ServerFS2.Monitoring;
using ServerFS2.Operations;
using ServerFS2.Service;

namespace ServerFS2.Processor
{
	public static class MainManager
	{
		#region Common
		public static void StartMonitoring(Device device = null)
		{
			CallbackManager.AddProgress(new FS2ProgressInfo("Старт мониторинга"));
			USBManager.Initialize();
			MonitoringManager.StartMonitoring(device);
		}

		public static void StopMonitoring(Device device = null)
		{
			CallbackManager.AddProgress(new FS2ProgressInfo("Остановка мониторинга"));
			MonitoringManager.StopMonitoring(device);
			USBManager.Dispose();
		}

		public static void SuspendMonitoring(Device device = null)
		{
			CallbackManager.AddProgress(new FS2ProgressInfo("Приостановка мониторинга"));
			MonitoringManager.SuspendMonitoring(device);
		}

		public static void ResumeMonitoring(Device device = null)
		{
			CallbackManager.AddProgress(new FS2ProgressInfo("Возобновление мониторинга"));
			MonitoringManager.ResumeMonitoring(device);
		}
		#endregion

		#region Monitoring
		public static List<DeviceState> GetDeviceStates()
		{
			var deviceStates = new List<DeviceState>();
			foreach (var device in ConfigurationManager.Devices)
			{
				if (device.IsMonitoringDisabled)
				{
					var deviceStatesManager = new DeviceStatesManager();
					deviceStatesManager.ForseUpdateDeviceStates(device);
				}
				device.DeviceState.SerializableStates = device.DeviceState.States;
				deviceStates.Add(device.DeviceState);
			}
			return deviceStates;
		}

		public static List<DeviceState> GetDeviceParameters()
		{
			var deviceStates = new List<DeviceState>();
			foreach (var device in ConfigurationManager.Devices)
			{
				device.DeviceState.SerializableStates = device.DeviceState.States;
				deviceStates.Add(device.DeviceState);
			}
			return deviceStates;
		}

		public static List<ZoneState> GetZoneStates()
		{
			var zoneStates = new List<ZoneState>();
			foreach (var zone in ConfigurationManager.Zones)
			{
				zoneStates.Add(zone.ZoneState);
			}
			return zoneStates;
		}

		public static void AddToIgnoreList(List<Device> devices, string userName)
		{
			foreach (var device in devices)
			{
				CustomMessageJournalHelper.Add("Добавление устройства в список обхода", userName, device.ParentPanel, device);
			}
			MonitoringManager.AddTaskIgnore(devices);
		}

		public static void RemoveFromIgnoreList(List<Device> devices, string userName)
		{
			foreach (var device in devices)
			{
				CustomMessageJournalHelper.Add("Удаление устройства из списка обхода", userName, device.ParentPanel, device);
			}
			MonitoringManager.AddTaskResetIgnore(devices);
		}

		public static void SetZoneGuard(Zone zone, string userName)
		{
			MonitoringManager.AddTaskSetGuard(zone, userName);
		}

		public static void UnSetZoneGuard(Zone zone, string userName)
		{
			MonitoringManager.AddTaskResetGuard(zone, userName);
		}

		public static void SetDeviceGuard(Device device, string userName)
		{
			MonitoringManager.AddTaskSetGuard(device, userName);
		}

		public static void UnSetDeviceGuard(Device device, string userName)
		{
			MonitoringManager.AddTaskResetGuard(device, userName);
		}

		public static void ResetStates(List<PanelResetItem> panelResetItems, string userName)
		{
			MonitoringManager.AddPanelResetItems(panelResetItems, userName);
		}

		public static void ExecuteCommand(Device device, string commandName, string userName)
		{
			var tableNo = MetadataHelper.GetDeviceTableNo(device);
			if (tableNo != null)
			{
				var deviceId = MetadataHelper.GetIdByUid(device.DriverUID);
				var devicePropInfo = MetadataHelper.Metadata.devicePropInfos.FirstOrDefault(x => (x.tableType == tableNo) && (x.name == commandName));
				if (devicePropInfo != null)
				{
					CustomMessageJournalHelper.Add(devicePropInfo.caption, userName, device.ParentPanel, device);
				}
			}
			MonitoringManager.AddCommand(device, commandName);
		}
		#endregion

		#region Administrator
		public static void SetNewConfiguration(DeviceConfiguration deviceConfiguration, string userName)
		{
			try
			{
				StopMonitoring();
				ConfigurationManager.DeviceConfiguration = deviceConfiguration;
				ConfigurationManager.Update();
				CustomMessageJournalHelper.Add("Сохранение новой конфигурации", userName);
			}
			catch (Exception e)
			{
				Logger.Error(e, "MainManager.SetNewConfig");
			}
			finally
			{
				Task.Factory.StartNew(() =>
				{
					StartMonitoring();
				});
			}
		}

		public static bool DeviceWriteConfiguration(Device device, bool isUSB, string userName)
		{
			CustomMessageJournalHelper.Add("Запись конфигурации", userName);
			return TempConfigSafeCall<bool>(x =>
			{
				//USBManager.Initialize();
				var result = ConfigurationWriterOperationHelper.Write(x);
				//USBManager.Dispose();
				return result;
			}, device, isUSB);
		}

		public static List<Guid> DeviceWriteAllConfiguration(List<Guid> deviceUIDs, string userName)
		{
			CustomMessageJournalHelper.Add("Команда оператора. Запись новой конфигурации", userName);
			return ConfigurationWriterOperationHelper.WriteAll(deviceUIDs);
		}

		public static void DeviceSetPassword(Device device, bool isUSB, DevicePasswordType devicePasswordType, string password, string userName)
		{
			TempConfigSafeCall(x =>
				{
					CustomMessageJournalHelper.Add("Установка пароля", userName, x);
					SetPasswordOperationHelper.SetPassword(x, devicePasswordType, password);
				},
				device, isUSB);
		}

		public static void DeviceDatetimeSync(Device device, bool isUSB, string userName)
		{
			TempConfigSafeCall<bool>(x =>
			{
				CustomMessageJournalHelper.Add("Установка времени", userName, x);
				ServerHelper.SynchronizeTime(x);
				return true;
			}, device, isUSB);
		}

		public static string DeviceGetInformation(Device device, bool isUSB, string userName)
		{
			return TempConfigSafeCall<string>(x =>
			{
				CustomMessageJournalHelper.Add("Получение информации об устройстве", userName, x);
				var result = GetInformationOperationHelper.GetDeviceInformation(x);
				return result;
			}, device, isUSB);
			//return TempConfigSafeCall<string>(GetInformationOperationHelper.GetDeviceInformation, device, isUSB);
		}

		public static List<string> DeviceGetSerialList(Device device, string userName)
		{
			CustomMessageJournalHelper.Add("Получение информации об устройстве", userName, device);
			try
			{
				StopMonitoring();
				return USBManager.GetAllSerialNos();
			}
			catch (Exception e)
			{
				Logger.Error(e, "MainManager.SetNewConfig");
				throw;
			}
			finally
			{
				StartMonitoring();
			}
		}

		public static string DeviceVerifyFirmwareVersion(Device device, bool isUSB, string fileName)
		{
			return TempConfigSafeCall(x =>
				FirmwareUpdateOperationHelper.Verify(x, isUSB, fileName),
				device, isUSB);
		}

		public static void DeviceUpdateFirmware(Device device, bool isUSB, string userName)
		{
			TempConfigSafeCall(x =>
				{
					CustomMessageJournalHelper.Add("Команда на смену ПО", userName, x);
					SetConfigurationOperationHelper.UpdateFullFlash(device);
				},
				device, isUSB);
		}

		public static DeviceConfiguration DeviceReadConfiguration(Device device, bool isUSB, string userName)
		{
			return TempConfigSafeCall(x =>
			{
				CustomMessageJournalHelper.Add("Чтение конфигурации", userName, x);
				var getConfigurationOperationHelper = new GetConfigurationOperationHelper(false);
				return getConfigurationOperationHelper.GetDeviceConfiguration(x);
			}, device, isUSB);
		}

		public static FS2JournalItemsCollection DeviceReadJournal(Device device, bool isUSB, string userName)
		{
			return TempConfigSafeCall<FS2JournalItemsCollection>(x =>
				{
					CustomMessageJournalHelper.Add("Чтение журнала", userName, device);
					return ReadJournalOperationHelper.GetJournalItemsCollection(x);
				}, device, isUSB);
		}

		public static DeviceConfiguration DeviceAutoDetectChildren(Device device, bool fastSearch, string userName)
		{
			try
			{
				var rootDevice = AutoDetectOperationHelper.AutoDetectDevice(device);
				var deviceConfiguration = new DeviceConfiguration()
				{
					RootDevice = rootDevice
				};
				if (deviceConfiguration.RootDevice == null)
				{
					if ((device.Driver.DriverType == DriverType.USB_Channel_1) || (device.Driver.DriverType == DriverType.USB_Channel_2))
						throw new FS2Exception("Устройство " + device.Parent.PresentationName + " не найдено");
					throw new FS2Exception("Устройство " + device.PresentationName + " не найдено");
				}
				foreach (var child in rootDevice.Children)
				{
					deviceConfiguration.Devices.Add(child);
				}
				deviceConfiguration.Reorder();
				deviceConfiguration.Update();
				deviceConfiguration.InvalidateConfiguration();
				deviceConfiguration.UpdateCrossReferences();
				return deviceConfiguration;
			}
			catch (Exception e)
			{
				Logger.Error(e, "MainManager.DeviceAutoDetectChildren");
				throw;
			}
		}

		public static List<DeviceCustomFunction> DeviceGetCustomFunctions(DriverType driverType)
		{
			return DeviceCustomFunctionListHelper.GetDeviceCustomFunctionList(driverType);
		}

		public static void DeviceExecuteCustomFunction(Device device, bool isUSB, string functionName, string userName)
		{
			TempConfigSafeCall(x =>
			{
				CustomFunctionOperationHelper.Execute(device, functionName);
			}, device, isUSB);
		}

		public static List<GuardUser> DeviceGetGuardUsers(Device device, string userName)
		{
			CustomMessageJournalHelper.Add("Получение списка пользователей", userName, device);
			return GuardUsersOperationHelper.DeviceGetGuardUsers(device);
		}

		public static void DeviceSetGuardUsers(Device device, bool isUSB, List<GuardUser> guardUsers, string userName)
		{
			CustomMessageJournalHelper.Add("Отправка списка пользователей", userName, device);
			GuardUsersOperationHelper.DeviceSetGuardUsers(device, guardUsers);
		}

		public static string DeviceGetMDS5Data(Device device, string userName)
		{
			CustomMessageJournalHelper.Add("Получение данных модуля доставки сообщений", userName, device);
			throw new FS2Exception("Функция пока не реализована");
		}

		public static void SetAuParameters(Device device, List<Property> properties, string userName)
		{
			CustomMessageJournalHelper.Add("Запись параметров устройства", userName, device.ParentPanel, device);
			DeviceParametersOperationHelper.Set(device, properties);
		}

		public static List<Property> GetAuParameters(Device device, string userName)
		{
			CustomMessageJournalHelper.Add("Чтение параметров устройства", userName, device.ParentPanel, device);
			return DeviceParametersOperationHelper.Get(device);
		}
		#endregion

		#region Helpers
		static T TempConfigSafeCall<T>(Func<Device, T> func, Device device, bool isUSB)
		{
			try
			{
				if (isUSB)
				{
					device = USBConfigHelper.SetTempDeviceConfiguration(device);
				}
				var result = func(device);
				return result;
			}
			catch (Exception e)
			{
				Logger.Error(e, "MainManager.TempConfigSafeCall Func");
				throw;
			}
			finally
			{
				if (isUSB)
				{
					USBConfigHelper.SetCurrentDeviceConfiguration();
				}			
			}
		}

		static void TempConfigSafeCall(Action<Device> action, Device device, bool isUSB)
		{
			try
			{
				if (isUSB)
				{
					device = USBConfigHelper.SetTempDeviceConfiguration(device);
				}
				action(device);
			}
			catch (Exception e)
			{
				Logger.Error(e, "MainManager.TempConfigSafeCall Action");
				throw;
			}
			finally
			{
				Task.Factory.StartNew(() =>
				{
					if (isUSB)
					{
						USBConfigHelper.SetCurrentDeviceConfiguration();
					}
				});
			}
		}
		#endregion
	}
}