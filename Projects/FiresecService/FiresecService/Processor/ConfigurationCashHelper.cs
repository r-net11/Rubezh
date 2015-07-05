using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecService.Properties;
using Infrastructure.Common;
using Ionic.Zip;
using System;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

namespace FiresecService
{
	public static class ConfigurationCashHelper
	{
		public static SecurityConfiguration SecurityConfiguration { get; private set; }
		public static SystemConfiguration SystemConfiguration { get; private set; }

		private static readonly string ConfigFileName = AppDataFolderHelper.GetServerAppDataPath("Config.fscp");
		private static readonly string ConfigDirectory = AppDataFolderHelper.GetServerAppDataPath("Config");
		private static readonly string ContentDirectory = Path.Combine(ConfigDirectory, "Content");

		public static void Update()
		{
			CreateConfigDirectory();
			if (!IsValidConfigFiles())
			{
				ExtractAllConfigFiles();
			}

			SecurityConfiguration = GetSecurityConfiguration() ?? new SecurityConfiguration();
			SystemConfiguration = GetSystemConfiguration() ?? new SystemConfiguration();
			SKDManager.SKDConfiguration = GetSKDConfiguration() ?? new SKDConfiguration();

			SystemConfiguration.UpdateConfiguration();
			SKDManager.UpdateConfiguration();
		}

		private static void ExtractAllConfigFiles()
		{
			if (File.Exists(ConfigFileName))
			{
				new ZipFile(ConfigFileName).ExtractAll(ConfigDirectory, ExtractExistingFileAction.DoNotOverwrite);
			}
			else
			{
				MessageBox.Show(Resources.ConfigFileNotExist, Resources.ErrorCaption, MessageBoxButton.OK, MessageBoxImage.Error);
				Application.Current.MainWindow.Close();
			}
		}

		public static bool IsValidConfigFiles()
		{
			return File.Exists(Path.Combine(ConfigDirectory, "LayoutsConfiguration.xml"))
				   && File.Exists(Path.Combine(ConfigDirectory, "PlansConfiguration.xml"))
				   && File.Exists(Path.Combine(ConfigDirectory, "SecurityConfiguration.xml"))
				   && File.Exists(Path.Combine(ConfigDirectory, "SKDConfiguration.xml"))
				   && File.Exists(Path.Combine(ConfigDirectory, "SKDLibraryConfiguration.xml"))
				   && File.Exists(Path.Combine(ConfigDirectory, "SystemConfiguration.xml"))
				   && File.Exists(Path.Combine(ConfigDirectory, "ZipConfigurationItemsCollection.xml"));
		}

		private static void CreateConfigDirectory()
		{
			if (!Directory.Exists(ConfigDirectory))
			{
				Directory.CreateDirectory(ConfigDirectory);
			}

			if (!Directory.Exists(ContentDirectory))
			{
				Directory.CreateDirectory(ContentDirectory);
			}
		}


		public static SecurityConfiguration GetSecurityConfiguration()
		{
			var securityConfiguration = GetConfigurationFomZip("SecurityConfiguration.xml", typeof(SecurityConfiguration)) as SecurityConfiguration;

			if (securityConfiguration == null) return null;

			securityConfiguration.AfterLoad();

			return securityConfiguration;
		}

		static SystemConfiguration GetSystemConfiguration()
		{
			var systemConfiguration = (SystemConfiguration)GetConfigurationFomZip("SystemConfiguration.xml", typeof(SystemConfiguration));

			if (systemConfiguration == null) return null;

			systemConfiguration.AfterLoad();
			return systemConfiguration;
		}

		static SKDConfiguration GetSKDConfiguration()
		{
			var skdConfiguration = (SKDConfiguration)GetConfigurationFomZip("SKDConfiguration.xml", typeof(SKDConfiguration));

			if (skdConfiguration == null) return null;

			skdConfiguration.AfterLoad();
			return skdConfiguration;
		}

		static VersionedConfiguration GetConfigurationFomZip(string fileName, Type type)
		{
			try
			{
				var configDirectoryName = AppDataFolderHelper.GetServerAppDataPath("Config");
				var filePath = Path.Combine(configDirectoryName, fileName);

				VersionedConfiguration versionedConfiguration;

				if (!File.Exists(filePath))
					return new VersionedConfiguration();

				using (Stream stream = new FileStream(filePath, FileMode.Open))
				{
					var xmlSerializer = new XmlSerializer(type);
					versionedConfiguration = (VersionedConfiguration)xmlSerializer.Deserialize(stream);

				}

				versionedConfiguration.ValidateVersion();
				return versionedConfiguration;
			}
			catch (Exception e)
			{
				Logger.Error(e, "ConfigActualizeHelper.GetFile " + fileName);
			}
			return null;
		}
	}
}