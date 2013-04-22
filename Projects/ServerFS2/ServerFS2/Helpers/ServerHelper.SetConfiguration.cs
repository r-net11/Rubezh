using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using ClientFS2.ConfigurationWriter;
using FiresecAPI;
using FiresecAPI.Models;
using ServerFS2.Helpers;
using Device = FiresecAPI.Models.Device;

namespace ServerFS2
{
	public static partial class ServerHelper
	{
		public static void SetDeviceRom(Device device)
		{
			var bytes = new List<byte>();
			bytes = new List<byte>();
			bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
			bytes.Add(Convert.ToByte(device.Parent.IntAddress + 2));
			bytes.Add(Convert.ToByte(device.AddressOnShleif));
			bytes.Add(0x02);
			bytes.Add(0x52);
			bytes.AddRange(BitConverter.GetBytes(0x100).Reverse());
			bytes.Add(Convert.ToByte(0xFF));
			//DeviceRom[0x5c] = 10;
			foreach (var b in DeviceRom)
			{
				if (bytes.Count < 64)
					bytes.Add(b);
				else
				{
					SendCode(bytes, 1000, 1000, true);
					bytes = new List<byte>();
				}
			}
			bytes.Add(0x3E);
			while (bytes.Count < 64)
				bytes.Add(0);
			SendCode(bytes, 1000, 1000, true);
		}

		public static void SetDeviceRam(Device device)
		{
			var bytes = new List<byte>();
			var begin = _deviceRamFirstIndex / 0x100;
			bytes = new List<byte>();
			bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
			bytes.Add(Convert.ToByte(device.Parent.IntAddress + 2));
			bytes.Add(Convert.ToByte(device.AddressOnShleif));
			bytes.Add(0x02);
			bytes.Add(0x52);
			bytes.AddRange(BitConverter.GetBytes((begin + 1) * 0x100).Reverse());
			bytes.Add(Convert.ToByte(0xFF));
			foreach (var b in DeviceRam)
			{
				if (bytes.Count < 64)
					bytes.Add(b);
				else
				{
					SendCode(bytes, 1000, 1000, true);
					bytes = new List<byte>();
				}
			}
			bytes.Add(0x3E);
			while (bytes.Count < 64)
				bytes.Add(0);
			SendCode(bytes, 1000, 1000, true);
		}
	}
}