using System.Windows.Controls;
using Infrastructure;
using Infrastructure.Common;

namespace FireMonitor.Views
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
			ServiceFactory.LoginService.ExecuteReconnect();
        }
    }
}