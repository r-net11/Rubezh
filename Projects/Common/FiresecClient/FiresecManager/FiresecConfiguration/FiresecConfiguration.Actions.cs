using System;
using System.Linq;
using FiresecAPI.Models;

namespace FiresecClient
{
	public partial class FiresecConfiguration
	{
		public Device AddDevice(Device parentDevice, Driver driver, int intAddress)
		{
			var device = new Device()
			{
				DriverUID = driver.UID,
				Driver = driver,
				IntAddress = intAddress,
				Parent = parentDevice
			};
			if (parentDevice.Driver.DriverType == DriverType.MPT)
			{
				device.ZoneNo = parentDevice.ZoneNo;
			}
			parentDevice.Children.Add(device);
			AddAutoCreateChildren(device);
			AddAutoChildren(device);

			parentDevice.OnChanged();
			DeviceConfiguration.Update();
			return device;
		}

		void AddAutoCreateChildren(Device device)
		{
			foreach (var autoCreateDriverId in device.Driver.AutoCreateChildren)
			{
				var autoCreateDriver = DriversConfiguration.Drivers.FirstOrDefault(x => x.UID == autoCreateDriverId);

				for (int i = autoCreateDriver.MinAutoCreateAddress; i <= autoCreateDriver.MaxAutoCreateAddress; i++)
				{
					AddDevice(device, autoCreateDriver, i);
				}
			}
		}

		void AddAutoChildren(Device device)
		{
			if (device.Driver.AutoChild != Guid.Empty)
			{
				var driver = FiresecManager.FiresecConfiguration.DriversConfiguration.Drivers.FirstOrDefault(x => x.UID == device.Driver.AutoChild);

				for (int i = 0; i < device.Driver.AutoChildCount; i++)
				{
					var autoDevice = AddDevice(device, driver, device.IntAddress + i);
				}
			}
		}

		public void RemoveDevice(Device device)
		{
			Zone zone = null;
			if (device.ZoneNo.HasValue)
				zone = DeviceConfiguration.Zones.FirstOrDefault(x => x.No == device.ZoneNo);
			if (zone != null)
			{
				zone.DevicesInZone.Remove(device);
				zone.OnChanged();
			}

			var parentDevice = device.Parent;
			parentDevice.Children.Remove(device);

			parentDevice.OnChanged();
			DeviceConfiguration.Update();
			InvalidateConfiguration();
		}

		public void AddZone(Zone zone)
		{
			DeviceConfiguration.Zones.Add(zone);
		}

		public void RemoveZone(Zone zone)
		{
			foreach (var device in zone.DeviceInZoneLogic)
			{
				DeviceConfiguration.InvalidateOneDevice(device);
				device.OnChanged();
			}
			foreach (var device in zone.DevicesInZone)
			{
				device.Zone = null;
				device.ZoneNo = null;
				device.OnChanged();
			}
			DeviceConfiguration.Zones.Remove(zone);
			DeviceConfiguration.Devices.ForEach(x => { if ((x.ZoneNo != null) && (x.ZoneNo.Value == zone.No)) x.ZoneNo = null; });
			InvalidateConfiguration();
		}
	}
}