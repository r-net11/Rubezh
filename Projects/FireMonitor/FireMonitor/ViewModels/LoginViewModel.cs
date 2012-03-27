using System.Configuration;
using Common;
using Controls.MessageBox;
using FiresecClient;
using Infrastructure.Common;

namespace FireMonitor.ViewModels
{
    public class LoginViewModel : DialogContent
    {
        PasswordViewType _passwordViewType;

        public LoginViewModel(PasswordViewType passwordViewType)
        {
            Title = "Оперативная задача. Авторизация.";
            ConnectCommand = new RelayCommand(OnConnect);
            CancelCommand = new RelayCommand(OnCancel);

            _passwordViewType = passwordViewType;
            Password = ConfigurationManager.AppSettings["DefaultPassword"] as string;
            switch (_passwordViewType)
            {
                case PasswordViewType.Connect:
                    UserName = ConfigurationManager.AppSettings["DefaultLogin"] as string;
                    CanEditUserName = true;
                    break;

                case PasswordViewType.Reconnect:
                    UserName = "";
                    CanEditUserName = true;
                    break;

                case PasswordViewType.Validate:
                    UserName = FiresecManager.CurrentUser.Login;
                    CanEditUserName = false;
                    break;
            }
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

        bool _canEditUserName;
        public bool CanEditUserName
        {
            get { return _canEditUserName; }
            set
            {
                _canEditUserName = value;
                OnPropertyChanged("CanEditUserName");
            }
        }

        public RelayCommand ConnectCommand { get; private set; }
        void OnConnect()
        {
            string message = string.Empty;
            switch (_passwordViewType)
            {
                case PasswordViewType.Connect:
                    message = FiresecManager.Connect(ConnectHelper.ClientCallbackAddress, ConnectHelper.ServiceAddress, UserName, Password);
                    break;

                case PasswordViewType.Reconnect:
                    message = FiresecManager.Reconnect(UserName, Password);
                    break;

                case PasswordViewType.Validate:
                    message = "Валидация не пройдена";
                    if (HashHelper.CheckPass(Password, FiresecManager.CurrentUser.PasswordHash))
                        message = null;
                    break;
            }

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

        public enum PasswordViewType
        {
            Connect,
            Reconnect,
            Validate
        }
    }
}