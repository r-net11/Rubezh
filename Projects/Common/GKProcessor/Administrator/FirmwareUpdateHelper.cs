using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Journal;

namespace GKProcessor
{
	public class FirmwareUpdateHelper
	{
		public List<string> ErrorList = new List<string>();
		string Error;
		public GKProgressCallback ProgressCallback { get; private set; }

		public void Update(XDevice device, string fileName, string userName)
		{
			var firmWareBytes = HexFileToBytesList(fileName);
			Update(device, firmWareBytes, userName);
			GKProcessorManager.StopProgress(ProgressCallback);
			if (Error != null)
				ErrorList.Add(Error);
		}

		public void Update(XDevice device, List<byte> firmWareBytes, string userName)
		{
			GKProcessorManager.AddGKMessage(JournalEventNameType.Обновление_ПО_прибора, "", device, userName, true);
			ProgressCallback = GKProcessorManager.StartProgress("Обновление прошивки " + device.PresentationName, "", firmWareBytes.Count / 256, true, GKProgressClientType.Administrator);
			GKProcessorManager.DoProgress("Проверка связи " + device.PresentationName, ProgressCallback);
			if (!DeviceBytesHelper.Ping(device))
			{
				Error = "Устройство " + device.PresentationName + " недоступно";
				return;
			}
			if (!DeviceBytesHelper.GoToTechnologicalRegime(device, ProgressCallback))
			{
				Error = "Не удалось перевести " + device.PresentationName + " в технологический режим\n" +
						"Устройство не доступно, либо вашего " +
						"IP адреса нет в списке разрешенного адреса ГК";
				return;
			}
			DeviceBytesHelper.GetDeviceInfo(device);
			GKProcessorManager.DoProgress("Удаление программы " + device.PresentationName, ProgressCallback);
			if (!Clear(device))
			{
				Error = "Устройство " + device.PresentationName + " недоступно";
				return;
			}
			var data = new List<byte>();
			var offset = 0;
			if (device.Driver.IsKauOrRSR2Kau)
				offset = 0x10000;
			for (int i = 0; i < firmWareBytes.Count; i = i + 0x100)
			{
				if (ProgressCallback.IsCanceled)
				{ Error = "Операция обновления прибора " + device.PresentationName + " отменена"; return; }
				GKProcessorManager.DoProgress("Запись блока данных " + i / 0x100 + 1, ProgressCallback);
				data = new List<byte>(BitConverter.GetBytes(i + offset));
				data.AddRange(firmWareBytes.GetRange(i, 0x100));
				var result = SendManager.Send(device, 260, 0x12, 0, data, true, false, 10000);
				if (result.HasError)
				{ Error = "В заданное времени не пришел ответ от устройства"; return; }
			}
			if (!DeviceBytesHelper.GoToWorkingRegime(device, ProgressCallback))
			{
				Error = "Не удалось перевести " + device.PresentationName + " в рабочий режим\n" +
						"Устройство не доступно, либо вашего " +
						"IP адреса нет в списке разрешенного адреса ГК";
				return;
			}
		}

		public void UpdateFSCS(HexFileCollectionInfo hxcFileInfo, List<XDevice> devices, string userName)
		{
			foreach (var device in devices)
			{
				var fileInfo = hxcFileInfo.HexFileInfos.FirstOrDefault(x => x.DriverType == device.DriverType);
				if (fileInfo == null)
				{
					Error = "Не найден файл прошивки для устройства типа " + device.DriverType;
					return;
				}
				var bytes = StringsToBytes(fileInfo.Lines);
				Update(device, bytes, userName);
				GKProcessorManager.StopProgress(ProgressCallback);
				if (Error != null)
					ErrorList.Add(Error);
				Error = null;
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

		bool Clear(XDevice device)
		{
			var sendResult = SendManager.Send(device, 0, 16, 0, null, true, false, 4000);
			if (sendResult.HasError)
				return false;
			return true;
		}
	}
}