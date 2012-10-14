using FiresecAPI.Models;
using FiresecService.Configuration;
using FiresecAPI;

namespace FiresecService.Service
{
    public partial class FiresecService
    {
        public OperationResult<bool> SetDeviceConfiguration(DeviceConfiguration deviceConfiguration)
        {
            ConfigurationFileManager.SetDeviceConfiguration(deviceConfiguration);
            OperationResult<bool> result = new OperationResult<bool>();
            return result;
        }

		public DriversConfiguration GetDriversConfiguration()
        {
			return ConfigurationFileManager.GetDriversConfiguration();
        }

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

        public DeviceLibraryConfiguration GetDeviceLibraryConfiguration()
        {
			return ConfigurationFileManager.GetLibraryConfiguration();
        }
        public void SetDeviceLibraryConfiguration(DeviceLibraryConfiguration deviceLibraryConfiguration)
        {
            ConfigurationFileManager.SetLibraryConfiguration(deviceLibraryConfiguration);
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