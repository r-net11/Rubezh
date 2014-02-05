using System;
using Common;
using Infrastructure;

namespace FireMonitor
{
	public static class AppConfigHelper
	{
		public static void InitializeAppSettings()
		{
			var appSettings = new AppSettings();
			try
			{
				//appSettings.HasLicenseToControl = LicenseHelper.CheckLicense(false);
			}
			catch (Exception e)
			{
				Logger.Error(e, "AppConfigHelper.InitializeAppSettings");
			}
			ServiceFactory.AppSettings = appSettings;
		}
	}
}