using RubezhAPI.Models;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace SecurityModule.ViewModels
{
	public class UserViewModel : BaseViewModel
	{
		public UserViewModel(User user)
		{
			User = user;
		}

		User _user;
		public User User
		{
			get { return _user; }
			set
			{
				_user = value;
				OnPropertyChanged(() => User);
			}
		}
	}
}