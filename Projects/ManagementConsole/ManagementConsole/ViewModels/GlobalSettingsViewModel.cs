using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;

namespace ManagementConsole
{
	public class GlobalSettingsViewModel : BaseViewModel
	{
		public GlobalSettingsViewModel()
		{
			SaveCommand = new RelayCommand(OnSave);
			RemoteAddress = GlobalSettingsHelper.GlobalSettings.RemoteAddress;
			RemotePort = GlobalSettingsHelper.GlobalSettings.RemotePort;
			Login = GlobalSettingsHelper.GlobalSettings.Login;
			Password = GlobalSettingsHelper.GlobalSettings.Password;
			AutoConnect = GlobalSettingsHelper.GlobalSettings.AutoConnect;
			DoNotOverrideFS1 = GlobalSettingsHelper.GlobalSettings.DoNotOverrideFS1;
			LibVlcDllsPath = GlobalSettingsHelper.GlobalSettings.LibVlcDllsPath;
			IsExpertMode = GlobalSettingsHelper.GlobalSettings.IsExpertMode;
			EnableRemoteConnections = GlobalSettingsHelper.GlobalSettings.EnableRemoteConnections;
			IsImitatorEnabled = GlobalSettingsHelper.GlobalSettings.IsImitatorEnabled;
			Modules = GlobalSettingsHelper.GlobalSettings.Modules;
			FS_RemoteAddress = GlobalSettingsHelper.GlobalSettings.FS_RemoteAddress;
			FS_Port = GlobalSettingsHelper.GlobalSettings.FS_Port;
			FS_Login = GlobalSettingsHelper.GlobalSettings.FS_Login;
			FS_Password = GlobalSettingsHelper.GlobalSettings.FS_Password;
		}

		string _remoteAddress;
		public string RemoteAddress
		{
			get { return _remoteAddress; }
			set
			{
				_remoteAddress = value;
				OnPropertyChanged("RemoteAddress");
			}
		}

		int _remotePort;
		public int RemotePort
		{
			get { return _remotePort; }
			set
			{
				_remotePort = value;
				OnPropertyChanged("RemotePort");
			}
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

		string _password;
		public string Password
		{
			get { return _password; }
			set
			{
				_password = value;
				OnPropertyChanged("Password");
			}
		}

		bool _autoConnect;
		public bool AutoConnect
		{
			get { return _autoConnect; }
			set
			{
				_autoConnect = value;
				OnPropertyChanged("AutoConnect");
			}
		}

		bool _doNotOverrideFS1;
		public bool DoNotOverrideFS1
		{
			get { return _doNotOverrideFS1; }
			set
			{
				_doNotOverrideFS1 = value;
				OnPropertyChanged("DoNotOverrideFS1");
			}
		}

		string _libVlcDllsPath;
		public string LibVlcDllsPath
		{
			get { return _libVlcDllsPath; }
			set
			{
				_libVlcDllsPath = value;
				OnPropertyChanged("LibVlcDllsPath");
			}
		}

		bool _isExpertMode;
		public bool IsExpertMode
		{
			get { return _isExpertMode; }
			set
			{
				_isExpertMode = value;
				OnPropertyChanged("IsExpertMode");
			}
		}

		bool _enableRemoteConnections;
		public bool EnableRemoteConnections
		{
			get { return _enableRemoteConnections; }
			set
			{
				_enableRemoteConnections = value;
				OnPropertyChanged("EnableRemoteConnections");
			}
		}

		bool _isImitatorEnabled;
		public bool IsImitatorEnabled
		{
			get { return _isImitatorEnabled; }
			set
			{
				_isImitatorEnabled = value;
				OnPropertyChanged("IsImitatorEnabled");
			}
		}

		string _modules;
		public string Modules
		{
			get { return _modules; }
			set
			{
				_modules = value;
				OnPropertyChanged("Modules");
			}
		}

		string _fS_RemoteAddress;
		public string FS_RemoteAddress
		{
			get { return _fS_RemoteAddress; }
			set
			{
				_fS_RemoteAddress = value;
				OnPropertyChanged("FS_RemoteAddress");
			}
		}

		int _fS_Port;
		public int FS_Port
		{
			get { return _fS_Port; }
			set
			{
				_fS_Port = value;
				OnPropertyChanged("FS_Port");
			}
		}

		string _fS_Login;
		public string FS_Login
		{
			get { return _fS_Login; }
			set
			{
				_fS_Login = value;
				OnPropertyChanged("FS_Login");
			}
		}

		string _fS_Password;
		public string FS_Password
		{
			get { return _fS_Password; }
			set
			{
				_fS_Password = value;
				OnPropertyChanged("FS_Password");
			}
		}

		public RelayCommand SaveCommand { get; private set; }
		void OnSave()
		{
			GlobalSettingsHelper.GlobalSettings.RemoteAddress = RemoteAddress;
			GlobalSettingsHelper.GlobalSettings.RemotePort = RemotePort;
			GlobalSettingsHelper.GlobalSettings.Login = Login;
			GlobalSettingsHelper.GlobalSettings.Password = Password;
			GlobalSettingsHelper.GlobalSettings.AutoConnect = AutoConnect;
			GlobalSettingsHelper.GlobalSettings.DoNotOverrideFS1 = DoNotOverrideFS1;
			GlobalSettingsHelper.GlobalSettings.LibVlcDllsPath = LibVlcDllsPath;
			GlobalSettingsHelper.GlobalSettings.IsExpertMode = IsExpertMode;
			GlobalSettingsHelper.GlobalSettings.EnableRemoteConnections = EnableRemoteConnections;
			GlobalSettingsHelper.GlobalSettings.IsImitatorEnabled = IsImitatorEnabled;
			GlobalSettingsHelper.GlobalSettings.Modules = Modules;
			GlobalSettingsHelper.GlobalSettings.FS_RemoteAddress = FS_RemoteAddress;
			GlobalSettingsHelper.GlobalSettings.FS_Port = FS_Port;
			GlobalSettingsHelper.GlobalSettings.FS_Login = FS_Login;
			GlobalSettingsHelper.GlobalSettings.FS_Password = FS_Password;
			GlobalSettingsHelper.Save();	
		}
	}
}