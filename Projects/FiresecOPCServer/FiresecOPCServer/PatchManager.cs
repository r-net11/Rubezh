using System.IO;
using Infrastructure.Common;
using FiresecClient;
using XFiresecAPI;
using Common.GK;

namespace FiresecOPCServer
{
	public static class PatchManager
	{
		public static void Patch()
		{
			try
			{
				Patcher.AddPatchToList("OPC", "DeleteConfigurationLogs", () => Patch1());
				Patcher.Patch();
			}
			catch { }
		}

		static void Patch1()
		{
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
