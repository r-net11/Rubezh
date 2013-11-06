using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GKProcessor;
using Common;

namespace FireMonitor
{
	public static class PatchManager
	{
		public static void Patch()
		{
			try
			{
				Patcher.Patch();
			}
			catch (Exception e)
			{
				Logger.Error(e, "PatchManager.Patch");
			}
		}
	}
}
