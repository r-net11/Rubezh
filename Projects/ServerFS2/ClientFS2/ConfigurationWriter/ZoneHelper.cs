using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter
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