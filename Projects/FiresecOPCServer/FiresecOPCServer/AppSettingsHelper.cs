using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Common;

namespace FiresecOPCServer
{
    public static class AppSettingsHelper
    {
        public static void InitializeAppSettings()
        {
            try
            {
                AppSettings.IsImitatorEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["IsImitatorEnabled"] as string);
            }
            catch (Exception e)
            {
                Logger.Error(e, "AppSettingsHelper.InitializeAppSettings");
            }
        }
    }
}