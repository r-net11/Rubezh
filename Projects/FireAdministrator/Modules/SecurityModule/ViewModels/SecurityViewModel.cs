using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace SecurityModule.ViewModels
{
    public class SecurityViewModel : RegionViewModel
    {
        public SecurityViewModel()
        {
            AddUserCommand = new RelayCommand(OnAddUser);
            EditUserCommand = new RelayCommand(OnEditUser, CanEditUser);
            DeleteUserCommand = new RelayCommand(OnDeleteUser, CanEditUser);
            AddGroupCommand = new RelayCommand(OnAddGroup);
            EditGroupCommand = new RelayCommand(OnEditGroup, CanGroupEdit);
            DeleteGroupCommand = new RelayCommand(OnDeleteGroup, CanGroupEdit);
        }

        List<User> _usersList;
        public void Initialize()
        {
            var _usersList = FiresecManager.SecurityConfiguration.Users;
            var groups = FiresecManager.SecurityConfiguration.UserGroups;
            //var permissions = FiresecManager.Configuration.Perimissions;

            Users = new ObservableCollection<UserViewModel>();
            if (_usersList != null)
                foreach (var user in _usersList)
                {
                    Users.Add(new UserViewModel(user));
                }

            Groups = new ObservableCollection<GroupViewModel>();
            if (groups != null)
                foreach (var group in groups)
                {
                    Groups.Add(new GroupViewModel(group));
                }
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
            var newUser = new User();
            var newUserViewModel = new NewUserViewModel(newUser);
            var result = ServiceFactory.UserDialogs.ShowModalWindow(newUserViewModel);
            if (result)
            {
                _usersList.Add(newUser);
                var userViewModel = new UserViewModel(newUser);
                Users.Add(userViewModel);
            }
        }

        public RelayCommand EditUserCommand { get; private set; }
        void OnEditUser()
        {
            MessageBox.Show("Hello!");
        }

        bool CanEditUser()
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

        bool CanGroupEdit()
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
    }
}