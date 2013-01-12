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
                return null;
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

        public DriversConfiguration ConvertMetadataFromFiresec()
        {
            var driversConfiguration = new DriversConfiguration();
            var coreDriversConfig = FiresecSerializedClient.GetMetaData().Result;
            if (coreDriversConfig == null)
                return null;
            foreach (var innerDriver in coreDriversConfig.drv)
            {
                var driver = DriverConverter.Convert(coreDriversConfig, innerDriver);
                if (driver == null)
                {
                    Logger.Error("Не удается найти данные для драйвера " + innerDriver.name);
                    LoadingErrorManager.Add("Не удается найти данные для драйвера " + innerDriver.name);
                }
                else
                {
                    if (driver.IsIgnore == false)
                        driversConfiguration.Drivers.Add(driver);
                }
            }
            DriverConfigurationParametersHelper.CreateKnownProperties(driversConfiguration.Drivers);
            return driversConfiguration;
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
				//var result = FiresecSerializedClient.GetCoreConfig();
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
                throw new FiresecException("Нулевая кофигурация устройств при обновлении");
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

		public void SynchronyzeConfiguration()
		{
            var result = FiresecSerializedClient.GetCoreConfig();
            if (result.HasError || result.Result == null)
			{
				LoadingErrorManager.Add("Ошибка при пполучении конфигурации устройств из драйвера Firesec");
			}

            var firesecDeviceConfiguration = ConvertOnlyDevices(result.Result);
			Update(firesecDeviceConfiguration);
			Update(ConfigurationCash.DeviceConfiguration);
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
					LoadingErrorManager.Add("Для устройства " + device.PresentationAddressAndName + " не найдено устройство в конфигурации firesec-1");
				}
			}
            foreach (var firesecDevice in firesecDeviceConfiguration.Devices)
            {
                var device = ConfigurationCash.DeviceConfiguration.Devices.FirstOrDefault(x => x.PathId == firesecDevice.PathId);
                if (device == null)
                {
					LoadingErrorManager.Add("Для устройства " + firesecDevice.PresentationAddressAndName + " не найдено устройство в конфигурации firesec-2");
                }
            }
		}
	}
}