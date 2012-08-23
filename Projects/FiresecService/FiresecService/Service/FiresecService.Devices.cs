using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecService.Configuration;
using FiresecService.Processor;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public OperationResult<bool> SetDeviceConfiguration(DeviceConfiguration deviceConfiguration)
		{
            if (!AppSettings.IsFSEnabled)
            {
                return new OperationResult<bool>("Сервер работает в конфигурации без поддержки адресной системы");
            }
			Watcher.Ignore = true;
			try
			{
				FiresecManager.ConfigurationConverter.Update(deviceConfiguration);
				foreach (var device in deviceConfiguration.Devices)
				{
					device.PlaceInTree = device.GetPlaceInTree();
				}
				ConfigurationFileManager.SetDeviceConfiguration(deviceConfiguration);
				ConfigurationCash.DeviceConfiguration = deviceConfiguration;

				FiresecManager.ConvertBack(deviceConfiguration, true);

				OperationResult<bool> result = null;
				if (!AppSettings.DoNotOverrideFiresec1Config)
				{
					result = FiresecSerializedClient.SetNewConfig(FiresecManager.ConfigurationConverter.FiresecConfiguration).ToOperationResult();
				}

				var thread = new Thread(new ThreadStart(() => { ClientsCash.OnConfigurationChanged(); }));
				thread.Start();

				return result;
			}
			catch (Exception e)
			{
				return new OperationResult<bool>() { HasError = true, Error = e.Message, Result = false };
			}
			finally
			{
				Watcher.Ignore = false;
			}
		}

		public OperationResult<bool> DeviceWriteConfiguration(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			var firesecConfiguration = FiresecManager.ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceWriteConfig(firesecConfiguration, device.GetPlaceInTree()).ToOperationResult();
		}

		[Obsolete]
		public OperationResult<bool> DeviceWriteAllConfiguration(DeviceConfiguration deviceConfiguration)
		{
			OperationResult<bool> result = null;
			var firesecConfiguration = FiresecManager.ConvertBack(deviceConfiguration, false);
			foreach (var device in deviceConfiguration.Devices)
			{
				if (device.Driver.CanWriteDatabase)
				{
					result = FiresecSerializedClient.DeviceWriteConfig(firesecConfiguration, device.GetPlaceInTree()).ToOperationResult();
					if (result.HasError)
					{
						return result;
					}
				}
			}

			return result;
		}

		public OperationResult<bool> DeviceSetPassword(DeviceConfiguration deviceConfiguration, Guid deviceUID, DevicePasswordType devicePasswordType, string password)
		{
			var firesecConfiguration = FiresecManager.ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceSetPassword(firesecConfiguration, device.GetPlaceInTree(), password, (int)devicePasswordType).ToOperationResult();
		}

		public OperationResult<bool> DeviceDatetimeSync(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			var firesecConfiguration = FiresecManager.ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceDatetimeSync(firesecConfiguration, device.GetPlaceInTree()).ToOperationResult();
		}

		public OperationResult<string> DeviceGetInformation(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			var firesecConfiguration = FiresecManager.ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceGetInformation(firesecConfiguration, device.GetPlaceInTree()).ToOperationResult();
		}

		public OperationResult<List<string>> DeviceGetSerialList(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			var firesecConfiguration = FiresecManager.ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			var result = FiresecSerializedClient.DeviceGetSerialList(firesecConfiguration, device.GetPlaceInTree()).ToOperationResult();

			var operationResult = new OperationResult<List<string>>()
			{
				HasError = result.HasError,
				Error = result.Error
			};
			if (result.Result != null)
				operationResult.Result = result.Result.Split(';').ToList();
			return operationResult;
		}

		public OperationResult<string> DeviceUpdateFirmware(DeviceConfiguration deviceConfiguration, Guid deviceUID, byte[] bytes, string fileName)
		{
			fileName = Guid.NewGuid().ToString();
			Directory.CreateDirectory("Temp");
			fileName = Directory.GetCurrentDirectory() + "\\Temp\\" + fileName;
			using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
			{
				stream.Write(bytes, 0, bytes.Length);
			}

			var firesecConfiguration = FiresecManager.ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceUpdateFirmware(firesecConfiguration, device.GetPlaceInTree(), fileName).ToOperationResult();
		}

		public OperationResult<string> DeviceVerifyFirmwareVersion(DeviceConfiguration deviceConfiguration, Guid deviceUID, byte[] bytes, string fileName)
		{
			fileName = Guid.NewGuid().ToString();
			Directory.CreateDirectory("Temp");
			fileName = Directory.GetCurrentDirectory() + "\\Temp\\" + fileName;
			using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
			{
				stream.Write(bytes, 0, bytes.Length);
			}

			var firesecConfiguration = FiresecManager.ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceVerifyFirmwareVersion(firesecConfiguration, device.GetPlaceInTree(), fileName).ToOperationResult();
		}

		public OperationResult<string> DeviceReadEventLog(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			var firesecConfiguration = FiresecManager.ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceReadEventLog(firesecConfiguration, device.GetPlaceInTree()).ToOperationResult();
		}

		public OperationResult<DeviceConfiguration> DeviceAutoDetectChildren(DeviceConfiguration deviceConfiguration, Guid deviceUID, bool fastSearch)
		{
			var firesecConfiguration = FiresecManager.ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			var result = FiresecSerializedClient.DeviceAutoDetectChildren(firesecConfiguration, device.GetPlaceInTree(), fastSearch);

			var operationResult = new OperationResult<DeviceConfiguration>()
			{
				HasError = result.HasError,
				Error = result.ErrorString
			};

			var configurationManager = new ConfigurationConverter();
			operationResult.Result = configurationManager.ConvertOnlyDevices(result.Result);
			return operationResult;
		}

		public OperationResult<DeviceConfiguration> DeviceReadConfiguration(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			var firesecConfiguration = FiresecManager.ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			var result = FiresecSerializedClient.DeviceReadConfig(firesecConfiguration, device.GetPlaceInTree());

			var operationResult = new OperationResult<DeviceConfiguration>()
			{
				HasError = result.HasError,
				Error = result.ErrorString
			};

			var configurationManager = new ConfigurationConverter();
			operationResult.Result = configurationManager.ConvertOnlyDevices(result.Result);
			return operationResult;
		}

		public OperationResult<List<DeviceCustomFunction>> DeviceCustomFunctionList(Guid driverUID)
		{
			var result = FiresecSerializedClient.DeviceCustomFunctionList(driverUID.ToString().ToUpper());

			var operationResult = new OperationResult<List<DeviceCustomFunction>>()
			{
				HasError = result.HasError,
				Error = result.ErrorString
			};

			operationResult.Result = DeviceCustomFunctionConverter.Convert(result.Result);
			return operationResult;
		}

		public OperationResult<string> DeviceCustomFunctionExecute(DeviceConfiguration deviceConfiguration, Guid deviceUID, string functionName)
		{
			var firesecConfiguration = FiresecManager.ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceCustomFunctionExecute(firesecConfiguration, device.GetPlaceInTree(), functionName).ToOperationResult();
		}

		public OperationResult<string> DeviceGetGuardUsersList(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			var firesecConfiguration = FiresecManager.ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceGetGuardUsersList(firesecConfiguration, device.GetPlaceInTree()).ToOperationResult();
		}

		public OperationResult<bool> DeviceSetGuardUsersList(DeviceConfiguration deviceConfiguration, Guid deviceUID, string users)
		{
			var firesecConfiguration = FiresecManager.ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceSetGuardUsersList(firesecConfiguration, device.GetPlaceInTree(), users).ToOperationResult();
		}

		public OperationResult<string> DeviceGetMDS5Data(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			var firesecConfiguration = FiresecManager.ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceGetMDS5Data(firesecConfiguration, device.GetPlaceInTree()).ToOperationResult();
		}

		public void AddToIgnoreList(List<Guid> deviceGuids)
		{
			var devicePaths = new List<string>();
			foreach (var guid in deviceGuids)
			{
				var device = ConfigurationCash.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == guid);
				devicePaths.Add(device.PlaceInTree);
			}

			FiresecSerializedClient.AddToIgnoreList(devicePaths);
		}

		public void RemoveFromIgnoreList(List<Guid> deviceGuids)
		{
			var devicePaths = new List<string>();
			foreach (var guid in deviceGuids)
			{
				var device = ConfigurationCash.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == guid);
				devicePaths.Add(device.PlaceInTree);
			}

			FiresecSerializedClient.RemoveFromIgnoreList(devicePaths);
		}

		public void ResetStates(List<ResetItem> resetItems)
		{
			var firesecResetHelper = new FiresecResetHelper(FiresecManager);
			firesecResetHelper.ResetStates(resetItems);
		}

		public void SetZoneGuard(Guid secPanelUID, int localZoneNo)
		{
			var device = ConfigurationCash.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.UID == secPanelUID);
			if (device != null)
			{
				int reguestId = 0;
				FiresecSerializedClient.ExecuteRuntimeDeviceMethod(device.PlaceInTree, "SetZoneToGuard", localZoneNo.ToString(), ref reguestId);
			}
		}

		public void UnSetZoneGuard(Guid secPanelUID, int localZoneNo)
		{
			var device = ConfigurationCash.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.UID == secPanelUID);
			if (device != null)
			{
				int reguestId = 0;
				FiresecSerializedClient.ExecuteRuntimeDeviceMethod(device.PlaceInTree, "UnSetZoneFromGuard", localZoneNo.ToString(), ref reguestId);
			}
		}

		public void AddUserMessage(string message)
		{
			FiresecSerializedClient.AddUserMessage(message);
		}

		public OperationResult<bool> ExecuteCommand(Guid deviceUID, string methodName)
		{
			var device = ConfigurationCash.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				FiresecSerializedClient.ExecuteCommand(device.PlaceInTree, methodName).ToOperationResult();
			}
			var operationResult = new OperationResult<bool>()
			{
				Result = false,
				HasError = true,
				Error = "Не найдено устройство по идентификатору"
			};
			return operationResult;
		}

		public OperationResult<bool> CheckHaspPresence()
		{
            if (!AppSettings.IsFSEnabled)
                return new OperationResult<bool>() { Result = true };

			return FiresecSerializedClient.CheckHaspPresence().ToOperationResult();
		}
	}
}