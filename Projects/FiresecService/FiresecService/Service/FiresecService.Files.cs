﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common;
using FiresecAPI.Automation;
using FiresecAPI.Models;
using Infrastructure.Common;
using Ionic.Zip;
using FiresecAPI.Journal;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public List<string> GetFileNamesList(string directory)
		{
			return HashHelper.GetFileNamesList(AppDataFolderHelper.GetServerAppDataPath(directory));
		}

		public Dictionary<string, string> GetDirectoryHash(string directory)
		{
			return HashHelper.GetDirectoryHash(AppDataFolderHelper.GetServerAppDataPath(directory));
		}

		public Stream GetFile(string fileName)
		{
			return new FileStream(AppDataFolderHelper.GetServerAppDataPath(fileName), FileMode.Open, FileAccess.Read);
		}

		public Stream GetConfig()
		{
			var filePath = AppDataFolderHelper.GetServerAppDataPath("Config.fscp");
			return new FileStream(filePath, FileMode.Open, FileAccess.Read);
		}

		string CreateTempServer(Stream stream)
		{
			var folderName = AppDataFolderHelper.GetFolder("TempServer");
			var configFileName = Path.Combine(folderName, "Config.fscp");
			if (Directory.Exists(folderName))
				Directory.Delete(folderName, true);
			Directory.CreateDirectory(folderName);

			using (var configFileStream = File.Create(configFileName))
			{
				CopyStream(stream, configFileStream);
			}
			stream.Close();

			return configFileName;
		}

		public void SetConfig(Stream stream)
		{
			var newFileName = CreateTempServer(stream);

			var fileName = AppDataFolderHelper.GetServerAppDataPath("Config.fscp");

			var zipFile = new ZipFile(fileName);
			var newZipFile = ZipFile.Read(newFileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });

			var configurationList = GetConfigurationList(zipFile);
			var newConfigurationList = GetConfigurationList(newZipFile);

			foreach (var zipConfigurationItem in configurationList.GetWellKnownZipConfigurationItems())
			{
				var name = zipConfigurationItem.Name;
				var entry = newZipFile[name];
				if (entry != null)
				{
					var zipStream = entry.OpenReader();
					if (zipFile.Entries.Any(x => x.FileName == name))
						zipFile.RemoveEntry(name);
					zipFile.AddEntry(name, zipStream);

					var newConfiguratino = newConfigurationList.ZipConfigurationItems.FirstOrDefault(x => x.Name == name);
					if (newConfiguratino != null)
					{
						configurationList.ZipConfigurationItems.RemoveAll(x => x.Name == name);
						configurationList.ZipConfigurationItems.Add(newConfiguratino);
					}
				}
			}
			var imagesDirectory = Path.Combine(AppDataFolderHelper.GetFolder("TempServer"), "Unzip", "Content");

			for (int x = zipFile.Count - 1; x >= 0; x--)
			{
				ZipEntry e = zipFile[x];
				if (e.FileName.StartsWith("Content/"))
					zipFile.RemoveEntry(e.FileName);
			}
			if (Directory.Exists(imagesDirectory))
			{
				zipFile.AddDirectory(imagesDirectory, "Content");
			}
			AddConfigurationList(zipFile, configurationList);
			zipFile.Save();
			zipFile.Dispose();
			newZipFile.Dispose();
			File.Delete(newFileName);

			AddJournalMessage(JournalEventNameType.Применение_конфигурации, null);
			ConfigurationCashHelper.Update();
			GKProcessor.SetNewConfig();
			SKDProcessor.SetNewConfig();
			ScheduleRunner.SetNewConfig();
			ProcedureRunner.SetNewConfig();
		}

		ZipConfigurationItemsCollection GetConfigurationList(ZipFile zipFile)
		{
			var entry = zipFile["ZipConfigurationItemsCollection.xml"];
			if (entry != null)
			{
				var zipStream = entry.OpenReader();
				return ZipSerializeHelper.DeSerialize<ZipConfigurationItemsCollection>(zipStream);
			}
			return new ZipConfigurationItemsCollection();
		}

		static void AddConfigurationList(ZipFile zipFile, ZipConfigurationItemsCollection configurationsList)
		{
			var configurationStream = ZipSerializeHelper.Serialize(configurationsList);
			if (zipFile.Entries.Any(x => x.FileName == "ZipConfigurationItemsCollection.xml"))
				zipFile.RemoveEntry("ZipConfigurationItemsCollection.xml");
			configurationStream.Position = 0;
			zipFile.AddEntry("ZipConfigurationItemsCollection.xml", configurationStream);
		}

		public static void CopyStream(Stream input, Stream output)
		{
			var buffer = new byte[8 * 1024];
			int length;
			while ((length = input.Read(buffer, 0, buffer.Length)) > 0)
			{
				output.Write(buffer, 0, length);
			}
			output.Close();
		}
	}
}