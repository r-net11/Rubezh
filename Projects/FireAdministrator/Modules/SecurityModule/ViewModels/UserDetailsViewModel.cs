using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using Controls.MessageBox;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace SecurityModule.ViewModels
{
    public class UserDetailsViewModel : SaveCancelDialogContent
    {
        public RemoteAccessViewModel RemoteAccess { get; set; }
        public User User { get; private set; }
        public bool IsNew { get; private set; }
        public bool IsNotNew { get { return !IsNew; } }

        public UserDetailsViewModel(User user = null)
        {
            if (user != null)
            {
                Title = string.Format("Свойства учетной записи: {0}", user.Name);
                IsNew = false;
                IsChangePassword = false;
                User = user;
            }
            else
            {
                Title = "Создание новой учетной записи";
                IsNew = true;
                IsChangePassword = true;

                User = new User()
                {
                    Id = FiresecManager.SecurityConfiguration.Users.Max(x => x.Id) + 1,
                    Name = "",
                    Login = "",
                    PasswordHash = HashHelper.GetHashFromString("")
                };
            }

            CopyProperties();
        }

        void CopyProperties()
        {
            Login = User.Login;
            Name = User.Name;

            Roles = new ObservableCollection<UserRole>();
            foreach (var role in FiresecManager.SecurityConfiguration.UserRoles)
                Roles.Add(role);

            if (IsNew)
            {
                if (Roles.Count > 0)
                    UserRole = Roles[0];
            }
            else
            {
                UserRole = Roles.FirstOrDefault(role => role.Id == User.RoleId);
            }

            RemoteAccess = (IsNew || User.RemoreAccess == null) ?
                new RemoteAccessViewModel(new RemoteAccess() { RemoteAccessType = RemoteAccessType.RemoteAccessBanned }) :
                new RemoteAccessViewModel(User.RemoreAccess);
        }

        string _login;
        public string Login
        {
            get { return _login; }
            set
            {
                _login = value;
                OnPropertyChanged("Login");
            }
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

        string _password;
        public string Password
        {
            get { return _password != null ? _password : ""; }
            set
            {
                _password = value;
                OnPropertyChanged("Password");
            }
        }

        string _passwordConfirmation;
        public string PasswordConfirmation
        {
            get { return _passwordConfirmation != null ? _passwordConfirmation : ""; }
            set
            {
                _passwordConfirmation = value;
                OnPropertyChanged("PasswordConfirmation");
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

        public ObservableCollection<UserRole> Roles { get; private set; }

        UserRole _userRole;
        public UserRole UserRole
        {
            get { return _userRole; }
            set
            {
                _userRole = value;
                if (_userRole != null)
                {
                    if (Roles[0] == null)
                        Roles.RemoveAt(0);

                    Permissions = new ObservableCollection<PermissionViewModel>(
                        _userRole.Permissions.Select(permissionType => new PermissionViewModel(permissionType) { IsEnable = true })
                    );

                    CheckPermissions();
                }
                else
                {
                    Permissions = new ObservableCollection<PermissionViewModel>();
                }

                OnPropertyChanged("UserRole");
            }
        }

        void CheckPermissions()
        {
            if (UserRole.Id != User.RoleId && UserRole.Permissions.IsNotNullOrEmpty())
                return;

            foreach (var permissionType in User.Permissions)
            {
                if (!Permissions.Any(x => x.PermissionType == permissionType))
                    User.Permissions.Remove(permissionType);
            }

            foreach (var permission in Permissions)
            {
                if (!User.Permissions.Any(x => x == permission.PermissionType))
                    permission.IsEnable = false;
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

        void SaveProperties()
        {
            User.Login = Login;
            User.Name = Name;

            if (IsChangePassword)
                User.PasswordHash = HashHelper.GetHashFromString(Password);

            User.RoleId = UserRole.Id;
            User.Permissions = new List<PermissionType>(
                Permissions.Where(x => x.IsEnable).Select(x => x.PermissionType)
            );
            User.RemoreAccess = RemoteAccess.GetModel();
        }

        protected override void Save(ref bool cancel)
        {
            if (CheckLogin() && CheckPassword() && CheckRole())
                SaveProperties();
            else
                cancel = true;
        }

        void ShowMessage(string message)
        {
            MessageBoxService.Show(message);
        }

        bool CheckLogin()
        {
            if (string.IsNullOrWhiteSpace(Login))
            {
                ShowMessage("Логин не может быть пустым");
                return false;
            }
            else if (Login != User.Login && FiresecManager.SecurityConfiguration.Users.Any(user => user.Login == Login))
            {
                ShowMessage("Пользователь с таким логином уже существует");
                return false;
            }
            return true;
        }

        bool CheckPassword()
        {
            if (Password != PasswordConfirmation)
            {
                ShowMessage("Поля \"Пароль\" и \"Подтверждение\" должны совпадать");
                return false;
            }
            return true;
        }

        bool CheckRole()
        {
            if (UserRole == null)
            {
                ShowMessage("Не выбрана роль");
                return false;
            }
            return true;
        }
    }
}