using System.Windows.Controls;

namespace FireAdministrator.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
            Loaded += new System.Windows.RoutedEventHandler(LoginView_Loaded);
        }

        void LoginView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _btnOK.Focus();
        }
    }
}