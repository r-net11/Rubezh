using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Win32;

namespace Infrastructure.Common
{
	public static class RevisorLoadHelper
	{
		public static void Load()
		{
			RegistrySettingsHelper.Set("IsException", false.ToString());

			var proc = Process.GetProcessesByName("Revisor");
			var revisorpath = RegistrySettingsHelper.Get("RevisorPath");
			if (String.IsNullOrEmpty(revisorpath))
			{
				revisorpath = @"Revisor.exe";
#if DEBUG
				revisorpath = @"..\..\..\Revisor\Revisor\bin\Debug\Revisor.exe";
#endif
			}
			if ((proc.Count() == 0) && (Process.GetCurrentProcess().ProcessName != "FireMonitor.vshost"))
			{
				Process.Start(revisorpath);
			}
		}
	}
}