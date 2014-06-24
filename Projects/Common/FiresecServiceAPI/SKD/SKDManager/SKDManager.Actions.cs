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

		public static void AddDeviceToZone(SKDDevice device, SKDZone zone)
		{
			device.ZoneUID = zone.UID;
			device.Zone = zone;
			zone.Devices.Add(device);
			zone.OnChanged();
			device.OnChanged();
		}

		public static void EditZone(SKDZone zone)
		{
			foreach (var device in zone.Devices)
				device.OnChanged();
			zone.OnChanged();
		}

		public static void RemoveDeviceFromZone(SKDDevice device, SKDZone zone)
		{
			if (zone != null)
			{
				device.Zone = null;
				device.ZoneUID = Guid.Empty;
				zone.Devices.Remove(device);
				zone.OnChanged();
				device.OnChanged();
			}
		}
	}
}