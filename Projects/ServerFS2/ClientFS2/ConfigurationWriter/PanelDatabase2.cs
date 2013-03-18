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
		public List<ByteDescription> RootBytes { get; set; }

		public List<TableBase> Tables = new List<TableBase>();
		public TableBase FirstTable;
		public List<TableGroup> DevicesTableGroups = new List<TableGroup>();
		public TableGroup RemoteZonesTableGroup = new TableGroup("Внешние зоны");
		public TableGroup LocalZonesTableGroup = new TableGroup("Локальные зоны");
		public TableGroup DirectionsTableGroup = new TableGroup("Направления");
		public TableGroup ReferenceTableGroup = new TableGroup("Ссылки");
		public List<EffectorDeviceTable> RemoteDeviceTables = new List<EffectorDeviceTable>();
		public ByteDescription Crc16ByteDescription;

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
				foreach (var referenceTable in table.ReferenceTables)
				{
					BytesDatabase.Add(referenceTable.BytesDatabase);
					ReferenceTableGroup.Tables.Add(referenceTable);
				}
			}
			BytesDatabase.Order();
			BytesDatabase.ResolveTableReferences();
			BytesDatabase.ResolverReferences();

			CreateRootBytes();
		}

		void CreateEmptyTable()
		{
			FirstTable = new TableBase(this);
			for (int i = 0; i < 100; i++)
			{
				FirstTable.BytesDatabase.AddByte(0);
			}
			Crc16ByteDescription = FirstTable.BytesDatabase.AddShort((short)0, "CRC от ROM части базы");
			Tables.Add(FirstTable);
		}

		void CreateZones()
		{
			foreach (var zone in BinaryPanel.BinaryRemoteZones)
			{
				var remoteZoneTable = new RemoteZoneTable(this, zone);
				Tables.Add(remoteZoneTable);
				RemoteZonesTableGroup.Tables.Add(remoteZoneTable);
			}

			foreach (var zone in BinaryPanel.BinaryLocalZones)
			{
				var zoneTable = new ZoneTable(this, zone);
				Tables.Add(zoneTable);
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
				var tableGroup = new TableGroup(devicesGroup.Name);
				DevicesTableGroups.Add(tableGroup);
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
			var tableGroup = new TableGroup("Указатель на таблицу Внешних ИУ");
			DevicesTableGroups.Add(tableGroup);

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
				DirectionsTableGroup.Tables.Add(table);
			}
		}

		void CreateRootBytes()
		{
			RootBytes = new List<ByteDescription>();

			RootBytes.Add(FirstTable.GetTreeRootByteDescription());
			RootBytes.Add(RemoteZonesTableGroup.GetTreeRootByteDescription());
			RootBytes.Add(LocalZonesTableGroup.GetTreeRootByteDescription());

			foreach (var tableGroup in DevicesTableGroups)
			{
				var byteDescription = tableGroup.GetTreeRootByteDescription();
				RootBytes.Add(byteDescription);
			}

			RootBytes.Add(DirectionsTableGroup.GetTreeRootByteDescription());
			RootBytes.Add(ReferenceTableGroup.GetTreeRootByteDescription());
		}
	}
}