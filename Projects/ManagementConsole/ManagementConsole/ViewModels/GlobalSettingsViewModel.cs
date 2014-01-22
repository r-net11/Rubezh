using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
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
			Login = GlobalSettingsHelper.GlobalSettings.Login;
			Password = GlobalSettingsHelper.GlobalSettings.Password;
			AutoConnect = GlobalSettingsHelper.GlobalSettings.AutoConnect;
			IsGKAsAService = GlobalSettingsHelper.GlobalSettings.IsGKAsAService;
			UseGKHash = GlobalSettingsHelper.GlobalSettings.UseGKHash;
			DoNotOverrideFS1 = GlobalSettingsHelper.GlobalSettings.DoNotOverrideFS1;
			DoNotAutoconnectAdm = GlobalSettingsHelper.GlobalSettings.DoNotAutoconnectAdm;
			RunRevisor = GlobalSettingsHelper.GlobalSettings.RunRevisor;
			FS_RemoteAddress = GlobalSettingsHelper.GlobalSettings.FS_RemoteAddress;
			FS_Port = GlobalSettingsHelper.GlobalSettings.FS_Port;
			FS_Login = GlobalSettingsHelper.GlobalSettings.FS_Login;
			FS_Password = GlobalSettingsHelper.GlobalSettings.FS_Password;

			Modules = new List<ModuleViewModel>();
			Modules.Add(new ModuleViewModel("DevicesModule.dll"));
			Modules.Add(new ModuleViewModel("PlansModule.dll"));
			Modules.Add(new ModuleViewModel("PlansModule.Kursk.dll"));
			Modules.Add(new ModuleViewModel("LibraryModule.dll"));
			Modules.Add(new ModuleViewModel("SecurityModule.dll"));
			Modules.Add(new ModuleViewModel("FiltersModule.dll"));
			Modules.Add(new ModuleViewModel("SoundsModule.dll"));
			Modules.Add(new ModuleViewModel("InstructionsModule.dll"));
			Modules.Add(new ModuleViewModel("SettingsModule.dll"));
			Modules.Add(new ModuleViewModel("GKModule.dll"));
			Modules.Add(new ModuleViewModel("OPCModule.dll"));
			Modules.Add(new ModuleViewModel("NotificationModule.dll"));
			Modules.Add(new ModuleViewModel("VideoModule.dll"));
			Modules.Add(new ModuleViewModel("DiagnosticsModule.dll"));
			Modules.Add(new ModuleViewModel("AlarmModule.dll"));
			Modules.Add(new ModuleViewModel("JournalModule.dll"));
			Modules.Add(new ModuleViewModel("ReportsModule.dll"));
			Modules.Add(new ModuleViewModel("SkudModule.dll"));
			Modules.Add(new ModuleViewModel("SKUDModule.dll"));
			Modules.Add(new ModuleViewModel("SKDModule.dll"));
			Modules.Add(new ModuleViewModel("LayoutModule.dll"));

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

		bool _isGKAsAService;
		public bool IsGKAsAService
		{
			get { return _isGKAsAService; }
			set
			{
				_isGKAsAService = value;
				OnPropertyChanged("IsGKAsAService");
			}
		}

		bool _useGKHash;
		public bool UseGKHash
		{
			get { return _useGKHash; }
			set
			{
				_useGKHash = value;
				OnPropertyChanged("UseGKHash");
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
			GlobalSettingsHelper.GlobalSettings.Login = Login;
			GlobalSettingsHelper.GlobalSettings.Password = Password;
			GlobalSettingsHelper.GlobalSettings.AutoConnect = AutoConnect;
			GlobalSettingsHelper.GlobalSettings.DoNotAutoconnectAdm = DoNotAutoconnectAdm;
			GlobalSettingsHelper.GlobalSettings.RunRevisor = RunRevisor;
			GlobalSettingsHelper.GlobalSettings.FS_Password = FS_Password;
			GlobalSettingsHelper.GlobalSettings.DoNotOverrideFS1 = DoNotOverrideFS1;
			GlobalSettingsHelper.GlobalSettings.FS_RemoteAddress = FS_RemoteAddress;
			GlobalSettingsHelper.GlobalSettings.FS_Port = FS_Port;
			GlobalSettingsHelper.GlobalSettings.FS_Login = FS_Login;
			GlobalSettingsHelper.GlobalSettings.FS_Password = FS_Password;

			GlobalSettingsHelper.GlobalSettings.IsGKAsAService = IsGKAsAService;
			GlobalSettingsHelper.GlobalSettings.UseGKHash = UseGKHash;

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