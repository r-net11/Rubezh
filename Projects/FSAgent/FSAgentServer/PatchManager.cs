using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Infrastructure.Common;

namespace FSAgentServer
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
			var patchNo = PatchHelper.GetPatchNo("FSAgent");
			if (patchNo > 0)
				return;

			if (Directory.Exists("Pictures"))
			{
				Directory.Delete("Pictures", true);
			}
			if (Directory.Exists("Logs"))
			{
				Directory.Delete("Logs", true);
			}

			PatchHelper.SetPatchNo("FSAgent", 1);
		}
	}
}