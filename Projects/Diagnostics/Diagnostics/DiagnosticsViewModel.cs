using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Infrastructure.Common;
using Microsoft.Win32;
using System.Management; 
  
namespace Diagnostics
{
    public class DiagnosticsViewModel
    {
        public DiagnosticsViewModel()
        {
            GetLogsCommand = new RelayCommand(OnGetLogs);
            RemLogsCommand = new RelayCommand(OnRemLogs);
        }
        
        public RelayCommand GetLogsCommand { get; private set; }
        public void OnGetLogs()
        {
            var admlogspath = @"..\..\..\..\FireAdministrator\bin\Debug\Logs";
            var monlogspath = @"..\..\..\..\FireMonitor\bin\Debug\Logs";
            var srvlogspath = @"..\..\..\..\FiresecService\bin\Debug\Logs";
            var curlogspath = "Logs\\Logs " + DateTime.Today.Date.ToShortDateString();
            var admdir = new DirectoryInfo(admlogspath);
            var mondir = new DirectoryInfo(monlogspath);
            var srvdir = new DirectoryInfo(srvlogspath);
            
            if (!Directory.Exists(curlogspath+"\\FireAdministrator"))
                Directory.CreateDirectory(curlogspath + "\\FireAdministrator");
            if (!Directory.Exists(curlogspath + "\\FireMonitor"))
                Directory.CreateDirectory(curlogspath + "\\FireMonitor");
            if (!Directory.Exists(curlogspath + "\\FiresecService"))
                Directory.CreateDirectory(curlogspath + "\\FiresecService");

            FileInfo[] admfiles = admdir.GetFiles();
            foreach (FileInfo file in admfiles)
            {
                var temppath = Path.Combine(curlogspath,"FireAdministrator\\" + file.Name);
                file.CopyTo(temppath, true);
            }

            FileInfo[] monfiles = mondir.GetFiles();
            foreach (FileInfo file in monfiles)
            {
                var temppath = Path.Combine(curlogspath, "FireMonitor\\" + file.Name);
                file.CopyTo(temppath, true);
            }

            FileInfo[] srvfiles = srvdir.GetFiles();
            foreach (FileInfo file in srvfiles)
            {
                var temppath = Path.Combine(curlogspath, "FiresecService\\" + file.Name);
                file.CopyTo(temppath, true);
            }

            //var OS = System.Environment.OSVersion;
            //var processor_name = Registry.LocalMachine.OpenSubKey(@"Hardware\Description\System\CentralProcessor\0", RegistryKeyPermissionCheck.ReadSubTree);   //This registry entry contains entry for processor info.
            //var ram = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
            //var raminfo = "";
            //foreach (ManagementObject queryObj in ram.Get())
            //{
            //    raminfo = "RAM: " + Math.Round(System.Convert.ToDouble(queryObj["Capacity"]) / 1024 / 1024, 2) + "Gb " +
            // queryObj["Speed"] + "MHz";
            //}
            //System.IO.File.WriteAllText(@"Logs\systeminfo.txt", OS.ToString() + "\n" + processor_name.GetValue("ProcessorNameString")
            //    + "\n" + raminfo);

			StringBuilder sb = new StringBuilder(string.Empty);
			sb.AppendLine("System information");
			try
			{
				var process = Process.GetCurrentProcess();
                sb.AppendFormat("Process [{0}]:    {1} x{2}\n", process.Id, process.ProcessName, GetBitCount(Environment.Is64BitProcess));
                sb.AppendFormat("Operation System:  {0} {1} Bit Operating System\n", Environment.OSVersion, GetBitCount(Environment.Is64BitOperatingSystem));
                sb.AppendFormat("ComputerName:      {0}\n", Environment.MachineName);
				sb.AppendFormat("UserDomainName:    {0}\n", Environment.UserDomainName);
                sb.AppendFormat("UserName:          {0}\n", Environment.UserName);
				sb.AppendFormat("Base Directory:    {0}\n", AppDomain.CurrentDomain.BaseDirectory);
                sb.AppendFormat("SystemDirectory:   {0}\n", Environment.SystemDirectory);
				sb.AppendFormat("ProcessorCount:    {0}\n", Environment.ProcessorCount);
				sb.AppendFormat("SystemPageSize:    {0}\n", Environment.SystemPageSize);
				sb.AppendFormat(".Net Framework:    {0}", Environment.Version);
			}
			catch (Exception ex)
			{
				sb.Append(ex.ToString());
			}
            System.IO.File.WriteAllText(@"Logs\systeminfo.txt", sb.ToString());
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
                Directory.Delete("Logs", true);
                var OS = System.Environment.OSVersion;
                var processor_name = Registry.LocalMachine.OpenSubKey(
                    @"Hardware\Description\System\CentralProcessor\0", RegistryKeyPermissionCheck.ReadSubTree);
                    //This registry entry contains entry for processor info.
                Directory.CreateDirectory("Logs");
                System.IO.File.WriteAllText(@"Logs\systeminfo.txt",
                                            OS.ToString() + "\n" + processor_name.GetValue("ProcessorNameString"));
            }
            catch
            {}
        }
    }
}
