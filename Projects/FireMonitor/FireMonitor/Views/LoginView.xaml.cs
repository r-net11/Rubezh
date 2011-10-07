using System.Windows;
using System.Windows.Input;
using Common;
using FiresecClient;

namespace FireMonitor
{
    public partial class LoginView : Window
    {
        public enum PasswordViewType
        {
            Connect,
            Reconnect,
            Validate
        }

        public LoginView()
        {
            InitializeComponent();
        }

        PasswordViewType _passwordViewType;
        public bool IsLoggedIn { get; private set; }

        public void Initialize(PasswordViewType passwordViewType)
        {
            _passwordViewType = passwordViewType;
            _password.Password = "";
            switch (_passwordViewType)
            {
                case PasswordViewType.Connect:
                    _userName.Text = "adm";
                    _userName.IsEnabled = true;
                    break;

                case PasswordViewType.Reconnect:
                    _userName.Text = "";
                    _userName.IsEnabled = true;
                    break;

                case PasswordViewType.Validate:
                    _userName.Text = FiresecManager.CurrentUser.Name;
                    _userName.IsEnabled = false;
                    break;
            }
        }

        void OnConnect(object sender, RoutedEventArgs e)
        {
            string message = string.Empty;
            switch (_passwordViewType)
            {
                case PasswordViewType.Connect:
                    message = FiresecManager.Connect(_userName.Text, _password.Password);
                    break;

                case PasswordViewType.Reconnect:
                    message = FiresecManager.Reconnect(_userName.Text, _password.Password);
                    break;

                case PasswordViewType.Validate:
                    message = "Валидация не пройдена";
                    if (HashHelper.CheckPass(_password.Password, FiresecManager.CurrentUser.PasswordHash))
                    {
                        message = null;
                    }
                    break;
            }

            if (message == null)
            {
                IsLoggedIn = true;
                Close();
            }
            _info.Text = message;
        }

        void OnCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void LoginViewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && e.IsRepeat == false && e.IsToggled == false)
                OnConnect(sender, new RoutedEventArgs());
        }
    }
}