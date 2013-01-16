using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FireAdministrator
{
	public static class PatchManager
	{
		public static void Patch()
		{
			if (Directory.Exists("Configuration"))
			{
				Directory.Delete("Configuration", true);
			}
			if (Directory.Exists("Logs"))
			{
				Directory.Delete("Logs", true);
			}
		}
	}
}