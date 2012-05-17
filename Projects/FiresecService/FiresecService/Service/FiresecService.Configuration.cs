using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecService.Configuration;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public List<Driver> GetDrivers()
		{
			return FiresecManager.ConfigurationManager.Drivers;
		}

		public DeviceConfigurationStates GetStates(bool forceConvert = false)
		{
			if (forceConvert)
				FiresecManager.ConvertStates();
			return FiresecManager.DeviceConfigurationStates;
		}

		public DeviceConfiguration GetDeviceConfiguration()
		{
			return FiresecManager.ConfigurationManager.DeviceConfiguration;
		}

		public SecurityConfiguration GetSecurityConfiguration()
		{
			return FiresecManager.ConfigurationManager.SecurityConfiguration;
		}

		public void SetSecurityConfiguration(SecurityConfiguration securityConfiguration)
		{
			ConfigurationFileManager.SetSecurityConfiguration(securityConfiguration);
			FiresecManager.ConfigurationManager.SecurityConfiguration = securityConfiguration;
		}

		public SystemConfiguration GetSystemConfiguration()
		{
			return FiresecManager.ConfigurationManager.SystemConfiguration;
		}

		public void SetSystemConfiguration(SystemConfiguration systemConfiguration)
		{
			ConfigurationFileManager.SetSystemConfiguration(systemConfiguration);
			FiresecManager.ConfigurationManager.SystemConfiguration = systemConfiguration;
		}

		public LibraryConfiguration GetLibraryConfiguration()
		{
			return FiresecManager.ConfigurationManager.LibraryConfiguration;
		}

		public void SetLibraryConfiguration(LibraryConfiguration libraryConfiguration)
		{
			ConfigurationFileManager.SetLibraryConfiguration(libraryConfiguration);
			FiresecManager.ConfigurationManager.LibraryConfiguration = libraryConfiguration;
		}

		public PlansConfiguration GetPlansConfiguration()
		{
			return FiresecManager.ConfigurationManager.PlansConfiguration;
		}

		public void SetPlansConfiguration(PlansConfiguration plansConfiguration)
		{
			ConfigurationFileManager.SetPlansConfiguration(plansConfiguration);
			FiresecManager.ConfigurationManager.PlansConfiguration = plansConfiguration;
		}
	}
}