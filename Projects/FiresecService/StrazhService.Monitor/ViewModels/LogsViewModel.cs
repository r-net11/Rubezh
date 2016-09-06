using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Common;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Ionic.Zip;
using Localization.StrazhService.Monitor.ViewModels;
using StrazhService.Monitor.Events;

namespace StrazhService.Monitor.ViewModels
{
	public class LogsViewModel : BaseViewModel
	{
		private string _log;

		public string Log
		{
			get { return _log; }
			set
			{
				if (_log == value)
					return;
				_log = value;
				OnPropertyChanged(() => Log);
			}
		}

		public RelayCommand SaveLogsAndConfigsCommand { get; private set; }

		public LogsViewModel()
		{
			ServiceRepository.Instance.Events.GetEvent<ServerLogsReceivedEvent>().Unsubscribe(OnServerLogsReceivedEvent);
			ServiceRepository.Instance.Events.GetEvent<ServerLogsReceivedEvent>().Subscribe(OnServerLogsReceivedEvent);
			SaveLogsAndConfigsCommand = new RelayCommand(OnSaveLogsAndConfigs);
		}

		private void OnServerLogsReceivedEvent(string logs)
		{
			Log = logs;
		}

		private void OnSaveLogsAndConfigs()
		{
			var saveFolderPath = new FolderBrowserDialog { Description = "Choose a Folder" };
			if (saveFolderPath.ShowDialog() != DialogResult.OK)
			{
				return;
			}

			Logger.Info(string.Format("Сохранить логи и конфигурацию в папке '{0}'", saveFolderPath.SelectedPath));

			try
			{
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

				var msg = CommonViewModels.SaveLogOperationWithoutError;
				Logger.Info(msg);
				MessageBoxService.Show(msg);
			}
			catch (Exception e)
			{
				var msg = string.Format(CommonViewModels.SaveLogOperationWithError, e);
				Logger.Warn(msg);
				MessageBoxService.ShowWarning(msg);
			}
		}

		private static int GetBitCount(bool is64)
		{
			return is64 ? 64 : 86;
		}
	}
}