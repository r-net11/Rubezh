using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Text;

namespace FiresecNTService
{
    public partial class FiresecNTService : ServiceBase
    {
        public FiresecNTService()
        {
            InitializeComponent();
        }

        static void Main(string[] args)
        {
            var servicesToRun = new ServiceBase[] { new FiresecNTService() };
            ServiceBase.Run(servicesToRun);
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            return;

            var commandLineArgs = Environment.GetCommandLineArgs();
            var dirInfo = new DirectoryInfo(commandLineArgs[0]);
            var nameFiresecExe = commandLineArgs[1];
            var pathFiresecExe = dirInfo.FullName.Replace(dirInfo.Name, "") + nameFiresecExe;
            try
            {
                var output = new StringBuilder();
                if (!Win32API.CreateProcessAsUser(pathFiresecExe, dirInfo.FullName.Replace(dirInfo.Name, ""), "winlogon", out output))
                    Process.Start(pathFiresecExe);
            }
            catch (Win32Exception)
            {
            }
        }

        protected override void OnStop()
        {
            base.OnStop();
            return;

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