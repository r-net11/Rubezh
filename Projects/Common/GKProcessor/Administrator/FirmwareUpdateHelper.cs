using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GKProcessor
{
	public class FirmwareUpdateHelper
	{
		public GKProgressCallback ProgressCallback { get; private set; }

		public string Update(GKDevice device, List<byte> firmWareBytes, string userName, Guid clientUID)
		{
			GKProcessorManager.AddGKMessage(JournalEventNameType.Обновление_ПО_прибора, JournalEventDescriptionType.NULL, "", device, userName);
			ProgressCallback = GKProcessorManager.StartProgress("Обновление прошивки " + device.PresentationName, "", firmWareBytes.Count / 256, false, GKProgressClientType.Administrator, clientUID);
			GKProcessorManager.DoProgress("Проверка связи " + device.PresentationName, ProgressCallback, clientUID);
			if (DeviceBytesHelper.Ping(device).HasError)
			{
				return "Устройство " + device.PresentationName + " недоступно";

			}
			if (!DeviceBytesHelper.GoToTechnologicalRegime(device, ProgressCallback, clientUID))
			{
				return "Не удалось перевести " + device.PresentationName + " в технологический режим\n" +
						"Устройство не доступно, либо вашего IP адреса нет в списке разрешенного адреса ГК";

			}
			GKProcessorManager.DoProgress("Удаление программы " + device.PresentationName, ProgressCallback, clientUID);
			if (!Clear(device))
			{
				return "Устройство " + device.PresentationName + " недоступно";

			}
			var data = new List<byte>();
			var offset = 0;
			if (device.Driver.IsKau)
				offset = 0x10000;
			for (int i = 0; i < firmWareBytes.Count; i = i + 0x100)
			{
				GKProcessorManager.DoProgress("Запись блока данных " + i / 0x100 + 1, ProgressCallback, clientUID);
				data = new List<byte>(BitConverter.GetBytes(i + offset));
				data.AddRange(firmWareBytes.GetRange(i, 0x100));
				for (int j = 0; j < 10; j++)
				{
					var result = SendManager.Send(device, 260, 0x12, 0, data, true, false, 10000);
					if (!result.HasError)
						break;
					if (j == 9)
					{
						return "В заданное времени не пришел ответ от устройства";
					}
				}
			}
			if (!DeviceBytesHelper.GoToWorkingRegime(device, ProgressCallback, clientUID))
			{
				return "Не удалось перевести " + device.PresentationName + " в рабочий режим\n" +
						"Устройство не доступно, либо вашего " +
						"IP адреса нет в списке разрешенного адреса ГК";
			}
			return null;
		}

		public static List<byte> HexFileToBytesList(string filePath)
		{
			var strings = File.ReadAllLines(filePath).ToList();
			strings.RemoveAt(0);
			strings.RemoveRange(strings.Count - 1, 1);
			return StringsToBytes(strings);
		}

		static List<byte> StringsToBytes(List<string> strings)
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

		bool Clear(GKDevice device)
		{
			var sendResult = SendManager.Send(device, 0, 16, 0, null, true, false, 4000);
			if (sendResult.HasError)
				return false;
			return true;
		}
	}
}