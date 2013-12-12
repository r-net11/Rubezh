using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using System.IO;

namespace GKProcessor
{
	public static class FirmwareUpdateHelper
	{
		public static string Error { get; private set; }
		public static void Update(XDevice device, string fileName)
		{
			var firmWareBytes = HexFileToBytesList(fileName);
			if (!DeviceBytesHelper.GoToTechnologicalRegime(device))
			{
				Error = "Не удалось перевести " + device.PresentationName + " в технологический режим\n" +
						"Устройство не доступно, либо вашего " +
						"IP адреса нет в списке разрешенного адреса ГК";
				GKProcessorManager.OnStopProgress();
				return ;
			}
			var softVersion = DeviceBytesHelper.GetDeviceInfo(device);
			DeviceBytesHelper.Clear(device);
			var data = new List<byte>();
			var offset = 0;
			if (device.Driver.IsKauOrRSR2Kau)
				offset = 0x10000;
			for (int i = 0; i < firmWareBytes.Count; i = i + 0x100)
			{
				data = new List<byte>(BitConverter.GetBytes(i + offset));
				data.AddRange(firmWareBytes.GetRange(i, 0x100));
				SendManager.Send(device, 260, 0x12, 0, data);
			}
			DeviceBytesHelper.GoToWorkingRegime(device);
		}

		//public static void Update(XDevice device, string fileName)
		//{
		//    var firmWareBytes = HexFileToBytesList(fileName);
		//    var data = new List<byte>();
		//    var allBytes = new List<List<byte>>();
		//    var offset = 0;
		//    if (device.Driver.IsKauOrRSR2Kau)
		//        offset = 0x10000;
		//    for (int i = 0; i < firmWareBytes.Count; i = i + 0x100)
		//    {
		//        data = new List<byte>(BitConverter.GetBytes(i + offset));
		//        data.AddRange(firmWareBytes.GetRange(i, 0x100));
		//        allBytes.Add(data);
		//    }
		//    BytesHelper.BytesToFile("1.txt", allBytes);
		//}

		static List<byte> HexFileToBytesList(string filePath)
		{
			var bytes = new List<byte>();
			var strings = File.ReadAllLines(filePath).ToList();
			strings.RemoveAt(0);
			strings.RemoveRange(strings.Count - 1, 1);
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
	}
}