using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecService.Converters;

namespace FiresecService
{
	public partial class ConfigurationManager
	{
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
			SetValidChars();
			Update();

			var plans = FiresecSerializedClient.GetPlans().Result;
			PlansConfiguration = ConvertPlans(plans);

			ConfigurationFileManager.SetDeviceConfiguration(DeviceConfiguration);
			ConfigurationFileManager.SetPlansConfiguration(PlansConfiguration);
		}

		public void ConvertBack(DeviceConfiguration deviceConfiguration, bool includeSecurity)
		{
			deviceConfiguration.Update();

			foreach (var device in deviceConfiguration.Devices)
			{
				device.Driver = Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
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
			FiresecConfiguration = firesecConfiguration;
			ConvertDevices();
			return DeviceConfiguration;
		}

		public void ConvertMetadataFromFiresec()
		{
			DriverConverter.Metadata = FiresecSerializedClient.GetMetaData().Result;
			Drivers = new List<Driver>();
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
						Drivers.Add(driver);
				}
			}
		}

		public void SetValidChars()
		{
			DriverConverter.Metadata = FiresecSerializedClient.GetMetaData().Result;
			var validCharsBuilder = new StringBuilder(DriverConverter.Metadata.drv.Last().validChars);
			validCharsBuilder.Append('№');
			DeviceConfiguration.ValidChars = validCharsBuilder.ToString();
		}
	}
}