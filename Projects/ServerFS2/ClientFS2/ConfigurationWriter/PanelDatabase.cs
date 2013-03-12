using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter
{
	public class PanelDatabase
	{
		public Device PanelDevice { get; set; }
		public BytesDatabase BytesDatabase1 { get; set; }
		public BytesDatabase BytesDatabase2 { get; set; }
		public List<BytesDatabase> ReferenceBytesDatabase { get; set; }
		public List<TableBase> Tables { get; set; }

		public PanelDatabase(Device panelDevice)
		{
			PanelDevice = panelDevice;
			BytesDatabase1 = new BytesDatabase();
			BytesDatabase2 = new BytesDatabase();
			ReferenceBytesDatabase = new List<BytesDatabase>();
			Tables = new List<TableBase>();

			CreateDevices(panelDevice);
			CreateZones(panelDevice);
			CreateDirections(panelDevice);
			foreach (var bytesDatabase in ReferenceBytesDatabase)
			{
				BytesDatabase2.Add(bytesDatabase);
			}
			BytesDatabase2.Order();
			//BytesDatabase2.ResolverDeviceHeaderReferences();
			BytesDatabase2.ResolverReferences();
		}

		void PreCreateTables()
		{

		}

		void CreateDevices(Device panelDevice)
		{
			foreach (var device in panelDevice.Children)
			{
				if (device.Driver.Category == DeviceCategoryType.Sensor)
				{
					var binaryNonIUDevice = new BinaryNonIUDevice(panelDevice, device);
					BytesDatabase2.Add(binaryNonIUDevice.BytesDatabase);
				}
				if (device.Driver.Category == DeviceCategoryType.Effector)
				{
					var binaryIUDevice = new BinaryIUDevice(panelDevice, device);
					BytesDatabase2.Add(binaryIUDevice.BytesDatabase);
				}
			}
		}

		void CreateZones(Device panelDevice)
		{
			var localZones = GetLocalZonesForPanelDevice(panelDevice);
			foreach (var zone in localZones)
			{
				var binaryLocalZone = new BinaryZone(panelDevice, zone);
				BytesDatabase2.Add(binaryLocalZone.BytesDatabase);
				ReferenceBytesDatabase.AddRange(binaryLocalZone.ReferenceBytesDatabase);
			}

			var remoteZones = ZonePanelRelations.GetAllZonesForPanel(panelDevice, true);
			foreach (var zone in remoteZones)
			{
				var binaryRemoteZone = new BinaryRemoteZone(panelDevice, zone);
				BytesDatabase2.Add(binaryRemoteZone.BytesDatabase);
			}
		}

		void CreateDirections(Device panelDevice)
		{
			var localDirections = new List<Direction>();
			foreach (var direction in ConfigurationManager.DeviceConfiguration.Directions)
			{
				foreach (var zoneUID in direction.ZoneUIDs)
				{
					var zone = ConfigurationManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == zoneUID);
					if (zone != null)
					{
						if (ZonePanelRelations.IsLocalZone(zone, panelDevice))
							localDirections.Add(direction);
					}
				}
			}
			foreach (var direction in localDirections)
			{
				var binaryDirection = new BinaryDirection(panelDevice, direction);
				BytesDatabase2.Add(binaryDirection.BytesDatabase);
			}
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