using System.Windows;
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
                    IsConnected = Check(_passwordTextBox.Password, FiresecManager.CurrentUser.PasswordHash);
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

        public static bool Check(string password, string hash)
        {
            var mD5CryptoServiceProvider = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            passwordBytes = mD5CryptoServiceProvider.ComputeHash(passwordBytes);
            var stringBuilder = new System.Text.StringBuilder();
            foreach (byte passwordByte in passwordBytes)
            {
                stringBuilder.Append(passwordByte.ToString("x2").ToLower());
            }
            string realHash = stringBuilder.ToString();

            return realHash.ToLower() == hash.ToLower();
        }
    }
}