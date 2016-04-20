using Common;
using RubezhAPI;
using System;
using System.IO;
using System.Xml.Serialization;

namespace Infrastructure.Common
{
	public static class GlobalSettingsHelper
	{
		static string FileName = AppDataFolderHelper.GetGlobalSettingsFileName();
		public static GlobalSettings GlobalSettings { get; private set; }

		static GlobalSettingsHelper()
		{
			Load();
		}

		public static void Load()
		{
			try
			{
				GlobalSettings = new GlobalSettings();
				GlobalSettings.SetDefaultModules();
				if (File.Exists(FileName))
				{
					using (var fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					{
						var xmlSerializer = new XmlSerializer(typeof(GlobalSettings));
						GlobalSettings = (GlobalSettings)xmlSerializer.Deserialize(fileStream);
					}
				}
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
				var xmlSerializer = new XmlSerializer(typeof(GlobalSettings));
				using (var fileStream = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
				{
					xmlSerializer.Serialize(fileStream, GlobalSettings);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e);
			}
		}
		public static void Reset()
		{
			GlobalSettings = new GlobalSettings();
			GlobalSettings.ModuleItems.Add("SettingsModule.dll");
			Save();
		}
	}
}