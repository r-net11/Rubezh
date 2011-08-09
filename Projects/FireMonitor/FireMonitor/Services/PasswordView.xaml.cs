using System.Windows;
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

        void OnSave(object sender, RoutedEventArgs e)
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

        void OnCancel(object sender, RoutedEventArgs e)
        {
            IsAutorised = false;
            Close();
        }

        public static bool Check(string password, string hash)
        {
            var md5CrytoProvider = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(password);
            bs = md5CrytoProvider.ComputeHash(bs);
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