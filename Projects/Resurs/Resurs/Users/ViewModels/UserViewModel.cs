using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class UserViewModel: BaseViewModel
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