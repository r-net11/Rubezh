using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FiresecClient;

namespace FireMonitor
{
    public partial class LoginScreen : Window
    {
        public LoginScreen()
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
                FiresecManager.Disconnect();
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
