using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace FiresecService.Configuration
{
	public partial class ConfigurationManager
	{
		public List<Driver> Drivers { get; set; }
		public DeviceConfiguration DeviceConfiguration { get; set; }
		public LibraryConfiguration LibraryConfiguration { get; set; }
		public SystemConfiguration SystemConfiguration { get; set; }
		public PlansConfiguration PlansConfiguration { get; set; }
		public SecurityConfiguration SecurityConfiguration { get; set; }

		public ConfigurationManager()
		{
			Drivers = new List<Driver>();
			DeviceConfiguration = new DeviceConfiguration();
			LibraryConfiguration = new LibraryConfiguration();
			SystemConfiguration = new SystemConfiguration();
			PlansConfiguration = new PlansConfiguration();
			SecurityConfiguration = new SecurityConfiguration();
		}

		public void Update()
		{
			var hasInvalidDriver = false;
			DeviceConfiguration.Update();
			foreach (var device in DeviceConfiguration.Devices)
			{
				device.Driver = Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
				if (device.Driver == null)
				{
					hasInvalidDriver = true;
					device.Parent.Children.Remove(device);
				}
			}
			if (hasInvalidDriver)
				DeviceConfiguration.Update();
		}
	}
}