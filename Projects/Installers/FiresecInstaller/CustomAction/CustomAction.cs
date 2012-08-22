using System.Diagnostics;
using Microsoft.Deployment.WindowsInstaller;

namespace CustomAction
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult KillFiresecProcess(Session session)
        {
            session.Log("Begin Kill terminate processes");
            string firesecServiceName = session["FIRESECSERVICENAME"];
            string fireMonitorName = session["FIREMONITORNAME"];
            string fireAdministratorName = session["FIREADMINISTRATORNAME"];
            string firesecNtService = "FiresecNTService";
            string socketService = "scktsrvr";
            string oldFiresecService = "FS_SER~1";
            string oldFiresecService2 = "fs_server";
            Process[] processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                if ((process.ProcessName == firesecServiceName)
                    || (process.ProcessName == fireMonitorName)
                    || (process.ProcessName == fireAdministratorName)
                    || (process.ProcessName == oldFiresecService)
                    || (process.ProcessName == oldFiresecService2)
                    || (process.ProcessName == firesecNtService)
                    || (process.ProcessName == socketService))
                    process.Kill();
            }

            return ActionResult.Success;
        }
    }
}