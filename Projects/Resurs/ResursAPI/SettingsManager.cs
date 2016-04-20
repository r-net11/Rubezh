using Common;
using Infrastructure.Common.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ResursAPI
{
	public static class SettingsManager
	{
		static string FileName = FolderHelper.GlobalSettingsFileName;
		public static ResursSettings ResursSettings { get; private set; }

		static SettingsManager()
		{
			Load();
		}

		public static void Load()
		{
			try
			{
				ResursSettings = new ResursSettings();
				if (File.Exists(FileName))
				{
					using (var fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					{
						var xmlSerializer = new XmlSerializer(typeof(ResursSettings));
						ResursSettings = (ResursSettings)xmlSerializer.Deserialize(fileStream);
					}
				}
				else
					Save();
			}
			catch (Exception e)
			{
				Logger.Error(e);
			}
		}
		public static void Save()
		{
			try
			{
				var xmlSerializer = new XmlSerializer(typeof(ResursSettings));
				using (var fileStream = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
				{
					xmlSerializer.Serialize(fileStream, ResursSettings);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e);
			}
		}
	}
}