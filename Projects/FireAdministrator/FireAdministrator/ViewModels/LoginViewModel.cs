using System.Configuration;
using Controls.MessageBox;
using FiresecClient;
using Infrastructure.Common;

namespace FireAdministrator.ViewModels
{
    public class LoginViewModel : DialogContent
    {
        public LoginViewModel()
        {
            Title = "Администратор. Авторизация";
            ConnectCommand = new RelayCommand(OnConnect);
            CancelCommand = new RelayCommand(OnCancel);

            UserName = ConfigurationManager.AppSettings["DefaultLogin"] as string;
            Password = ConfigurationManager.AppSettings["DefaultPassword"] as string;
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
            string clientCallbackAddress = ConfigurationManager.AppSettings["ClientCallbackAddress"] as string;
            string serverAddress = ConfigurationManager.AppSettings["ServiceAddress"] as string;
            string message = FiresecManager.Connect(clientCallbackAddress, serverAddress, UserName, Password);
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