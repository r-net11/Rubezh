using System.Windows;
using System.Windows.Input;
using FiresecClient;

namespace FireAdministrator
{
    public partial class LoginScreen : Window
    {
        public LoginScreen()
        {
            InitializeComponent();
            _userName.Text = "adm";
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

        void LoginViewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && e.IsRepeat == false && e.IsToggled == false)
                OnConnect(sender, new RoutedEventArgs());
        }
    }
}