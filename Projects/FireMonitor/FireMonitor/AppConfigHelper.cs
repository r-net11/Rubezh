using System;
using System.Configuration;
using Infrastructure;
using Common;

namespace FireMonitor
{
    public static class AppConfigHelper
    {
        public static void InitializeAppSettings()
        {
            var appSettings = new AppSettings();
            try
            {
                appSettings.LibVlcDllsPath = ConfigurationManager.AppSettings["LibVlcDllsPath"] as string;
                appSettings.CanControl = Convert.ToBoolean(ConfigurationManager.AppSettings["CanControl"] as string);
                var licenseHelper = new LicenseHelper(10);
                appSettings.HasLicenseToControl = licenseHelper.CheckLicense();
            }
            catch (Exception e)
            {
                Logger.Error(e, "AppConfigHelper.InitializeAppSettings");
            }
            ServiceFactory.AppSettings = appSettings;
        }
    }
}