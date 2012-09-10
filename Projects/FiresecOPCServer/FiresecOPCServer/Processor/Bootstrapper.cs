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
            var serverAddress = ConfigurationManager.AppSettings["ServiceAddress"];
            var message = FiresecManager.Connect(ClientType.Other, serverAddress, "adm", "");
            if (message == null)
            {
                FiresecManager.GetConfiguration(false);
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