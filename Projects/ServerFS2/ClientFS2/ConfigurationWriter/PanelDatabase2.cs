using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using System.Diagnostics;
using ClientFS2.ConfigurationWriter.Classes;
using FiresecAPI.Models.Binary;
using System.Security.Cryptography;
using System.Windows;

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
		public TableBase ServiceTable;
		public TableBase LastTable;
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
            CreateDirections();
			CreateRemoteZones();
			CreateLocalZones();
			CreateServiceTable();
			CreateDevices();
			CreateLastTable();

			foreach (var table in Tables)
			{
				table.Create();
			}
			foreach (var table in Tables)
			{
				foreach (var byteDescription in table.BytesDatabase.ByteDescriptions)
				{
					byteDescription.TableHeader = table;
				}
			}
			foreach (var table in Tables)
			{
				if (table.BytesDatabase.Name == "Адресный лист")
				{
					var count = BytesDatabase.ByteDescriptions.Count;
					var bytesBefireEnd = 256 - count % 256;
					if (bytesBefireEnd < 32)
					{
						var leftTable = new TableBase(this, "Дополняюшая таблица");
						for (int i = 0; i < bytesBefireEnd; i++)
						{
							leftTable.BytesDatabase.AddByte(0, "Дополняющий байт");
						}
						BytesDatabase.Add(leftTable.BytesDatabase);
					}
				}
				BytesDatabase.Add(table.BytesDatabase);
			}
			BytesDatabase.Order();
			BytesDatabase.ResolveTableReferences();
			BytesDatabase.ResolverReferences();

			var crcBytes = BytesDatabase.GetBytes();
			crcBytes.RemoveRange(0, 256);
			crcBytes.RemoveRange(crcBytes.Count - 36, 36);
			var md5 = MD5.Create();
			var md5Bytes = md5.ComputeHash(crcBytes.ToArray());
			for (int i = 0; i < 16; i++)
			{
				var md5Byte = md5Bytes[i];
				LastTable.BytesDatabase.ByteDescriptions[i + 1].Value = md5Byte;
			}

			CreateRootBytes();
		}

		void CreateEmptyTable()
		{
			FirstTable = new TableBase(this);
			for (int i = 0; i < 256; i++)
			{
				FirstTable.BytesDatabase.AddByte(0);
			}
			Crc16ByteDescription = FirstTable.BytesDatabase.AddShort(0, "CRC от ROM части базы");
			Tables.Add(FirstTable);
		}

		void CreateLocalZones()
		{
			foreach (var zone in BinaryPanel.BinaryLocalZones.OrderBy(x=>x.Zone.No))
			{
				var zoneTable = new ZoneTable(this, zone);
				Tables.Add(zoneTable);
				LocalZonesTableGroup.Tables.Add(zoneTable);
			}
		}

		void CreateRemoteZones()
		{
			foreach (var zone in BinaryPanel.BinaryRemoteZones)
			{
				var remoteZoneTable = new RemoteZoneTable(this, zone);
				Tables.Add(remoteZoneTable);
				RemoteZonesTableGroup.Tables.Add(remoteZoneTable);
			}
		}

		void CreateServiceTable()
		{
			var remotePanels = new HashSet<Device>();
			var addressList = new List<int>();
			for (int i = 0; i < 32; i++)
			{
				addressList.Add(0);
			}
			if (ParentPanel.Parent.Driver.DriverType != DriverType.Computer)
			{
				foreach (var binaryZones in BinaryPanel.BinaryRemoteZones)
				{
					remotePanels.Add(binaryZones.ParentPanel);
					//AddToDevicelist(addressList, binaryZones.ParentPanel);
				}
				foreach (var binaryZones in BinaryPanel.BinaryLocalZones)
				{
					foreach (var device in binaryZones.Zone.DevicesInZone)
					{
						remotePanels.Add(device.ParentPanel);
						//AddToDevicelist(addressList, device.ParentPanel);
					}
				}
				//remotePanels.Add(BinaryPanel.ParentPanel);
				//AddToDevicelist(addressList, BinaryPanel.ParentPanel);
			}

			foreach (var device in ConfigurationCash.DeviceConfiguration.Devices)
			{
				if (device.Driver.DriverType == DriverType.PDUDirection || device.Driver.DriverType == DriverType.PDU_PTDirection)
				{
					foreach (var pduDevice in device.PDUGroupLogic.Devices)
					{
						if (pduDevice.Device.ParentPanel.UID == ParentPanel.UID)
						{
							remotePanels.Add(device.ParentPanel);
							//AddToDevicelist(addressList, device.ParentPanel);
						}
					}
				}
				if (device.Driver.DriverType == DriverType.IndicationBlock)
				{
					foreach (var pageDevice in device.Children)
					{
						foreach (var indicatorDevice in pageDevice.Children)
						{
							foreach (var zone in indicatorDevice.IndicatorLogic.Zones)
							{
								if (zone.DevicesInZone.Any(x => x.ParentPanel.UID == ParentPanel.UID))
								{
									remotePanels.Add(indicatorDevice.ParentPanel);
									//AddToDevicelist(addressList, indicatorDevice.ParentPanel);
								}
							}
							if (indicatorDevice.IndicatorLogic.Device != null && indicatorDevice.IndicatorLogic.Device.ParentPanel.UID == ParentPanel.UID)
							{
								remotePanels.Add(device);
								//AddToDevicelist(addressList, device);
							}
						}
					}
				}
			}

			var remotePanelLists = remotePanels.OrderBy(x => x.IntAddress).ToList();
			remotePanelLists.RemoveAll(x => x.IntAddress == ParentPanel.IntAddress);
			for (int i = 0; i < remotePanelLists.Count; i++)
			{
				var device = remotePanelLists[i];
				var deviceCode = DriversHelper.GetCodeForFriver(device.Driver.DriverType);
				addressList[i] = deviceCode;
			}

			ServiceTable = new TableBase(this, "Адресный лист");
			foreach (var address in addressList)
			{
				ServiceTable.BytesDatabase.AddByte(address, "", true);
			}
			Tables.Add(ServiceTable);
		}

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
				var tableGroup = new TableGroup(devicesGroup.Name, devicesGroup.Length);
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
		}

		void CreateRemoteDevices()
		{
			var tableGroup = new TableGroup("Указатель на таблицу Внешних ИУ", -1);
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
            var binaryPanel = BinaryConfigurationHelper.Current.BinaryPanels.FirstOrDefault(x => x.ParentPanel.UID == ParentPanel.UID);
            if (binaryPanel.ParentPanel.Children.Any(x => x.Driver.DriverType == DriverType.Valve))
            {
                foreach (var direction in binaryPanel.TempDirections.OrderBy(x => x.Id))
                {
                    var table = new DirectionTable(this, direction);
                    Tables.Add(table);
                    DirectionsTableGroup.Tables.Add(table);
                }
            }
        }

		void CreateLastTable()
		{
			LastTable = new TableBase(this, "Последняя таблица");
			LastTable.BytesDatabase.AddByte(8, "Версия MD5");
			for (int i = 0; i < 16; i++)
			{
				LastTable.BytesDatabase.AddByte(0, "MD5", true, true);
			}
			for (int i = 0; i < 19; i++)
			{
				LastTable.BytesDatabase.AddByte(0, "Для нужд базы");
			}
			Tables.Add(LastTable);
		}

		void CreateRootBytes()
		{
			RootBytes = new List<ByteDescription>();

			RootBytes.Add(FirstTable.GetTreeRootByteDescription());
            RootBytes.Add(DirectionsTableGroup.GetTreeRootByteDescription());
			RootBytes.Add(RemoteZonesTableGroup.GetTreeRootByteDescription());
			RootBytes.Add(LocalZonesTableGroup.GetTreeRootByteDescription());

			foreach (var tableGroup in DevicesTableGroups)
			{
				var byteDescription = tableGroup.GetTreeRootByteDescription();
				RootBytes.Add(byteDescription);
			}

			RootBytes.Add(ReferenceTableGroup.GetTreeRootByteDescription());

			//var bytes = new List<byte>();
			//foreach (var byteDescription in RootBytes)
			//{
			//    bytes.Add((byte)byteDescription.Value);
			//}
			//var md5 = MD5.Create();
			//var md5Bytes = md5.ComputeHash(bytes.ToArray());
			//for (int i = 0; i < 16; i++ )
			//{
			//    var md5Byte = md5Bytes[i];
			//    LastTable.BytesDatabase.ByteDescriptions[i + 1].Value = md5Byte;
			//}

			RootBytes.Add(LastTable.GetTreeRootByteDescription());
		}
	}
}