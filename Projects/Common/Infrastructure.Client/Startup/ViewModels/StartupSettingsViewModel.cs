using Infrastructure.Common.Windows.ViewModels;
using System.Windows;
using Infrastructure.Common;
using Infrastructure.Common.Windows;

namespace Infrastructure.Client.Startup.ViewModels
{
	public class StartupSettingsViewModel : SaveCancelDialogViewModel
	{
		public StartupSettingsViewModel()
		{
			Title = Resources.Language.Startup.ViewModels.StartupSettingsViewModel.Title;
			TopMost = true;
			AutoConnect = GlobalSettingsHelper.GlobalSettings.AutoConnect;
			RemotePort = GlobalSettingsHelper.GlobalSettings.RemotePort;
			ReportRemotePort = GlobalSettingsHelper.GlobalSettings.ReportRemotePort;
			RemoteAddress = GlobalSettingsHelper.GlobalSettings.RemoteAddress;
			Login = GlobalSettingsHelper.GlobalSettings.Login;
			Password = GlobalSettingsHelper.GlobalSettings.Password;
		}

		public override void OnLoad()
		{
			Surface.Owner = StartupService.Instance.OwnerWindow;
			Surface.ShowInTaskbar = false;
			Surface.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		}
		public override int GetPreferedMonitor()
		{
			return MonitorHelper.FindMonitor(StartupService.Instance.OwnerWindow.RestoreBounds);
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
			GlobalSettingsHelper.GlobalSettings.AutoConnect = AutoConnect;
			GlobalSettingsHelper.GlobalSettings.ReportRemotePort = ReportRemotePort;
			GlobalSettingsHelper.GlobalSettings.RemotePort = RemotePort;
			GlobalSettingsHelper.GlobalSettings.RemoteAddress = RemoteAddress;
			GlobalSettingsHelper.GlobalSettings.Login = Login;
			GlobalSettingsHelper.GlobalSettings.Password = Password;
			GlobalSettingsHelper.Save();
			ConnectionSettingsManager.Update();
			return base.Save();
		}
		protected override bool CanSave()
		{
			return base.CanSave();
		}
	}
}