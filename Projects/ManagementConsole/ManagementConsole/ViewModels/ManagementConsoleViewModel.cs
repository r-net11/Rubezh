﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Ionic.Zip;
using Microsoft.Win32;
using Cursor = System.Windows.Input.Cursor;
using Cursors = System.Windows.Input.Cursors;
using MessageBox = System.Windows.MessageBox;

namespace ManagementConsole
{
	public class ManagementConsoleViewModel : BaseViewModel
	{
		public GlobalSettingsViewModel GlobalSettingsViewModel { get; private set; }

		public ManagementConsoleViewModel()
		{
			GetLogsCommand = new RelayCommand(OnGetLogs);
			RemLogsCommand = new RelayCommand(OnRemLogs);
			GlobalSettingsViewModel = new GlobalSettingsViewModel();
			LogsFolderPath = AppDataFolderHelper.GetLogsFolder();
		}

		public bool IsServerAuto
		{
			get
			{
				var readkey = Registry.CurrentUser.OpenSubKey(@"software\Microsoft\Windows\CurrentVersion\Run");
				if (readkey != null)
				{
					if (readkey.GetValue("FiresecService") == null)
						return false;
					readkey.Close();
				}
				return true;
			}
			set
			{
				var readkey = Registry.CurrentUser.CreateSubKey(@"software\Microsoft\Windows\CurrentVersion\Run");
				if (readkey != null)
				{
					if (value)
					{
						var path = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"..\FiresecService\FiresecService.exe");
						readkey.SetValue("FiresecService", path);
					}
					else
						readkey.DeleteValue("FiresecService");
					readkey.Close();
				}
				OnPropertyChanged("IsServerAuto");
			}
		}

		public bool IsFsAgentAuto
		{
			get
			{
				var readkey = Registry.CurrentUser.OpenSubKey(@"software\Microsoft\Windows\CurrentVersion\Run");
				if (readkey != null)
				{
					if (readkey.GetValue("FSAgentServer") == null)
						return false;
					readkey.Close();
				}
				return true;
			}
			set
			{
				var readkey = Registry.CurrentUser.CreateSubKey(@"software\Microsoft\Windows\CurrentVersion\Run");
				if (readkey != null)
				{
					if (value)
					{
						var path = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"..\FSAgent\FSAgentServer.exe");
						readkey.SetValue("FSAgentServer", path);
					}
					else
						readkey.DeleteValue("FSAgentServer");
					readkey.Close();
				}
				OnPropertyChanged("IsFSAgentAuto");
			}
		}

		public bool IsOpcServerAuto
		{
			get
			{
				var readkey = Registry.CurrentUser.OpenSubKey(@"software\Microsoft\Windows\CurrentVersion\Run");
				if (readkey != null)
				{
					if (readkey.GetValue("FiresecOPCServer") == null)
						return false;
					readkey.Close();
				}
				return true;
			}
			set
			{
				var readkey = Registry.CurrentUser.CreateSubKey(@"software\Microsoft\Windows\CurrentVersion\Run");
				if (readkey != null)
				{
					if (value)
					{
						var path = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"..\FiresecOPCServer\FiresecOPCServer.exe");
						readkey.SetValue("FiresecOPCServer", path);
					}
					else
						readkey.DeleteValue("FiresecOPCServer");
					readkey.Close();
				}
				OnPropertyChanged("IsOpcServerAuto");
			}
		}

		public string LogsFolderPath { get; private set; }

		public RelayCommand GetLogsCommand { get; private set; }
		public void OnGetLogs()
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
				streamWriter.WriteLine("Process [{0}]:    {1} x{2}", process.Id, process.ProcessName, GetBitCount(Environment.Is64BitProcess));
				streamWriter.WriteLine("Operation System:  {0} {1} Bit Operating System", Environment.OSVersion, GetBitCount(Environment.Is64BitOperatingSystem));
				streamWriter.WriteLine("ComputerName:      {0}", Environment.MachineName);
				streamWriter.WriteLine("UserDomainName:    {0}", Environment.UserDomainName);
				streamWriter.WriteLine("UserName:          {0}", Environment.UserName);
				streamWriter.WriteLine("Base Directory:    {0}", Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
				streamWriter.WriteLine("SystemDirectory:   {0}", Environment.SystemDirectory);
				streamWriter.WriteLine("ProcessorCount:    {0}", Environment.ProcessorCount);
				streamWriter.WriteLine("SystemPageSize:    {0}", Environment.SystemPageSize);
				streamWriter.WriteLine(".Net Framework:    {0}", Environment.Version);
			}
			catch (Exception ex)
			{
				streamWriter.WriteLine(ex.ToString());
			}
			streamWriter.Flush();
			memoryStream.Position = 0;
			zipFile.AddEntry("systeminfo.txt", memoryStream);

			Mouse.OverrideCursor = Cursors.Wait;
			zipFile.Save();
			Mouse.OverrideCursor = Cursors.Arrow;
		}

		static int GetBitCount(bool is64)
		{
			return is64 ? 64 : 86;
		}

		public RelayCommand RemLogsCommand { get; private set; }
		public void OnRemLogs()
		{
			foreach (var directoryName in Directory.GetDirectories(LogsFolderPath))
			{
				foreach (var fileName in Directory.GetFiles(directoryName))
				{
					File.Delete(fileName);
				}
			}
		}
	}
}