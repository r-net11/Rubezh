using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecService.Converters;
using System;

namespace FiresecService
{
	public partial class ConfigurationManager
	{
		public List<Driver> Drivers { get; set; }
		public DeviceConfiguration DeviceConfiguration { get; set; }
		public LibraryConfiguration LibraryConfiguration { get; set; }
		public SystemConfiguration SystemConfiguration { get; set; }
		public PlansConfiguration PlansConfiguration { get; set; }
		public SecurityConfiguration SecurityConfiguration { get; set; }

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