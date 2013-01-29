using System.Windows.Controls;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;

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
			var result = ServiceFactory.LoginService.ExecuteReconnect();
			if (result)
			{
				ServiceFactory.Events.GetEvent<UserChangedEvent>().Publish(true);
			}
        }
    }
}