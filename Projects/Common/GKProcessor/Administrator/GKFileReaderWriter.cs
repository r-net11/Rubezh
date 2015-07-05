using System;
using System.Collections.Generic;
using System.IO;
using Common;
using FiresecAPI;
using FiresecAPI.GK;
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