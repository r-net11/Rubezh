using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using FiresecClient;
using System.Windows;

namespace SecurityModule.ViewModels
{
    public class SecurityViewModel : RegionViewModel
    {
        public void Initialize()
        {
            Users = new ObservableCollection<UserViewModel>();
            foreach (var user in FiresecManager.CoreConfig.user)
            {
				Users.Add(new UserViewModel(user));
            }

            Groups = new ObservableCollection<GroupViewModel>();
            foreach(var group in FiresecManager.CoreConfig.userGroup)
            {
				Groups.Add(new GroupViewModel(group));
            }

			AddUserCommand = new RelayCommand(OnAddUser);
			EditUserCommand = new RelayCommand(OnEditUser, CanEditUser);
			DeleteUserCommand = new RelayCommand(OnDeleteUser, CanEditUser);
			AddGroupCommand = new RelayCommand(OnAddGroup);
			EditGroupCommand = new RelayCommand(OnEditGroup, CanGroupEdit);
			DeleteGroupCommand = new RelayCommand(OnDeleteGroup, CanGroupEdit);
        }

		//Users
        ObservableCollection<UserViewModel> _users;
        public ObservableCollection<UserViewModel> Users
        {
            get { return _users; }
            set
            {
                _users = value;
                OnPropertyChanged("Users");
            }
        }

        UserViewModel _selectedUser;
        public UserViewModel SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                OnPropertyChanged("SelectedUser");
            }
        }
		//Groups
        ObservableCollection<GroupViewModel> _groups;
        public ObservableCollection<GroupViewModel> Groups
        {
            get { return _groups; }
            set
            {
                _groups = value;
                OnPropertyChanged("Groups");
            }
        }
		
        GroupViewModel _selecteGroup;
        public GroupViewModel SelectedGroup
        {
            get { return _selecteGroup; }
            set
            {
                _selecteGroup = value;
                OnPropertyChanged("SelecteGroup");
            }
        }
		//Commands
		public RelayCommand AddUserCommand { get; private set; }
		void OnAddUser()
		{
		}
		
		public RelayCommand EditUserCommand { get; private set; }
		void OnEditUser()
		{
			MessageBox.Show("Hello!");
		}
		bool CanEditUser(Object obj)
		{
			return Users.Count > 0 && SelectedUser != null;
		}
		public RelayCommand DeleteUserCommand { get; private set; }
		void OnDeleteUser()
		{
			var dialogResult = MessageBox.Show("Вы уверены, что хотите удалить пользователя " + SelectedUser.FullName, "Подтверждение", MessageBoxButton.YesNo);
			if (dialogResult == MessageBoxResult.Yes)
				Users.Remove(SelectedUser);

		}
		public RelayCommand AddGroupCommand { get; private set; }
		void OnAddGroup()
		{
		}
		public RelayCommand EditGroupCommand { get; private set; }
		void OnEditGroup()
		{

		}
		bool CanGroupEdit(Object obj)
		{
			return Groups.Count > 0 && SelectedGroup != null;
		}
		public RelayCommand DeleteGroupCommand { get; private set; }
		void OnDeleteGroup()
		{
			var dialogResult = MessageBox.Show("Вы уверены, что хотите удалить группу " + SelectedGroup.Name, "Подтверждение", MessageBoxButton.YesNo);
			if (dialogResult == MessageBoxResult.Yes)
				Groups.Remove(SelectedGroup);

		}
        public override void Dispose()
        {
        }
    }
}
