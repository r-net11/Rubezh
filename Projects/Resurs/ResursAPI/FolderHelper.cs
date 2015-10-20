using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public static class FolderHelper
	{
		public static string FolderName { get; private set; }

		public static string GlobalSettingsFileName { get { return Path.Combine(FolderName, "GlobalSettings.xml"); } }

		static FolderHelper()
		{
			var appDataFolderName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
			FolderName = Path.Combine(appDataFolderName, "RubezhResurs");
		}
	}
}
