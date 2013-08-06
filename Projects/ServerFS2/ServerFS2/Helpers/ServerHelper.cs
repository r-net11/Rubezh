using System;
using System.Collections.Generic;
using System.Linq;
using ServerFS2.Helpers;
using Device = FiresecAPI.Models.Device;

namespace ServerFS2
{
	public static partial class ServerHelper
	{
		public static void SynchronizeTime(Device device)
		{
			USBManager.Send(device, "Синхронизация времени",  0x02, 0x11, DateConverter.ConvertToBytes(DateTime.Now));
		}

		public static List<byte> GetBytesFromFlashDB(Device device, int pointer, int count)
		{
			var response = USBManager.Send(device, "Чтение отдельных байт из FLASH", 0x01, 0x52, BitConverter.GetBytes(pointer).Reverse(), count - 1);
			return response.Bytes;
		}

		public static List<byte> GetBytesFromRomDB(Device device, int pointer, int count)
		{
			var response = USBManager.Send(device, "Чтение отдельных байт из ROM", 0x38, BitConverter.GetBytes(pointer).Reverse(), count - 1);
			return response.Bytes;
		}
	}
}