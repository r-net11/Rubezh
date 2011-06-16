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
			SecuritySubjects = new ObservableCollection<string> { Const.Users, Const.Groups };
			SelectedSecuritySubject = Const.Users;
			IsUsersVisible = true;

            var users = FiresecManager.Configuration.Users;
            var groups = FiresecManager.Configuration.UserGroups;
            var permissions = FiresecManager.Configuration.Perimissions;
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

		bool _isUsersVisible;
		public bool IsUsersVisible
		{
			get
			{
				return _isUsersVisible;
			}
			set
			{
				_isUsersVisible = value;
				OnPropertyChanged("IsUsersVisible");
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

		ObservableCollection<string> _securitySubjects;

		public ObservableCollection<string> SecuritySubjects
		{
			get
			{
				return _securitySubjects;
			}
			set
			{
				_securitySubjects = value;
				OnPropertyChanged("SecuritySubjects");
			}
		}
		string _selectedSecuritySubject;
		public string SelectedSecuritySubject
		{
			get
			{
				return _selectedSecuritySubject;
			}
			set
			{
				_selectedSecuritySubject = value;
				OnPropertyChanged("SelectedSecuritySubject");
				TuneViews();
			}
		}

		private void TuneViews()
		{
			IsUsersVisible = !IsUsersVisible;
		}
        public override void Dispose()
        {
        }
    }
}
