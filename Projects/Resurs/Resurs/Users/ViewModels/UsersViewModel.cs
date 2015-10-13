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
			AddCommand = new RelayCommand(OnAdd,CanAdd);
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
			if(DialogService.ShowModalWindow(userDetailsViewModel))
			{
				var userViewModel = new UserViewModel(userDetailsViewModel.User);

				Users.Add(userViewModel);
				DBCash.AddJournal(JournalType.AddUser, DBCash.CurrentUser.UID, userDetailsViewModel.User.UID, DBCash.CurrentUser.Name, userDetailsViewModel.User.Name);
				SelectedUser = userViewModel;
			}
		}

		bool CanAdd()
		{
			return SelectedUser != null && DBCash.CurrentUser.UserPermissions.Any(x => x.PermissionType == PermissionType.EditUser);
		}

		void Build ()
		{
			Users = new ObservableCollection<UserViewModel>();
			 DBCash.Users.ForEach(x => Users.Add(new UserViewModel(x)));
			SelectedUser = Users.FirstOrDefault();
		}

		public RelayCommand EditCommand { get; set; }
		void OnEdit()
		{
			var userDetailsViewModel = new UserDetailsViewModel(SelectedUser.User);
			if (DialogService.ShowModalWindow(userDetailsViewModel))
			{
				SelectedUser.User = userDetailsViewModel.User;
				if (userDetailsViewModel.IsChange)
					DBCash.AddJournal(JournalType.EditUser, DBCash.CurrentUser.UID, SelectedUser.User.UID, DBCash.CurrentUser.Name, SelectedUser.User.Name);
			}
		}

		bool CanEdit()
		{
			return SelectedUser != null && DBCash.CurrentUser.UserPermissions.Any(x => x.PermissionType == PermissionType.EditUser);
		}

		public bool IsVisibility
		{
			get {return DBCash.CurrentUser.UserPermissions.Any(x=> x.PermissionType == PermissionType.User);}
		}

		public RelayCommand RemoveCommand { get; set; }
		void OnDelete()
		{
			if (MessageBoxService.ShowQuestion(string.Format("Вы уверенны, что хотите удалить пользователя \"{0}\" из списка", SelectedUser.User.Name)))
			{
				var index = Users.IndexOf(SelectedUser);
				DBCash.AddJournal(JournalType.DeleteUser, DBCash.CurrentUser.UID, SelectedUser.User.UID, DBCash.CurrentUser.Name, SelectedUser.User.Name);
				DBCash.DeleteUser(SelectedUser.User);
				Users.Remove(SelectedUser);

				index = Math.Min(index, Users.Count - 1);
				if (index > -1)
					SelectedUser = Users[index];
			}
		}

		bool CanDelete()
		{
			return SelectedUser != null && SelectedUser.User.UID != DBCash.CurrentUser.UID;
		}
	}
}