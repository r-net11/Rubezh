using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using FiresecAPI.Models;

namespace FiresecClient
{
	public partial class FiresecConfiguration
	{
		public string GetCommaSeparatedZones(List<Zone> zones)
		{
			if (zones.Count == 1)
			{
                var Zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == zones[0].UID);
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
                    var Zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == zone.UID);
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
                var Zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == zoneUIDs.First());
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
                    var Zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
                    if (Zone != null)
                    {
                        orderedZones.Add(Zone.No);
                    }
                }
                orderedZones = orderedZones.OrderBy(x => x).ToList();
				int prevZoneNo = 0;
				if(orderedZones.Count != 0)
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
					result += "Сработка устройств ";
					foreach (var device in clause.Devices)
					{
						result += device.PresentationAddressAndName + ", ";
					}
					if (result.EndsWith(", "))
						result = result.Remove(result.Length - 2, 2);
					continue;
				}

				if (clause.State == ZoneLogicState.Failure)
				{
					result += "состояние неисправность прибора";
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

		public string GetIndicatorString(Device indicatorDevice)
		{
			switch (indicatorDevice.IndicatorLogic.IndicatorLogicType)
			{
				case IndicatorLogicType.Device:
					{
						if (indicatorDevice.IndicatorLogic.DeviceUID != Guid.Empty)
						{
							var deviceString = "Устр: ";
							deviceString += indicatorDevice.IndicatorLogic.Device.PresentationAddressAndName;
							return deviceString;
						}
						break;
					}
				case IndicatorLogicType.Zone:
					{
                        if (indicatorDevice.IndicatorLogic.ZoneUIDs.Count > 0)
                            return "Зоны: " + GetCommaSeparatedZones(indicatorDevice.IndicatorLogic.Zones);
						break;
					}
			}
			return "";
		}
	}
}