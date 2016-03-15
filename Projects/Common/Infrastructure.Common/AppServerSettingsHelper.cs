﻿using Common;
using FiresecAPI;
using System;
using System.IO;
using System.Xml.Serialization;

namespace Infrastructure.Common
{
	public static class AppServerSettingsHelper
	{
		private static string FileName = AppDataFolderHelper.GetAppServerSettingsFileName();

		public static AppServerSettings AppServerSettings { get; private set; }

		static AppServerSettingsHelper()
		{
			Load();
		}

		public static void Load()
		{
			try
			{
				AppServerSettings = new AppServerSettings();
				if (File.Exists(FileName))
				{
					using (var fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					{
						var xmlSerializer = new XmlSerializer(typeof(AppServerSettings));
						AppServerSettings = (AppServerSettings)xmlSerializer.Deserialize(fileStream);
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
				var xmlSerializer = new XmlSerializer(typeof(AppServerSettings));
				using (var fileStream = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
				{
					xmlSerializer.Serialize(fileStream, AppServerSettings);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e);
			}
		}

		public static void Reset()
		{
			AppServerSettings = new AppServerSettings();
			Save();
		}
	}
}