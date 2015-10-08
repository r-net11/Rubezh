﻿using Common;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class UserDetailsViewModel : SaveCancelDialogViewModel
	{
		public User User { get; private set; }

		public bool IsNew { get; private set; }

		public UserDetailsViewModel(User user= null)
		{
			if (user != null)
			{
				Title = string.Format("Свойства учетной записи: {0}", user.Name);
				IsNew = true;
				IsChangePassword = false;
				User = DBCash.GetUser(user.UID);
			}

			else
			{
				Title = "Создание новой учетной записи";
				IsNew = false;
				IsChangePassword = true;

				User = new User()
				{
					Name = "",
					Login = "",
					PasswordHash = HashHelper.GetHashFromString("")
				};

			}
			PermissionsViewModel = new PermissionsViewModel(User);
			CopyProperty();
		}
		void CopyProperty()
		{
			Login = User.Login;
			Name = User.Name;
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

		bool CheckLogin()
		{
			if (string.IsNullOrWhiteSpace(Login))
			{
				MessageBoxService.Show("Логин не может быть пустым");
				return false;
			}
			else if (Login != User.Login && DBCash.Users.Any(x => x.Login == Login))
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

		void SaveProperties()
		{
			User.Login = Login;
			User.Name = Name;

			if (IsChangePassword)
				User.PasswordHash = HashHelper.GetHashFromString(Password);

			PermissionsViewModel.SavePermissions(User);
		}

		protected override bool Save()
		{
			if (CheckLogin() && CheckPassword())
				SaveProperties();
			else
				return false;
			DBCash.SaveUser(User);
			return base.Save();
		}

	}
}