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

		[CustomAction]
		public static ActionResult CheckOpcCoreComponentsInstalled(Session session)
		{
			session.Log("Check if OPC Core Components Redistributable installed");

			try
			{
				var is64 = Environment.Is64BitOperatingSystem;
				bool isInstall = false;
				var regitryView = is64 ? RegistryView.Registry64 : RegistryView.Registry32;
				using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, regitryView))
				{
					var lKey = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
					var subKeyNames = lKey.GetSubKeyNames();
					
					foreach (var subKeyName in subKeyNames)
					{
						var subKey = lKey.OpenSubKey(subKeyName);
						if (subKey.ValueCount > 0)
						{
							var value = subKey.GetValue("Publisher", null);
							if (value != null)
							{
								var publisher = value is string ? (string)value : null;

								if (publisher == "OPC Foundation")
								{
									value = subKey.GetValue("DisplayName", null);
									if (value != null)
									{
										var displayName = value is string ? (string)value : string.Empty;
										if (displayName.Contains("OPC Core Components Redistributable"))
										{
											isInstall = true;
										}
									}
								}
							}
						}
					}
				}
				if (isInstall && is64)
					session["IS_INSTALL_OPC_CORE_COMPONETSx64"] = "1";
				if (isInstall && !is64)
					session["IS_INSTALL_OPC_CORE_COMPONETSx32"] = "1";
			}
			catch
			{ }
			return ActionResult.Success;
		}
	}
}