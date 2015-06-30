﻿using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace ServerFS2.ConfigurationWriter
{
	public class RomDatabase
	{
		FlashDatabase FlashDatabase;
		public BytesDatabase BytesDatabase { get; set; }

		BytesDatabase LocalZonesBytesDatabase;
		BytesDatabase RemoteDevicesBytesDatabase;
		List<BytesDatabase> LocalDevicesBytesDatabase = new List<BytesDatabase>();
		BytesDatabase DirectionsBytesDatabase;

		public RomDatabase(FlashDatabase flashDatabase, int startOffset)
		{
			FlashDatabase = flashDatabase;
			BytesDatabase = new BytesDatabase();

			var headBytesDatabase = new BytesDatabase("Заголовок");
			if (FlashDatabase.ParentPanel.Driver.DriverType == DriverType.BUNS || FlashDatabase.ParentPanel.Driver.DriverType == DriverType.USB_BUNS)
			{
				headBytesDatabase.AddString("BUNS", "Сигнатура базы", 4);
			}
			else
			{
				headBytesDatabase.AddString("BASE", "Сигнатура базы", 4);
			}
			headBytesDatabase.AddShort(5, "Версия базы");
			headBytesDatabase.AddReference(FlashDatabase.BytesDatabase.ByteDescriptions.Last(), "Абсолютный указатель на конец базы во внешней энергонезависимой паияти");
			var pointerToLast = headBytesDatabase.AddReference((ByteDescription)null, "Абсолютный указатель на конец блока, записываемого в память кристалла");
			BytesDatabase.Add(headBytesDatabase);

			foreach (var tableGroup in FlashDatabase.DevicesTableGroups)
			{
				var bytesDatabase = new BytesDatabase(tableGroup.Name);
				bytesDatabase.AddReferenceToTable(tableGroup.Tables.FirstOrDefault(), tableGroup.Name);
				bytesDatabase.AddByte(tableGroup.ComputedLength, "Длина записи в таблице");
				bytesDatabase.AddShort(tableGroup.Count, "Текущее число записей в таблице");
				BytesDatabase.Add(bytesDatabase);
			}

			var emptyBytesDatabase = new BytesDatabase();
			for (int i = 0; i < 1542 - BytesDatabase.ByteDescriptions.Count; i++)
			{
				emptyBytesDatabase.AddByte(0);
			}
			BytesDatabase.Add(emptyBytesDatabase);

			CreateLocalZonesHeaders();
			CreateRemoteDevicesHeaders();
			CreateDirectionsHeaders();
			CreateLocalDevicesHeaders();
			AddLocalZonesHeaderPointers();
			AddRemoteZonesHeaders();
			AddRemoteDevicesHeadersPointers();
			AddLocalDevicesHeadersPointers();
			AddServiceTablePointers();
			AddDirectionsHeadersPointers();

            BytesDatabase.Add(DirectionsBytesDatabase);
			BytesDatabase.Add(LocalZonesBytesDatabase);
			BytesDatabase.Add(RemoteDevicesBytesDatabase);
			foreach (var localDevicesBytesDatabase in LocalDevicesBytesDatabase)
			{
				BytesDatabase.Add(localDevicesBytesDatabase);
			}

			pointerToLast.AddressReference = BytesDatabase.ByteDescriptions.LastOrDefault();

			BytesDatabase.Order(startOffset);
			BytesDatabase.ResolveTableReferences();
			foreach (var byteDescription in BytesDatabase.ByteDescriptions)
			{
				if (byteDescription.TableBaseReference != null)
				{
					byteDescription.AddressReference = FlashDatabase.BytesDatabase.ByteDescriptions.FirstOrDefault(x => x.TableHeader != null && x.TableHeader.ReferenceUID == byteDescription.TableBaseReference.ReferenceUID);
				}
			}
			BytesDatabase.ResolveReferences();

			var allBytes = BytesDatabase.GetBytes();
			var crc16Value = Crc16Helper.ComputeChecksum(allBytes);
			FlashDatabase.BytesDatabase.SetShort(FlashDatabase.Crc16ByteDescription, crc16Value);
		}

		void AddRemoteZonesHeaders()
		{
			var tableBase = FlashDatabase.RemoteZonesTableGroup.Tables.FirstOrDefault();
			if(tableBase == null)
				tableBase = FlashDatabase.LocalZonesTableGroup.Tables.FirstOrDefault();
			var bytesDatabase = new BytesDatabase("Указатели Внешние зоны");
			bytesDatabase.AddReferenceToTable(tableBase, "Абсолютный указатель на таблицу");
			bytesDatabase.AddByte(FlashDatabase.RemoteZonesTableGroup.Length, "Длина записи в таблице");
			bytesDatabase.AddShort(FlashDatabase.RemoteZonesTableGroup.Count, "Текущее число записей в таблице");
			BytesDatabase.Add(bytesDatabase);
		}

		void CreateLocalZonesHeaders()
		{
			var bytesDatabase = new BytesDatabase("Локальные зоны");
			foreach (var table in FlashDatabase.LocalZonesTableGroup.Tables)
			{
				bytesDatabase.AddReferenceToTable(table, "Абсолютный адрес размещения локальной зоны " + (table as ZoneTable).Zone.PresentationName);
			}
			LocalZonesBytesDatabase = bytesDatabase;
		}

		void AddLocalZonesHeaderPointers()
		{
			var bytesDatabase = new BytesDatabase("Указатель на указатели на локальные зоны");
			bytesDatabase.AddReference(LocalZonesBytesDatabase, "Абсолютный указатель на таблицу");
			bytesDatabase.AddByte(0, "Длина записи в таблице");
			bytesDatabase.AddShort(FlashDatabase.LocalZonesTableGroup.Tables.Count, "Текущее число записей в таблице");
			BytesDatabase.Add(bytesDatabase);
		}

		void CreateRemoteDevicesHeaders()
		{
			var bytesDatabase = new BytesDatabase("Внешние устройства");
			foreach (var table in FlashDatabase.RemoteDeviceTables)
			{
				bytesDatabase.AddReferenceToTable(table, "Абсолютный адрес размещения внешнего устройства");
			}
			RemoteDevicesBytesDatabase = bytesDatabase;
		}

		void AddRemoteDevicesHeadersPointers()
		{
			var bytesDatabase = new BytesDatabase("Указатель на указатели на Внешние устройства");
			bytesDatabase.AddReference(RemoteDevicesBytesDatabase, "Абсолютный указатель на таблицу");
			bytesDatabase.AddByte(0, "Длина записи в таблице");
			bytesDatabase.AddShort(0, "Текущее число записей в таблице");
			BytesDatabase.Add(bytesDatabase);
		}

		void CreateDirectionsHeaders()
		{
            var maxDirectionNo = 0;
            var localBinaryPanel = BinaryConfigurationHelper.Current.BinaryPanels.FirstOrDefault(x => x.ParentPanel.UID == this.FlashDatabase.ParentPanel.UID);
            if (localBinaryPanel.TempDirections.Count > 0)
            {
                maxDirectionNo = localBinaryPanel.TempDirections.Max(x => x.Id);
            }
            DirectionsBytesDatabase = new BytesDatabase("Направления");
            for (int i = 1; i <= maxDirectionNo; i++)
            {
                var direction = localBinaryPanel.TempDirections.FirstOrDefault(x => x.Id == i);
                if (direction != null)
                {
                    var table = FlashDatabase.DirectionsTableGroup.Tables.FirstOrDefault(x => x.UID == direction.UID);
                    DirectionsBytesDatabase.AddReferenceToTable(table, "Абсолютный адрес размещения Направления");
                }
                else
                {
                    TableBase table = null;
                    DirectionsBytesDatabase.AddReferenceToTable(table, "Пропуск направления");
                }
            }
		}

		void AddDirectionsHeadersPointers()
		{
			var bytesDatabase = new BytesDatabase("Указатель на указатели на Направления");
			bytesDatabase.AddReference(DirectionsBytesDatabase, "Абсолютный указатель на таблицу");
			bytesDatabase.AddByte(0, "Длина записи в таблице");
            bytesDatabase.AddShort(DirectionsBytesDatabase.ByteDescriptions.Count / 3, "Текущее число записей в таблице");
			BytesDatabase.Add(bytesDatabase);
		}

		void AddServiceTablePointers()
		{
			var bytesDatabase = new BytesDatabase("Указатель Служебную таблицу");
			bytesDatabase.AddReferenceToTable(FlashDatabase.AddressListTable, "Абсолютный указатель на таблицу");
			bytesDatabase.AddByte(1, "Длина записи в таблице");
			bytesDatabase.AddShort(FlashDatabase.AddressListTable.BytesDatabase.ByteDescriptions.Count, "Текущее число записей в таблице");
			BytesDatabase.Add(bytesDatabase);
		}

		void CreateLocalDevicesHeaders()
		{
			var devicesOnShleifs = DevicesOnShleifHelper.GetLocalForPanelToMax(FlashDatabase.ParentPanel);
			foreach (var devicesOnShleif in devicesOnShleifs)
			{
				var bytesDatabase = new BytesDatabase("Устройства шлейфа " + devicesOnShleif.ShleifNo.ToString());
				foreach (var device in devicesOnShleif.Devices)
				{
					TableBase table = null;
					var tableName = "Пропуск устройства";
					if (device != null)
					{
						table = FlashDatabase.Tables.FirstOrDefault(x => x.UID == device.UID);
						tableName = "Абсолютный адрес размещения устройства " + device.PresentationAddressAndName;
					}
					bytesDatabase.AddReferenceToTable(table, tableName);
				}
				LocalDevicesBytesDatabase.Add(bytesDatabase);
			}
		}

		void AddLocalDevicesHeadersPointers()
		{
			for (int i = 0; i < 16; i++)
			{
				BytesDatabase shleifBytesDatabase = null;
				if (i < LocalDevicesBytesDatabase.Count)
					shleifBytesDatabase = LocalDevicesBytesDatabase[i];
				var bytesDatabase = new BytesDatabase("Указатель на указатели на локальное устройство шлейфа " + (i + 1).ToString());
				bytesDatabase.AddReference(shleifBytesDatabase, "Абсолютный указатель на таблицу");
				bytesDatabase.AddByte(0, "Длина записи в таблице");
				var count = 0;
				if (shleifBytesDatabase != null)
					count = shleifBytesDatabase.ByteDescriptions.Count / 3;
				bytesDatabase.AddByte(count, "Текущее число записей в таблице");
				BytesDatabase.Add(bytesDatabase);
			}
		}
	}
}