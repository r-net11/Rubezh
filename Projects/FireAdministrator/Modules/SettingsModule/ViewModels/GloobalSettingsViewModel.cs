using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using FiresecAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Ionic.Zip;
using Microsoft.Win32;

namespace SettingsModule.ViewModels
{
	public class GloobalSettingsViewModel : SaveCancelDialogViewModel
	{
		public static GloobalSettingsViewModel Curent { get; private set; }
		public ModulesViewModel ModulesViewModel { get; private set; }

		public GloobalSettingsViewModel()
		{
			Title = "Настройки";
			Curent = this;
			SaveLogsCommand = new RelayCommand(OnSaveLogs);
			RemoveLogsCommand = new RelayCommand(OnRemoveLogs);
			ResetDatabaseCommand = new RelayCommand(OnResetDatabase);
			ResetConfigurationCommand = new RelayCommand(OnResetConfiguration);
			ResetXLibaryCommand = new RelayCommand(OnResetXLibary);
			ResetSKDLibaryCommand = new RelayCommand(OnResetSKDLibary);
			ResetSettingsCommand = new RelayCommand(OnResetSettings);
			ModulesViewModel = new ModulesViewModel();
			LogsFolderPath = AppDataFolderHelper.GetLogsFolder();
		}

		public bool IsServerAuto
		{
			get
			{
				var registryKey = Registry.CurrentUser.OpenSubKey(@"software\Microsoft\Windows\CurrentVersion\Run");
				if (registryKey != null)
				{
					if (registryKey.GetValue("FiresecService") == null)
						return false;
					registryKey.Close();
				}
				return true;
			}
			set
			{
				var registryKey = Registry.CurrentUser.CreateSubKey(@"software\Microsoft\Windows\CurrentVersion\Run");
				if (registryKey != null)
				{
					if (value)
					{
						var path = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"..\FiresecService\FiresecService.exe");
						registryKey.SetValue("FiresecService", path);
					}
					else
						registryKey.DeleteValue("FiresecService");
					registryKey.Close();
				}
				OnPropertyChanged("IsServerAuto");
			}
		}

		public bool IsFsAgentAuto
		{
			get
			{
				var registryKey = Registry.CurrentUser.OpenSubKey(@"software\Microsoft\Windows\CurrentVersion\Run");
				if (registryKey != null)
				{
					if (registryKey.GetValue("FSAgentServer") == null)
						return false;
					registryKey.Close();
				}
				return true;
			}
			set
			{
				var registryKey = Registry.CurrentUser.CreateSubKey(@"software\Microsoft\Windows\CurrentVersion\Run");
				if (registryKey != null)
				{
					if (value)
					{
						var path = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"..\FSAgent\FSAgentServer.exe");
						registryKey.SetValue("FSAgentServer", path);
					}
					else
						registryKey.DeleteValue("FSAgentServer");
					registryKey.Close();
				}
				OnPropertyChanged("IsFSAgentAuto");
			}
		}

		public bool IsOpcServerAuto
		{
			get
			{
				var registryKey = Registry.CurrentUser.OpenSubKey(@"software\Microsoft\Windows\CurrentVersion\Run");
				if (registryKey != null)
				{
					if (registryKey.GetValue("FiresecOPCServer") == null)
						return false;
					registryKey.Close();
				}
				return true;
			}
			set
			{
				var registryKey = Registry.CurrentUser.CreateSubKey(@"software\Microsoft\Windows\CurrentVersion\Run");
				if (registryKey != null)
				{
					if (value)
					{
						var path = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"..\OPC\FiresecOPCServer.exe");
						registryKey.SetValue("FiresecOPCServer", path);
					}
					else
						registryKey.DeleteValue("FiresecOPCServer");
					registryKey.Close();
				}
				OnPropertyChanged("IsOpcServerAuto");
			}
		}

		public bool IsGKOpcServerAuto
		{
			get
			{
				var registryKey = Registry.CurrentUser.OpenSubKey(@"software\Microsoft\Windows\CurrentVersion\Run");
				if (registryKey != null)
				{
					if (registryKey.GetValue("GKOPCServer") == null)
						return false;
					registryKey.Close();
				}
				return true;
			}
			set
			{
				var registryKey = Registry.CurrentUser.CreateSubKey(@"software\Microsoft\Windows\CurrentVersion\Run");
				if (registryKey != null)
				{
					if (value)
					{
						var path = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"..\GKOPC\GKOPCServer.exe");
						registryKey.SetValue("GKOPCServer", path);
					}
					else
						registryKey.DeleteValue("GKOPCServer");
					registryKey.Close();
				}
				OnPropertyChanged("IsGKOpcServerAuto");
			}
		}

		public string LogsFolderPath { get; private set; }

		public RelayCommand SaveLogsCommand { get; private set; }
		public void OnSaveLogs()
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

			WaitHelper.Execute(() =>
			{
				zipFile.Save();
			});
		}

		static int GetBitCount(bool is64)
		{
			return is64 ? 64 : 86;
		}

		public RelayCommand RemoveLogsCommand { get; private set; }
		public void OnRemoveLogs()
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
		public void OnResetDatabase()
		{
			var result = MessageBoxService.Show("Вы уверены, что хотите сбросить базу данных?");
			if (result == MessageBoxResult.OK)
			{
				File.Copy(AppDataFolderHelper.GetFileInFolder("Empty", "Firesec.sdf"), AppDataFolderHelper.GetFileInFolder("DB", "Firesec.sdf"), true);
				File.Copy(AppDataFolderHelper.GetFileInFolder("Empty", "FSDB.sdf"), AppDataFolderHelper.GetFileInFolder("DB", "FSDB.sdf"), true);
				File.Copy(AppDataFolderHelper.GetFileInFolder("Empty", "GkJournalDatabase.sdf"), AppDataFolderHelper.GetFileInFolder("DB", "GkJournalDatabase.sdf"), true);
			}
		}

		public RelayCommand ResetConfigurationCommand { get; private set; }
		public void OnResetConfiguration()
		{
			var result = MessageBoxService.Show("Вы уверены, что хотите сбросить по конфигурацию?");
			if (result == MessageBoxResult.OK)
			{
				File.Copy(AppDataFolderHelper.GetFileInFolder("Empty", "Config.fscp"), AppDataFolderHelper.GetFileInFolder("Server", "Config.fscp"), true);
			}
		}

		public RelayCommand ResetXLibaryCommand { get; private set; }
		public void OnResetXLibary()
		{
			var result = MessageBoxService.Show("Вы уверены, что хотите сбросить по умолчанию настройки библиотеки устройств?");
			if (result == MessageBoxResult.OK)
			{
				XDeviceLibraryConfigurationPatchHelper.Patch();
			}
		}

		public RelayCommand ResetSKDLibaryCommand { get; private set; }
		public void OnResetSKDLibary()
		{
			var result = MessageBoxService.Show("Вы уверены, что хотите сбросить по умолчанию настройки библиотеки устройств СКД?");
			if (result == MessageBoxResult.OK)
			{
				XDeviceLibraryConfigurationPatchHelper.PatchSKDLibrary();
			}
		}

		public RelayCommand ResetSettingsCommand { get; private set; }
		public void OnResetSettings()
		{
			var result = MessageBoxService.Show("Вы уверены, что хотите сбросить по умолчанию настройки?");
			if (result == MessageBoxResult.OK)
			{
				GlobalSettingsHelper.GlobalSettings = new GlobalSettings();
				GlobalSettingsHelper.Save();
				ModulesViewModel = new ModulesViewModel();
				OnPropertyChanged("GlobalSettingsViewModel");
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

		public GlobalSettings GlobalSettings
		{
			get { return GlobalSettingsHelper.GlobalSettings; }
		}

		protected override bool Cancel()
		{
			GlobalSettingsHelper.Load();
			return false;
		}

		protected override bool Save()
		{
			ModulesViewModel.Save();
			GlobalSettingsHelper.Save();
			return true;
		}
	}
}