using System;
using System.Collections.Generic;
using System.Linq;
using FS2Api;
using ServerFS2.Helpers;
using ServerFS2.Service;
using Device = FiresecAPI.Models.Device;
using ServerFS2.Monitoring;

namespace ServerFS2
{
	public static partial class ServerHelper
	{
		public static event Action<int, int, string> Progress;

		public static void SynchronizeTime(Device device)
		{
			USBManager.SendCodeToPanel(device, 0x02, 0x11, DateConverter.ConvertToBytes(DateTime.Now));
		}

		public static List<byte> GetBytesFromFlashDB(Device device, int pointer, int count)
		{
			return USBManager.SendCodeToPanel(device, 0x01, 0x52, BitConverter.GetBytes(pointer).Reverse(), count - 1).Bytes;
		}
	}
}