using Common;
using Infrastructure.Common.Windows;
using RubezhAPI;
using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GKProcessor
{
	public class GKFileReaderWriter
	{
		public string Error { get; private set; }

		public OperationResult<string> ReadConfigFileFromGK(GKDevice gkControllerDevice, GKProgressCallback progressCallback, Guid clientUID)
		{
			progressCallback = GKProcessorManager.StartProgress("Чтение конфигурационного файла из " + gkControllerDevice.PresentationName, "Проверка связи", 1, true, GKProgressClientType.Administrator, clientUID);
			try
			{
				using (var gkLifecycleManager = new GKLifecycleManager(gkControllerDevice, "Чтение файла ГК"))
				{
					gkLifecycleManager.AddItem("Чтение информационного блока");
					var gkFileInfo = ReadInfoBlock(gkControllerDevice);
					if (Error != null)
					{
						gkLifecycleManager.AddItem("Ошибка чтения информационного блока");
						return OperationResult<string>.FromError("Ошибка чтения информационного блока");
					}

					var allbytes = new List<byte>();
					uint i = 2;
					progressCallback = GKProcessorManager.StartProgress("Чтение конфигурационного файла из " + gkControllerDevice.PresentationName, "", (int)(gkFileInfo.FileSize / 256), true, GKProgressClientType.Administrator, clientUID);
					while (true)
					{
						if (progressCallback.IsCanceled)
							return OperationResult<string>.FromError("Операция отменена");
						GKProcessorManager.DoProgress("Чтение блока данных " + i, progressCallback, clientUID);
						gkLifecycleManager.Progress((int)i, (int)(gkFileInfo.FileSize / 256));

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
			}
			catch (Exception e)
			{
				Logger.Error(e, "GKDescriptorsWriter.WriteConfig");
				return OperationResult<string>.FromError("Непредвиденная ошибка");
			}
			finally
			{
				if (progressCallback != null)
					GKProcessorManager.StopProgress(progressCallback, clientUID);
			}
		}

		public void WriteFileToGK(GKDevice gkControllerDevice, Guid clientUID)
		{
			using (var gkLifecycleManager = new GKLifecycleManager(gkControllerDevice, "Запись файла в ГК"))
			{
				gkLifecycleManager.AddItem("Формирование хэша");
				var gkFileInfo = new GKFileInfo();
				gkFileInfo.Initialize(gkControllerDevice);
				var bytesList = new List<byte>();
				bytesList.AddRange(gkFileInfo.InfoBlock);
				bytesList.AddRange(gkFileInfo.FileBytes);

				gkLifecycleManager.AddItem("Перевод в режим записи файла");
				var sendResult = SendManager.Send(gkControllerDevice, 0, 21, 0);
				if (sendResult.HasError)
				{
					Error = "Невозможно начать процедуру записи ";
					gkLifecycleManager.AddItem("Ошибка");
					return;
				}

				var progressCallback = GKProcessorManager.StartProgress("Запись файла в " + gkControllerDevice.PresentationName, null, bytesList.Count / 256, false, GKProgressClientType.Administrator, clientUID);
				for (var i = 0; i < bytesList.Count; i += 256)
				{
					gkLifecycleManager.Progress(i + 1, bytesList.Count);
					GKProcessorManager.DoProgress("Запись блока данных " + i + 1, progressCallback, clientUID);

					var bytesBlock = BitConverter.GetBytes((uint)(i / 256 + 1)).ToList();
					bytesBlock.AddRange(bytesList.GetRange(i, Math.Min(256, bytesList.Count - i)));
					for (int j = 0; j < 10; j++)
					{
						sendResult = SendManager.Send(gkControllerDevice, (ushort)bytesBlock.Count(), 22, 0, bytesBlock);
						var result = sendResult.Bytes.Count > 0 && sendResult.Bytes[0] == 1;
						if (!sendResult.HasError && result)
							break;
						if (j == 9)
						{
							Error = "Невозможно записать блок данных " + i;
							gkLifecycleManager.AddItem("Ошибка");
							return;
						}
					}
				}
				gkLifecycleManager.AddItem("Запись последнего блока данных");
				var endBlock = BitConverter.GetBytes((uint)(bytesList.Count() / 256 + 1)).ToList();
				sendResult = SendManager.Send(gkControllerDevice, 0, 22, 0, endBlock);
				var endResult = sendResult.Bytes.Count > 0 && sendResult.Bytes[0] == 0;
				if (sendResult.HasError || !endResult)
				{
					Error = "Невозможно завершить запись файла ";
					gkLifecycleManager.AddItem("Ошибка");
				}
				var sendResultRead = SendManager.Send(gkControllerDevice, 4, 23, 256, new List<byte>(BitConverter.GetBytes(1)));
				if (!gkFileInfo.InfoBlock.SequenceEqual(sendResultRead.Bytes))
				{
					Error = "Не удалось корректно записать информационный блок ";
					gkLifecycleManager.AddItem("Ошибка");
				}
			}
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