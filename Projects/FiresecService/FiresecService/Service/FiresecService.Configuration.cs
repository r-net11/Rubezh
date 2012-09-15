using FiresecAPI.Models;
using FiresecService.Configuration;

namespace FiresecService.Service
{
    public partial class FiresecService
    {
		public DriversConfiguration GetDriversConfiguration()
        {
			return ConfigurationFileManager.GetDriversConfiguration();
        }
		//public void SetDriversConfiguration(DriversConfiguration driversConfiguration)
		//{
		//    ConfigurationFileManager.SetDriversConfiguration(driversConfiguration);
		//}

        public DeviceConfiguration GetDeviceConfiguration()
        {
			return ConfigurationFileManager.GetDeviceConfiguration();
        }

        public SecurityConfiguration GetSecurityConfiguration()
        {
			return ConfigurationFileManager.GetSecurityConfiguration();
        }
        public void SetSecurityConfiguration(SecurityConfiguration securityConfiguration)
        {
            ConfigurationFileManager.SetSecurityConfiguration(securityConfiguration);
            ConfigurationCash.SecurityConfiguration = securityConfiguration;
        }

        public SystemConfiguration GetSystemConfiguration()
        {
			return ConfigurationFileManager.GetSystemConfiguration();
        }
        public void SetSystemConfiguration(SystemConfiguration systemConfiguration)
        {
            ConfigurationFileManager.SetSystemConfiguration(systemConfiguration);
        }

        public LibraryConfiguration GetLibraryConfiguration()
        {
			return ConfigurationFileManager.GetLibraryConfiguration();
        }
        public void SetLibraryConfiguration(LibraryConfiguration libraryConfiguration)
        {
            ConfigurationFileManager.SetLibraryConfiguration(libraryConfiguration);
        }

        public PlansConfiguration GetPlansConfiguration()
        {
			return ConfigurationFileManager.GetPlansConfiguration();
        }
        public void SetPlansConfiguration(PlansConfiguration plansConfiguration)
        {
            ConfigurationFileManager.SetPlansConfiguration(plansConfiguration);
        }
    }
}