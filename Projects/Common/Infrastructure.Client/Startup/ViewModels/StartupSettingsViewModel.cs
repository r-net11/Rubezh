using Infrastructure.Common.Windows.ViewModels;
using System.Windows;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using RubezhAPI.Models;

namespace Infrastructure.Client.Startup.ViewModels
{
	public class StartupSettingsViewModel : SaveCancelDialogViewModel
	{
		public StartupSettingsViewModel(ClientType clientType)
		{
			Title = "Настройки подключения";
			TopMost = true;
			ClientType = clientType;
			RemotePort = GlobalSettingsHelper.GlobalSettings.RemotePort;
			ReportRemotePort = GlobalSettingsHelper.GlobalSettings.ReportRemotePort;
			RemoteAddress = GlobalSettingsHelper.GlobalSettings.RemoteAddress;
			switch (clientType)
			{
				case ClientType.Administrator:
					AutoConnect = GlobalSettingsHelper.GlobalSettings.AdminAutoConnect;
					Login = GlobalSettingsHelper.GlobalSettings.AdminLogin;
					Password = GlobalSettingsHelper.GlobalSettings.AdminPassword;
					break;
				case ClientType.Monitor:
					AutoConnect = GlobalSettingsHelper.GlobalSettings.MonitorAutoConnect;
					Login = GlobalSettingsHelper.GlobalSettings.MonitorLogin;
					Password = GlobalSettingsHelper.GlobalSettings.MonitorPassword;
					break;
			}
		}

		public ClientType ClientType { get; set; }

		public override void OnLoad()
		{
			Surface.Owner = StartupService.Instance.OwnerWindow;
			Surface.ShowInTaskbar = false;
			Surface.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		}

		bool _autoConnect;
		public bool AutoConnect
		{
			get { return _autoConnect; }
			set
			{
				_autoConnect = value;
				OnPropertyChanged(() => AutoConnect);
			}
		}
		string _remoteAddress;
		public string RemoteAddress
		{
			get { return _remoteAddress; }
			set
			{
				_remoteAddress = value;
				OnPropertyChanged(() => RemoteAddress);
			}
		}
		int _remotePort;
		public int RemotePort
		{
			get { return _remotePort; }
			set
			{
				_remotePort = value;
				OnPropertyChanged(() => RemotePort);
			}
		}
		int _reportRemotePort;
		public int ReportRemotePort
		{
			get { return _reportRemotePort; }
			set
			{
				_reportRemotePort = value;
				OnPropertyChanged(() => ReportRemotePort);
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
			get { return _password; }
			set
			{
				_password = value;
				OnPropertyChanged(() => Password);
			}
		}

		protected override bool Save()
		{
			GlobalSettingsHelper.GlobalSettings.ReportRemotePort = ReportRemotePort;
			GlobalSettingsHelper.GlobalSettings.RemotePort = RemotePort;
			GlobalSettingsHelper.GlobalSettings.RemoteAddress = RemoteAddress;

			switch (ClientType)
			{
				case ClientType.Administrator:
					GlobalSettingsHelper.GlobalSettings.AdminAutoConnect = AutoConnect;
					GlobalSettingsHelper.GlobalSettings.AdminLogin = Login;
					GlobalSettingsHelper.GlobalSettings.AdminPassword = Password;
					break;
				case ClientType.Monitor:
					GlobalSettingsHelper.GlobalSettings.MonitorAutoConnect = AutoConnect;
					GlobalSettingsHelper.GlobalSettings.MonitorLogin = Login;
					GlobalSettingsHelper.GlobalSettings.MonitorPassword = Password;
					break;
			}

			GlobalSettingsHelper.Save();
			return base.Save();
		}
		protected override bool CanSave()
		{
			return base.CanSave();
		}
	}
}