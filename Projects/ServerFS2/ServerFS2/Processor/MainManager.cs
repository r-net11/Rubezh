using FiresecAPI.Models;
using System;
using System.Collections.Generic;
using FS2Api;
using ServerFS2.Service;

namespace ServerFS2.Processor
{
	public static class MainManager
	{
		public static void StartMonitoring()
		{

		}

		public static void StopMonitoring()
		{

		}

		public static void SuspendMonitoring()
		{
			CallbackManager.Add(new FS2Callbac() { FS2ProgressInfo = new FS2ProgressInfo("Приостановка мониторинга") });
		}

		public static void ResumeMonitoring()
		{
			CallbackManager.Add(new FS2Callbac() { FS2ProgressInfo = new FS2ProgressInfo("Возобновление мониторинга") });
		}

		public static void CancelProgress()
		{
			CallbackManager.Add(new FS2Callbac() { FS2ProgressInfo = new FS2ProgressInfo("Отмена операции") });
		}

		public static bool SetNewConfig(DeviceConfiguration deviceConfiguration)
		{
			return true;
		}

		public static bool DeviceWriteConfig(Device device, bool isUSB)
		{
			return true;
		}

		public static bool DeviceSetPassword(Device device, bool isUSB, DevicePasswordType devicePasswordType, string password)
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

		public static string DeviceReadEventLog(Device device, bool isUSB)
		{
			throw new FS2Exception("Функция пока не реализована");
		}

		public static DeviceConfiguration DeviceAutoDetectChildren(Device device, bool fastSearch)
		{
			return new DeviceConfiguration();
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
	}
}