using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
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
            Title = "Создание новой учетной записи";
            IsNew = true;

            User = new User()
            {
                Id = FiresecManager.SecurityConfiguration.Users.Max(x => x.Id) + 1,
                Name = "",
                Login = "",
                PasswordHash = HashHelper.GetHashFromString(""),
            };

            Initialize();
        }

        public UserDetailsViewModel(User user)
        {
            Title = string.Format("Свойства учетной записи: {0}", user.Name);
            IsNew = false;

            User = user;

            Initialize();
        }

        void Initialize()
        {
            Password = string.Empty;
            NewPassword = string.Empty;
            NewPasswordConfirmation = string.Empty;

            Roles = new ObservableCollection<UserRole>() { null };
            if (FiresecManager.SecurityConfiguration.UserRoles.IsNotNullOrEmpty())
            {
                foreach (var role in FiresecManager.SecurityConfiguration.UserRoles)
                    Roles.Add(role);
            }

            CopyProperties();
        }

        public bool IsNew { get; private set; }
        public bool IsNotNew { get { return !IsNew; } }

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
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged("Password");
            }
        }

        string _newPass;
        public string NewPassword
        {
            get { return _newPass; }
            set
            {
                _newPass = value;
                OnPropertyChanged("NewPassword");
            }
        }

        string _newPassConfirm;
        public string NewPasswordConfirmation
        {
            get { return _newPassConfirm; }
            set
            {
                _newPassConfirm = value;
                OnPropertyChanged("NewPasswordConfirmation");
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
                {
                    User.Permissions.Remove(permissionType);
                }
            }

            foreach (var permission in Permissions)
            {
                if (!User.Permissions.Any(x => x == permission.PermissionType))
                {
                    permission.IsEnable = false;
                }
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
            Login = User.Login;
            Name = User.Name;
            UserRole = IsNew ? Roles[0] : Roles.Where(role => role != null).First(role => role.Id == User.RoleId);
        }

        void SaveProperties()
        {
            User.Login = Login;
            User.Name = Name;
            if (IsNew)
            {
                User.PasswordHash = HashHelper.GetHashFromString(Password);
            }
            else if (IsChangePassword)
            {
                User.PasswordHash = HashHelper.GetHashFromString(NewPassword);
            }
            User.RoleId = UserRole.Id;
            User.Permissions = new List<PermissionType>(
                Permissions.Where(x => x.IsEnable).
                Select(x => x.PermissionType)
            );
        }

        protected override void Save(ref bool cancel)
        {
            if (CheckLogin() && CheckPass() && CheckRole())
                SaveProperties();
            else
                cancel = true;
        }

        void ShowMessage(string message)
        {
            DialogBox.DialogBox.Show(message, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        bool CheckLogin()
        {
            if (string.IsNullOrWhiteSpace(Login))
            {
                ShowMessage("Сначала введите логин");
                return false;
            }
            else if (Login != User.Login && FiresecManager.SecurityConfiguration.Users.Any(user => user.Login == Login))
            {
                ShowMessage("Введенный логин уже зарезервирован");
                return false;
            }
            return true;
        }

        bool CheckPass()
        {
            if (IsNew)
            {
                if (NewPassword != NewPasswordConfirmation)
                {
                    ShowMessage("Поля \"Пароль\" и \"Подтверждение\" должны совпадать");
                    return false;
                }
            }
            else if (IsChangePassword)
            {
                if (User.PasswordHash != HashHelper.GetHashFromString(Password))
                {
                    ShowMessage("Значение в поле \"Действующий пароль\" неверное");
                    return false;
                }
                if (NewPassword != NewPasswordConfirmation)
                {
                    ShowMessage("Поля \"Новый пароль\" и \"Подтверждение\" должны совпадать");
                    return false;
                }
            }
            return true;
        }

        bool CheckRole()
        {
            if (UserRole == null)
            {
                ShowMessage("Сначала выберите роль");
                return false;
            }
            return true;
        }
    }
}