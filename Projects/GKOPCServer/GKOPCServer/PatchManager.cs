using System.IO;
using Infrastructure.Common;
using Common.GK;

namespace GKOPCServer
{
	public static class PatchManager
	{
		public static void Initialize()
		{
			try
			{
				Patcher.AddPatchToList("GKOPC", "Patch1", ()=>Patch1());
			}
			catch { }
		}

		static void Patch1()
		{
		}
	}
}