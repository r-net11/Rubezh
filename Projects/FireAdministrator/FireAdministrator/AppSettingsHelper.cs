using System;
using System.Configuration;
using Infrastructure;

namespace FireAdministrator
{
    public static class AppSettingsHelper
    {
        public static void InitializeAppSettings()
        {
            var appSettings = new AppSettings()
            {
                FS_Address = ConfigurationManager.AppSettings["FS_Address"] as string,
                FS_Port = Convert.ToInt32(ConfigurationManager.AppSettings["FS_Port"] as string),
                FS_Login = ConfigurationManager.AppSettings["FS_Login"] as string,
                FS_Password = ConfigurationManager.AppSettings["FS_Password"] as string,

                ServiceAddress = ConfigurationManager.AppSettings["ServiceAddress"] as string,
                LibVlcDllsPath = ConfigurationManager.AppSettings["LibVlcDllsPath"] as string,
                Theme = ConfigurationManager.AppSettings["Theme"] as string,
				DoNotOverrideFS1 = Convert.ToBoolean(ConfigurationManager.AppSettings["DoNotOverrideFS1"] as string),
				IsExpertMode = Convert.ToBoolean(ConfigurationManager.AppSettings["IsExpertMode"] as string),
            };
#if DEBUG
            appSettings.IsDebug = true;
#endif
            ServiceFactory.AppSettings = appSettings;
        }
    }
}