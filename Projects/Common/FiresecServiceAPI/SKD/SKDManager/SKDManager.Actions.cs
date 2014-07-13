using System;
using System.Collections.Generic;

namespace FiresecAPI.SKD
{
	public partial class SKDManager
	{
		public static List<byte> CreateHash()
		{
			return new List<byte>();
		}

		public static bool CompareHashes(List<byte> hash1, List<byte> hash2)
		{
			return true;
		}

		public static void EditZone(SKDZone zone)
		{
			foreach (var device in zone.Devices)
				device.OnChanged();
			zone.OnChanged();
		}

		public static void RemoceZone(SKDZone zone)
		{
			foreach (var device in zone.Devices)
			{
				device.Zone = null;
				device.ZoneUID = Guid.Empty;
				device.OnChanged();
			}
			Zones.Remove(zone);
		}

		public static void EditDevice(SKDDevice device)
		{
			if (device.Door != null)
			{
				device.Door.OnChanged();
			}
		}

		public static void DeleteDevice(SKDDevice device)
		{
			Devices.Remove(device);
			if (device.Door != null)
			{
				device.Door.InDeviceUID = Guid.Empty;
				device.Door.OutDeviceUID = Guid.Empty;
				device.Door.OnChanged();
			}
		}
	}
}