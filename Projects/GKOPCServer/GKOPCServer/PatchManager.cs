using GKProcessor;

namespace GKOPCServer
{
	public static class PatchManager
	{
		public static void Patch()
		{
			try
			{
				Patcher.AddPatch("GKOPC.Patch1", () => Patch1());
				Patcher.Patch();
			}
			catch { }
		}

		static void Patch1()
		{
		}
	}
}