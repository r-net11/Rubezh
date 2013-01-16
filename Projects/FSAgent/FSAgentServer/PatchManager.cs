using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FSAgentServer
{
	public static class PatchManager
	{
		public static void Patch()
		{
			if (Directory.Exists("Pictures"))
			{
				Directory.Delete("Pictures", true);
			}
			if (Directory.Exists("Logs"))
			{
				Directory.Delete("Logs", true);
			}
		}
	}
}