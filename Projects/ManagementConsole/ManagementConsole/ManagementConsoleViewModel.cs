using System;
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
		static string administratorLogsPath = AppDataFolderHelper.GetLogsFolder("Administrator");
		static string monitorLogsPath = AppDataFolderHelper.GetLogsFolder("Monitor");
		static string serverLogsPath = AppDataFolderHelper.GetLogsFolder("Server");
		static string fsAgentLogsPath = AppDataFolderHelper.GetLogsFolder("FSAgent");
		static string opcLogsPath = AppDataFolderHelper.GetLogsFolder("OPC");
		static string multiclientLogspath = AppDataFolderHelper.GetLogsFolder("Multiclient");
		static string configPath = AppDataFolderHelper.GetServerAppDataPath("Config.fscp");
		static string fspath = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"..\FiresecService\FiresecService.exe");
		static string agnpath = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"..\FSAgent\FSAgentServer.exe");
		static string opcpath = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"..\FiresecOPCServer\FiresecOPCServer.exe");

		public ManagementConsoleViewModel()
		{
			#region DEBUG PATHS
#if DEBUG
			fspath = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"..\..\..\..\FiresecService\bin\Debug\FiresecService.exe");
			agnpath = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"..\..\..\..\FSAgent\FSAgentServer\bin\Debug\FSAgentServer.exe");
			opcpath = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"..\..\..\..\FiresecOPCServer\FiresecOPCServer\bin\Debug\FiresecOPCServer.exe");
#endif
			#endregion
			GetLogsCommand = new RelayCommand(OnGetLogs);
			RemLogsCommand = new RelayCommand(OnRemLogs);
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
						readkey.SetValue("FiresecService", fspath);
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
						readkey.SetValue("FSAgentServer", agnpath);
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
						readkey.SetValue("FiresecOPCServer", opcpath);
					else
						readkey.DeleteValue("FiresecOPCServer");
					readkey.Close();
				}
				OnPropertyChanged("IsOpcServerAuto");
			}
		}

		static string rootlogspath = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"..\Logs");
		static string curlogspath
		{
			get
			{
				return rootlogspath + "\\Logs " + DateTime.Today.Date.ToShortDateString();
			}
		}

		public RelayCommand GetLogsCommand { get; private set; }
		public void OnGetLogs()
		{
			if (!OnSelectFolder())
				return;
			if (File.Exists(curlogspath + ".zip"))
				File.Delete(curlogspath + ".zip");
			var zip = new ZipFile(curlogspath + ".zip");
			var administratorDirectory = new DirectoryInfo(administratorLogsPath);
			var monitorDirectory = new DirectoryInfo(monitorLogsPath);
			var serverDirectory = new DirectoryInfo(serverLogsPath);
			var fsAgentDirectory = new DirectoryInfo(fsAgentLogsPath);
			var opcDirectory = new DirectoryInfo(opcLogsPath);
			var multiclientDirectory = new DirectoryInfo(multiclientLogspath);
			var configurationDirectory = new DirectoryInfo(configPath);
			if (administratorDirectory.Exists)
			{
				var admfiles = administratorDirectory.GetFiles();
				foreach (var file in admfiles)
					zip.AddFile(file.FullName, "FireAdministrator");
			}

			if (monitorDirectory.Exists)
			{
				var monfiles = monitorDirectory.GetFiles();
				foreach (var file in monfiles)
					zip.AddFile(file.FullName, "FireMonitor");
			}

			if (serverDirectory.Exists)
			{
				var srvfiles = serverDirectory.GetFiles();
				foreach (var file in srvfiles)
					zip.AddFile(file.FullName, "FiresecService");
			}

			if (fsAgentDirectory.Exists)
			{
				var agnfiles = fsAgentDirectory.GetFiles();
				foreach (var file in agnfiles)
					zip.AddFile(file.FullName, "FSAgentServer");
			}

			if (opcDirectory.Exists)
			{
				var opcfiles = opcDirectory.GetFiles();
				foreach (var file in opcfiles)
					zip.AddFile(file.FullName, "FiresecOPCServer");
			}

			if (multiclientDirectory.Exists)
			{
				var opcfiles = multiclientDirectory.GetFiles();
				foreach (var file in opcfiles)
					zip.AddFile(file.FullName, "Multiclient");
			}

			if (configurationDirectory.Exists)
			{
				var srvfiles = configurationDirectory.GetFiles();
				foreach (var file in srvfiles)
					zip.AddFile(file.FullName, "Configuration");
			}

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
			zip.AddEntry("systeminfo.txt", memoryStream);
			Mouse.OverrideCursor = Cursors.Wait;
			zip.Save();
			Mouse.OverrideCursor = Cursors.Arrow;
		}
		private static int GetBitCount(bool is64)
		{
			return is64 ? 64 : 86;
		}
		public RelayCommand RemLogsCommand { get; private set; }
		public void OnRemLogs()
		{
			if (Directory.Exists(administratorLogsPath))
				try { Directory.Delete(administratorLogsPath, true); }
				catch { }
			if (Directory.Exists(monitorLogsPath))
				try { Directory.Delete(monitorLogsPath, true); }
				catch { }
			if (Directory.Exists(serverLogsPath))
				try { Directory.Delete(serverLogsPath, true); }
				catch { }
			if (Directory.Exists(opcLogsPath))
				try { Directory.Delete(opcLogsPath, true); }
				catch { }
			if (Directory.Exists(multiclientLogspath))
				try { Directory.Delete(multiclientLogspath, true); }
				catch { }
			if (Directory.Exists(fsAgentLogsPath))
				try { Directory.Delete(fsAgentLogsPath, true); }
				catch { }
		}

		static bool OnSelectFolder()
		{
			try
			{
				var folder = new FolderBrowserDialog { Description = "Choose a Folder" };
				if (folder.ShowDialog() == DialogResult.OK)
				{
					rootlogspath = folder.SelectedPath;
					return true;
				}
				return false;
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "Ошибка при выполнении операции");
				return false;
			}
		}
	}
}