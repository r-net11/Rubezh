using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter
{
	public class PanelDatabase
	{
		public Device ParentPanel { get; set; }
		public BytesDatabase BytesDatabase1 { get; set; }
		public BytesDatabase BytesDatabase2 { get; set; }
		public List<BytesDatabase> ReferenceBytesDatabase { get; set; }
		public List<TableBase> Tables { get; set; }

		public PanelDatabase(Device panelDevice)
		{
			ParentPanel = panelDevice;
			BytesDatabase1 = new BytesDatabase();
			BytesDatabase2 = new BytesDatabase();
			ReferenceBytesDatabase = new List<BytesDatabase>();
			Tables = new List<TableBase>();

			CreateDevices();
			CreateZones();
			CreateDirections();

			foreach (var table in Tables)
			{
				var firstByteDescriptions = table.BytesDatabase.ByteDescriptions.FirstOrDefault();
				if (firstByteDescriptions != null)
					firstByteDescriptions.TableHeader = table;
			}
			foreach (var table in Tables)
			{
				table.Create();
			}
			foreach (var table in Tables)
			{
				BytesDatabase2.Add(table.BytesDatabase);
			}
			foreach (var table in Tables)
			{
				ReferenceBytesDatabase.AddRange(table.ReferenceBytesDatabase);
			}
			foreach (var bytesDatabase in ReferenceBytesDatabase)
			{
				foreach(var byteDescription in bytesDatabase.ByteDescriptions)
				{
					if (byteDescription.TableBaseReference != null)
					{
						;
					}
				}
				BytesDatabase2.Add(bytesDatabase);
			}
			BytesDatabase2.Order();
			BytesDatabase2.ResolverDeviceHeaderReferences();
			BytesDatabase2.ResolverReferences();
		}

		void CreateDevices()
		{
			foreach (var device in ParentPanel.Children)
			{
				if (device.Driver.Category == DeviceCategoryType.Sensor)
				{
					var sensorDeviceTable = new SensorDeviceTable(this, device);
					Tables.Add(sensorDeviceTable);
				}
				if (device.Driver.Category == DeviceCategoryType.Effector)
				{
					var effectorDeviceTable = new EffectorDeviceTable(this, device);
					Tables.Add(effectorDeviceTable);
				}
			}
		}

		void CreateZones()
		{
			var localZones = GetLocalZonesForPanelDevice();
			foreach (var zone in localZones)
			{
				var zoneTable = new ZoneTable(this, zone);
				Tables.Add(zoneTable);
			}

			var remoteZones = ZonePanelRelations.GetAllZonesForPanel(ParentPanel, true);
			foreach (var zone in remoteZones)
			{
				var remoteZoneTable = new RemoteZoneTable(this, zone);
				Tables.Add(remoteZoneTable);
			}
		}

		void CreateDirections()
		{
			var localDirections = new List<Direction>();
			foreach (var direction in ConfigurationManager.DeviceConfiguration.Directions)
			{
				foreach (var zoneUID in direction.ZoneUIDs)
				{
					var zone = ConfigurationManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == zoneUID);
					if (zone != null)
					{
						if (ZonePanelRelations.IsLocalZone(zone, ParentPanel))
							localDirections.Add(direction);
					}
				}
			}
			foreach (var direction in localDirections)
			{
				var binaryDirection = new DirectionTable(this, direction);
				Tables.Add(binaryDirection);
			}
		}

		List<Zone> GetLocalZonesForPanelDevice()
		{
			var localZones = new List<Zone>();
			foreach (var zone in ConfigurationManager.DeviceConfiguration.Zones)
			{
				foreach (var device in zone.DevicesInZone)
				{
					if (device.ParentPanel.UID == ParentPanel.UID)
						localZones.Add(zone);
				}
				foreach (var device in zone.DevicesInZoneLogic)
				{
					if (device.ParentPanel.UID == ParentPanel.UID)
						localZones.Add(zone);
				}
			}
			return localZones;
		}
	}
}