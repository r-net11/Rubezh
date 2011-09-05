using FiresecService.Converters;

namespace FiresecService
{
    public static class ConfigurationConverter
    {
        public static void Convert()
        {
            FiresecManager.ConveretCoreConfigurationFromFiresec();
            ConfigurationFileManager.SetDeviceConfiguration(FiresecManager.DeviceConfiguration);
            ConfigurationFileManager.SetSecurityConfiguration(FiresecManager.SecurityConfiguration);

            var plans = FiresecInternalClient.GetPlans();
            var plansConfiguration = PlansConverter.Convert(plans);

            ConfigurationFileManager.SetPlansConfiguration(plansConfiguration);
        }
    }
}
