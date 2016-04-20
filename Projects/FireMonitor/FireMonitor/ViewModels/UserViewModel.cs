using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.Common.Windows;
using RubezhClient;
using Infrastructure;
using Infrastructure.Events;
using Infrastructure.Common.Windows.Windows;
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
			var changeUserViewModel = new ChangeUserViewModel(botstrapper);
			DialogService.ShowModalWindow(changeUserViewModel);	
		}
	}
}