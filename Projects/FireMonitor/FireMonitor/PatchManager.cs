using System;
using Common;
using GKProcessor;

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