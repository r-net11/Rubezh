using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.Models
{
	public static class ConfigurationCash
	{
		public static DriversConfiguration DriversConfiguration { get; set; }
		public static DeviceConfiguration DeviceConfiguration { get; set; }
		public static PlansConfiguration PlansConfiguration { get; set; }
		public static DeviceConfigurationStates DeviceConfigurationStates { get; set; }
		static ConfigurationCash()
		{
			DriversConfiguration = new DriversConfiguration();
			DeviceConfigurationStates = new DeviceConfigurationStates();
		}
	}
}