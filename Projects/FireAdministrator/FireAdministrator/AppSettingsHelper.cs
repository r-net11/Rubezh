using System;
using System.Configuration;
using Infrastructure;
using FireAdministrator.Properties;

namespace FireAdministrator
{
	public static class AppSettingsHelper
	{
		public static void InitializeAppSettings()
		{
			var appSettings = new AppSettings()
			{
				ServiceAddress = ConfigurationManager.AppSettings["ServiceAddress"] as string,
				LibVlcDllsPath = ConfigurationManager.AppSettings["LibVlcDllsPath"] as string,
				UserName = Settings.Default.UserName,
				Password = Settings.Default.Password,
				SavePassword = Settings.Default.SavePassword
			};
#if DEBUG
			appSettings.IsDebug = Convert.ToBoolean(ConfigurationManager.AppSettings["IsDebug"] as string);
#endif


			ServiceFactory.AppSettings = appSettings;
		}
		public static void SaveAppSettings()
		{
			Settings.Default.UserName = ServiceFactory.AppSettings.UserName;
			Settings.Default.Password = ServiceFactory.AppSettings.Password;
			Settings.Default.SavePassword = ServiceFactory.AppSettings.SavePassword;
			Settings.Default.Save();
		}
	}
}