using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Resurs.ViewModels
{
	public class UsersViewModel : BaseViewModel
	{
		public UsersViewModel()
		{
			RemoveCommand = new RelayCommand(OnDelete);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			AddCommand = new RelayCommand(OnAdd);
			Build();
		}

		ObservableCollection<UserViewModel> _users;
		public ObservableCollection<UserViewModel> Users
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

		public RelayCommand AddCommand { get; set; }

		void OnAdd()
		{
			var userDetailsViewModel = new UserDetailsViewModel();
			DialogService.ShowModalWindow(userDetailsViewModel);
			{
				var userViewModel = new UserViewModel(userDetailsViewModel.User);

				Users.Add(userViewModel);
				CashUser.Users.Add(userDetailsViewModel.User);
				SelectedUser = userViewModel;
			}
		}

		bool CanEdit()
		{
			return SelectedUser != null;
		}

		void Build ()
		{
			Users = new ObservableCollection<UserViewModel>();
			CashUser.Users.ForEach(x => Users.Add(new UserViewModel(x)));
			SelectedUser = Users.FirstOrDefault();
		}

		public RelayCommand EditCommand { get; set; }
		void OnEdit()
		{
			var userDetailsViewModel = new UserDetailsViewModel(SelectedUser.User);
			if (DialogService.ShowModalWindow(userDetailsViewModel))
			{
				CashUser.Users.Remove(SelectedUser.User);
				CashUser.Users.Add(userDetailsViewModel.User);
				SelectedUser.User = userDetailsViewModel.User;
			}
		}

		public RelayCommand RemoveCommand { get; set; }

		void OnDelete()
		{
			if (MessageBoxService.ShowQuestion(string.Format("Вы уверенны, что хотите удалить пользователя \"{0}\" из списка", SelectedUser.User.Name)))
			{
				var index = Users.IndexOf(SelectedUser);
				CashUser.Users.Remove(SelectedUser.User);
				Users.Remove(SelectedUser);

				index = Math.Min(index, Users.Count - 1);
				if (index > -1)
					SelectedUser = Users[index];
			}
		}
	}
}