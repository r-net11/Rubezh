using System.IO;
using Infrastructure.Common;
using FiresecClient;
using XFiresecAPI;

namespace FiresecOPCServer
{
	public static class PatchManager
	{
		public static void Patch()
		{
			try
			{
				Patch1();
			}
			catch { }
		}

		static void Patch1()
		{
			var patchNo = PatchHelper.GetPatchNo("OPC");
			if (patchNo > 0)
				return;

			if (Directory.Exists("Configuration"))
			{
				Directory.Delete("Configuration", true);
			}
			if (Directory.Exists("Logs"))
			{
				Directory.Delete("Logs", true);
			}

			PatchHelper.SetPatchNo("Administrator", 1);
		}
	}
}