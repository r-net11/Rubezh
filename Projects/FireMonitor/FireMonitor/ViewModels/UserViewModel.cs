using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using FiresecClient;
using Infrastructure;
using Infrastructure.Events;
using Infrastructure.Common.Windows;
using System.Diagnostics;
using System.Windows;

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
			var changeUserViewModel = new ChangeUserViewModel();
			DialogService.ShowModalWindow(changeUserViewModel);	
		}
	}
}