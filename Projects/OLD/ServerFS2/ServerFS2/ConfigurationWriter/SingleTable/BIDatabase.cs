﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FiresecAPI.Models;

namespace ServerFS2.ConfigurationWriter
{
	public class BIDatabase : NonPanelDatabase
	{
		List<IndicatorItem> IndicatorItems;

		public BIDatabase(Device device, SystemDatabaseCreator configurationWriterHelper)
		{
			IndicatorItems = new List<IndicatorItem>();

			ConfigurationWriterHelper = configurationWriterHelper;
			ParentPanel = device;
			BytesDatabase = new BytesDatabase();

			Initialize();
			CreateTables();
			CreateRootBytes();

			var crcBytes = BytesDatabase.GetBytes().ToList();
			crcBytes.RemoveRange(0, 0x4000);
			crcBytes.RemoveRange(0, 74);
			var crc16Value = Crc16Helper.ComputeChecksum(crcBytes);
			BytesDatabase.SetShort(Crc16ByteDescription, crc16Value);

			CreateRootBytes();
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
					switch(device.IndicatorLogic.Device.Driver.DriverType)
					{
						case DriverType.PumpStation:
							indicatorItem.PumpStations.Add(deviceIndicator);
							break;

						case DriverType.Pump:
						case DriverType.JokeyPump:
						case DriverType.Compressor:
						case DriverType.DrenazhPump:
						case DriverType.CompensationPump:
							indicatorItem.Pumps.Add(deviceIndicator);
							break;

						case DriverType.AM1_T:
							indicatorItem.AM1_T_Devices.Add(deviceIndicator);
							break;

						default:
							indicatorItem.Devices.Add(deviceIndicator);
							break;
					}
				}
			}
			foreach (var indicatorItem in IndicatorItems)
			{
				indicatorItem.ZoneIndicators = indicatorItem.ZoneIndicators.OrderBy(x => x.Zone.No).ToList();
			}
		}

		void CreateTables()
		{
			FirstTable = new BytesDatabase();
			for (int i = 0; i < 0x4000; i++)
			{
				FirstTable.AddByte(0);
			}
			FirstTable.AddShort(5, "Версия БД");
			Crc16ByteDescription = FirstTable.AddShort(0, "CRC от ROM части базы", ignoreUnequal: true);
			var lengtByteDescription = FirstTable.AddInt(0, "Размер БД");
			FirstTable.AddShort(IndicatorItems.Count, "Число приборов");
			BytesDatabase.Add(FirstTable);

			BytesDatabase.AddByte(1, "Хэш");
			for (int i = 0; i < 16; i++)
			{
				BytesDatabase.AddByte(0, "Хэш", ignoreUnequal: true);
			}
			for (int i = 0; i < 47; i++)
			{
				BytesDatabase.AddByte(255, "Доп. информация");
			}


            IndicatorItems = IndicatorItems.OrderBy(x => x.ParentPanel.IntAddress).ToList();

			foreach (var indicatorItem in IndicatorItems)
			{
				var panelDatabase = ConfigurationWriterHelper.PanelDatabases.FirstOrDefault(x => x.ParentPanel.UID == indicatorItem.ParentPanel.UID);

				var paneBytesDatabase = new BytesDatabase("Запись прибора");

				paneBytesDatabase.AddByte(indicatorItem.ParentPanel.IntAddress, "Номер прибора");
				for (int i = 0; i < 16; i++)
				{
					var value = panelDatabase.FlashDatabase.LastTable.BytesDatabase.ByteDescriptions[i + 1].Value;
					paneBytesDatabase.AddByte(value, "MD5", ignoreUnequal:true);
				}
				var offset = panelDatabase.FlashDatabase.LastTable.BytesDatabase.ByteDescriptions.FirstOrDefault().Offset;
				var offsetBytes = BitConverter.GetBytes(offset + 1);
				for (int i = 0; i < 3; i++)
				{
					paneBytesDatabase.AddByte(offsetBytes[i], "Смещение MD5");
				}
				paneBytesDatabase.AddShort(indicatorItem.ZoneIndicators.Count, "Количество зон");
				indicatorItem.ZonesReference = paneBytesDatabase.AddReference(new ByteDescription(), "Смещение зон");
				paneBytesDatabase.AddShort(indicatorItem.Devices.Count, "Количество ИУ");
				indicatorItem.DevicesReference = paneBytesDatabase.AddReference(new ByteDescription(), "Смещение ИУ");
				paneBytesDatabase.AddShort(indicatorItem.AM1_T_Devices.Count, "Количество ТМ");
				indicatorItem.AM1_TReference = paneBytesDatabase.AddReference(new ByteDescription(), "Смещение ТМ");
				paneBytesDatabase.AddShort(indicatorItem.PumpStations.Count, "Количество НС");
				indicatorItem.PumpStationReference = paneBytesDatabase.AddReference(new ByteDescription(), "Смещение НС");
				paneBytesDatabase.AddShort(indicatorItem.Pumps.Count, "Количество Насосов");
				indicatorItem.PumpReference = paneBytesDatabase.AddReference(new ByteDescription(), "Смещение Насосов");

				BytesDatabase.Add(paneBytesDatabase);
			}

			foreach (var indicatorItem in IndicatorItems)
			{
				var panelDatabase = ConfigurationWriterHelper.PanelDatabases.FirstOrDefault(x => x.ParentPanel.UID == indicatorItem.ParentPanel.UID);

				var firstFlag = true;
				foreach (var zoneIndicator in indicatorItem.ZoneIndicators)
				{
					var paneBytesDatabase = new BytesDatabase("Зона " + zoneIndicator.Zone.PresentationName);
					var zoneTable = panelDatabase.FlashDatabase.LocalZonesTableGroup.Tables.FirstOrDefault(x => x.UID == zoneIndicator.Zone.UID) as ZoneTable;
					var offset = zoneTable.BytesDatabase.ByteDescriptions.FirstOrDefault().Offset;
					var offsetBytes = BitConverter.GetBytes(offset);
					for (int i = 0; i < 3; i++)
					{
						paneBytesDatabase.AddByte(offsetBytes[i], "Смещение");
					}
					paneBytesDatabase.AddShort(zoneTable.BinaryZone.LocalNo, "Локальный номер");
					var zoneType = 0;
					if (zoneIndicator.Zone.ZoneType == ZoneType.Guard)
						zoneType = 1;
					paneBytesDatabase.AddByte(zoneType, "ID Зоны");
					var deviceNo = (zoneIndicator.IndicatorDevice.Parent.IntAddress - 1) * 50 + zoneIndicator.IndicatorDevice.IntAddress;
					paneBytesDatabase.AddByte(deviceNo, "Номер светодиода");
					BytesDatabase.Add(paneBytesDatabase);

					if (firstFlag)
					{
						indicatorItem.ZonesReference.AddressReference = paneBytesDatabase.ByteDescriptions.FirstOrDefault();
						firstFlag = false;
					}
				}

				firstFlag = true;
				var sortedDevices = indicatorItem.Devices.OrderBy(x => x.Device.IntAddress).ToList();
				foreach (var deviceIndicator in sortedDevices)
				{
					var paneBytesDatabase = new BytesDatabase("Устройство ИУ " + deviceIndicator.Device.DottedPresentationNameAndAddress);

					EffectorDeviceTable effectorDeviceTable = null;
					foreach (var tableGroup in panelDatabase.FlashDatabase.DevicesTableGroups)
					{
						effectorDeviceTable = tableGroup.Tables.FirstOrDefault(x => x.UID == deviceIndicator.Device.UID) as EffectorDeviceTable;
						if (effectorDeviceTable != null)
						{
							break;
						}
					}
					var offset = effectorDeviceTable.BytesDatabase.ByteDescriptions.FirstOrDefault().Offset + 3;
					var offsetBytes = BitConverter.GetBytes(offset);
					for (int i = 0; i < 3; i++)
					{
						paneBytesDatabase.AddByte(offsetBytes[i], "Смещение");
					}
					paneBytesDatabase.AddByte(effectorDeviceTable.Device.AddressOnShleif, "Адрес");
					paneBytesDatabase.AddByte(effectorDeviceTable.Device.ShleifNo - 1, "Шлейф");

					var firstValue = 16 * (int)deviceIndicator.IndicatorDevice.IndicatorLogic.OnColor + (int)deviceIndicator.IndicatorDevice.IndicatorLogic.OffColor;
					var sectondValue = 16 * (int)deviceIndicator.IndicatorDevice.IndicatorLogic.FailureColor + (int)deviceIndicator.IndicatorDevice.IndicatorLogic.ConnectionColor;
					paneBytesDatabase.AddByte(firstValue, "Индикация");
					paneBytesDatabase.AddByte(sectondValue, "Индикация");
					for (int i = 0; i < 14; i++)
					{
						paneBytesDatabase.AddByte(0, "Индикация");
					}

					var deviceCode = FiresecAPI.Models.DriversHelper.GetCodeForDriver(effectorDeviceTable.Device.Driver.DriverType);
					paneBytesDatabase.AddByte(deviceCode, "Тип ИУ");
					var deviceNo = (deviceIndicator.IndicatorDevice.Parent.IntAddress - 1) * 50 + deviceIndicator.IndicatorDevice.IntAddress;
					paneBytesDatabase.AddByte(deviceNo, "Номер светодиода");
					BytesDatabase.Add(paneBytesDatabase);

					if (firstFlag)
					{
						indicatorItem.DevicesReference.AddressReference = paneBytesDatabase.ByteDescriptions.FirstOrDefault();
						firstFlag = false;
					}
				}

				firstFlag = true;
				var sortedAM1_T_Devices = indicatorItem.AM1_T_Devices.OrderBy(x => x.Device.IntAddress).ToList();
				foreach (var deviceIndicator in sortedAM1_T_Devices)
				{
					var paneBytesDatabase = new BytesDatabase("Устройство АМ1-Т " + deviceIndicator.Device.DottedPresentationNameAndAddress);

					SensorDeviceTable sensorDeviceTable = null;
					foreach (var tableGroup in panelDatabase.FlashDatabase.DevicesTableGroups)
					{
						sensorDeviceTable = tableGroup.Tables.FirstOrDefault(x => x.UID == deviceIndicator.Device.UID) as SensorDeviceTable;
						if (sensorDeviceTable != null)
						{
							break;
						}
					}
					var offset = sensorDeviceTable.BytesDatabase.ByteDescriptions.FirstOrDefault().Offset + 2;
					var offsetBytes = BitConverter.GetBytes(offset);
					for (int i = 0; i < 3; i++)
					{
						paneBytesDatabase.AddByte(offsetBytes[i], "Смещение");
					}
					paneBytesDatabase.AddByte(sensorDeviceTable.Device.AddressOnShleif, "Адрес");
					paneBytesDatabase.AddByte(sensorDeviceTable.Device.ShleifNo - 1, "Шлейф");

					var firstValue = 16 * (int)deviceIndicator.IndicatorDevice.IndicatorLogic.OnColor + (int)deviceIndicator.IndicatorDevice.IndicatorLogic.OffColor;
					var sectondValue = 16 * (int)deviceIndicator.IndicatorDevice.IndicatorLogic.FailureColor + (int)deviceIndicator.IndicatorDevice.IndicatorLogic.ConnectionColor;
					paneBytesDatabase.AddByte(firstValue, "Индикация");
					paneBytesDatabase.AddByte(sectondValue, "Индикация");
					for (int i = 0; i < 14; i++)
					{
						paneBytesDatabase.AddByte(0, "Индикация");
					}

					var deviceCode = FiresecAPI.Models.DriversHelper.GetCodeForDriver(sensorDeviceTable.Device.Driver.DriverType);
					paneBytesDatabase.AddByte(deviceCode, "Тип ИУ");
					var deviceNo = (deviceIndicator.IndicatorDevice.Parent.IntAddress - 1) * 50 + deviceIndicator.IndicatorDevice.IntAddress;
					paneBytesDatabase.AddByte(deviceNo, "Номер светодиода");
					BytesDatabase.Add(paneBytesDatabase);

					if (firstFlag)
					{
						indicatorItem.AM1_TReference.AddressReference = paneBytesDatabase.ByteDescriptions.FirstOrDefault();
						firstFlag = false;
					}
				}

				firstFlag = true;
				foreach (var deviceIndicator in indicatorItem.PumpStations)
				{
					var paneBytesDatabase = new BytesDatabase("НС " + deviceIndicator.Device.DottedPresentationNameAndAddress);

					EffectorDeviceTable effectorDeviceTable = null;
					foreach (var tableGroup in panelDatabase.FlashDatabase.DevicesTableGroups)
					{
						effectorDeviceTable = tableGroup.Tables.FirstOrDefault(x => x.UID == deviceIndicator.Device.UID) as EffectorDeviceTable;
						if (effectorDeviceTable != null)
						{
							break;
						}
					}
					var offset = effectorDeviceTable.BytesDatabase.ByteDescriptions.FirstOrDefault().Offset + 3;
					var offsetBytes = BitConverter.GetBytes(offset);
					for (int i = 0; i < 3; i++)
					{
						paneBytesDatabase.AddByte(offsetBytes[i], "Смещение");
					}
					paneBytesDatabase.AddByte(effectorDeviceTable.Device.PumpAddress, "Адрес");

					var deviceNo = (deviceIndicator.IndicatorDevice.Parent.IntAddress - 1) * 50 + deviceIndicator.IndicatorDevice.IntAddress;
					paneBytesDatabase.AddByte(deviceNo, "Номер светодиода");
					BytesDatabase.Add(paneBytesDatabase);

					if (firstFlag)
					{
						indicatorItem.PumpStationReference.AddressReference = paneBytesDatabase.ByteDescriptions.FirstOrDefault();
						firstFlag = false;
					}
				}

				firstFlag = true;
				var sortedPumps = indicatorItem.Pumps.OrderBy(x => x.Device.PumpAddress).ToList();
				foreach (var deviceIndicator in sortedPumps)
				{
					var paneBytesDatabase = new BytesDatabase("Насос " + deviceIndicator.Device.DottedPresentationNameAndAddress);

					EffectorDeviceTable effectorDeviceTable = null;
					foreach (var tableGroup in panelDatabase.FlashDatabase.DevicesTableGroups)
					{
						effectorDeviceTable = tableGroup.Tables.FirstOrDefault(x => x.UID == deviceIndicator.Device.Parent.UID) as EffectorDeviceTable;
						if (effectorDeviceTable != null)
						{
							break;
						}
					}

					var offset = effectorDeviceTable.BytesDatabase.ByteDescriptions.FirstOrDefault(x => x.Description == "Адрес насоса " + deviceIndicator.Device.PumpAddress).Offset + 8;
					var offsetBytes = BitConverter.GetBytes(offset);
					for (int i = 0; i < 3; i++)
					{
						paneBytesDatabase.AddByte(offsetBytes[i], "Смещение");
					}
					paneBytesDatabase.AddByte(deviceIndicator.Device.PumpAddress, "Адрес");
					paneBytesDatabase.AddByte(0, "Шлейф");

					var deviceNo = (deviceIndicator.IndicatorDevice.Parent.IntAddress - 1) * 50 + deviceIndicator.IndicatorDevice.IntAddress;
					paneBytesDatabase.AddByte(deviceNo, "Номер светодиода");
					BytesDatabase.Add(paneBytesDatabase);

					if (firstFlag)
					{
						indicatorItem.PumpReference.AddressReference = paneBytesDatabase.ByteDescriptions.FirstOrDefault();
						firstFlag = false;
					}
				}
			}

			Tables.Add(FirstTable);
			BytesDatabase.SetShort(lengtByteDescription, BytesDatabase.ByteDescriptions.Count - 0x4000 - 74);
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
		public ByteDescription AM1_TReference { get; set; }
		public ByteDescription PumpStationReference { get; set; }
		public ByteDescription PumpReference { get; set; }
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