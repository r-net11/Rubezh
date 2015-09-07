using System.Diagnostics;
using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Windows;
using Microsoft.Win32;

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
				catch(Exception e)
				{
					//MessageBox.Show(e.ToString());
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
				var regitryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
				using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, regitryView))
				{
					var lKey = hklm.OpenSubKey(@"SOFTWARE\PostgreSQL\Installations");
					session["NOTPOSTGRESINSTALLED"] = lKey.SubKeyCount > 0 ? "0" : "1";
				}
			}
			catch (Exception)
			{
			}
			return ActionResult.Success;
		}
	}
}