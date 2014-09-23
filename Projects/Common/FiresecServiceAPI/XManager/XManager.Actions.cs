using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;

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

		public static void RemoveDevice(XDevice device)
		{
			var parentDevice = device.Parent;
			foreach (var zone in device.Zones)
			{
				zone.Devices.Remove(device);
				zone.OnChanged();
			}
			foreach (var direction in device.Directions)
			{
				direction.InputDevices.Remove(device);
				direction.OutputDevices.Remove(device);
				var directionDevice = direction.DirectionDevices.FirstOrDefault(x => x.Device == device);
				if (directionDevice != null)
				{
					direction.DirectionDevices.Remove(directionDevice);
					direction.InputDevices.Remove(device);
				}

				direction.OnChanged();
			}
			parentDevice.Children.Remove(device);
			Devices.Remove(device);
		}

		#region RebuildRSR2Addresses
		public static void RebuildRSR2Addresses(XDevice parentDevice)
		{
			foreach (var shliefDevice in parentDevice.Children)
			{
				RebuildRSR2Addresses_Children = new List<XDevice>();
				RebuildRSR2Addresses_AddChild(shliefDevice);

				byte currentAddress = 1;
				foreach (var device in RebuildRSR2Addresses_Children)
				{
					device.IntAddress = currentAddress;
					if (!device.Driver.IsGroupDevice)
					{
						currentAddress++;
					}
					device.OnChanged();
				}
			}
		}

		static List<XDevice> RebuildRSR2Addresses_Children;
		static void RebuildRSR2Addresses_AddChild(XDevice device)
		{
			if (device.DriverType != XDriverType.RSR2_MVP_Part && device.DriverType != XDriverType.RSR2_KAU_Shleif)
				RebuildRSR2Addresses_Children.Add(device);

			foreach (var child in device.Children)
			{
				RebuildRSR2Addresses_AddChild(child);
			}
		}
		#endregion

		public static void AddZone(XZone zone)
		{
			Zones.Add(zone);
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
			Zones.Remove(zone);
			zone.OnChanged();
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
			Directions.Add(direction);
		}

		public static void RemoveDirection(XDirection direction)
		{
			foreach (var zone in direction.InputZones)
			{
				zone.Directions.Remove(direction);
				zone.OnChanged();
			}
			Directions.Remove(direction);
			direction.OnChanged();
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
					directionZone.StateBit = existingDirectionZone.StateBit;
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
				if(device.Driver.AvailableStateBits.Contains(XStateBit.Fire1))
				{
					directionDevice.StateBit = XStateBit.Fire1;
				}
				else if (device.Driver.AvailableStateBits.Contains(XStateBit.Fire2))
				{
					directionDevice.StateBit = XStateBit.Fire2;
				}
				else if (device.Driver.AvailableStateBits.Contains(XStateBit.On))
				{
					directionDevice.StateBit = XStateBit.On;
				}
				var existingDirectionDevice = oldDirectionDevices.FirstOrDefault(x => x.Device == device);
				if (existingDirectionDevice != null)
				{
					directionDevice.StateBit = existingDirectionDevice.StateBit;
				}
				direction.DirectionDevices.Add(directionDevice);
				direction.InputDevices.Add(device);
				device.Directions.Add(direction);
				device.OnChanged();
			}
			direction.OnChanged();
		}

		public static void ChangeDeviceLogic(XDevice device, XDeviceLogic deviceLogic)
		{
			foreach (var clause in device.DeviceLogic.ClausesGroup.Clauses)
			{
				foreach (var direction in clause.Directions)
				{
					direction.OutputDevices.Remove(device);
					direction.OnChanged();
					device.Directions.Remove(direction);
				}
			}
			device.DeviceLogic = deviceLogic;
			DeviceConfiguration.InvalidateOneLogic(device, device.DeviceLogic);
			device.OnChanged();
		}

		public static void ChangeDriver(XDevice device, XDriver driver)
		{
			var changeZone = !(device.Driver.HasZone && driver.HasLogic);
			device.Driver = driver;
			device.DriverUID = driver.UID;
			if (driver.IsRangeEnabled)
				device.IntAddress = driver.MinAddress;

			device.Children.Clear();
			if (driver.IsGroupDevice)
			{
				var groupDriver = XManager.Drivers.FirstOrDefault(x => x.DriverType == device.Driver.GroupDeviceChildType);

				for (byte i = 0; i < device.Driver.GroupDeviceChildrenCount; i++)
				{
					var autoDevice = XManager.AddChild(device, null, groupDriver, (byte)(device.IntAddress + i));
				}
			}

			if (changeZone)
			{
				RemoveDeviceFromZone(device, null);
				ChangeDeviceLogic(device, new XDeviceLogic());
			}
			device.Properties = new List<XProperty>();
		}
	}
}