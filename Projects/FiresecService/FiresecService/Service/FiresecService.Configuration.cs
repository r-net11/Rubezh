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
                return FiresecManager.Drivers;
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
                return FiresecManager.DeviceConfiguration;
            }
        }

        public SecurityConfiguration GetSecurityConfiguration()
        {
            lock (Locker)
            {
                return FiresecManager.SecurityConfiguration;
            }
        }

        public void SetSecurityConfiguration(SecurityConfiguration securityConfiguration)
        {
            ConfigurationFileManager.SetSecurityConfiguration(securityConfiguration);
            FiresecManager.SecurityConfiguration = securityConfiguration;
        }

        public SystemConfiguration GetSystemConfiguration()
        {
            lock (Locker)
            {
                return FiresecManager.SystemConfiguration;
            }
        }

        public void SetSystemConfiguration(SystemConfiguration systemConfiguration)
        {
            ConfigurationFileManager.SetSystemConfiguration(systemConfiguration);
            FiresecManager.SystemConfiguration = systemConfiguration;
        }

        public LibraryConfiguration GetLibraryConfiguration()
        {
            lock (Locker)
            {
                return FiresecManager.LibraryConfiguration;
            }
        }

        public void SetLibraryConfiguration(LibraryConfiguration libraryConfiguration)
        {
            ConfigurationFileManager.SetLibraryConfiguration(libraryConfiguration);
            FiresecManager.LibraryConfiguration = libraryConfiguration;
        }

        public PlansConfiguration GetPlansConfiguration()
        {
            lock (Locker)
            {
                return FiresecManager.PlansConfiguration;
            }
        }

        public void SetPlansConfiguration(PlansConfiguration plansConfiguration)
        {
            ConfigurationFileManager.SetPlansConfiguration(plansConfiguration);
            FiresecManager.PlansConfiguration = plansConfiguration;
        }
    }
}