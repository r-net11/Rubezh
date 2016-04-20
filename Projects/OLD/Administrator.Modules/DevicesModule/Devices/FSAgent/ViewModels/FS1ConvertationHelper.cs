using System;
using System.IO;
using System.IO.Compression;
using System.Windows;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Ionic.Zip;

namespace Infrastructure
{
	public class FS1ConvertationHelper
	{
		public void ConvertConfiguration()
		{
			if (FiresecManager.FiresecDriver == null)
				return;

			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите конвертировать конфигурацию?"))
			{
				WaitHelper.Execute(() =>
				{
					LoadingService.Show("Конвертирование конфигурации", "Конвертирование конфигурации", 6);
					var convertationResult = FiresecManager.FiresecDriver.Convert();
					if (convertationResult.HasError)
					{
						MessageBoxService.ShowError(convertationResult.Error);
						return;
					}
					LoadingService.Show("Синхронизация конфигурации", "Конвертирование конфигурации", 6);
					FiresecManager.FiresecDriver.Synchronyze(false);

					ServiceFactory.SaveService.FSChanged = false;
					ServiceFactory.SaveService.PlansChanged = false;
					LoadingService.DoStep("Обновление конфигурации");
					FiresecManager.UpdateConfiguration();

					LoadingService.DoStep("Сохранение конфигурации");
					var tempFolderName = AppDataFolderHelper.GetTempFolder();
					if (!Directory.Exists(tempFolderName))
						Directory.CreateDirectory(tempFolderName);

					var tempFileName = AppDataFolderHelper.GetTempFileName();
					if (File.Exists(tempFileName))
						File.Delete(tempFileName);

					TempZipConfigurationItemsCollection = new ZipConfigurationItemsCollection();

					AddConfiguration(tempFolderName, "DeviceConfiguration.xml", FiresecManager.FiresecConfiguration.DeviceConfiguration, 1, 1);
					AddConfiguration(tempFolderName, "PlansConfiguration.xml", FiresecManager.PlansConfiguration, 1, 1);
					AddConfiguration(tempFolderName, "ZipConfigurationItemsCollection.xml", TempZipConfigurationItemsCollection, 1, 1);

					var destinationImagesDirectory = AppDataFolderHelper.GetFolder(Path.Combine(tempFolderName, "Content"));
					if (Directory.Exists(ServiceFactory.ContentService.ContentFolder))
					{
						if (Directory.Exists(destinationImagesDirectory))
							Directory.Delete(destinationImagesDirectory);
						if (!Directory.Exists(destinationImagesDirectory))
							Directory.CreateDirectory(destinationImagesDirectory);
						var sourceImagesDirectoryInfo = new DirectoryInfo(ServiceFactory.ContentService.ContentFolder);
						foreach (var fileInfo in sourceImagesDirectoryInfo.GetFiles())
						{
							fileInfo.CopyTo(Path.Combine(destinationImagesDirectory, fileInfo.Name));
						}
					}

					var zipFile = new ZipFile(tempFileName);
					zipFile.AddDirectory(tempFolderName);
					zipFile.Save(tempFileName);
					zipFile.Dispose();
					if (Directory.Exists(tempFolderName))
						Directory.Delete(tempFolderName, true);

					if (Directory.Exists(tempFolderName))
						Directory.Delete(tempFolderName, true);

					using (var fileStream = new FileStream(tempFileName, FileMode.Open))
					{
						FiresecManager.FiresecService.SetConfig(fileStream);
					}
					File.Delete(tempFileName);

					LoadingService.DoStep("Оповещение клиентов об изменении конфигурации");
					FiresecManager.FiresecService.NotifyClientsOnConfigurationChanged();
					LoadingService.Close();
				});
				ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
			}
		}

		static ZipConfigurationItemsCollection TempZipConfigurationItemsCollection = new ZipConfigurationItemsCollection();

		void AddConfiguration(string folderName, string name, VersionedConfiguration configuration, int minorVersion, int majorVersion)
		{
			configuration.BeforeSave();
			configuration.Version = new ConfigurationVersion() { MinorVersion = minorVersion, MajorVersion = majorVersion };
			ZipSerializeHelper.Serialize(configuration, Path.Combine(folderName, name), true);

			TempZipConfigurationItemsCollection.ZipConfigurationItems.Add(new ZipConfigurationItem(name, minorVersion, majorVersion));
		}
		
		public byte[] ZipStr(String str)
		{
			using (MemoryStream output = new MemoryStream())
			{
				using (DeflateStream gzip =
				  new DeflateStream(output, CompressionMode.Compress))
				{
					using (StreamWriter writer =
					  new StreamWriter(gzip, System.Text.Encoding.UTF8))
					{
						writer.Write(str);
					}
				}

				return output.ToArray();
			}
		}

		public string UnZipStr(byte[] input)
		{
			using (MemoryStream inputStream = new MemoryStream(input))
			{
				using (DeflateStream gzip =
				  new DeflateStream(inputStream, CompressionMode.Decompress))
				{
					using (StreamReader reader =
					  new StreamReader(gzip, System.Text.Encoding.UTF8))
					{
						return reader.ReadToEnd();
					}
				}
			}
		}

		string EncodeBase64(string toEncode)
		{
			byte[] toEncodeAsBytes
				  = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
			string returnValue
				  = System.Convert.ToBase64String(toEncodeAsBytes);
			return returnValue;
		}
	}
}