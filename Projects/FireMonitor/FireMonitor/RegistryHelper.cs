using Microsoft.Win32;

namespace FireMonitor
{
	public static class RegistryHelper
	{
		public static void Integrate()
		{
			var executablePath = System.Reflection.Assembly.GetEntryAssembly().Location;

			RegistryKey shellRegistryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true);
			shellRegistryKey.SetValue("Shell", executablePath);

			RegistryKey taskManagerRegistryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", true);
			taskManagerRegistryKey.SetValue("DisableTaskMgr", "dword:00000001");

			Registry.LocalMachine.Flush();
		}

		public static void Desintegrate()
		{
			RegistryKey shellRegistryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true);
			shellRegistryKey.SetValue("Shell", "explorer.exe");

			RegistryKey taskManagerRegistryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", true);
			taskManagerRegistryKey.SetValue("DisableTaskMgr", "dword:00000000");

			Registry.LocalMachine.Flush();
		}
	}
}