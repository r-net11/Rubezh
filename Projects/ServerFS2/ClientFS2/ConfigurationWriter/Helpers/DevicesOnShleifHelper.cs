using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter
{
	public static class DevicesOnShleifHelper
	{
		public static List<DevicesOnShleif> GetLocalForZone(Device parentPanel, Zone zone)
		{
			var devicesOnShleifs = new List<DevicesOnShleif>();
			for (int i = 1; i <= parentPanel.Driver.ShleifCount; i++)
			{
				var devicesOnShleif = new DevicesOnShleif()
				{
					ShleifNo = i
				};
				devicesOnShleifs.Add(devicesOnShleif);
			}
			foreach (var device in zone.DevicesInZoneLogic)
			{
				if (device.ParentPanel.UID == parentPanel.UID)
				{
					var devicesOnShleif = devicesOnShleifs.FirstOrDefault(x => x.ShleifNo == device.ShleifNo);
					devicesOnShleif.Devices.Add(device);
				}
			}
			return devicesOnShleifs;
		}

		public static List<DevicesOnShleif> GetRemoteForZone(Device parentPanel, Zone zone)
		{
			var devicesOnShleifs = new List<DevicesOnShleif>();
			for (int i = 1; i <= parentPanel.Driver.ShleifCount; i++)
			{
				var devicesOnShleif = new DevicesOnShleif()
				{
					ShleifNo = i
				};
				devicesOnShleifs.Add(devicesOnShleif);
			}
			foreach (var device in zone.DevicesInZoneLogic)
			{
				if (device.ParentPanel.UID != parentPanel.UID)
				{
					var devicesOnShleif = devicesOnShleifs.FirstOrDefault(x => x.ShleifNo == device.ShleifNo);
					devicesOnShleif.Devices.Add(device);
				}
			}
			return devicesOnShleifs;
		}

		public static List<DevicesOnShleif> GetLocalForPanel(Device parentPanel)
		{
			var devicesOnShleifs = new List<DevicesOnShleif>();
			for (int i = 1; i <= parentPanel.Driver.ShleifCount; i++)
			{
				var devicesOnShleif = new DevicesOnShleif()
				{
					ShleifNo = i
				};
				devicesOnShleifs.Add(devicesOnShleif);
			}
			foreach (var device in parentPanel.Children)
			{
				if (device.ParentPanel.UID == parentPanel.UID)
				{
					var devicesOnShleif = devicesOnShleifs.FirstOrDefault(x => x.ShleifNo == device.ShleifNo);
					if (devicesOnShleif != null)
					{
						devicesOnShleif.Devices.Add(device);
					}
				}
			}
			return devicesOnShleifs;
		}
	}
}