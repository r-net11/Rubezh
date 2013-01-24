using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;

namespace Infrastructure.Common
{
	public static class UACHelper
	{
		public static bool IsAdministrator
		{
			get
			{
				var windowsIdentity = WindowsIdentity.GetCurrent();
				var windowsPrincipal = new WindowsPrincipal(windowsIdentity);
				return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
			}
		}
	}
}