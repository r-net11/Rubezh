using System.Collections.Generic;
using System.IO;
using System.Linq;
using FiresecAPI.Models;

namespace ServerFS2.Helpers
{
	public static class FileDBHelper
	{
		public static List<byte> GetRomDBFromFS1File(Device parentPanel)
		{
			var fileName = @"C:\Program Files\Firesec\TstData\" + parentPanel.Driver.ShortName + " - " + GetAddress(parentPanel) + "_rom.bin";
			if (File.Exists(fileName))
			{
				var byteArray = File.ReadAllBytes(fileName);
				if (byteArray != null)
				{
					var bytes = byteArray.ToList();
					return bytes;
				}
			}
			return new List<byte>();
		}

		public static List<byte> GetFlashDBFromFS1File(Device parentPanel)
		{
			var fileName = @"C:\Program Files\Firesec\TstData\" + parentPanel.Driver.ShortName + " - " + GetAddress(parentPanel) + "_flash.bin";
			if (File.Exists(fileName))
			{
				var byteArray = File.ReadAllBytes(fileName);
				if (byteArray != null)
				{
					var bytes = byteArray.ToList();
					return bytes;
				}
			}
			return new List<byte>();
		}

		static string GetAddress(Device device)
		{
			var result = device.DottedAddress;
			if (device.Parent.Driver.DriverType == DriverType.Computer)
				result = "USB " + result;
			return result;
		}
	}
}