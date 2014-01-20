using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using FiresecAPI;

namespace GKProcessor
{
	public class GKFileReaderWriter
	{
		public string Error { get; private set; }

		public XDeviceConfiguration ReadConfigFileFromGK(XDevice gkDevice)
		{
			try
			{
				var gkFileInfo = ReadInfoBlock(gkDevice);
				if (Error != null)
					return null;
				var allbytes = new List<byte>();
				uint i = 2;
				GKProcessorManager.OnStartProgress("Чтение конфигурационного файла из " + gkDevice.PresentationName, "", gkFileInfo.DescriptorsCount, true, GKProgressClientType.Administrator);
				while (true)
				{
					if (GKProcessorManager.IsProgressCanceled)
						{ Error = "Операция отменена"; return null; }
					GKProcessorManager.OnDoProgress("Чтение блока данных " + i);
					var data = new List<byte>(BitConverter.GetBytes(i++));
					var sendResult = SendManager.Send(gkDevice, 4, 23, 256, data);
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
				UpdateConfigurationHelper.Update(deviceConfiguration);
				UpdateConfigurationHelper.PrepareDescriptors(deviceConfiguration);
				return deviceConfiguration;
			}
			catch (Exception e)
			{ Logger.Error(e, "GKDescriptorsWriter.WriteConfig"); Error = "Непредвиденная ошибка"; return null; }
			finally
			{ GKProcessorManager.OnStopProgress(); }
		}

		public void WriteFileToGK(XDevice gkDevice)
		{
			var bytesList = new List<byte>();
			var gkFileInfo = new GKFileInfo();
			gkFileInfo.Initialize(XManager.DeviceConfiguration, gkDevice);
			bytesList.AddRange(gkFileInfo.InfoBlock);
			var sendResult = SendManager.Send(gkDevice, 0, 21, 0);
			if (sendResult.HasError)
				{ Error = "Невозможно начать процедуру записи "; return; }
            bytesList.AddRange(gkFileInfo.FileBytes);
			GKProcessorManager.OnStartProgress("Запись файла в " + gkDevice.PresentationName, null, bytesList.Count / 256, true, GKProgressClientType.Administrator);
			for (var i = 0; i < bytesList.Count; i += 256)
			{
				if (GKProcessorManager.IsProgressCanceled)
					{ Error = "Операция отменена"; return; }
				GKProcessorManager.OnDoProgress("Запись блока данных " + i + 1);
				var bytesBlock = BitConverter.GetBytes((uint)(i / 256 + 1)).ToList();
				bytesBlock.AddRange(bytesList.GetRange(i, Math.Min(256, bytesList.Count - i)));
				sendResult = SendManager.Send(gkDevice, (ushort)bytesBlock.Count(), 22, 0, bytesBlock);
				if (sendResult.HasError)
				{
					Error = "Невозможно записать блок данных " + i;
					break;
				}
			}
			var endBlock = BitConverter.GetBytes((uint) (bytesList.Count()/256 + 1)).ToList();
			sendResult = SendManager.Send(gkDevice, 0, 22, 0, endBlock);
			if (sendResult.HasError)
				{ Error = "Невозможно завершить запись файла "; }
		}

		public GKFileInfo ReadInfoBlock(XDevice gkDevice)
		{
			try
			{
				var data = new List<byte>(BitConverter.GetBytes(1));
				var sendResult = SendManager.Send(gkDevice, 4, 23, 256, data);
				if (sendResult.HasError)
					{ Error = "Невозможно прочитать информационный блок"; return null; }
				if (sendResult.Bytes.Count == 0)
					{ Error = "Информационный блок отсутствует"; return null; }
				if (sendResult.Bytes.Count < 256)
					{ Error = "Информационный блок поврежден"; return null; }
				GKProcessorManager.OnStopProgress();
				var infoBlock = GKFileInfo.BytesToGKFileInfo(sendResult.Bytes);
				if (GKFileInfo.Error != null)
					{ Error = GKFileInfo.Error; return null; }
				return infoBlock;
			}
			catch (Exception e)
				{ Logger.Error(e, "GKDescriptorsWriter.WriteConfig"); return null; }
			finally
			{ GKProcessorManager.OnStopProgress(); }
		}
	}
}