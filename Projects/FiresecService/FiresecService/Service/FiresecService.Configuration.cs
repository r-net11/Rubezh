using System.Collections.Generic;
using FiresecAPI.Models;

namespace FiresecService
{
    public partial class FiresecService
    {
        public List<Driver> GetDrivers()
        {
            lock (Locker)
            {
				return FiresecManager.ConfigurationManager.Drivers;
            }
        }

        public DeviceConfigurationStates GetStates()
        {
            lock (Locker)
            {
				return FiresecManager.DeviceConfigurationStates;
            }
        }

        public DeviceConfiguration GetDeviceConfiguration()
        {
            lock (Locker)
            {
				return FiresecManager.ConfigurationManager.DeviceConfiguration;
            }
        }

        public SecurityConfiguration GetSecurityConfiguration()
        {
            lock (Locker)
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
            lock (Locker)
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
            lock (Locker)
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
            lock (Locker)
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