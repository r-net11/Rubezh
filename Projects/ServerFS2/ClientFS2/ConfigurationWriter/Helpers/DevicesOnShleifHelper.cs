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

			var effectorDevices = GetDevicesInLogic(zone);
			foreach (var device in effectorDevices.OrderBy(x=>x.IntAddress))
			{
				if (device.ParentPanel.UID == parentPanel.UID)
				{
					var shleifNo = device.ShleifNo;
					if(device.Driver.DriverType == DriverType.PumpStation)
						shleifNo = 1;
					var devicesOnShleif = devicesOnShleifs.FirstOrDefault(x => x.ShleifNo == shleifNo);
					if (devicesOnShleif != null)
					{
						devicesOnShleif.Devices.Add(device);
					}
				}
			}
			return devicesOnShleifs;
		}

		public static List<Device> GetRemoteForZone(Device parentPanel, Zone zone)
		{
			var devices = new HashSet<Device>();
			foreach (var device in GetDevicesInLogic(zone))
			{
				foreach (var clause in device.ZoneLogic.Clauses)
				{
					var allZonesAreRemote = true;
					foreach (var clauseZone in clause.Zones)
					{
						if (clauseZone.DevicesInZone.FirstOrDefault().ParentPanel.UID == device.ParentPanel.UID)
							allZonesAreRemote = false;
					}
					if (clause.Operation.Value == ZoneLogicOperation.Any || allZonesAreRemote)
					//if (clause.Operation.Value == ZoneLogicOperation.Any)
					{
						foreach (var clauseZone in clause.Zones)
						{
							if (clauseZone.UID == zone.UID)
							{
								if (device.ParentPanel.UID != parentPanel.UID)
								{
									devices.Add(device);
								}
							}
						}
					}
				}
			}
			return devices.ToList();
		}

		public static List<Device> GetRemoteForZone2(Device parentPanel, Zone zone)
		{
			var devices = new HashSet<Device>();
			foreach (var device in GetDevicesInLogic(zone))
			{
				foreach (var clause in device.ZoneLogic.Clauses)
				{
					var allZonesAreRemote = true;
					foreach (var clauseZone in clause.Zones)
					{
						if (clauseZone.DevicesInZone.FirstOrDefault().ParentPanel.UID == device.ParentPanel.UID)
							allZonesAreRemote = false;
					}
					//if (clause.Operation.Value == ZoneLogicOperation.Any || allZonesAreRemote)
					if (clause.Operation.Value == ZoneLogicOperation.All)
					{
						foreach (var clauseZone in clause.Zones)
						{
							if (clauseZone.UID == zone.UID)
							{
								if (device.ParentPanel.UID != parentPanel.UID)
								{
									devices.Add(device);
								}
							}
						}
					}
				}
			}
			return devices.ToList();
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

		static List<Device> GetDevicesInLogic(Zone zone)
		{
			var result = zone.DevicesInZoneLogic;
			foreach (var device in zone.DevicesInZone)
			{
				if (device.Driver.DriverType == DriverType.MPT)
				{
					result.Add(device);
				}
			}
			var hashSet = new HashSet<Device>(result);
			return hashSet.ToList();
		}
	}
}