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
            appSettings.FS_Address = ConfigurationManager.AppSettings["FS_Address"] as string;
            appSettings.FS_Port = Convert.ToInt32(ConfigurationManager.AppSettings["FS_Port"] as string);
            appSettings.FS_Login = ConfigurationManager.AppSettings["FS_Login"] as string;
            appSettings.FS_Password = ConfigurationManager.AppSettings["FS_Password"] as string;
            appSettings.AutoConnect = Convert.ToBoolean(ConfigurationManager.AppSettings["AutoConnect"] as string);

            appSettings.ServiceAddress = ConfigurationManager.AppSettings["ServiceAddress"] as string;
            appSettings.RemoteAddress = ConfigurationManager.AppSettings["RemoteAddress"] as string;
            appSettings.RemotePort = Convert.ToInt32(ConfigurationManager.AppSettings["RemotePort"] as string);

            appSettings.LibVlcDllsPath = ConfigurationManager.AppSettings["LibVlcDllsPath"] as string;
            appSettings.CanControl = Convert.ToBoolean(ConfigurationManager.AppSettings["CanControl"] as string);
            var licenseHelper = new LicenseHelper(10);
            appSettings.HasLicenseToControl = licenseHelper.CheckLicense();

            if (string.IsNullOrEmpty(appSettings.FS_Address))
                appSettings.FS_Address = appSettings.RemoteAddress;
            if (appSettings.FS_Address == "localhost")
                appSettings.FS_Address = "127.0.0.1";

            ServiceFactory.AppSettings = appSettings;
        }
    }
}