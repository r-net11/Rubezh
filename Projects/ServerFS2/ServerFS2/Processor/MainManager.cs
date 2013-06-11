using FiresecAPI.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using FS2Api;
using ServerFS2.Service;
using ServerFS2.ConfigurationWriter;
using System.Threading;
using System.Diagnostics;
using ServerFS2.Monitor;

namespace ServerFS2.Processor
{
	public static class MainManager
	{
		#region Common
		public static event Action<FS2JournalItem> NewJournalItem;

		public static void StartMonitoring()
		{
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
		}

		public static void SuspendMonitoring()
		{
			CallbackManager.Add(new FS2Callbac() { FS2ProgressInfo = new FS2ProgressInfo("Приостановка мониторинга") });
		}

		public static void ResumeMonitoring()
		{
			CallbackManager.Add(new FS2Callbac() { FS2ProgressInfo = new FS2ProgressInfo("Возобновление мониторинга") });
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
			MonitoringProcessor.AddCommand(device, commandName);
		}
		#endregion

		#region Administrator
		public static void SetNewConfig(DeviceConfiguration deviceConfiguration)
		{
			ConfigurationManager.DeviceConfiguration = deviceConfiguration;
			ConfigurationManager.Update();
		}

		public static void DeviceWriteConfig(Device device, bool isUSB)
		{
			CallbackManager.Add(new FS2Callbac() { FS2ProgressInfo = new FS2ProgressInfo("Формирование базы данных устройств") });
			var systemDatabaseCreator = new SystemDatabaseCreator();
			systemDatabaseCreator.Run();

			var panelDatabase = systemDatabaseCreator.PanelDatabases.FirstOrDefault(x => x.ParentPanel.UID == device.UID);
			if (panelDatabase == null)
				throw new FS2Exception("Не найдена сформированная для устройства база данных");

			var parentPanel = panelDatabase.ParentPanel;
			var bytes1 = panelDatabase.RomDatabase.BytesDatabase.GetBytes();
			var bytes2 = panelDatabase.FlashDatabase.BytesDatabase.GetBytes();
			CallbackManager.Add(new FS2Callbac() { FS2ProgressInfo = new FS2ProgressInfo("Запись базы данных в прибор") });
			ServerHelper.SetDeviceConfig(parentPanel, bytes2, bytes1);
		}

		public static void DeviceSetPassword(Device device, bool isUSB, DevicePasswordType devicePasswordType, string password)
		{
			throw new FS2Exception("Функция пока не реализована");
		}

		public static void DeviceDatetimeSync(Device device, bool isUSB)
		{
			ServerHelper.SynchronizeTime(device);
		}

		public static string DeviceGetInformation(Device device, bool isUSB)
		{
			throw new FS2Exception("Функция пока не реализована");
		}

		public static List<string> DeviceGetSerialList(Device device)
		{
			throw new FS2Exception("Функция пока не реализована");
		}

		public static string DeviceUpdateFirmware(Device device, bool isUSB, string fileName)
		{
			throw new FS2Exception("Функция пока не реализована");
		}

		public static string DeviceVerifyFirmwareVersion(Device device, bool isUSB, string fileName)
		{
			throw new FS2Exception("Функция пока не реализована");
		}

		public static DeviceConfiguration DeviceReadConfig(Device device, bool isUSB)
		{
			return ServerHelper.GetDeviceConfig(device);
		}

		public static List<FS2JournalItem> DeviceReadEventLog(Device device, bool isUSB)
		{
			return ServerHelper.GetJournalItems(device);
		}

		public static DeviceConfiguration DeviceAutoDetectChildren(Device device, bool fastSearch)
		{
			var rootDevice = ServerHelper.AutoDetectDevice();
			var deviceConfiguration = new DeviceConfiguration()
			{
				RootDevice = rootDevice
			};
			return deviceConfiguration;
		}

		public static List<DeviceCustomFunction> DeviceCustomFunctionList(DriverType driverType)
		{
			switch (driverType)
			{
				case DriverType.Rubezh_2AM:
					return new List<DeviceCustomFunction>()
					{
						new DeviceCustomFunction()
						{
							Code = "Set_BlindMode",
							Name = "Установить режим \"глухой панели\"",
							Description = "Установить режим \"глухой панели\"",
						},
						new DeviceCustomFunction()
						{
							Code = "Reset_BlindMode",
							Name = "Снять режим \"глухой панели\"",
							Description = "Снять режим \"глухой панели\"",
						}
					};
					break;
			}
			throw new FS2Exception("Функция пока не реализована");
		}

		public static void DeviceCustomFunctionExecute(Device device, bool isUSB, string functionName)
		{
			throw new FS2Exception("Функция пока не реализована");
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
	}
}