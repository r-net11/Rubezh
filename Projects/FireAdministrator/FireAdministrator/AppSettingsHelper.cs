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
				ServiceAddress = ConfigurationManager.AppSettings["ServiceAddress"] as string,
				LibVlcDllsPath = ConfigurationManager.AppSettings["LibVlcDllsPath"] as string,
			};
#if DEBUG
			appSettings.IsDebug = Convert.ToBoolean(ConfigurationManager.AppSettings["IsDebug"] as string);
#endif
			ServiceFactory.AppSettings = appSettings;
		}
	}
}