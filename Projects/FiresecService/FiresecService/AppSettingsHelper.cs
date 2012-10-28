using System;
using System.Configuration;

namespace FiresecService
{
    public static class AppSettingsHelper
    {
        public static void InitializeAppSettings()
        {
            AppSettings.EnableRemoteConnections = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableRemoteConnections"] as string);
            AppSettings.Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"] as string);
        }
    }
}