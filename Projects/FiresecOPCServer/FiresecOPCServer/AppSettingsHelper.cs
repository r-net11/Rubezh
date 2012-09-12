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
            AppSettings.ServerAddress = ConfigurationManager.AppSettings["ServiceAddress"] as string;
            AppSettings.Login = ConfigurationManager.AppSettings["Login"] as string;
            AppSettings.Password = ConfigurationManager.AppSettings["Password"] as string;
        }
    }
}