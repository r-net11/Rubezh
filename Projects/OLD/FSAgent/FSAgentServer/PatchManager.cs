using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Infrastructure.Common.Windows;
using Common;
using GKProcessor;

namespace FSAgentServer
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