using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Common;

namespace Infrastructure.Common
{
	public static class RevisorLoadHelper
	{
		public static void Load()
		{
			RegistrySettingsHelper.SetBool("IsException", false);

			var proc = Process.GetProcessesByName("Revisor");
			var revisorPath = RegistrySettingsHelper.GetString("RevisorPath");
			if (String.IsNullOrEmpty(revisorPath))
			{
				var executableFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
				revisorPath = executableFolder + @"\Revisor.exe";
#if DEBUG
				revisorPath = @"..\..\..\Revisor\Revisor\bin\Debug\Revisor.exe";
#endif
			}
			if (proc.Count() == 0 && Process.GetCurrentProcess().ProcessName != "FireMonitor.vshost")
			{
				if (File.Exists(revisorPath))
				{
					Process.Start(revisorPath);
				}
				else
				{
					Logger.Error("RevisorLoadHelper RevisorPath not found " + revisorPath);
				}
			}
		}
	}
}