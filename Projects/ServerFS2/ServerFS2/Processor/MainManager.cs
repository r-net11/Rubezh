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

namespace ServerFS2.Processor
{
	public static class MainManager
	{
		#region Common
		public static event Action<FS2JournalItem> NewJournalItem;

		public static void StartMonitoring()
		{
			USBManager.Initialize();
			MonitoringDevice.NewJournalItem -= new Action<FS2JournalItem>(OnNewItem);
			MonitoringDevice.NewJournalItem += new Action<FS2JournalItem>(OnNewItem);
			MonitoringProcessor.StartMonitoring();
		}

		static void OnNewItem(FS2JournalItem journalItem)
		{
			if (NewJournalItem != null)
				NewJournalItem(journalItem);
		}

		public static void StopMonitoring()
		{
			MonitoringProcessor.StopMonitoring();
			USBManager.Dispose();
		}

		public static void SuspendMonitoring()
		{
			CallbackManager.Add(new FS2Callbac() { FS2ProgressInfo = new FS2ProgressInfo("Приостановка мониторинга") });
			MonitoringProcessor.SuspendMonitoring();
		}

		public static void ResumeMonitoring()
		{
			CallbackManager.Add(new FS2Callbac() { FS2ProgressInfo = new FS2ProgressInfo("Возобновление мониторинга") });
			MonitoringProcessor.ResumeMonitoring();
		}
		#endregion

		#region Monitoring
		public static List<DeviceState> GetDeviceStates()
		{
			var deviceStates = new List<DeviceState>();
			foreach (var device in ConfigurationManager.Devices)
			{
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
			foreach (var device in devices)
			{
				MonitoringProcessor.AddTaskIgnore(device);
			}
		}

		public static void RemoveFromIgnoreList(List<Device> devices)
		{
			foreach (var device in devices)
			{
				MonitoringProcessor.AddTaskResetIgnore(device);
			}
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
			MonitoringProcessor.AddPanelResetItems(panelResetItems);
		}

		public static void ExecuteCommand(Guid deviceUID, string methodName)
		{
			throw new FS2Exception("Функция пока не реализована");
		}

		public static void AddCommand(Device device, string commandName)
		{
			MonitoringProcessor.ExecuteCommand(device, commandName);
		}
		#endregion

		#region Administrator
		public static void SetNewConfig(DeviceConfiguration deviceConfiguration)
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
				StartMonitoring();
			}
		}

		public static void DeviceWriteConfig(Device device, bool isUSB)
		{
			TempConfigSafeCall((x) =>
			{
				ConfigurationWriterOperationHelper.Write(x);
			}, device, isUSB);
		}

		public static void DeviceSetPassword(Device device, bool isUSB, DevicePasswordType devicePasswordType, string password)
		{
			TempConfigSafeCall((x) =>
			{
				throw new FS2Exception("Функция пока не реализована");
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
				throw new FS2Exception("Функция пока не реализована");
			}, device, isUSB);
		}

		public static List<string> DeviceGetSerialList(Device device)
		{
			return USBManager.GetAllSerialNos();
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

		public static DeviceConfiguration DeviceReadConfig(Device device, bool isUSB)
		{
			return TempConfigSafeCall<DeviceConfiguration>((x) =>
			{
				var getConfigurationOperationHelper = new GetConfigurationOperationHelper(false);
				return getConfigurationOperationHelper.GetDeviceConfig(x);
			}, device, isUSB);
		}

		public static List<FS2JournalItem> DeviceReadEventLog(Device device, bool isUSB)
		{
			return TempConfigSafeCall<List<FS2JournalItem>>((x) =>
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

		public static List<DeviceCustomFunction> DeviceCustomFunctionList(DriverType driverType)
		{
			return DeviceCustomFunctionListHelper.GetDeviceCustomFunctionList(driverType);
		}

		public static void DeviceCustomFunctionExecute(Device device, bool isUSB, string functionName)
		{
			TempConfigSafeCall((x) =>
			{
				throw new FS2Exception("Функция пока не реализована");
			}, device, isUSB);
		}

		public static string DeviceGetGuardUsersList(Device device)
		{
			throw new FS2Exception("Функция пока не реализована");
		}

		public static void DeviceSetGuardUsersList(Device device, string users)
		{
			throw new FS2Exception("Функция пока не реализована");
		}

		public static string DeviceGetMDS5Data(Device device)
		{
			throw new FS2Exception("Функция пока не реализована");
		}

		public static void SetConfigurationParameters(Device device, List<Property> properties)
		{
			throw new FS2Exception("Функция пока не реализована");
		}

		public static List<Property> GetConfigurationParameters(Device device)
		{
			throw new FS2Exception("Функция пока не реализована");
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
				if (isUSB)
				{
					USBConfigHelper.SetCurrentDeviceConfiguration();
				}
			}
		}
		#endregion
	}
}