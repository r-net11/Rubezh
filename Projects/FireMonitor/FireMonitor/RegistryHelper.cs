using Microsoft.Win32;

namespace FireMonitor
{
	public static class RegistryHelper
	{
		public static void Integrate()
		{
			var executablePath = System.Reflection.Assembly.GetEntryAssembly().Location;

			RegistryKey shellRegistryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true);
			shellRegistryKey.SetValue("Shell", executablePath);

			RegistryKey taskManagerRegistryKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System");
			taskManagerRegistryKey.SetValue("DisableTaskMgr", "dword:00000001");

			RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).Flush();
		}

		public static void Desintegrate()
		{
			RegistryKey shellRegistryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true);
			shellRegistryKey.SetValue("Shell", "explorer.exe");

			RegistryKey taskManagerRegistryKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System");
			taskManagerRegistryKey.SetValue("DisableTaskMgr", "dword:00000000");

			RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).Flush();
		}
	}
}