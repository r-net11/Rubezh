using System.IO;
using Infrastructure.Common.Windows;
using FiresecClient;
using XFiresecAPI;
using GKProcessor;

namespace FiresecOPCServer
{
	public static class PatchManager
	{
		public static void Patch()
		{
			try
			{
				Patcher.Patch();
			}
			catch { }
		}
	}
}