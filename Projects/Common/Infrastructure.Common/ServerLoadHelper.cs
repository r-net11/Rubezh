using System;
using System.Diagnostics;
using System.Linq;
using Common;

namespace Infrastructure.Common
{
	public static class ServerLoadHelper
	{
		public static void Load()
		{
#if DEBUG
            return;
#endif
			Process[] procs = Process.GetProcessesByName("FiresecService");
			if (procs.Count() == 0)
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
			proc.StartInfo.FileName = "..\\..\\..\\FiresecService\\bin\\Debug\\FiresecService.exe";
#if RELEASE
					proc.StartInfo.FileName = "..\\FiresecService\\FiresecService.exe";
#endif
			proc.Start();
		}
	}
}