using System;
using System.Diagnostics;
using System.Reflection;
using Common;
using Microsoft.Win32;

namespace Infrastructure.Common
{
	public static class ShellIntegrationHelper
	{
		public static void Integrate()
		{
			try
			{
				var registryKey = Registry.CurrentUser.CreateSubKey(@"software\Microsoft\Windows\CurrentVersion\Run");
				if (registryKey != null)
				{
					registryKey.DeleteValue("RubezhService");
					registryKey.DeleteValue("FSAgentServer");
					registryKey.DeleteValue("GKOPCServer");
					registryKey.Close();
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "RubezhMonitor.Integrate 3");
			}

			try
			{
				var executablePath = Assembly.GetEntryAssembly().Location;
				RegistryKey shellRegistryKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true);
				shellRegistryKey.SetValue("Shell", executablePath);
				shellRegistryKey.Flush();
			}
			catch (Exception e)
			{
				Logger.Error(e, "RubezhMonitor.Integrate 1");
			}
			try
			{
				RegistryKey taskManagerRegistryKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System");
				taskManagerRegistryKey.SetValue("DisableTaskMgr", 1, RegistryValueKind.DWord);
				taskManagerRegistryKey.Flush();
			}
			catch (Exception e)
			{
				Logger.Error(e, "RubezhMonitor.Integrate 2");
			}

			RegistryHelper.RegistryHelper.Integrate();

			GlobalSettingsHelper.GlobalSettings.Server_EnableRemoteConnections = false;
			GlobalSettingsHelper.Save();
		}

		public static void Desintegrate()
		{
			try
			{
				RegistryKey shellRegistryKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true);
				shellRegistryKey.SetValue("Shell", "explorer.exe");
				shellRegistryKey.Flush();

				RegistryKey shellRegistryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true);
				shellRegistryKeyLocalMachine.SetValue("Shell", "explorer.exe");
				shellRegistryKeyLocalMachine.Flush();

				RegistryKey taskManagerRegistryKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System");
				taskManagerRegistryKey.SetValue("DisableTaskMgr", 0, RegistryValueKind.DWord);
				taskManagerRegistryKey.Flush();

				RegistryHelper.RegistryHelper.Desintegrate();
			}
			catch (Exception e)
			{
				Logger.Error(e, "RubezhMonitor.Desintegrate");
			}
		}

		public static bool IsIntegrated
		{
			get
			{
				try
				{
					RegistryKey shellRegistryKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", false);
					object shell = shellRegistryKey.GetValue("Shell");
					var assemblyLocation = (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).Location;
					return shell != null && shell.ToString() == assemblyLocation;
				}
				catch (Exception e)
				{
					Logger.Error(e, "RubezhMonitor.IsIntegrated");
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