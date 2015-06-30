using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace ServerFS2.ConfigurationWriter
{
	public static class ZoneHelper
	{
		public static List<Device> GetZonePanels(Zone zone)
		{
			var devices = new HashSet<Device>();
			foreach (var zoneDevice in zone.DevicesInZone)
			{
				devices.Add(zoneDevice.ParentPanel);
			}
			return devices.ToList();
		}
	}
}