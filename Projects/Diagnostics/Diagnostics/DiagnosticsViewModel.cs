using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;

namespace Diagnostics
{
	public class DiagnosticsViewModel : BaseViewModel
	{
		public DiagnosticsViewModel()
		{
			GetLogsCommand = new RelayCommand(OnGetLogs);
			RemLogsCommand = new RelayCommand(OnRemLogs);
		}

		private string fspath;
		private bool isServerAuto;
		public bool IsServerAuto
		{
			get
			{
				var readkey = Registry.CurrentUser.OpenSubKey(@"software\Microsoft\Windows\CurrentVersion\Run");
			    if (readkey != null)
			    {
			        if (readkey.GetValue("FiresecService") != null)
			            fspath = readkey.GetValue("FiresecService").ToString();
			        else
			        {
			            fspath = @"C:\Program Files\Rubezh\FiresecService\FiresecService.exe";
			            return false;
			        }
			        readkey.Close();
			    }
			    return true;
			}
			set
			{
				var readkey = Registry.CurrentUser.CreateSubKey(@"software\Microsoft\Windows\CurrentVersion\Run");
                isServerAuto = value;
                if (isServerAuto)
				{
				    if (readkey != null) readkey.SetValue("FiresecService", fspath);
				}
				else
					if (!String.IsNullOrEmpty(readkey.GetValue("FiresecService").ToString()))
						readkey.DeleteValue("FiresecService");
			    if (readkey != null) readkey.Close();
                OnPropertyChanged("IsServerAuto");
			}
		}

        private string opcpath;
        private bool isOpcServerAuto;
        public bool IsOpcServerAuto
        {
            get
            {
                var readkey = Registry.CurrentUser.OpenSubKey(@"software\Microsoft\Windows\CurrentVersion\Run");
                if (readkey != null)
                {
                    if (readkey.GetValue("FiresecOPCServer") != null)
                        opcpath = readkey.GetValue("FiresecOPCServer").ToString();
                    else
                    {
                        opcpath = @"C:\Program Files\Rubezh\FiresecOPCServer\FiresecOPCServer.exe";
                        return false;
                    }
                    readkey.Close();
                }
                return true;
            }
            set
            {
                var readkey = Registry.CurrentUser.CreateSubKey(@"software\Microsoft\Windows\CurrentVersion\Run");
                isOpcServerAuto = value;
                if (isOpcServerAuto)
                {
                    if (readkey != null) readkey.SetValue("FiresecOPCServer", opcpath);
                }
                else
                    if (!String.IsNullOrEmpty(readkey.GetValue("FiresecOPCServer").ToString()))
                        readkey.DeleteValue("FiresecOPCServer");
                if (readkey != null) readkey.Close();
                OnPropertyChanged("IsOpcServerAuto");
            }
        }

		public RelayCommand GetLogsCommand { get; private set; }
		public void OnGetLogs()
		{
			var admlogspath = @"..\FireAdministrator\Logs";
			var monlogspath = @"..\FireMonitor\Logs";
			var srvlogspath = @"..\FiresecService\Logs";
            var srvconfpath = @"..\FiresecService\Configuration";
#if DEBUG
			admlogspath = @"..\..\..\..\FireAdministrator\bin\Debug\Logs";
			monlogspath = @"..\..\..\..\FireMonitor\bin\Debug\Logs";
			srvlogspath = @"..\..\..\..\FiresecService\bin\Debug\Logs";
            srvconfpath = @"..\..\..\..\FiresecService\bin\Debug\Configuration";
#endif
			var curlogspath = "..\\Logs\\Logs " + DateTime.Today.Date.ToShortDateString();
			var admdir = new DirectoryInfo(admlogspath);
			var mondir = new DirectoryInfo(monlogspath);
			var srvdir = new DirectoryInfo(srvlogspath);
            var srvconfdir = new DirectoryInfo(srvconfpath);

			if (!Directory.Exists(curlogspath + "\\FireAdministrator"))
				Directory.CreateDirectory(curlogspath + "\\FireAdministrator");
			if (!Directory.Exists(curlogspath + "\\FireMonitor"))
				Directory.CreateDirectory(curlogspath + "\\FireMonitor");
			if (!Directory.Exists(curlogspath + "\\FiresecService"))
				Directory.CreateDirectory(curlogspath + "\\FiresecService");
            if (!Directory.Exists(curlogspath + "\\Configuration"))
                Directory.CreateDirectory(curlogspath + "\\Configuration");

			if (admdir.Exists)
			{
				FileInfo[] admfiles = admdir.GetFiles();
				foreach (FileInfo file in admfiles)
				{
					var temppath = Path.Combine(curlogspath, "FireAdministrator\\" + file.Name);
                    if (file.Length > 0)
                        file.CopyTo(temppath, true);
				}
			}

			if (mondir.Exists)
			{
				FileInfo[] monfiles = mondir.GetFiles();
				foreach (FileInfo file in monfiles)
				{
					var temppath = Path.Combine(curlogspath, "FireMonitor\\" + file.Name);
                    if (file.Length > 0)
                        file.CopyTo(temppath, true);
				}
			}

			if (srvdir.Exists)
			{
				FileInfo[] srvfiles = srvdir.GetFiles();
				foreach (FileInfo file in srvfiles)
				{
					var temppath = Path.Combine(curlogspath, "FiresecService\\" + file.Name);
                    if (file.Length > 0)
                        file.CopyTo(temppath, true);
				}
			}

            if (srvconfdir.Exists)
            {
                FileInfo[] srvfiles = srvconfdir.GetFiles();
                foreach (FileInfo file in srvfiles)
                {
                    var temppath = Path.Combine(curlogspath, "Configuration\\" + file.Name);
                    if ((file.Length > 0)&&(file.Extension == ".xml"))
                        file.CopyTo(temppath, true);
                }
            }

			StringBuilder sb = new StringBuilder(string.Empty);
			sb.AppendLine("System information");
			try
			{
				var process = Process.GetCurrentProcess();
                sb.AppendFormat("Process [{0}]:    {1} x{2}", process.Id, process.ProcessName, GetBitCount(Environment.Is64BitProcess)); sb.AppendLine();
                sb.AppendFormat("Operation System:  {0} {1} Bit Operating System", Environment.OSVersion, GetBitCount(Environment.Is64BitOperatingSystem)); sb.AppendLine();
                sb.AppendFormat("ComputerName:      {0}", Environment.MachineName); sb.AppendLine();
                sb.AppendFormat("UserDomainName:    {0}", Environment.UserDomainName); sb.AppendLine();
                sb.AppendFormat("UserName:          {0}", Environment.UserName); sb.AppendLine();
                sb.AppendFormat("Base Directory:    {0}", AppDomain.CurrentDomain.BaseDirectory); sb.AppendLine();
                sb.AppendFormat("SystemDirectory:   {0}", Environment.SystemDirectory); sb.AppendLine();
                sb.AppendFormat("ProcessorCount:    {0}", Environment.ProcessorCount); sb.AppendLine();
                sb.AppendFormat("SystemPageSize:    {0}", Environment.SystemPageSize); sb.AppendLine();
				sb.AppendFormat(".Net Framework:    {0}", Environment.Version);
			}
			catch (Exception ex)
			{
				sb.Append(ex.ToString());
			}
            System.IO.File.WriteAllText(@"..\Logs\systeminfo.txt", sb.ToString(), Encoding.GetEncoding(1252));
		}
		private static int GetBitCount(bool is64)
		{
			return is64 ? 64 : 86;
		}
		public RelayCommand RemLogsCommand { get; private set; }
		public void OnRemLogs()
		{
			try
			{
				var curlogspath = "..\\Logs";
				var directories = Directory.GetDirectories(curlogspath);
				foreach (var directory in directories)
					Directory.Delete(directory, true);
			}
			catch
			{ }
		}
	}
}