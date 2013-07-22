using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace ServerFS2.ConfigurationWriter
{
	public class ZonePanelRelationsInfo
	{
		public List<ZonePanelItem> ZonePanelItems { get; set; }
		public ZonePanelRelationsInfo()
		{
			ZonePanelItems = new List<ZonePanelItem>();
			foreach (var zone in ConfigurationManager.Zones)
			{
				foreach (var device in zone.DevicesInZone)
				{
					Add(zone, device.ParentPanel, false);
				}
			}
			foreach (var zone in ConfigurationManager.Zones)
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
				//var localZoneNo = 1;
				var localZones = GetAllZonesForPanel(device, false);
				foreach (var zone in localZones)
				{
					//zone.No = localZoneNo;
					//localZoneNo++;
				}
				var remoteZones = GetAllZonesForPanel(device, true);
				foreach (var zone in remoteZones)
				{
					//zone.No = localZoneNo;
					//localZoneNo++;
				}
			}
		}

		public List<Device> GetAllPanelDevices()
		{
			var devices = new List<Device>();
			foreach (var device in ConfigurationManager.Devices)
			{
				if (device.Driver.IsPanel)
					devices.Add(device);
			}
			return devices;
		}

		public List<ZonePanelItem> GetAllZonesForPanel(Device panelDevice, bool isRemote = false)
		{
			var zones = new List<ZonePanelItem>();
			foreach (var zonePanelItem in ZonePanelItems)
			{
				if (zonePanelItem.PanelDevice.UID == panelDevice.UID && zonePanelItem.IsRemote == isRemote)
					zones.Add(zonePanelItem);
			}
			return zones;
		}

		void Add(Zone zone, Device panelDevice, bool isRemote)
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
}