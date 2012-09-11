
namespace FiresecService
{
	public static class AppSettingsHelper
	{
		public static void InitializeAppSettings()
		{
			AppSettings.ServiceAddress = System.Configuration.ConfigurationManager.AppSettings["ServiceAddress"] as string;
			AppSettings.LocalServiceAddress = System.Configuration.ConfigurationManager.AppSettings["LocalServiceAddress"] as string;
#if DEBUG
			AppSettings.IsDebug = true;
#endif
		}
	}
}