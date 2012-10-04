using System.Collections.Generic;
using System.Linq;
using XFiresecAPI;
using System;

namespace FiresecClient
{
	public partial class XManager
	{
		public static void ChangeDeviceZones(XDevice device, List<Guid> zoneUIDs)
		{
			foreach (var zone in device.Zones)
			{
				zone.Devices.Remove(device);
				zone.OnChanged();
			}
			device.Zones.Clear();

			foreach (var zoneUID in zoneUIDs)
			{
				var zone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == zoneUID);
				{
					if (zone != null)
					{
						device.Zones.Add(zone);
						zone.Devices.Add(device);
						zone.OnChanged();
					}
				}
			}
			device.ZoneUIDs = zoneUIDs;
			device.OnChanged();
		}

		public static void AddDeviceToZone(XDevice device, XZone zone)
		{
			if (!device.Zones.Contains(zone))
				device.Zones.Add(zone);
			if (!device.ZoneUIDs.Contains(zone.UID))
				device.ZoneUIDs.Add(zone.UID);
			zone.Devices.Add(device);
			zone.OnChanged();
			device.OnChanged();
		}

		public static void RemoveDeviceFromZone(XDevice device, XZone zone)
		{
			device.Zones.Remove(zone);
			device.ZoneUIDs.Remove(zone.UID);
			zone.Devices.Remove(device);
			zone.OnChanged();
			device.OnChanged();
		}

		public static void AddDevice(XDevice device)
		{

		}

		public static void RemoveDevice(XDevice parentDevice, XDevice device)
		{
			foreach (var zone in device.Zones)
			{
				zone.Devices.Remove(device);
				zone.OnChanged();
			}
			device.OnChanged();
			parentDevice.Children.Remove(device);
		}

		public static void AddZone(XZone zone)
		{
			DeviceConfiguration.Zones.Add(zone);
		}

		public static void RemoveZone(XZone zone)
		{
			foreach (var device in zone.Devices)
			{
				device.Zones.Remove(zone);
				device.ZoneUIDs.Remove(zone.UID);
				device.OnChanged();
			}
			foreach (var direction in zone.Directions)
			{
				direction.Zones.Remove(zone);
				direction.ZoneUIDs.Remove(zone.UID);
				direction.OnChanged();
			}
			zone.OnChanged();
			DeviceConfiguration.Zones.Remove(zone);
		}

		public static void AddDirection(XDirection direction)
		{
			DeviceConfiguration.Directions.Add(direction);
		}

		public static void RemoveDirection(XDirection direction)
		{
			foreach (var zone in direction.Zones)
			{
				zone.Directions.Remove(direction);
				zone.OnChanged();
			}
			direction.OnChanged();
			DeviceConfiguration.Directions.Remove(direction);
		}

		public static void ChangeDirectionZones(XDirection direction, List<Guid> zoneUIDs)
		{
			foreach (var zone in direction.Zones)
			{
				zone.Directions.Remove(direction);
				zone.OnChanged();
			}
			direction.Zones.Clear();

			foreach (var zoneUID in zoneUIDs)
			{
				var zone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == zoneUID);
				{
					if (zone != null)
					{
						direction.Zones.Add(zone);
						zone.Directions.Add(direction);
						zone.OnChanged();
					}
				}
			}
			direction.ZoneUIDs = zoneUIDs;
			direction.OnChanged();
		}

		public static void ChangeDirectionDevices(XDirection direction, List<XDevice> devices)
		{
			direction.OnChanged();
		}
	}
}