using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace Firesec
{
	public partial class ConfigurationConverter
	{
		FiresecSerializedClient FiresecSerializedClient;

		public ConfigurationConverter(FiresecSerializedClient firesecSerializedClient)
		{
			FiresecSerializedClient = firesecSerializedClient;
		}

		public OperationResult<DeviceConfiguration> ConvertCoreConfig()
		{
			var result = FiresecSerializedClient.GetCoreConfig();
			if (result.HasError)
			{
				return new OperationResult<DeviceConfiguration>(result.Error);
			}
			var coreConfig = result.Result;
			if (coreConfig == null)
				return null;

			var deviceConfiguration = new DeviceConfiguration();
			ConvertZones(deviceConfiguration, coreConfig);
			ConvertDirections(deviceConfiguration, coreConfig);
			ConvertGuardUsers(deviceConfiguration, coreConfig);
			ConvertDevices(deviceConfiguration, coreConfig);
			Update(deviceConfiguration);
			return new OperationResult<DeviceConfiguration>() { Result = deviceConfiguration };
		}

		public OperationResult<PlansConfiguration> ConvertPlans(DeviceConfiguration deviceConfiguration)
		{
			var result = FiresecSerializedClient.GetPlans();
			if (result.HasError)
			{
				return new OperationResult<PlansConfiguration>(result.Error);
			}
			var plans = result.Result;
			if (plans == null)
				return new OperationResult<PlansConfiguration>();
			var plansConfiguration = ConvertPlans(plans, deviceConfiguration);
			return new OperationResult<PlansConfiguration>() { Result = plansConfiguration };
		}

		public DeviceConfiguration ConvertDevicesAndZones(Models.CoreConfiguration.config coreConfig)
		{
			var deviceConfiguration = new DeviceConfiguration();
			ConvertZones(deviceConfiguration, coreConfig);
			ConvertDevices(deviceConfiguration, coreConfig);
			return deviceConfiguration;
		}

		public DeviceConfiguration ConvertOnlyDevices(Firesec.Models.CoreConfiguration.config coreConfig)
		{
			var deviceConfiguration = new DeviceConfiguration();
			ConvertDevices(deviceConfiguration, coreConfig);
			return deviceConfiguration;
		}

		public OperationResult<DriversConfiguration> ConvertMetadataFromFiresec()
		{
			var driversConfiguration = new DriversConfiguration();
			var metadataResult = FiresecSerializedClient.GetMetaData();
			if (metadataResult.HasError)
			{
				LoadingErrorManager.Add(metadataResult.Error);
				return new OperationResult<DriversConfiguration>(metadataResult.Error);
			}
			var coreDriversConfig = metadataResult.Result;
			if (coreDriversConfig == null)
			{
				return new OperationResult<DriversConfiguration>("Список драйверов пуст");
			}

			foreach (var innerDriver in coreDriversConfig.drv)
			{
				var driver = DriverConverter.Convert(coreDriversConfig, innerDriver);
				if (driver == null)
				{
					if (innerDriver.name != "БУНС-2" && innerDriver.name != "USB БУНС-2")
					{
						Logger.Error("Не удается найти данные для драйвера " + innerDriver.name);
						LoadingErrorManager.Add("Не удается найти данные для драйвера " + innerDriver.name);
					}
				}
				else
				{
					if (driver.IsIgnore == false)
						driversConfiguration.Drivers.Add(driver);
				}
			}

			DriverConfigurationParametersHelper.CreateKnownProperties(driversConfiguration.Drivers);
#if DEBUG
			//ZipSerializeHelper.Serialize<DriversConfiguration>(driversConfiguration, "C:/DriversConfiguration.xml");
#endif
			return new OperationResult<DriversConfiguration>() { Result = driversConfiguration };
		}

		public Firesec.Models.CoreConfiguration.config ConvertBack(DeviceConfiguration deviceConfiguration, bool includeSecurity)
		{
			deviceConfiguration.Update();

			foreach (var device in deviceConfiguration.Devices)
			{
				device.Driver = ConfigurationCash.DriversConfiguration.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
			}

			var coreConfig = new Firesec.Models.CoreConfiguration.config();
			if (includeSecurity)
			{
				var result = EmptyConfigHelper.GetCoreConfig();
				if (result.HasError)
				{
					return null;
				}
				coreConfig = result.Result;
				if (coreConfig == null)
				{
					Logger.Error("ConfigurationConverter.FiresecConfiguration == null");
					return null;
				}
				coreConfig.part = null;
			}

			int gid = 0;
			ConvertZonesBack(deviceConfiguration, coreConfig);
			ConvertDevicesBack(deviceConfiguration, coreConfig);
			ConvertDirectionsBack(deviceConfiguration, coreConfig, ref gid);
			ConvertGuardUsersBack(deviceConfiguration, coreConfig, ref gid);

			return coreConfig;
		}

		void Update(DeviceConfiguration deviceConfiguration = null)
		{
			if (deviceConfiguration == null)
			{
				Logger.Error("ConfigurationConverter.Update deviceConfiguration = null");
				throw new Exception("Нулевая кофигурация устройств при обновлении");
			}

			var hasInvalidDriver = false;
			deviceConfiguration.Update();
			foreach (var device in deviceConfiguration.Devices)
			{
				device.Driver = ConfigurationCash.DriversConfiguration.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
				if (device.Driver == null)
				{
					hasInvalidDriver = true;
					if (device.Parent != null)
						device.Parent.Children.Remove(device);
				}
			}
			if (hasInvalidDriver)
				deviceConfiguration.Update();

			deviceConfiguration.UpdateIdPath();
		}

		public void SynchronyzeConfiguration(bool removeMissing = false)
		{
			var result = FiresecSerializedClient.GetCoreConfig();
			if (result.HasError || result.Result == null)
			{
				LoadingErrorManager.Add("Ошибка при пполучении конфигурации устройств из драйвера Firesec");
			}

			var firesecDeviceConfiguration = ConvertOnlyDevices(result.Result);
			Update(firesecDeviceConfiguration);
			Update(ConfigurationCash.DeviceConfiguration);

			var missingDevices = new List<Device>();
			foreach (var device in ConfigurationCash.DeviceConfiguration.Devices)
			{
				var firesecDevice = firesecDeviceConfiguration.Devices.FirstOrDefault(x => x.PathId == device.PathId);
				if (firesecDevice != null)
				{
					device.PlaceInTree = firesecDevice.GetPlaceInTree();
					device.DatabaseId = firesecDevice.DatabaseId;
				}
				else
				{
					missingDevices.Add(device);
					LoadingErrorManager.Add("Для устройства " + device.PresentationAddressAndName + " не найдено устройство в конфигурации firesec-1");
				}
			}
			if (removeMissing)
			{
				foreach (var device in missingDevices)
				{
					if (device.Parent != null)
					{
						device.Parent.Children.Remove(device);
					}
				}
				if (missingDevices.Count > 0)
				{
					ConfigurationCash.DeviceConfiguration.Update();
				}
			}

			foreach (var firesecDevice in firesecDeviceConfiguration.Devices)
			{
				var device = ConfigurationCash.DeviceConfiguration.Devices.FirstOrDefault(x => x.PathId == firesecDevice.PathId);
				if (device == null)
				{
					LoadingErrorManager.Add("Для устройства " + firesecDevice.PresentationAddressAndName + " не найдено устройство в конфигурации firesec");
				}
			}
		}
	}
}