using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models;
using System.IO;
using ServerFS2;

namespace ClientFS2.ConfigurationWriter
{
	public class PDUPTDatabase : SingleTable
	{
		List<PDUItem> PDUItems;

		public PDUPTDatabase(Device device, ConfigurationWriterHelper configurationWriterHelper)
		{
			UnequalBytes = new List<SingleTableUnequalByteViewModel>();
			PDUItems = new List<PDUItem>();

			ConfigurationWriterHelper = configurationWriterHelper;
			ParentPanel = device;
			BytesDatabase = new BytesDatabase();

			Initialize();
			CreateEmptyTable();

			var crcBytes = BytesDatabase.GetBytes();
			crcBytes.RemoveRange(0, 0x4C);
			var crc16Value = Crc16Helper.ComputeChecksum(crcBytes);
			BytesDatabase.SetShort(Crc16ByteDescription, crc16Value);

			CreateRootBytes();
			MergeDatabase();
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

		void CreateEmptyTable()
		{
			FirstTable = new BytesDatabase();
			for (int i = 0; i < 0x4000; i++)
			{
				FirstTable.AddByte(0);
			}
			FirstTable.AddShort(1, "Версия БД");
			Crc16ByteDescription = FirstTable.AddShort(0, "CRC от ROM части базы");
			var lengtByteDescription = FirstTable.AddInt(0, "Размер БД");
			FirstTable.AddShort(PDUItems.Count, "Количество приборов");

			var devicesCount = 0;
			foreach (var pduItem in PDUItems)
			{
				pduItem.DevicePDUDirections = pduItem.DevicePDUDirections.OrderBy(x => x.Device.IntAddress).ToList();
				devicesCount += pduItem.DevicePDUDirections.Count;
			}
			FirstTable.AddShort(devicesCount, "Количество направлений");
			BytesDatabase.Add(FirstTable);

			BytesDatabase.AddByte(1, "Хэш");
			for (int i = 0; i < 15; i++)
			{
				BytesDatabase.AddByte(255, "Хэш");
			}
			for (int i = 0; i < 48; i++)
			{
				BytesDatabase.AddByte(255, "Доп. информация");
			}

			var emptyBytesDatabase1 = new BytesDatabase("Пусто");
			var emptyBytesCount1 = 0x404C - BytesDatabase.ByteDescriptions.Count;
			for (int i = 0; i < emptyBytesCount1; i++)
			{
				emptyBytesDatabase1.AddByte(255, "Пустой байт 1");
			}
			BytesDatabase.Add(emptyBytesDatabase1);

			foreach (var pduItem in PDUItems)
			{
				var panelDatabase = ConfigurationWriterHelper.PanelDatabases.FirstOrDefault(x => x.ParentPanel.UID == pduItem.ParentPanel.UID);
				var paneBytesDatabase = new BytesDatabase("Прибор " + pduItem.ParentPanel.DottedPresentationNameAndAddress);

				paneBytesDatabase.AddByte(pduItem.ParentPanel.IntAddress, "Номер прибора");
				for (int i = 0; i < 16; i++)
				{
					var value = panelDatabase.PanelDatabase2.LastTable.BytesDatabase.ByteDescriptions[i + 1].Value;
					paneBytesDatabase.AddByte(value, "MD5");
				}
				var offset = panelDatabase.PanelDatabase2.LastTable.BytesDatabase.ByteDescriptions.FirstOrDefault().Offset;
				var offsetBytes = BitConverter.GetBytes(offset + 1);
				for (int i = 0; i < 4; i++)
				{
					paneBytesDatabase.AddByte(offsetBytes[i], "Смещение MD5");
				}

				var deviceCode = FiresecAPI.Models.DriversHelper.GetCodeForFriver(pduItem.ParentPanel.Driver.DriverType);
				paneBytesDatabase.AddByte(deviceCode, "Тип прибора");

				BytesDatabase.Add(paneBytesDatabase);
			}

			var emptyBytesDatabase2 = new BytesDatabase("Пусто");
			var emptyBytesCount2 = 0x430C-BytesDatabase.ByteDescriptions.Count;
			for (int i = 0; i < emptyBytesCount2; i++)
			{
				emptyBytesDatabase2.AddByte(255, "Пустой байт 2");
			}
			BytesDatabase.Add(emptyBytesDatabase2);


			var pduPTTables = new List<PDUPTTable>();
			foreach (var pduItem in PDUItems)
			{
				var panelDatabase = ConfigurationWriterHelper.PanelDatabases.FirstOrDefault(x => x.ParentPanel.UID == pduItem.ParentPanel.UID);
				foreach (var devicePDUDirection in pduItem.DevicePDUDirections)
				{
					var pduTable = new PDUPTTable(devicePDUDirection, panelDatabase);
					pduPTTables.Add(pduTable);
					devicePDUDirection.ReferenceToByteDescriptions = pduTable.BytesDatabase.ByteDescriptions.FirstOrDefault();
				}
			}


			foreach (var pduItem in PDUItems)
			{
				foreach (var devicePDUDirection in pduItem.DevicePDUDirections)
				{
					var mptDevice = devicePDUDirection.PDUGroupDevice.Device;
					var paneBytesDatabase = new BytesDatabase("Направление " + devicePDUDirection.Device.DottedPresentationNameAndAddress);
					paneBytesDatabase.AddByte(devicePDUDirection.Device.IntAddress, "Номер направления");
					paneBytesDatabase.AddShort(0, "Задержка запуска");
					var uiCount = 1 + mptDevice.Children.Count;
					paneBytesDatabase.AddByte(uiCount, "Количество ИУ");
					var offsetByteDescription = paneBytesDatabase.AddInt(0, "Смещение");
					offsetByteDescription.AddressReference = devicePDUDirection.ReferenceToByteDescriptions;
					BytesDatabase.Add(paneBytesDatabase);
				}
			}

			var emptyBytesDatabase3 = new BytesDatabase("Пусто");
			var emptyBytesCount3 = 0x4334 - BytesDatabase.ByteDescriptions.Count;
			for (int i = 0; i < emptyBytesCount3; i++)
			{
				emptyBytesDatabase3.AddByte(255, "Пустой байт 3");
			}
			BytesDatabase.Add(emptyBytesDatabase3);

			//pduPTTables = pduPTTables.OrderBy(x => x.Device.IntAddress * 256 + x.Device.ParentPanel.IntAddress).ToList();
			foreach (var pduTable in pduPTTables)
			{
				BytesDatabase.Add(pduTable.BytesDatabase);
			}

			Tables.Add(FirstTable);
			BytesDatabase.SetShort(lengtByteDescription, BytesDatabase.ByteDescriptions.Count - 0x404C);
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