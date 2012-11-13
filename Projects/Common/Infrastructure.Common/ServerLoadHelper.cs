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
			Process[] processes = Process.GetProcessesByName("FiresecService");
            Process[] processes2 = Process.GetProcessesByName("FiresecService.vshost");
			if ((processes.Count() == 0) && (processes2.Count() == 0))
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
            var fileName = @"..\FiresecService\FiresecService.exe";
#if DEBUG
            fileName = "..\\..\\..\\FiresecService\\bin\\Debug\\FiresecService.exe";
#endif

            if (!File.Exists(fileName))
			{
				Logger.Error("ServerLoadHelper.Start File Not Exist " + fileName);
			}
            proc.StartInfo.FileName = fileName;
			proc.Start();
		}
	}
}