using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using XFiresecAPI;

namespace FiresecClient
{
	public partial class XManager
	{
		public static string GetPresentationZone(XDevice device)
		{
			if (device.Driver.HasZone)
			{
				var stringBuilder = new StringBuilder();
				if (device.Zones == null)
					device.Zones = new List<XZone>();
				if (device.Zones.Count == 1)
				{
					stringBuilder.Append(device.Zones[0].PresentationName);
				}
				else if (device.Zones.Count > 1)
				{
					stringBuilder.Append("зоны: ");
					stringBuilder.Append(GetCommaSeparatedZones(device.Zones));
				}
				return stringBuilder.ToString();
			}

			if (device.Driver.HasLogic && device.DeviceLogic != null)
				return GetPresentationZone(device.DeviceLogic);

			return "";
		}

		public static string GetPresentationZone(XDeviceLogic DeviceLogic)
		{
			var stringBuilder = new StringBuilder();
			var index = 0;
			foreach (var clause in DeviceLogic.Clauses)
			{
				if (index > 0)
					stringBuilder.Append(" " + clause.ClauseJounOperationType.ToDescription() + " ");

				if (clause.ClauseConditionType == ClauseConditionType.IfNot)
					stringBuilder.Append("Если НЕ ");
				stringBuilder.Append(clause.StateType.ToDescription() + " в ");
				stringBuilder.Append(clause.ClauseOperationType.ToDescription() + " ");
				stringBuilder.Append(GetCommaSeparatedDevices(clause.Devices));
				stringBuilder.Append(GetCommaSeparatedZones(clause.Zones));
				stringBuilder.Append(GetCommaSeparatedDirections(clause.Directions));
				index++;
			}
			return stringBuilder.ToString();
		}

		public static string GetCommaSeparatedZones(List<XZone> zones)
		{
			if (zones.Count() > 0)
			{
				var orderedZones = zones.OrderBy(x => x.No).Select(x => x.No).ToList();
				int prevZoneNo = orderedZones[0];
				var groupOfZones = new List<List<int>>();

				for (int i = 0; i < orderedZones.Count; i++)
				{
					var zoneNo = orderedZones[i];
					var haveZonesBetween = Zones.Any(x => (x.No > prevZoneNo) && (x.No < zoneNo));
					if (haveZonesBetween)
					{
						groupOfZones.Add(new List<int>() { zoneNo });
					}
					else
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

		public static string GetCommaSeparatedDevices(IEnumerable<XDevice> devices)
		{
			var stringBuilder = new StringBuilder();
			var deviceCount = 0;
			foreach (var device in devices)
			{
				if (deviceCount > 0)
					stringBuilder.Append(", ");
				stringBuilder.Append(device.ShortName + device.DottedAddress);
				deviceCount++;
			}
			return stringBuilder.ToString();
		}

		public static string GetCommaSeparatedDirections(IEnumerable<XDirection> directions)
		{
			if (directions.Count() > 0)
			{
				var orderedDirections = directions.OrderBy(x => x.No).Select(x => x.No).ToList();
				int prevDirectionNo = orderedDirections[0];
				List<List<ushort>> groupOfDirections = new List<List<ushort>>();

				for (int i = 0; i < orderedDirections.Count; i++)
				{
					var directionNo = orderedDirections[i];
					var haveDirectionsBetween = Zones.Any(x => (x.No > prevDirectionNo) && (x.No < directionNo));
					if (haveDirectionsBetween)
					{
						groupOfDirections.Add(new List<ushort>() { directionNo });
					}
					else
					{
						if (groupOfDirections.Count == 0)
						{
							groupOfDirections.Add(new List<ushort>() { directionNo });
						}
						else
						{
							groupOfDirections.Last().Add(directionNo);
						}
					}
					prevDirectionNo = directionNo;
				}

				var presenrationDirections = new StringBuilder();
				for (int i = 0; i < groupOfDirections.Count; i++)
				{
					var directionGroup = groupOfDirections[i];

					if (i > 0)
						presenrationDirections.Append(", ");

					presenrationDirections.Append(directionGroup.First().ToString());
					if (directionGroup.Count > 1)
					{
						presenrationDirections.Append(" - " + directionGroup.Last().ToString());
					}
				}

				return presenrationDirections.ToString();
			}
			return "";
		}
	}
}