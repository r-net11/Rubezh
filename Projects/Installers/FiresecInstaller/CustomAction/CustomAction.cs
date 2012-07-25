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
			string _firesecServiceName = session["FIRESECSERVICENAME"];
			string _fireMonitorName = session["FIREMONITORNAME"];
			string _fireAdministratorName = session["FIREADMINISTRATORNAME"];
			string _firesecNtService = "FiresecNTService";
			string _socketService = "scktsrvr";
			string _oldFiresecService = "FS_SER~1";
			string _oldFiresecService2 = "fs_server";
			Process[] processes = Process.GetProcesses();
			foreach (var process in processes)
			{
				if ((process.ProcessName == _firesecServiceName)
					|| (process.ProcessName == _fireMonitorName)
					|| (process.ProcessName == _fireAdministratorName)
					|| (process.ProcessName == _oldFiresecService)
					|| (process.ProcessName == _oldFiresecService2)
					|| (process.ProcessName == _firesecNtService)
					|| (process.ProcessName == _socketService))
					process.Kill();
			}

			return ActionResult.Success;
		}
	}
}