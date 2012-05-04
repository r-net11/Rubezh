using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecService.Configuration;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public List<Driver> GetDrivers()
		{
			//lock (this)
			{
				return FiresecManager.ConfigurationManager.Drivers;
			}
		}

		public DeviceConfigurationStates GetStates()
		{
			//lock (this)
			{
				return FiresecManager.DeviceConfigurationStates;
			}
		}

		public DeviceConfiguration GetDeviceConfiguration()
		{
			//lock (this)
			{
				return FiresecManager.ConfigurationManager.DeviceConfiguration;
			}
		}

		public SecurityConfiguration GetSecurityConfiguration()
		{
			//lock (this)
			{
				return FiresecManager.ConfigurationManager.SecurityConfiguration;
			}
		}

		public void SetSecurityConfiguration(SecurityConfiguration securityConfiguration)
		{
			ConfigurationFileManager.SetSecurityConfiguration(securityConfiguration);
			FiresecManager.ConfigurationManager.SecurityConfiguration = securityConfiguration;
		}

		public SystemConfiguration GetSystemConfiguration()
		{
			//lock (this)
			{
				return FiresecManager.ConfigurationManager.SystemConfiguration;
			}
		}

		public void SetSystemConfiguration(SystemConfiguration systemConfiguration)
		{
			ConfigurationFileManager.SetSystemConfiguration(systemConfiguration);
			FiresecManager.ConfigurationManager.SystemConfiguration = systemConfiguration;
		}

		public LibraryConfiguration GetLibraryConfiguration()
		{
			//lock (this)
			{
				return FiresecManager.ConfigurationManager.LibraryConfiguration;
			}
		}

		public void SetLibraryConfiguration(LibraryConfiguration libraryConfiguration)
		{
			ConfigurationFileManager.SetLibraryConfiguration(libraryConfiguration);
			FiresecManager.ConfigurationManager.LibraryConfiguration = libraryConfiguration;
		}

		public PlansConfiguration GetPlansConfiguration()
		{
			//lock (this)
			{
				return FiresecManager.ConfigurationManager.PlansConfiguration;
			}
		}

		public void SetPlansConfiguration(PlansConfiguration plansConfiguration)
		{
			ConfigurationFileManager.SetPlansConfiguration(plansConfiguration);
			FiresecManager.ConfigurationManager.PlansConfiguration = plansConfiguration;
		}
	}
}