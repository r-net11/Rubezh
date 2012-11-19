using System;
using System.Collections.Generic;
using System.Linq;

namespace FiresecAPI.Models
{
    public partial class DeviceConfiguration
    {
        public void UpdateCrossReferences()
        {
            foreach (var zone in Zones)
            {
                zone.DevicesInZone = new List<Device>();
                zone.DevicesInZoneLogic = new List<Device>();
                zone.IndicatorsInZone = new List<Device>();
            }
            foreach (var device in Devices)
            {
                UpdateOneDeviceCrossReferences(device);
            }
        }

        public void UpdateOneDeviceCrossReferences(Device device)
        {
            device.Zone = null;
            device.ZonesInLogic = new List<Zone>();
            device.DependentDevices = new List<Device>();

            if (device.Driver.IsZoneDevice)
            {
                if ((device.ZoneUID != Guid.Empty))
                {
                    var zone = Zones.FirstOrDefault(x => x.UID == device.ZoneUID);
                    device.Zone = zone;
                    zone.DevicesInZone.Add(device);
                }
            }

            if (device.Driver.IsZoneLogicDevice)
            {
                if (device.ZoneLogic == null)
                    device.ZoneLogic = new ZoneLogic();
                foreach (var clause in device.ZoneLogic.Clauses)
                {
					var zones = new List<Zone>();
                    foreach (var zoneUID in clause.ZoneUIDs)
                    {
                        var zone = Zones.FirstOrDefault(x => x.UID == zoneUID);
                        if (zone != null)
                        {
							zones.Add(zone);
                            device.ZonesInLogic.Add(zone);
                            zone.DevicesInZoneLogic.Add(device);
                        }
                    }
					clause.Zones = zones;

                    clause.Device = null;
                    if (clause.DeviceUID != Guid.Empty)
                    {
                        var clauseDevice = Devices.FirstOrDefault(x => x.UID == clause.DeviceUID);
                        clause.Device = clauseDevice;
                        clauseDevice.DependentDevices.Add(device);
                    }
                }
            }

            if (device.Driver.DriverType == DriverType.Indicator)
            {
                if (device.IndicatorLogic == null)
                    device.IndicatorLogic = new IndicatorLogic();

                var indicatorLogicDevice = Devices.FirstOrDefault(x => x.UID == device.IndicatorLogic.DeviceUID);
				if (indicatorLogicDevice != null && indicatorLogicDevice.DependentDevices!=null)
                {
                    device.IndicatorLogic.Device = indicatorLogicDevice;
                    indicatorLogicDevice.DependentDevices.Add(device);
                }

				var zones = new List<Zone>();
                foreach (var zoneUID in device.IndicatorLogic.ZoneUIDs)
				{
                    var zone = Zones.FirstOrDefault(x => x.UID == zoneUID);
					if (zone != null)
					{
						zones.Add(zone);
						zone.IndicatorsInZone.Add(device);
					}
				}
				device.IndicatorLogic.Zones = zones;
            }

            if (device.Driver.DriverType == DriverType.PDUDirection)
            {
                if (device.PDUGroupLogic == null)
                    device.PDUGroupLogic = new PDUGroupLogic();

                foreach (var pduGroupDevice in device.PDUGroupLogic.Devices)
                {
                    pduGroupDevice.Device = null;
                    if (pduGroupDevice.DeviceUID != Guid.Empty)
                    {
                        var pduDevice = Devices.FirstOrDefault(x => x.UID == pduGroupDevice.DeviceUID);
                        if (pduDevice != null)
                        {
                            pduGroupDevice.Device = pduDevice;
                            pduDevice.DependentDevices.Add(device);
                        }
                    }
                }
            }
        }

        public void InvalidateConfiguration()
        {
            foreach (var device in Devices)
            {
                InvalidateOneDevice(device);
            }
        }

        public void InvalidateOneDevice(Device device)
        {
            if (device.Driver.IsZoneDevice)
                InvalidateZoneDevice(device);
            else
                device.ZoneUID = Guid.Empty;

            if (device.Driver.IsZoneLogicDevice)
                InvalidateZoneLogicDevice(device);
            else
                device.ZoneLogic = new ZoneLogic();

            if (device.Driver.DriverType == DriverType.Indicator)
                InvalidateIndicator(device);
            else
                device.IndicatorLogic = new IndicatorLogic();

            if (device.Driver.DriverType == DriverType.PDUDirection)
                InvalidatePDUDirection(device);
            else
                device.PDUGroupLogic = new PDUGroupLogic();

            UpdateChildMPTZones();
        }

        public void InvalidateZoneDevice(Device device)
        {
            if (device.ZoneUID != Guid.Empty)
            {
                var zone = Zones.FirstOrDefault(x => x.UID == device.ZoneUID);
                if (zone == null)
                    device.ZoneUID = Guid.Empty;
            }
        }

        public void InvalidateZoneLogicDevice(Device device)
        {
            if (device.ZoneLogic != null)
            {
                var clauses = new List<Clause>();
                foreach (var clause in device.ZoneLogic.Clauses)
                {
                    if (Devices.Any(x => x.UID == clause.DeviceUID) == false)
                    {
                        clause.DeviceUID = Guid.Empty;
                        clause.Device = null;
                    }

                    var zoneUIDs = new List<Guid>();
                    if (clause.ZoneUIDs == null)
                        clause.ZoneUIDs = new List<Guid>();
                    foreach (var zoneUID in clause.ZoneUIDs)
                    {
                        var zone = Zones.FirstOrDefault(x => x.UID == zoneUID);
                        if (zone != null)
                        {
                            zoneUIDs.Add(zoneUID);
                        }
                    }
                    clause.ZoneUIDs = zoneUIDs;

                    if ((clause.DeviceUID != Guid.Empty) || (clause.ZoneUIDs.Count > 0) || clause.State == ZoneLogicState.Failure)
                        clauses.Add(clause);
                }
                device.ZoneLogic.Clauses = clauses;
            }
        }

		public void InvalidateIndicator(Device device)
		{
			device.IndicatorLogic.Device = Devices.FirstOrDefault(x => x.UID == device.IndicatorLogic.DeviceUID);
			if (device.IndicatorLogic.Device == null)
			{
				device.IndicatorLogic.DeviceUID = Guid.Empty;
			}

            var zoneUIDs = new List<Guid>();
			if (device.IndicatorLogic.ZoneUIDs == null)
			{
                device.IndicatorLogic.ZoneUIDs = new List<Guid>();
			}
            foreach (var zoneUID in device.IndicatorLogic.ZoneUIDs)
			{
                var zone = Zones.FirstOrDefault(x => x.UID == zoneUID);
				if (zone != null)
				{
                    zoneUIDs.Add(zoneUID);
				}
			}
            device.IndicatorLogic.ZoneUIDs = zoneUIDs;
		}

        public void InvalidatePDUDirection(Device device)
        {
			var pduGroupLogic = new PDUGroupLogic();
			foreach (var pduGroupDevice in device.PDUGroupLogic.Devices)
			{
				if (Devices.Any(x => x.UID == pduGroupDevice.DeviceUID))
				{
					pduGroupLogic.Devices.Add(pduGroupDevice);
				}
			}
			device.PDUGroupLogic = pduGroupLogic;
        }

        void UpdateChildMPTZones()
        {
            foreach (var device in Devices)
            {
                if (device.Driver.DriverType == DriverType.MPT)
                {
                    foreach (var child in device.Children)
                    {
                        child.ZoneUID = device.ZoneUID;
                    }
                }
            }
        }
    }
}