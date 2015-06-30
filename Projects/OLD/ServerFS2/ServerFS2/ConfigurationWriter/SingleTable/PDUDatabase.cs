﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace ServerFS2.ConfigurationWriter
{
public class PDUDatabase : NonPanelDatabase
	{
		List<PDUItem> PDUItems;

		public PDUDatabase(Device device, SystemDatabaseCreator configurationWriterHelper)
		{
			PDUItems = new List<PDUItem>();

			ConfigurationWriterHelper = configurationWriterHelper;
			ParentPanel = device;
			BytesDatabase = new BytesDatabase();

			Initialize();
			CreateTables();
			CreateRootBytes();

			var crcBytes = BytesDatabase.GetBytes();
			crcBytes.RemoveRange(0, 0x4000);
			crcBytes.RemoveRange(0, 12);
			crcBytes.RemoveRange(0, 64);
			var crc16Value = Crc16Helper.ComputeChecksum(crcBytes);
			BytesDatabase.SetShort(Crc16ByteDescription, crc16Value);

			CreateRootBytes();
		}

		void Initialize()
		{
			foreach (var directionDevice in ParentPanel.Children)
			{
				foreach(var pduGroupDevice in directionDevice.PDUGroupLogic.Devices)
				{
					var pduItem = AddPDUItem(directionDevice, pduGroupDevice.Device.ParentPanel);
					var devicePDUDirection = new DevicePDUDirection()
					{
						Device = directionDevice,
						PDUGroupDevice = pduGroupDevice
					};
					pduItem.DevicePDUDirections.Add(devicePDUDirection);
				}
			}
		}

		void CreateTables()
		{
			FirstTable = new BytesDatabase();
			for (int i = 0; i < 0x4000; i++)
			{
				FirstTable.AddByte(0);
			}
			FirstTable.AddShort(4, "Версия БД");
			Crc16ByteDescription = FirstTable.AddShort(0, "CRC от ROM части базы", ignoreUnequal: true);
			var lengtByteDescription = FirstTable.AddInt(0, "Размер БД");
			FirstTable.AddShort(PDUItems.Count, "Количество приборов");

			var devicesCount = 0;
			foreach (var pduItem in PDUItems)
			{
				devicesCount += pduItem.DevicePDUDirections.Count;
			}
			FirstTable.AddShort(devicesCount, "Количество ИУ");

			FirstTable.AddByte(1, "Хэш");
			for (int i = 0; i < 16; i++)
			{
				FirstTable.AddByte(0, "Хэш", ignoreUnequal: true);
			}
			for (int i = 0; i < 47; i++)
			{
				FirstTable.AddByte(255, "Доп. информация");
			}

			BytesDatabase.Add(FirstTable);

			foreach (var pduItem in PDUItems)
			{
				var panelDatabase = ConfigurationWriterHelper.PanelDatabases.FirstOrDefault(x => x.ParentPanel.UID == pduItem.ParentPanel.UID);

				var paneBytesDatabase = new BytesDatabase("Прибор " + pduItem.ParentPanel.DottedPresentationNameAndAddress);

				paneBytesDatabase.AddByte(pduItem.ParentPanel.IntAddress, "Номер прибора");
				for (int i = 0; i < 16; i++)
				{
					var value = panelDatabase.FlashDatabase.LastTable.BytesDatabase.ByteDescriptions[i + 1].Value;
					paneBytesDatabase.AddByte(value, "MD5", ignoreUnequal: true);
				}
				var offset = panelDatabase.FlashDatabase.LastTable.BytesDatabase.ByteDescriptions.FirstOrDefault().Offset;
				var offsetBytes = BitConverter.GetBytes(offset + 1);
				for (int i = 0; i < 4; i++)
				{
					paneBytesDatabase.AddByte(offsetBytes[i], "Смещение MD5");
				}

				var deviceCode = FiresecAPI.Models.DriversHelper.GetCodeForDriver(pduItem.ParentPanel.Driver.DriverType);
				paneBytesDatabase.AddByte(deviceCode, "Тип прибора");

				BytesDatabase.Add(paneBytesDatabase);
			}

			var emptyBytesDatabase = new BytesDatabase("Пусто");
			var emptyBytesCount = 0x42CC-BytesDatabase.ByteDescriptions.Count + 64;
			for (int i = 0; i < emptyBytesCount; i++)
			{
				emptyBytesDatabase.AddByte(255, "Пустой байт");
			}
			//emptyBytesDatabase.AddByte(0, "Пустой байт");
			BytesDatabase.Add(emptyBytesDatabase);

			var pduTables = new List<PDUTable>();
			foreach (var pduItem in PDUItems)
			{
				var panelDatabase = ConfigurationWriterHelper.PanelDatabases.FirstOrDefault(x => x.ParentPanel.UID == pduItem.ParentPanel.UID);

				foreach (var devicePDUDirection in pduItem.DevicePDUDirections)
				{
					var pduTable = new PDUTable(devicePDUDirection, panelDatabase);
					pduTables.Add(pduTable);
				}
			}

			pduTables = pduTables.OrderBy(x => x.Device.IntAddress * 256 + x.Device.ParentPanel.IntAddress).ToList();
			foreach (var pduTable in pduTables)
			{
				BytesDatabase.Add(pduTable.BytesDatabase);
			}

			Tables.Add(FirstTable);
			BytesDatabase.SetShort(lengtByteDescription, BytesDatabase.ByteDescriptions.Count - 0x4000 - 12 - 64);
		}

		PDUItem AddPDUItem(Device pduDirectionDevice, Device device)
		{
			PDUItem pduItem = PDUItems.FirstOrDefault(x => x.ParentPanel.UID == device.UID);
			if (pduItem == null)
			{
				pduItem = new PDUItem(pduDirectionDevice, device);
				PDUItems.Add(pduItem);
			}
			return pduItem;
		}
	}
}