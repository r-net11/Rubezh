using System.Windows.Controls;
using FireMonitor.ViewModels;
using Infrastructure;
using Infrastructure.Common;

namespace FireMonitor
{
    public partial class UserView : UserControl
    {
        public UserView()
        {
            InitializeComponent();
            ChangeUserCommand = new RelayCommand(OnChangeUser);
            DataContext = this;
        }

        public RelayCommand ChangeUserCommand { get; private set; }
        void OnChangeUser()
        {
            var loginViewModel = new LoginViewModel(LoginViewModel.PasswordViewType.Reconnect);
            if (ServiceFactory.UserDialogs.ShowModalWindow(loginViewModel))
            {
            }
        }
    }
}