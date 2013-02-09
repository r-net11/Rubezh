using System.Windows;
using FiresecAPI.Models;
using Infrastructure.Client.Login.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;

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

		public bool ExecuteConnect(string login = null, string password = null)
		{
			return Execute(LoginViewModel.PasswordViewType.Connect, login, password);
		}
		public bool ExecuteReconnect()
		{
			return Execute(LoginViewModel.PasswordViewType.Reconnect);
		}
		public bool ExecuteValidate()
		{
			return Execute(LoginViewModel.PasswordViewType.Validate);
		}

		bool Execute(LoginViewModel.PasswordViewType passwordViewType, string login = null, string password = null)
		{
			Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
			var loginViewModel = new LoginViewModel(_clientType, passwordViewType) { Title = _title };
			bool isAutoconnect = GlobalSettingsHelper.GlobalSettings.AutoConnect && passwordViewType == LoginViewModel.PasswordViewType.Connect;

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
					if (isAutoconnect)
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
			}
			Application.Current.ShutdownMode = ShutdownMode.OnLastWindowClose;
			Login = loginViewModel.UserName;
			Password = loginViewModel.Password;
			return loginViewModel.IsConnected;
		}
	}
}