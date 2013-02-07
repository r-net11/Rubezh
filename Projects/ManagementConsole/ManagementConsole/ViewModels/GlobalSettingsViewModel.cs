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
			RemoteFSAgentPort = GlobalSettingsHelper.GlobalSettings.RemoteFSAgentPort;
			AutoConnect = GlobalSettingsHelper.GlobalSettings.AutoConnect;
			DoNotOverrideFS1 = GlobalSettingsHelper.GlobalSettings.DoNotOverrideFS1;
			LibVlcDllsPath = GlobalSettingsHelper.GlobalSettings.LibVlcDllsPath;
			Modules = GlobalSettingsHelper.GlobalSettings.Modules;
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

		int _remoteFSAgentPort;
		public int RemoteFSAgentPort
		{
			get { return _remoteFSAgentPort; }
			set
			{
				_remoteFSAgentPort = value;
				OnPropertyChanged("RemoteFSAgentPort");
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

		public RelayCommand SaveCommand { get; private set; }
		void OnSave()
		{
			GlobalSettingsHelper.GlobalSettings.RemoteAddress = RemoteAddress;
			GlobalSettingsHelper.GlobalSettings.RemotePort = RemotePort;
			GlobalSettingsHelper.GlobalSettings.RemoteFSAgentPort = RemoteFSAgentPort;
			GlobalSettingsHelper.GlobalSettings.AutoConnect = AutoConnect;
			GlobalSettingsHelper.GlobalSettings.DoNotOverrideFS1 = DoNotOverrideFS1;
			GlobalSettingsHelper.GlobalSettings.LibVlcDllsPath = LibVlcDllsPath;
			GlobalSettingsHelper.GlobalSettings.Modules = Modules;
			GlobalSettingsHelper.Save();			
		}
	}
}