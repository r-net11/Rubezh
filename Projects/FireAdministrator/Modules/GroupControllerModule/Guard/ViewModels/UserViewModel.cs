using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class UserViewModel : BaseViewModel
	{
		public UserViewModel(XGuardUser guardUser)
		{
			GuardUser = guardUser;
		}

		XGuardUser _guardUser;
		public XGuardUser GuardUser
		{
			get { return _guardUser; }
			set
			{
				_guardUser = value;
				OnPropertyChanged("GuardUser");
			}
		}
	}
}