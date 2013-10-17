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
			try
			{
				Patch1();
			}
			catch { }
		}

		static void Patch1()
		{
			var patchNo = PatchHelper.GetPatchNo("Monitor");
			if (patchNo > 0)
				return;

			PatchHelper.SetPatchNo("Monitor", 1);
		}
	}
}