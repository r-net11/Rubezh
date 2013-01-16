using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Infrastructure.Common;

namespace FireMonitor
{
	public static class PatchManager
	{
		public static void Patch()
		{
			var assembly = Assembly.GetEntryAssembly();
			if (assembly != null)
			{
				var localFiresecDBFileName = Path.Combine(Path.GetDirectoryName(assembly.Location), "GkJournalDatabase.sdf");
				var appDatalFiresecDBFileName = AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf");
				if (File.Exists(localFiresecDBFileName))
				{
					if (File.Exists(appDatalFiresecDBFileName))
					{
						File.Delete(appDatalFiresecDBFileName);
					}
					File.Move(localFiresecDBFileName, appDatalFiresecDBFileName);
				}
			}
			if (Directory.Exists("ClientSettings"))
			{
				Directory.Delete("ClientSettings", true);
			}
			if (Directory.Exists("Configuration"))
			{
				Directory.Delete("Configuration", true);
			}
			if (Directory.Exists("Logs"))
			{
				Directory.Delete("Logs", true);
			}
		}
	}
}