using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FS2Api;
using ServerFS2.ConfigurationWriter;
using ServerFS2.Monitoring;
using ServerFS2.Service;
using Common;
using System.Threading;
using ServerFS2.Operations;
using System.Threading.Tasks;

namespace ServerFS2.Processor
{
	public static class MainManager
	{
		#region Common
		public static event Action<FS2JournalItem> NewJournalItem;
		static void OnNewItem(FS2JournalItem journalItem)
		{
			if (NewJournalItem != null)
				NewJournalItem(journalItem);
		}

		public static void StartMonitoring(Device device = null)
		{
			USBManager.Initialize();
			MonitoringPanel.NewJournalItem -= new Action<FS2JournalItem>(OnNewItem);
			MonitoringPanel.NewJournalItem += new Action<FS2JournalItem>(OnNewItem);
			MonitoringManager.StartMonitoring(device);
		}

		public static void StopMonitoring(Device device = null)
		{
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

		public static void AddToIgnoreList(List<Device> devices)
		{
			MonitoringManager.AddTaskIgnore(devices);
		}

		public static void RemoveFromIgnoreList(List<Device> devices)
		{
			MonitoringManager.AddTaskResetIgnore(devices);
		}

		public static void SetZoneGuard(Guid zoneUID)
		{
			throw new FS2Exception("Функция пока не реализована");
		}

		public static void UnSetZoneGuard(Guid zoneUID)
		{
			throw new FS2Exception("Функция пока не реализована");
		}

		public static void SetDeviceGuard(Guid deviceUID)
		{
			throw new FS2Exception("Функция пока не реализована");
		}

		public static void UnSetDeviceGuard(Guid deviceUID)
		{
			throw new FS2Exception("Функция пока не реализована");
		}

		public static void ResetStates(List<PanelResetItem> panelResetItems)
		{
			MonitoringManager.AddPanelResetItems(panelResetItems);
		}

		public static void ExecuteCommand(Guid deviceUID, string methodName)
		{
			throw new FS2Exception("Функция пока не реализована");
		}

		public static void AddCommand(Device device, string commandName)
		{
			MonitoringManager.ExecuteCommand(device, commandName);
		}
		#endregion

		#region Administrator
		public static void SetNewConfiguration(DeviceConfiguration deviceConfiguration)
		{
			try
			{
				StopMonitoring();
				ConfigurationManager.DeviceConfiguration = deviceConfiguration;
				ConfigurationManager.Update();
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

		public static void DeviceWriteConfiguration(Device device, bool isUSB)
		{
			TempConfigSafeCall((x) =>
			{
				ConfigurationWriterOperationHelper.Write(x);
			}, device, isUSB);
		}

		public static void DeviceWriteAllConfiguration()
		{
			throw new FS2Exception("Функция пока не реализована");
			ConfigurationWriterOperationHelper.WriteAll();
		}

		public static void DeviceSetPassword(Device device, bool isUSB, DevicePasswordType devicePasswordType, string password)
		{
			TempConfigSafeCall((x) =>
			{
				SetConfigurationOperationHelper.SetPassword(device, devicePasswordType, password);
			}, device, isUSB);
		}

		public static void DeviceDatetimeSync(Device device, bool isUSB)
		{
			TempConfigSafeCall((x) =>
			{
				ServerHelper.SynchronizeTime(x);
			}, device, isUSB);
		}

		public static string DeviceGetInformation(Device device, bool isUSB)
		{
			return TempConfigSafeCall<string>((x) =>
			{
				var getConfigurationOperationHelper = new GetConfigurationOperationHelper(false);
				return getConfigurationOperationHelper.GetDeviceInformation(device);
			}, device, isUSB);
		}

		public static List<string> DeviceGetSerialList(Device device)
		{
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
			return TempConfigSafeCall<string>((x) =>
			{
				return FirmwareUpdateOperationHelper.Verify(device, isUSB, fileName);
			}, device, isUSB);
		}

		public static void DeviceUpdateFirmware(Device device, bool isUSB, string fileName)
		{
			TempConfigSafeCall((x) =>
			{
				FirmwareUpdateOperationHelper.Update(device, isUSB, fileName);
			}, device, isUSB);
		}

		public static DeviceConfiguration DeviceReadConfiguration(Device device, bool isUSB)
		{
			return TempConfigSafeCall<DeviceConfiguration>((x) =>
			{
				var getConfigurationOperationHelper = new GetConfigurationOperationHelper(false);
				return getConfigurationOperationHelper.GetDeviceConfiguration(x);
			}, device, isUSB);
		}

		public static FS2JournalItemsCollection DeviceReadJournal(Device device, bool isUSB)
		{
			return TempConfigSafeCall<FS2JournalItemsCollection>((x) =>
			{
				return ReadJournalOperationHelper.GetJournalItems(x);
			}, device, isUSB);
		}

		public static DeviceConfiguration DeviceAutoDetectChildren(Device device, bool fastSearch)
		{
			var rootDevice = AutoDetectOperationHelper.AutoDetectDevice();
			var deviceConfiguration = new DeviceConfiguration()
			{
				RootDevice = rootDevice
			};
			return deviceConfiguration;
		}

		public static List<DeviceCustomFunction> DeviceGetCustomFunctions(DriverType driverType)
		{
			return DeviceCustomFunctionListHelper.GetDeviceCustomFunctionList(driverType);
		}

		public static void DeviceExecuteCustomFunction(Device device, bool isUSB, string functionName)
		{
			TempConfigSafeCall((x) =>
			{
				throw new FS2Exception("Функция пока не реализована");
			}, device, isUSB);
		}

		public static List<GuardUser> DeviceGetGuardUsers(Device device)
		{
			return GuardUsersOperationHelper.DeviceGetGuardUsers(device);
		}

		public static void DeviceSetGuardUsers(Device device, List<GuardUser> guardUsers)
		{
			DeviceSetGuardUsers(device, guardUsers);
		}

		public static string DeviceGetMDS5Data(Device device)
		{
			throw new FS2Exception("Функция пока не реализована");
		}

		public static void SetConfigurationParameters(Device device, List<Property> properties)
		{
			DeviceParametersOperationHelper.SetDeviceParameters(device, properties);
		}

		public static List<Property> GetConfigurationParameters(Device device)
		{
			return DeviceParametersOperationHelper.GetDeviceParameters(device);
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