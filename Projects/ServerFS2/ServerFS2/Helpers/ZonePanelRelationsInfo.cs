using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using ServerFS2;

namespace ClientFS2.ConfigurationWriter
{
	public static class ZonePanelRelationsInfo
	{
		public static List<ZonePanelItem> ZonePanelItems { get; set; }

		public static void Initialize()
		{
			ZonePanelItems = new List<ZonePanelItem>();
			foreach (var zone in ConfigurationManager.DeviceConfiguration.Zones)
			{
				foreach (var device in zone.DevicesInZone)
				{
					Add(zone, device.ParentPanel, false);
				}
			}
			foreach (var zone in ConfigurationManager.DeviceConfiguration.Zones)
			{
				foreach (var device in zone.DevicesInZoneLogic)
				{
					if (ZonePanelItems.Any(x => x.PanelDevice.UID != device.ParentPanel.UID))
					{
						Add(zone, device.ParentPanel, true);
					}
				}
			}
			var panelDevices = GetAllPanelDevices();
			foreach (var device in panelDevices)
			{
				var localZoneNo = 1;
				var localZones = GetAllZonesForPanel(device, false);
				foreach (var zone in localZones)
				{
					zone.No = localZoneNo;
					localZoneNo++;
				}
				var remoteZones = GetAllZonesForPanel(device, true);
				foreach (var zone in remoteZones)
				{
					zone.No = localZoneNo;
					localZoneNo++;
				}
			}
		}

		public static List<Device> GetAllPanelDevices()
		{
			var devices = new List<Device>();
			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.IsPanel)
					devices.Add(device);
			}
			return devices;
		}

		public static List<ZonePanelItem> GetAllZonesForPanel(Device panelDevice, bool isRemote = false)
		{
			var zones = new List<ZonePanelItem>();
			foreach (var zonePanelItem in ZonePanelItems)
			{
				if (zonePanelItem.PanelDevice.UID == panelDevice.UID && zonePanelItem.IsRemote == isRemote)
					zones.Add(zonePanelItem);
			}
			return zones;
		}

		public static bool IsLocalZone(Zone zone, Device panelDevice)
		{
			return ZonePanelItems.Any(x => x.Zone.UID == zone.UID && x.PanelDevice.UID == panelDevice.UID && x.IsRemote == false);
		}

		public static int GetLocalZoneNo(Zone zone, Device panelDevice)
		{
			var zonePanelItem = ZonePanelItems.FirstOrDefault(x => x.Zone.UID == zone.UID && x.PanelDevice.UID == panelDevice.UID && x.IsRemote == false);
			if (zonePanelItem != null)
			{
				return zonePanelItem.No;
			}
			return 0;
		}

		static void Add(Zone zone, Device panelDevice, bool isRemote)
		{
			if (!ZonePanelItems.Any(x => x.Zone.UID == zone.UID && x.PanelDevice.UID == panelDevice.UID && x.IsRemote == isRemote))
			{
				var zonePanelItem = new ZonePanelItem()
				{
					Zone = zone,
					PanelDevice = panelDevice,
					IsRemote = isRemote
				};
				ZonePanelItems.Add(zonePanelItem);
			}
		}
	}

	public class ZonePanelItem
	{
		public Zone Zone { get; set; }
		public Device PanelDevice { get; set; }
		public bool IsRemote { get; set; }
		public int No { get; set; }
	}
}