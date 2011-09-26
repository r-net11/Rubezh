using System;
using System.Threading;
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

            string[] args = { _userName.Text, _password.Password };
            ThreadPool.QueueUserWorkItem(new WaitCallback(TryConnect), args);
        }

        void TryConnect(object obj)
        {
            string[] args = obj as string[];
            IsLoggedIn = FiresecManager.Connect(args[0], args[1], true, false);
            if (IsLoggedIn)
            {
                Dispatcher.Invoke(new Action(() => Close()));
            }

            Dispatcher.Invoke(new Action(() => _info.Text = "Не удается установить связь с сервером"));
        }

        void OnCancel(object sender, RoutedEventArgs e)
        {
            IsLoggedIn = false;
            Close();
        }
    }
}