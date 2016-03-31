using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using FiresecClient;
using Infrastructure;
using Infrastructure.Events;

namespace FireMonitor.ViewModels
{
	public class UserViewModel : BaseViewModel
	{
		public UserViewModel()
		{
			ChangeUserCommand = new RelayCommand(OnChangeUser);
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