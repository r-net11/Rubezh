using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Common;

namespace Resurs.ViewModels
{
	public class FilterUsersViewModel
	{
		public FilterUsersViewModel(Filter filter)
		{
			FilterUserViewModel = new ObservableCollection<FilterUserViewModel>();
			DBCash.GetAllUsers().ForEach(x => FilterUserViewModel.Add(new FilterUserViewModel(x)));
			FilterUserViewModel.ForEach(x =>
				{
					if (filter.UserUIDs.Contains(x.User.UID))
						x.IsChecked = true;
				});
		}
		public ObservableCollection<FilterUserViewModel> FilterUserViewModel { get; set; }

		public List<Guid?> GetUserUIDs()
		{
			List<Guid?> UserUIDs = new List<Guid?>();
			foreach (var tariff in FilterUserViewModel)
			{
				if (tariff.IsChecked)
					UserUIDs.Add(tariff.User.UID);
			}
			return UserUIDs;
		}
	}
}
