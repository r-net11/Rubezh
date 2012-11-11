using System;
using System.Configuration;
using Infrastructure;
using Common;

namespace FireAdministrator
{
    public static class AppSettingsHelper
    {
        public static void InitializeAppSettings()
        {
            var appSettings = new AppSettings();
            try
            {
                appSettings.LibVlcDllsPath = ConfigurationManager.AppSettings["LibVlcDllsPath"] as string;
                appSettings.DoNotOverrideFS1 = Convert.ToBoolean(ConfigurationManager.AppSettings["DoNotOverrideFS1"] as string);
                appSettings.IsExpertMode = Convert.ToBoolean(ConfigurationManager.AppSettings["IsExpertMode"] as string);
            }
            catch (Exception e)
            {
                Logger.Error(e, "AppSettingsHelper.InitializeAppSettings");
            }
#if DEBUG
            appSettings.IsDebug = true;
#endif
            ServiceFactory.AppSettings = appSettings;
        }
    }
}