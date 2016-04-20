using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using ManagementConsole.ViewModels;

namespace ManagementConsole
{
	public class GlobalSettingsViewModel : BaseViewModel
	{
		public GlobalSettingsViewModel()
		{
			SaveCommand = new RelayCommand(OnSave);
			RemoteAddress = GlobalSettingsHelper.GlobalSettings.RemoteAddress;
			RemotePort = GlobalSettingsHelper.GlobalSettings.RemotePort;
			Login		= GlobalSettingsHelper.GlobalSettings.AdminLogin;
			Password	= GlobalSettingsHelper.GlobalSettings.AdminPassword;
			AutoConnect = GlobalSettingsHelper.GlobalSettings.AdminAutoConnect;
			Server_EnableRemoteConnections = GlobalSettingsHelper.GlobalSettings.Server_EnableRemoteConnections;
			DBServerName = GlobalSettingsHelper.GlobalSettings.DBServerName;
			CreateNewDBOnOversize = GlobalSettingsHelper.GlobalSettings.CreateNewDBOnOversize;
			DoNotAutoconnectAdm = GlobalSettingsHelper.GlobalSettings.DoNotAutoconnectAdm;
			RunRevisor = GlobalSettingsHelper.GlobalSettings.RunRevisor;

			Modules = new List<ModuleViewModel>();
			
			Modules.Add(new ModuleViewModel("PlansModule.dll"));
			Modules.Add(new ModuleViewModel("PlansModule.Kursk.dll"));
			Modules.Add(new ModuleViewModel("SecurityModule.dll"));
			Modules.Add(new ModuleViewModel("SoundsModule.dll"));
			Modules.Add(new ModuleViewModel("SettingsModule.dll"));
			Modules.Add(new ModuleViewModel("GKModule.dll"));
			Modules.Add(new ModuleViewModel("OPCModule.dll"));
			Modules.Add(new ModuleViewModel("NotificationModule.dll"));
			Modules.Add(new ModuleViewModel("VideoModule.dll"));
			Modules.Add(new ModuleViewModel("DiagnosticsModule.dll"));
			Modules.Add(new ModuleViewModel("ReportsModule.dll"));
			Modules.Add(new ModuleViewModel("SKDModule.dll"));
			Modules.Add(new ModuleViewModel("LayoutModule.dll"));
			Modules.Add(new ModuleViewModel("AutomationModule.dll"));
			Modules.Add(new ModuleViewModel("FiltersModule.dll"));
			Modules.Add(new ModuleViewModel("JournalModule.dll"));

			Modules.Add(new ModuleViewModel("DevicesModule.dll"));
			Modules.Add(new ModuleViewModel("LibraryModule.dll"));
			Modules.Add(new ModuleViewModel("InstructionsModule.dll"));
			Modules.Add(new ModuleViewModel("AlarmModule.dll"));

			if (GlobalSettingsHelper.GlobalSettings.ModuleItems == null)
				GlobalSettingsHelper.GlobalSettings.SetDefaultModules();
			foreach (var moduleName in GlobalSettingsHelper.GlobalSettings.ModuleItems)
			{
				var moduleViewModel = Modules.FirstOrDefault(x => x.Name == moduleName);
				if (moduleViewModel != null)
				{
					moduleViewModel.IsSelected = true;
				}
			}

			ManagementConsoleViewModel.Curent.HasChanges = false;
		}

		public List<ModuleViewModel> Modules { get; private set; }

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

		bool _doNotAutoconnectAdm;
		public bool DoNotAutoconnectAdm
		{
			get { return _doNotAutoconnectAdm; }
			set
			{
				_doNotAutoconnectAdm = value;
				OnPropertyChanged("DoNotAutoconnectAdm");
			}
		}

		bool _runRevisor;
		public bool RunRevisor
		{
			get { return _runRevisor; }
			set
			{
				_runRevisor = value;
				OnPropertyChanged("RunRevisor");
			}
		}

		bool _server_EnableRemoteConnections;
		public bool Server_EnableRemoteConnections
		{
			get { return _server_EnableRemoteConnections; }
			set
			{
				_server_EnableRemoteConnections = value;
				OnPropertyChanged("Server_EnableRemoteConnections");
			}
		}

		string _dbServerName;
		public string DBServerName
		{
			get { return _dbServerName; }
			set
			{
				_dbServerName = value;
				OnPropertyChanged("DBServerName");
			}
		}

		bool _createNewDBOnOversize;
		public bool CreateNewDBOnOversize
		{
			get { return _createNewDBOnOversize; }
			set
			{
				_createNewDBOnOversize = value;
				OnPropertyChanged("CreateNewDBOnOversize");
			}
		}

		public bool IsDebug
		{
			get
			{
#if DEBUG
				return true;
#endif
				return false;
			}
		}

		public RelayCommand SaveCommand { get; private set; }
		void OnSave()
		{
			GlobalSettingsHelper.GlobalSettings.RemoteAddress = RemoteAddress;
			GlobalSettingsHelper.GlobalSettings.RemotePort = RemotePort;
			GlobalSettingsHelper.GlobalSettings.AdminLogin = Login;
			GlobalSettingsHelper.GlobalSettings.AdminPassword = Password;
			GlobalSettingsHelper.GlobalSettings.AdminAutoConnect = AutoConnect;
			GlobalSettingsHelper.GlobalSettings.DoNotAutoconnectAdm = DoNotAutoconnectAdm;
			GlobalSettingsHelper.GlobalSettings.RunRevisor = RunRevisor;

			GlobalSettingsHelper.GlobalSettings.Server_EnableRemoteConnections = Server_EnableRemoteConnections;
			GlobalSettingsHelper.GlobalSettings.DBServerName = DBServerName;
			GlobalSettingsHelper.GlobalSettings.CreateNewDBOnOversize = CreateNewDBOnOversize;

			GlobalSettingsHelper.GlobalSettings.ModuleItems = new List<string>();
			foreach (var moduleViewModel in Modules)
			{
				if (moduleViewModel.IsSelected)
				{
					GlobalSettingsHelper.GlobalSettings.ModuleItems.Add(moduleViewModel.Name);
				}
			}

			GlobalSettingsHelper.Save();
			ManagementConsoleViewModel.Curent.HasChanges = false;
		}

		new void OnPropertyChanged(string propertyName)
		{
			base.OnPropertyChanged(propertyName);
			ManagementConsoleViewModel.Curent.HasChanges = true;
		}
	}
}