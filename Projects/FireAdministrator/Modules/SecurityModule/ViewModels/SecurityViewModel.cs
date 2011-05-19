using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using FiresecClient;

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
        }

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
        public GroupViewModel SelecteGroup
        {
            get { return _selecteGroup; }
            set
            {
                _selecteGroup = value;
                OnPropertyChanged("SelecteGroup");
            }
        }

        public override void Dispose()
        {
        }
    }
}
