using Common;
using StrazhAPI;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using FiresecService.Properties;
using FiresecService.Service.Validators;
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

			// Проверяем состав конфигурации на предмет соответствия лицензионным ограничениям
			ConfigurationElementsAgainstLicenseDataValidator.Instance.Validate();
		}

		private static void ExtractAllConfigFiles()
		{
			if (File.Exists(ConfigFileName))
			{
				new ZipFile(ConfigFileName).ExtractAll(ConfigDirectory, ExtractExistingFileAction.DoNotOverwrite);
			}
			else
			{
				//MessageBox.Show(Resources.ConfigFileNotExist, Resources.ErrorCaption, MessageBoxButton.OK, MessageBoxImage.Error);
				//Application.Current.MainWindow.Close();
				throw new FileNotFoundException(Resources.ConfigFileNotExist);
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
			var securityConfiguration = GetConfigurationFromZip("SecurityConfiguration.xml", typeof(SecurityConfiguration)) as SecurityConfiguration;

			if (securityConfiguration == null) return null;

			securityConfiguration.AfterLoad();

			return securityConfiguration;
		}

		public static LayoutsConfiguration GetLayoutsConfiguration()
		{
			// Данный метод десериализации конфигурации макетов валится с ошибкой:
			// Cannot serialize member StrazhAPI.Models.Layouts.LayoutPart.Properties of type StrazhAPI.Models.Layouts.ILayoutProperties because it is an interface.
			//var configuration = GetConfigurationFromZip("LayoutsConfiguration.xml", typeof(LayoutsConfiguration)) as LayoutsConfiguration;

			// Поэтому используем другой метод десериализации макетов
			var configDirectoryName = AppDataFolderHelper.GetServerAppDataPath("Config");
			var filePath = Path.Combine(configDirectoryName, "LayoutsConfiguration.xml");
			var configuration = ZipSerializeHelper.DeSerialize<LayoutsConfiguration>(filePath, false);

			if (configuration == null) return null;

			configuration.AfterLoad();

			return configuration;
		}

		private static SystemConfiguration GetSystemConfiguration()
		{
		//	var configDirectoryName = AppDataFolderHelper.GetServerAppDataPath("Config");
		//	var filePath = Path.Combine(configDirectoryName, "SystemConfiguration.xml");
		//	var systemConfiguration = ZipSerializeHelper.DeSerialize<SystemConfiguration>(filePath, false);
			var systemConfiguration = (SystemConfiguration)GetConfigurationFromZip("SystemConfiguration.xml", typeof(SystemConfiguration));

			if (systemConfiguration == null) return null;

			systemConfiguration.AfterLoad();
			return systemConfiguration;
		}

		private static SKDConfiguration GetSKDConfiguration()
		{
			var skdConfiguration = (SKDConfiguration)GetConfigurationFromZip("SKDConfiguration.xml", typeof(SKDConfiguration));

			if (skdConfiguration == null) return null;

			skdConfiguration.AfterLoad();
			return skdConfiguration;
		}

		private static VersionedConfiguration GetConfigurationFromZip(string fileName, Type type)
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