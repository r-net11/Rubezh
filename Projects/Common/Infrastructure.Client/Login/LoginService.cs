using System.Windows;
using FiresecAPI.Models;
using Infrastructure.Client.Login.ViewModels;
using Infrastructure.Client.Properties;
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
			bool isClientAutoconnect;
			bool isAutoconnect;
			
			switch (_clientType)
			{
				case ClientType.Administrator:
					isClientAutoconnect = GlobalSettingsHelper.GlobalSettings.AdminAutoConnect;
					break;
				case ClientType.Monitor:
					isClientAutoconnect = GlobalSettingsHelper.GlobalSettings.MonitorAutoConnect;
					break;
				default:
					isClientAutoconnect = false;
					break;
			}
			if (isClientAutoconnect && passwordViewType == LoginViewModel.PasswordViewType.Connect)
			{
				isAutoconnect = true;
			}
			else 
			{ 
				isAutoconnect = false; 
			}
			
			var saveCredential = !isAutoconnect;
			if (isAutoconnect)
			{
				switch (_clientType)
				{
					case ClientType.Administrator:
						loginViewModel.UserName = GlobalSettingsHelper.GlobalSettings.AdminLogin;
						loginViewModel.Password = GlobalSettingsHelper.GlobalSettings.AdminPassword;
						break;
					case ClientType.Monitor:
						loginViewModel.UserName = GlobalSettingsHelper.GlobalSettings.MonitorLogin;
						loginViewModel.Password = GlobalSettingsHelper.GlobalSettings.MonitorPassword;
						break;
				}
				
			}
			else
			{
				loginViewModel.UserName = Settings.Default.UserName;
				loginViewModel.Password = Settings.Default.Password;
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
					if (isAutoconnect && (loginViewModel.UserName != "adm" || !GlobalSettingsHelper.GlobalSettings.DoNotAutoconnectAdm))
						loginViewModel.SaveCommand.Execute();
					else
						DialogService.ShowModalWindow(loginViewModel);
				}
				if (!string.IsNullOrEmpty(loginViewModel.Message))
					MessageBoxService.Show(loginViewModel.Message);
				isAutoconnect = false;
			}
			if (loginViewModel.IsConnected && saveCredential && (Settings.Default.UserName != loginViewModel.UserName || Settings.Default.Password != (loginViewModel.SavePassword ? loginViewModel.Password : string.Empty)))
			{
				Settings.Default.UserName = loginViewModel.UserName;
				Settings.Default.Password = loginViewModel.SavePassword ? loginViewModel.Password : string.Empty;
				Settings.Default.Save();
			}
			Application.Current.ShutdownMode = ShutdownMode.OnLastWindowClose;
			Login = loginViewModel.UserName;
			Password = loginViewModel.Password;
			return loginViewModel.IsConnected;
		}
	}
}