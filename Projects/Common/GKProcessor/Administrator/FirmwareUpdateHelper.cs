using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using System.IO;
using FiresecAPI;
namespace GKProcessor
{
	public class FirmwareUpdateHelper
	{
		public List<string> ErrorList = new List<string>();
		string Error;
		public void Update(XDevice device, string fileName)
		{
			var firmWareBytes = HexFileToBytesList(fileName);
			Update(device, firmWareBytes);
			if (Error != null)
				ErrorList.Add(Error);
		}

		public void Update(XDevice device, List<byte> firmWareBytes)
		{
			var progressCallback = GKProcessorManager.OnStartProgress("Обновление прошивки " + device.PresentationName, "", firmWareBytes.Count / 256, true, GKProgressClientType.Administrator);
			GKProcessorManager.OnDoProgress("Опрос устройства " + device.PresentationName, progressCallback);
			if (!DeviceBytesHelper.Ping(device))
			{
				Error = "Устройство " + device.PresentationName + " недоступно";
				GKProcessorManager.OnStopProgress(progressCallback);
				return;
			}
			if (!DeviceBytesHelper.GoToTechnologicalRegime(device, progressCallback))
			{
				Error = "Не удалось перевести " + device.PresentationName + " в технологический режим\n" +
						"Устройство не доступно, либо вашего " +
						"IP адреса нет в списке разрешенного адреса ГК";
				GKProcessorManager.OnStopProgress(progressCallback);
				return;
			}
			var softVersion = DeviceBytesHelper.GetDeviceInfo(device);
			GKProcessorManager.OnDoProgress("Удаление программы " + device.PresentationName, progressCallback);
			Clear(device);
			var data = new List<byte>();
			var offset = 0;
			if (device.Driver.IsKauOrRSR2Kau)
				offset = 0x10000;
			for (int i = 0; i < firmWareBytes.Count; i = i + 0x100)
			{
				if (progressCallback.IsCanceled)
				{ Error = "Операция обновления прошивки отменена"; GKProcessorManager.OnStopProgress(progressCallback); return; }
				GKProcessorManager.OnDoProgress("Запись блока данных " + i / 0x100 + 1, progressCallback);
				data = new List<byte>(BitConverter.GetBytes(i + offset));
				data.AddRange(firmWareBytes.GetRange(i, 0x100));
				var result = SendManager.Send(device, 260, 0x12, 0, data, true, false, 10000);
				if (result.HasError)
				{ Error = "В заданное времени не пришел ответ от устройства"; GKProcessorManager.OnStopProgress(progressCallback); return; }
			}
			if (!DeviceBytesHelper.GoToWorkingRegime(device, progressCallback))
			{
				Error = "Не удалось перевести " + device.PresentationName + " в технологический режим\n" +
						"Устройство не доступно, либо вашего " +
						"IP адреса нет в списке разрешенного адреса ГК";
				GKProcessorManager.OnStopProgress(progressCallback);
				return;
			}
			GKProcessorManager.OnStopProgress(progressCallback);
		}

		public void UpdateFSCS(HexFileCollectionInfo hxcFileInfo, string userName, List<XDevice> devices)
		{
			foreach (var device in devices)
			{
				var fileInfo = new HEXFileInfo();
				if (device.DriverType == XDriverType.GK)
					fileInfo = hxcFileInfo.HexFileInfos.FirstOrDefault(x => x.DriverType == XDriverType.GK);
				if (device.DriverType == XDriverType.KAU)
					fileInfo = hxcFileInfo.HexFileInfos.FirstOrDefault(x => x.DriverType == XDriverType.KAU);
				if (device.DriverType == XDriverType.RSR2_KAU)
					fileInfo = hxcFileInfo.HexFileInfos.FirstOrDefault(x => x.DriverType == XDriverType.RSR2_KAU);
				if (fileInfo == null)
					return;
				var bytes = StringsToBytes(fileInfo.Lines);
				Update(device, bytes);
				if (Error != null)
					ErrorList.Add(Error);
				GKProcessorManager.AddGKMessage("Обновление ПО прибора", "", XStateClass.Info, device, userName, true);
			}
		}

		List<byte> HexFileToBytesList(string filePath)
		{
			var strings = File.ReadAllLines(filePath).ToList();
			strings.RemoveAt(0);
			strings.RemoveRange(strings.Count - 1, 1);
			return StringsToBytes(strings);
		}

		List<byte> StringsToBytes(List<string> strings)
		{
			var bytes = new List<byte>();
			foreach (var str in strings)
			{
				var count = Convert.ToInt32(str.Substring(1, 2), 16);
				if (count != 0x10)
					continue;
				for (var i = 9; i < count * 2 + 9; i += 2)
				{
					bytes.Add(Convert.ToByte(str.Substring(i, 2), 16));
				}
			}
			return bytes;
		}

		public bool Clear(XDevice device)
		{
			var sendResult = SendManager.Send(device, 0, 16, 0, null, true, false, 4000);
			if (sendResult.HasError)
			{
				Error = "Устройство " + device.PresentationName + " недоступно";
				return false;
			}
			return true;
		}
	}
}