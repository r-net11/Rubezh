using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace FiresecOPCServer
{
    public static class AppSettingsHelper
    {
        public static void InitializeAppSettings()
        {
            AppSettings.FS_Address = ConfigurationManager.AppSettings["FS_Address"] as string;
            AppSettings.FS_Port = Convert.ToInt32(ConfigurationManager.AppSettings["FS_Port"] as string);
            AppSettings.FS_Login = ConfigurationManager.AppSettings["FS_Login"] as string;
            AppSettings.FS_Password = ConfigurationManager.AppSettings["FS_Password"] as string;
            
            AppSettings.ServiceAddress = ConfigurationManager.AppSettings["ServiceAddress"] as string;
            AppSettings.RemoteAddress = ConfigurationManager.AppSettings["RemoteAddress"] as string;
            AppSettings.RemotePort = Convert.ToInt32(ConfigurationManager.AppSettings["RemotePort"] as string);

            AppSettings.Login = ConfigurationManager.AppSettings["Login"] as string;
            AppSettings.Password = ConfigurationManager.AppSettings["Password"] as string;
            AppSettings.IsImitatorEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["IsImitatorEnabled"] as string);

            if (string.IsNullOrEmpty(AppSettings.FS_Address))
                AppSettings.FS_Address = AppSettings.RemoteAddress;
            if (AppSettings.FS_Address == "localhost")
                AppSettings.FS_Address = "127.0.0.1";
        }
    }
}