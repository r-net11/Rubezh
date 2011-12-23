using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;

namespace FiresecWindowsService
{
    class Program : ServiceBase
    {
        static void Main(string[] args)
        {
            ServiceBase.Run(new Program());
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            var commandLineArgs = Environment.GetCommandLineArgs();
            DirectoryInfo dirInfo = new DirectoryInfo(commandLineArgs[0]);
            var nameFiresecExe = commandLineArgs[1];
            var pathFiresecExe = dirInfo.FullName.Replace(dirInfo.Name, "") + nameFiresecExe;
            Process.Start(pathFiresecExe, "/Start /Hide");
        }

        protected override void OnStop()
        {
            base.OnStop();
            var processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                if (process.ProcessName == "FiresecService")
                {
                    process.Kill();
                }
            }
        }
    }
}
