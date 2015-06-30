using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Common;
using Infrastructure.Common;
using Infrastructure.Models;
using System.Xml.Serialization;

namespace Infrastructure
{
	public static class ClientSettings
	{
		public static readonly string SKDSettingsFileName = AppDataFolderHelper.GetMonitorSettingsPath("SKDMissmatchSettings.xml");

		static SKDMissmatchSettings _skdMissmatchSettings;
		public static SKDMissmatchSettings SKDMissmatchSettings
		{
			get { return _skdMissmatchSettings ?? (_skdMissmatchSettings = new SKDMissmatchSettings()); }
			set { _skdMissmatchSettings = value; }
		}

		public static void LoadSettings()
		{
			try
			{
				LoadSKDMissmatchSettings();
			}
			catch (Exception e)
			{
				Logger.Error(e, "ClientSettings.LoadSettings");
			}
		}

		public static void SaveSettings()
		{
			try
			{
				if (Directory.Exists(AppDataFolderHelper.GetMonitorSettingsPath()) == false)
					Directory.CreateDirectory(AppDataFolderHelper.GetMonitorSettingsPath());

				SaveSKDMissmatchSettings();
			}
			catch (Exception e)
			{
				Logger.Error(e, "ClientSettings.SaveSettings");
			}
		}

		static void LoadSKDMissmatchSettings()
		{
			if (File.Exists(SKDSettingsFileName))
			{
				using (var fileStream = new FileStream(SKDSettingsFileName, FileMode.Open))
				{
					var xmlSerializer = new XmlSerializer(typeof(SKDMissmatchSettings));
					SKDMissmatchSettings = (SKDMissmatchSettings)xmlSerializer.Deserialize(fileStream);
				}
			}
			else
			{
				SKDMissmatchSettings = new SKDMissmatchSettings();
			}
		}

		static void SaveSKDMissmatchSettings()
		{
			using (var fileStream = new FileStream(SKDSettingsFileName, FileMode.Create))
			{
				var xmlSerializer = new XmlSerializer(typeof(SKDMissmatchSettings));
				xmlSerializer.Serialize(fileStream, SKDMissmatchSettings);
			}
		}
	}
}