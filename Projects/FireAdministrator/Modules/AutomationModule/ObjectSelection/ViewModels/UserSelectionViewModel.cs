using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Common;
using StrazhAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using Localization.Automation.ViewModels;

namespace AutomationModule.ViewModels
{
	public class UserSelectionViewModel : SaveCancelDialogViewModel
	{
		public UserSelectionViewModel(User user)
		{
			Title = CommonViewModel.UserSelectionViewModel_Title;

			Users = new SortableObservableCollection<UserViewModel>();
			if (FiresecManager.SecurityConfiguration.Users != null)
			{
				foreach (var currentUser in FiresecManager.SecurityConfiguration.Users.OrderBy(x => x.Name))
					Users.Add(new UserViewModel(currentUser));
			}
			SelectedUser = user == null
				? Users.FirstOrDefault()
				: Users.FirstOrDefault(u => u.User.UID == user.UID);
		}

		SortableObservableCollection<UserViewModel> _users;
		public SortableObservableCollection<UserViewModel> Users
		{
			get { return _users; }
			set
			{
				_users = value;
				OnPropertyChanged(() => Users);
			}
		}

		UserViewModel _selectedUser;
		public UserViewModel SelectedUser
		{
			get { return _selectedUser; }
			set
			{
				_selectedUser = value;
				OnPropertyChanged(() => SelectedUser);
			}
		}
	}
}