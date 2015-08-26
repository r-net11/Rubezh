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

		public OperationResult<string> ReadConfigFileFromGK(GKDevice gkControllerDevice, GKProgressCallback progressCallback)
		{
			progressCallback = GKProcessorManager.StartProgress("Чтение конфигурационного файла из " + gkControllerDevice.PresentationName, "Проверка связи", 1, true, GKProgressClientType.Administrator);
			try
			{
				var gkFileInfo = ReadInfoBlock(gkControllerDevice);
				if (Error != null)
					return OperationResult<string>.FromError("Ошибка чтения информационного блока");
				var allbytes = new List<byte>();
				uint i = 2;
				progressCallback = GKProcessorManager.StartProgress("Чтение конфигурационного файла из " + gkControllerDevice.PresentationName, "", (int)(gkFileInfo.FileSize / 256), true, GKProgressClientType.Administrator);
				while (true)
				{
					if (progressCallback.IsCanceled)
						return OperationResult<string>.FromError("Операция отменена");
					GKProcessorManager.DoProgress("Чтение блока данных " + i, progressCallback);
					var data = new List<byte>(BitConverter.GetBytes(i++));
					var sendResultBytesCount = 256;
					for (int j = 0; j < 10; j++)
					{
						var sendResult = SendManager.Send(gkControllerDevice, 4, 23, 256, data);
						allbytes.AddRange(sendResult.Bytes);
						sendResultBytesCount = sendResult.Bytes.Count();
						if (!sendResult.HasError)
							break;
						if (j == 9)
						{
							return OperationResult<string>.FromError("Невозможно прочитать блок данных " + i);
						}
					}
					if (sendResultBytesCount < 256)
						break;
				}
				if (allbytes.Count == 0)
					return OperationResult<string>.FromError("Конфигурационный файл отсутствует");

				var folderName = AppDataFolderHelper.GetFolder("TempServer");
				var configFileName = Path.Combine(folderName, "ConfigFromGK.fscp");
				if (Directory.Exists(folderName))
					Directory.Delete(folderName, true);
				Directory.CreateDirectory(folderName);
				var fileStream = new FileStream(configFileName, FileMode.CreateNew, FileAccess.ReadWrite);
				fileStream.Write(allbytes.ToArray(), 0, allbytes.Count);
				fileStream.Close();

				return new OperationResult<string>(configFileName);
			}
			catch (Exception e)
			{
				Logger.Error(e, "GKDescriptorsWriter.WriteConfig");
				return OperationResult<string>.FromError("Непредвиденная ошибка"); 
			}
			finally
			{
				if (progressCallback != null)
					GKProcessorManager.StopProgress(progressCallback);
			}
		}

		public void WriteFileToGK(GKDevice gkControllerDevice)
		{
			var gkFileInfo = new GKFileInfo();
			gkFileInfo.Initialize(gkControllerDevice);

			var bytesList = new List<byte>();
			bytesList.AddRange(gkFileInfo.InfoBlock);
			var sendResult = SendManager.Send(gkControllerDevice, 0, 21, 0);
			if (sendResult.HasError)
			{ Error = "Невозможно начать процедуру записи "; return; }
			bytesList.AddRange(gkFileInfo.FileBytes);
			var progressCallback = GKProcessorManager.StartProgress("Запись файла в " + gkControllerDevice.PresentationName, null, bytesList.Count / 256, false, GKProgressClientType.Administrator);
			for (var i = 0; i < bytesList.Count; i += 256)
			{
				GKProcessorManager.DoProgress("Запись блока данных " + i + 1, progressCallback);
				var bytesBlock = BitConverter.GetBytes((uint)(i / 256 + 1)).ToList();
				bytesBlock.AddRange(bytesList.GetRange(i, Math.Min(256, bytesList.Count - i)));
				for (int j = 0; j < 10; j++)
				{
					sendResult = SendManager.Send(gkControllerDevice, (ushort)bytesBlock.Count(), 22, 0, bytesBlock);
					if (!sendResult.HasError)
						break;
					if (j == 9)
					{
						Error = "Невозможно записать блок данных " + i;
						return;
					}
				}
			}
			var endBlock = BitConverter.GetBytes((uint)(bytesList.Count() / 256 + 1)).ToList();
			sendResult = SendManager.Send(gkControllerDevice, 0, 22, 0, endBlock);
			if (sendResult.HasError)
			{ Error = "Невозможно завершить запись файла "; }
		}

		public GKFileInfo ReadInfoBlock(GKDevice gkControllerDevice)
		{
			try
			{
				var data = new List<byte>(BitConverter.GetBytes(1));
				var sendResult = SendManager.Send(gkControllerDevice, 4, 23, 256, data);
				if (sendResult.HasError)
				{ Error = "Устройство недоступно"; return null; }
				if (sendResult.Bytes.Count == 0)
				{ Error = "Информационный блок отсутствует"; return null; }
				if (sendResult.Bytes.Count < 256)
				{ Error = "Информационный блок поврежден"; return null; }
				var infoBlock = GKFileInfo.BytesToGKFileInfo(sendResult.Bytes);
				if (GKFileInfo.Error != null)
				{ Error = GKFileInfo.Error; return null; }
				return infoBlock;
			}
			catch (Exception e)
			{ Logger.Error(e, "GKDescriptorsWriter.WriteConfig"); return null; }
		}
	}
}