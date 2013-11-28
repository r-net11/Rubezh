using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using XFiresecAPI;

namespace GKProcessor
{
	public static class GKFileReaderWriter
	{
		public static string ParsingError { get; private set; }

		public static XDeviceConfiguration ReadConfigFileFromGK()
		{
			var gkDevice = XManager.Devices.FirstOrDefault(x => x.DriverType == XDriverType.GK);
			if (gkDevice == null)
			{ ParsingError = "Ошибка конфигурации, не найден ГК в конфигурации"; return null; }
			var allbytes = new List<byte>();
			uint i = 1;
			LoadingService.Show("Чтение конфигурационного файла из " + gkDevice.PresentationName);
			while (true)
			{
				LoadingService.DoStep("Чтение объекта " + i);
				var data = new List<byte>(BitConverter.GetBytes(i++));
				var sendResult = SendManager.Send(gkDevice, 4, 23, 256, data);
				if (sendResult.HasError)
				{ ParsingError = "Невозможно прочитать блок данных " + i; break; }
				allbytes.AddRange(sendResult.Bytes);
				if (sendResult.Bytes.Count() < 256)
					break;
			}
			var deviceConfiguration = ZipFileConfigurationHelper.LoadFromZipFile(new MemoryStream(allbytes.ToArray()));
			deviceConfiguration.Devices.ForEach(x => x.Driver = XManager.Drivers.FirstOrDefault(y => y.UID == x.DriverUID));
			foreach (var zone in deviceConfiguration.Zones)
			{
				var device = deviceConfiguration.Devices.FirstOrDefault(x => x.ZoneUIDs.Contains(zone.UID));
				if (device != null)
					zone.GkDatabaseParent = device.GKParent;
			}

			UpdateConfigurationHelper.Update(deviceConfiguration);
			UpdateConfigurationHelper.PrepareDescriptors(deviceConfiguration);
			LoadingService.Close();
			return deviceConfiguration;
		}

		public static void WriteConfigFileToGK()
		{
			var gkDevice = XManager.Devices.FirstOrDefault(x => x.DriverType == XDriverType.GK);
			var folderName = AppDataFolderHelper.GetLocalFolder("Administrator/Configuration");
			var configFileName = Path.Combine(folderName, "fileToGk.fscp");

			ZipFileConfigurationHelper.SaveToZipFile(configFileName, XManager.DeviceConfiguration);
			if (!File.Exists(configFileName))
				return;
			var bytesList = File.ReadAllBytes(configFileName).ToList();
			var tempBytes = new List<List<byte>>();
			var sendResult = SendManager.Send(gkDevice, 0, 21, 0);
			if (sendResult.HasError)
			{ ParsingError = "Невозможно начать процедуру записи "; return; }
			LoadingService.Show("Запись конфигурационного файла в " + gkDevice.PresentationName, null, bytesList.Count);
			for (int i = 0; i < bytesList.Count(); i += 256)
			{
				LoadingService.SaveDoStep("Запись объекта " + i + 1);
				var bytesBlock = BitConverter.GetBytes((uint)(i / 256 + 1)).ToList();
				bytesBlock.AddRange(bytesList.GetRange(i, Math.Min(256, bytesList.Count - i)));
				tempBytes.Add(bytesBlock.GetRange(4, bytesBlock.Count - 4));
				sendResult = SendManager.Send(gkDevice, (ushort)bytesBlock.Count(), 22, 0, bytesBlock);
				if (sendResult.HasError)
				{ ParsingError = "Невозможно записать блок данных " + i; return; }
			}
			var endBlock = BitConverter.GetBytes((uint)(bytesList.Count() / 256 + 1)).ToList();
			sendResult = SendManager.Send(gkDevice, 0, 22, 0, endBlock);
			if (sendResult.HasError)
			{ ParsingError = "Невозможно завершить запись файла "; }
		}
	}
}