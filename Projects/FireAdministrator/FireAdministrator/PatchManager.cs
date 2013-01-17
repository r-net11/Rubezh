using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Infrastructure.Common;
using Common;

namespace FireAdministrator
{
	public static class PatchManager
	{
		public static void Patch()
		{
			try
			{
				Patch1();
			}
			catch (Exception e)
			{
				Logger.Error(e, "PatchManager.Patch");
			}
		}

		static void Patch1()
		{
			var patchNo = PatchHelper.GetPatchNo("Administrator");
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