using System.Windows;
using FiresecClient;
using System.Configuration;
using FiresecAPI.Models;
using System;

namespace DeviveModelManager
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var FS_Address = ConfigurationManager.AppSettings["FS_Address"] as string;
            var FS_Port = Convert.ToInt32(ConfigurationManager.AppSettings["FS_Port"] as string);
            var FS_Login = ConfigurationManager.AppSettings["FS_Login"] as string;
            var FS_Password = ConfigurationManager.AppSettings["FS_Password"] as string;
            var serverAddress = ConfigurationManager.AppSettings["ServiceAddress"] as string;
            var Login = ConfigurationManager.AppSettings["Login"] as string;
            var Password = ConfigurationManager.AppSettings["Password"] as string;

            FiresecManager.Connect(ClientType.Assad, serverAddress, Login, Password);
            FiresecManager.GetConfiguration();
            FiresecManager.InitializeFiresecDriver(FS_Address, FS_Port, FS_Login, FS_Password);
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
