using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using System.IO;

namespace ClientFS2.ConfigurationWriter
{
	public class BIDatabase : SingleTable
	{
		List<IndicatorItem> IndicatorItems;

		public BIDatabase(Device device, ConfigurationWriterHelper configurationWriterHelper)
		{
			IndicatorItems = new List<IndicatorItem>();

			ConfigurationWriterHelper = configurationWriterHelper;
			ParentPanel = device;
			BytesDatabase = new BytesDatabase();

			Initialize();
			CreateEmptyTable();

			CreateRootBytes();
			MergeDatabase();
		}

		void Initialize()
		{
			foreach (var device in GetIndicatorDevices())
			{
				foreach (var zone in device.IndicatorLogic.Zones)
				{
					foreach (var zonePanel in ZoneHelper.GetZonePanels(zone))
					{
						var indicatorItem = AddIndicatorItem(device, zonePanel);
						var zoneIndicator = new ZoneIndicator()
						{
							IndicatorDevice = device,
							Zone = zone
						};
						indicatorItem.ZoneIndicators.Add(zoneIndicator);
					}
				}
				if (device.IndicatorLogic.Device != null)
				{
					if (device.IndicatorLogic.Device.Driver.DriverType == DriverType.Indicator)
						continue;
					var indicatorItem = AddIndicatorItem(device, device.IndicatorLogic.Device.ParentPanel);
					var deviceIndicator = new DeviceIndicator()
					{
						IndicatorDevice = device,
						Device = device.IndicatorLogic.Device
					};
					if (device.IndicatorLogic.Device.Driver.DriverType == DriverType.PumpStation)
					{
						indicatorItem.PumpStations.Add(deviceIndicator);
					}
					else
					{
						indicatorItem.Devices.Add(deviceIndicator);
					}
				}
			}
			foreach (var indicatorItem in IndicatorItems)
			{
				indicatorItem.ZoneIndicators = indicatorItem.ZoneIndicators.OrderBy(x => x.Zone.No).ToList();
			}
		}

		void CreateEmptyTable()
		{
			FirstTable = new BytesDatabase();
			for (int i = 0; i < 0x4000; i++)
			{
				FirstTable.AddByte(0);
			}
			FirstTable.AddShort(4, "Версия БД");
			Crc16ByteDescription = FirstTable.AddShort((short)0, "CRC от ROM части базы", true, true);
			var lengtByteDescription = FirstTable.AddInt(0, "Размер БД");
			FirstTable.AddShort((short)IndicatorItems.Count, "Число приборов");
			BytesDatabase.Add(FirstTable);

			foreach (var indicatorItem in IndicatorItems)
			{
				var panelDatabase = ConfigurationWriterHelper.PanelDatabases.FirstOrDefault(x => x.ParentPanel.UID == indicatorItem.ParentPanel.UID);
				//if (panelDatabase == null)
				//{
				//    continue;
				//}

				var paneBytesDatabase = new BytesDatabase("Запись прибора");

				paneBytesDatabase.AddByte((byte)indicatorItem.ParentPanel.IntAddress, "Номер прибора");
				for (int i = 0; i < 16; i++)
				{
					var value = panelDatabase.PanelDatabase2.LastTable.BytesDatabase.ByteDescriptions[i + 1].Value;
					paneBytesDatabase.AddByte((byte)value, "MD5", ignoreUnequal:true);
				}
				var offset = panelDatabase.PanelDatabase2.LastTable.BytesDatabase.ByteDescriptions.FirstOrDefault().Offset;
				var offsetBytes = BitConverter.GetBytes(offset + 1);
				for (int i = 0; i < 3; i++)
				{
					paneBytesDatabase.AddByte(offsetBytes[i], "Смещение MD5");
				}
				paneBytesDatabase.AddShort((short)indicatorItem.ZoneIndicators.Count, "Количество зон");
				indicatorItem.ZonesReference = paneBytesDatabase.AddReference(new ByteDescription(), "Смещение зон");
				paneBytesDatabase.AddShort((short)indicatorItem.Devices.Count, "Количество ИУ");
				indicatorItem.DevicesReference = paneBytesDatabase.AddReference(new ByteDescription(), "Смещение ИУ");
				paneBytesDatabase.AddShort((short)indicatorItem.AM1_T_Devices.Count, "Количество ТМ");
				paneBytesDatabase.AddReference(new ByteDescription(), "Смещение ТМ");
				paneBytesDatabase.AddShort((short)indicatorItem.PumpStations.Count, "Количество НС");
				indicatorItem.PumpStationReference = paneBytesDatabase.AddReference(new ByteDescription(), "Смещение НС");
				paneBytesDatabase.AddShort((short)indicatorItem.Pumps.Count, "Количество Насосов");
				paneBytesDatabase.AddReference(new ByteDescription(), "Смещение Насосов");

				BytesDatabase.Add(paneBytesDatabase);
			}

			foreach (var indicatorItem in IndicatorItems)
			{
				var panelDatabase = ConfigurationWriterHelper.PanelDatabases.FirstOrDefault(x => x.ParentPanel.UID == indicatorItem.ParentPanel.UID);
				//if (panelDatabase == null)
				//{
				//    continue;
				//}

				var firstFlag = true;
				foreach (var zoneIndicator in indicatorItem.ZoneIndicators)
				{
					var paneBytesDatabase = new BytesDatabase("Зона " + zoneIndicator.Zone.PresentationName);
					var zoneTable = panelDatabase.PanelDatabase2.LocalZonesTableGroup.Tables.FirstOrDefault(x => x.UID == zoneIndicator.Zone.UID) as ZoneTable;
					var offset = zoneTable.BytesDatabase.ByteDescriptions.FirstOrDefault().Offset;
					var offsetBytes = BitConverter.GetBytes(offset);
					for (int i = 0; i < 3; i++)
					{
						paneBytesDatabase.AddByte(offsetBytes[i], "Смещение");
					}
					paneBytesDatabase.AddShort((short)zoneTable.BinaryZone.LocalNo, "Локальный номер");
					var zoneType = 0;
					if (zoneIndicator.Zone.ZoneType == ZoneType.Guard)
						zoneType = 1;
					paneBytesDatabase.AddByte((byte)zoneType, "ID Зоны");
					var deviceNo = (zoneIndicator.IndicatorDevice.Parent.IntAddress - 1) * 50 + zoneIndicator.IndicatorDevice.IntAddress;
					paneBytesDatabase.AddByte((byte)deviceNo, "Номер светодиода");
					BytesDatabase.Add(paneBytesDatabase);

					if (firstFlag)
					{
						indicatorItem.ZonesReference.AddressReference = paneBytesDatabase.ByteDescriptions.FirstOrDefault();
						firstFlag = false;
					}
				}

				firstFlag = true;
				foreach (var deviceIndicator in indicatorItem.Devices)
				{
					var paneBytesDatabase = new BytesDatabase("Устройство ИУ " + deviceIndicator.Device.DottedPresentationNameAndAddress);

					EffectorDeviceTable effectorDeviceTable = null;
					foreach (var tableGroup in panelDatabase.PanelDatabase2.DevicesTableGroups)
					{
						effectorDeviceTable = tableGroup.Tables.FirstOrDefault(x => x.UID == deviceIndicator.Device.UID) as EffectorDeviceTable;
						if (effectorDeviceTable != null)
						{
							break;
						}
					}
					var offset = effectorDeviceTable.BytesDatabase.ByteDescriptions.FirstOrDefault().Offset;
					var offsetBytes = BitConverter.GetBytes(offset);
					for (int i = 0; i < 3; i++)
					{
						paneBytesDatabase.AddByte(offsetBytes[i], "Смещение");
					}
					paneBytesDatabase.AddByte((byte)effectorDeviceTable.Device.AddressOnShleif, "Адрес");
					paneBytesDatabase.AddByte((byte)(effectorDeviceTable.Device.ShleifNo - 1), "Шлейф");

					for (int i = 0; i < 16; i++)
					{
						paneBytesDatabase.AddByte((byte)0, "Индикация");
					}

					var deviceCode = DriversHelper.GetCodeForFriver(effectorDeviceTable.Device.Driver.DriverType);
					paneBytesDatabase.AddByte((byte)deviceCode, "Тип ИУ");
					var deviceNo = (deviceIndicator.IndicatorDevice.Parent.IntAddress - 1) * 50 + deviceIndicator.IndicatorDevice.IntAddress;
					paneBytesDatabase.AddByte((byte)deviceNo, "Номер светодиода");
					BytesDatabase.Add(paneBytesDatabase);

					if (firstFlag)
					{
						indicatorItem.DevicesReference.AddressReference = paneBytesDatabase.ByteDescriptions.FirstOrDefault();
						firstFlag = false;
					}
				}

				firstFlag = true;
				foreach (var deviceIndicator in indicatorItem.PumpStations)
				{
					var paneBytesDatabase = new BytesDatabase("НС " + deviceIndicator.Device.DottedPresentationNameAndAddress);

					EffectorDeviceTable effectorDeviceTable = null;
					foreach (var tableGroup in panelDatabase.PanelDatabase2.DevicesTableGroups)
					{
						effectorDeviceTable = tableGroup.Tables.FirstOrDefault(x => x.UID == deviceIndicator.Device.UID) as EffectorDeviceTable;
						if (effectorDeviceTable != null)
						{
							break;
						}
					}
					var offset = effectorDeviceTable.BytesDatabase.ByteDescriptions.FirstOrDefault().Offset;
					var offsetBytes = BitConverter.GetBytes(offset);
					for (int i = 0; i < 3; i++)
					{
						paneBytesDatabase.AddByte(offsetBytes[i], "Смещение");
					}
					paneBytesDatabase.AddByte((byte)effectorDeviceTable.Device.PumpAddress, "Адрес");

					var deviceNo = (deviceIndicator.IndicatorDevice.Parent.IntAddress - 1) * 50 + deviceIndicator.IndicatorDevice.IntAddress;
					paneBytesDatabase.AddByte((byte)deviceNo, "Номер светодиода");
					BytesDatabase.Add(paneBytesDatabase);

					if (firstFlag)
					{
						indicatorItem.PumpStationReference.AddressReference = paneBytesDatabase.ByteDescriptions.FirstOrDefault();
						firstFlag = false;
					}
				}
			}

			Tables.Add(FirstTable);
			BytesDatabase.SetShort(lengtByteDescription, (short)(BytesDatabase.ByteDescriptions.Count - 0x4000 - 10));
		}

		IndicatorItem AddIndicatorItem(Device indicatorDevice, Device device)
		{
			IndicatorItem indicatorItem = IndicatorItems.FirstOrDefault(x => x.ParentPanel.UID == device.UID);
			if (indicatorItem == null)
			{
				indicatorItem = new IndicatorItem(indicatorDevice, device);
				IndicatorItems.Add(indicatorItem);
			}
			return indicatorItem;
		}

		List<Device> GetIndicatorDevices()
		{
			var result = new List<Device>();
			foreach (var device in ParentPanel.Children)
			{
				foreach (var child in device.Children)
				{
					result.Add(child);
				}
			}
			return result;
		}
	}

	public class IndicatorItem
	{
		public IndicatorItem(Device indicatorDevice, Device device)
		{
			ParentPanel = device;
			IndicatorDevice = indicatorDevice;
			ZoneIndicators = new List<ZoneIndicator>();
			Devices = new List<DeviceIndicator>();
			AM1_T_Devices = new List<DeviceIndicator>();
			PumpStations = new List<DeviceIndicator>();
			Pumps = new List<DeviceIndicator>();
		}

		public Device ParentPanel { get; set; }
		public Device IndicatorDevice { get; set; }
		public List<ZoneIndicator> ZoneIndicators { get; set; }
		public List<DeviceIndicator> Devices { get; set; }
		public List<DeviceIndicator> AM1_T_Devices { get; set; }
		public List<DeviceIndicator> PumpStations { get; set; }
		public List<DeviceIndicator> Pumps { get; set; }

		public ByteDescription ZonesReference { get; set; }
		public ByteDescription DevicesReference { get; set; }
		public ByteDescription PumpStationReference { get; set; }
	}

	public class ZoneIndicator
	{
		public Zone Zone { get; set; }
		public Device IndicatorDevice { get; set; }
	}

	public class DeviceIndicator
	{
		public Device Device { get; set; }
		public Device IndicatorDevice { get; set; }
	}
}