using System;
using Common;
using GKProcessor;
using Infrastructure.Common;

namespace FSAgentServer
{
	public static class PatchManager
	{
		public static void Patch()
		{
			try
			{
				Patcher.AddPatch("FiresecService.Config_2", () => Patch1());
				Patcher.Patch();
			}
			catch (Exception e)
			{
				Logger.Error(e, "PatchManager.Patch");
			}
		}

		static void Patch1()
		{
			XDeviceLibraryConfigurationPatchHelper.Patch();
		}
	}
}