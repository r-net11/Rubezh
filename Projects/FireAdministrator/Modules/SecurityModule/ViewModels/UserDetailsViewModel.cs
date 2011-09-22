using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace SecurityModule.ViewModels
{
    public class UserDetailsViewModel : SaveCancelDialogContent
    {
        public User User { get; private set; }

        public UserDetailsViewModel()
        {
            Title = "Новый пользователь";
            User = new User()
            {
                Id = (int.Parse(FiresecManager.SecurityConfiguration.Users.Last().Id) + 1).ToString(),
                FullName = "",
                Name = "",
                PasswordHash = HashHelper.GetHashFromString(""),
            };

            Initialize();
        }

        public UserDetailsViewModel(User user)
        {
            Title = "Редактирование пользователя";
            User = user;

            Initialize();
        }

        void Initialize()
        {
            CopyProperties();
        }

        string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        string _fullName;
        public string FullName
        {
            get { return _fullName; }
            set
            {
                _fullName = value;
                OnPropertyChanged("FullName");
            }
        }

        string _passHash;
        public string PasswordHash
        {
            get { return _passHash; }
            set
            {
                _passHash = value;
                OnPropertyChanged("PasswordHash");
            }
        }

        string _newPassHash;
        public string NewPasswordHash
        {
            get { return _newPassHash; }
            set
            {
                _newPassHash = value;
                OnPropertyChanged("NewPasswordHash");
            }
        }

        bool _isChangePassword;
        public bool IsChangePassword
        {
            get { return _isChangePassword; }
            set
            {
                _isChangePassword = value;
                OnPropertyChanged("IsChangePassword");
            }
        }

        UserGroup _userRole;
        public UserGroup UserRole
        {
            get { return _userRole; }
            set
            {
                _userRole = value;
                if (_userRole != null)
                {
                    Permissions = new ObservableCollection<PermissionViewModel>(
                        _userRole.Permissions.Select(x => new PermissionViewModel((PermissionType) (int.Parse(x))))
                    );

                    foreach (var permissionId in User.Permissions)
                    {
                        var permission = Permissions.FirstOrDefault(x => ((int) (x.PermissionType)).ToString() == permissionId);
                        if (permission == null)
                        {
                            User.Permissions.Remove(permissionId);
                        }
                        else
                        {
                            permission.IsEnable = true;
                        }
                    }
                }
                OnPropertyChanged("UserRole");
            }
        }

        ObservableCollection<PermissionViewModel> _permissions;
        public ObservableCollection<PermissionViewModel> Permissions
        {
            get { return _permissions; }
            set
            {
                _permissions = value;
                OnPropertyChanged("Permissions");
            }
        }

        void CopyProperties()
        {
            Name = User.Name;
            FullName = User.FullName;
            PasswordHash = User.PasswordHash;
            if (User.Groups.IsNotNullOrEmpty())
            {
                UserRole = FiresecManager.SecurityConfiguration.UserGroups.FirstOrDefault(group => group.Id == User.Groups[0]);
            }
            else
            {
                UserRole = null;
            }
        }

        void SaveProperties()
        {
            User.Name = Name;
            User.FullName = FullName;
            if (IsChangePassword)
                User.PasswordHash = NewPasswordHash;
            User.Groups = new List<string>() { UserRole.Id };
            User.Permissions = new List<string>(
                Permissions.
                Where(x => x.IsEnable).
                Select(x => ((int) (x.PermissionType)).ToString())
            );
        }

        protected override void Save(ref bool cancel)
        {
            SaveProperties();
        }

        protected override bool CanSave()
        {
            return User.Name != Name;
        }
    }
}