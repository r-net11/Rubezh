using System.Windows.Controls;
using SecurityModule.ViewModels;

namespace SecurityModule.Views
{
    public partial class UserDetailsView : UserControl
    {
        public UserDetailsView()
        {
            InitializeComponent();
        }

        void Password_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            (DataContext as UserDetailsViewModel).Password = (sender as PasswordBox).Password;
        }

        void NewPassword_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            (DataContext as UserDetailsViewModel).NewPassword = (sender as PasswordBox).Password;
        }

        void NewPasswordConfirmition_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            (DataContext as UserDetailsViewModel).NewPasswordConfirmation = (sender as PasswordBox).Password;
        }
    }
}