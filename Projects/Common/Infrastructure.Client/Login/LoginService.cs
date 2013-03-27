using System.Windows;
using FiresecAPI.Models;
using Infrastructure.Client.Login.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Client.Properties;

namespace Infrastructure.Client.Login
{
	public class LoginService
	{
		private ClientType _clientType;
		private string _title;
		public string Login { get; private set; }
		public string Password { get; private set; }

		public LoginService(ClientType clientType, string title = null)
		{
			_clientType = clientType;
			_title = title ?? "Авторизация";
		}

		public bool ExecuteConnect(string login = null, string password = null, bool isMulticlient = false)
		{
			return Execute(LoginViewModel.PasswordViewType.Connect, login, password, isMulticlient);
		}
		public bool ExecuteReconnect()
		{
			return Execute(LoginViewModel.PasswordViewType.Reconnect);
		}
		public bool ExecuteValidate()
		{
			return Execute(LoginViewModel.PasswordViewType.Validate);
		}

		bool Execute(LoginViewModel.PasswordViewType passwordViewType, string login = null, string password = null, bool isMulticlient = false)
		{
			Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
			var loginViewModel = new LoginViewModel(_clientType, passwordViewType) { Title = _title };
			bool isAutoconnect = GlobalSettingsHelper.GlobalSettings.AutoConnect && passwordViewType == LoginViewModel.PasswordViewType.Connect;
			if (isAutoconnect)
			{
				loginViewModel.UserName = GlobalSettingsHelper.GlobalSettings.Login;
				loginViewModel.Password = GlobalSettingsHelper.GlobalSettings.Password;
			}

			while (!loginViewModel.IsConnected && !loginViewModel.IsCanceled)
			{
				if (login != null && password != null)
				{
					loginViewModel.UserName = login;
					loginViewModel.Password = password;
					loginViewModel.SaveCommand.Execute();
				}
				else
				{
					if (isAutoconnect && (Settings.Default.UserName != "adm" || !GlobalSettingsHelper.GlobalSettings.DoNotAutoconnectAdm))
					{
						loginViewModel.SaveCommand.Execute();
					}
					else
					{
						DialogService.ShowModalWindow(loginViewModel);
					}
				}
				if (!string.IsNullOrEmpty(loginViewModel.Message))
					MessageBoxService.Show(loginViewModel.Message);
				isAutoconnect = false;

				if (isMulticlient)
					break;
			}
			Application.Current.ShutdownMode = ShutdownMode.OnLastWindowClose;
			Login = loginViewModel.UserName;
			Password = loginViewModel.Password;
			return loginViewModel.IsConnected;
		}
	}
}