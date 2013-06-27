using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using FiresecAPI.Models;
using ServerFS2.ConfigurationWriter;
using ServerFS2.Helpers;
using Device = FiresecAPI.Models.Device;
using System.Diagnostics;

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
		List<Zone> zones;
		ZonePanelRelationsInfo zonePanelRelationsInfo;
		DeviceConfiguration remoteDeviceConfiguration;
		bool CheckMonitoringSuspend = false;

		public GetConfigurationOperationHelper(bool checkMonitoringSuspend)
		{
			CheckMonitoringSuspend = checkMonitoringSuspend;
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
			var device = new Device
			{
				Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == driverType),
				StateWordBytes = USBManager.CreateBytesArray(DeviceFlash[pointer + 3], DeviceFlash[pointer + 4]),
				StateWordOffset = pointer + 3,
				RawParametersBytes = USBManager.CreateBytesArray(DeviceFlash[pointer + 29], DeviceFlash[pointer + 30]),
				RawParametersOffset = pointer + 29
			};
			device.IntAddress = driverType == DriverType.Exit ? DeviceFlash[pointer + 1] : DeviceFlash[pointer + 1] + 256 * (DeviceFlash[pointer + 2] + 1);
			device.DriverUID = device.Driver.UID;
			Device groupDevice;
			var parentAddress = 0;
			var config = DeviceFlash[pointer + 31];
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
						if (zones.Any(x => x.No == zone.No))
						// Если зона с таким номером уже добавлена, то добавляем её в clauses и продолжаем цикл
						{
							clause.ZoneUIDs.Add(zones.FirstOrDefault(x => x.No == zone.No).UID);
							continue;
						}

						clause.ZoneUIDs.Add(zone.UID);
						zones.Add(zone);
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
					if (groupDevice == null) // если такое ГУ ещё не добавлено
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
				if ((config & 16) >> 4 == 1)
				{
					var localNo = (config & 14) >> 1;
					groupDevice = (PanelDevice.Children.FirstOrDefault(x => x.IntAddress == device.IntAddress - localNo));
					if (groupDevice == null) // если такое ГУ ещё не добавлено
					{
						groupDevice = new Device
						{
							IntAddress = device.IntAddress - localNo
						};
						PanelDevice.Children.Add(groupDevice);
					}
					groupDevice.Children.Add(device);
					switch (localNo + 1) // смотрим сколько дочерних устройств у группового устройства
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
			var device = new Device
			{
				Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == driverType),
				IntAddress = DeviceFlash[pointer] + 256 * (DeviceFlash[pointer + 1] + 1),
				StateWordBytes = USBManager.CreateBytesArray(DeviceFlash[pointer + 2], DeviceFlash[pointer + 3]),
				StateWordOffset = pointer + 2,
				RawParametersBytes = USBManager.CreateBytesArray(DeviceFlash[pointer + 8], DeviceFlash[pointer + 9]),
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
			{
				var driverCode = DeviceFlash[pointer + 10];
				var rmCount = BytesHelper.ExtractShort(DeviceFlash, pointer + 11); // количество РМ привязанных к сработке виртуальных кнопок
				for (int i = 0; i < rmCount; i++)
				{
					var rmPointer = BytesHelper.ExtractTriple(DeviceFlash, pointer + 13 + i * 3); // абсолютный адрес размещения привязанного к сработке РМ (3)
					var clause = new Clause
					{
						DeviceUID = device.UID,
						State = ZoneLogicState.AM1TOn
					};
					var rmDevice = new Device()
					{
						IntAddress = DeviceFlash[rmPointer + 1] + 256 * (DeviceFlash[rmPointer + 2] + 1)
					};
					foreach (var deviceChild in PanelDevice.Children)
					{
						if (deviceChild.IntAddress == rmDevice.IntAddress)
						{
							rmDevice = deviceChild;
							break;
						}
						if ((deviceChild.Children != null) && (deviceChild.Children.Count > 0))
							foreach (var devicechildchild in deviceChild.Children)
							{
								if (devicechildchild.IntAddress == rmDevice.IntAddress)
								{
									rmDevice = devicechildchild;
									break;
								}
							}
					}
					rmDevice.ZoneLogic.Clauses.Add(clause);
				}
				device.DriverUID = MetadataHelper.GetUidById(driverCode);
				device.Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
				var config = DeviceFlash[pointer + 9];
				if ((config & 16) >> 4 == 1)
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
				if ((config & 16) >> 4 == 1)
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
				if ((config & 16) >> 4 == 1)
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
				No = BytesHelper.ExtractShort(DeviceFlash, pointer + 33)
			};
			// 0,1,2,3 - Внутренние параметры (снят с охраны/ на охране, неисправность, пожар, ...)
			if (zones.FirstOrDefault(x => x.No == zone.No) != null) // Если зона с таким номером уже добавлена, то пропускаем её
			{
				var entrySize = BytesHelper.ExtractShort(DeviceFlash, pointer + 26); // Длина записи (2) pointer + 26
				pointer = pointer + entrySize;
				return;
			}
			zone.Name = BytesHelper.ExtractString(DeviceFlash, pointer + 6);
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
			zones.Add(zone);
			remoteDeviceConfiguration.Zones.Add(zone);
			pointer = pointer + 48 + shleifCount * 4 + tableDynamicSize + 1;
		}

		public DeviceConfiguration GetDeviceConfiguration(Device panelDevice)
		{
			PanelDevice = (Device)panelDevice.Clone();
			shleifCount = PanelDevice.Driver.ShleifCount;
			PanelDevice.Children = new List<Device>();
			zones = new List<Zone>();

			remoteDeviceConfiguration = new DeviceConfiguration();
			remoteDeviceConfiguration.RootDevice = PanelDevice;
			remoteDeviceConfiguration.Devices.Add(PanelDevice);

			var panelDatabaseReader = new ReadPanelDatabaseOperationHelper(PanelDevice, CheckMonitoringSuspend);
			panelDatabaseReader.RomDBFirstIndex = panelDatabaseReader.GetRomFirstIndex(PanelDevice);
			panelDatabaseReader.FlashDBLastIndex = panelDatabaseReader.GetFlashLastIndex(PanelDevice);
			
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			DeviceRom = panelDatabaseReader.GetRomDBBytes(PanelDevice);
			stopwatch.Stop();
			Trace.WriteLine("GetRomDBBytes " + stopwatch.Elapsed.TotalSeconds);


			stopwatch = new Stopwatch();
			stopwatch.Start();
			DeviceFlash = panelDatabaseReader.GetFlashDBBytes(PanelDevice);
			stopwatch.Stop();
			Trace.WriteLine("GetFlashDBBytes " + stopwatch.Elapsed.TotalSeconds);
			

			zonePanelRelationsInfo = new ZonePanelRelationsInfo();
			ParseZonesRom(1542, panelDatabaseReader.RomDBFirstIndex);
			#region Zones
			//if ((pPointer = DeviceRom[1542] * 256 * 256 + DeviceRom[1543] * 256 + DeviceRom[1544]) != 0)
			//{
			//    // [1546] - длина записи
			//    int count = DeviceRom[1546] * 256 + DeviceRom[1547];
			//    pointer = 0;
			//    if (count != 0)
			//        pointer = DeviceRom[pPointer - RomDBFirstIndex] * 256 * 256 + DeviceRom[pPointer - RomDBFirstIndex + 1] * 256 + DeviceRom[pPointer - RomDBFirstIndex + 2];
			//    for (int i = 0; i < count; i++)
			//    {
			//        var zone = new Zone();
			//        // 0,1,2,3 - Внутренние параметры (снят с охраны/ на охране, неисправность, пожар, ...)
			//        zone.No = DeviceFlash[pointer + 33] * 256 + DeviceFlash[pointer + 34]; // Глобальный номер зоны
			//        if (zones.FirstOrDefault(x => x.No == zone.No) != null) // Если зона с таким номером уже добавлена, то пропускаем её
			//        {
			//            pointer = pointer + DeviceFlash[pointer + 26] * 256 + DeviceFlash[pointer + 27]; // Длина записи (2) pointer + 26
			//            continue;
			//        }
			//        zone.Name = new string(Encoding.Default.GetChars(DeviceFlash.GetRange(pointer + 6, 20).ToArray()));
			//        zone.Name.Replace(" ", "");
			//        // Длина нижеследующих параметров (1) pointer + 28
			//        // Конфин (1) (0-пожараная, 1-охранная, 2-комбирированная, 3-технологическая) pointer + 29
			//        // Битовые атрибуты(1) (0x01 - автопостановка, 0x02 - обойденная/тихая тревога, 0x04 - к зоне привязан МПТ(для фильтра при управлении СПТ через зону)
			//        // 0x08 - охранная зона с задержкой, 0x10 - охранная зона проходная  pointer + 30
			//        // Количество датчиков для формирования "пожар" (иначе "внимание") (1)  pointer + 31
			//        // Количество потерянных ИП в зоне (1) pointer + 32
			//        // Глобальный номер зоны (2) pointer + 33
			//        // Время входной/выходной задержки (для охранных с задержкой входа/выхода), 10 x сек(2) pointer + 35
			//        // Номер направления куда входит зона (1) pointer + 37
			//        // Время автоперевзятия, сек(1) pointer + 38
			//        // Общее количество связанных с зоной ИУ (2) pointer + 39
			//        // Указатель на ведущее МПТ в зоне из таблицы МПТ или 0 (3) pointer + 41

			//        int tableDynamicSize = 0; // размер динамической части таблицы
			//        for (int sleifNo = 0; sleifNo < shleifCount; sleifNo++)
			//        {
			//            var inExecDeviceCount = DeviceFlash[pointer + 44 + sleifNo * 4];
			//            tableDynamicSize += inExecDeviceCount * 3;
			//            // Количество связанных ИУ sleifNo шлейфа (1) pointer + 44 + sleifNo*4(т.к. для каждого шлейфа эта информация занимает 4 байта - кол-во связанных ИУ - 1 байт и абс адрес - 4 байта)
			//            //pPointer = DeviceRom[pointer + 45 + sleifNo * 4] * 256 * 256 + DeviceRom[pointer + 46 + sleifNo * 4] * 256 + DeviceRom[pointer + 47 + sleifNo * 4]; // Указатель на размещение абсолютного адреса первого в списке связанного ИУ sleifNo шлейфа или 0 при отсутсвие ИУ (3) pointer + 45 + sleifNo*4
			//            //for (int inExecDeviceNo = 0; inExecDeviceNo < inExecDeviceCount; inExecDeviceNo++)
			//            //{
			//            //    int localPointer = DeviceRom[pPointer + inExecDeviceNo * 3 - 0x100] * 256 * 256 +
			//            //                       DeviceRom[pPointer + inExecDeviceNo * 3 + 1 - 0x100] * 256 +
			//            //                       DeviceRom[pPointer + inExecDeviceNo * 3 + 2 - 0x100];
			//            //    int intAddress = DeviceRom[localPointer - 0x100 + 1] +
			//            //                     (DeviceRom[localPointer - 0x100 + 2] + 1) * 256;
			//            //    child = device.Children.FirstOrDefault(x => x.IntAddress == intAddress);
			//            //    zone.DevicesInZoneLogic.Add(child);
			//            //    child.ZoneLogic = new ZoneLogic();
			//            //    child.ZoneLogic.Clauses = new List<Clause>();
			//            //    var clause = new Clause();
			//            //    clause.ZoneUIDs.Add(zone.UID);
			//            //    clause.Zones.Add(zone);
			//            //    child.ZoneLogic.Clauses.Add(clause);
			//            //}
			//        }

			//        var outExecDeviceCount = DeviceFlash[pointer + 44 + shleifCount * 4]; // количество связанных внешних ИУ, кроме тех у которых в логике "межприборное И"
			//        tableDynamicSize += outExecDeviceCount * 3;
			//        pPointer = DeviceFlash[pointer + 45 + shleifCount * 4] * 256 * 256 + DeviceFlash[pointer + 46 + shleifCount * 4] * 256 + DeviceFlash[pointer + 47 + shleifCount * 4]; // Указатель на размещение абсолютного адреса первого в списке связанного внешнего ИУ или 0 при отсутсвие ИУ (3)
			//        for (int outExecDeviceNo = 0; outExecDeviceNo < outExecDeviceCount; outExecDeviceNo++)
			//        {
			//            int localPointer = DeviceFlash[pPointer + outExecDeviceNo * 3] * 256 * 256 +
			//                               DeviceFlash[pPointer + outExecDeviceNo * 3 + 1] * 256 +
			//                               DeviceFlash[pPointer + outExecDeviceNo * 3 + 2];
			//            int intAddress = DeviceFlash[localPointer + 1] +
			//                             (DeviceFlash[localPointer + 2] + 1) * 256;
			//            // ... //
			//        }

			//        var outPanelCount = DeviceFlash[pointer + 48 + shleifCount * 4]; // Количество внешних приборов, ИУ которого могут управляться нашими ИП по логике "межприборное И" или 0 (1)
			//        tableDynamicSize += outPanelCount; // не умнажаем на 3, т.к. адрес прибора записывается в 1 байт
			//        var zonePanelItem = new ZonePanelItem();
			//        zonePanelItem.IsRemote = true;
			//        zonePanelItem.No = DeviceFlash[pointer + 4] * 256 + DeviceFlash[pointer + 5]; // локальный номер зоны
			//        zonePanelItem.PanelDevice = PanelDevice;
			//        zonePanelItem.Zone = zone;
			//        zonePanelRelationsInfo.ZonePanelItems.Add(zonePanelItem);
			//        zones.Add(zone);
			//        remoteDeviceConfiguration.Zones.Add(zone);
			//        pointer = pointer + 48 + shleifCount * 4 + tableDynamicSize + 1;
			//    }
			//}
			#endregion
			#region OutZones
			outZonesBegin = DeviceRom[1548] * 256 * 256 + DeviceRom[1549] * 256 + DeviceRom[1550];
			outZonesCount = DeviceRom[1552] * 256 + DeviceRom[1553];
			outZonesEnd = outZonesBegin + outZonesCount * 9;
			#endregion
			#region Хидеры таблиц на исполнительные устройства
			ParseUIDevicesRom(DriverType.RM_1);
			ParseUIDevicesRom(DriverType.MPT);
			ParseUIDevicesRom(DriverType.MDU);
			ParseUIDevicesRom(DriverType.MRO);
			ParseUIDevicesRom(DriverType.MRO_2);
			ParseUIDevicesRom(DriverType.Exit);
			#region RM
			//if ((pointer = DeviceRom[12] * 256 * 256 + DeviceRom[13] * 256 + DeviceRom[14]) != 0) //РМ-1
			//{
			//    var count = DeviceRom[16] * 256 + DeviceRom[17]; // текущее число записей в таблице
			//    pointer -= 0x100;
			//    groupDevice = new Device();
			//    for (int i = 0; i < count; i++)
			//    {
			//        child = new Device();
			//        child.ZoneLogic = new ZoneLogic();
			//        child.ZoneLogic.Clauses = new List<Clause>();
			//        child.DriverUID = new Guid("4a60242a-572e-41a8-8b87-2fe6b6dc4ace");
			//        child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
			//        // адрес прибора привязки в сети (0 для локальной) (1) pointer + 0
			//        child.IntAddress = DeviceFlash[pointer + 1] + 256 * (DeviceFlash[pointer + 2] + 1);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 3]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 4]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 29]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 30]);
			//        // динамические параметры для базы (1) pointer + 5
			//        var description =
			//            new string(Encoding.Default.GetChars(DeviceFlash.GetRange(pointer + 6, 20).ToArray()));
			//        Trace.WriteLine(child.PresentationAddressAndName + " { " + String.Join(" ", child.InnerDeviceParameters.Select(p => p.ToString("X2")).ToArray()) + " } ");
			//        var configAndParamSize = DeviceFlash[pointer + 26]; // длина переменной части блока с конфигурацией и сырыми параметрами (1)
			//        // общая длина записи (2) pointer + 27
			//        var config = new BitArray(new byte[] { DeviceFlash[pointer + 31] });
			//        pointer = pointer + configAndParamSize; // конфиг и сырые параметры
			//        /* Настройка логики */
			//        byte outAndOr = 1;
			//        int tableDynamicSize = 0; // размер динамической части таблицы + 1
			//        while (outAndOr != 0)
			//        {
			//            pointer = pointer + tableDynamicSize;
			//            tableDynamicSize = 0;
			//            byte inAndOr = DeviceFlash[pointer + 29]; // логика внутри группы зон с одинаковым типом события 0x01 - "и", 0x02 - "или"
			//            byte eventType = DeviceFlash[pointer + 30]; // Тип события по которому срабатывать в этой группе зон (1)
			//            outAndOr = DeviceFlash[pointer + 31];
			//            if (outAndOr == 0x01)
			//                child.ZoneLogic.JoinOperator = ZoneLogicJoinOperator.And;
			//            if (outAndOr == 0x02)
			//                child.ZoneLogic.JoinOperator = ZoneLogicJoinOperator.Or;
			//            int zonesCount = DeviceFlash[pointer + 32] * 256 + DeviceFlash[pointer + 33];
			//            tableDynamicSize += 5;
			//            var clause = new Clause();
			//            clause.State = GetDeviceConfigHelper.GetEventTypeByCode(eventType);
			//            if (inAndOr == 0x01)
			//                clause.Operation = ZoneLogicOperation.All;
			//            else
			//                clause.Operation = ZoneLogicOperation.Any;
			//            for (int zoneNo = 0; zoneNo < zonesCount; zoneNo++)
			//            {
			//                tableDynamicSize += 3;
			//                var localPointer = DeviceRom[pointer + 34 + zoneNo * 3] * 256 * 256 +
			//                                   DeviceRom[pointer + 35 + zoneNo * 3] * 256 +
			//                                   DeviceRom[pointer + 36 + zoneNo * 3] - 0x100;
			//                // ... здесь инициализируются все зоны учавствующие в логике ... //
			//                var zone = new Zone();
			//                if ((localPointer >= outzonesbegin - 0x100) && (localPointer < outzonesend - 0x100))// зона внешняя
			//                {
			//                    zone.No = DeviceRom[localPointer + 6] * 256 + DeviceRom[localPointer + 7];
			//                    continue;
			//                }
			//                zone.No = DeviceRom[localPointer + 33] * 256 + DeviceRom[localPointer + 34]; // Глобальный номер зоны
			//                zone.Name =
			//                    new string(Encoding.Default.GetChars(DeviceRom.GetRange(localPointer + 6, 20).ToArray()));
			//                zone.Name.Replace(" ", "");
			//                zone.DevicesInZoneLogic.Add(child);
			//                if (zones.FirstOrDefault(x => x.No == zone.No) != null)
			//                // Если зона с таким номером уже добавлена, то добавляем её в clauses и продолжаем цикл
			//                {
			//                    clause.ZoneUIDs.Add(zones.FirstOrDefault(x => x.No == zone.No).UID);
			//                    continue;
			//                }

			//                clause.ZoneUIDs.Add(zone.UID);
			//                zones.Add(zone);
			//                var zonePanelItem = new ZonePanelItem();
			//                zonePanelItem.IsRemote = true;
			//                zonePanelItem.No = DeviceRom[localPointer + 4] * 256 + DeviceRom[localPointer + 5];
			//                // локальный номер зоны
			//                zonePanelItem.PanelDevice = device;
			//                zonePanelItem.Zone = zone;
			//                zonePanelRelationsInfo.ZonePanelItems.Add(zonePanelItem);
			//                remoteDeviceConfiguration.Zones.Add(zone);
			//            }
			//            if (inAndOr != 0)
			//                child.ZoneLogic.Clauses.Add(clause);
			//        }
			//        pointer = pointer + tableDynamicSize + 29;
			//        if (config[4])
			//        {
			//            var localNoInPPU = Convert.ToInt32(config[3]) * 4 + Convert.ToInt32(config[2]) * 2 + Convert.ToInt32(config[1]);
			//            groupDevice = (device.Children.FirstOrDefault(x => x.IntAddress == child.IntAddress - localNoInPPU));
			//            if (groupDevice == null) // если такое ГУ ещё не добавлено
			//            {
			//                groupDevice = new Device();
			//                device.Children.Add(groupDevice);
			//                groupDevice.IntAddress = child.IntAddress - localNoInPPU;
			//            }
			//            groupDevice.Children.Add(child);
			//            switch (localNoInPPU + 1) // смотрим сколько дочерних устройств у группового устройства
			//            {
			//                case 2:
			//                    groupDevice.DriverUID = new Guid("EA5F5372-C76C-4E92-B879-0AFA0EE979C7"); // РМ-2
			//                    groupDevice.Driver = Drivers.FirstOrDefault(x => x.UID == groupDevice.DriverUID);
			//                    break;
			//                case 3:
			//                    groupDevice.DriverUID = new Guid("15E38FA6-DC41-454B-83E5-D7789064B2E1"); // РМ-3
			//                    groupDevice.Driver = Drivers.FirstOrDefault(x => x.UID == groupDevice.DriverUID);
			//                    break;
			//                case 4:
			//                    groupDevice.DriverUID = new Guid("3CB0E7FB-670F-4F32-8123-4B310AEE1DB8"); // РМ-4
			//                    groupDevice.Driver = Drivers.FirstOrDefault(x => x.UID == groupDevice.DriverUID);
			//                    break;
			//                case 5:
			//                    groupDevice.DriverUID = new Guid("A7C09BA8-DD00-484C-8BEA-245F2920DFBB"); // РМ-5
			//                    groupDevice.Driver = Drivers.FirstOrDefault(x => x.UID == groupDevice.DriverUID);
			//                    break;
			//                default:
			//                    groupDevice.DriverUID = new Guid("EA5F5372-C76C-4E92-B879-0AFA0EE979C7"); // РМ-2
			//                    groupDevice.Driver = Drivers.FirstOrDefault(x => x.UID == groupDevice.DriverUID);
			//                    break;
			//            }
			//            continue;
			//        }
			//        device.Children.Add(child);
			//    }
			//}
			#endregion
			#region MPT
			//if ((pointer = DeviceRom[18] * 256 * 256 + DeviceRom[19] * 256 + DeviceRom[20]) != 0) // МПТ-1
			//{
			//    var count = DeviceRom[22] * 256 + DeviceRom[23]; // текущее число записей в таблице
			//    pointer -= 0x100;
			//    for (int i = 0; i < count; i++)
			//    {
			//        child = new Device();
			//        child.ZoneLogic = new ZoneLogic();
			//        child.ZoneLogic.Clauses = new List<Clause>();
			//        child.DriverUID = new Guid("33a85f87-e34c-45d6-b4ce-a4fb71a36c28");
			//        child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
			//        // адрес прибора привязки в сети (0 для локальной) (1) pointer + 0
			//        child.IntAddress = DeviceFlash[pointer + 1] + 256 * (DeviceFlash[pointer + 2] + 1);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 3]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 4]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 29]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 30]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 31]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 32]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 33]);
			//        // динамические параметры для базы (1) pointer + 5
			//        var description =
			//            new string(Encoding.Default.GetChars(DeviceFlash.GetRange(pointer + 6, 20).ToArray()));
			//        Trace.WriteLine(child.PresentationAddressAndName + " { " + String.Join(" ", child.InnerDeviceParameters.Select(p => p.ToString("X2")).ToArray()) + " } ");
			//        // длина переменной части блока с конфигурацией и сырыми параметрами (1) pointer + 26
			//        // общая длина записи (2) pointer + 27
			//        // сырые параметры устройств МПТ (5) pointer + 29
			//        // параметры конфигурации заливаемые с компа мпт-7 байт pointer + 34 следующие:
			//        // конфиг (1) pointer + 34
			//        // адрес родителя (1) pointer + 35
			//        // шлейф родителя (1) pointer + 36
			//        // задержка запуска (2) pointer + 37
			//        var zoneNo = DeviceFlash[pointer + 39] * 256 + DeviceFlash[pointer + 40];
			//        child.Zone = zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(x => x.No == zoneNo).Zone;
			//        // номер привязанной зоны (2) pointer + 40
			//        child.ZoneUID = child.Zone.UID;
			//        pointer = pointer + 49;
			//        device.Children.Add(child);
			//    }
			//}
			#endregion
			#region MDU
			//if ((pointer = DeviceRom[120] * 256 * 256 + DeviceRom[121] * 256 + DeviceRom[122]) != 0) // МДУ (в документе это МУК-1Э, а не МДУ)
			//{
			//    var count = DeviceRom[124] * 256 + DeviceRom[125]; // текущее число записей в таблице
			//    pointer -= 0x100;
			//    for (int i = 0; i < count; i++)
			//    {
			//        child = new Device();
			//        child.ZoneLogic = new ZoneLogic();
			//        child.ZoneLogic.Clauses = new List<Clause>();
			//        child.DriverUID = new Guid("043fbbe0-8733-4c8d-be0c-e5820dbf7039");
			//        child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
			//        // адрес прибора привязки в сети (0 для локальной) (1) pointer + 0
			//        child.IntAddress = DeviceFlash[pointer + 1] + 256 * (DeviceFlash[pointer + 2] + 1);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 3]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 4]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 29]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 30]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 31]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 32]);
			//        // динамические параметры для базы (1) pointer + 5
			//        var description =
			//            new string(Encoding.Default.GetChars(DeviceFlash.GetRange(pointer + 6, 20).ToArray()));
			//        Trace.WriteLine(child.PresentationAddressAndName + " { " + String.Join(" ", child.InnerDeviceParameters.Select(p => p.ToString("X2")).ToArray()) + " } ");
			//        // длина переменной части блока с конфигурацией и сырыми параметрами (1) pointer + 26
			//        // общая длина записи (2) pointer + 27
			//        // сырые параметры устройств МУК-1Э (4) pointer + 29
			//        // конфиг (1) pointer + 33

			//        /* Настройка логики */
			//        byte outAndOr = 0x01;
			//        while (outAndOr != 0)
			//        {
			//            byte inAndOr = DeviceFlash[pointer + 34];
			//            // логика внутри группы зон с одинаковым типом события 0x01 - "и", 0x02 - "или"
			//            byte eventType = DeviceFlash[pointer + 35];
			//            // Тип события по которому срабатывать в этой группе зон (1)

			//            #region Events Description

			//            // $01  -  включение автоматики
			//            // $02  -  тревога
			//            // $03  -  поставлен на охрану
			//            // $05  -  снят с охраны
			//            // $06  -  ПЦН
			//            // $07  -  меандр
			//            // $04  -  пожар
			//            // $08  -  неисправность
			//            // $09  -  включение НС
			//            // $0A  -  выключение автоматики НС
			//            // $10  -  выходная задержка
			//            // $20  -  внимание
			//            // $40  -  срабатывание модуля пожаротушения
			//            // $80  -  тушение
			//            // $0B  -  активация устройства АМ-1Т или МДУ

			//            #endregion

			//            outAndOr = DeviceFlash[pointer + 36];
			//            if (outAndOr == 0x01)
			//                child.ZoneLogic.JoinOperator = ZoneLogicJoinOperator.And;
			//            if (outAndOr == 0x02)
			//                child.ZoneLogic.JoinOperator = ZoneLogicJoinOperator.Or;
			//            int zonesCount = DeviceFlash[pointer + 37] * 256 + DeviceFlash[pointer + 38];
			//            pointer += 5;
			//            var clause = new Clause();
			//            clause.State = GetDeviceConfigHelper.GetEventTypeByCode(eventType);
			//            if (inAndOr == 0x01)
			//                clause.Operation = ZoneLogicOperation.All;
			//            else
			//                clause.Operation = ZoneLogicOperation.Any;
			//            for (int zoneNo = 0; zoneNo < zonesCount; zoneNo++)
			//            {
			//                var localPointer = DeviceFlash[pointer + 34] * 256 * 256 +
			//                                   DeviceFlash[pointer + 35] * 256 +
			//                                   DeviceFlash[pointer + 36] - 0x100;
			//                // ... здесь инициализируются все зоны учавствующие в логике ... //
			//                var zone = new Zone();
			//                zone.No = DeviceFlash[localPointer + 33] * 256 + DeviceFlash[localPointer + 34];
			//                // Глобальный номер зоны
			//                zone.Name =
			//                    new string(Encoding.Default.GetChars(DeviceFlash.GetRange(localPointer + 6, 20).ToArray()));
			//                zone.Name.Replace(" ", "");
			//                zone.DevicesInZoneLogic.Add(child);
			//                pointer += 3;
			//                if (zones.FirstOrDefault(x => x.No == zone.No) != null)
			//                // Если зона с таким номером уже добавлена, то добавляем её в clauses и продолжаем цикл
			//                {
			//                    clause.ZoneUIDs.Add(zones.FirstOrDefault(x => x.No == zone.No).UID);
			//                    continue;
			//                }

			//                clause.ZoneUIDs.Add(zone.UID);
			//                zones.Add(zone);
			//                var zonePanelItem = new ZonePanelItem();
			//                zonePanelItem.IsRemote = true;
			//                zonePanelItem.No = DeviceFlash[localPointer + 4] * 256 + DeviceFlash[localPointer + 5];
			//                // локальный номер зоны
			//                zonePanelItem.PanelDevice = device;
			//                zonePanelItem.Zone = zone;
			//                zonePanelRelationsInfo.ZonePanelItems.Add(zonePanelItem);
			//                remoteDeviceConfiguration.Zones.Add(zone);
			//            }
			//            child.ZoneLogic.Clauses.Add(clause);
			//        }
			//        pointer = pointer + 34;
			//        device.Children.Add(child);
			//    }
			//}
			#endregion
			#region MRO
			//if ((pointer = DeviceRom[84] * 256 * 256 + DeviceRom[85] * 256 + DeviceRom[86]) != 0) // МРО-2
			//{
			//    var count = DeviceRom[88] * 256 + DeviceRom[89]; // текущее число записей в таблице
			//    pointer -= 0x100;
			//    for (int i = 0; i < count; i++)
			//    {
			//        child = new Device();
			//        child.ZoneLogic = new ZoneLogic();
			//        child.ZoneLogic.Clauses = new List<Clause>();
			//        child.DriverUID = new Guid("2d078d43-4d3b-497c-9956-990363d9b19b");
			//        child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
			//        // адрес прибора привязки в сети (0 для локальной) (1) pointer + 0
			//        child.IntAddress = DeviceFlash[pointer + 1] + 256 * (DeviceFlash[pointer + 2] + 1);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 3]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 4]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 29]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 30]);
			//        // динамические параметры для базы (1) pointer + 5
			//        var description =
			//            new string(Encoding.Default.GetChars(DeviceFlash.GetRange(pointer + 6, 20).ToArray()));
			//        Trace.WriteLine(child.PresentationAddressAndName + " { " + String.Join(" ", child.InnerDeviceParameters.Select(p => p.ToString("X2")).ToArray()) + " } ");
			//        // длина переменной части блока с конфигурацией и сырыми параметрами (1) pointer + 26
			//        // общая длина записи (2) pointer + 27
			//        // сырые параметры устройства МРО-2 (2) pointer + 28
			//        // конфиг (1) pointer + 31

			//        /* Настройка логики */
			//        byte outAndOr = 0x01;
			//        while (outAndOr != 0)
			//        {
			//            byte inAndOr = DeviceFlash[pointer + 32];
			//            // логика внутри группы зон с одинаковым типом события 0x01 - "и", 0x02 - "или"
			//            byte eventType = DeviceFlash[pointer + 33];
			//            // Тип события по которому срабатывать в этой группе зон (1)
			//            outAndOr = DeviceFlash[pointer + 34];
			//            if (outAndOr == 0x01)
			//                child.ZoneLogic.JoinOperator = ZoneLogicJoinOperator.And;
			//            if (outAndOr == 0x02)
			//                child.ZoneLogic.JoinOperator = ZoneLogicJoinOperator.Or;
			//            int zonesCount = DeviceFlash[pointer + 35] * 256 + DeviceFlash[pointer + 36];
			//            pointer += 5;
			//            var clause = new Clause();
			//            clause.State = GetDeviceConfigHelper.GetEventTypeByCode(eventType);
			//            if (inAndOr == 0x01)
			//                clause.Operation = ZoneLogicOperation.All;
			//            else
			//                clause.Operation = ZoneLogicOperation.Any;
			//            for (int zoneNo = 0; zoneNo < zonesCount; zoneNo++)
			//            {
			//                var localPointer = DeviceFlash[pointer + 32] * 256 * 256 +
			//                                   DeviceFlash[pointer + 33] * 256 +
			//                                   DeviceFlash[pointer + 34] - 0x100;
			//                // ... здесь инициализируются все зоны учавствующие в логике ... //
			//                var zone = new Zone();
			//                zone.No = DeviceFlash[localPointer + 33] * 256 + DeviceFlash[localPointer + 34];
			//                // Глобальный номер зоны
			//                zone.Name =
			//                    new string(Encoding.Default.GetChars(DeviceFlash.GetRange(localPointer + 6, 20).ToArray()));
			//                zone.Name.Replace(" ", "");
			//                zone.DevicesInZoneLogic.Add(child);
			//                pointer += 3;
			//                if (zones.FirstOrDefault(x => x.No == zone.No) != null)
			//                // Если зона с таким номером уже добавлена, то добавляем её в clauses и продолжаем цикл
			//                {
			//                    clause.ZoneUIDs.Add(zones.FirstOrDefault(x => x.No == zone.No).UID);
			//                    continue;
			//                }
			//                clause.ZoneUIDs.Add(zone.UID);
			//                zones.Add(zone);
			//                var zonePanelItem = new ZonePanelItem();
			//                zonePanelItem.IsRemote = true;
			//                zonePanelItem.No = DeviceFlash[localPointer + 4] * 256 + DeviceFlash[localPointer + 5];
			//                // локальный номер зоны
			//                zonePanelItem.PanelDevice = device;
			//                zonePanelItem.Zone = zone;
			//                zonePanelRelationsInfo.ZonePanelItems.Add(zonePanelItem);
			//                remoteDeviceConfiguration.Zones.Add(zone);
			//            }
			//            child.ZoneLogic.Clauses.Add(clause);
			//        }
			//        pointer = pointer + 32;
			//        device.Children.Add(child);
			//    }
			//}
			#endregion
			#region MRO-2M
			//if ((pointer = DeviceRom[144] * 256 * 256 + DeviceRom[145] * 256 + DeviceRom[146]) != 0) // МРО-2М
			//{
			//    var count = DeviceRom[148] * 256 + DeviceRom[149]; // текущее число записей в таблице
			//    pointer -= 0x100;
			//    for (int i = 0; i < count; i++)
			//    {
			//        child = new Device();
			//        child.ZoneLogic = new ZoneLogic();
			//        child.ZoneLogic.Clauses = new List<Clause>();
			//        child.DriverUID = new Guid("713702A8-E3A1-4328-9A43-DE9CB5167133");
			//        child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
			//        // адрес прибора привязки в сети (0 для локальной) (1) pointer + 0
			//        child.IntAddress = DeviceFlash[pointer + 1] + 256 * (DeviceFlash[pointer + 2] + 1);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 3]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 4]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 29]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 30]);
			//        // динамические параметры для базы (1) pointer + 5
			//        var description =
			//            new string(Encoding.Default.GetChars(DeviceFlash.GetRange(pointer + 6, 20).ToArray()));
			//        Trace.WriteLine(child.PresentationAddressAndName + " { " + String.Join(" ", child.InnerDeviceParameters.Select(p => p.ToString("X2")).ToArray()) + " } ");
			//        pointer = pointer + DeviceFlash[pointer + 26];// длина переменной части блока с конфигурацией и сырыми параметрами (1)
			//        // общая длина записи (2) pointer + 27

			//        /* Настройка логики */
			//        byte outAndOr = 0x01;
			//        while (outAndOr != 0)
			//        {
			//            var logic = new BitArray(new byte[] { DeviceFlash[pointer + 29] });
			//            int inAndOr = Convert.ToInt32(logic[1]) * 2 + Convert.ToInt32(logic[0]);
			//            var messageNo = Convert.ToInt32(logic[7]) * 8 + Convert.ToInt32(logic[6]) * 4 + Convert.ToInt32(logic[5]) * 2 + Convert.ToInt32(logic[4]);
			//            var messageType = Convert.ToInt32(logic[3]);
			//            // логика внутри группы зон с одинаковым типом события 0x01 - "и", 0x02 - "или"
			//            byte eventType = DeviceFlash[pointer + 30];
			//            // Тип события по которому срабатывать в этой группе зон (1)
			//            outAndOr = DeviceFlash[pointer + 31];
			//            if (outAndOr == 0x01)
			//                child.ZoneLogic.JoinOperator = ZoneLogicJoinOperator.And;
			//            if (outAndOr == 0x02)
			//                child.ZoneLogic.JoinOperator = ZoneLogicJoinOperator.Or;
			//            int zonesCount = DeviceFlash[pointer + 32] * 256 + DeviceFlash[pointer + 33];
			//            pointer += 5;
			//            var clause = new Clause();
			//            clause.State = GetDeviceConfigHelper.GetEventTypeByCode(eventType);
			//            if (inAndOr == 0x01)
			//                clause.Operation = ZoneLogicOperation.All;
			//            else
			//                clause.Operation = ZoneLogicOperation.Any;
			//            clause.ZoneLogicMROMessageNo = (ZoneLogicMROMessageNo)messageNo;
			//            clause.ZoneLogicMROMessageType = (ZoneLogicMROMessageType)messageType;
			//            for (int zoneNo = 0; zoneNo < zonesCount; zoneNo++)
			//            {
			//                var localPointer = DeviceFlash[pointer + 29] * 256 * 256 +
			//                                   DeviceFlash[pointer + 30] * 256 +
			//                                   DeviceFlash[pointer + 31] - 0x100;
			//                // ... здесь инициализируются все зоны учавствующие в логике ... //
			//                var zone = new Zone();
			//                zone.No = DeviceFlash[localPointer + 33] * 256 + DeviceFlash[localPointer + 34];
			//                // Глобальный номер зоны
			//                zone.Name =
			//                    new string(Encoding.Default.GetChars(DeviceFlash.GetRange(localPointer + 6, 20).ToArray()));
			//                zone.Name.Replace(" ", "");
			//                zone.DevicesInZoneLogic.Add(child);
			//                pointer += 3;
			//                if (zones.FirstOrDefault(x => x.No == zone.No) != null)
			//                // Если зона с таким номером уже добавлена, то добавляем её в clauses и продолжаем цикл
			//                {
			//                    clause.ZoneUIDs.Add(zones.FirstOrDefault(x => x.No == zone.No).UID);
			//                    continue;
			//                }
			//                clause.ZoneUIDs.Add(zone.UID);
			//                zones.Add(zone);
			//                var zonePanelItem = new ZonePanelItem();
			//                zonePanelItem.IsRemote = true;
			//                zonePanelItem.No = DeviceFlash[localPointer + 4] * 256 + DeviceFlash[localPointer + 5];
			//                // локальный номер зоны
			//                zonePanelItem.PanelDevice = device;
			//                zonePanelItem.Zone = zone;
			//                zonePanelRelationsInfo.ZonePanelItems.Add(zonePanelItem);
			//                remoteDeviceConfiguration.Zones.Add(zone);
			//            }
			//            child.ZoneLogic.Clauses.Add(clause);
			//        }
			//        pointer = pointer + 29;
			//        device.Children.Add(child);
			//    }
			//}
			#endregion
			#region Exit
			//if ((pointer = DeviceRom[126] * 256 * 256 + DeviceRom[127] * 256 + DeviceRom[128]) != 0)
			//{
			//    var count = DeviceRom[130] * 256 + DeviceRom[131]; // текущее число записей в таблице
			//    pointer -= 0x100;
			//    groupDevice = new Device();
			//    for (int i = 0; i < count; i++)
			//    {
			//        child = new Device();
			//        child.ZoneLogic = new ZoneLogic();
			//        child.ZoneLogic.Clauses = new List<Clause>();
			//        child.Driver = Drivers.FirstOrDefault(x => x.DriverType == DriverType.Exit);
			//        child.DriverUID = child.Driver.UID;
			//        // адрес прибора привязки в сети (0 для локальной) (1) pointer + 0
			//        child.IntAddress = DeviceFlash[pointer + 1];
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 3]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 4]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 29]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 30]);
			//        // динамические параметры для базы (1) pointer + 5
			//        var description = new string(Encoding.Default.GetChars(DeviceFlash.GetRange(pointer + 6, 20).ToArray()));
			//        Trace.WriteLine(child.PresentationAddressAndName + " { " + String.Join(" ", child.InnerDeviceParameters.Select(p => p.ToString("X2")).ToArray()) + " } ");
			//        // длина переменной части блока с конфигурацией и сырыми параметрами (1) pointer + 26
			//        // общая длина записи (2) pointer + 27
			//        // сырые параметры (1) pointer + 29
			//        // конфиг (1) pointer + 30

			//        /* Настройка логики */
			//        byte outAndOr = 0x01;
			//        int tableDynamicSize = 0; // размер динамической части таблицы + 1
			//        while (outAndOr != 0)
			//        {
			//            pointer = pointer + tableDynamicSize;
			//            byte inAndOr = DeviceFlash[pointer + 31]; // логика внутри группы зон с одинаковым типом события 0x01 - "и", 0x02 - "или"
			//            byte eventType = DeviceFlash[pointer + 32]; // Тип события по которому срабатывать в этой группе зон (1)
			//            outAndOr = DeviceFlash[pointer + 33];
			//            if (outAndOr == 0x01)
			//                child.ZoneLogic.JoinOperator = ZoneLogicJoinOperator.And;
			//            if (outAndOr == 0x02)
			//                child.ZoneLogic.JoinOperator = ZoneLogicJoinOperator.Or;
			//            int zonesCount = DeviceFlash[pointer + 34] * 256 + DeviceFlash[pointer + 35];
			//            tableDynamicSize += 5;
			//            var clause = new Clause();
			//            clause.State = GetDeviceConfigHelper.GetEventTypeByCode(eventType);
			//            if (inAndOr == 0x01)
			//                clause.Operation = ZoneLogicOperation.All;
			//            else
			//                clause.Operation = ZoneLogicOperation.Any;
			//            for (int zoneNo = 0; zoneNo < zonesCount; zoneNo++)
			//            {
			//                var localPointer = DeviceFlash[pointer + 36 + zoneNo * 3] * 256 * 256 +
			//                                   DeviceFlash[pointer + 37 + zoneNo * 3] * 256 + DeviceFlash[pointer + 38 + zoneNo * 3] - 0x100;
			//                // ... здесь инициализируются все зоны учавствующие в логике ... //
			//                var zone = new Zone();
			//                zone.No = DeviceFlash[localPointer + 33] * 256 + DeviceFlash[localPointer + 34]; // Глобальный номер зоны
			//                zone.Name = new string(Encoding.Default.GetChars(DeviceFlash.GetRange(localPointer + 6, 20).ToArray()));
			//                zone.Name.Replace(" ", "");
			//                zone.DevicesInZoneLogic.Add(child);
			//                tableDynamicSize += 3;
			//                if (zones.FirstOrDefault(x => x.No == zone.No) != null) // Если зона с таким номером уже добавлена, то добавляем её в clauses и продолжаем цикл
			//                {
			//                    clause.ZoneUIDs.Add(zones.FirstOrDefault(x => x.No == zone.No).UID);
			//                    continue;
			//                }

			//                clause.ZoneUIDs.Add(zone.UID);
			//                zones.Add(zone);
			//                var zonePanelItem = new ZonePanelItem();
			//                zonePanelItem.IsRemote = true;
			//                zonePanelItem.No = DeviceFlash[localPointer + 4] * 256 + DeviceFlash[localPointer + 5]; // локальный номер зоны
			//                zonePanelItem.PanelDevice = device;
			//                zonePanelItem.Zone = zone;
			//                zonePanelRelationsInfo.ZonePanelItems.Add(zonePanelItem);
			//                remoteDeviceConfiguration.Zones.Add(zone);
			//            }
			//            pointer = pointer + 36 + zonesCount * 3;
			//            child.ZoneLogic.Clauses.Add(clause);
			//        }
			//        device.Children.Add(child);
			//    }
			//}
			#endregion
			#endregion
			#region Хидеры таблиц на не исполнительные устройства по типам
			ParseNoUIDevicesRom(DriverType.SmokeDetector);
			ParseNoUIDevicesRom(DriverType.HeatDetector);
			ParseNoUIDevicesRom(DriverType.CombinedDetector);
			ParseNoUIDevicesRom(DriverType.HandDetector);
			ParseNoUIDevicesRom(DriverType.AM_1);
			ParseNoUIDevicesRom(DriverType.AMP_4);
			ParseNoUIDevicesRom(DriverType.AM1_O);
			ParseNoUIDevicesRom(DriverType.AM1_T);
			ParseNoUIDevicesRom(DriverType.RadioHandDetector);
			ParseNoUIDevicesRom(DriverType.RadioSmokeDetector);
			ParseUIDevicesRom(DriverType.Valve);
			#region IP-64
			//if ((pointer = DeviceRom[24] * 256 * 256 + DeviceRom[25] * 256 + DeviceRom[26]) != 0) // ИП-64
			//{
			//    var count = DeviceRom[28] * 256 + DeviceRom[29]; // текущее число записей в таблице
			//    pointer -= 0x100;
			//    for (int i = 0; i < count; i++)
			//    {
			//        child = new Device();
			//        child.DriverUID = new Guid("1e045ad6-66f9-4f0b-901c-68c46c89e8da");
			//        child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
			//        child.IntAddress = DeviceFlash[pointer] + 256 * (DeviceFlash[pointer + 1] + 1);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 2]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 3]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 8]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 9]);
			//        Trace.WriteLine(child.PresentationAddressAndName + " { " + String.Join(" ", child.InnerDeviceParameters.Select(p => p.ToString("X2")).ToArray()) + " } ");
			//        int zoneNo = DeviceFlash[pointer + 5] * 256 + DeviceFlash[pointer + 6];
			//        if (zoneNo != 0)
			//        {
			//            child.Zone =
			//                zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(
			//                    x => (x.No == zoneNo) && x.PanelDevice.IntAddress == device.IntAddress).Zone;
			//            child.ZoneUID = child.Zone.UID;
			//        }
			//        pointer = pointer + 12;
			//        device.Children.Add(child);
			//    }
			//}
			#endregion
			#region IP-29
			//if ((pointer = DeviceRom[30] * 256 * 256 + DeviceRom[31] * 256 + DeviceRom[32]) != 0)  // ИП-29
			//{
			//    var count = DeviceRom[34] * 256 + DeviceRom[35]; // текущее число записей в таблице
			//    pointer -= 0x100;
			//    for (int i = 0; i < count; i++)
			//    {
			//        child = new Device();
			//        child.DriverUID = new Guid("799686b6-9cfa-4848-a0e7-b33149ab940c");
			//        child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
			//        child.IntAddress = DeviceFlash[pointer] + 256 * (DeviceFlash[pointer + 1] + 1);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 2]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 3]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 8]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 9]);
			//        Trace.WriteLine(child.PresentationAddressAndName + " { " + String.Join(" ", child.InnerDeviceParameters.Select(p => p.ToString("X2")).ToArray()) + " } ");
			//        int zoneNo = DeviceFlash[pointer + 5] * 256 + DeviceFlash[pointer + 6];
			//        if (zoneNo != 0)
			//        {
			//            child.Zone =
			//                zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(
			//                    x => (x.No == zoneNo) && x.PanelDevice.IntAddress == device.IntAddress).Zone;
			//            child.ZoneUID = child.Zone.UID;
			//        }
			//        pointer = pointer + 12;
			//        device.Children.Add(child);
			//    }
			//}
			#endregion
			#region IP-64K
			//if ((pointer = DeviceRom[36] * 256 * 256 + DeviceRom[37] * 256 + DeviceRom[38]) != 0)  // ИП-64К
			//{
			//    var count = DeviceRom[40] * 256 + DeviceRom[41]; // текущее число записей в таблице
			//    pointer -= 0x100;
			//    for (int i = 0; i < count; i++)
			//    {
			//        child = new Device();
			//        child.DriverUID = new Guid("37f13667-bc77-4742-829b-1c43fa404c1f");
			//        child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
			//        child.IntAddress = DeviceFlash[pointer] + 256 * (DeviceFlash[pointer + 1] + 1);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 2]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 3]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 8]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 9]);
			//        Trace.WriteLine(child.PresentationAddressAndName + " { " + String.Join(" ", child.InnerDeviceParameters.Select(p => p.ToString("X2")).ToArray()) + " } ");
			//        int zoneNo = DeviceFlash[pointer + 5] * 256 + DeviceFlash[pointer + 6];
			//        if (zoneNo != 0)
			//        {
			//            child.Zone =
			//                zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(
			//                    x => (x.No == zoneNo) && x.PanelDevice.IntAddress == device.IntAddress).Zone;
			//            child.ZoneUID = child.Zone.UID;
			//        }
			//        pointer = pointer + 16;
			//        device.Children.Add(child);
			//    }
			//}
			#endregion
			#region AM-1P
			//if ((pointer = DeviceRom[42] * 256 * 256 + DeviceRom[43] * 256 + DeviceRom[44]) != 0)  // АМ-1П, КО, КЗ, КУА, КнВклШУЗ, КнРазблАвт, КнВыклШУЗ 
			//{
			//    var count = DeviceRom[46] * 256 + DeviceRom[47]; // текущее число записей в таблице
			//    pointer -= 0x100;
			//    for (int i = 0; i < count; i++)
			//    {
			//        child = new Device();
			//        child.IntAddress = DeviceFlash[pointer] + 256 * (DeviceFlash[pointer + 1] + 1);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 2]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 3]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 8]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 9]);
			//        int zoneNo = DeviceFlash[pointer + 5] * 256 + DeviceFlash[pointer + 6];
			//        if (zoneNo != 0)
			//        {
			//            child.Zone =
			//                zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(
			//                    x => (x.No == zoneNo) && x.PanelDevice.IntAddress == device.IntAddress).Zone;

			//            child.ZoneUID = child.Zone.UID;
			//        }
			//        var tableDynamicSize = DeviceFlash[pointer + 7];
			//        var driverType = DeviceFlash[pointer + 10];
			//        var rmCount = DeviceFlash[pointer + 11] * 256 + DeviceFlash[pointer + 12]; // количество РМ привязанных к сработке виртуальных кнопок
			//        for (int j = 0; j < rmCount; j++)
			//        {
			//            var rmPointer = DeviceFlash[pointer + 13 + j * 3] * 256 * 256 + DeviceFlash[pointer + 14 + j * 3] * 256 + DeviceFlash[pointer + 15 + j * 3] - 0x100; // абсолютный адрес размещения привязанного к сработке РМ (3)
			//            var clause = new Clause();
			//            clause.DeviceUID = child.UID;
			//            clause.State = ZoneLogicState.AM1TOn;
			//            var rm = new Device();
			//            foreach (var devicechild in device.Children)
			//            {
			//                if (devicechild.IntAddress == DeviceFlash[rmPointer + 1] + 256 * (DeviceFlash[rmPointer + 2] + 1))
			//                {
			//                    rm = devicechild;
			//                    break;
			//                }
			//                if ((devicechild.Children != null) && (devicechild.Children.Count > 0))
			//                    foreach (var devicechildchild in devicechild.Children)
			//                    {
			//                        if (devicechildchild.IntAddress == DeviceFlash[rmPointer + 1] + 256 * (DeviceFlash[rmPointer + 2] + 1))
			//                        {
			//                            rm = devicechildchild;
			//                            break;
			//                        }
			//                    }
			//            }
			//            rm.ZoneLogic.Clauses.Add(clause);
			//        }
			//        child.DriverUID = MetadataHelper.GetUidById(driverType);
			//        child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
			//        var config = new BitArray(new byte[] { DeviceFlash[pointer + 9] });
			//        pointer = pointer + 8 + tableDynamicSize; // указатель на следующую запись в таблице
			//        if (config[4])
			//        {
			//            var localNoInPPU = Convert.ToInt32(config[3]) * 4 + Convert.ToInt32(config[2]) * 2 + Convert.ToInt32(config[1]);
			//            if ((device.Children.FirstOrDefault(x => x.IntAddress == child.IntAddress - localNoInPPU)) == null) // если такое ГУ ещё не добавлено
			//            {
			//                groupDevice = new Device();
			//                groupDevice.DriverUID = new Guid("E495C37A-A414-4B47-AF24-FEC1F9E43D86"); // АМ-4
			//                groupDevice.Driver = Drivers.FirstOrDefault(x => x.UID == groupDevice.DriverUID);
			//                device.Children.Add(groupDevice);
			//                groupDevice.IntAddress = child.IntAddress - localNoInPPU;
			//            }
			//            groupDevice.Children.Add(child);
			//            continue;
			//        }
			//        Trace.WriteLine(child.PresentationAddressAndName + " { " + String.Join(" ", child.InnerDeviceParameters.Select(p => p.ToString("X2")).ToArray()) + " } ");
			//        device.Children.Add(child);
			//    }
			//}
			#endregion
			#region RPI
			//if ((pointer = DeviceRom[48] * 256 * 256 + DeviceRom[49] * 256 + DeviceRom[50]) != 0)  // РПИ
			//{
			//    var count = DeviceRom[52] * 256 + DeviceRom[53]; // текущее число записей в таблице
			//    pointer -= 0x100;
			//    for (int i = 0; i < count; i++)
			//    {
			//        child = new Device();
			//        child.DriverUID = new Guid("641fa899-faa0-455b-b626-646e5fbe785a");
			//        child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
			//        child.IntAddress = DeviceFlash[pointer] + 256 * (DeviceFlash[pointer + 1] + 1);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 2]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 3]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 8]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 9]);
			//        Trace.WriteLine(child.PresentationAddressAndName + " { " + String.Join(" ", child.InnerDeviceParameters.Select(p => p.ToString("X2")).ToArray()) + " } ");
			//        int zoneNo = DeviceFlash[pointer + 5] * 256 + DeviceFlash[pointer + 6];
			//        if (zoneNo != 0)
			//        {
			//            child.Zone =
			//                zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(
			//                    x => (x.No == zoneNo) && x.PanelDevice.IntAddress == device.IntAddress).Zone;
			//            child.ZoneUID = child.Zone.UID;
			//        }
			//        pointer = pointer + 11; // указатель на следующую запись в таблице

			//        device.Children.Add(child);
			//    }
			//}
			#endregion
			#region AMP_4
			//if ((pointer = DeviceRom[78] * 256 * 256 + DeviceRom[79] * 256 + DeviceRom[80]) != 0) // АМП-4
			//{
			//    var count = DeviceRom[82] * 256 + DeviceRom[83]; // текущее число записей в таблице
			//    pointer -= 0x100;
			//    for (int i = 0; i < count; i++)
			//    {
			//        child = new Device();
			//        child.DriverUID = new Guid("D8997F3B-64C4-4037-B176-DE15546CE568"); // АМ-1
			//        child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
			//        child.IntAddress = DeviceFlash[pointer] + 256 * (DeviceFlash[pointer + 1] + 1);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 2]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 3]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 8]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 9]);
			//        Trace.WriteLine(child.PresentationAddressAndName + " { " + String.Join(" ", child.InnerDeviceParameters.Select(p => p.ToString("X2")).ToArray()) + " } ");
			//        int zoneNo = DeviceFlash[pointer + 5] * 256 + DeviceFlash[pointer + 6];
			//        if (zoneNo != 0)
			//        {
			//            child.Zone =
			//                zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(
			//                    x => (x.No == zoneNo) && x.PanelDevice.IntAddress == device.IntAddress).Zone;
			//            child.ZoneUID = child.Zone.UID;
			//        }
			//        var tableDynamicSize = DeviceFlash[pointer + 7];
			//        var config = new BitArray(new byte[] { DeviceFlash[pointer + 7 + tableDynamicSize] });
			//        var localNoInPPU = Convert.ToInt32(config[3]) * 4 + Convert.ToInt32(config[2]) * 2 + Convert.ToInt32(config[1]);
			//        groupDevice = (device.Children.FirstOrDefault(x => x.IntAddress == child.IntAddress - localNoInPPU));
			//        if (groupDevice == null) // если такое ГУ ещё не добавлено
			//        {
			//            groupDevice = new Device();
			//            groupDevice.DriverUID = new Guid("A15D9258-D5B5-4A81-A60A-3C9A308FB528"); // АМП-4
			//            groupDevice.Driver = Drivers.FirstOrDefault(x => x.UID == groupDevice.DriverUID);
			//            device.Children.Add(groupDevice);
			//            groupDevice.IntAddress = child.IntAddress - localNoInPPU;
			//        }
			//        pointer = pointer + 8 + tableDynamicSize; // указатель на следующую запись в таблице
			//        groupDevice.Children.Add(child);
			//    }
			//}
			#endregion
			#region AM1_O
			//if ((pointer = DeviceRom[54] * 256 * 256 + DeviceRom[55] * 256 + DeviceRom[56]) != 0) // АМ-1О
			//{
			//    var count = DeviceRom[58] * 256 + DeviceRom[59]; // текущее число записей в таблице
			//    pointer -= 0x100;
			//    for (int i = 0; i < count; i++)
			//    {
			//        child = new Device();
			//        child.DriverUID = new Guid("efca74b2-ad85-4c30-8de8-8115cc6dfdd2");
			//        child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
			//        child.IntAddress = DeviceFlash[pointer] + 256 * (DeviceFlash[pointer + 1] + 1);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 2]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 3]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 8]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 9]);
			//        Trace.WriteLine(child.PresentationAddressAndName + " { " + String.Join(" ", child.InnerDeviceParameters.Select(p => p.ToString("X2")).ToArray()) + " } ");
			//        int zoneNo = DeviceFlash[pointer + 5] * 256 + DeviceFlash[pointer + 6];
			//        if (zoneNo != 0)
			//        {
			//            child.Zone =
			//                zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(
			//                    x => (x.No == zoneNo) && x.PanelDevice.IntAddress == device.IntAddress).Zone;
			//            child.ZoneUID = child.Zone.UID;
			//        }
			//        var tableDynamicSize = DeviceFlash[pointer + 7];
			//        var config = new BitArray(new byte[] { DeviceFlash[pointer + 7 + tableDynamicSize] });
			//        pointer = pointer + 8 + tableDynamicSize; // указатель на следующую запись в таблице
			//        if (config[4])
			//        {
			//            var localNoInPPU = Convert.ToInt32(config[3]) * 4 + Convert.ToInt32(config[2]) * 2 + Convert.ToInt32(config[1]);
			//            groupDevice = (device.Children.FirstOrDefault(x => x.IntAddress == child.IntAddress - localNoInPPU));
			//            if (groupDevice == null) // если такое ГУ ещё не добавлено
			//            {
			//                groupDevice = new Device();
			//                groupDevice.DriverUID = new Guid("E495C37A-A414-4B47-AF24-FEC1F9E43D86"); // АМ-4
			//                groupDevice.Driver = Drivers.FirstOrDefault(x => x.UID == groupDevice.DriverUID);
			//                device.Children.Add(groupDevice);
			//                groupDevice.IntAddress = child.IntAddress - localNoInPPU;
			//            }
			//            else
			//            {
			//                if ((groupDevice.Driver.DriverType == DriverType.AM4_P))
			//                {
			//                    groupDevice.Children.FirstOrDefault(x => x.IntAddress == child.IntAddress);
			//                }
			//            }
			//            groupDevice.Children.Add(child);
			//            continue;
			//        }
			//        device.Children.Add(child);
			//    }
			//}
			#endregion
			#region AM1_T
			//if ((pointer = DeviceRom[96] * 256 * 256 + DeviceRom[97] * 256 + DeviceRom[98]) != 0) // АМ-1Т
			//{
			//    var count = DeviceRom[100] * 256 + DeviceRom[101]; // текущее число записей в таблице
			//    pointer -= 0x100;
			//    for (int i = 0; i < count; i++)
			//    {
			//        child = new Device();
			//        child.DriverUID = new Guid("f5a34ce2-322e-4ed9-a75f-fc8660ae33d8");
			//        child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
			//        child.IntAddress = DeviceFlash[pointer] + 256 * (DeviceFlash[pointer + 1] + 1);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 2]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 3]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 8]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 9]);
			//        Trace.WriteLine(child.PresentationAddressAndName + " { " + String.Join(" ", child.InnerDeviceParameters.Select(p => p.ToString("X2")).ToArray()) + " } ");
			//        int zoneNo = DeviceFlash[pointer + 5] * 256 + DeviceFlash[pointer + 6];
			//        if (zoneNo != 0)
			//        {
			//            child.Zone =
			//                zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(
			//                    x => (x.No == zoneNo) && x.PanelDevice.IntAddress == device.IntAddress).Zone;
			//            child.ZoneUID = child.Zone.UID;
			//        }
			//        var tableDynamicSize = DeviceFlash[pointer + 7];
			//        var config = new BitArray(new byte[] { DeviceFlash[pointer + 9] });
			//        pointer = pointer + 8 + tableDynamicSize; // указатель на следующую запись в таблице
			//        if (config[4])
			//        {
			//            var localNoInPPU = Convert.ToInt32(config[3]) * 4 + Convert.ToInt32(config[2]) * 2 + Convert.ToInt32(config[1]);
			//            groupDevice = device.Children.FirstOrDefault(x => x.IntAddress == child.IntAddress - localNoInPPU);
			//            if (groupDevice == null) // если такое ГУ ещё не добавлено
			//            {
			//                groupDevice = new Device();
			//                groupDevice.DriverUID = new Guid("E495C37A-A414-4B47-AF24-FEC1F9E43D86"); // АМ-4
			//                groupDevice.Driver = Drivers.FirstOrDefault(x => x.UID == groupDevice.DriverUID);
			//                device.Children.Add(groupDevice);
			//                groupDevice.IntAddress = child.IntAddress - localNoInPPU;
			//            }

			//            groupDevice.Children.Add(child);
			//            continue;
			//        }
			//        device.Children.Add(child);
			//    }
			//}
			#endregion
			#region Пока не определено
			//if (BytesHelper.ExtractTriple(DeviceRom, 60) != 0)
			//    MessageBox.Show("Пока не определено"); // Внешние ИУ
			if (BytesHelper.ExtractTriple(DeviceRom, 66) != 0)
				MessageBox.Show("Пока не определено"); // МУК
			//if (BytesHelper.ExtractTriple(DeviceRom, 72) != 0)
			//    MessageBox.Show("Пока не определено"); // БУНС
			if (BytesHelper.ExtractTriple(DeviceRom, 102) != 0)
				MessageBox.Show("Пока не определено"); // АМТ-4
			if (BytesHelper.ExtractTriple(DeviceRom, 114) != 0)
				MessageBox.Show("Пока не определено"); // АСПТ
			#endregion
			#region IPR513-11R
			//if ((pointer = DeviceRom[132] * 256 * 256 + DeviceRom[133] * 256 + DeviceRom[134]) != 0) // ИПР513-11Р
			//{
			//    var count = DeviceRom[136] * 256 + DeviceRom[137]; // текущее число записей в таблице
			//    pointer -= 0x100;
			//    groupDevice = new Device();
			//    groupDevice.DriverUID = new Guid("AB3EF7B1-68AD-4A1B-88A8-997357C3FC5B"); // МРК-30
			//    groupDevice.Driver = Drivers.FirstOrDefault(x => x.UID == groupDevice.DriverUID);
			//    for (int i = 0; i < count; i++)
			//    {
			//        child = new Device();
			//        child.DriverUID = new Guid("D57CDEF3-ACBC-4773-955E-22A1F016D025"); // ИПР513-11Р
			//        child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
			//        child.IntAddress = DeviceFlash[pointer] + 256 * (DeviceFlash[pointer + 1] + 1);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 2]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 3]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 8]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 9]);
			//        Trace.WriteLine(child.PresentationAddressAndName + " { " + String.Join(" ", child.InnerDeviceParameters.Select(p => p.ToString("X2")).ToArray()) + " } ");
			//        int zoneNo = DeviceFlash[pointer + 5] * 256 + DeviceFlash[pointer + 6];
			//        if (zoneNo != 0)
			//        {
			//            child.Zone =
			//                zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(
			//                    x => (x.No == zoneNo) && x.PanelDevice.IntAddress == device.IntAddress).Zone;
			//            child.ZoneUID = child.Zone.UID;
			//        }
			//        var tableDynamicSize = DeviceFlash[pointer + 7];
			//        pointer = pointer + 8 + tableDynamicSize; // указатель на следующую запись в таблице
			//        groupDevice.IntAddress = DeviceFlash[pointer - 1] + 256 * child.ShleifNo;
			//        var oldDevice = device.Children.FirstOrDefault(x => x.IntAddress == groupDevice.IntAddress);
			//        if (oldDevice != null) // если уже есть такое групповое устройство, то берем его
			//        {
			//            groupDevice = oldDevice;
			//            groupDevice.Children.Add(child);
			//            continue;
			//        }
			//        groupDevice.Children.Add(child);
			//        device.Children.Add(groupDevice);
			//    }
			//}
			#endregion
			#region IP 212-64R
			//if ((pointer = DeviceRom[138] * 256 * 256 + DeviceRom[139] * 256 + DeviceRom[140]) != 0) // ИП 212-64Р
			//{
			//    var count = DeviceRom[142] * 256 + DeviceRom[143]; // текущее число записей в таблице
			//    pointer -= 0x100;
			//    groupDevice = new Device();
			//    groupDevice.DriverUID = new Guid("AB3EF7B1-68AD-4A1B-88A8-997357C3FC5B"); // МРК-30
			//    groupDevice.Driver = Drivers.FirstOrDefault(x => x.UID == groupDevice.DriverUID);
			//    for (int i = 0; i < count; i++)
			//    {
			//        child = new Device();
			//        child.DriverUID = new Guid("CFD407D1-5D19-43EC-9650-A86EC4422EC6"); // ИП 212-64Р
			//        child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
			//        child.IntAddress = DeviceFlash[pointer] + 256 * (DeviceFlash[pointer + 1] + 1);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 2]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 3]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 8]);
			//        child.InnerDeviceParameters.Add(DeviceFlash[pointer + 9]);
			//        Trace.WriteLine(child.PresentationAddressAndName + " { " + String.Join(" ", child.InnerDeviceParameters.Select(p => p.ToString("X2")).ToArray()) + " } ");
			//        int zoneNo = DeviceFlash[pointer + 5] * 256 + DeviceFlash[pointer + 6];
			//        if (zoneNo != 0)
			//        {
			//            child.Zone =
			//                zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(
			//                    x => (x.No == zoneNo) && x.PanelDevice.IntAddress == device.IntAddress).Zone;
			//            child.ZoneUID = child.Zone.UID;
			//        }
			//        var tableDynamicSize = DeviceFlash[pointer + 7];
			//        pointer = pointer + 8 + tableDynamicSize; // указатель на следующую запись в таблице
			//        groupDevice.IntAddress = DeviceFlash[pointer - 1] + 256 * child.ShleifNo;
			//        var oldDevice = device.Children.FirstOrDefault(x => x.IntAddress == groupDevice.IntAddress);
			//        if (oldDevice != null) // если уже есть такое групповое устройство, то берем его
			//        {
			//            groupDevice = oldDevice;
			//            groupDevice.Children.Add(child);
			//            continue;
			//        }
			//        device.Children.Add(groupDevice);
			//    }
			//}
			#endregion
			#endregion
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
	
		private int GetDeviceOffset(DriverType driverType)
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
				device.StateWordBytes = data.GetRange(0, 2);
				if ((device.Driver.DriverType == DriverType.SmokeDetector) || (device.Driver.DriverType == DriverType.RadioSmokeDetector))
				{
					device.DeviceState.Dustiness = (float) data[8]/100;
					device.DeviceState.Smokiness = USBManager.Send(device.Parent, 0x01, 0x56, device.ShleifNo, device.AddressOnShleif).Bytes[0];
				}
				if (device.Driver.DriverType == DriverType.HeatDetector)
					device.DeviceState.Temperature = data[8];
				if (device.Driver.DriverType == DriverType.CombinedDetector)
				{
					device.DeviceState.Dustiness = (float) data[9]/100;
					device.DeviceState.Temperature = data[10];
					device.DeviceState.Smokiness = USBManager.Send(device.Parent, 0x01, 0x56, device.ShleifNo, device.AddressOnShleif).Bytes[0];
				}
			}
		}

		public string GetDeviceInformation(Device device)
		{
			string serialNo = "";
			List<byte> serialNoBytes;
			if (device.Driver.DriverType == DriverType.MS_1 || device.Driver.DriverType == DriverType.MS_2)
			{
				serialNoBytes = USBManager.Send(device, 0x01, 0x32).Bytes;
				serialNo = new string(Encoding.Default.GetChars(serialNoBytes.ToArray()));
			}
			else
			{
				serialNoBytes = USBManager.Send(device, 0x01, 0x52, 0x00, 0x00, 0x00, 0xF4, 0x0B).Bytes;
				serialNo = new string(Encoding.Default.GetChars(serialNoBytes.ToArray()));
			}
			return serialNo;
		}
	}
}