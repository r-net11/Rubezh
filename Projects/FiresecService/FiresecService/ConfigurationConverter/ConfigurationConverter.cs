using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecService.Processor;
using FiresecService.Service;

namespace FiresecService.Configuration
{
	public partial class ConfigurationConverter
	{
		DeviceConfiguration DeviceConfiguration { get; set; }
		public FiresecSerializedClient FiresecSerializedClient;
		public Firesec.CoreConfiguration.config FiresecConfiguration { get; set; }
		int Gid { get; set; }
		public string DriversError { get; private set; }

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
				device.Driver = ConfigurationCash.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
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
			ConfigurationCash.Drivers = new List<Driver>();
			foreach (var innerDriver in DriverConverter.Metadata.drv)
			{
				var driver = DriverConverter.Convert(innerDriver);
				if (driver == null)
				{
					DriversError = "Не удается найти данные для драйвера " + innerDriver.name;
				}
				else
				{
					if (driver.IsIgnore == false)
						ConfigurationCash.Drivers.Add(driver);
				}
			}
			DriverConfigurationParametersHelper.CreateKnownProperties(ConfigurationCash.Drivers);
		}

		public void Update()
		{
			var hasInvalidDriver = false;
			ConfigurationCash.DeviceConfiguration.Update();
			foreach (var device in ConfigurationCash.DeviceConfiguration.Devices)
			{
				device.Driver = ConfigurationCash.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
				if (device.Driver == null)
				{
					hasInvalidDriver = true;
					device.Parent.Children.Remove(device);
				}
			}
			if (hasInvalidDriver)
				ConfigurationCash.DeviceConfiguration.Update();
		}
	}
}