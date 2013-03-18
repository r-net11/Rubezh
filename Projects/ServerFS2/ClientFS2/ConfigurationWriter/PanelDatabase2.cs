using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using System.Diagnostics;
using ClientFS2.ConfigurationWriter.Classes;
using FiresecAPI.Models.Binary;

namespace ClientFS2.ConfigurationWriter
{
	public class PanelDatabase2
	{
		public Device ParentPanel { get; set; }
		public BytesDatabase BytesDatabase { get; set; }
		public BinaryPanel BinaryPanel { get; set; }

		public List<TableBase> Tables = new List<TableBase>();
		public List<TableGroup> TableGroups = new List<TableGroup>();
		public TableGroup RemoteZonesTableGroup = new TableGroup();
		public TableGroup LocalZonesTableGroup = new TableGroup();
		public List<ZoneTable> LocalZoneTables = new List<ZoneTable>();
		public List<EffectorDeviceTable> RemoteDeviceTables = new List<EffectorDeviceTable>();
		public List<TableBase> DirectionsTables = new List<TableBase>();

		public List<ByteDescription> RootBytes { get; set; }

		public PanelDatabase2(Device parentPanel)
		{
			ParentPanel = parentPanel;
			BytesDatabase = new BytesDatabase();
			BinaryPanel = ConfigurationWriterHelper.BinaryConfigurationHelper.BinaryPanels.FirstOrDefault(x => x.ParentPanel == ParentPanel);

			CreateEmptyTable();
			CreateZones();
			CreateDevices();
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

			CreateRootBytes();
		}

		void CreateRootBytes()
		{
			RootBytes = new List<ByteDescription>();

			var remoteZonesByteDescription = new ByteDescription()
			{
				Description = "Внешние Зоны"
			};
			RootBytes.Add(remoteZonesByteDescription);
			foreach (var table in RemoteZonesTableGroup.Tables)
			{
				var zoneByteDescription = new ByteDescription()
				{
					Description = table.BytesDatabase.Name
				};
				remoteZonesByteDescription.Children.Add(zoneByteDescription);
				foreach (var byteDescription in table.BytesDatabase.ByteDescriptions)
				{
					zoneByteDescription.Children.Add(byteDescription);
				}
			}

			var localZonesByteDescription = new ByteDescription()
			{
				Description = "Локальные Зоны"
			};
			RootBytes.Add(localZonesByteDescription);
			foreach (var table in LocalZonesTableGroup.Tables)
			{
				var zoneByteDescription = new ByteDescription()
				{
					Description = table.BytesDatabase.Name
				};
				localZonesByteDescription.Children.Add(zoneByteDescription);
				foreach (var byteDescription in table.BytesDatabase.ByteDescriptions)
				{
					zoneByteDescription.Children.Add(byteDescription);
				}
			}

			foreach (var tableGroup in TableGroups)
			{
				var deviceGroupByteDescription = new ByteDescription()
				{
					Description = tableGroup.Name
				};
				RootBytes.Add(deviceGroupByteDescription);

				foreach (var table in tableGroup.Tables)
				{
					var deviceByteDescription = new ByteDescription()
					{
						Description = table.BytesDatabase.Name
					};
					deviceGroupByteDescription.Children.Add(deviceByteDescription);
					foreach (var byteDescription in table.BytesDatabase.ByteDescriptions)
					{
						deviceByteDescription.Children.Add(byteDescription);
					}
				}
			}
		}

		void CreateZones()
		{
			RemoteZonesTableGroup = new TableGroup()
			{
				Name = "Внешние зоны"
			};
			foreach (var zone in BinaryPanel.BinaryRemoteZones)
			{
				var remoteZoneTable = new RemoteZoneTable(this, zone);
				Tables.Add(remoteZoneTable);
				RemoteZonesTableGroup.Tables.Add(remoteZoneTable);
			}

			LocalZonesTableGroup = new TableGroup()
			{
				Name = "Локальные зоны"
			};
			foreach (var zone in BinaryPanel.BinaryLocalZones)
			{
				var zoneTable = new ZoneTable(this, zone);
				Tables.Add(zoneTable);
				LocalZoneTables.Add(zoneTable);
				LocalZonesTableGroup.Tables.Add(zoneTable);
			}
		}

		static int CreateLocalDevices_Miliseconds = 0;

		void CreateDevices()
		{
			var startDateTime = DateTime.Now;

			var devicesGroupHelper = new DevicesGroupHelper(BinaryPanel);
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
				foreach (var device in devicesGroup.BinaryDevices)
				{
					TableBase deviceTable = null;
					if (device.Device.Driver.Category == DeviceCategoryType.Sensor)
					{
						deviceTable = new SensorDeviceTable(this, device.Device);
					}
					if (device.Device.Driver.Category == DeviceCategoryType.Effector)
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

			var deltaMiliseconds = (DateTime.Now - startDateTime).Milliseconds;
			CreateLocalDevices_Miliseconds += deltaMiliseconds;
			Trace.WriteLine("CreateLocalDevices_Miliseconds=" + CreateLocalDevices_Miliseconds.ToString());
		}

		void CreateRemoteDevices()
		{
			var tableGroup = new TableGroup()
			{
				Name = "Указатель на таблицу Внешних ИУ"
			};
			TableGroups.Add(tableGroup);

			foreach (var binaryRemoteDevice in BinaryPanel.BinaryRemoteDevices)
			{
				var table = RemoteDeviceTables.FirstOrDefault(x => x.UID == binaryRemoteDevice.Device.UID);
				if (table == null)
				{
					table = new EffectorDeviceTable(this, binaryRemoteDevice, true);
					Tables.Add(table);
					tableGroup.Tables.Add(table);
					RemoteDeviceTables.Add(table);
				}
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