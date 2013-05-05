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
public class PDUDatabase : SingleTable
	{
		List<PDUItem> PDUItems;

		public PDUDatabase(Device device, ConfigurationWriterHelper configurationWriterHelper)
		{
			UnequalBytes = new List<SingleTableUnequalByteViewModel>();
			PDUItems = new List<PDUItem>();

			ConfigurationWriterHelper = configurationWriterHelper;
			ParentPanel = device;
			BytesDatabase = new BytesDatabase();

			Initialize();
			CreateEmptyTable();

			var crcBytes = BytesDatabase.GetBytes();
			crcBytes.RemoveRange(0, 10);
			var crc16Value = Crc16Helper.ComputeChecksum(crcBytes);
			BytesDatabase.SetShort(Crc16ByteDescription, (short)crc16Value);

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
			FirstTable.AddShort(2, "Версия БД");
			Crc16ByteDescription = FirstTable.AddShort((short)0, "CRC от ROM части базы", true, true);
			var lengtByteDescription = FirstTable.AddInt(0, "Размер БД");
			FirstTable.AddShort((short)PDUItems.Count, "Количество приборов");

			var devicesCount = 0;
			foreach (var pduItem in PDUItems)
			{
				devicesCount += pduItem.DevicePDUDirections.Count;
			}
			FirstTable.AddShort((short)devicesCount, "Количество ИУ");
			BytesDatabase.Add(FirstTable);

			foreach (var pduItem in PDUItems)
			{
				var panelDatabase = ConfigurationWriterHelper.PanelDatabases.FirstOrDefault(x => x.ParentPanel.UID == pduItem.ParentPanel.UID);

				var paneBytesDatabase = new BytesDatabase("Прибор " + pduItem.ParentPanel.DottedPresentationNameAndAddress);

				paneBytesDatabase.AddByte((byte)pduItem.ParentPanel.IntAddress, "Номер прибора");
				for (int i = 0; i < 16; i++)
				{
					var value = panelDatabase.PanelDatabase2.LastTable.BytesDatabase.ByteDescriptions[i + 1].Value;
					paneBytesDatabase.AddByte((byte)value, "MD5");
				}
				var offset = panelDatabase.PanelDatabase2.LastTable.BytesDatabase.ByteDescriptions.FirstOrDefault().Offset;
				var offsetBytes = BitConverter.GetBytes(offset + 1);
				for (int i = 0; i < 4; i++)
				{
					paneBytesDatabase.AddByte(offsetBytes[i], "Смещение MD5");
				}

				var deviceCode = FiresecAPI.Models.DriversHelper.GetCodeForFriver(pduItem.ParentPanel.Driver.DriverType);
				paneBytesDatabase.AddByte((byte)deviceCode, "Тип прибора");

				BytesDatabase.Add(paneBytesDatabase);
			}

			var emptyBytesDatabase = new BytesDatabase("Пусто");
			var emptyBytesCount = 0x42CC-BytesDatabase.ByteDescriptions.Count;
			for (int i = 0; i < emptyBytesCount; i++)
			{
				emptyBytesDatabase.AddByte((byte)255, "Пустой байт");
			}
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
			BytesDatabase.SetShort(lengtByteDescription, (short)(BytesDatabase.ByteDescriptions.Count - 0x4000 - 12));
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