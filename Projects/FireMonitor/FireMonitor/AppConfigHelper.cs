using System;
using System.Configuration;
using Infrastructure;

namespace FireMonitor
{
    public static class AppConfigHelper
    {
        public static void InitializeAppSettings()
        {
            var appSettings = new AppSettings();
            appSettings.ServiceAddress = ConfigurationManager.AppSettings["ServiceAddress"] as string;
            appSettings.DefaultLogin = ConfigurationManager.AppSettings["DefaultLogin"] as string;
            appSettings.DefaultPassword = ConfigurationManager.AppSettings["DefaultPassword"] as string;
            appSettings.AutoConnect = Convert.ToBoolean(ConfigurationManager.AppSettings["AutoConnect"] as string);
            appSettings.LibVlcDllsPath = ConfigurationManager.AppSettings["LibVlcDllsPath"] as string;
            appSettings.ShowOnlyVideo = Convert.ToBoolean(ConfigurationManager.AppSettings["ShowOnlyVideo"] as string);
            appSettings.IsDebug = Convert.ToBoolean(ConfigurationManager.AppSettings["IsDebug"] as string);
            ServiceFactory.AppSettings = appSettings;
        }
    }
}