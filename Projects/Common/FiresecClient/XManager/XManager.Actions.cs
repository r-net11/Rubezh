using System.Collections.Generic;
using System.Linq;
using XFiresecAPI;
using System;

namespace FiresecClient
{
	public partial class XManager
	{
        public static void ChangeDeviceZones(XDevice device, List<XZone> zones)
        {
            foreach (var zone in device.Zones)
            {
                zone.Devices.Remove(device);
                zone.OnChanged();
            }
            device.Zones.Clear();
            device.ZoneUIDs.Clear();
            foreach (var zone in zones)
            {
                device.Zones.Add(zone);
                device.ZoneUIDs.Add(zone.UID);
                zone.Devices.Add(device);
                zone.OnChanged();
            }
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
			device.InitializeDefaultProperties();
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

		public static void ChangeDirectionZones(XDirection direction, List<XZone> zones)
		{
			foreach (var zone in direction.Zones)
			{
				zone.Directions.Remove(direction);
				zone.OnChanged();
			}
			direction.Zones.Clear();
            direction.ZoneUIDs.Clear();
            foreach (var zone in zones)
            {
                direction.Zones.Add(zone);
                direction.ZoneUIDs.Add(zone.UID);
                zone.Directions.Add(direction);
                zone.OnChanged();
            }
			direction.OnChanged();
		}

		public static void ChangeDirectionDevices(XDirection direction, List<XDevice> devices)
		{
            foreach (var device in direction.Devices)
            {
                device.Directions.Remove(direction);
                device.OnChanged();
            }
            direction.Devices.Clear();
            direction.DeviceUIDs.Clear();
            foreach (var device in devices)
            {
                direction.DeviceUIDs.Add(device.UID);
                direction.Devices.Add(device);
                device.OnChanged();
            }
			direction.OnChanged();
		}
	}
}