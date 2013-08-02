using System.Windows.Controls;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using FiresecClient;

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

		public string UserName
		{
			get { return FiresecManager.CurrentUser.Name; }
		}

		public RelayCommand ChangeUserCommand { get; private set; }
		void OnChangeUser()
		{
			var oldUserName = UserName;
			var result = ServiceFactory.LoginService.ExecuteReconnect();
			if (result)
			{
				var userChangedEventArgs = new UserChangedEventArgs()
				{
					IsReconnect = true,
					OldName = oldUserName,
					NewName = UserName
				};
				ServiceFactory.Events.GetEvent<UserChangedEvent>().Publish(userChangedEventArgs);
			}
		}
	}
}