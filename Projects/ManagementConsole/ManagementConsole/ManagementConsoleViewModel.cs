using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Ionic.Zip;
using Microsoft.Win32;
using MessageBox = System.Windows.MessageBox;

namespace ManagementConsole
{
	public class ManagementConsoleViewModel : BaseViewModel
    {
        #region RELEASE PATHS
        static string admlogspath = @"..\FireAdministrator\Logs";
        static string monlogspath = @"..\FireMonitor\Logs";
        static string srvlogspath = @"..\FiresecService\Logs";
        static string agnlogspath = @"..\FSAgent\Logs";
        static string opclogspath = @"..\FiresecOPCServer\Logs";
        static string srvconfpath = @"..\FiresecService\Configuration";
        static string fspath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"..\FiresecService\FiresecService.exe");
        static string agnpath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"..\FSAgent\FSAgentServer.exe");
        static string opcpath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"..\FiresecOPCServer\FiresecOPCServer.exe");
        #endregion
        public ManagementConsoleViewModel()
		{
            #region DEBUG PATHS
#if DEBUG

            admlogspath = @"..\..\..\..\FireAdministrator\bin\Debug\Logs";
            monlogspath = @"..\..\..\..\FireMonitor\bin\Debug\Logs";
            srvlogspath = @"..\..\..\..\FiresecService\bin\Debug\Logs";
            agnlogspath = @"..\..\..\..\FSAgent\FSAgentServer\bin\Debug\Logs";
            opclogspath = @"..\..\..\..\FiresecOPCServer\FiresecOPCServer\bin\Debug\Logs";
            srvconfpath = @"..\..\..\..\FiresecService\bin\Debug\Configuration";
            fspath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\..\FiresecService\bin\Debug\FiresecService.exe");
            agnpath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\..\FSAgent\FSAgentServer\bin\Debug\FSAgentServer.exe");
            opcpath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\..\FiresecOPCServer\FiresecOPCServer\bin\Debug\FiresecOPCServer.exe");
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

        static string rootlogspath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"..\Logs");
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
			var admdir = new DirectoryInfo(admlogspath);
			var mondir = new DirectoryInfo(monlogspath);
			var srvdir = new DirectoryInfo(srvlogspath);
            var agndir = new DirectoryInfo(agnlogspath);
		    var opcdir = new DirectoryInfo(opclogspath);
            var srvconfdir = new DirectoryInfo(srvconfpath);
			if (admdir.Exists)
			{
				var admfiles = admdir.GetFiles();
				foreach (var file in admfiles)
                    zip.AddFile(file.FullName, "FireAdministrator");
			}

            if (mondir.Exists)
            {
                var monfiles = mondir.GetFiles();
                foreach (var file in monfiles)
                    zip.AddFile(file.FullName, "FireMonitor");
            }

            if (srvdir.Exists)
            {
                var srvfiles = srvdir.GetFiles();
                foreach (var file in srvfiles)
                    zip.AddFile(file.FullName, "FiresecService");
            }

            if (agndir.Exists)
            {
                var agnfiles = agndir.GetFiles();
                foreach (var file in agnfiles)
                    zip.AddFile(file.FullName, "FSAgentServer");
            }

            if (opcdir.Exists)
            {
                var opcfiles = opcdir.GetFiles();
                foreach (var file in opcfiles)
                    zip.AddFile(file.FullName, "FiresecOPCServer");
            }

            if (srvconfdir.Exists)
            {
                var srvfiles = srvconfdir.GetFiles();
                foreach (var file in srvfiles)
                    zip.AddFile(file.FullName, "Configuration");
            }
            
            var memoryStream = new MemoryStream();
		    var sb = new StreamWriter(memoryStream);
            sb.WriteLine("System information");
			try
			{
				var process = Process.GetCurrentProcess();
                sb.WriteLine("Process [{0}]:    {1} x{2}", process.Id, process.ProcessName, GetBitCount(Environment.Is64BitProcess));
                sb.WriteLine("Operation System:  {0} {1} Bit Operating System", Environment.OSVersion, GetBitCount(Environment.Is64BitOperatingSystem));
                sb.WriteLine("ComputerName:      {0}", Environment.MachineName); 
                sb.WriteLine("UserDomainName:    {0}", Environment.UserDomainName);
                sb.WriteLine("UserName:          {0}", Environment.UserName);
                sb.WriteLine("Base Directory:    {0}", AppDomain.CurrentDomain.BaseDirectory);
                sb.WriteLine("SystemDirectory:   {0}", Environment.SystemDirectory);
                sb.WriteLine("ProcessorCount:    {0}", Environment.ProcessorCount);
                sb.WriteLine("SystemPageSize:    {0}", Environment.SystemPageSize);
                sb.WriteLine(".Net Framework:    {0}", Environment.Version);
			}
			catch (Exception ex)
			{
                sb.WriteLine(ex.ToString());
			}
            sb.Flush();
		    memoryStream.Position = 0;
            zip.AddEntry("systeminfo.txt", memoryStream);
            zip.Save();
		}
		private static int GetBitCount(bool is64)
		{
			return is64 ? 64 : 86;
		}
		public RelayCommand RemLogsCommand { get; private set; }
		public void OnRemLogs()
		{
            if (Directory.Exists(admlogspath))
                try{Directory.Delete(admlogspath, true);} catch{ }
            if (Directory.Exists(monlogspath))
                try{Directory.Delete(monlogspath, true);} catch{ }
            if (Directory.Exists(srvlogspath))
                try{Directory.Delete(srvlogspath, true);} catch{ }
            if (Directory.Exists(opclogspath))
                try{Directory.Delete(opclogspath, true);} catch{ }
            if (Directory.Exists(agnlogspath))
                try{Directory.Delete(agnlogspath, true);} catch{ }
        }

	    static bool OnSelectFolder()
        {
            try
            {
                var folder = new FolderBrowserDialog {Description = "Choose a Folder"};
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