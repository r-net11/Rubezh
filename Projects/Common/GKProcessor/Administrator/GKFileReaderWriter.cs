using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common;

namespace GKProcessor
{
	public class GKFileReaderWriter
	{
		public string Error { get; private set; }

		public string ReadConfigFileFromGK(GKDevice gkControllerDevice)
		{
			var progressCallback = GKProcessorManager.StartProgress("Чтение конфигурационного файла из " + gkControllerDevice.PresentationName, "Проверка связи", 1, true, GKProgressClientType.Administrator);
			try
			{
				var gkFileInfo = ReadInfoBlock(gkControllerDevice);
				if (Error != null)
					return null;
				var allbytes = new List<byte>();
				uint i = 2;
				progressCallback = GKProcessorManager.StartProgress("Чтение конфигурационного файла из " + gkControllerDevice.PresentationName, "", (int)(gkFileInfo.FileSize / 256), true, GKProgressClientType.Administrator);
				while (true)
				{
					if (progressCallback.IsCanceled)
					{ Error = "Операция отменена"; return null; }
					GKProcessorManager.DoProgress("Чтение блока данных " + i, progressCallback);

					var sendResultBytesCount = 256;
					for (int j = 0; j < 10; j++)
					{
						if (j == 9)
						{
							Error = "Невозможно прочитать блок данных " + i;
							return null;
						}
					}
					if (sendResultBytesCount < 256)
						break;
				}
				if (allbytes.Count == 0)
				{ Error = "Конфигурационный файл отсутствует"; return null; }

				var folderName = AppDataFolderHelper.GetFolder("TempServer");
				var configFileName = Path.Combine(folderName, "ConfigFromGK.fscp");
				if (Directory.Exists(folderName))
					Directory.Delete(folderName, true);
				Directory.CreateDirectory(folderName);
				var fileStream = new FileStream(configFileName, FileMode.CreateNew, FileAccess.ReadWrite);
				fileStream.Write(allbytes.ToArray(), 0, allbytes.Count);
				fileStream.Close();

				return configFileName;
			}
			catch (Exception e)
			{ Logger.Error(e, "GKDescriptorsWriter.WriteConfig"); Error = "Непредвиденная ошибка"; return null; }
			finally
			{
				if (progressCallback != null)
					GKProcessorManager.StopProgress(progressCallback);
			}
		}

		public void WriteFileToGK(GKDevice gkControllerDevice)
		{
			var gkFileInfo = new GKFileInfo();
			gkFileInfo.Initialize(GKManager.DeviceConfiguration, gkControllerDevice);

			var bytesList = new List<byte>();
			bytesList.AddRange(gkFileInfo.InfoBlock);
			bytesList.AddRange(gkFileInfo.FileBytes);
			var progressCallback = GKProcessorManager.StartProgress("Запись файла в " + gkControllerDevice.PresentationName, null, bytesList.Count / 256, false, GKProgressClientType.Administrator);
			for (var i = 0; i < bytesList.Count; i += 256)
			{
				GKProcessorManager.DoProgress("Запись блока данных " + i + 1, progressCallback);
				var bytesBlock = BitConverter.GetBytes((uint)(i / 256 + 1)).ToList();
				bytesBlock.AddRange(bytesList.GetRange(i, Math.Min(256, bytesList.Count - i)));
				for (int j = 0; j < 10; j++)
				{
					if (j == 9)
					{
						Error = "Невозможно записать блок данных " + i;
						return;
					}
				}
			}
			var endBlock = BitConverter.GetBytes((uint)(bytesList.Count() / 256 + 1)).ToList();
		}

		public GKFileInfo ReadInfoBlock(GKDevice gkControllerDevice)
		{
			try
			{
				if (GKFileInfo.Error != null)
				{ Error = GKFileInfo.Error; return null; }

				return new GKFileInfo();
			}
			catch (Exception e)
			{ Logger.Error(e, "GKDescriptorsWriter.WriteConfig"); return null; }
		}
	}
}