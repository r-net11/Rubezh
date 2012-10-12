using System;
using System.Collections.Generic;
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
				device.ZoneUID = parentDevice.ZoneUID;
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
					if (device.Driver.IsChildAddressReservedRange)
						i += device.IntAddress;
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
			DeviceConfiguration.Devices.Remove(device);
			if (device.Zone != null)
			{
                device.Zone.UpdateExternalDevices();
                device.Zone.DevicesInZone.Remove(device);
                device.Zone.OnChanged();
			}

            foreach (var zone in device.ZonesInLogic)
            {
                zone.DevicesInZoneLogic.Remove(device);
                zone.OnChanged();
            }

			foreach (var zone in device.IndicatorLogic.Zones)
			{
				zone.IndicatorsInZone.Remove(device);
				zone.OnChanged();
			}

			var dependentDevices = new List<Device>(device.DependentDevices);
            foreach (var dependentDevice in dependentDevices)
            {
                DeviceConfiguration.InvalidateOneDevice(dependentDevice);
                DeviceConfiguration.UpdateOneDeviceCrossReferences(dependentDevice);
				dependentDevice.OnChanged();
            }

            var children = new List<Device>(device.Children);
            foreach (var child in children)
            {
                RemoveDevice(child);
            }
			var parentDevice = device.Parent;
			parentDevice.Children.Remove(device);
			parentDevice.OnChanged();
			device.OnChanged();
		}

		public void AddZone(Zone zone)
		{
			DeviceConfiguration.Zones.Add(zone);
		}

		public void RemoveZone(Zone zone)
		{
            DeviceConfiguration.Zones.Remove(zone);
			zone.OnColorTypeChanged();
            foreach (var device in zone.DevicesInZone)
            {
                device.Zone = null;
                device.ZoneUID = Guid.Empty;
                device.OnChanged();
            }
			var devicesInZoneLogic = new List<Device>(zone.DevicesInZoneLogic);
			foreach (var device in devicesInZoneLogic)
			{
                DeviceConfiguration.InvalidateOneDevice(device);
                DeviceConfiguration.UpdateOneDeviceCrossReferences(device);
				device.OnChanged();
			}
            zone.UpdateExternalDevices();
			var indicatorsInZone = new List<Device>(zone.IndicatorsInZone);
			foreach (var device in indicatorsInZone)
			{
                DeviceConfiguration.InvalidateOneDevice(device);
                DeviceConfiguration.UpdateOneDeviceCrossReferences(device);
				device.OnChanged();
			}
		}

		public void ChangeZone(Zone zone)
		{
			foreach (var device in zone.DevicesInZone)
			{
				device.OnChanged();
			}
			var devicesInZoneLogic = new List<Device>(zone.DevicesInZoneLogic);
			foreach (var device in devicesInZoneLogic)
			{
				DeviceConfiguration.UpdateOneDeviceCrossReferences(device);
				device.OnChanged();
			}
			var indicatorsInZone = new List<Device>(zone.IndicatorsInZone);
			foreach (var device in indicatorsInZone)
			{
				DeviceConfiguration.UpdateOneDeviceCrossReferences(device);
				device.OnChanged();
			}
			zone.OnChanged();
		}

        List<Device> GetMPTGroup(Device device)
        {
            var devices = new List<Device>();
            devices.Add(device);
            if (device.Driver.DriverType == DriverType.MPT)
            {
                foreach (var child in device.Children)
                {
                    devices.Add(child);
                }
            }
            return devices;
        }

        public void AddDeviceToZone(Device parentDevice, Zone zone)
        {
            foreach (var device in GetMPTGroup(parentDevice))
            {
                if (device.Zone != null)
                {
					device.Zone.DevicesInZone.Remove(device);
                    device.Zone.UpdateExternalDevices();
                    device.Zone.OnChanged();
                }
                device.Zone = zone;
                if (zone != null)
                {
                    device.ZoneUID = zone.UID;
                    zone.DevicesInZone.Add(device);
                    zone.UpdateExternalDevices();
                    zone.OnChanged();
                }
                else
                    device.ZoneUID = Guid.Empty;

				device.OnChanged();
            }
        }

        public void RemoveDeviceFromZone(Device parentDevice, Zone zone)
        {
            foreach (var device in GetMPTGroup(parentDevice))
            {
				var oldZone = device.Zone;
                device.Zone = null;
                device.ZoneUID = Guid.Empty;
				if (oldZone != null)
				{
					oldZone.UpdateExternalDevices();
				}
				if (zone != null)
				{
					zone.DevicesInZone.Remove(device);
					zone.UpdateExternalDevices();
					zone.OnChanged();
				}
				device.OnChanged();
            }
        }

        public void SetDeviceZoneLogic(Device device, ZoneLogic zoneLogic)
        {
            foreach (var zone in device.ZonesInLogic)
            {
                zone.DevicesInZoneLogic.Remove(device);
                zone.OnChanged();
            }
            device.ZonesInLogic.Clear();

			var dependentDevices = new List<Device>(device.DependentDevices);
            foreach (var dependentDevice in dependentDevices)
            {
                DeviceConfiguration.InvalidateOneDevice(dependentDevice);
                DeviceConfiguration.UpdateOneDeviceCrossReferences(dependentDevice);
                device.OnChanged();
            }
            device.DependentDevices.Clear();

            device.ZoneLogic = zoneLogic;
            DeviceConfiguration.InvalidateOneDevice(device);
            DeviceConfiguration.UpdateOneDeviceCrossReferences(device);
            device.UpdateHasExternalDevices();
            device.OnChanged();
        }

        public void SetIndicatorLogic(Device device, IndicatorLogic indicatorLogic)
        {
            foreach (var zone in device.IndicatorLogic.Zones)
                zone.IndicatorsInZone.Remove(device);
            device.IndicatorLogic = indicatorLogic;
            DeviceConfiguration.InvalidateIndicator(device);
            DeviceConfiguration.UpdateOneDeviceCrossReferences(device);
            device.OnChanged();
        }

        public void SetPDUGroupLogic(Device device, PDUGroupLogic pduGroupLogic)
        {
            foreach (var pduGroupDevice in device.PDUGroupLogic.Devices)
            {
                pduGroupDevice.Device = null;
                if (pduGroupDevice.DeviceUID != Guid.Empty)
                {
                    var pduDevice = DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == pduGroupDevice.DeviceUID);
                    if (pduDevice != null)
                    {
                        pduGroupDevice.Device = pduDevice;
                        pduDevice.DependentDevices.Remove(device);
                    }
                }
            }

            device.PDUGroupLogic = pduGroupLogic;
            DeviceConfiguration.InvalidatePDUDirection(device);
            DeviceConfiguration.UpdateOneDeviceCrossReferences(device);
            device.OnChanged();
        }

		public void SetIsNotUsed(Device device, bool isUsed)
		{
			device.IsNotUsed = isUsed;
			device.OnChanged();
			if (isUsed)
			{
				SetDeviceZoneLogic(device, new ZoneLogic());
			}
		}

		public void ChangeDriver(Device device, Driver driver)
		{
			device.Driver = driver;
			device.DriverUID = driver.UID;
			RemoveDeviceFromZone(device, null);
			SetDeviceZoneLogic(device, new ZoneLogic());
			device.Properties = new List<Property>();
		}
	}
}