using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using XFiresecAPI;

namespace GKProcessor
{
	public static class GKFileReaderWriter
	{
		public static string Error { get; private set; }

		public static XDeviceConfiguration ReadConfigFileFromGK(XDevice gkDevice)
		{
			try
			{
				var gkFileInfo = ReadInfoBlock(gkDevice);
				var allbytes = new List<byte>();
				uint i = 2;
				LoadingService.Show("Чтение конфигурационного файла из " + gkDevice.PresentationName, "", gkFileInfo.DescriptorsCount, true);
				while (true)
				{
					if(LoadingService.IsCanceled)
						{ Error = "Операция отменена"; return null; }
					LoadingService.DoStep("Чтение блока данных " + i);
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
				var configMemoryStream = ZipSerializeHelper.Serialize(XManager.DeviceConfiguration);
				configMemoryStream.Position = 0;

				var deviceConfiguration = ZipFileConfigurationHelper.UnZipFromStream(new MemoryStream(allbytes.ToArray()));
				if (!String.IsNullOrEmpty(ZipFileConfigurationHelper.Error))
					{ Error = ZipFileConfigurationHelper.Error; return null; }
				UpdateConfigurationHelper.Update(deviceConfiguration);
				UpdateConfigurationHelper.PrepareDescriptors(deviceConfiguration);
				LoadingService.Close();
				return deviceConfiguration;
			}
			catch (Exception e)
				{ Logger.Error(e, "GKDescriptorsWriter.WriteConfig"); return null; }
			finally
				{ LoadingService.Close();}
		}

		public static void WriteFileToGK(XDevice gkDevice, bool writeConfig)
		{
			var bytesList = new List<byte>();
			var gkFileInfo = new GKFileInfo();
			gkFileInfo.Initialize(0, 1, XManager.DeviceConfiguration, gkDevice);
			bytesList.AddRange(gkFileInfo.InfoBlock);
			var sendResult = SendManager.Send(gkDevice, 0, 21, 0);
			if (sendResult.HasError)
				{ Error = "Невозможно начать процедуру записи "; return; }
			if (writeConfig)
				bytesList.AddRange(gkFileInfo.FileBytes);
			LoadingService.Show("Запись файла в " + gkDevice.PresentationName, null, bytesList.Count / 256, true);
			for (var i = 0; i < bytesList.Count(); i += 256)
			{
				if (LoadingService.IsCanceled)
					{ Error = "Операция отменена"; return; }
				LoadingService.DoStep("Запись блока данных " + i + 1);
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

		public static GKFileInfo ReadInfoBlock(XDevice gkDevice)
		{
			try
			{
				LoadingService.Show("Чтение информационного блока " + gkDevice.PresentationName);
				var data = new List<byte>(BitConverter.GetBytes(1));
				var sendResult = SendManager.Send(gkDevice, 4, 23, 256, data);
				if (sendResult.HasError)
					{ Error = "Невозможно прочитать информационный блок"; return null; }
				if (sendResult.Bytes.Count == 0)
					{ Error = "Информационный блок отсутствует"; return null; }
				LoadingService.Close();
				return GKFileInfo.InfoBlockToGKInfo(sendResult.Bytes);
			}
			catch (Exception e)
				{ Logger.Error(e, "GKDescriptorsWriter.WriteConfig"); return null; }
			finally
				{ LoadingService.Close(); }
		}
	}
}