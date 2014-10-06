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

		public GKDeviceConfiguration ReadConfigFileFromGK(GKDevice gkControllerDevice)
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
					var data = new List<byte>(BitConverter.GetBytes(i++));
					var sendResult = SendManager.Send(gkControllerDevice, 4, 23, 256, data);
					if (sendResult.HasError)
					{ Error = "Невозможно прочитать блок данных " + i; return null; }
					allbytes.AddRange(sendResult.Bytes);
					if (sendResult.Bytes.Count() < 256)
						break;
				}
				if (allbytes.Count == 0)
				{ Error = "Конфигурационный файл отсутствует"; return null; }

				var deviceConfiguration = ZipFileConfigurationHelper.UnZipFromStream(new MemoryStream(allbytes.ToArray()));
				if (ZipFileConfigurationHelper.Error != null)
				{ Error = ZipFileConfigurationHelper.Error; return null; }
				return deviceConfiguration;
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
			var sendResult = SendManager.Send(gkControllerDevice, 0, 21, 0);
			if (sendResult.HasError)
			{ Error = "Невозможно начать процедуру записи "; return; }
			bytesList.AddRange(gkFileInfo.FileBytes);
			var progressCallback = GKProcessorManager.StartProgress("Запись файла в " + gkControllerDevice.PresentationName, null, bytesList.Count / 256, true, GKProgressClientType.Administrator);
			for (var i = 0; i < bytesList.Count; i += 256)
			{
				if (progressCallback.IsCanceled)
				{ Error = "Операция отменена"; return; }
				GKProcessorManager.DoProgress("Запись блока данных " + i + 1, progressCallback);
				var bytesBlock = BitConverter.GetBytes((uint)(i / 256 + 1)).ToList();
				bytesBlock.AddRange(bytesList.GetRange(i, Math.Min(256, bytesList.Count - i)));
				sendResult = SendManager.Send(gkControllerDevice, (ushort)bytesBlock.Count(), 22, 0, bytesBlock);
				if (sendResult.HasError)
				{
					Error = "Невозможно записать блок данных " + i;
					break;
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