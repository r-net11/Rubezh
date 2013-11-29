using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using System.IO;

namespace GKProcessor
{
	public static class FirmwareUpdateHelper
	{
		public static void Update(XDevice device, string fileName)
		{
			var firmWareBytes = HexFileToBytesList(fileName);
			DeviceBytesHelper.GoToTechnologicalRegime(device);
			var softVersion = DeviceBytesHelper.GetDeviceInfo(device);
			DeviceBytesHelper.Clear(device);
			var data = new List<byte>();
			for (int i = 0; i < firmWareBytes.Count; i = i + 0x100)
			{
				data = new List<byte>(BitConverter.GetBytes((i + 1) * 0x100));
				data.Reverse();
				data.AddRange(firmWareBytes.GetRange(i, 0x100));
				SendManager.Send(device, 260, 0x12, 0, data);
			}
			DeviceBytesHelper.GoToWorkingRegime(device);
		}

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