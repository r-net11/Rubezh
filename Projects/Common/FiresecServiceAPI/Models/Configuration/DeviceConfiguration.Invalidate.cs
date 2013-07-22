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
                if ((device.ZoneUID != Guid.Empty))
                {
                    var zone = Zones.FirstOrDefault(x => x.UID == device.ZoneUID);
                    device.Zone = zone;
                    if (zone != null)
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
							if (!zone.DevicesInZoneLogic.Any(x => x.UID == device.UID))
							{
								zone.DevicesInZoneLogic.Add(device);
							}
						}
					}
					clause.Zones = zones;

					clause.Devices = new List<Device>();
					if (clause.DeviceUIDs != null)
					{
						foreach (var deviceUID in clause.DeviceUIDs)
						{
							var clauseDevice = Devices.FirstOrDefault(x => x.UID == deviceUID);
							if (clauseDevice != null)
							{
								clause.Devices.Add(clauseDevice);
								clauseDevice.DependentDevices.Add(device);
							}
						}
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
						if (!zone.IndicatorsInZone.Any(x=>x.UID == device.UID))
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
					if(clause.DeviceUIDs == null)
						clause.DeviceUIDs = new List<Guid>();
					var deviceUIDs = new List<Guid>();
					foreach (var deviceUID in clause.DeviceUIDs)
					{
						var deviceInClause = Devices.FirstOrDefault(x => x.UID == deviceUID);
						if (deviceInClause != null)
						{
							deviceUIDs.Add(deviceUID);
						}
					}
					clause.DeviceUIDs = deviceUIDs;

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

                    if (clause.DeviceUIDs.Count > 0 || clause.ZoneUIDs.Count > 0 || clause.State == ZoneLogicState.Failure || clause.State == ZoneLogicState.DoubleFire)
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

        public string GetCommaSeparatedZones(List<Zone> zones)
        {
            if (zones.Count == 1)
            {
                var Zone = Zones.FirstOrDefault(x => x.PresentationName == zones[0].PresentationName);
                if (Zone != null)
                {
                    return Zone.PresentationName;
                }
            }
            if (zones.Count > 0)
            {
                var orderedZones = new List<int>();
                foreach (var zone in zones)
                {
                    var Zone = Zones.FirstOrDefault(x => x.PresentationName == zone.PresentationName);
                    if (Zone != null)
                    {
                        orderedZones.Add(Zone.No);
                    }
                }
                orderedZones = orderedZones.OrderBy(x => x).ToList();
                int prevZoneNo = 0;
                if (orderedZones.Count > 0)
                    prevZoneNo = orderedZones[0];
                var groupOfZones = new List<List<int>>();

                for (int i = 0; i < orderedZones.Count; i++)
                {
                    var zoneNo = orderedZones[i];

                    if (orderedZones.Any(x => x == prevZoneNo + 1))
                    {
                        if (groupOfZones.Count == 0)
                        {
                            groupOfZones.Add(new List<int>() { zoneNo });
                        }
                        else
                        {
                            groupOfZones.Last().Add(zoneNo);
                        }
                    }
                    else
                    {
                        groupOfZones.Add(new List<int>() { zoneNo });
                    }
                    prevZoneNo = zoneNo;
                }

                var presenrationZones = new StringBuilder();
                for (int i = 0; i < groupOfZones.Count; i++)
                {
                    var zoneGroup = groupOfZones[i];

                    if (i > 0)
                        presenrationZones.Append(", ");

                    presenrationZones.Append(zoneGroup.First().ToString());
                    if (zoneGroup.Count > 1)
                    {
                        presenrationZones.Append(" - " + zoneGroup.Last().ToString());
                    }
                }

                return presenrationZones.ToString();
            }
            return "";
        }

        public string GetCommaSeparatedZones(List<Guid> zoneUIDs)
        {
            if (zoneUIDs.Count == 1)
            {
                var Zone = Zones.FirstOrDefault(x => x.UID == zoneUIDs.First());
                if (Zone != null)
                {
                    return Zone.PresentationName;
                }
            }
            if (zoneUIDs.Count > 0)
            {
                var orderedZones = new List<int>();
                foreach (var zoneUID in zoneUIDs)
                {
                    var Zone = Zones.FirstOrDefault(x => x.UID == zoneUID);
                    if (Zone != null)
                    {
                        orderedZones.Add(Zone.No);
                    }
                }
                orderedZones = orderedZones.OrderBy(x => x).ToList();
                int prevZoneNo = 0;
                if (orderedZones.Count > 0)
                    prevZoneNo = orderedZones[0];
                var groupOfZones = new List<List<int>>();

                for (int i = 0; i < orderedZones.Count; i++)
                {
                    var zoneNo = orderedZones[i];

                    if (orderedZones.Any(x => x == prevZoneNo + 1))
                    {
                        if (groupOfZones.Count == 0)
                        {
                            groupOfZones.Add(new List<int>() { zoneNo });
                        }
                        else
                        {
                            groupOfZones.Last().Add(zoneNo);
                        }
                    }
                    else
                    {
                        groupOfZones.Add(new List<int>() { zoneNo });
                    }
                    prevZoneNo = zoneNo;
                }

                var presenrationZones = new StringBuilder();
                for (int i = 0; i < groupOfZones.Count; i++)
                {
                    var zoneGroup = groupOfZones[i];

                    if (i > 0)
                        presenrationZones.Append(", ");

                    presenrationZones.Append(zoneGroup.First().ToString());
                    if (zoneGroup.Count > 1)
                    {
                        presenrationZones.Append(" - " + zoneGroup.Last().ToString());
                    }
                }

                return presenrationZones.ToString();
            }
            return "";
        }

        public string GetPresentationZone(ZoneLogic zoneLogic)
        {
            string result = "";

            for (int i = 0; i < zoneLogic.Clauses.Count; i++)
            {
                var clause = zoneLogic.Clauses[i];

                if (i > 0)
                {
                    switch (zoneLogic.JoinOperator)
                    {
                        case ZoneLogicJoinOperator.And:
                            result += " и ";
                            break;
                        case ZoneLogicJoinOperator.Or:
                            result += " или ";
                            break;
                        default:
                            break;
                    }
                }

				if (clause.DeviceUIDs != null && clause.DeviceUIDs.Count > 0)
                {
					result += "Количество устройств для сработки " + clause.DeviceUIDs.Count;
                    continue;
                }

                if (clause.State == ZoneLogicState.Failure)
                {
                    result += "состояние неисправность прибора";
                    continue;
                }

				if (clause.State == ZoneLogicState.DoubleFire)
				{
					result += "состояние Включение без задержки по пожару в двух зонах";
					continue;
				}

                result += "состояние " + clause.State.ToDescription();
                result += " " + clause.Operation.ToDescription() + " " + GetCommaSeparatedZones(clause.Zones);
            }

            return result;
        }

        public string GetPresentationZone(Device device)
        {
            if (device.Driver.IsZoneDevice)
            {
                if (device.Zone != null)
                    return device.Zone.PresentationName;
                return null;
            }

            if (device.Driver.IsZoneLogicDevice && device.ZoneLogic != null)
                return GetPresentationZone(device.ZoneLogic);
            if (device.Driver.DriverType == DriverType.Indicator && device.IndicatorLogic != null)
                return device.IndicatorLogic.ToString();
            if ((device.Driver.DriverType == DriverType.PDUDirection) && (device.PDUGroupLogic != null))
                return device.PDUGroupLogic.ToString();

            return null;
        }
    }
}