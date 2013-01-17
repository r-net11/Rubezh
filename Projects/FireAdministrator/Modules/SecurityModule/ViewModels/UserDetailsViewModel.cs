using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SecurityModule.ViewModels
{
	public class UserDetailsViewModel : SaveCancelDialogViewModel
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
				UserRole = Roles.FirstOrDefault();
			}
			else
			{
				UserRole = Roles.FirstOrDefault(role => role.UID == User.RoleUID);
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
						_userRole.PermissionStrings.Select(x => new PermissionViewModel(x) { IsEnable = true })
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
			if (UserRole.UID != User.RoleUID)
				return;

			foreach (var permissionString in User.PermissionStrings)
			{
				if (!Permissions.Any(x => x.Name == permissionString))
					User.PermissionStrings.Remove(permissionString);
			}

			foreach (var permission in Permissions)
			{
				if (!User.PermissionStrings.Any(x => x == permission.Name))
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

			User.RoleUID = UserRole.UID;
            PreventAdminPermissions();
            User.PermissionStrings = new List<string>();
            foreach (var permission in Permissions)
            {
                if (permission.IsEnable)
                {
                    User.PermissionStrings.Add(permission.Name);
                }
            }
            User.RemoreAccess = RemoteAccess.GetModel();
        }

		protected override bool Save()
		{
			if (CheckLogin() && CheckPassword() && CheckRole())
				SaveProperties();
			else
				return false;
			return base.Save();
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

        void PreventAdminPermissions()
        {
			if (FiresecManager.CurrentUser.UID == User.UID && FiresecManager.CurrentUser.PermissionStrings.Contains(PermissionType.Adm_Security.ToString()))
            {
                var Adm_SecurityPermission = Permissions.FirstOrDefault(x => x.Name == PermissionType.Adm_Security.ToString());
                if (Adm_SecurityPermission != null)
                {
                    Adm_SecurityPermission.IsEnable = true;
                }

				var Adm_SetNewConfigPermission = Permissions.FirstOrDefault(x => x.Name == PermissionType.Adm_SetNewConfig.ToString());
                if (Adm_SetNewConfigPermission != null)
                {
                    Adm_SetNewConfigPermission.IsEnable = true;
                }
            }
        }
	}
}