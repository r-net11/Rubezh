using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class UserViewModel : BaseViewModel
	{
		public UserViewModel(GuardUser guardUser)
		{
			GuardUser = guardUser;
		}

		GuardUser _guardUser;
		public GuardUser GuardUser
		{
			get { return _guardUser; }
			set
			{
				_guardUser = value;
				OnPropertyChanged(() => GuardUser);
			}
		}
	}
}