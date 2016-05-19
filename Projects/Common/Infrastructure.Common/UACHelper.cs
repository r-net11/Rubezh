using System.Security.Principal;

namespace Infrastructure.Common
{
	public static class UACHelper
	{
		public static bool IsAdministrator
		{
			get
			{
#if DEBUG
				return true;
#endif
				var windowsIdentity = WindowsIdentity.GetCurrent();
				var windowsPrincipal = new WindowsPrincipal(windowsIdentity);
				return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
			}
		}
	}
}