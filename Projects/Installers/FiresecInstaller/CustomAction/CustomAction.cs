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
            string serverName = session["SERVERNAME"];
            string monitorName = session["MONITORNAME"];
            string administratorName = session["ADMINISTRATORNAME"];
            string firesecNtService = "FiresecNTService";
            string socketService = "scktsrvr";
            string oldFiresecService = "FS_SER~1";
            string oldFiresecService2 = "fs_server";
            Process[] processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                if ((process.ProcessName == serverName)
                    || (process.ProcessName == monitorName)
                    || (process.ProcessName == administratorName)
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