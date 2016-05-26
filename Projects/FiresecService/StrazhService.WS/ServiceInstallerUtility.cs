using System.Configuration.Install;
using System.Reflection;

namespace StrazhService.WS
{
	public class ServiceInstallerUtility
	{
		private static readonly string exePath = Assembly.GetExecutingAssembly().Location;

		public static bool Install()
		{
			try
			{
				ManagedInstallerClass.InstallHelper(new[] {exePath});
			}
			catch
			{
				return false;
			}
			return true;
		}

		public static bool Uninstall()
		{
			try
			{
				ManagedInstallerClass.InstallHelper(new[] {"/u", exePath});
			}
			catch
			{
				return false;
			}
			return true;
		}
	}
}