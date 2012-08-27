using System;

namespace FiresecService
{
    public static class AppSettingsHelper
    {
        public static void InitializeAppSettings()
        {
            AppSettings.OldFiresecLogin = System.Configuration.ConfigurationManager.AppSettings["OldFiresecLogin"] as string;
            AppSettings.OldFiresecPassword = System.Configuration.ConfigurationManager.AppSettings["OldFiresecPassword"] as string;
            AppSettings.ServiceAddress = System.Configuration.ConfigurationManager.AppSettings["ServiceAddress"] as string;
            AppSettings.LocalServiceAddress = System.Configuration.ConfigurationManager.AppSettings["LocalServiceAddress"] as string;
            AppSettings.DoNotOverrideFiresec1Config = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["DoNotOverrideFiresec1Config"] as string);
            AppSettings.IsImitatorEnabled = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsImitatorEnabled"] as string);
            AppSettings.IsOPCEnabled = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsOPCEnabled"] as string);
			AppSettings.IsFSEnabled = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsFSEnabled"] as string);
			AppSettings.IsGKEnabled = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsGKEnabled"] as string);
#if DEBUG
            AppSettings.IsDebug = true;
#endif
        }
    }
}