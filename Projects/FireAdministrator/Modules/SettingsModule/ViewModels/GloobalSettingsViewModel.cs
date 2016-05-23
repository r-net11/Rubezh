using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using StrazhAPI;
using FiresecClient;
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

		public AppServerViewModel AppServerViewModel { get; private set; }

		public GloobalSettingsViewModel()
		{
			Title = "Параметры";
			Curent = this;

			SaveLogsCommand = new RelayCommand(OnSaveLogs);
			RemoveLogsCommand = new RelayCommand(OnRemoveLogs);
			ResetDatabaseCommand = new RelayCommand(OnResetDatabase);
			ResetConfigurationCommand = new RelayCommand(OnResetConfiguration);
			ResetSKDLibaryCommand = new RelayCommand(OnResetSKDLibary);
			ResetSettingsCommand = new RelayCommand(OnResetSettings);

			AppServerViewModel = new AppServerViewModel();

			LogsFolderPath = AppDataFolderHelper.GetLogsFolder();
		}

		public string ServerAutoLabel { get { return "Сервер приложений"; } }
		public bool IsServerAuto
		{
			get
			{
				var registryKey = Registry.CurrentUser.OpenSubKey(@"software\Microsoft\Windows\CurrentVersion\Run");
				if (registryKey != null)
				{
					if (registryKey.GetValue("StrazhService") == null)
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
						var path = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"..\StrazhService\StrazhService.exe");
						registryKey.SetValue("StrazhService", path);
					}
					else
						registryKey.DeleteValue("StrazhService");
					registryKey.Close();
				}
				OnPropertyChanged(() => IsServerAuto);
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
			zipFile.Save();
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
				var result = FiresecManager.FiresecService.ResetSKDDatabase();
				if (result.HasError)
					MessageBoxService.Show(result.Error);
			}
		}

		public RelayCommand ResetConfigurationCommand { get; private set; }
		void OnResetConfiguration()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите сбросить по конфигурацию?"))
			{
				File.Copy(AppDataFolderHelper.GetFileInFolder("Empty", "Config.fscp"), AppDataFolderHelper.GetFileInFolder("Server", "Config.fscp"), true);
			}
		}

		public RelayCommand ResetSKDLibaryCommand { get; private set; }
		void OnResetSKDLibary()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите сбросить по умолчанию настройки библиотеки устройств СКД?"))
			{
				DeviceLibraryConfigurationPatchHelper.PatchSKDLibrary();
			}
		}

		public RelayCommand ResetSettingsCommand { get; private set; }
		void OnResetSettings()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите сбросить по умолчанию настройки?"))
			{
				GlobalSettingsHelper.Reset();
				AppServerSettingsHelper.Reset();
				AppServerViewModel = new AppServerViewModel();
				OnPropertyChanged(() => AppServerViewModel);
				OnPropertyChanged(() => GlobalSettings);
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
			AppServerSettingsHelper.Load();
			GlobalSettingsHelper.Load();
			return false;
		}

		protected override bool Save()
		{
			AppServerViewModel.Save();
			AppServerSettingsHelper.Save();
			GlobalSettingsHelper.Save();
			return true;
		}
	}
}