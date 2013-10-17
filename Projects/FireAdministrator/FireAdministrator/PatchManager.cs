using System;
using System.IO;
using Common;
using Infrastructure.Common;

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

			PatchHelper.SetPatchNo("Administrator", 1);
		}
	}
}