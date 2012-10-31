using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using FiresecAPI.Models;
using System.Diagnostics;
using XFiresecAPI;

namespace Firesec
{
	public partial class ConfigurationConverter
	{
		DeviceConfiguration DeviceConfiguration { get; set; }
		public FiresecSerializedClient FiresecSerializedClient;
		public Firesec.Models.CoreConfiguration.config FiresecConfiguration { get; set; }
		int Gid { get; set; }
		public StringBuilder DriversError { get; private set; }

		public ConfigurationConverter()
		{
			DriversError = new StringBuilder();
		}

		public void Convert()
		{
			FiresecConfiguration = FiresecSerializedClient.GetCoreConfig().Result;
			if (FiresecConfiguration == null)
				FiresecConfiguration = new Models.CoreConfiguration.config();
			DeviceConfiguration = new DeviceConfiguration();
			ConvertZones();
			ConvertDirections();
			ConvertGuardUsers();
			ConvertDevices();
			ConfigurationCash.DeviceConfiguration = DeviceConfiguration;
            Update(ConfigurationCash.DeviceConfiguration);
			var plans = FiresecSerializedClient.GetPlans().Result;
			ConfigurationCash.PlansConfiguration = ConvertPlans(plans);
		}

        public Firesec.Models.CoreConfiguration.config ConvertBack(DeviceConfiguration deviceConfiguration, bool includeSecurity)
		{
			DeviceConfiguration = deviceConfiguration;
			DeviceConfiguration.Update();

			foreach (var device in DeviceConfiguration.Devices)
			{
				device.Driver = ConfigurationCash.DriversConfiguration.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
			}

			if (includeSecurity)
			{
				FiresecConfiguration = FiresecSerializedClient.GetCoreConfig().Result;
                if (FiresecConfiguration == null)
                {
                    FiresecConfiguration = new Models.CoreConfiguration.config();
                    Logger.Error("ConfigurationConverter.FiresecConfiguration == null");
                }
			}
			else
			{
				FiresecConfiguration = new Firesec.Models.CoreConfiguration.config();
			}
            FiresecConfiguration.part = null;

			Gid = 0;
			ConvertZonesBack();
			ConvertDevicesBack();
			ConvertDirectionsBack();
			ConvertGuardUsersBack();

            return FiresecConfiguration;
		}

		public DeviceConfiguration ConvertOnlyDevices(Firesec.Models.CoreConfiguration.config firesecConfiguration)
		{
			DeviceConfiguration = new DeviceConfiguration();
			FiresecConfiguration = firesecConfiguration;
			ConvertDevices();
			return DeviceConfiguration;
		}

		public void ConvertMetadataFromFiresec()
		{
			DriverConverter.Metadata = FiresecSerializedClient.GetMetaData().Result;
			ConfigurationCash.DriversConfiguration = new DriversConfiguration();
			foreach (var innerDriver in DriverConverter.Metadata.drv)
			{
				var driver = DriverConverter.Convert(innerDriver);
				if (driver == null)
				{
					Logger.Error("Не удается найти данные для драйвера " + innerDriver.name);
					DriversError.AppendLine("Не удается найти данные для драйвера " + innerDriver.name);
				}
				else
				{
					if (driver.IsIgnore == false)
						ConfigurationCash.DriversConfiguration.Drivers.Add(driver);
				}
			}
			DriverConfigurationParametersHelper.CreateKnownProperties(ConfigurationCash.DriversConfiguration.Drivers);
		}

        void Update(DeviceConfiguration deviceConfiguration = null)
        {
			if (deviceConfiguration == null)
			{
				Logger.Error("ConfigurationConverter.Update deviceConfiguration = null");
                return;
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
			if (result.HasError)
			{
				Logger.Error("SynchronyzeConfiguration FiresecSerializedClient.GetCoreConfig HasError" + result.Error);
                DriversError.AppendLine("Ошибка при загрузке конфигурации из драйвера");
				return;
			}
            var coreConfig = result.Result;
			if (coreConfig == null)
			{
                Logger.Error("SynchronyzeConfiguration FiresecSerializedClient.GetCoreConfig coreConfig=null");
                DriversError.AppendLine("Ошибка при загрузке конфигурации из драйвера");
				return;
			}

			var firesecDeviceConfiguration = ConvertOnlyDevices(coreConfig);
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
					DriversError.AppendLine("Для устройства " + device.PresentationAddressAndDriver + " не найдено устройство в конфигурации firesec-1");
				}
			}
            foreach (var firesecDevice in firesecDeviceConfiguration.Devices)
            {
                var device = ConfigurationCash.DeviceConfiguration.Devices.FirstOrDefault(x => x.PathId == firesecDevice.PathId);
                if (device == null)
                {
                    DriversError.AppendLine("Для устройства " + firesecDevice.PresentationAddressAndDriver + " не найдено устройство в конфигурации firesec-2");
                }
            }
		}
	}
}