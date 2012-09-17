using System;
using System.Linq;
using FiresecAPI.Models;
using System.Collections.Generic;

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

            foreach (var dependentDevice in device.DependentDevices)
            {
                DeviceConfiguration.InvalidateOneDevice(dependentDevice);
                DeviceConfiguration.UpdateOneDeviceCrossReferences(dependentDevice);
                device.OnChanged();
            }

			var parentDevice = device.Parent;
			parentDevice.Children.Remove(device);
			parentDevice.OnChanged();
		}

		public void AddZone(Zone zone)
		{
			DeviceConfiguration.Zones.Add(zone);
		}

		public void RemoveZone(Zone zone)
		{
            DeviceConfiguration.Zones.Remove(zone);
            foreach (var device in zone.DevicesInZone)
            {
                device.Zone = null;
                device.ZoneNo = null;
                device.OnChanged();
            }
			foreach (var device in zone.DevicesInZoneLogic)
			{
                //DeviceConfiguration.InvalidateOneDevice(device);
                DeviceConfiguration.UpdateOneDeviceCrossReferences(device);
				device.OnChanged();
			}
            zone.UpdateExternalDevices();
            foreach (var device in zone.IndicatorsInZone)
			{
                //DeviceConfiguration.InvalidateOneDevice(device);
                DeviceConfiguration.UpdateOneDeviceCrossReferences(device);
				device.OnChanged();
			}
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
                device.OnChanged();
                if (device.Zone != null)
                {
                    device.Zone.UpdateExternalDevices();
                    device.Zone.DevicesInZone.Remove(device);
                    device.Zone.OnChanged();
                }
                device.Zone = zone;
                if (zone != null)
                {
                    device.ZoneNo = zone.No;
                    zone.DevicesInZone.Add(device);
                    zone.UpdateExternalDevices();
                    zone.OnChanged();
                }
                else
                    device.ZoneNo = null;
            }
        }

        public void RemoveDeviceFromZone(Device parentDevice, Zone zone)
        {
            foreach (var device in GetMPTGroup(parentDevice))
            {
                device.OnChanged();
                if (device.Zone != null)
                {
                    device.Zone.UpdateExternalDevices();
                }
                device.Zone = null;
                device.ZoneNo = null;
                zone.DevicesInZone.Remove(device);
                zone.UpdateExternalDevices();
                zone.OnChanged();
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

            foreach (var dependentDevice in device.DependentDevices)
            {
                DeviceConfiguration.InvalidateOneDevice(dependentDevice);
                DeviceConfiguration.UpdateOneDeviceCrossReferences(dependentDevice);
                device.OnChanged();
            }
            device.DependentDevices.Clear();

            device.ZoneLogic = zoneLogic;
            DeviceConfiguration.InvalidateOneDevice(device);
            DeviceConfiguration.UpdateOneDeviceCrossReferences(device);
            device.HasExternalDevices = HasExternalDevices(device);
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
	}
}