using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecService.Configuration;

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
			DeviceConfiguration = ConfigurationFileManager.GetDeviceConfiguration();
			SecurityConfiguration = ConfigurationFileManager.GetSecurityConfiguration();
			LibraryConfiguration = ConfigurationFileManager.GetLibraryConfiguration();
			SystemConfiguration = ConfigurationFileManager.GetSystemConfiguration();
			PlansConfiguration = ConfigurationFileManager.GetPlansConfiguration();
		}
	}
}