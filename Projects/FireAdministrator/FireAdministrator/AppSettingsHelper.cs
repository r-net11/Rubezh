using System;
using Infrastructure;
using System.Configuration;

namespace FireAdministrator
{
    public static class AppSettingsHelper
    {
        public static void InitializeAppSettings()
        {
            var appSettings = new AppSettings();
            appSettings.ServiceAddress = ConfigurationManager.AppSettings["ServiceAddress"] as string;
            appSettings.DefaultLogin = ConfigurationManager.AppSettings["DefaultLogin"] as string;
            appSettings.DefaultPassword = ConfigurationManager.AppSettings["DefaultPassword"] as string;
            appSettings.AutoConnect = Convert.ToBoolean(ConfigurationManager.AppSettings["AutoConnect"] as string);
            appSettings.ShowVideo = Convert.ToBoolean(ConfigurationManager.AppSettings["ShowVideo"] as string);
            appSettings.LibVlcDllsPath = ConfigurationManager.AppSettings["LibVlcDllsPath"] as string;
#if DEBUG
            appSettings.ShowGC = Convert.ToBoolean(ConfigurationManager.AppSettings["ShowGC"] as string);
            appSettings.ShowSKUD = Convert.ToBoolean(ConfigurationManager.AppSettings["ShowSKUD"] as string);
            appSettings.IsDebug = Convert.ToBoolean(ConfigurationManager.AppSettings["IsDebug"] as string);
#endif
            ServiceFactory.AppSettings = appSettings;
        }
    }
}