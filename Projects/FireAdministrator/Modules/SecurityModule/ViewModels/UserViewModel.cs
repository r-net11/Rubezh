using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

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
				OnPropertyChanged("User");
			}
		}
	}
}