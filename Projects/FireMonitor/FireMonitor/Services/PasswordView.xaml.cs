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

namespace FireMonitor.Services
{
    public partial class PasswordView : Window
    {
        public PasswordView()
        {
            InitializeComponent();
            IsAutorised = false;
            _login.Text = FiresecManager.CurrentUser.Name;
        }

        public bool IsAutorised { get; private set; }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            _info.Text = "Соединение с сервером";

            string login = _login.Text;
            string password = _pass.Text;

            bool result = Check(password, FiresecManager.CurrentUser.PasswordHash);

            if (result)
            {
                IsAutorised = true;
            }
            else
            {
                _info.Text = "Неправильное имя пользователя и пароль";
                return;
            }
            Close();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            IsAutorised = false;
            Close();
        }

        public static bool Check(string password, string hash)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(password);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            string realHash = s.ToString();

            if (realHash.ToLower() == hash.ToLower())
            {
                return true;
            }
            return false;
        }
    }
}
