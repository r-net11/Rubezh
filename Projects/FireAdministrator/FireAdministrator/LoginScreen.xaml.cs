using System.Windows;
using FiresecClient;

namespace FireAdministrator
{
    public partial class LoginScreen : Window
    {
        public LoginScreen()
        {
            InitializeComponent();
            IsLoggedIn = false;
        }

        public bool IsLoggedIn { get; private set; }

        void OnConnect(object sender, RoutedEventArgs e)
        {
            _info.Text = "Соединение с сервером";

            string name = _login.Text;
            string password = _pass.Text;

            bool result = FiresecManager.Connect(name, password);

            if (result)
            {
                IsLoggedIn = true;
                _info.Text = "Соединение с сервером установлено";
            }
            else
            {
                _info.Text = "Не удается установить связь с сервером";
                //FiresecManager.Disconnect();
                return;
            }
            Close();
        }

        void OnCancel(object sender, RoutedEventArgs e)
        {
            IsLoggedIn = false;
            Close();
        }
    }
}