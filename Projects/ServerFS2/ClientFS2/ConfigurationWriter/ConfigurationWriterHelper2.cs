using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter
{
	public class ConfigurationWriterHelper2
	{
		public ConfigurationWriterHelper2()
		{
			BytesDatabase1 = new BytesDatabase();
			BytesDatabase2 = new BytesDatabase();
		}

		public BytesDatabase BytesDatabase1 { get; set; }
		public BytesDatabase BytesDatabase2 { get; set; }

		public void Run()
		{
			ZonePanelRelations.Initialize();
			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.IsPanel)
				{
					FormDeviceDatabase(device);
				}
			}
		}

		void FormDeviceDatabase(Device panelDevice)
		{
			var localZones = GetLocalZonesForPanelDevice(panelDevice);
			foreach (var zone in localZones)
			{
				var binaryZone = new BinaryZone(panelDevice, zone);
			}
			var remote = new BinaryRemoteZone(panelDevice);
			var binaryDirections = new BinaryDirection(panelDevice);
		}

		List<Zone> GetLocalZonesForPanelDevice(Device panelDevice)
		{
			var localZones = new List<Zone>();
			foreach (var zone in ConfigurationManager.DeviceConfiguration.Zones)
			{
				foreach (var device in zone.DevicesInZone)
				{
					if (device.ParentPanel.UID == panelDevice.UID)
						localZones.Add(zone);
				}
				foreach (var device in zone.DevicesInZoneLogic)
				{
					if (device.ParentPanel.UID == panelDevice.UID)
						localZones.Add(zone);
				}
			}
			return localZones;
		}
	}
}