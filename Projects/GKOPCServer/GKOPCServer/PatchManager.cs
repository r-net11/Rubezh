using System.IO;
using Infrastructure.Common;
using GKProcessor;

namespace GKOPCServer
{
	public static class PatchManager
	{
		public static void Patch()
		{
			try
			{
				Patcher.AddPatchToList("GKOPC.Patch1", () => Patch1());
				Patcher.Patch();
			}
			catch { }
		}

		static void Patch1()
		{
		}
	}
}