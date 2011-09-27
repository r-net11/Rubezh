using System.Windows;
using FiresecClient;

namespace FireAdministrator
{
    public partial class LoginScreen : Window
    {
        public LoginScreen()
        {
            InitializeComponent();
        }

        public bool IsLoggedIn { get; private set; }

        void OnConnect(object sender, RoutedEventArgs e)
        {
            string message = FiresecManager.Connect(_userName.Text, _password.Password);
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
    }
}