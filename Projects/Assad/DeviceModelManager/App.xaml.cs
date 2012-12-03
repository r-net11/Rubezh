using System.Windows;
using FiresecClient;
using System.Configuration;
using FiresecAPI.Models;
using System;
using Infrastructure.Common;

namespace DeviveModelManager
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var serverAddress = ConfigurationManager.AppSettings["ServiceAddress"] as string;
            var login = ConfigurationManager.AppSettings["Login"] as string;
            var password = ConfigurationManager.AppSettings["Password"] as string;

			AppSettingsManager.RemoteAddress = serverAddress;
            FiresecManager.Connect(ClientType.Assad, serverAddress, login, password);
            FiresecManager.GetConfiguration();
            FiresecManager.InitializeFiresecDriver(false);
            FiresecManager.FiresecDriver.Synchronyze();
            FiresecManager.FiresecDriver.StartWatcher(false, false);

            var view = new View();
            var viewModel = new ViewModel();
            view.DataContext = viewModel;
            view.Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            FiresecManager.Disconnect();
        }
    }
}
