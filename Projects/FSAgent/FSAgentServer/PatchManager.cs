using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Infrastructure.Common;
using Common;
using Common.GK;

namespace FSAgentServer
{
	public static class PatchManager
	{
		public static void Initialize()
		{
			try
			{
                Patcher.AddPatchToList("FSAgent","DeletePicturesLogs",()=>Patch1());
            }
			catch(Exception e)
			{
				Logger.Error(e, "PatchManager.Patch");
			}
		}

		static void Patch1()
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