using System.Windows;
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
        public bool IsConnected { get; private set; }

        public void Initialize(PasswordViewType passwordViewType)
        {
            _passwordViewType = passwordViewType;

            _passwordTextBox.Password = "";

            switch (passwordViewType)
            {
                case PasswordViewType.Connect:
                    _userNameTextBox.Text = "adm";
                    _userNameTextBox.IsEnabled = true;
                    break;

                case PasswordViewType.Reconnect:
                    _userNameTextBox.Text = "";
                    _userNameTextBox.IsEnabled = true;
                    break;

                case PasswordViewType.Validate:
                    _userNameTextBox.Text = FiresecManager.CurrentUser.Name;
                    _userNameTextBox.IsEnabled = false;
                    break;
            }
        }

        void OnSave(object sender, RoutedEventArgs e)
        {
            _statusTextBlock.Text = "Соединение с сервером";

            switch (_passwordViewType)
            {
                case PasswordViewType.Connect:
                    IsConnected = FiresecManager.Connect(_userNameTextBox.Text, _passwordTextBox.Password);
                    break;

                case PasswordViewType.Reconnect:
                    IsConnected = FiresecManager.Reconnect(_userNameTextBox.Text, _passwordTextBox.Password);
                    break;

                case PasswordViewType.Validate:
                    IsConnected = HashHelper.CheckPass(_passwordTextBox.Password, FiresecManager.CurrentUser.PasswordHash);
                    break;
            }

            if (IsConnected)
            {
                Close();
            }
            else
            {
                _statusTextBlock.Text = "Неправильное имя пользователя и пароль";
            }
        }

        void OnCancel(object sender, RoutedEventArgs e)
        {
            IsConnected = false;
            Close();
        }
    }
}