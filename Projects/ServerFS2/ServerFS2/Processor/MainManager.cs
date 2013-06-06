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
		public static event Action<FS2JournalItem> NewJournalItem;
		
		public static void StartMonitoring()
		{
			MonitoringDevice.NewJournalItem += new Action<FS2JournalItem>(OnNewItem);
			MonitoringProcessor.StartMonitoring();
		}

		static void OnNewItem(FS2JournalItem journalItem)
		{
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

		public static bool SetNewConfig(DeviceConfiguration deviceConfiguration)
		{
			ConfigurationManager.DeviceConfiguration = deviceConfiguration;
			ConfigurationManager.Update();
			return true;
		}

		public static bool DeviceWriteConfig(Device device, bool isUSB)
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

			return true;
		}

		public static bool DeviceSetPassword(Device device, bool isUSB, DevicePasswordType devicePasswordType, string password)
		{
			throw new FS2Exception("Функция пока не реализована");
		}

		public static void DeviceDatetimeSync(Device device, bool isUSB)
		{
			Trace.WriteLine("DeviceDatetimeSync Start");
			for (int i = 0; i < 10; i++)
			{
				Trace.WriteLine("DeviceDatetimeSync i=" + i.ToString());
				FS2Contract.CheckCancellationRequested();
				CallbackManager.Add(new FS2Callbac() { FS2ProgressInfo = new FS2ProgressInfo("Test Callbac " + i.ToString()) });
				Thread.Sleep(1000);
			}
			return;

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
			switch(driverType)
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

		public static string DeviceCustomFunctionExecute(Device device, bool isUSB, string functionName)
		{
			throw new FS2Exception("Функция пока не реализована");
		}

		public static string DeviceGetGuardUsersList(Device device)
		{
			throw new FS2Exception("Функция пока не реализована");
		}

		public static bool DeviceSetGuardUsersList(Device device, string users)
		{
			throw new FS2Exception("Функция пока не реализована");
		}

		public static string DeviceGetMDS5Data(Device device)
		{
			throw new FS2Exception("Функция пока не реализована");
		}

		public static bool SetConfigurationParameters(Device device, List<Property> properties)
		{
			throw new FS2Exception("Функция пока не реализована");
		}

		public static List<Property> GetConfigurationParameters(Device device)
		{
			throw new FS2Exception("Функция пока не реализована");
		}

        public static void ResetFire(Device device)
        {
            ServerHelper.ResetFire(device);
        }

        public static void ResetTest(Device device, List<byte> status)
        {
            ServerHelper.ResetTest(device, status);
        }

		public static void ResetPanelBit(Device device, List<byte> statusBytes, int bitNo)
		{
			ServerHelper.ResetPanelBit(device, statusBytes, bitNo);
		}

        public static void AddDeviceToCheckList(Device device)
        {
            ServerHelper.AddDeviceToCheckList(device);
        }

        public static void RemoveDeviceFromCheckList(Device device)
        {
            ServerHelper.RemoveDeviceFromCheckList(device);
        }

        public static List<byte> GetDeviceStatus(Device device)
        {
            return ServerHelper.GetDeviceStatus(device);
        }

		public static void ResetStates(List<ResetItem> resetItems)
		{

		}
	}
}