using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Windows;
using Infrastructure.Common;

namespace Infrastructure.Client.Startup.ViewModels
{
	public class StartupSettingsViewModel : SaveCancelDialogViewModel
	{
		private StartupViewModel _startupViewModel;
		public StartupSettingsViewModel(StartupViewModel startupViewModel)
		{
			Title = "Настройки подключения";
			AutoConnect = GlobalSettingsHelper.GlobalSettings.AutoConnect;
			RemotePort = GlobalSettingsHelper.GlobalSettings.RemotePort;
			RemoteAddress = GlobalSettingsHelper.GlobalSettings.RemoteAddress;
			Login = GlobalSettingsHelper.GlobalSettings.Login;
			Password = GlobalSettingsHelper.GlobalSettings.Password;
			_startupViewModel = startupViewModel;
		}

		public override void OnLoad()
		{
			Surface.Owner = _startupViewModel.Surface;
			Surface.ShowInTaskbar = false;
			Surface.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		}

		private bool _autoConnect;
		public bool AutoConnect
		{
			get { return _autoConnect; }
			set
			{
				_autoConnect = value;
				OnPropertyChanged(() => AutoConnect);
			}
		}
		private string _remoteAddress;
		public string RemoteAddress
		{
			get { return _remoteAddress; }
			set
			{
				_remoteAddress = value;
				OnPropertyChanged(() => RemoteAddress);
			}
		}
		private int _remotePort;
		public int RemotePort
		{
			get { return _remotePort; }
			set
			{
				_remotePort = value;
				OnPropertyChanged(() => RemotePort);
			}
		}
		private string _login;
		public string Login
		{
			get { return _login; }
			set
			{
				_login = value;
				OnPropertyChanged(() => Login);
			}
		}
		private string _password;
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
			GlobalSettingsHelper.GlobalSettings.RemotePort = RemotePort;
			GlobalSettingsHelper.GlobalSettings.RemoteAddress = RemoteAddress;
			GlobalSettingsHelper.GlobalSettings.Login = Login;
			GlobalSettingsHelper.GlobalSettings.Password = Password;
			GlobalSettingsHelper.Save();
			return base.Save();
		}
		protected override bool CanSave()
		{
			return base.CanSave();
		}
	}
}
