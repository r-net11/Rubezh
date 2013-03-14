using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using System.Diagnostics;
using ClientFS2.ConfigurationWriter.Classes;

namespace ClientFS2.ConfigurationWriter
{
	public class PanelDatabase2
	{
		public Device ParentPanel { get; set; }
		public BytesDatabase BytesDatabase { get; set; }

		public List<TableBase> Tables = new List<TableBase>();
		public List<TableGroup> TableGroups = new List<TableGroup>();
		public TableGroup RemoteZonesTableGroup = new TableGroup();
		public TableGroup LocalZonesTableGroup = new TableGroup();
		public List<TableBase> LocalZoneTables = new List<TableBase>();
		public List<TableBase> RemoteDeviceTables = new List<TableBase>();
		public List<TableBase> DirectionsTables = new List<TableBase>();

		public PanelDatabase2(Device panelDevice)
		{
			ParentPanel = panelDevice;
			BytesDatabase = new BytesDatabase();

			CreateEmptyTable();
			CreateZones();
			CreateLocalDevices();
			CreateDirections();

			foreach (var table in Tables)
			{
				table.Create();
			}
			foreach (var table in Tables)
			{
				var firstByteDescriptions = table.BytesDatabase.ByteDescriptions.FirstOrDefault();
				if (firstByteDescriptions != null)
					firstByteDescriptions.TableHeader = table;
			}
			foreach (var table in Tables)
			{
				BytesDatabase.Add(table.BytesDatabase);
			}
			foreach (var table in Tables)
			{
				foreach (var referenceBytesDatabase in table.ReferenceBytesDatabase)
				{
					BytesDatabase.Add(referenceBytesDatabase);
				}
			}
			BytesDatabase.Order();
			BytesDatabase.ResolveTableReferences();
			BytesDatabase.ResolverReferences();
		}

		void CreateRemoteDevices()
		{
			var tableGroup = new TableGroup()
			{
				Name = "Указатель на таблицу Внешних ИУ"
			};
			TableGroups.Add(tableGroup);

			var devices = GetOuterDevices();
			foreach (var device in devices)
			{
				var table = new EffectorDeviceTable(this, device, true);
				Tables.Add(table);
				tableGroup.Tables.Add(table);
				RemoteDeviceTables.Add(table);
			}
		}

		void CreateLocalDevices()
		{
			var devicesGroupHelper = new DevicesGroupHelper(ParentPanel);
			foreach (var devicesGroup in devicesGroupHelper.DevicesGroups)
			{
				if (devicesGroup.IsRemoteDevicesPointer)
				{
					CreateRemoteDevices();
					continue;
				}
				var tableGroup = new TableGroup()
				{
					Name = devicesGroup.Name
				};
				TableGroups.Add(tableGroup);
				foreach (var device in devicesGroup.Devices)
				{
					TableBase deviceTable = null;
					if (device.Driver.Category == DeviceCategoryType.Sensor)
					{
						deviceTable = new SensorDeviceTable(this, device);
					}
					if (device.Driver.Category == DeviceCategoryType.Effector)
					{
						deviceTable = new EffectorDeviceTable(this, device, false);
					}
					if (deviceTable != null)
					{
						Tables.Add(deviceTable);
						tableGroup.Tables.Add(deviceTable);
					}
				}
			}
		}

		void CreateZones()
		{
			var remoteTableGroup = new TableGroup()
			{
				Name = "Внешние зоны"
			};
			RemoteZonesTableGroup = remoteTableGroup;
			var remoteZones = ZonePanelRelations.GetAllZonesForPanel(ParentPanel, true);
			foreach (var zone in remoteZones)
			{
				var remoteZoneTable = new RemoteZoneTable(this, zone);
				Tables.Add(remoteZoneTable);
				remoteTableGroup.Tables.Add(remoteZoneTable);
			}

			var localTableGroup = new TableGroup()
			{
				Name = "Локальные зоны"
			};
			LocalZonesTableGroup = localTableGroup;
			var localZones = GetLocalZonesForPanelDevice();
			foreach (var zone in localZones)
			{
				var zoneTable = new ZoneTable(this, zone);
				Tables.Add(zoneTable);
				LocalZoneTables.Add(zoneTable);
				localTableGroup.Tables.Add(zoneTable);
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
				var table = new DirectionTable(this, direction);
				Tables.Add(table);
				DirectionsTables.Add(table);
			}
		}

		void CreateEmptyTable()
		{
			var table = new TableBase(this);
			var crcValue = 0;
			table.BytesDatabase.AddShort((short)crcValue, "CRC от ROM части базы");
			for (int i = 0; i < 98; i++)
			{
				table.BytesDatabase.AddByte(0);
			}
			Tables.Add(table);
		}

		public List<Device> GetOuterDevices()
		{
			var devices = new List<Device>();
			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			{
				foreach (var zone in device.ZonesInLogic)
				{
					foreach (var deviceInZone in zone.DevicesInZoneLogic)
					{
						if (deviceInZone.ParentPanel.UID != ParentPanel.UID)
						{
							if (!devices.Any(x => x.UID == deviceInZone.UID))
							{
								devices.Add(deviceInZone);
							}
						}
					}
				}
			}
			if (devices.Count > 0)
			{
				Trace.WriteLine("GetOuterDevices.Count=" + devices.Count.ToString());
			}
			return devices;
		}

		List<Zone> GetLocalZonesForPanelDevice()
		{
			var localZones = new List<Zone>();
			foreach (var zone in ConfigurationManager.DeviceConfiguration.Zones)
			{
				foreach (var device in zone.DevicesInZone)
				{
					if (device.ParentPanel.UID == ParentPanel.UID)
					{
						if (!localZones.Contains(zone))
							localZones.Add(zone);
					}
				}
				foreach (var device in zone.DevicesInZoneLogic)
				{
					if (device.ParentPanel.UID == ParentPanel.UID)
					{
						if (!localZones.Contains(zone))
							localZones.Add(zone);
					}
				}
			}
			return localZones;
		}
	}
}