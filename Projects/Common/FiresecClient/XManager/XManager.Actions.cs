using System.Collections.Generic;
using System.Linq;
using XFiresecAPI;

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
			if (zone != null)
			{
				device.Zones.Remove(zone);
				device.ZoneUIDs.Remove(zone.UID);
				zone.Devices.Remove(device);
				zone.OnChanged();
				device.OnChanged();
			}
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
            foreach (var direction in device.Directions)
            {
                direction.InputDevices.Remove(device);
                var directionDevice = direction.DirectionDevices.FirstOrDefault(x => x.Device == device);
                if (directionDevice != null)
                {
                    direction.DirectionDevices.Remove(directionDevice);
                    direction.InputDevices.Remove(device);
                }
                direction.OnChanged();
            }
			device.OnChanged();
			parentDevice.Children.Remove(device);
			DeviceConfiguration.Devices.Remove(device);
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
				direction.InputZones.Remove(zone);
                var directionZone = direction.DirectionZones.FirstOrDefault(x => x.Zone == zone);
                if (directionZone != null)
                {
                    direction.DirectionZones.Remove(directionZone);
                    direction.InputZones.Remove(zone);
                }
				direction.OnChanged();
			}
			zone.OnChanged();
			DeviceConfiguration.Zones.Remove(zone);
		}

		public static void EditZone(XZone zone)
		{
			foreach (var device in zone.Devices)
			{
				device.OnChanged();
			}
			foreach (var device in zone.DevicesInLogic)
			{
				device.OnChanged();
			}
			foreach (var direction in zone.Directions)
			{
				direction.OnChanged();
			}
			zone.OnChanged();
		}

		public static void AddDirection(XDirection direction)
		{
			DeviceConfiguration.Directions.Add(direction);
		}

		public static void RemoveDirection(XDirection direction)
		{
			foreach (var zone in direction.InputZones)
			{
				zone.Directions.Remove(direction);
				zone.OnChanged();
			}
			direction.OnChanged();
			DeviceConfiguration.Directions.Remove(direction);
		}

		public static void ChangeDirectionZones(XDirection direction, List<XZone> zones)
		{
			foreach (var zone in direction.InputZones)
			{
				zone.Directions.Remove(direction);
				zone.OnChanged();
			}
			direction.InputZones.Clear();
            var oldDirectionZones = new List<XDirectionZone>(direction.DirectionZones);
            direction.DirectionZones.Clear();
            foreach (var zone in zones)
            {
                direction.InputZones.Add(zone);
                var directionZone = new XDirectionZone()
                {
                    ZoneUID = zone.UID,
                    Zone = zone
                };
                var existingDirectionZone = oldDirectionZones.FirstOrDefault(x => x.Zone == zone);
                if (existingDirectionZone != null)
                {
                    directionZone.StateType = existingDirectionZone.StateType;
                }
                direction.DirectionZones.Add(directionZone);
                zone.Directions.Add(direction);
                zone.OnChanged();
            }
			direction.OnChanged();
		}

		public static void ChangeDirectionDevices(XDirection direction, List<XDevice> devices)
		{
            foreach (var device in direction.InputDevices)
            {
                device.Directions.Remove(direction);
                device.OnChanged();
            }
            direction.InputDevices.Clear();
            var oldDirectionDevices = new List<XDirectionDevice>(direction.DirectionDevices);
            direction.DirectionDevices.Clear();
            foreach (var device in devices)
            {
                var directionDevice = new XDirectionDevice()
                {
                    DeviceUID = device.UID,
                    Device = device
                };
				if(device.Driver.AvailableStates.Contains(XStateType.Fire1))
				{
					directionDevice.StateType = XStateType.Fire1;
				}
				else if (device.Driver.AvailableStates.Contains(XStateType.Fire2))
				{
					directionDevice.StateType = XStateType.Fire2;
				}
				else if (device.Driver.AvailableStates.Contains(XStateType.On))
				{
					directionDevice.StateType = XStateType.On;
				}
                var existingDirectionDevice = oldDirectionDevices.FirstOrDefault(x => x.Device == device);
                if (existingDirectionDevice != null)
                {
                    directionDevice.StateType = existingDirectionDevice.StateType;
                }
                direction.DirectionDevices.Add(directionDevice);
                direction.InputDevices.Add(device);
                device.OnChanged();
            }
			direction.OnChanged();
		}

		public static void ChangeDeviceLogic(XDevice device, XDeviceLogic deviceLogic)
		{
			foreach (var clause in device.DeviceLogic.Clauses)
			{
				foreach (var direction in clause.Directions)
				{
					direction.OutputDevices.Remove(device);
					direction.OnChanged();
					device.Directions.Remove(direction);
				}
			}
			device.DeviceLogic = deviceLogic;
			InvalidateOneLogic(device);
			device.OnChanged();
		}

		public static void ChangeDriver(XDevice device, XDriver driver)
		{
			var changeZone = !(device.Driver.HasZone && driver.HasLogic);
			device.Driver = driver;
			device.DriverUID = driver.UID;
			if (driver.IsRangeEnabled)
				device.IntAddress = driver.MinAddress;
			if (changeZone)
			{
				RemoveDeviceFromZone(device, null);
				ChangeDeviceLogic(device, new XDeviceLogic());
			}
			device.Properties = new List<XProperty>();
		}
	}
}