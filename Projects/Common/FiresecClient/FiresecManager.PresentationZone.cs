using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		public static string GetClausePresentationName(List<ulong> zones)
		{
			if (zones.Count > 0)
			{
				var orderedZones = zones.OrderBy(x => x).ToList();
				ulong prevZoneNo = orderedZones[0];
				List<List<ulong>> groupOfZones = new List<List<ulong>>();

				for (int i = 0; i < orderedZones.Count; i++)
				{
					var zoneNo = orderedZones[i];
					var haveZonesBetween = DeviceConfiguration.Zones.Any(x => (x.No > prevZoneNo) && (x.No < zoneNo));
					if (haveZonesBetween)
					{
						groupOfZones.Add(new List<ulong>() { zoneNo });
					}
					else
					{
						if (groupOfZones.Count == 0)
						{
							groupOfZones.Add(new List<ulong>() { zoneNo });
						}
						else
						{
							groupOfZones.Last().Add(zoneNo);
						}
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

		public static string GetPresentationZone(ZoneLogic zoneLogic)
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

				if (clause.DeviceUID != Guid.Empty)
				{
					result += "Сработка устройства " + clause.Device.PresentationAddress + " - " + clause.Device.Driver.Name;
					continue;
				}

				if (clause.State == ZoneLogicState.Failure)
				{
					result += "состояние неисправность прибора";
					continue;
				}

				result += "состояние " + clause.State.ToDescription();

				string stringOperation = null;
				switch (clause.Operation)
				{
					case ZoneLogicOperation.All:
						stringOperation = "во всех зонах из";
						break;

					case ZoneLogicOperation.Any:
						stringOperation = "в любой зоне из";
						break;

					default:
						break;
				}

				result += " " + stringOperation + " " + GetClausePresentationName(clause.Zones);
			}

			return result;
		}

		public static string GetPresentationZone(Device device)
		{
			if (device.Driver.IsZoneDevice)
			{
				var zone = DeviceConfiguration.Zones.FirstOrDefault(x => x.No == device.ZoneNo);
				if (zone != null)
					return zone.PresentationName;
				return "";
			}

			if (device.Driver.IsZoneLogicDevice && device.ZoneLogic != null)
				return GetPresentationZone(device.ZoneLogic);
			if (device.Driver.IsIndicatorDevice && device.IndicatorLogic != null)
				return device.IndicatorLogic.ToString();
			if ((device.Driver.DriverType == DriverType.PDUDirection) && (device.PDUGroupLogic != null))
				return device.PDUGroupLogic.ToString();

			return "";
		}
	}
}