using System;
using System.Diagnostics;
using System.Linq;
using Common;
using System.IO;
using System.Windows.Forms;

namespace Infrastructure.Common
{
	public static class ServerLoadHelper
	{
		public static void Load()
		{
#if DEBUG
            return;
#endif
			Process[] processes = Process.GetProcessesByName("FiresecService");
			if (processes.Count() == 0)
			{
				try
				{
					Start();
				}
				catch (Exception e)
				{
					Logger.Error(e, "ServerLoadHelper.Load");
				}
			}
		}

		public static void Reload()
		{
			Start();
		}

		static void Start()
		{
			System.Diagnostics.Process proc = new System.Diagnostics.Process();
			var fileName = "..\\..\\..\\FiresecService\\bin\\Debug\\FiresecService.exe";
			
#if RELEASE
					fileName = "..\\FiresecService\\FiresecService.exe";
#endif
			if (!File.Exists(fileName))
			{
				Logger.Error("ServerLoadHelper.Start File Not Exist " + fileName);
			}
			else
			{
				var exePath = System.Windows.Application.ResourceAssembly.Location;
				fileName = exePath.Replace("FiresecService\\FiresecService.exe", "FireMonitor\\FireMonitor.exe");
				fileName = exePath.Replace("FiresecService\\FiresecService.exe", "FireAdministrator\\FireAdministrator.exe");
				if (!File.Exists(fileName))
				{
					Logger.Error("ServerLoadHelper.Start File Not Exist 2 " + fileName);
				}
			}

			proc.StartInfo.FileName = fileName;
			proc.Start();
		}
	}
}