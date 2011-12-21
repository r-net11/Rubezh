using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.Diagnostics;

namespace FiresecCustomAction
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult KillFiresecServiceProcess(Session session)
        {
            session.Log("Begin Kill terminate processes");
            string _firesecServiceName = session["FIRESECSERVICENAME"];
            string _fireMonitorName = session["FIREMONITORNAME"];
            string _fireAdministratorName = session["FIREADMINISTRATORNAME"];
            Process[] processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                if ((process.ProcessName == _firesecServiceName)
                    ||(process.ProcessName == _fireMonitorName)
                    ||(process.ProcessName == _fireAdministratorName))
                    process.Kill();
            }

            return ActionResult.Success;
        }
    }
}
