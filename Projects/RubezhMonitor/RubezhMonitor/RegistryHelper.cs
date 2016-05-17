using System.Diagnostics;
using System.Reflection;
using Microsoft.Win32;
using System;
using Common;

namespace FireMonitor
{
	public static class RegistryHelper
	{
		public static void Integrate()
		{
			try
			{
				var executablePath = Assembly.GetEntryAssembly().Location;

				RegistryKey shellRegistryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true);
				shellRegistryKey.SetValue("Shell", executablePath);
				shellRegistryKey.Flush();

				RegistryKey taskManagerRegistryKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System");
				taskManagerRegistryKey.SetValue("DisableTaskMgr", 1, RegistryValueKind.DWord);
				taskManagerRegistryKey.Flush();
			}
			catch (Exception e)
			{
				Logger.Error(e, "FireMonitor.Integrate");
			}
		}

		public static void Desintegrate()
		{
			try
			{
				RegistryKey shellRegistryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true);
				shellRegistryKey.SetValue("Shell", "explorer.exe");
				shellRegistryKey.Flush();

				RegistryKey taskManagerRegistryKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System");
				taskManagerRegistryKey.SetValue("DisableTaskMgr", 0, RegistryValueKind.DWord);
				taskManagerRegistryKey.Flush();
			}
			catch (Exception e)
			{
				Logger.Error(e, "FireMonitor.Desintegrate");
			}
		}

		public static bool IsIntegrated
		{
			get
			{
				try
				{
					RegistryKey shellRegistryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", false);
					object shell = shellRegistryKey.GetValue("Shell");
					return shell != null && shell.ToString() == (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).Location;
				}
				catch (Exception e)
				{
					Logger.Error(e, "FireMonitor.IsIntegrated");
					return false;
				}
			}
		}
		public static void ShutDown()
		{
			var processStartInfo = new ProcessStartInfo()
			{
				FileName = "shutdown.exe",
				Arguments = "/s /t 00 /f",
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden
			};
			Process.Start(processStartInfo);
		}
	}
}