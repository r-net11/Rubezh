using System;
using System.IO;
using Infrastructure.Common;

namespace Diagnostics
{
    public class DiagnosticsViewModel
    {
        public DiagnosticsViewModel()
        {
            GetLogsCommand = new RelayCommand(OnGetLogs);
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
            FileInfo systeminfo = OS
        }
    }
}
