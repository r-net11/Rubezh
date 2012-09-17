using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                if ((device.ZoneNo != null))
                {
                    var zone = Zones.FirstOrDefault(x => x.No == device.ZoneNo);
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
                    foreach (var clauseZone in clause.ZoneNos)
                    {
                        var zone = Zones.FirstOrDefault(x => x.No == clauseZone);
                        if (zone != null)
                        {
                            device.ZonesInLogic.Add(zone);
                            zone.DevicesInZoneLogic.Add(device);
                        }
                    }

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
                if (indicatorLogicDevice != null)
                {
                    device.IndicatorLogic.Device = indicatorLogicDevice;
                    indicatorLogicDevice.DependentDevices.Add(device);
                }
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
                device.ZoneNo = null;

            if (device.Driver.IsZoneLogicDevice)
                InvalidateZoneLogicDevice(device);
            else
                device.ZoneLogic = null;

            if (device.Driver.DriverType == DriverType.Indicator)
                InvalidateIndicator(device);
            else
                device.IndicatorLogic = null;

            if (device.Driver.DriverType == DriverType.PDUDirection)
                InvalidatePDUDirection(device);
            else
                device.PDUGroupLogic = null;

            UpdateChildMPTZones();
        }

        public void InvalidateZoneDevice(Device device)
        {
            if (device.ZoneNo != null)
            {
                var zone = Zones.FirstOrDefault(x => x.No == device.ZoneNo);
                if (zone == null)
                    device.ZoneNo = null;
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

                    var zoneNos = new List<int>();
                    var zones = new List<Zone>();
                    if (clause.ZoneNos == null)
                        clause.ZoneNos = new List<int>();
                    foreach (var zoneNo in clause.ZoneNos)
                    {
                        var zone = Zones.FirstOrDefault(x => x.No == zoneNo);
                        if (zone != null)
                        {
                            zoneNos.Add(zoneNo);
                            zones.Add(zone);
                        }
                    }
                    clause.ZoneNos = zoneNos;
                    clause.Zones = zones;

                    if ((clause.Device != null) || (clause.ZoneNos.Count > 0) || clause.State == ZoneLogicState.Failure)
                        clauses.Add(clause);
                }
                device.ZoneLogic.Clauses = clauses;
            }
        }

        public void InvalidateIndicator(Device device)
        {
            if (device.Driver.DriverType == DriverType.Indicator)
            {
                device.IndicatorLogic.Device = Devices.FirstOrDefault(x => x.UID == device.IndicatorLogic.DeviceUID);
                if (device.IndicatorLogic.Device == null)
                {
                    device.IndicatorLogic.DeviceUID = Guid.Empty;
                }

                var zoneNos = new List<int>();
                var zones = new List<Zone>();
                if (device.IndicatorLogic.ZoneNos == null)
                {
                    device.IndicatorLogic.ZoneNos = new List<int>();
                }
                foreach (var zoneNo in device.IndicatorLogic.ZoneNos)
                {
                    var zone = Zones.FirstOrDefault(x => x.No == zoneNo);
                    if (zone != null)
                    {
                        zoneNos.Add(zoneNo);
                        zones.Add(zone);
                    }
                }
                device.IndicatorLogic.ZoneNos = zoneNos;
                device.IndicatorLogic.Zones = zones;
            }
        }

        public void InvalidatePDUDirection(Device device)
        {
            foreach (var pduGroupDevice in device.PDUGroupLogic.Devices)
            {
                if (Devices.Any(x => x.UID == pduGroupDevice.DeviceUID) == false)
                    pduGroupDevice.DeviceUID = Guid.Empty;
            }
        }

        void UpdateChildMPTZones()
        {
            foreach (var device in Devices)
            {
                if (device.Driver.DriverType == DriverType.MPT)
                {
                    foreach (var child in device.Children)
                    {
                        child.ZoneNo = device.ZoneNo;
                    }
                }
            }
        }
    }
}