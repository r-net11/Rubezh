using System.Windows;
using FiresecClient;

namespace FireMonitor
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            IsLoggedIn = false;
        }

        public bool IsLoggedIn { get; private set; }

        private void OnConnect(object sender, RoutedEventArgs e)
        {
            _info.Text = "Соединение с сервером";

            string login = _login.Text;
            string password = _pass.Text;

            bool result = FiresecManager.Connect(login, password);

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

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            IsLoggedIn = false;
            Close();
        }
    }
}