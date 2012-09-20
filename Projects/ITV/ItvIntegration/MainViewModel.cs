using System;
using System.Configuration;
using System.Windows;
using FiresecClient.Itv;

namespace ItvIntegration
{
    public class MainViewModel : BaseViewModel
    {
        public DevicesViewModel DevicesViewModel { get; private set; }
        public ZonesViewModel ZonesViewModel { get; private set; }

        public MainViewModel()
        {
            ShowImitatorCommand = new RelayCommand(OnShowImitator);

            var FS_Address = ConfigurationManager.AppSettings["FS_Address"] as string;
            var FS_Port = Convert.ToInt32(ConfigurationManager.AppSettings["FS_Port"] as string);
            var FS_Login = ConfigurationManager.AppSettings["FS_Login"] as string;
            var FS_Password = ConfigurationManager.AppSettings["FS_Password"] as string;
            var serverAddress = ConfigurationManager.AppSettings["ServiceAddress"] as string;
            var Login = ConfigurationManager.AppSettings["Login"] as string;
            var Password = ConfigurationManager.AppSettings["Password"] as string;

            var message = ItvManager.Connect(serverAddress, Login, Password, FS_Address, FS_Port, FS_Login, FS_Password);
            if (message != null)
            {
                MessageBox.Show(message);
                return;
            }

            DevicesViewModel = new DevicesViewModel();
            ZonesViewModel = new ZonesViewModel();
        }

        public RelayCommand ShowImitatorCommand { get; private set; }
        void OnShowImitator()
        {
            ItvManager.ShowImitator();
        }
    }
}