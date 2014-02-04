using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;

namespace SKDDriver
{
	public static class DeviceBytesHelper
	{
		public static string GetInfo(SKDDevice device)
		{
			var bytes = new List<byte>();
			bytes.Add(1);
			var result = SKDDeviceProcessor.SendBytes(device, bytes);
			if (result.HasError)
				return result.Error;
			return "Версия " + result.Bytes[0].ToString();
		}

		public static bool SynchroniseTime(SKDDevice device)
		{
			var bytes = new List<byte>();
			bytes.Add(2);
			var result = SKDDeviceProcessor.SendBytes(device, bytes);
			return !result.HasError;
		}
	}
}