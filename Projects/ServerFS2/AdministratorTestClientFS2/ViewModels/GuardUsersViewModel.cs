using System.Collections.Generic;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using ServerFS2.Processor;

namespace AdministratorTestClientFS2.ViewModels
{
	class GuardUsersViewModel : DialogViewModel
	{
		public GuardUsersViewModel(Device device)
		{
			Title = "Охранные пользователи";
			selectedDeivce = device;
			SetGuardUsersCommand = new RelayCommand(OnSetGuardUsers);
		}
		private Device selectedDeivce;

		private List<GuardUser> guardUsers;
		public List<GuardUser> GuardUsers
		{
			get { return guardUsers; }
			set
			{
				guardUsers = value;
				OnPropertyChanged("GuardUsers");
			}
		}
		public RelayCommand SetGuardUsersCommand { get; private set; }
		private void OnSetGuardUsers()
		{
			MainManager.DeviceSetGuardUsers(selectedDeivce, GuardUsers);
		}
	}
}