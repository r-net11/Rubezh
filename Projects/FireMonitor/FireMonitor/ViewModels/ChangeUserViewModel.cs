using Common;
using RubezhAPI.Models;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;

namespace FireMonitor.ViewModels
{
	public class ChangeUserViewModel : SaveCancelDialogViewModel
	{
		Bootstrapper botstrapper;
		public ChangeUserViewModel(Bootstrapper botstrapper)
		{
			this.botstrapper = botstrapper;
			Title = "Смена пользователя";
			Password = "";
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
			get { return _password; }
			set
			{
				_password = value;
				OnPropertyChanged(() => Password);
			}
		}
		private string GetRestartCommandLineArguments()
		{
			string commandLineArguments = null;
			if (Login != null && _password != null)
				commandLineArguments = "login='" + _login + "' password='" + _password + "'";
			return commandLineArguments;
		}
		protected override bool Save()
		{
			var user = ClientManager.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == Login);
			if (user != null)
			{
				if (!HashHelper.CheckPass(Password, user.PasswordHash))
				{
					MessageBoxService.Show("Неверно задан логин/пароль");
					return false;
				}

				if (!user.HasPermission(PermissionType.Oper_Login))
				{
					MessageBoxService.Show("У данного пользователя нет права на вход");
					return false;
				}
				
					ClientManager.Disconnect();
					botstrapper.Restart(Login, Password);

					return base.Save();
			}
			else 
				MessageBoxService.Show("Неверно задан логин/пароль");
			
			return false;
		}
	}
}