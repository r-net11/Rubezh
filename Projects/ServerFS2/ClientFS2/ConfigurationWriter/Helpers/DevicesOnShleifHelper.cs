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
			foreach (var device in parentPanel.GetRealChildren())
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

		public static List<DevicesOnShleif> GetLocalForPanelToMax(Device parentPanel)
		{
			var devicesOnShleifs = GetLocalForPanel(parentPanel);

			foreach (var devicesOnShleif in devicesOnShleifs)
			{
				var maxAddress = 0;
				if (devicesOnShleif.Devices.Count > 0)
					maxAddress = devicesOnShleif.Devices.Max(x => x.AddressOnShleif);

				var devices = new List<Device>();
				for (int i = 1; i <= maxAddress; i++)
				{
					devices.Add(null);
				}
				foreach (var device in devicesOnShleif.Devices)
				{
					devices[device.AddressOnShleif - 1] = device;
				}
				devicesOnShleif.Devices = devices;
			}

			return devicesOnShleifs;
		}
	}
}