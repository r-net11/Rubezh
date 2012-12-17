using System.Diagnostics;
using Microsoft.Deployment.WindowsInstaller;

namespace CustomAction
{
    public class CustomActions
    {
        [CustomAction]
		public static ActionResult CloseApplications(Session session)
        {
            session.Log("Begin Kill terminate processes");
            Process[] processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                if ((process.ProcessName == "FiresecService")
					|| (process.ProcessName == "FSAgentServer")
                    || (process.ProcessName == "FireMonitor")
                    || (process.ProcessName == "FireAdministrator")
					|| (process.ProcessName == "Revisor")
					|| (process.ProcessName == "FiresecOPCServer")
                    || (process.ProcessName == "FS_SER~1")
                    || (process.ProcessName == "fs_server")
                    || (process.ProcessName == "FiresecNTService")
                    || (process.ProcessName == "scktsrvr"))
                    process.Kill();
            }
            return ActionResult.Success;
        }
    }
}