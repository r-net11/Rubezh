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

namespace FireAdministrator
{
    public partial class LoginScreen : Window
    {
        public LoginScreen()
        {
            InitializeComponent();
            UserName = null;
        }

        public string UserName { get; private set; }

        private void OnOk(object sender, RoutedEventArgs e)
        {
            string name = _login.Text;
            string password = _pass.Text;

            FiresecManager.Start(name, password);

            if (true)
            {
                UserName = name;
            }
            Close();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            UserName = null;
            Close();
        }
    }
}
