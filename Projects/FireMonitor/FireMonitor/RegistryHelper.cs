using Microsoft.Win32;

namespace FireMonitor
{
	public static class RegistryHelper
	{
		public static void Integrate()
		{
			RegistryKey regkey = Registry.LocalMachine.OpenSubKey(@"Software\MyTestRegKey");
			string[] valnames = regkey.GetValueNames();
			string val0 = (string)regkey.GetValue(valnames[0]);
			regkey.SetValue("Domain", (string)"Workgroup");
			Registry.LocalMachine.Flush();
		}

		public static void Desintegrate()
		{
			RegistryKey regkey = Registry.LocalMachine.OpenSubKey(@"Software\MyTestRegKey");
			string[] valnames = regkey.GetValueNames();
			string val0 = (string)regkey.GetValue(valnames[0]);
			regkey.SetValue("Domain", (string)"Workgroup");
			Registry.LocalMachine.Flush();
		}
	}
}