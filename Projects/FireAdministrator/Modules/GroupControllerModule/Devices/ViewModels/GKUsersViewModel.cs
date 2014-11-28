using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;
using System.Collections.ObjectModel;

namespace GKModule.ViewModels
{
	public class GKUsersViewModel : DialogViewModel
	{
		public GKUsersViewModel(List<GKUser> users)
		{
			Title = "Пользователи ГК";
			Users = new ObservableCollection<GKUser>(users);
		}

		public ObservableCollection<GKUser> Users { get; private set; }
	}
}