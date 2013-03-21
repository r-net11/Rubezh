﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerFS2;

namespace ClientFS2.ConfigurationWriter
{
	public class PanelDatabase1
	{
		PanelDatabase2 PanelDatabase2;
		public BytesDatabase BytesDatabase { get; set; }

		BytesDatabase LocalZonesBytesDatabase;
		BytesDatabase RemoteDevicesBytesDatabase;
		List<BytesDatabase> LocalDevicesBytesDatabase = new List<BytesDatabase>();
		BytesDatabase DirectionsBytesDatabase;

		public PanelDatabase1(PanelDatabase2 panelDatabase2, int startOffset)
		{
			PanelDatabase2 = panelDatabase2;
			BytesDatabase = new BytesDatabase();

			var headBytesDatabase = new BytesDatabase("Заголовок");
			headBytesDatabase.AddString("BASE", "Сигнатура базы", 4);
			headBytesDatabase.AddShort((short)4, "Версия базы");
			headBytesDatabase.AddReference(PanelDatabase2.BytesDatabase.ByteDescriptions.Last(), "Абсолютный указатель на конец базы во внешней энергонезависимой паияти");
			var pointerToLast = headBytesDatabase.AddReference((ByteDescription)null, "Абсолютный указатель на конец блока, записываемого в память кристалла");
			BytesDatabase.Add(headBytesDatabase);

			foreach (var tableGroup in PanelDatabase2.DevicesTableGroups)
			{
				var bytesDatabase = new BytesDatabase(tableGroup.Name);
				bytesDatabase.AddReferenceToTable(tableGroup.Tables.FirstOrDefault(), tableGroup.Name);
				bytesDatabase.AddByte((byte)tableGroup.Length, "Длина записи в таблице");
				bytesDatabase.AddShort((short)tableGroup.Count, "Текущее число записей в таблице");
				BytesDatabase.Add(bytesDatabase);
			}

			var emptyBytesDatabase = new BytesDatabase();
			for (int i = 0; i < 1542 - BytesDatabase.ByteDescriptions.Count; i++)
			{
				emptyBytesDatabase.AddByte(0);
			}
			BytesDatabase.Add(emptyBytesDatabase);

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

			BytesDatabase.Add(LocalZonesBytesDatabase);
			BytesDatabase.Add(RemoteDevicesBytesDatabase);
			foreach (var localDevicesBytesDatabase in LocalDevicesBytesDatabase)
			{
				BytesDatabase.Add(localDevicesBytesDatabase);
			}
			BytesDatabase.Add(DirectionsBytesDatabase);

			pointerToLast.AddressReference = BytesDatabase.ByteDescriptions.LastOrDefault();

			BytesDatabase.Order(startOffset);
			BytesDatabase.ResolveTableReferences();
			foreach (var byteDescription in BytesDatabase.ByteDescriptions)
			{
				if (byteDescription.TableBaseReference != null)
				{
					byteDescription.AddressReference = PanelDatabase2.BytesDatabase.ByteDescriptions.FirstOrDefault(x => x.TableHeader != null && x.TableHeader.UID == byteDescription.TableBaseReference.UID);
				}
			}
			BytesDatabase.ResolverReferences();

			var crc16Value = Crc16Helper.ComputeChecksum(BytesDatabase.GetBytes());
			PanelDatabase2.BytesDatabase.SetShort(PanelDatabase2.Crc16ByteDescription, (short)crc16Value);
		}

		void AddOuterZonesHeaders()
		{
			var bytesDatabase = new BytesDatabase("Указатели Внешние зоны");
			bytesDatabase.AddReferenceToTable(PanelDatabase2.RemoteZonesTableGroup.Tables.FirstOrDefault(), "Абсолютный указатель на таблицу");
			bytesDatabase.AddByte((byte)PanelDatabase2.RemoteZonesTableGroup.Length, "Длина записи в таблице");
			bytesDatabase.AddShort((short)PanelDatabase2.RemoteZonesTableGroup.Count, "Текущее число записей в таблице");
			BytesDatabase.Add(bytesDatabase);
		}

		void CreateLocalZonesHeaders()
		{
			var bytesDatabase = new BytesDatabase("Локальные зоны");
			foreach (var table in PanelDatabase2.LocalZonesTableGroup.Tables)
			{
				bytesDatabase.AddReferenceToTable(table, "Абсолютный адрес размещения зоны");
			}
			LocalZonesBytesDatabase = bytesDatabase;
		}

		void AddLocalZonesHeaderPointers()
		{
			var bytesDatabase = new BytesDatabase("Указатель на указатели на локальные зоны");
			bytesDatabase.AddReference(LocalZonesBytesDatabase, "Абсолютный указатель на таблицу");
			bytesDatabase.AddByte((byte)0, "Длина записи в таблице");
			bytesDatabase.AddShort((short)PanelDatabase2.LocalZonesTableGroup.Tables.Count, "Текущее число записей в таблице");
			BytesDatabase.Add(bytesDatabase);
		}

		void CreateOuterDevicesHeaders()
		{
			var bytesDatabase = new BytesDatabase("Внешние устройства");
			foreach (var table in PanelDatabase2.RemoteDeviceTables)
			{
				bytesDatabase.AddReferenceToTable(table, "Абсолютный адрес размещения внешнего устройства");
			}
			RemoteDevicesBytesDatabase = bytesDatabase;
		}

		void AddOuterDevicesHeadersPointers()
		{
			var bytesDatabase = new BytesDatabase("Указатель на указатели на Внешние устройства");
			bytesDatabase.AddReference(RemoteDevicesBytesDatabase, "Абсолютный указатель на таблицу");
			bytesDatabase.AddByte((byte)0, "Длина записи в таблице");
			bytesDatabase.AddShort((short)PanelDatabase2.RemoteDeviceTables.Count, "Текущее число записей в таблице");
			BytesDatabase.Add(bytesDatabase);
		}

		void CreateDirectionsHeaders()
		{
			var bytesDatabase = new BytesDatabase("Направления");
			foreach (var table in PanelDatabase2.DirectionsTableGroup.Tables)
			{
				bytesDatabase.AddReferenceToTable(table, "Абсолютный адрес размещения Направления");
			}
			DirectionsBytesDatabase = bytesDatabase;
		}

		void AddDirectionsHeadersPointers()
		{
			var bytesDatabase = new BytesDatabase("Указатель на указатели на Направления");
			bytesDatabase.AddReference(DirectionsBytesDatabase, "Абсолютный указатель на таблицу");
			bytesDatabase.AddByte((byte)0, "Длина записи в таблице");
			bytesDatabase.AddShort((short)PanelDatabase2.DirectionsTableGroup.Tables.Count, "Текущее число записей в таблице");
			BytesDatabase.Add(bytesDatabase);
		}

		void AddServiceTablePointers()
		{
			var bytesDatabase = new BytesDatabase("Указатель Служебную таблицу");
			bytesDatabase.AddReferenceToTable(PanelDatabase2.ServiceTable, "Абсолютный указатель на таблицу");
			bytesDatabase.AddByte((byte)1, "Длина записи в таблице");
			bytesDatabase.AddShort((short)PanelDatabase2.ServiceTable.BytesDatabase.ByteDescriptions.Count, "Текущее число записей в таблице");
			BytesDatabase.Add(bytesDatabase);
		}

		void CreateLocalDevicesHeaders()
		{
			var devicesOnShleifs = DevicesOnShleifHelper.GetLocalForPanelToMax(PanelDatabase2.ParentPanel);
			foreach (var devicesOnShleif in devicesOnShleifs)
			{
				var bytesDatabase = new BytesDatabase("Устройства шлейфа " + devicesOnShleif.ShleifNo.ToString());
				foreach (var device in devicesOnShleif.Devices)
				{
					TableBase table = null;
					var tableName = "Пропуск устройства";
					if (device != null)
					{
						table = PanelDatabase2.Tables.FirstOrDefault(x => x.UID == device.UID);
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
				bytesDatabase.AddByte((byte)0, "Длина записи в таблице");
				var count = 0;
				if (shleifBytesDatabase != null)
					count = shleifBytesDatabase.ByteDescriptions.Count / 3;
				bytesDatabase.AddByte((byte)1, "Текущее число записей в таблице");
				BytesDatabase.Add(bytesDatabase);
			}
		}
	}
}