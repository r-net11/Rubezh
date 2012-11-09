using System;
using System.IO;
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

            var OS = System.Environment.OSVersion;
            var processor_name = Registry.LocalMachine.OpenSubKey(@"Hardware\Description\System\CentralProcessor\0", RegistryKeyPermissionCheck.ReadSubTree);   //This registry entry contains entry for processor info.
            System.IO.File.WriteAllText(@"Logs\systeminfo.txt", OS.ToString() + "\n" + processor_name.GetValue("ProcessorNameString"));
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
