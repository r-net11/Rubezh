using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecService.Processor;
using FiresecService.Service;
using Common;
using FiresecService.ViewModels;

namespace FiresecService.Configuration
{
	public partial class ConfigurationConverter
	{
		DeviceConfiguration DeviceConfiguration { get; set; }
		public FiresecSerializedClient FiresecSerializedClient;
		public Firesec.CoreConfiguration.config FiresecConfiguration { get; set; }
		int Gid { get; set; }
		public StringBuilder DriversError { get; private set; }

		public ConfigurationConverter()
		{
			DriversError = new StringBuilder();
		}

		public void Convert()
		{
			FiresecConfiguration = FiresecSerializedClient.GetCoreConfig().Result;
			DeviceConfiguration = new DeviceConfiguration();
			ConvertZones();
			ConvertDirections();
			ConvertGuardUsers();
			ConvertDevices();
			Update();

			ConfigurationCash.DeviceConfiguration = DeviceConfiguration;
			ConfigurationFileManager.SetDeviceConfiguration(DeviceConfiguration);

			var plans = FiresecSerializedClient.GetPlans().Result;
			ConfigurationCash.PlansConfiguration = ConvertPlans(plans);
			ConfigurationFileManager.SetPlansConfiguration(ConfigurationCash.PlansConfiguration);
		}

		public void ConvertBack(DeviceConfiguration deviceConfiguration, bool includeSecurity)
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
				FiresecConfiguration.part = null;
			}
			else
			{
				FiresecConfiguration = new Firesec.CoreConfiguration.config();
			}

			Gid = 0;
			ConvertZonesBack();
			ConvertDevicesBack();
			ConvertDirectionsBack();
			ConvertGuardUsersBack();
		}

		public DeviceConfiguration ConvertOnlyDevices(Firesec.CoreConfiguration.config firesecConfiguration)
		{
			DeviceConfiguration = new DeviceConfiguration();
			FiresecConfiguration = firesecConfiguration;
			ConvertDevices();
			return DeviceConfiguration;
		}

		public void ConvertMetadataFromFiresec()
		{
			DriverConverter.Metadata = FiresecSerializedClient.GetMetaData().Result;
			ConfigurationCash.DriversConfiguration.Drivers = new List<Driver>();
			foreach (var innerDriver in DriverConverter.Metadata.drv)
			{
				var driver = DriverConverter.Convert(innerDriver);
				if (driver == null)
				{
					DriversError.AppendLine("Не удается найти данные для драйвера " + innerDriver.name);
				}
				else
				{
					if (driver.IsIgnore == false)
						ConfigurationCash.DriversConfiguration.Drivers.Add(driver);
				}
			}
			DriverConfigurationParametersHelper.CreateKnownProperties(ConfigurationCash.DriversConfiguration.Drivers);

			if (ConfigurationCash.DriversConfiguration.Drivers.Count > 0)
			{
				ConfigurationFileManager.SetDriversConfiguration(ConfigurationCash.DriversConfiguration);
			}
		}

		public void Update(DeviceConfiguration deviceConfiguration = null)
		{
			if (deviceConfiguration == null)
				deviceConfiguration = ConfigurationCash.DeviceConfiguration;

			var hasInvalidDriver = false;
			deviceConfiguration.Update();
			foreach (var device in deviceConfiguration.Devices)
			{
				device.Driver = ConfigurationCash.DriversConfiguration.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
				if (device.Driver == null)
				{
					hasInvalidDriver = true;
					device.Parent.Children.Remove(device);
				}
			}
			if (hasInvalidDriver)
				deviceConfiguration.Update();

			deviceConfiguration.UpdateIdPath();
		}

		public void SynchronyzeConfiguration()
		{
			var coreConfig = FiresecSerializedClient.GetCoreConfig().Result;
			if (coreConfig == null)
			{
				Logger.Error("SynchronyzeConfiguration coreConfig=null");
				UILogger.Log("Ошибка при синхронизации конфигурации");
				return;
			}

			var firesecDeviceConfiguration = ConvertOnlyDevices(coreConfig);
			Update(firesecDeviceConfiguration);
			firesecDeviceConfiguration.Update();
			foreach (var device in ConfigurationCash.DeviceConfiguration.Devices)
			{
				var firesecDevice = firesecDeviceConfiguration.Devices.FirstOrDefault(x => x.PathId == device.PathId);
				if (firesecDevice != null)
				{
					device.PlaceInTree = firesecDevice.GetPlaceInTree();
				}
				else
				{
					DriversError.AppendLine("Для устройства " + device.PresentationAddressAndDriver + " не найдено устройство в конфигурации firesec-1");
				}
			}
		}
	}
}