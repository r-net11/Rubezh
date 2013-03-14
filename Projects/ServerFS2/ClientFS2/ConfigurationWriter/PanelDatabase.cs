using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using System.Diagnostics;
using ClientFS2.ConfigurationWriter.Classes;

namespace ClientFS2.ConfigurationWriter
{
	public class PanelDatabase
	{
		public Device ParentPanel { get; set; }
		public BytesDatabase BytesDatabase1 { get; set; }
		public BytesDatabase BytesDatabase2 { get; set; }
		public List<TableBase> Tables { get; set; }
		public List<TableGroup> TableGroups { get; set; }
		public TableGroup RemoteZonesTableGroup { get; set; }
		public TableGroup LocalZonesTableGroup { get; set; }
		public List<TableBase> LocalZoneTables { get; set; }
		public List<TableBase> RemoteDeviceTables { get; set; }
		public List<TableBase> DirectionsTables { get; set; }

		public PanelDatabase(Device panelDevice)
		{
			ParentPanel = panelDevice;
			BytesDatabase1 = new BytesDatabase();
			BytesDatabase2 = new BytesDatabase();
			Tables = new List<TableBase>();
			TableGroups = new List<TableGroup>();
			RemoteZonesTableGroup = new TableGroup();
			LocalZonesTableGroup = new TableGroup();
			LocalZoneTables = new List<TableBase>();
			RemoteDeviceTables = new List<TableBase>();
			DirectionsTables = new List<TableBase>();

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
				BytesDatabase2.Add(table.BytesDatabase);
			}
			foreach (var table in Tables)
			{
				foreach (var referenceBytesDatabase in table.ReferenceBytesDatabase)
				{
					BytesDatabase2.Add(referenceBytesDatabase);
				}
			}
			BytesDatabase2.Order();
			BytesDatabase2.ResolveTableReferences();
			BytesDatabase2.ResolverReferences();

			CreateDatabase1();
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
					DriverType = devicesGroup.DriverType,
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
			table.BytesDatabase.SetGroupName("");
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

		void CreateDatabase1()
		{
			var headBytesDatabase = new BytesDatabase();
			headBytesDatabase.AddString("base", "Сигнатура базы", 4);
			headBytesDatabase.AddShort((short)1, "Версия базы");
			headBytesDatabase.AddReference((ByteDescription)null, "Абсолютный указатель на конец базы во внешней энергонезависимой паияти");
			headBytesDatabase.AddReference((ByteDescription)null, "Абсолютный указатель на конец блока, записываемого в память кристалла");
			headBytesDatabase.SetGroupName("");
			BytesDatabase1.Add(headBytesDatabase);

			foreach (var tableGroup in TableGroups)
			{
				var bytesDatabase = new BytesDatabase();
				bytesDatabase.AddReferenceToTable(tableGroup.Tables.FirstOrDefault(), tableGroup.Name);
				bytesDatabase.AddByte((byte)tableGroup.Length, "Длина записи в таблице");
				bytesDatabase.AddShort((short)tableGroup.Count, "Текущее число записей в таблице");
				bytesDatabase.SetGroupName(tableGroup.Name);
				BytesDatabase1.Add(bytesDatabase);
			}

			var emptyBytesDatabase = new BytesDatabase();
			for (int i = 0; i < 1542 - BytesDatabase1.ByteDescriptions.Count; i++)
			{
				emptyBytesDatabase.AddByte(0);
			}
			emptyBytesDatabase.SetGroupName("");
			BytesDatabase1.Add(emptyBytesDatabase);

			CreateLocalZonesHeaders();
			CreateOuterDevicesHeaders();
			CreateDirectionsHeaders();
			CreateLocalDevicesHeaders();
			AddLocalZonesHeaderPointers();
			AddOuterZonesHeaders();
			AddOuterDevicesHeadersPointers();
			AddLocalDevicesHeadersPointers();
			AddServiceTablePointers();
			AddDirectionsHeadersPointers();

			BytesDatabase1.Add(LocalZonesBytesDatabase);
			BytesDatabase1.Add(RemoteDevicesBytesDatabase);
			foreach (var localDevicesBytesDatabase in LocalDevicesBytesDatabase)
			{
				BytesDatabase1.Add(localDevicesBytesDatabase);
			}
			BytesDatabase1.Add(DirectionsBytesDatabase);

			BytesDatabase1.Order();
			BytesDatabase1.ResolveTableReferences();
			foreach (var byteDescription in BytesDatabase1.ByteDescriptions)
			{
				if (byteDescription.TableBaseReference != null)
				{
					byteDescription.AddressReference = BytesDatabase2.ByteDescriptions.FirstOrDefault(x => x.TableHeader != null && x.TableHeader.UID == byteDescription.TableBaseReference.UID);
				}
			}
			BytesDatabase1.ResolverReferences();
		}

		void AddOuterZonesHeaders()
		{
			var bytesDatabase = new BytesDatabase();
			bytesDatabase.AddReferenceToTable(RemoteZonesTableGroup.Tables.FirstOrDefault(), "Абсолютный указатель на таблицу");
			bytesDatabase.AddByte((byte)RemoteZonesTableGroup.Length, "Длина записи в таблице");
			bytesDatabase.AddShort((short)RemoteZonesTableGroup.Count, "Текущее число записей в таблице");
			bytesDatabase.SetGroupName("Указатели Внешние зоны");
			BytesDatabase1.Add(bytesDatabase);
		}

		void CreateLocalZonesHeaders()
		{
			var bytesDatabase = new BytesDatabase();
			foreach (var table in LocalZoneTables)
			{
				bytesDatabase.AddReferenceToTable(table, "Абсолютный адрес размещения зоны");
			}
			bytesDatabase.SetGroupName("Локальные зоны");
			LocalZonesBytesDatabase = bytesDatabase;
		}

				void AddLocalZonesHeaderPointers()
		{
			var bytesDatabase = new BytesDatabase();
			bytesDatabase.AddReference(LocalZonesBytesDatabase, "Абсолютный указатель на таблицу");
			var length = 0;
			if (LocalZoneTables.Count > 0)
				length = 3;
			bytesDatabase.AddByte((byte)length, "Длина записи в таблице");
			bytesDatabase.AddShort((short)LocalZoneTables.Count, "Текущее число записей в таблице");
			bytesDatabase.SetGroupName("Указатель на указатели на локальные зоны");
			BytesDatabase1.Add(bytesDatabase);
		}

		void CreateOuterDevicesHeaders()
		{
			var bytesDatabase = new BytesDatabase();
			foreach (var table in RemoteDeviceTables)
			{
				bytesDatabase.AddReferenceToTable(table, "Абсолютный адрес размещения внешнего устройства");
			}
			bytesDatabase.SetGroupName("Внешние устройства");
			RemoteDevicesBytesDatabase = bytesDatabase;
		}

		void AddOuterDevicesHeadersPointers()
		{
			var bytesDatabase = new BytesDatabase();
			bytesDatabase.AddReference(RemoteDevicesBytesDatabase, "Абсолютный указатель на таблицу");
			var length = 0;
			if (RemoteDeviceTables.Count > 0)
				length = 3;
			bytesDatabase.AddByte((byte)length, "Длина записи в таблице");
			bytesDatabase.AddShort((short)RemoteDeviceTables.Count, "Текущее число записей в таблице");
			bytesDatabase.SetGroupName("Указатель на указатели на Внешние устройства");
			BytesDatabase1.Add(bytesDatabase);
		}

		void CreateDirectionsHeaders()
		{
			var bytesDatabase = new BytesDatabase();
			foreach (var table in DirectionsTables)
			{
				bytesDatabase.AddReferenceToTable(table, "Абсолютный адрес размещения Направления");
			}
			bytesDatabase.SetGroupName("Направления");
			DirectionsBytesDatabase = bytesDatabase;
		}

		void AddDirectionsHeadersPointers()
		{
			var bytesDatabase = new BytesDatabase();
			bytesDatabase.AddReference(DirectionsBytesDatabase, "Абсолютный указатель на таблицу");
			var length = 0;
			if (DirectionsTables.Count > 0)
				length = 3;
			bytesDatabase.AddByte((byte)length, "Длина записи в таблице");
			bytesDatabase.AddShort((short)DirectionsTables.Count, "Текущее число записей в таблице");
			bytesDatabase.SetGroupName("Указатель на указатели на Направления");
			BytesDatabase1.Add(bytesDatabase);
		}

		void AddServiceTablePointers()
		{
			var bytesDatabase = new BytesDatabase();
			bytesDatabase.AddReference((BytesDatabase)null, "Абсолютный указатель на таблицу");
			bytesDatabase.AddByte((byte)0, "Длина записи в таблице");
			bytesDatabase.AddShort((short)0, "Текущее число записей в таблице");
			bytesDatabase.SetGroupName("Указатель Служебную таблицу");
			BytesDatabase1.Add(bytesDatabase);
		}

		void CreateLocalDevicesHeaders()
		{
			var devicesOnShleifs = new List<DevicesOnShleif>();
			for (int i = 1; i <= ParentPanel.Driver.ShleifCount; i++)
			{
				var devicesOnShleif = new DevicesOnShleif()
				{
					ShleifNo = i
				};
				devicesOnShleifs.Add(devicesOnShleif);
			}
			foreach (var device in ParentPanel.Children)
			{
				if (device.ParentPanel.UID == ParentPanel.UID)
				{
					var devicesOnShleif = devicesOnShleifs.FirstOrDefault(x => x.ShleifNo == device.ShleifNo);
					devicesOnShleif.Devices.Add(device);
				}
			}
			foreach (var devicesOnShleif in devicesOnShleifs)
			{
				var bytesDatabase = new BytesDatabase();
				foreach (var device in devicesOnShleif.Devices)
				{
					var table = Tables.FirstOrDefault(x => x.UID == device.UID);
					bytesDatabase.AddReferenceToTable(table, "Абсолютный адрес размещения устройства " + device.PresentationAddressAndName);
				}
				bytesDatabase.SetGroupName("Устройства шлейфа " + devicesOnShleif.ShleifNo.ToString());
				LocalDevicesBytesDatabase.Add(bytesDatabase);
			}
		}

		void AddLocalDevicesHeadersPointers()
		{
			for(int i = 0; i < 16; i++)
			{
				BytesDatabase shleifBytesDatabase = null;
				if (i < LocalDevicesBytesDatabase.Count)
					shleifBytesDatabase = LocalDevicesBytesDatabase[i];
				var bytesDatabase = new BytesDatabase();
				bytesDatabase.AddReference(shleifBytesDatabase, "Абсолютный указатель на таблицу");
				var length = 0;
				if (shleifBytesDatabase != null)
					length = 3;
				bytesDatabase.AddByte((byte)length, "Длина записи в таблице");
				var count = 0;
				if(shleifBytesDatabase != null)
					count = shleifBytesDatabase.ByteDescriptions.Count / 3;
				bytesDatabase.AddByte((byte)count, "Текущее число записей в таблице");
				bytesDatabase.SetGroupName("Указатель на указатели на локальное устройство шлейфа " + (i+1).ToString());
				BytesDatabase1.Add(bytesDatabase);
			}
		}

		BytesDatabase LocalZonesBytesDatabase;
		BytesDatabase RemoteDevicesBytesDatabase;
		List<BytesDatabase> LocalDevicesBytesDatabase = new List<BytesDatabase>();
		BytesDatabase DirectionsBytesDatabase;
	}
}