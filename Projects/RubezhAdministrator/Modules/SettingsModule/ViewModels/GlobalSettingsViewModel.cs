using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using RubezhClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Ionic.Zip;
using Microsoft.Win32;

namespace SettingsModule.ViewModels
{
	public class GlobalSettingsViewModel : SaveCancelDialogViewModel
	{
		public static GlobalSettingsViewModel Curent { get; private set; }
		public ModulesViewModel ModulesViewModel { get; private set; }
		public DbSettingsViewModel DbSettingsViewModel { get; private set; }

		public GlobalSettingsViewModel()
		{
			Title = "Параметры";
			Curent = this;
			SaveLogsCommand = new RelayCommand(OnSaveLogs);
			RemoveLogsCommand = new RelayCommand(OnRemoveLogs);
			ResetDatabaseCommand = new RelayCommand(OnResetDatabase);
			ResetConfigurationCommand = new RelayCommand(OnResetConfiguration);
			ResetSettingsCommand = new RelayCommand(OnResetSettings);
			ModulesViewModel = new ModulesViewModel();
			DbSettingsViewModel = new DbSettingsViewModel();
			LogsFolderPath = AppDataFolderHelper.GetLogsFolder();

			GetServerAuto();
			GetGKOpcServerAuto();
			Monitor_F1_Enabled = GlobalSettingsHelper.GlobalSettings.Monitor_F1_Enabled;
			Monitor_F2_Enabled = GlobalSettingsHelper.GlobalSettings.Monitor_F2_Enabled;
			Monitor_F3_Enabled = GlobalSettingsHelper.GlobalSettings.Monitor_F3_Enabled;
			Monitor_F4_Enabled = GlobalSettingsHelper.GlobalSettings.Monitor_F4_Enabled;

			RemoteAddress = GlobalSettingsHelper.GlobalSettings.RemoteAddress;
			RemotePort = GlobalSettingsHelper.GlobalSettings.RemotePort;
			ReportRemotePort = GlobalSettingsHelper.GlobalSettings.ReportRemotePort;
			AdministratorLogin = GlobalSettingsHelper.GlobalSettings.AdminLogin;
			AdministratorPassword = GlobalSettingsHelper.GlobalSettings.AdminPassword;
			AdministratorAutoConnect = GlobalSettingsHelper.GlobalSettings.AdminAutoConnect;
			MonitorLogin = GlobalSettingsHelper.GlobalSettings.MonitorLogin;
			MonitorPassword = GlobalSettingsHelper.GlobalSettings.MonitorPassword;
			WebLogin = GlobalSettingsHelper.GlobalSettings.WebLogin;
			WebPassword = GlobalSettingsHelper.GlobalSettings.WebPassword;
			MonitorAutoConnect = GlobalSettingsHelper.GlobalSettings.MonitorAutoConnect;
			RunRevisor = GlobalSettingsHelper.GlobalSettings.RunRevisor;
			Server_EnableRemoteConnections = GlobalSettingsHelper.GlobalSettings.Server_EnableRemoteConnections;
			Server_RemoteIpAddress = GlobalSettingsHelper.GlobalSettings.Server_RemoteIpAddress;
		}

		void GetServerAuto()
		{
			using (var registryKey = Registry.CurrentUser.OpenSubKey(@"software\Microsoft\Windows\CurrentVersion\Run"))
			{
				IsServerAuto = registryKey != null && registryKey.GetValue("RubezhService") != null;
			}
		}

		void SetServerAuto()
		{
			using (var registryKey = Registry.CurrentUser.CreateSubKey(@"software\Microsoft\Windows\CurrentVersion\Run"))
			{
				if (registryKey != null)
				{
					if (IsServerAuto)
					{
						var entryAssemblyLocation = Assembly.GetEntryAssembly().Location;
						var rubezhDirectoryPath = Path.GetDirectoryName(Path.GetDirectoryName(entryAssemblyLocation));
						var path = Path.GetFullPath(rubezhDirectoryPath + @"..\RubezhService\RubezhService.exe");
						registryKey.SetValue("RubezhService", path);
					}
					else if (registryKey.GetValue("RubezhService") != null)
						registryKey.DeleteValue("RubezhService");
				}
			}
		}

		void GetGKOpcServerAuto()
		{
			using (var registryKey = Registry.CurrentUser.OpenSubKey(@"software\Microsoft\Windows\CurrentVersion\Run"))
			{
				IsGKOpcServerAuto = registryKey != null && registryKey.GetValue("GKOPCServer") != null;
			}
		}

		void SetGKOpcServerAuto()
		{
			using (var registryKey = Registry.CurrentUser.CreateSubKey(@"software\Microsoft\Windows\CurrentVersion\Run"))
			{
				if (registryKey != null)
				{
					if (IsGKOpcServerAuto)
					{
						var path = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"..\GKOPC\GKOPCServer.exe");
						registryKey.SetValue("GKOPCServer", path);
					}
					else if (registryKey.GetValue("GKOPCServer") != null)
						registryKey.DeleteValue("GKOPCServer");
				}
			}
		}

		public string LogsFolderPath { get; private set; }

		public RelayCommand SaveLogsCommand { get; private set; }
		void OnSaveLogs()
		{
			var saveFolderPath = new FolderBrowserDialog { Description = "Choose a Folder" };
			if (saveFolderPath.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			var saveLogsPath = Path.Combine(saveFolderPath.SelectedPath, "Logs.zip");

			if (File.Exists(saveLogsPath))
				File.Delete(saveLogsPath);
			var zipFile = new ZipFile(saveLogsPath);

			var logsFolderPath = AppDataFolderHelper.GetLogsFolder();
			var logsDirectory = new DirectoryInfo(logsFolderPath);
			if (logsDirectory.Exists)
			{
				foreach (var directoryInfo in logsDirectory.GetDirectories())
				{
					var fileInfo = directoryInfo.GetFiles();
					foreach (var file in fileInfo)
						zipFile.AddFile(file.FullName, directoryInfo.Name);
				}
			}

			var configPath = AppDataFolderHelper.GetServerAppDataPath("Config.fscp");
			var configFileInfo = new FileInfo(configPath);
			zipFile.AddFile(configFileInfo.FullName, "Config");

			var memoryStream = new MemoryStream();
			var streamWriter = new StreamWriter(memoryStream);
			streamWriter.WriteLine("System information");
			try
			{
				var process = Process.GetCurrentProcess();
				streamWriter.WriteLine("Process [{0}]:	{1} x{2}", process.Id, process.ProcessName, GetBitCount(Environment.Is64BitProcess));
				streamWriter.WriteLine("Operation System:  {0} {1} Bit Operating System", Environment.OSVersion, GetBitCount(Environment.Is64BitOperatingSystem));
				streamWriter.WriteLine("ComputerName:	  {0}", Environment.MachineName);
				streamWriter.WriteLine("UserDomainName:	{0}", Environment.UserDomainName);
				streamWriter.WriteLine("UserName:		  {0}", Environment.UserName);
				streamWriter.WriteLine("Base Directory:	{0}", Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
				streamWriter.WriteLine("SystemDirectory:   {0}", Environment.SystemDirectory);
				streamWriter.WriteLine("ProcessorCount:	{0}", Environment.ProcessorCount);
				streamWriter.WriteLine("SystemPageSize:	{0}", Environment.SystemPageSize);
				streamWriter.WriteLine(".Net Framework:	{0}", Environment.Version);
			}
			catch (Exception ex)
			{
				streamWriter.WriteLine(ex.ToString());
			}
			streamWriter.Flush();
			memoryStream.Position = 0;
			zipFile.AddEntry("systeminfo.txt", memoryStream);

			WaitHelper.Execute(zipFile.Save);
		}

		static int GetBitCount(bool is64)
		{
			return is64 ? 64 : 86;
		}

		public RelayCommand RemoveLogsCommand { get; private set; }
		void OnRemoveLogs()
		{
			foreach (var directoryName in Directory.GetDirectories(LogsFolderPath))
			{
				foreach (var fileName in Directory.GetFiles(directoryName))
				{
					File.Delete(fileName);
				}
			}
		}

		public RelayCommand ResetDatabaseCommand { get; private set; }
		void OnResetDatabase()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите сбросить базу данных?"))
			{
				var result = ClientManager.RubezhService.ResetDB();
				if (result.HasError)
					MessageBoxService.Show(result.Error);
			}
		}

		public RelayCommand ResetConfigurationCommand { get; private set; }
		void OnResetConfiguration()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите сбросить конфигурацию?"))
			{
				File.Copy(AppDataFolderHelper.GetFileInFolder("Empty", "Config.fscp"), AppDataFolderHelper.GetFileInFolder("Server", "Config.fscp"), true);
			}
		}

		public RelayCommand ResetSettingsCommand { get; private set; }
		void OnResetSettings()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите сбросить по умолчанию настройки?"))
			{
				GlobalSettingsHelper.Reset();
				ModulesViewModel = new ModulesViewModel();
				OnPropertyChanged(() => ModulesViewModel);
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
		bool _monitor_F1_Enabled;
		public bool Monitor_F1_Enabled
		{
			get { return _monitor_F1_Enabled; }
			set
			{
				_monitor_F1_Enabled = value;
				OnPropertyChanged(() => Monitor_F1_Enabled);
			}
		}

		bool _monitor_F2_Enabled;
		public bool Monitor_F2_Enabled
		{
			get { return _monitor_F2_Enabled; }
			set
			{
				_monitor_F2_Enabled = value;
				OnPropertyChanged(() => Monitor_F2_Enabled);
			}
		}

		bool _monitor_F3_Enabled;
		public bool Monitor_F3_Enabled
		{
			get { return _monitor_F3_Enabled; }
			set
			{
				_monitor_F3_Enabled = value;
				OnPropertyChanged(() => Monitor_F3_Enabled);
			}
		}

		bool _monitor_F4_Enabled;
		public bool Monitor_F4_Enabled
		{
			get { return _monitor_F4_Enabled; }
			set
			{
				_monitor_F4_Enabled = value;
				OnPropertyChanged(() => Monitor_F4_Enabled);
			}
		}

		bool _isServerAuto;
		public bool IsServerAuto
		{
			get { return _isServerAuto; }
			set
			{
				_isServerAuto = value;
				OnPropertyChanged(() => IsServerAuto);
			}
		}

		bool _server_EnableRemoteConnections;
		public bool Server_EnableRemoteConnections
		{
			get { return _server_EnableRemoteConnections; }
			set
			{
				_server_EnableRemoteConnections = value;
				OnPropertyChanged(() => Server_EnableRemoteConnections);
			}
		}

		string _Server_RemoteIpAddress;
		public string Server_RemoteIpAddress
		{
			get { return _Server_RemoteIpAddress; }
			set
			{
				_Server_RemoteIpAddress = value;
				OnPropertyChanged(() => Server_RemoteIpAddress);
			}
		}

		bool _isGKOpcServerAuto;
		public bool IsGKOpcServerAuto
		{
			get { return _isGKOpcServerAuto; }
			set
			{
				_isGKOpcServerAuto = value;
				OnPropertyChanged(() => IsGKOpcServerAuto);
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

		string _monitorLogin;
		public string MonitorLogin
		{
			get { return _monitorLogin; }
			set
			{
				_monitorLogin = value;
				OnPropertyChanged(() => MonitorLogin);
			}
		}


		string _webLogin;
		public string WebLogin {
			get { return _webLogin; }
			set
			{
				_webLogin = value;
				OnPropertyChanged(() => WebLogin);
			}
		}

		string _webPassword;
		public string WebPassword
		{
			get { return _webPassword; }
			set
			{
				_webPassword = value;
				OnPropertyChanged(() => WebPassword);
			}
		}


		string _administratorLogin;
		public string AdministratorLogin
		{
			get { return _administratorLogin; }
			set
			{
				_administratorLogin = value;
				OnPropertyChanged(() => AdministratorLogin);
			}
		}

		string _administratorPassword;
		public string AdministratorPassword
		{
			get { return _administratorPassword; }
			set
			{
				_administratorPassword = value;
				OnPropertyChanged(() => AdministratorPassword);
			}
		}
		string _monitorPassword;
		public string MonitorPassword
		{
			get { return _monitorPassword; }
			set
			{
				_monitorPassword = value;
				OnPropertyChanged(() => MonitorPassword);
			}
		}

		bool _administratorAutoConnect;
		public bool AdministratorAutoConnect
		{
			get { return _administratorAutoConnect; }
			set
			{
				_administratorAutoConnect = value;
				OnPropertyChanged(() => AdministratorAutoConnect);
			}
		}

		bool _monitorAutoConnect;
		public bool MonitorAutoConnect
		{
			get { return _monitorAutoConnect; }
			set
			{
				_monitorAutoConnect = value;
				OnPropertyChanged(() => MonitorAutoConnect);
			}
		}


		bool _runRevisor;
		public bool RunRevisor
		{
			get { return _runRevisor; }
			set
			{
				_runRevisor = value;
				OnPropertyChanged(() => RunRevisor);
			}
		}

		protected override bool Cancel()
		{
			GlobalSettingsHelper.Load();
			return false;
		}

		protected override bool Save()
		{
			SetServerAuto();
			SetGKOpcServerAuto();
			GlobalSettingsHelper.GlobalSettings.Monitor_F1_Enabled = Monitor_F1_Enabled;
			GlobalSettingsHelper.GlobalSettings.Monitor_F2_Enabled = Monitor_F2_Enabled;
			GlobalSettingsHelper.GlobalSettings.Monitor_F3_Enabled = Monitor_F3_Enabled;
			GlobalSettingsHelper.GlobalSettings.Monitor_F4_Enabled = Monitor_F4_Enabled;
			GlobalSettingsHelper.GlobalSettings.RemoteAddress = RemoteAddress;
			GlobalSettingsHelper.GlobalSettings.RemotePort = RemotePort;
			GlobalSettingsHelper.GlobalSettings.ReportRemotePort = ReportRemotePort;
			GlobalSettingsHelper.GlobalSettings.AdminLogin = AdministratorLogin;
			GlobalSettingsHelper.GlobalSettings.AdminPassword = AdministratorPassword;
			GlobalSettingsHelper.GlobalSettings.AdminAutoConnect = AdministratorAutoConnect;
			GlobalSettingsHelper.GlobalSettings.MonitorLogin = MonitorLogin;
			GlobalSettingsHelper.GlobalSettings.MonitorPassword = MonitorPassword;
			GlobalSettingsHelper.GlobalSettings.WebLogin = WebLogin;
			GlobalSettingsHelper.GlobalSettings.WebPassword = WebPassword;
			GlobalSettingsHelper.GlobalSettings.MonitorAutoConnect = MonitorAutoConnect;
			GlobalSettingsHelper.GlobalSettings.Server_EnableRemoteConnections = Server_EnableRemoteConnections;
			GlobalSettingsHelper.GlobalSettings.Server_RemoteIpAddress = Server_RemoteIpAddress;
			GlobalSettingsHelper.GlobalSettings.RunRevisor = RunRevisor;
			ModulesViewModel.Save();
			DbSettingsViewModel.Save();
			GlobalSettingsHelper.Save();
			return true;
		}
	}
}