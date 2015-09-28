using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.Processor
{
	public static class SettingsManager
	{
		static string FileName { get; set; }
		public static ResursSettings ResursSettings { get; set; }

		public static void Load()
		{
			ResursSettings = new ResursSettings();
		}

		public static void Save()
		{

		}
	}
}