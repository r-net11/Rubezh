using System.Windows.Controls;

namespace FireAdministrator.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
            KeyDown += new System.Windows.Input.KeyEventHandler(LoginView_KeyDown);
            Loaded += new System.Windows.RoutedEventHandler(LoginView_Loaded);
        }

        void LoginView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _btnOK.Focus();
        }

        void LoginView_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            
        }
    }
}
