using System;
using System.Configuration.Install;
using System.Reflection;
using Common;

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
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове ServiceInstallerUtility.Install");
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
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове ServiceInstallerUtility.Uninstall");
				return false;
			}
			return true;
		}
	}
}