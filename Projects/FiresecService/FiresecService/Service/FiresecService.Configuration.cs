using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecService.Configuration;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public List<Driver> GetDrivers()
		{
			if (ConfigurationCash.DriversConfiguration.Drivers.Count == 0)
			{
				return ConfigurationFileManager.GetDriversConfiguration().Drivers;
			}
			return ConfigurationCash.DriversConfiguration.Drivers;
		}

		public DeviceConfigurationStates GetStates(bool forceConvert = false)
		{
			if (forceConvert)
			{
				FiresecManager.ConvertStates();
			}
			return ConfigurationCash.DeviceConfigurationStates;
		}

		public DeviceConfiguration GetDeviceConfiguration()
		{
			return ConfigurationFileManager.GetDeviceConfiguration();
		}

		public SecurityConfiguration GetSecurityConfiguration()
		{
			return ConfigurationCash.SecurityConfiguration;
		}

		public void SetSecurityConfiguration(SecurityConfiguration securityConfiguration)
		{
			ConfigurationFileManager.SetSecurityConfiguration(securityConfiguration);
			ConfigurationCash.SecurityConfiguration = securityConfiguration;
		}

		public SystemConfiguration GetSystemConfiguration()
		{
			return ConfigurationCash.SystemConfiguration;
		}

		public void SetSystemConfiguration(SystemConfiguration systemConfiguration)
		{
			ConfigurationFileManager.SetSystemConfiguration(systemConfiguration);
			ConfigurationCash.SystemConfiguration = systemConfiguration;
		}

		public LibraryConfiguration GetLibraryConfiguration()
		{
			return ConfigurationCash.LibraryConfiguration;
		}

		public void SetLibraryConfiguration(LibraryConfiguration libraryConfiguration)
		{
			ConfigurationFileManager.SetLibraryConfiguration(libraryConfiguration);
			ConfigurationCash.LibraryConfiguration = libraryConfiguration;
		}

		public PlansConfiguration GetPlansConfiguration()
		{
			return ConfigurationFileManager.GetPlansConfiguration();
		}

		public void SetPlansConfiguration(PlansConfiguration plansConfiguration)
		{
			ConfigurationFileManager.SetPlansConfiguration(plansConfiguration);
			ConfigurationCash.PlansConfiguration = plansConfiguration;
		}
	}
}