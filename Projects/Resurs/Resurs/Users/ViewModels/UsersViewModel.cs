using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Resurs.ViewModels
{
	public class UsersViewModel : BaseViewModel
	{
		public UsersViewModel()
		{
			RemoveCommand = new RelayCommand(OnDelete, CanDelete);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			AddCommand = new RelayCommand(OnAdd);

			Users = new ObservableCollection<UserViewModel>();
			DBCash.Users.ForEach(x => Users.Add(new UserViewModel(x)));
			SelectedUser = Users.FirstOrDefault();
			if (DBCash.GetUser(SelectedUser.User.UID) != null)
				SelectedUser.User = DBCash.GetUser(SelectedUser.User.UID);
		}

		public ObservableCollection<UserViewModel> Users { get; private set; }

		UserViewModel _selectedUser;
		public UserViewModel SelectedUser
		{
			get { return _selectedUser; }
			set
			{
				_selectedUser = value;
				if (_selectedUser != null)
					_selectedUser.User = DBCash.GetUser(SelectedUser.User.UID);
				OnPropertyChanged(() => SelectedUser);
			}
		}

		public RelayCommand AddCommand { get; set; }
		void OnAdd()
		{
			var userDetailsViewModel = new UserDetailsViewModel();
			if (DialogService.ShowModalWindow(userDetailsViewModel))
			{
				DBCash.AddJournalForUser(JournalType.AddUser, userDetailsViewModel.User);

				var userViewModel = new UserViewModel(userDetailsViewModel.User);
				Users.Add(userViewModel);
				SelectedUser = userViewModel;
			}
		}

		public RelayCommand EditCommand { get; set; }
		void OnEdit()
		{
			var userDetailsViewModel = new UserDetailsViewModel(SelectedUser.User);
			if (DialogService.ShowModalWindow(userDetailsViewModel))
			{
				SelectedUser.User = userDetailsViewModel.User;
				if (DBCash.CurrentUser != null && DBCash.CurrentUser.UID == userDetailsViewModel.User.UID)
					DBCash.CurrentUser = userDetailsViewModel.User;
				Bootstrapper.MainViewModel.UpdateTabsIsVisible();
				if (userDetailsViewModel.IsChange)
					DBCash.AddJournalForUser(JournalType.EditUser, SelectedUser.User);
			}
		}

		bool CanEdit()
		{
			return SelectedUser != null ;
		}

		public bool IsVisible
		{
			get { return DBCash.CheckPermission(PermissionType.ViewUser); }
		}

		public bool IsEdit { get { return DBCash.CheckPermission(PermissionType.EditUser); } }

		public RelayCommand RemoveCommand { get; set; }
		void OnDelete()
		{
			if (MessageBoxService.ShowQuestion(string.Format("Вы уверенны, что хотите удалить пользователя \"{0}\" из списка", SelectedUser.User.Name)))
			{
				var index = Users.IndexOf(SelectedUser);
				DBCash.AddJournalForUser(JournalType.DeleteUser, SelectedUser.User);
				DBCash.DeleteUser(SelectedUser.User);
				Users.Remove(SelectedUser);

				index = Math.Min(index, Users.Count - 1);
				if (index > -1)
					SelectedUser = Users[index];
			}
		}

		bool CanDelete()
		{
			return SelectedUser != null && DBCash.CurrentUser != null && SelectedUser.User.UID != DBCash.CurrentUser.UID;
		}

		public void Select(Guid userUID)
		{
			if (userUID != Guid.Empty && IsVisible)
			{
				var userViewModel = Users.FirstOrDefault(x => x.User.UID == userUID);
				if (userViewModel != null)
				{
					Bootstrapper.MainViewModel.SelectedTabIndex = 6;
					SelectedUser = userViewModel;
				}
			}
		}
	}
}