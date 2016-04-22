using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using RubezhClient;
using Infrastructure;
using Infrastructure.Events;
using Infrastructure.Common.Windows;
using System.Diagnostics;
using System.Windows;

namespace FireMonitor.ViewModels
{
	public class UserViewModel : BaseViewModel
	{
		Bootstrapper botstrapper;
		public UserViewModel(Bootstrapper botstrapper)
		{
			this.botstrapper = botstrapper;
			ChangeUserCommand = new RelayCommand(OnChangeUser);
		}

		public string UserName
		{
			get { return ClientManager.CurrentUser.Name; }
		}

		public RelayCommand ChangeUserCommand { get; private set; }
		void OnChangeUser()
		{
			var changeUserViewModel = new ChangeUserViewModel(botstrapper, ClientManager.CurrentUser.UID);
			DialogService.ShowModalWindow(changeUserViewModel);	
		}
	}
}