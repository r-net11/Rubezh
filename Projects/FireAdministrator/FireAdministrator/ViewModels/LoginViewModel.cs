using System.Configuration;
using Controls.MessageBox;
using FiresecClient;
using Infrastructure.Common;
using System;
using Infrastructure;
using Common;

namespace FireAdministrator.ViewModels
{
    public class LoginViewModel : DialogContent
    {
        public LoginViewModel()
        {
            Title = "Администратор. Авторизация";
            ConnectCommand = new RelayCommand(OnConnect);
            CancelCommand = new RelayCommand(OnCancel);

            UserName = ServiceFactory.AppSettings.DefaultLogin;
            Password = ServiceFactory.AppSettings.DefaultPassword;
        }

        public static bool DefaultConnect()
        {
            var userName = ServiceFactory.AppSettings.DefaultLogin;
            var password = ServiceFactory.AppSettings.DefaultPassword;
            if (userName != null && password != null)
            {
                string clientCallbackAddress = ConfigurationHelper.ClientCallbackAddress;
                string serverAddress = ServiceFactory.AppSettings.ServiceAddress;
                string message = FiresecManager.Connect(clientCallbackAddress, serverAddress, userName, password);
                if (message == null)
                {
                    return true;
                }
                MessageBoxService.Show(message);
            }
            return false;
        }

        string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                OnPropertyChanged("UserName");
            }
        }

        string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged("Password");
            }
        }

        public RelayCommand ConnectCommand { get; private set; }
        void OnConnect()
        {
            string message = FiresecManager.Connect(null, ServiceFactory.AppSettings.ServiceAddress, UserName, Password);
            if (message == null)
            {
                Close(true);
                return;
            }
            MessageBoxService.Show(message);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}