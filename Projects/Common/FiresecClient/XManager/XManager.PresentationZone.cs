using System;
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
				var indx = 0;
				foreach(var zoneUID in device.ZoneUIDs)
				{
					var zone = DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == zoneUID);
					if (zone != null)
					{
						if (indx > 0)
						stringBuilder.Append(", ");
						stringBuilder.Append(zone.PresentationName);
						indx++;
					}
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
            foreach (var clause in DeviceLogic.Clauses)
            {
                stringBuilder.Append(clause.StateType.ToDescription() + " в ");
                stringBuilder.Append(clause.ClauseOperationType.ToDescription() + " ");
                stringBuilder.Append(GetCommaSeparatedDevices(clause.Devices));
                stringBuilder.Append(GetCommaSeparatedZones(clause.Zones));
				stringBuilder.Append(GetCommaSeparatedDirections(clause.Directions));
            }
            return stringBuilder.ToString();
        }

		public static string GetCommaSeparatedZones(List<XZone> zones)
		{
			if (zones.Count() > 0)
			{
				var orderedZones = zones.OrderBy(x => x.No).Select(x=>x.No).ToList();
				int prevZoneNo = orderedZones[0];
				List<List<ushort>> groupOfZones = new List<List<ushort>>();

				for (int i = 0; i < orderedZones.Count; i++)
				{
					var zoneNo = orderedZones[i];
					var haveZonesBetween = DeviceConfiguration.Zones.Any(x => (x.No > prevZoneNo) && (x.No < zoneNo));
					if (haveZonesBetween)
					{
						groupOfZones.Add(new List<ushort>() { zoneNo });
					}
					else
					{
						if (groupOfZones.Count == 0)
						{
							groupOfZones.Add(new List<ushort>() { zoneNo });
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
				stringBuilder.Append(device.Driver.ShortName + device.DottedAddress);
				deviceCount++;
			}
			return stringBuilder.ToString();
		}

		public static string GetCommaSeparatedDirections(IEnumerable<XDirection> directions)
		{
			var stringBuilder = new StringBuilder();
			var directionCount = 0;
			foreach (var direction in directions)
			{
				if (directionCount > 0)
					stringBuilder.Append(", ");
				stringBuilder.Append(direction.PresentationName);
				directionCount++;
			}
			return stringBuilder.ToString();
		}
	}
}