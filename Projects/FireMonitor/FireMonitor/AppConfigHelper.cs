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
#if DEBUG
            appSettings.IsDebug = Convert.ToBoolean(ConfigurationManager.AppSettings["IsDebug"] as string);
            appSettings.CanControl = Convert.ToBoolean(ConfigurationManager.AppSettings["CanControl"] as string);
            appSettings.ShowGK = Convert.ToBoolean(ConfigurationManager.AppSettings["ShowGK"] as string);
#endif
            ServiceFactory.AppSettings = appSettings;
        }
    }
}