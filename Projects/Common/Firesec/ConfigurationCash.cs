using FiresecAPI.Models;

namespace Firesec
{
	public static class ConfigurationCash
	{
		public static DriversConfiguration DriversConfiguration { get; set; }
		public static DeviceConfiguration DeviceConfiguration { get; set; }
		public static LibraryConfiguration LibraryConfiguration { get; set; }
		public static SystemConfiguration SystemConfiguration { get; set; }
		public static PlansConfiguration PlansConfiguration { get; set; }
		public static SecurityConfiguration SecurityConfiguration { get; set; }
		public static DeviceConfigurationStates DeviceConfigurationStates { get; set; }
	}
}