using FiresecAPI.Models;
using FiresecService.Configuration;

namespace FiresecService
{
    public static class ConfigurationCash
    {
        public static SecurityConfiguration SecurityConfiguration { get; set; }

        static ConfigurationCash()
        {
            SecurityConfiguration = ZipFileManager.GetSecurityConfiguration();
        }
    }
}