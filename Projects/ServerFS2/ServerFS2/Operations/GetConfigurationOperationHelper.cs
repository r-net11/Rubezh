using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FiresecAPI.Models;
using ServerFS2.ConfigurationWriter;
using ServerFS2.Helpers;
using Device = FiresecAPI.Models.Device;

namespace ServerFS2
{
	public class GetConfigurationOperationHelper
	{
		public List<byte> DeviceFlash;
		public List<byte> DeviceRom;
		Device PanelDevice;
		int shleifCount;
		int outZonesBegin;
		int outZonesCount;
		int outZonesEnd;
		List<Zone> Zones;
		ZonePanelRelationsInfo zonePanelRelationsInfo;
		DeviceConfiguration remoteDeviceConfiguration;
		bool CheckMonitoringSuspend = false;

		public GetConfigurationOperationHelper(bool checkMonitoringSuspend)
		{
			CheckMonitoringSuspend = checkMonitoringSuspend;
		}

		public DeviceConfiguration GetDeviceConfiguration(Device panelDevice)
		{
			PanelDevice = (Device)panelDevice.Clone();
			shleifCount = PanelDevice.Driver.ShleifCount;
			PanelDevice.Children = new List<Device>();
			Zones = new List<Zone>();
			remoteDeviceConfiguration = new DeviceConfiguration();
			remoteDeviceConfiguration.RootDevice = PanelDevice;
			remoteDeviceConfiguration.Devices.Add(PanelDevice);
			var panelDatabaseReader = new ReadPanelDatabaseOperationHelper(PanelDevice, CheckMonitoringSuspend);
			panelDatabaseReader.RomDBFirstIndex = panelDatabaseReader.GetRomFirstIndex(PanelDevice);
			if (panelDatabaseReader.RomDBFirstIndex == -1)
				return null;
			panelDatabaseReader.FlashDBLastIndex = panelDatabaseReader.GetFlashLastIndex(PanelDevice);
			if (panelDatabaseReader.FlashDBLastIndex == -1)
				return null;
			DeviceRom = panelDatabaseReader.GetRomDBBytes(PanelDevice);
			if (DeviceRom == null)
				return null;
			DeviceFlash = panelDatabaseReader.GetFlashDBBytes(PanelDevice);
			if (DeviceFlash == null)
				return null;

			zonePanelRelationsInfo = new ZonePanelRelationsInfo();
			ParseZonesRom(1542, panelDatabaseReader.RomDBFirstIndex);

			outZonesBegin = DeviceRom[1548] * 256 * 256 + DeviceRom[1549] * 256 + DeviceRom[1550];
			outZonesCount = DeviceRom[1552] * 256 + DeviceRom[1553];
			outZonesEnd = outZonesBegin + outZonesCount * 9;

			ParseUIDevicesRom(DriverType.MDU);
			ParseNoUIDevicesRom(DriverType.AM1_T);

			ParseUIDevicesRom(DriverType.RM_1);
			ParseUIDevicesRom(DriverType.MPT);
			ParseUIDevicesRom(DriverType.MRO);
			ParseUIDevicesRom(DriverType.MRO_2);
			ParseUIDevicesRom(DriverType.Exit);

			ParseNoUIDevicesRom(DriverType.SmokeDetector);
			ParseNoUIDevicesRom(DriverType.HeatDetector);
			ParseNoUIDevicesRom(DriverType.CombinedDetector);
			ParseNoUIDevicesRom(DriverType.HandDetector);
			ParseNoUIDevicesRom(DriverType.AM_1);
			ParseNoUIDevicesRom(DriverType.AMP_4);
			ParseNoUIDevicesRom(DriverType.AM1_O);
			ParseNoUIDevicesRom(DriverType.RadioHandDetector);
			ParseNoUIDevicesRom(DriverType.RadioSmokeDetector);
			ParseUIDevicesRom(DriverType.Valve);

			foreach (var childDevice in PanelDevice.Children)
			{
				childDevice.Parent = PanelDevice;
				remoteDeviceConfiguration.Devices.Add(childDevice);
			}
			foreach (var device in remoteDeviceConfiguration.Devices)
			{
				GetCurrentDeviceState(device);
			}
			return remoteDeviceConfiguration;
		}

		void ParseUIDevicesRom(DriverType driverType)
		{
			var romPointer = GetDeviceOffset(driverType);
			var pointer = BytesHelper.ExtractTriple(DeviceRom, romPointer);
			if (pointer != 0)
			{
				var count = BytesHelper.ExtractShort(DeviceRom, romPointer + 4); // текущее число записей в таблице
				for (var i = 0; i < count; i++)
				{
					ParseUIDevicesFlash(ref pointer, driverType);
				}
			}
		}

		void ParseUIDevicesFlash(ref int pointer, DriverType driverType)
		{
			int rawParametersLength;
			switch (driverType)
			{
				case DriverType.Valve:
					rawParametersLength = 6;
					break;
				case DriverType.MPT:
					rawParametersLength = 15;
					break;
				case DriverType.BUNS:
					rawParametersLength = 0;
					break;
				case DriverType.MDU:
					rawParametersLength = 4;
					break;
				case DriverType.MRO_2:
				case DriverType.MRO:
					rawParametersLength = 3;
					break;
				default:
					rawParametersLength = 2;
					break;
			}

			//var length = DeviceFlash[pointer + 26];
			//CallbackManager.OnLog("Detalization Parameters " + driverType + " " + length);

			var device = new Device
			{
				Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == driverType),
				StateWordBytes = USBManager.CreateBytesArray(DeviceFlash[pointer + 3], DeviceFlash[pointer + 4]),
				StateWordOffset = pointer + 3,
				RawParametersBytes = USBManager.CreateBytesArray(DeviceFlash.GetRange(pointer + 29, rawParametersLength)),
				RawParametersOffset = pointer + 29
			};
			device.IntAddress = driverType == DriverType.Exit ? DeviceFlash[pointer + 1] : DeviceFlash[pointer + 1] + 256 * (DeviceFlash[pointer + 2] + 1);
			device.DriverUID = device.Driver.UID;
			Device groupDevice;
			var parentAddress = 0;
			var config = DeviceFlash[pointer + 31];
			var countInGroup = ((config & 64)>>6) * 4 + ((config & 32)>>5) * 2 + ((config & 16)>>4);
			switch (driverType)
			{
				case DriverType.MPT:
					device.StateWordBytes = USBManager.CreateBytesArray(DeviceFlash[pointer + 31], DeviceFlash[pointer + 32], DeviceFlash[pointer + 33]);
					break;

				case DriverType.MDU:
					device.StateWordBytes = USBManager.CreateBytesArray(DeviceFlash[pointer + 31], DeviceFlash[pointer + 32]);
					config = DeviceFlash[pointer + 33];
					break;

				case DriverType.MRO_2:
					device.StateWordBytes.Add(DeviceFlash[pointer + 31]);
					config = DeviceFlash[pointer + 32];
					parentAddress = DeviceFlash[pointer + 33] + 256 * (DeviceFlash[pointer + 34] + 1);
					break;
			}
			TraceHelper.TraceBytes(device.StateWordBytes, device.PresentationAddressAndName);
			var description = BytesHelper.ExtractString(DeviceFlash, pointer + 6);
			if (driverType != DriverType.MPT)
			{
				var configAndParamSize = DeviceFlash[pointer + 26]; // длина переменной части блока с конфигурацией и сырыми параметрами (1)
				// общая длина записи (2) pointer + 27
				pointer = pointer + configAndParamSize; // конфиг и сырые параметры
				byte clauseJoinOperatorByte = 1;
				var tableDynamicSize = 0; // размер динамической части таблицы
				while (clauseJoinOperatorByte != 0)
				{
					pointer = pointer + tableDynamicSize;
					tableDynamicSize = 0;
					var logicByte = DeviceFlash[pointer + 29];
					//var logic = new BitArray(new byte[] { DeviceFlash[pointer + 29] });
					var zoneLogicOperationByte = logicByte & 3;// Convert.ToInt32(logic[1]) * 2 + Convert.ToInt32(logic[0]);
					var messageNo = (logicByte & 0xF0) >> 4;// Convert.ToInt32(logic[7]) * 8 + Convert.ToInt32(logic[6]) * 4 + Convert.ToInt32(logic[5]) * 2 + Convert.ToInt32(logic[4]);
					var messageType = (logicByte & 8) >> 3;// Convert.ToInt32(logic[3]);
					var eventType = DeviceFlash[pointer + 30]; // Тип события по которому срабатывать в этой группе зон (1)
					clauseJoinOperatorByte = Convert.ToByte(DeviceFlash[pointer + 31] & 3);
					if (clauseJoinOperatorByte == 0x01)
						device.ZoneLogic.JoinOperator = ZoneLogicJoinOperator.And;
					if (clauseJoinOperatorByte == 0x02)
						device.ZoneLogic.JoinOperator = ZoneLogicJoinOperator.Or;
					var zonesCount = BytesHelper.ExtractShort(DeviceFlash, pointer + 32);
					tableDynamicSize += 5;
					var clause = new Clause
					{
						State = GetDeviceConfigHelper.GetEventTypeByCode(eventType),
						Operation = zoneLogicOperationByte == 0x01 ? ZoneLogicOperation.All : ZoneLogicOperation.Any
					};
					if (eventType == 0x0b) // Активация устройства АМ-1Т или МДУ (начиная с 10 версиии базы)
					{
						var deviceCount = zonesCount;
						for (var deviceNo = 0; deviceNo < deviceCount; deviceNo++)
						{
							tableDynamicSize += 3;
							var logicDevice = new Device();
							var localPointer = BytesHelper.ExtractTriple(DeviceFlash, pointer + 34 + deviceNo*3);
							var am1tPointer = BytesHelper.ExtractTriple(DeviceRom, 96);
							var mduPointer = BytesHelper.ExtractTriple(DeviceRom, 120);
							if (localPointer == am1tPointer)
							{
								logicDevice.IntAddress = DeviceFlash[am1tPointer] + 256 * (DeviceFlash[am1tPointer + 1] + 1);
								logicDevice = PanelDevice.Children.FirstOrDefault(x => x.IntAddress == logicDevice.IntAddress);
							}

							if (localPointer == mduPointer)
							{
								logicDevice.IntAddress = DeviceFlash[mduPointer + 1] + 256 * (DeviceFlash[mduPointer + 2] + 1);
								logicDevice = PanelDevice.Children.FirstOrDefault(x => x.IntAddress == logicDevice.IntAddress);
							}
							clause.DeviceUIDs.Add(logicDevice.UID);
						}
					}
					else
					{
						for (var zoneNo = 0; zoneNo < zonesCount; zoneNo++)
						{
							tableDynamicSize += 3;
							var localPointer = BytesHelper.ExtractTriple(DeviceFlash, pointer + 34 + zoneNo * 3);
							// ... здесь инициализируются все зоны учавствующие в логике ... //
							var zone = new Zone();
							if ((localPointer >= outZonesBegin) && (localPointer < outZonesEnd))// зона внешняя
							{
								zone.No = DeviceFlash[localPointer + 6] * 256 + DeviceFlash[localPointer + 7];
								continue;
							}
							zone.No = DeviceFlash[localPointer + 33] * 256 + DeviceFlash[localPointer + 34]; // Глобальный номер зоны
							zone.Name = BytesHelper.ExtractString(DeviceFlash, localPointer + 6);
							zone.DevicesInZoneLogic.Add(device);
							if (Zones.Any(x => x.No == zone.No))
							// Если зона с таким номером уже добавлена, то добавляем её в clauses и продолжаем цикл
							{
								clause.ZoneUIDs.Add(Zones.FirstOrDefault(x => x.No == zone.No).UID);
								continue;
							}

							clause.ZoneUIDs.Add(zone.UID);
							Zones.Add(zone);
							var zonePanelItem = new ZonePanelItem()
							{
								IsRemote = true,
								No = BytesHelper.ExtractShort(DeviceFlash, localPointer + 4),
								PanelDevice = PanelDevice,
								Zone = zone
							};
							zonePanelRelationsInfo.ZonePanelItems.Add(zonePanelItem);
							remoteDeviceConfiguration.Zones.Add(zone);
						}
					}
					if (zoneLogicOperationByte != 0)
						device.ZoneLogic.Clauses.Add(clause);
				}
				pointer = pointer + tableDynamicSize + 29;
			}

			if (driverType == DriverType.MRO_2)
			{
				if (parentAddress > 0x100)
				{
					groupDevice = (PanelDevice.Children.FirstOrDefault(x => x.IntAddress == parentAddress));
					if (groupDevice == null)
					{
						groupDevice = new Device
						{
							Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.MRO_2),
							IntAddress = parentAddress
						};
						groupDevice.DriverUID = groupDevice.Driver.UID;
						PanelDevice.Children.Add(groupDevice);
					}
					groupDevice.Children.Add(device);
					return;
				}
			}

			if (driverType == DriverType.RM_1)
			{
				if (countInGroup > 0)
				{
					var localNo = (config & 14) >> 1;
					groupDevice = (PanelDevice.Children.FirstOrDefault(x => x.IntAddress == device.IntAddress - localNo));
					if (groupDevice == null)
					{
						groupDevice = new Device
						{
							IntAddress = device.IntAddress - localNo
						};
						PanelDevice.Children.Add(groupDevice);
					}
					groupDevice.Children.Add(device);
					switch (countInGroup) // смотрим сколько дочерних устройств у группового устройства
					{
						case 2:
							groupDevice.Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.RM_2);
							groupDevice.DriverUID = groupDevice.Driver.UID;
							break;
						case 3:
							groupDevice.Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.RM_3);
							groupDevice.DriverUID = groupDevice.Driver.UID;
							break;
						case 4:
							groupDevice.Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.RM_4);
							groupDevice.DriverUID = groupDevice.Driver.UID;
							break;
						case 5:
							groupDevice.Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.RM_5);
							groupDevice.DriverUID = groupDevice.Driver.UID;
							break;
					}
					return;
				}
			}
			if (driverType == DriverType.MPT)
			{
				parentAddress = DeviceFlash[pointer + 35] + (DeviceFlash[pointer + 36] + 1) * 256;
				var startUpDelay = BytesHelper.ExtractShort(DeviceFlash, pointer + 37);
				var zoneNo = BytesHelper.ExtractShort(DeviceFlash, pointer + 39);
				var zonePanelItem = zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(x => x.No == zoneNo);
				if (zonePanelItem != null)
				{
					device.Zone = zonePanelItem.Zone;
					device.ZoneUID = device.Zone.UID;
				}
				// номер привязанной зоны (2) pointer + 40
				if (parentAddress > 0x100)
				{
					groupDevice = (PanelDevice.Children.FirstOrDefault(x => x.IntAddress == parentAddress));
					if (groupDevice == null) // если такое ГУ ещё не добавлено
					{
						groupDevice = new Device
						{
							Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.MPT),
							IntAddress = parentAddress
						};
						groupDevice.DriverUID = groupDevice.Driver.UID;
						PanelDevice.Children.Add(groupDevice);
					}
					groupDevice.Children.Add(device);
					pointer = pointer + 49;
					return;
				}
				pointer = pointer + 49;
			}
			PanelDevice.Children.Add(device);
		}

		void ParseNoUIDevicesRom(DriverType driverType)
		{
			var romPointer = GetDeviceOffset(driverType);
			var pointer = BytesHelper.ExtractTriple(DeviceRom, romPointer);
			if (pointer != 0)
			{
				var count = BytesHelper.ExtractShort(DeviceRom, romPointer + 4); // текущее число записей в таблице
				for (var i = 0; i < count; i++)
				{
					ParseNoUIDevicesFlash(ref pointer, driverType);
				}
			}
		}

		void ParseNoUIDevicesFlash(ref int pointer, DriverType driverType)
		{
			int rawParametersLength = 2;

			var device = new Device
			{
				Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == driverType),
				IntAddress = DeviceFlash[pointer] + 256 * (DeviceFlash[pointer + 1] + 1),
				StateWordBytes = USBManager.CreateBytesArray(DeviceFlash[pointer + 2], DeviceFlash[pointer + 3]),
				StateWordOffset = pointer + 2,
				RawParametersBytes = USBManager.CreateBytesArray(DeviceFlash.GetRange(pointer + 8, rawParametersLength)),
				RawParametersOffset = pointer + 8
			};
			device.DriverUID = device.Driver.UID;
			Device groupDevice;
			TraceHelper.TraceBytes(device.StateWordBytes, device.PresentationAddressAndName);
			var zoneNo = BytesHelper.ExtractShort(DeviceFlash, pointer + 5);
			var tableDynamicSize = DeviceFlash[pointer + 7];
			if (zoneNo != 0)
			{
				var zonePanelItem = zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(x => (x.No == zoneNo) && x.PanelDevice.IntAddress == PanelDevice.IntAddress);
				if (zonePanelItem != null)
				{
					device.Zone = zonePanelItem.Zone;
					device.ZoneUID = device.Zone.UID;
				}
			}
			if (driverType == DriverType.AM_1)
			//if (false)
			{
				var driverCode = DeviceFlash[pointer + 10];
				device.DriverUID = MetadataHelper.GetUidById(driverCode);
				device.Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
				var config = DeviceFlash[pointer + 9];
				if ((config & 64) >> 6 == 1)
				{
					var localNo = (config & 14) >> 1;
					groupDevice = PanelDevice.Children.FirstOrDefault(x => x.IntAddress == device.IntAddress - localNo);
					if (groupDevice == null) // если такое ГУ ещё не добавлено
					{
						groupDevice = new Device
						{
							Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AM4),
							IntAddress = device.IntAddress - localNo
						};
						groupDevice.DriverUID = groupDevice.Driver.UID;
						PanelDevice.Children.Add(groupDevice);
					}
					groupDevice.Children.Add(device);
					pointer = pointer + tableDynamicSize + 8;
					return;
				}
			}

			if (driverType == DriverType.AMP_4)
			{
				var config = DeviceFlash[pointer + 7 + tableDynamicSize];
				var localNo = (config & 14) >> 1;
				groupDevice = (PanelDevice.Children.FirstOrDefault(x => x.IntAddress == device.IntAddress - localNo));
				if (groupDevice == null) // если такое ГУ ещё не добавлено
				{
					groupDevice = new Device
					{
						Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AM4_P),
						IntAddress = device.IntAddress - localNo
					};
					groupDevice.DriverUID = groupDevice.Driver.UID;
					PanelDevice.Children.Add(groupDevice);
				}
				groupDevice.Children.Add(device);
				pointer = pointer + tableDynamicSize + 8;
				return;
			}

			if (driverType == DriverType.AM1_O)
			{
				var config = DeviceFlash[pointer + 7 + tableDynamicSize];
				if ((config & 64) >> 6 == 1)
				{
					var localNo = (config & 14) >> 1;
					groupDevice = (PanelDevice.Children.FirstOrDefault(x => x.IntAddress == device.IntAddress - localNo));
					if (groupDevice == null) // если такое ГУ ещё не добавлено
					{
						groupDevice = new Device
						{
							Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AM4),
							IntAddress = device.IntAddress - localNo
						};
						groupDevice.DriverUID = groupDevice.Driver.UID;
						PanelDevice.Children.Add(groupDevice);
					}
					groupDevice.Children.Add(device);
					pointer = pointer + tableDynamicSize + 8;
					return;
				}
			}

			if (driverType == DriverType.AM1_T)
			{
				var config = DeviceFlash[pointer + 9];
				if ((config & 64) >> 6 == 1)
				{
					var localNo = (config & 14) >> 1;
					groupDevice = PanelDevice.Children.FirstOrDefault(x => x.IntAddress == device.IntAddress - localNo);
					if (groupDevice == null) // если такое ГУ ещё не добавлено
					{
						groupDevice = new Device
						{
							Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AM4),
							IntAddress = device.IntAddress - localNo
						};
						groupDevice.DriverUID = groupDevice.Driver.UID;
						PanelDevice.Children.Add(groupDevice);
					}
					groupDevice.Children.Add(device);
					pointer = pointer + tableDynamicSize + 8;
					return;
				}
			}

			if ((driverType == DriverType.RadioHandDetector) || (driverType == DriverType.RadioSmokeDetector))
			{
				int parentAdress = DeviceFlash[pointer + tableDynamicSize + 8 - 1] + 256 * device.ShleifNo;
				groupDevice = PanelDevice.Children.FirstOrDefault(x => x.IntAddress == parentAdress);
				if (groupDevice == null)
				{
					groupDevice = new Device
					{
						Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.MRK_30),
						IntAddress = parentAdress
					};
					groupDevice.DriverUID = groupDevice.Driver.UID;
					PanelDevice.Children.Add(groupDevice);
				}
				groupDevice.Children.Add(device);
				pointer = pointer + tableDynamicSize + 8;
				return;
			}
			pointer = pointer + tableDynamicSize + 8;
			PanelDevice.Children.Add(device);
		}

		void ParseZonesRom(int romPointer, int romDBFirstIndex)
		{
			var pPointer = BytesHelper.ExtractTriple(DeviceRom, romPointer);
			var pointer = BytesHelper.ExtractTriple(DeviceRom, pPointer - romDBFirstIndex);
			if (pointer != 0)
			{
				var count = BytesHelper.ExtractShort(DeviceRom, romPointer + 4);
				for (int i = 0; i < count; i++)
				{
					ParseZonesFlash(ref pointer);
				}
			}
		}

		void ParseZonesFlash(ref int pointer)
		{
			var zone = new Zone
			{
				LocalDeviceNo = BytesHelper.ExtractShort(DeviceFlash, pointer + 4),
				Name = BytesHelper.ExtractString(DeviceFlash, pointer + 6),
				No = BytesHelper.ExtractShort(DeviceFlash, pointer + 33),
			};
			Trace.WriteLine("ParseZonesFlash " + zone.PresentationName + " - " + zone.LocalDeviceNo);
			// 0,1,2,3 - Внутренние параметры (снят с охраны/ на охране, неисправность, пожар, ...)
			if (Zones.FirstOrDefault(x => x.No == zone.No) != null) // Если зона с таким номером уже добавлена, то пропускаем её
			{
				var entrySize = BytesHelper.ExtractShort(DeviceFlash, pointer + 26); // Длина записи (2) pointer + 26
				pointer = pointer + entrySize;
				return;
			}
			int tableDynamicSize = 0; // размер динамической части таблицы
			for (var sleifNo = 0; sleifNo < shleifCount; sleifNo++)
			{
				var innerUIDeviceCount = DeviceFlash[pointer + 44 + sleifNo * 4];
				tableDynamicSize += innerUIDeviceCount * 3;
			}

			var outerUIDeviceCount = DeviceFlash[pointer + 44 + shleifCount * 4]; // количество связанных внешних ИУ, кроме тех у которых в логике "межприборное И"
			tableDynamicSize += outerUIDeviceCount * 3;
			var pPointer = BytesHelper.ExtractTriple(DeviceFlash, pointer + 45 + shleifCount * 4); // Указатель на размещение абсолютного адреса первого в списке связанного внешнего ИУ или 0 при отсутсвие ИУ (3)
			var outPanelCount = DeviceFlash[pointer + 48 + shleifCount * 4]; // Количество внешних приборов, ИУ которого могут управляться нашими ИП по логике "межприборное И" или 0 (1)
			tableDynamicSize += outPanelCount; // не умнажаем на 3, т.к. адрес прибора записывается в 1 байт
			var zonePanelItem = new ZonePanelItem
			{
				IsRemote = true,
				No = BytesHelper.ExtractShort(DeviceFlash, pointer + 4),
				PanelDevice = PanelDevice,
				Zone = zone
			};
			zonePanelRelationsInfo.ZonePanelItems.Add(zonePanelItem);
			Zones.Add(zone);
			remoteDeviceConfiguration.Zones.Add(zone);
			pointer = pointer + 48 + shleifCount * 4 + tableDynamicSize + 1;
		}

		static int GetDeviceOffset(DriverType driverType)
		{
			switch (driverType)
			{
				case DriverType.SmokeDetector: return 24;
				case DriverType.HeatDetector: return 30;
				case DriverType.CombinedDetector: return 36;
				case DriverType.HandDetector: return 48;
				case DriverType.AM_1: return 42;
				case DriverType.AMP_4: return 78;
				case DriverType.AM1_O: return 54;
				case DriverType.AM1_T: return 96;
				case DriverType.RadioHandDetector: return 132;
				case DriverType.RadioSmokeDetector: return 138;
				case DriverType.Valve: return 90;

				case DriverType.RM_1: return 12;
				case DriverType.MPT: return 18;
				case DriverType.MDU: return 120;
				case DriverType.MRO: return 84;
				case DriverType.MRO_2: return 144;
				case DriverType.Exit: return 126;
				default: return -1;
			}
		}

		public void GetCurrentDeviceState(Device device)
		{
			if(device.DeviceState == null)
				device.DeviceState = new DeviceState();
			var romPointer = GetDeviceOffset(device.Driver.DriverType);
			var pointer = BytesHelper.ExtractTriple(DeviceRom, romPointer);

			List<byte> data;
			if(device.Driver.Category == DeviceCategoryType.Sensor)
			{
				data = ServerHelper.GetBytesFromFlashDB(device.Parent, device.StateWordOffset, 11);
				if (data != null)
				{
					device.StateWordBytes = data.GetRange(0, 2);
					if ((device.Driver.DriverType == DriverType.SmokeDetector) || (device.Driver.DriverType == DriverType.RadioSmokeDetector))
					{
						device.DeviceState.Dustiness = (float)data[8] / 100;
						device.DeviceState.Smokiness = USBManager.Send(device.Parent, "Запрос задымленности", 0x01, 0x56, device.ShleifNo, device.AddressOnShleif).Bytes[0];
					}
					if (device.Driver.DriverType == DriverType.HeatDetector)
						device.DeviceState.Temperature = data[8];
					if (device.Driver.DriverType == DriverType.CombinedDetector)
					{
						device.DeviceState.Dustiness = (float)data[9] / 100;
						device.DeviceState.Temperature = data[10];
						device.DeviceState.Smokiness = USBManager.Send(device.Parent, "Запрос задымленности", 0x01, 0x56, device.ShleifNo, device.AddressOnShleif).Bytes[0];
					}
				}
			}
		}
	}
}