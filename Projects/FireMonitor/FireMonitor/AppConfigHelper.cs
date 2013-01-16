using System;
using System.Configuration;
using Common;
using Infrastructure;
using Infrastructure.Common;

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
				appSettings.HasLicenseToControl = LicenseHelper.CheckLicense(1) || LicenseHelper.CheckLicense(3);
            }
            catch (Exception e)
            {
                Logger.Error(e, "AppConfigHelper.InitializeAppSettings");
            }
            ServiceFactory.AppSettings = appSettings;
        }
    }
}