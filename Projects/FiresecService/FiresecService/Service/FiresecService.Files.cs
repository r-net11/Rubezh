using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;
using FiresecService.Configuration;
using Infrastructure.Common;
using Ionic.Zip;
using FiresecAPI;
using System.Text;
using FiresecAPI.Models;

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
			ZipConfigActualizeHelper.Actualize(filePath);
			return new FileStream(filePath, FileMode.Open, FileAccess.Read);
		}

		public void SetConfig(Stream stream)
		{
			var fileName = AppDataFolderHelper.GetServerAppDataPath("Config.fscp");
			var newFileName = AppDataFolderHelper.GetTempFileName();
			using (Stream newFile = File.OpenWrite(newFileName))
			{
				CopyStream(stream, newFile);
			}

			var zipFile = new ZipFile(fileName);
			var newZipFile = ZipFile.Read(newFileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });

			var configurationList = GetConfigurationList(zipFile);
			var newConfigurationList = GetConfigurationList(newZipFile);

			foreach (var zipConfigurationItem in configurationList.GetWellKnownZipConfigurationItems)
			{
				var name = zipConfigurationItem.Name;
				var memoryStream = new MemoryStream();
				var entry = newZipFile[name];
				if (entry != null)
				{
					entry.Extract(memoryStream);
					memoryStream.Position = 0;
					if (zipFile.Entries.Any(x => x.FileName == name))
						zipFile.RemoveEntry(name);
					zipFile.AddEntry(name, memoryStream);

					var newConfiguratino = newConfigurationList.ZipConfigurationItems.FirstOrDefault(x => x.Name == name);
					if (newConfiguratino != null)
					{
						configurationList.ZipConfigurationItems.RemoveAll(x => x.Name == name);
						configurationList.ZipConfigurationItems.Add(newConfiguratino);
					}
				}
			}
			AddConfigurationList(zipFile, configurationList);
			zipFile.Save();
			zipFile.Dispose();
			newZipFile.Dispose();

			File.Delete(newFileName);
		}

		ZipConfigurationItemsCollection GetConfigurationList(ZipFile zipFile)
		{
			var infoMemoryStream = new MemoryStream();
			var entry = zipFile["ZipConfigurationItemsCollection.xml"];
			if (entry != null)
			{
				entry.Extract(infoMemoryStream);
				infoMemoryStream.Position = 0;
				return ZipSerializeHelper.DeSerialize<ZipConfigurationItemsCollection>(infoMemoryStream);
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