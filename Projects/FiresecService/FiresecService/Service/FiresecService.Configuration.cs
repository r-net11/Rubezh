using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecService.Configuration;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public List<Driver> GetDrivers()
		{
			return ConfigurationCash.Drivers;
		}

		public DeviceConfigurationStates GetStates(bool forceConvert = false)
		{
			if (forceConvert)
			{
				FiresecManager.ConvertStates();
				FiresecManager.Watcher.OnStateChanged();
			}
			return FiresecManager.DeviceConfigurationStates;
		}

		public DeviceConfiguration GetDeviceConfiguration()
		{
			return ConfigurationFileManager.GetDeviceConfiguration();
			return ConfigurationCash.DeviceConfiguration;
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
			return ConfigurationCash.PlansConfiguration;
		}

		public void SetPlansConfiguration(PlansConfiguration plansConfiguration)
		{
			ConfigurationFileManager.SetPlansConfiguration(plansConfiguration);
			ConfigurationCash.PlansConfiguration = plansConfiguration;
		}
	}
}