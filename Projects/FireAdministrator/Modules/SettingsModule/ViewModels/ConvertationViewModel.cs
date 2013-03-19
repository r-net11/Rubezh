using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using Infrastructure;
using System.IO;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Ionic.Zip;
using FiresecAPI;
using FiresecAPI.Models;
using System.Windows;
using System.IO.Compression;

namespace SettingsModule.ViewModels
{
	public class ConvertationViewModel : BaseViewModel
	{
		public ConvertationViewModel()
		{
			ConvertConfigurationCommand = new RelayCommand(OnConvertConfiguration, CanConvertConfiguration);
			ConvertJournalCommand = new RelayCommand(OnConvertJournal, CanConvertJournal);
		}

		public RelayCommand ConvertConfigurationCommand { get; private set; }
		void OnConvertConfiguration()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите конвертировать конфигурацию?") == MessageBoxResult.Yes)
			{
				WaitHelper.Execute(() =>
				{
					LoadingService.ShowProgress("Конвертирование конфигурации", "Конвертирование конфигурации", 6);
					var convertstionResult = FiresecManager.FiresecDriver.Convert();
					if (convertstionResult.HasError)
					{
						MessageBoxService.ShowError(convertstionResult.Error);
						return;
					}
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
		bool CanConvertConfiguration()
		{
			return FiresecManager.FiresecDriver != null;
		}

		static ZipConfigurationItemsCollection TempZipConfigurationItemsCollection = new ZipConfigurationItemsCollection();

		static void AddConfiguration(string folderName, string name, VersionedConfiguration configuration, int minorVersion, int majorVersion)
		{
			configuration.BeforeSave();
			configuration.Version = new ConfigurationVersion() { MinorVersion = minorVersion, MajorVersion = majorVersion };
			ZipSerializeHelper.Serialize(configuration, Path.Combine(folderName, name));

			TempZipConfigurationItemsCollection.ZipConfigurationItems.Add(new ZipConfigurationItem(name, minorVersion, majorVersion));
		}

		public RelayCommand ConvertJournalCommand { get; private set; }
		void OnConvertJournal()
		{
#if DEBUG
			Encode();
			return;
#endif
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите конвертировать журнал событий?") == MessageBoxResult.Yes)
			{
				WaitHelper.Execute(() =>
				{
					var journalRecords = FiresecManager.FiresecDriver.ConvertJournal();
					FiresecManager.FiresecService.SetJournal(journalRecords);
				});
			}
		}
		bool CanConvertJournal()
		{
			return FiresecManager.FiresecDriver != null;
		}

		void Encode()
		{
			StreamReader streamReader = new StreamReader("D:/plans.txt");
			string text = streamReader.ReadToEnd();
			streamReader.Close();

			var bytes = ZipStr(text);
			var output = System.Convert.ToBase64String(bytes);
			;
		}

		public static byte[] ZipStr(String str)
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

		public static string UnZipStr(byte[] input)
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