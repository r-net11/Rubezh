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
			appSettings.LibVlcDllsPath = ConfigurationManager.AppSettings["LibVlcDllsPath"] as string;
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