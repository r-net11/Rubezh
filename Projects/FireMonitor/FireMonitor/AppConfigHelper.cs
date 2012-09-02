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
			appSettings.LibVlcDllsPath = ConfigurationManager.AppSettings["LibVlcDllsPath"] as string;
			appSettings.ShowOnlyVideo = Convert.ToBoolean(ConfigurationManager.AppSettings["ShowOnlyVideo"] as string);
#if DEBUG
			appSettings.IsDebug = true;
#endif
            appSettings.CanControl = Convert.ToBoolean(ConfigurationManager.AppSettings["CanControl"] as string);
			var licenseHelper = new LicenseHelper(10);
			appSettings.HasLicenseToControl = licenseHelper.CheckLicense();
			ServiceFactory.AppSettings = appSettings;
		}
	}
}