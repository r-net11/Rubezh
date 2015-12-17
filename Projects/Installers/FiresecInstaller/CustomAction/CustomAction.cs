using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace CustomAction
{
	public class CustomActions
	{
		[CustomAction]
		public static ActionResult CloseApplications(Session session) //--mode unattended --superaccount asd --superpassword 1
		{
			session.Log("Begin Kill terminate processes");
			Process[] processes = Process.GetProcesses();
			foreach (var process in processes)
			{
				try
				{
					if ((process.ProcessName == "FiresecService")
						|| (process.ProcessName == "FireMonitor")
						|| (process.ProcessName == "FireAdministrator")
						|| (process.ProcessName == "Revisor")
						|| (process.ProcessName == "GKOPCServer")
						|| (process.ProcessName == "FiresecNTService"))
					{
						process.Kill();
					}
				}
				catch (Exception)
				{
				}
			}
			return ActionResult.Success;
		}

		[CustomAction]
		public static ActionResult CheckPostgresInstalled(Session session)
		{
			session.Log("Check if Postgres installed");
			try
			{
				var is64 = Environment.Is64BitOperatingSystem;
				bool isInstall = false;
				var regitryView = is64 ? RegistryView.Registry64 : RegistryView.Registry32;
				using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, regitryView))
				{
					var lKey = hklm.OpenSubKey(@"SOFTWARE\PostgreSQL\Installations");
					isInstall = lKey == null || lKey.SubKeyCount == 0;
				}
				if (isInstall && is64)
					session["IS_INSTALL_POSTGRE64"] = "1";
				if (isInstall && !is64)
					session["IS_INSTALL_POSTGRE32"] = "1";

			}
			catch (Exception)
			{
			}
			return ActionResult.Success;
		}
	}
}