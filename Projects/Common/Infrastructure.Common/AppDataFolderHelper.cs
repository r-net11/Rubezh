using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Infrastructure.Common
{
	public class AppDataFolderHelper
	{
		static string AppDataFolderName;

		static AppDataFolderHelper()
		{
			var appDataFolderName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
			AppDataFolderName = Path.Combine(appDataFolderName, "Firesec2");
		}

		public static string GetTempFileName()
		{
			var tempFileName = Path.Combine(AppDataFolderName, Path.GetTempFileName());
			return tempFileName;
		}

		public static string GetClientConfigurationDirectory()
		{
			var tempFileName = Path.Combine(AppDataFolderName, "ClientConfiguration");
			return tempFileName;
		}

		public static string GetServerConfigurationDirectory()
		{
			var tempFileName = Path.Combine(AppDataFolderName, "ServerConfiguration");
			return tempFileName;
		}
	}
}