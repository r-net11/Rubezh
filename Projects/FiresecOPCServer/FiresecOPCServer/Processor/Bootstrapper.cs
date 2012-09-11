using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient;
using FiresecAPI.Models;
using System.Configuration;

namespace FiresecOPCServer
{
    public static class Bootstrapper
    {
        public static void Run()
        {
            var FS_Address = ConfigurationManager.AppSettings["FS_Address"] as string;
            var FS_Port = Convert.ToInt32(ConfigurationManager.AppSettings["FS_Port"] as string);
            var FS_Login = ConfigurationManager.AppSettings["FS_Login"] as string;
            var FS_Password = ConfigurationManager.AppSettings["FS_Password"] as string;
            var serverAddress = ConfigurationManager.AppSettings["ServiceAddress"] as string;
            var Login = ConfigurationManager.AppSettings["Login"] as string;
            var Password = ConfigurationManager.AppSettings["Password"] as string;

            var message = FiresecManager.Connect(ClientType.OPC, serverAddress, Login, Password);
            if (message == null)
            {
                FiresecManager.GetConfiguration(false, FS_Address, FS_Port, FS_Login, FS_Password);
                FiresecManager.GetStates();
                FiresecOPCManager.Start();
                FiresecCallbackService.ConfigurationChangedEvent += new Action(FiresecCallbackService_ConfigurationChangedEvent);
            }
        }

        static void FiresecCallbackService_ConfigurationChangedEvent()
        {
            FiresecOPCManager.OPCRefresh();
        }
    }
}