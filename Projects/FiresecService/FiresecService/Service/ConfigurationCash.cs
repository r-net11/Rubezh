using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace FiresecService.Service
{
	public static class ConfigurationCash
	{
		public static List<Driver> Drivers { get; set; }
		public static DeviceConfiguration DeviceConfiguration { get; set; }
		public static LibraryConfiguration LibraryConfiguration { get; set; }
		public static SystemConfiguration SystemConfiguration { get; set; }
		public static PlansConfiguration PlansConfiguration { get; set; }
		public static SecurityConfiguration SecurityConfiguration { get; set; }

		static ConfigurationCash()
		{
			Drivers = new List<Driver>();
			DeviceConfiguration = new DeviceConfiguration();
			LibraryConfiguration = new LibraryConfiguration();
			SystemConfiguration = new SystemConfiguration();
			PlansConfiguration = new PlansConfiguration();
			SecurityConfiguration = new SecurityConfiguration();
		}
	}
}