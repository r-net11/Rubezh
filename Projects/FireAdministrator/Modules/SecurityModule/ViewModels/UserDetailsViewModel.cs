using System;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using StrazhAPI.Enums;
using StrazhAPI.Models;
using FiresecClient;
using Infrastructure.Common;
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
			AvailableShellTypes = new ObservableCollection<ShellType>(Enum.GetValues(typeof(ShellType)).Cast<ShellType>().ToList());

			SetRolePermissionsCommand = new RelayCommand(OnSetRolePermissions);

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

			PermissionsViewModel = new PermissionsViewModel(User.PermissionStrings);
			CopyProperties();
		}

		void CopyProperties()
		{
			Login = User.Login;
			Name = User.Name;
			SelectedShellType = User.ShellType;

			RemoteAccess = (IsNew || User.RemoreAccess == null) ?
				new RemoteAccessViewModel(new RemoteAccess() { RemoteAccessType = RemoteAccessType.RemoteAccessBanned }) :
				new RemoteAccessViewModel(User.RemoreAccess);
		}

		PermissionsViewModel _permissionsViewModel;
		public PermissionsViewModel PermissionsViewModel
		{
			get { return _permissionsViewModel; }
			set
			{
				_permissionsViewModel = value;
				OnPropertyChanged(() => PermissionsViewModel);
			}
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		string _login;
		public string Login
		{
			get { return _login; }
			set
			{
				_login = value;
				OnPropertyChanged(() => Login);
			}
		}

		string _password;
		public string Password
		{
			get { return _password != null ? _password : ""; }
			set
			{
				_password = value;
				OnPropertyChanged(() => Password);
			}
		}

		string _passwordConfirmation;
		public string PasswordConfirmation
		{
			get { return _passwordConfirmation != null ? _passwordConfirmation : ""; }
			set
			{
				_passwordConfirmation = value;
				OnPropertyChanged(() => PasswordConfirmation);
			}
		}

		bool _isChangePassword;
		public bool IsChangePassword
		{
			get { return _isChangePassword; }
			set
			{
				_isChangePassword = value;
				OnPropertyChanged(() => IsChangePassword);
			}
		}

		public bool CanChangeLogin
		{
			get { return User != FiresecManager.CurrentUser; }
		}

		bool CheckLogin()
		{
			if (string.IsNullOrWhiteSpace(Login))
			{
				MessageBoxService.Show("Логин не может быть пустым");
				return false;
			}
			else if (Login != User.Login && FiresecManager.SecurityConfiguration.Users.Any(user => user.Login == Login))
			{
				MessageBoxService.Show("Пользователь с таким логином уже существует");
				return false;
			}
			return true;
		}

		bool CheckPassword()
		{
			if (Password != PasswordConfirmation)
			{
				MessageBoxService.Show("Поля \"Пароль\" и \"Подтверждение\" должны совпадать");
				return false;
			}
			return true;
		}

		void PreventAdminPermissions()
		{
			if (FiresecManager.CurrentUser.UID == User.UID && FiresecManager.CurrentUser.PermissionStrings.Contains(PermissionType.Adm_Security.ToString()))
			{
				var Adm_SecurityPermission = PermissionsViewModel.AllPermissions.FirstOrDefault(x => x.Name == PermissionType.Adm_Security.ToString());
				if (Adm_SecurityPermission != null)
				{
					Adm_SecurityPermission.IsChecked = true;
				}

				var Adm_SetNewConfigPermission = PermissionsViewModel.AllPermissions.FirstOrDefault(x => x.Name == PermissionType.Adm_SetNewConfig.ToString());
				if (Adm_SetNewConfigPermission != null)
				{
					Adm_SetNewConfigPermission.IsChecked = true;
				}
			}
		}

		public ObservableCollection<ShellType> AvailableShellTypes { get; private set; }

		private ShellType _selectedShellType;
		public ShellType SelectedShellType
		{
			get { return _selectedShellType; }
			set
			{
				if (_selectedShellType == value)
					return;
				_selectedShellType = value;
				OnPropertyChanged(() => SelectedShellType);
			}
		}

		public RelayCommand SetRolePermissionsCommand { get; private set; }
		void OnSetRolePermissions()
		{
			var roleSelectationViewModel = new RoleSelectationViewModel();
			if (DialogService.ShowModalWindow(roleSelectationViewModel))
			{
				PermissionsViewModel = new PermissionsViewModel(roleSelectationViewModel.SelectedRole.Role.PermissionStrings);
			}
		}

		void SaveProperties()
		{
			User.Login = Login;
			User.Name = Name;
			User.ShellType = SelectedShellType;

			if (IsChangePassword)
				User.PasswordHash = HashHelper.GetHashFromString(Password);

			PreventAdminPermissions();
			User.PermissionStrings = PermissionsViewModel.GetPermissionStrings();
			User.RemoreAccess = RemoteAccess.GetModel();
		}

		protected override bool Save()
		{
			if (CheckLogin() && CheckPassword())
				SaveProperties();
			else
				return false;
			return base.Save();
		}
	}
}