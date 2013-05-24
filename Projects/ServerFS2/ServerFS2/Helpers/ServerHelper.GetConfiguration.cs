using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using ClientFS2.ConfigurationWriter;
using FiresecAPI.Models;
using ServerFS2.Helpers;
using Device = FiresecAPI.Models.Device;

namespace ServerFS2
{
	public static partial class ServerHelper
	{
		public static List<byte> DeviceRom;
		public static List<byte> DeviceFlash;
		static int _deviceFlashFirstIndex;
		static int _deviceRomLastIndex;
		public static DeviceConfiguration GetDeviceConfig(Device selectedDevice)
		{
			var remoteDeviceConfiguration = new DeviceConfiguration();
			var device = (Device)selectedDevice.Clone();
			remoteDeviceConfiguration.RootDevice = device;
			remoteDeviceConfiguration.Devices.Add(device);
			device.Children = new List<Device>();
			var zones = new List<Zone>();
            if (selectedDevice.IsUsb)
                IsUsbDevice = true;
            if(IsUsbDevice)
            {
                IsUsbDevice = true;
                GetDeviceFlashFirstAndRomLastIndex(device, true);
                DeviceFlash = GetDeviceFlashUsb(device);
                DeviceRom = GetDeviceRomUsb(device);
            }
            else
            {
                //IsExtendedMode = false;
                GetDeviceFlashFirstAndRomLastIndex(device, false);
                DeviceFlash = GetDeviceFlash(device);
                DeviceRom = GetDeviceRom(device);
            }

            using (var DeviceRomStreamWriter = new BinaryWriter(new FileStream("DeviceRom.bin", FileMode.Create)))
            {
                DeviceRomStreamWriter.Write(DeviceRom.ToArray());
            }
            using (var DeviceFlashStreamWriter = new BinaryWriter(new FileStream("DeviceFlash.bin", FileMode.Create)))
            {
                DeviceFlashStreamWriter.Write(DeviceFlash.ToArray());
            }

			//SetDeviceRom(device);
			//SetDeviceFlash(device);
			int pointer;
			int pPointer;
			Device child;
			int sleifCount = device.Driver.ShleifCount;
			var zonePanelRelationsInfo = new ZonePanelRelationsInfo();
			var groupDevice = new Device();

			#region Хидеры таблицы указателей на указатели на зоны
			if ((pPointer = DeviceFlash[1542] * 256 * 256 + DeviceFlash[1543] * 256 + DeviceFlash[1544]) != 0)
			{
				// [1546] - длина записи
				int count = DeviceFlash[1546] * 256 + DeviceFlash[1547];
				pointer = 0;
				if (count != 0)
					pointer = DeviceFlash[pPointer - _deviceFlashFirstIndex] * 256 * 256 + DeviceFlash[pPointer - _deviceFlashFirstIndex + 1] * 256 + DeviceFlash[pPointer - _deviceFlashFirstIndex + 2] - 0x100;
				for (int i = 0; i < count; i++)
				{
					var zone = new Zone();
					// 0,1,2,3 - Внутренние параметры (снят с охраны/ на охране, неисправность, пожар, ...)
					zone.No = DeviceRom[pointer + 33] * 256 + DeviceRom[pointer + 34]; // Глобальный номер зоны
					if (zones.FirstOrDefault(x => x.No == zone.No) != null) // Если зона с таким номером уже добавлена, то пропускаем её
					{
						pointer = pointer + DeviceRom[pointer + 26] * 256 + DeviceRom[pointer + 27]; // Длина записи (2) pointer + 26
						continue;
					}
					zone.Name = new string(Encoding.Default.GetChars(DeviceRom.GetRange(pointer + 6, 20).ToArray()));
					zone.Name.Replace(" ", "");
					// Длина нижеследующих параметров (1) pointer + 28
					// Конфин (1) (0-пожараная, 1-охранная, 2-комбирированная, 3-технологическая) pointer + 29
					// Битовые атрибуты(1) (0x01 - автопостановка, 0x02 - обойденная/тихая тревога, 0x04 - к зоне привязан МПТ(для фильтра при управлении СПТ через зону)
					// 0x08 - охранная зона с задержкой, 0x10 - охранная зона проходная  pointer + 30
					// Количество датчиков для формирования "пожар" (иначе "внимание") (1)  pointer + 31
					// Количество потерянных ИП в зоне (1) pointer + 32
					// Глобальный номер зоны (2) pointer + 33
					// Время входной/выходной задержки (для охранных с задержкой входа/выхода), 10 x сек(2) pointer + 35
					// Номер направления куда входит зона (1) pointer + 37
					// Время автоперевзятия, сек(1) pointer + 38
					// Общее количество связанных с зоной ИУ (2) pointer + 39
					// Указатель на ведущее МПТ в зоне из таблицы МПТ или 0 (3) pointer + 41

					int tableDynamicSize = 0; // размер динамической части таблицы
					for (int sleifNo = 0; sleifNo < sleifCount; sleifNo++)
					{
						var inExecDeviceCount = DeviceRom[pointer + 44 + sleifNo * 4];
						tableDynamicSize += inExecDeviceCount * 3;
						// Количество связанных ИУ sleifNo шлейфа (1) pointer + 44 + sleifNo*4(т.к. для каждого шлейфа эта информация занимает 4 байта - кол-во связанных ИУ - 1 байт и абс адрес - 4 байта)
						//pPointer = DeviceRom[pointer + 45 + sleifNo * 4] * 256 * 256 + DeviceRom[pointer + 46 + sleifNo * 4] * 256 + DeviceRom[pointer + 47 + sleifNo * 4]; // Указатель на размещение абсолютного адреса первого в списке связанного ИУ sleifNo шлейфа или 0 при отсутсвие ИУ (3) pointer + 45 + sleifNo*4
						//for (int inExecDeviceNo = 0; inExecDeviceNo < inExecDeviceCount; inExecDeviceNo++)
						//{
						//    int localPointer = DeviceRom[pPointer + inExecDeviceNo * 3 - 0x100] * 256 * 256 +
						//                       DeviceRom[pPointer + inExecDeviceNo * 3 + 1 - 0x100] * 256 +
						//                       DeviceRom[pPointer + inExecDeviceNo * 3 + 2 - 0x100];
						//    int intAddress = DeviceRom[localPointer - 0x100 + 1] +
						//                     (DeviceRom[localPointer - 0x100 + 2] + 1) * 256;
						//    child = device.Children.FirstOrDefault(x => x.IntAddress == intAddress);
						//    zone.DevicesInZoneLogic.Add(child);
						//    child.ZoneLogic = new ZoneLogic();
						//    child.ZoneLogic.Clauses = new List<Clause>();
						//    var clause = new Clause();
						//    clause.ZoneUIDs.Add(zone.UID);
						//    clause.Zones.Add(zone);
						//    child.ZoneLogic.Clauses.Add(clause);
						//}
					}

					var outExecDeviceCount = DeviceRom[pointer + 44 + sleifCount * 4]; // количество связанных внешних ИУ, кроме тех у которых в логике "межприборное И"
					tableDynamicSize += outExecDeviceCount * 3;
					pPointer = DeviceRom[pointer + 45 + sleifCount * 4] * 256 * 256 + DeviceRom[pointer + 46 + sleifCount * 4] * 256 + DeviceRom[pointer + 47 + sleifCount * 4]; // Указатель на размещение абсолютного адреса первого в списке связанного внешнего ИУ или 0 при отсутсвие ИУ (3)
					for (int outExecDeviceNo = 0; outExecDeviceNo < outExecDeviceCount; outExecDeviceNo++)
					{
						int localPointer = DeviceRom[pPointer + outExecDeviceNo * 3 - 0x100] * 256 * 256 +
										   DeviceRom[pPointer + outExecDeviceNo * 3 + 1 - 0x100] * 256 +
										   DeviceRom[pPointer + outExecDeviceNo * 3 + 2 - 0x100];
						int intAddress = DeviceRom[localPointer - 0x100 + 1] +
										 (DeviceRom[localPointer - 0x100 + 2] + 1) * 256;
						// ... //
					}

					var outPanelCount = DeviceRom[pointer + 48 + sleifCount * 4]; // Количество внешних приборов, ИУ которого могут управляться нашими ИП по логике "межприборное И" или 0 (1)
					tableDynamicSize += outPanelCount; // не умнажаем на 3, т.к. адрес прибора записывается в 1 байт
					var zonePanelItem = new ZonePanelItem();
					zonePanelItem.IsRemote = true;
					zonePanelItem.No = DeviceRom[pointer + 4] * 256 + DeviceRom[pointer + 5]; // локальный номер зоны
					zonePanelItem.PanelDevice = device;
					zonePanelItem.Zone = zone;
					zonePanelRelationsInfo.ZonePanelItems.Add(zonePanelItem);
					zones.Add(zone);
					remoteDeviceConfiguration.Zones.Add(zone);
					pointer = pointer + 48 + sleifCount * 4 + tableDynamicSize + 1;
				}
			}
			#endregion

		    int outzonesbegin = DeviceFlash[1548] * 256 * 256 + DeviceFlash[1549] * 256 + DeviceFlash[1550];
		    int outzonescount = DeviceFlash[1552]*256 + DeviceFlash[1553];
		    int outzonesend = outzonesbegin + outzonescount*9;
            //#region Хидеры таблицы внешних для прибора зон, в которых не локальные ИП управляют локальными ИУ (в логике "межприборное И" у ИУ)
            //{
		        
            //}
            //#endregion
            #region Хидеры таблиц на исполнительные устройства
            if ((pointer = DeviceFlash[12] * 256 * 256 + DeviceFlash[13] * 256 + DeviceFlash[14]) != 0) //РМ-1
			{
				var count = DeviceFlash[16] * 256 + DeviceFlash[17]; // текущее число записей в таблице
				pointer -= 0x100;
				groupDevice = new Device();
				for (int i = 0; i < count; i++)
				{
					child = new Device();
					child.ZoneLogic = new ZoneLogic();
					child.ZoneLogic.Clauses = new List<Clause>();
					child.DriverUID = new Guid("4a60242a-572e-41a8-8b87-2fe6b6dc4ace");
					child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
					// адрес прибора привязки в сети (0 для локальной) (1) pointer + 0
					child.IntAddress = DeviceRom[pointer + 1] + 256 * (DeviceRom[pointer + 2] + 1);
					// внутренние параметры (2) pointer + 3
					// динамические параметры для базы (1) pointer + 5
					var description =
						new string(Encoding.Default.GetChars(DeviceRom.GetRange(pointer + 6, 20).ToArray()));
					var configAndPaFlashSize = DeviceRom[pointer + 26]; // длина переменной части блока с конфигурацией и сырыми параметрами (1)
					// общая длина записи (2) pointer + 27
					var config = new BitArray(new byte[] { DeviceRom[pointer + 31] });
					pointer = pointer + configAndPaFlashSize; // конфиг и сырые параметры
					/* Настройка логики */
					byte outAndOr = 1;
					int tableDynamicSize = 0; // размер динамической части таблицы + 1
					while (outAndOr != 0)
					{
						pointer = pointer + tableDynamicSize;
					    tableDynamicSize = 0;
						byte inAndOr = DeviceRom[pointer + 29]; // логика внутри группы зон с одинаковым типом события 0x01 - "и", 0x02 - "или"
                        byte eventType = DeviceRom[pointer + 30]; // Тип события по которому срабатывать в этой группе зон (1)
						outAndOr = DeviceRom[pointer + 31];
						if (outAndOr == 0x01)
							child.ZoneLogic.JoinOperator = ZoneLogicJoinOperator.And;
						if (outAndOr == 0x02)
							child.ZoneLogic.JoinOperator = ZoneLogicJoinOperator.Or;
						int zonesCount = DeviceRom[pointer + 32] * 256 + DeviceRom[pointer + 33];
						tableDynamicSize += 5;
						var clause = new Clause();
						clause.State = GetDeviceConfigHelper.GetEventTypeByCode(eventType);
						if (inAndOr == 0x01)
							clause.Operation = ZoneLogicOperation.All;
						else
							clause.Operation = ZoneLogicOperation.Any;
						for (int zoneNo = 0; zoneNo < zonesCount; zoneNo++)
						{
						    tableDynamicSize += 3;
							var localPointer = DeviceRom[pointer + 34 + zoneNo * 3] * 256 * 256 +
											   DeviceRom[pointer + 35 + zoneNo * 3] * 256 +
											   DeviceRom[pointer + 36 + zoneNo * 3] - 0x100;
                            // ... здесь инициализируются все зоны учавствующие в логике ... //
                            var zone = new Zone();
                            if ((localPointer >= outzonesbegin - 0x100)&&(localPointer < outzonesend - 0x100))// зона внешняя
                            {
                                zone.No = DeviceRom[localPointer + 6]*256 + DeviceRom[localPointer + 7];
                                continue;
                            }
							zone.No = DeviceRom[localPointer + 33] * 256 + DeviceRom[localPointer + 34]; // Глобальный номер зоны
							zone.Name =
								new string(Encoding.Default.GetChars(DeviceRom.GetRange(localPointer + 6, 20).ToArray()));
							zone.Name.Replace(" ", "");
							zone.DevicesInZoneLogic.Add(child);
							if (zones.FirstOrDefault(x => x.No == zone.No) != null)
							// Если зона с таким номером уже добавлена, то добавляем её в clauses и продолжаем цикл
							{
								clause.ZoneUIDs.Add(zones.FirstOrDefault(x => x.No == zone.No).UID);
								continue;
							}

							clause.ZoneUIDs.Add(zone.UID);
							zones.Add(zone);
							var zonePanelItem = new ZonePanelItem();
							zonePanelItem.IsRemote = true;
							zonePanelItem.No = DeviceRom[localPointer + 4] * 256 + DeviceRom[localPointer + 5];
							// локальный номер зоны
							zonePanelItem.PanelDevice = device;
							zonePanelItem.Zone = zone;
							zonePanelRelationsInfo.ZonePanelItems.Add(zonePanelItem);
							remoteDeviceConfiguration.Zones.Add(zone);
						}
                        if (inAndOr != 0)
                            child.ZoneLogic.Clauses.Add(clause);
					}
                    pointer = pointer + tableDynamicSize + 29;
					if (config[4])
					{
						var localNoInPPU = Convert.ToInt32(config[3]) * 4 + Convert.ToInt32(config[2]) * 2 + Convert.ToInt32(config[1]);
						groupDevice = (device.Children.FirstOrDefault(x => x.IntAddress == child.IntAddress - localNoInPPU));
						if (groupDevice == null) // если такое ГУ ещё не добавлено
						{
							groupDevice = new Device();
							device.Children.Add(groupDevice);
							groupDevice.IntAddress = child.IntAddress - localNoInPPU;
						}
						groupDevice.Children.Add(child);
						switch (localNoInPPU + 1) // смотрим сколько дочерних устройств у группового устройства
						{
							case 2:
								groupDevice.DriverUID = new Guid("EA5F5372-C76C-4E92-B879-0AFA0EE979C7"); // РМ-2
								groupDevice.Driver = Drivers.FirstOrDefault(x => x.UID == groupDevice.DriverUID);
								break;
							case 3:
								groupDevice.DriverUID = new Guid("15E38FA6-DC41-454B-83E5-D7789064B2E1"); // РМ-3
								groupDevice.Driver = Drivers.FirstOrDefault(x => x.UID == groupDevice.DriverUID);
								break;
							case 4:
								groupDevice.DriverUID = new Guid("3CB0E7FB-670F-4F32-8123-4B310AEE1DB8"); // РМ-4
								groupDevice.Driver = Drivers.FirstOrDefault(x => x.UID == groupDevice.DriverUID);
								break;
							case 5:
								groupDevice.DriverUID = new Guid("A7C09BA8-DD00-484C-8BEA-245F2920DFBB"); // РМ-5
								groupDevice.Driver = Drivers.FirstOrDefault(x => x.UID == groupDevice.DriverUID);
								break;
							default:
								groupDevice.DriverUID = new Guid("EA5F5372-C76C-4E92-B879-0AFA0EE979C7"); // РМ-2
								groupDevice.Driver = Drivers.FirstOrDefault(x => x.UID == groupDevice.DriverUID);
								break;
						}
						continue;
					}
					device.Children.Add(child);
				}
			}
			if ((pointer = DeviceFlash[18] * 256 * 256 + DeviceFlash[19] * 256 + DeviceFlash[20]) != 0) // МПТ-1
			{
				var count = DeviceFlash[22] * 256 + DeviceFlash[23]; // текущее число записей в таблице
				pointer -= 0x100;
				for (int i = 0; i < count; i++)
				{
					child = new Device();
					child.ZoneLogic = new ZoneLogic();
					child.ZoneLogic.Clauses = new List<Clause>();
					child.DriverUID = new Guid("33a85f87-e34c-45d6-b4ce-a4fb71a36c28");
					child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
					// адрес прибора привязки в сети (0 для локальной) (1) pointer + 0
					child.IntAddress = DeviceRom[pointer + 1] + 256 * (DeviceRom[pointer + 2] + 1);
					// внутренние параметры (2) pointer + 3
					// динамические параметры для базы (1) pointer + 5
					var description =
						new string(Encoding.Default.GetChars(DeviceRom.GetRange(pointer + 6, 20).ToArray()));
					// длина переменной части блока с конфигурацией и сырыми параметрами (1) pointer + 26
					// общая длина записи (2) pointer + 27
					// сырые параметры устройств МПТ (5) pointer + 29
					// параметры конфигурации заливаемые с компа мпт-7 байт pointer + 34 следующие:
					// конфиг (1) pointer + 34
					// адрес родителя (1) pointer + 35
					// шлейф родителя (1) pointer + 36
					// задержка запуска (2) pointer + 37
					var zoneNo = DeviceRom[pointer + 39] * 256 + DeviceRom[pointer + 40];
					child.Zone = zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(x => x.No == zoneNo).Zone;
					// номер привязанной зоны (2) pointer + 40
					child.ZoneUID = child.Zone.UID;
					pointer = pointer + 49;
					device.Children.Add(child);
				}
			}
			if ((pointer = DeviceFlash[120] * 256 * 256 + DeviceFlash[121] * 256 + DeviceFlash[122]) != 0) // МДУ (в документе это МУК-1Э, а не МДУ)
			{
				var count = DeviceFlash[124] * 256 + DeviceFlash[125]; // текущее число записей в таблице
				pointer -= 0x100;
				for (int i = 0; i < count; i++)
				{
					child = new Device();
					child.ZoneLogic = new ZoneLogic();
					child.ZoneLogic.Clauses = new List<Clause>();
					child.DriverUID = new Guid("043fbbe0-8733-4c8d-be0c-e5820dbf7039");
					child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
					// адрес прибора привязки в сети (0 для локальной) (1) pointer + 0
					child.IntAddress = DeviceRom[pointer + 1] + 256 * (DeviceRom[pointer + 2] + 1);
					// внутренние параметры (2) pointer + 3
					// динамические параметры для базы (1) pointer + 5
					var description =
						new string(Encoding.Default.GetChars(DeviceRom.GetRange(pointer + 6, 20).ToArray()));
					// длина переменной части блока с конфигурацией и сырыми параметрами (1) pointer + 26
					// общая длина записи (2) pointer + 27
					// сырые параметры устройств МУК-1Э (4) pointer + 29
					// конфиг (1) pointer + 33

					/* Настройка логики */
					byte outAndOr = 0x01;
					while (outAndOr != 0)
					{
						byte inAndOr = DeviceRom[pointer + 34];
						// логика внутри группы зон с одинаковым типом события 0x01 - "и", 0x02 - "или"
						byte eventType = DeviceRom[pointer + 35];
						// Тип события по которому срабатывать в этой группе зон (1)

						#region Events Description

						// $01  -  включение автоматики
						// $02  -  тревога
						// $03  -  поставлен на охрану
						// $05  -  снят с охраны
						// $06  -  ПЦН
						// $07  -  меандр
						// $04  -  пожар
						// $08  -  неисправность
						// $09  -  включение НС
						// $0A  -  выключение автоматики НС
						// $10  -  выходная задержка
						// $20  -  внимание
						// $40  -  срабатывание модуля пожаротушения
						// $80  -  тушение
						// $0B  -  активация устройства АМ-1Т или МДУ

						#endregion

						outAndOr = DeviceRom[pointer + 36];
						if (outAndOr == 0x01)
							child.ZoneLogic.JoinOperator = ZoneLogicJoinOperator.And;
						if (outAndOr == 0x02)
							child.ZoneLogic.JoinOperator = ZoneLogicJoinOperator.Or;
						int zonesCount = DeviceRom[pointer + 37] * 256 + DeviceRom[pointer + 38];
						pointer += 5;
						var clause = new Clause();
						clause.State = GetDeviceConfigHelper.GetEventTypeByCode(eventType);
						if (inAndOr == 0x01)
							clause.Operation = ZoneLogicOperation.All;
						else
							clause.Operation = ZoneLogicOperation.Any;
						for (int zoneNo = 0; zoneNo < zonesCount; zoneNo++)
						{
							var localPointer = DeviceRom[pointer + 34] * 256 * 256 +
											   DeviceRom[pointer + 35] * 256 +
											   DeviceRom[pointer + 36] - 0x100;
							// ... здесь инициализируются все зоны учавствующие в логике ... //
							var zone = new Zone();
							zone.No = DeviceRom[localPointer + 33] * 256 + DeviceRom[localPointer + 34];
							// Глобальный номер зоны
							zone.Name =
								new string(Encoding.Default.GetChars(DeviceRom.GetRange(localPointer + 6, 20).ToArray()));
							zone.Name.Replace(" ", "");
							zone.DevicesInZoneLogic.Add(child);
							pointer += 3;
							if (zones.FirstOrDefault(x => x.No == zone.No) != null)
							// Если зона с таким номером уже добавлена, то добавляем её в clauses и продолжаем цикл
							{
								clause.ZoneUIDs.Add(zones.FirstOrDefault(x => x.No == zone.No).UID);
								continue;
							}

							clause.ZoneUIDs.Add(zone.UID);
							zones.Add(zone);
							var zonePanelItem = new ZonePanelItem();
							zonePanelItem.IsRemote = true;
							zonePanelItem.No = DeviceRom[localPointer + 4] * 256 + DeviceRom[localPointer + 5];
							// локальный номер зоны
							zonePanelItem.PanelDevice = device;
							zonePanelItem.Zone = zone;
							zonePanelRelationsInfo.ZonePanelItems.Add(zonePanelItem);
							remoteDeviceConfiguration.Zones.Add(zone);
						}
						child.ZoneLogic.Clauses.Add(clause);
					}
					pointer = pointer + 34;
					device.Children.Add(child);
				}
			}

			if ((pointer = DeviceFlash[84] * 256 * 256 + DeviceFlash[85] * 256 + DeviceFlash[86]) != 0) // МРО-2
			{
				var count = DeviceFlash[88] * 256 + DeviceFlash[89]; // текущее число записей в таблице
				pointer -= 0x100;
				for (int i = 0; i < count; i++)
				{
					child = new Device();
					child.ZoneLogic = new ZoneLogic();
					child.ZoneLogic.Clauses = new List<Clause>();
					child.DriverUID = new Guid("2d078d43-4d3b-497c-9956-990363d9b19b");
					child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
					// адрес прибора привязки в сети (0 для локальной) (1) pointer + 0
					child.IntAddress = DeviceRom[pointer + 1] + 256 * (DeviceRom[pointer + 2] + 1);
					// внутренние параметры (2) pointer + 3
					// динамические параметры для базы (1) pointer + 5
					var description =
						new string(Encoding.Default.GetChars(DeviceRom.GetRange(pointer + 6, 20).ToArray()));
					// длина переменной части блока с конфигурацией и сырыми параметрами (1) pointer + 26
					// общая длина записи (2) pointer + 27
					// сырые параметры устройства МРО-2 (2) pointer + 28
					// конфиг (1) pointer + 31

					/* Настройка логики */
					byte outAndOr = 0x01;
					while (outAndOr != 0)
					{
						byte inAndOr = DeviceRom[pointer + 32];
						// логика внутри группы зон с одинаковым типом события 0x01 - "и", 0x02 - "или"
						byte eventType = DeviceRom[pointer + 33];
						// Тип события по которому срабатывать в этой группе зон (1)
						outAndOr = DeviceRom[pointer + 34];
						if (outAndOr == 0x01)
							child.ZoneLogic.JoinOperator = ZoneLogicJoinOperator.And;
						if (outAndOr == 0x02)
							child.ZoneLogic.JoinOperator = ZoneLogicJoinOperator.Or;
						int zonesCount = DeviceRom[pointer + 35] * 256 + DeviceRom[pointer + 36];
						pointer += 5;
						var clause = new Clause();
						clause.State = GetDeviceConfigHelper.GetEventTypeByCode(eventType);
						if (inAndOr == 0x01)
							clause.Operation = ZoneLogicOperation.All;
						else
							clause.Operation = ZoneLogicOperation.Any;
						for (int zoneNo = 0; zoneNo < zonesCount; zoneNo++)
						{
							var localPointer = DeviceRom[pointer + 32] * 256 * 256 +
											   DeviceRom[pointer + 33] * 256 +
											   DeviceRom[pointer + 34] - 0x100;
							// ... здесь инициализируются все зоны учавствующие в логике ... //
							var zone = new Zone();
							zone.No = DeviceRom[localPointer + 33] * 256 + DeviceRom[localPointer + 34];
							// Глобальный номер зоны
							zone.Name =
								new string(Encoding.Default.GetChars(DeviceRom.GetRange(localPointer + 6, 20).ToArray()));
							zone.Name.Replace(" ", "");
							zone.DevicesInZoneLogic.Add(child);
							pointer += 3;
							if (zones.FirstOrDefault(x => x.No == zone.No) != null)
							// Если зона с таким номером уже добавлена, то добавляем её в clauses и продолжаем цикл
							{
								clause.ZoneUIDs.Add(zones.FirstOrDefault(x => x.No == zone.No).UID);
								continue;
							}
							clause.ZoneUIDs.Add(zone.UID);
							zones.Add(zone);
							var zonePanelItem = new ZonePanelItem();
							zonePanelItem.IsRemote = true;
							zonePanelItem.No = DeviceRom[localPointer + 4] * 256 + DeviceRom[localPointer + 5];
							// локальный номер зоны
							zonePanelItem.PanelDevice = device;
							zonePanelItem.Zone = zone;
							zonePanelRelationsInfo.ZonePanelItems.Add(zonePanelItem);
							remoteDeviceConfiguration.Zones.Add(zone);
						}
						child.ZoneLogic.Clauses.Add(clause);
					}
					pointer = pointer + 32;
					device.Children.Add(child);
				}
			}

			if ((pointer = DeviceFlash[144] * 256 * 256 + DeviceFlash[145] * 256 + DeviceFlash[146]) != 0) // МРО-2М
			{
				var count = DeviceFlash[148] * 256 + DeviceFlash[149]; // текущее число записей в таблице
				pointer -= 0x100;
				for (int i = 0; i < count; i++)
				{
					child = new Device();
					child.ZoneLogic = new ZoneLogic();
					child.ZoneLogic.Clauses = new List<Clause>();
					child.DriverUID = new Guid("713702A8-E3A1-4328-9A43-DE9CB5167133");
					child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
					// адрес прибора привязки в сети (0 для локальной) (1) pointer + 0
					child.IntAddress = DeviceRom[pointer + 1] + 256 * (DeviceRom[pointer + 2] + 1);
					// внутренние параметры (2) pointer + 3
					// динамические параметры для базы (1) pointer + 5
					var description =
						new string(Encoding.Default.GetChars(DeviceRom.GetRange(pointer + 6, 20).ToArray()));
					pointer = pointer + DeviceRom[pointer + 26];// длина переменной части блока с конфигурацией и сырыми параметрами (1)
					// общая длина записи (2) pointer + 27

					/* Настройка логики */
					byte outAndOr = 0x01;
					while (outAndOr != 0)
					{
						var logic = new BitArray(new byte[] { DeviceRom[pointer + 29] });
						int inAndOr = Convert.ToInt32(logic[1]) * 2 + Convert.ToInt32(logic[0]);
						var messageNo = Convert.ToInt32(logic[7]) * 8 + Convert.ToInt32(logic[6]) * 4 + Convert.ToInt32(logic[5]) * 2 + Convert.ToInt32(logic[4]);
						var messageType = Convert.ToInt32(logic[3]);
						// логика внутри группы зон с одинаковым типом события 0x01 - "и", 0x02 - "или"
						byte eventType = DeviceRom[pointer + 30];
						// Тип события по которому срабатывать в этой группе зон (1)
						outAndOr = DeviceRom[pointer + 31];
						if (outAndOr == 0x01)
							child.ZoneLogic.JoinOperator = ZoneLogicJoinOperator.And;
						if (outAndOr == 0x02)
							child.ZoneLogic.JoinOperator = ZoneLogicJoinOperator.Or;
						int zonesCount = DeviceRom[pointer + 32] * 256 + DeviceRom[pointer + 33];
						pointer += 5;
						var clause = new Clause();
						clause.State = GetDeviceConfigHelper.GetEventTypeByCode(eventType);
						if (inAndOr == 0x01)
							clause.Operation = ZoneLogicOperation.All;
						else
							clause.Operation = ZoneLogicOperation.Any;
                        clause.ZoneLogicMROMessageNo = (ZoneLogicMROMessageNo)messageNo;
                        clause.ZoneLogicMROMessageType = (ZoneLogicMROMessageType)messageType;
						for (int zoneNo = 0; zoneNo < zonesCount; zoneNo++)
						{
							var localPointer = DeviceRom[pointer + 29] * 256 * 256 +
											   DeviceRom[pointer + 30] * 256 +
											   DeviceRom[pointer + 31] - 0x100;
							// ... здесь инициализируются все зоны учавствующие в логике ... //
							var zone = new Zone();
							zone.No = DeviceRom[localPointer + 33] * 256 + DeviceRom[localPointer + 34];
							// Глобальный номер зоны
							zone.Name =
								new string(Encoding.Default.GetChars(DeviceRom.GetRange(localPointer + 6, 20).ToArray()));
							zone.Name.Replace(" ", "");
							zone.DevicesInZoneLogic.Add(child);
							pointer += 3;
							if (zones.FirstOrDefault(x => x.No == zone.No) != null)
							// Если зона с таким номером уже добавлена, то добавляем её в clauses и продолжаем цикл
							{
								clause.ZoneUIDs.Add(zones.FirstOrDefault(x => x.No == zone.No).UID);
								continue;
							}
							clause.ZoneUIDs.Add(zone.UID);
							zones.Add(zone);
							var zonePanelItem = new ZonePanelItem();
							zonePanelItem.IsRemote = true;
							zonePanelItem.No = DeviceRom[localPointer + 4] * 256 + DeviceRom[localPointer + 5];
							// локальный номер зоны
							zonePanelItem.PanelDevice = device;
							zonePanelItem.Zone = zone;
							zonePanelRelationsInfo.ZonePanelItems.Add(zonePanelItem);
							remoteDeviceConfiguration.Zones.Add(zone);
						}
						child.ZoneLogic.Clauses.Add(clause);
					}
					pointer = pointer + 29;
					device.Children.Add(child);
				}
			}

			if ((pointer = DeviceFlash[126] * 256 * 256 + DeviceFlash[127] * 256 + DeviceFlash[128]) != 0)
			{
				var count = DeviceFlash[130] * 256 + DeviceFlash[131]; // текущее число записей в таблице
				pointer -= 0x100;
				groupDevice = new Device();
				for (int i = 0; i < count; i++)
				{
					child = new Device();
					child.ZoneLogic = new ZoneLogic();
					child.ZoneLogic.Clauses = new List<Clause>();
					child.Driver = Drivers.FirstOrDefault(x => x.DriverType == DriverType.Exit);
					child.DriverUID = child.Driver.UID;
					// адрес прибора привязки в сети (0 для локальной) (1) pointer + 0
					child.IntAddress = DeviceRom[pointer + 1];
					// внутренние параметры (2) pointer + 3
					// динамические параметры для базы (1) pointer + 5
					var description = new string(Encoding.Default.GetChars(DeviceRom.GetRange(pointer + 6, 20).ToArray()));
					// длина переменной части блока с конфигурацией и сырыми параметрами (1) pointer + 26
					// общая длина записи (2) pointer + 27
					// сырые параметры (1) pointer + 29
					// конфиг (1) pointer + 30

					/* Настройка логики */
					byte outAndOr = 0x01;
					int tableDynamicSize = 0; // размер динамической части таблицы + 1
					while (outAndOr != 0)
					{
						pointer = pointer + tableDynamicSize;
						byte inAndOr = DeviceRom[pointer + 31]; // логика внутри группы зон с одинаковым типом события 0x01 - "и", 0x02 - "или"
						byte eventType = DeviceRom[pointer + 32]; // Тип события по которому срабатывать в этой группе зон (1)
						outAndOr = DeviceRom[pointer + 33];
						if (outAndOr == 0x01)
							child.ZoneLogic.JoinOperator = ZoneLogicJoinOperator.And;
						if (outAndOr == 0x02)
							child.ZoneLogic.JoinOperator = ZoneLogicJoinOperator.Or;
						int zonesCount = DeviceRom[pointer + 34] * 256 + DeviceRom[pointer + 35];
						tableDynamicSize += 5;
						var clause = new Clause();
						clause.State = GetDeviceConfigHelper.GetEventTypeByCode(eventType);
						if (inAndOr == 0x01)
							clause.Operation = ZoneLogicOperation.All;
						else
							clause.Operation = ZoneLogicOperation.Any;
						for (int zoneNo = 0; zoneNo < zonesCount; zoneNo++)
						{
							var localPointer = DeviceRom[pointer + 36 + zoneNo * 3] * 256 * 256 +
											   DeviceRom[pointer + 37 + zoneNo * 3] * 256 + DeviceRom[pointer + 38 + zoneNo * 3] - 0x100;
							// ... здесь инициализируются все зоны учавствующие в логике ... //
							var zone = new Zone();
							zone.No = DeviceRom[localPointer + 33] * 256 + DeviceRom[localPointer + 34]; // Глобальный номер зоны
							zone.Name = new string(Encoding.Default.GetChars(DeviceRom.GetRange(localPointer + 6, 20).ToArray()));
							zone.Name.Replace(" ", "");
							zone.DevicesInZoneLogic.Add(child);
							tableDynamicSize += 3;
							if (zones.FirstOrDefault(x => x.No == zone.No) != null) // Если зона с таким номером уже добавлена, то добавляем её в clauses и продолжаем цикл
							{
								clause.ZoneUIDs.Add(zones.FirstOrDefault(x => x.No == zone.No).UID);
								continue;
							}

							clause.ZoneUIDs.Add(zone.UID);
							zones.Add(zone);
							var zonePanelItem = new ZonePanelItem();
							zonePanelItem.IsRemote = true;
							zonePanelItem.No = DeviceRom[localPointer + 4] * 256 + DeviceRom[localPointer + 5]; // локальный номер зоны
							zonePanelItem.PanelDevice = device;
							zonePanelItem.Zone = zone;
							zonePanelRelationsInfo.ZonePanelItems.Add(zonePanelItem);
							remoteDeviceConfiguration.Zones.Add(zone);
						}
						pointer = pointer + 36 + zonesCount * 3;
						child.ZoneLogic.Clauses.Add(clause);
					}
					device.Children.Add(child);
				}
			}

			#endregion
			#region Хидеры таблиц на не исполнительные устройства по типам

			if ((pointer = DeviceFlash[24] * 256 * 256 + DeviceFlash[25] * 256 + DeviceFlash[26]) != 0) // ИП-64
			{
				var count = DeviceFlash[28] * 256 + DeviceFlash[29]; // текущее число записей в таблице
				pointer -= 0x100;
				for (int i = 0; i < count; i++)
				{
					child = new Device();
					child.DriverUID = new Guid("1e045ad6-66f9-4f0b-901c-68c46c89e8da");
					child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
					child.IntAddress = DeviceRom[pointer] + 256 * (DeviceRom[pointer + 1] + 1);
					int zoneNo = DeviceRom[pointer + 5] * 256 + DeviceRom[pointer + 6];
					if (zoneNo != 0)
					{
						child.Zone =
							zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(
								x => (x.No == zoneNo) && x.PanelDevice.IntAddress == device.IntAddress).Zone;
						child.ZoneUID = child.Zone.UID;
					}
					pointer = pointer + 12;
					device.Children.Add(child);
				}
			}
			if ((pointer = DeviceFlash[30] * 256 * 256 + DeviceFlash[31] * 256 + DeviceFlash[32]) != 0)  // ИП-29
			{
				var count = DeviceFlash[34] * 256 + DeviceFlash[35]; // текущее число записей в таблице
				pointer -= 0x100;
				for (int i = 0; i < count; i++)
				{
					child = new Device();
					child.DriverUID = new Guid("799686b6-9cfa-4848-a0e7-b33149ab940c");
					child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
					child.IntAddress = DeviceRom[pointer] + 256 * (DeviceRom[pointer + 1] + 1);
					int zoneNo = DeviceRom[pointer + 5] * 256 + DeviceRom[pointer + 6];
					if (zoneNo != 0)
					{
						child.Zone =
							zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(
								x => (x.No == zoneNo) && x.PanelDevice.IntAddress == device.IntAddress).Zone;
						child.ZoneUID = child.Zone.UID;
					}
					pointer = pointer + 12;
					device.Children.Add(child);
				}
			}
			if ((pointer = DeviceFlash[36] * 256 * 256 + DeviceFlash[37] * 256 + DeviceFlash[38]) != 0)  // ИП-64К
			{
				var count = DeviceFlash[40] * 256 + DeviceFlash[41]; // текущее число записей в таблице
				pointer -= 0x100;
				for (int i = 0; i < count; i++)
				{
					child = new Device();
					child.DriverUID = new Guid("37f13667-bc77-4742-829b-1c43fa404c1f");
					child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
					child.IntAddress = DeviceRom[pointer] + 256 * (DeviceRom[pointer + 1] + 1);
					int zoneNo = DeviceRom[pointer + 5] * 256 + DeviceRom[pointer + 6];
					if (zoneNo != 0)
					{
						child.Zone =
							zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(
								x => (x.No == zoneNo) && x.PanelDevice.IntAddress == device.IntAddress).Zone;
						child.ZoneUID = child.Zone.UID;
					}
					pointer = pointer + 16;
					device.Children.Add(child);
				}
			}
			if ((pointer = DeviceFlash[42] * 256 * 256 + DeviceFlash[43] * 256 + DeviceFlash[44]) != 0)  // АМ-1П, КО, КЗ, КУА, КнВклШУЗ, КнРазблАвт, КнВыклШУЗ 
			{
				var count = DeviceFlash[46] * 256 + DeviceFlash[47]; // текущее число записей в таблице
				pointer -= 0x100;
				for (int i = 0; i < count; i++)
				{
					child = new Device();
					child.IntAddress = DeviceRom[pointer] + 256 * (DeviceRom[pointer + 1] + 1);
					int zoneNo = DeviceRom[pointer + 5] * 256 + DeviceRom[pointer + 6];
					if (zoneNo != 0)
					{
						child.Zone =
							zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(
								x => (x.No == zoneNo) && x.PanelDevice.IntAddress == device.IntAddress).Zone;
		
                        child.ZoneUID = child.Zone.UID;
					}
					var tableDynamicSize = DeviceRom[pointer + 7];
					var deviceType = DeviceRom[pointer + 10];
                    var rmCount = DeviceRom[pointer + 11] * 256 + DeviceRom[pointer + 12]; // количество РМ привязанных к сработке виртуальных кнопок
                    for (int j = 0; j < rmCount; j++)
                    {
                        var rmPointer = DeviceRom[pointer + 13 + j * 3] * 256 * 256 + DeviceRom[pointer + 14 + j * 3] * 256 + DeviceRom[pointer + 15 + j * 3] - 0x100; // абсолютный адрес размещения привязанного к сработке РМ (3)
                        var clause = new Clause();
                        clause.DeviceUID = child.UID;
                        clause.State = ZoneLogicState.AM1TOn;
                        var rm = new Device();
                        foreach (var devicechild in device.Children)
                        {
                            if (devicechild.IntAddress == DeviceRom[rmPointer + 1] + 256 * (DeviceRom[rmPointer + 2] + 1))
                            {
                                rm = devicechild;
                                break;
                            }
                            if ((devicechild.Children != null)&&(devicechild.Children.Count > 0))
                                foreach (var devicechildchild in devicechild.Children)
                                {
                                    if (devicechildchild.IntAddress == DeviceRom[rmPointer + 1] + 256 * (DeviceRom[rmPointer + 2] + 1))
                                    {
                                        rm = devicechildchild;
                                        break;
                                    }
                                }
                        }
                        rm.ZoneLogic.Clauses.Add(clause);
                    }
					child.DriverUID = MetadataHelper.GetUidById(deviceType);
					child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
					var config = new BitArray(new byte[] { DeviceRom[pointer + 9] });
					pointer = pointer + 8 + tableDynamicSize; // указатель на следующую запись в таблице
					if (config[4])
					{
						var localNoInPPU = Convert.ToInt32(config[3]) * 4 + Convert.ToInt32(config[2]) * 2 + Convert.ToInt32(config[1]);
						if ((device.Children.FirstOrDefault(x => x.IntAddress == child.IntAddress - localNoInPPU)) == null) // если такое ГУ ещё не добавлено
						{
							groupDevice = new Device();
							groupDevice.DriverUID = new Guid("E495C37A-A414-4B47-AF24-FEC1F9E43D86"); // АМ-4
							groupDevice.Driver = Drivers.FirstOrDefault(x => x.UID == groupDevice.DriverUID);
							device.Children.Add(groupDevice);
							groupDevice.IntAddress = child.IntAddress - localNoInPPU;
						}
						groupDevice.Children.Add(child);
						continue;
					}
					device.Children.Add(child);
				}
			}
			if ((pointer = DeviceFlash[48] * 256 * 256 + DeviceFlash[49] * 256 + DeviceFlash[50]) != 0)  // РПИ
			{
				var count = DeviceFlash[52] * 256 + DeviceFlash[53]; // текущее число записей в таблице
				pointer -= 0x100;
				for (int i = 0; i < count; i++)
				{
					child = new Device();
					child.DriverUID = new Guid("641fa899-faa0-455b-b626-646e5fbe785a");
					child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
					child.IntAddress = DeviceRom[pointer] + 256 * (DeviceRom[pointer + 1] + 1);
					int zoneNo = DeviceRom[pointer + 5] * 256 + DeviceRom[pointer + 6];
					if (zoneNo != 0)
					{
						child.Zone =
							zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(
								x => (x.No == zoneNo) && x.PanelDevice.IntAddress == device.IntAddress).Zone;
						child.ZoneUID = child.Zone.UID;
					}
					pointer = pointer + 11; // указатель на следующую запись в таблице
					device.Children.Add(child);
				}
			}

			if ((pointer = DeviceFlash[78] * 256 * 256 + DeviceFlash[79] * 256 + DeviceFlash[80]) != 0) // АМП-4
			{
				var count = DeviceFlash[82] * 256 + DeviceFlash[83]; // текущее число записей в таблице
				pointer -= 0x100;
				for (int i = 0; i < count; i++)
				{
					child = new Device();
					child.DriverUID = new Guid("D8997F3B-64C4-4037-B176-DE15546CE568"); // АМ-1
					child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
					child.IntAddress = DeviceRom[pointer] + 256 * (DeviceRom[pointer + 1] + 1);
					int zoneNo = DeviceRom[pointer + 5] * 256 + DeviceRom[pointer + 6];
					if (zoneNo != 0)
					{
						child.Zone =
							zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(
								x => (x.No == zoneNo) && x.PanelDevice.IntAddress == device.IntAddress).Zone;
						child.ZoneUID = child.Zone.UID;
					}
					var tableDynamicSize = DeviceRom[pointer + 7];
					var config = new BitArray(new byte[] { DeviceRom[pointer + 7 + tableDynamicSize] });
					var localNoInPPU = Convert.ToInt32(config[3]) * 4 + Convert.ToInt32(config[2]) * 2 + Convert.ToInt32(config[1]);
					groupDevice = (device.Children.FirstOrDefault(x => x.IntAddress == child.IntAddress - localNoInPPU));
					if (groupDevice == null) // если такое ГУ ещё не добавлено
					{
						groupDevice = new Device();
						groupDevice.DriverUID = new Guid("A15D9258-D5B5-4A81-A60A-3C9A308FB528"); // АМП-4
						groupDevice.Driver = Drivers.FirstOrDefault(x => x.UID == groupDevice.DriverUID);
						device.Children.Add(groupDevice);
						groupDevice.IntAddress = child.IntAddress - localNoInPPU;
					}
					pointer = pointer + 8 + tableDynamicSize; // указатель на следующую запись в таблице
					groupDevice.Children.Add(child);
				}
			}

			if ((pointer = DeviceFlash[54] * 256 * 256 + DeviceFlash[55] * 256 + DeviceFlash[56]) != 0) // АМ-1О
			{
				var count = DeviceFlash[58] * 256 + DeviceFlash[59]; // текущее число записей в таблице
				pointer -= 0x100;
				for (int i = 0; i < count; i++)
				{
					child = new Device();
					child.DriverUID = new Guid("efca74b2-ad85-4c30-8de8-8115cc6dfdd2");
					child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
					child.IntAddress = DeviceRom[pointer] + 256 * (DeviceRom[pointer + 1] + 1);
					int zoneNo = DeviceRom[pointer + 5] * 256 + DeviceRom[pointer + 6];
					if (zoneNo != 0)
					{
						child.Zone =
							zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(
								x => (x.No == zoneNo) && x.PanelDevice.IntAddress == device.IntAddress).Zone;
						child.ZoneUID = child.Zone.UID;
					}
					var tableDynamicSize = DeviceRom[pointer + 7];
					var config = new BitArray(new byte[] { DeviceRom[pointer + 7 + tableDynamicSize] });
					pointer = pointer + 8 + tableDynamicSize; // указатель на следующую запись в таблице
					if (config[4])
					{
						var localNoInPPU = Convert.ToInt32(config[3]) * 4 + Convert.ToInt32(config[2]) * 2 + Convert.ToInt32(config[1]);
						groupDevice = (device.Children.FirstOrDefault(x => x.IntAddress == child.IntAddress - localNoInPPU));
						if (groupDevice == null) // если такое ГУ ещё не добавлено
						{
							groupDevice = new Device();
							groupDevice.DriverUID = new Guid("E495C37A-A414-4B47-AF24-FEC1F9E43D86"); // АМ-4
							groupDevice.Driver = Drivers.FirstOrDefault(x => x.UID == groupDevice.DriverUID);
							device.Children.Add(groupDevice);
							groupDevice.IntAddress = child.IntAddress - localNoInPPU;
						}
						else
						{

							if ((groupDevice.Driver.DriverType == DriverType.AM4_P))
							{
								var groupDeviceChild =
									groupDevice.Children.FirstOrDefault(x => x.IntAddress == child.IntAddress);
								groupDeviceChild = child;
							}
						}
						groupDevice.Children.Add(child);
						continue;
					}
					device.Children.Add(child);
				}
			}

			if ((pointer = DeviceFlash[96] * 256 * 256 + DeviceFlash[97] * 256 + DeviceFlash[98]) != 0) // АМ-1Т
			{
				var count = DeviceFlash[100] * 256 + DeviceFlash[101]; // текущее число записей в таблице
				pointer -= 0x100;
				for (int i = 0; i < count; i++)
				{
					child = new Device();
					child.DriverUID = new Guid("f5a34ce2-322e-4ed9-a75f-fc8660ae33d8");
					child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
					child.IntAddress = DeviceRom[pointer] + 256 * (DeviceRom[pointer + 1] + 1);
					int zoneNo = DeviceRom[pointer + 5] * 256 + DeviceRom[pointer + 6];
					if (zoneNo != 0)
					{
						child.Zone =
							zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(
								x => (x.No == zoneNo) && x.PanelDevice.IntAddress == device.IntAddress).Zone;
						child.ZoneUID = child.Zone.UID;
					}
					var tableDynamicSize = DeviceRom[pointer + 7];
					var config = new BitArray(new byte[] { DeviceRom[pointer + 9] });
					pointer = pointer + 8 + tableDynamicSize; // указатель на следующую запись в таблице
					if (config[4])
					{
						var localNoInPPU = Convert.ToInt32(config[3]) * 4 + Convert.ToInt32(config[2]) * 2 + Convert.ToInt32(config[1]);
						groupDevice = device.Children.FirstOrDefault(x => x.IntAddress == child.IntAddress - localNoInPPU);
						if (groupDevice == null) // если такое ГУ ещё не добавлено
						{
							groupDevice = new Device();
							groupDevice.DriverUID = new Guid("E495C37A-A414-4B47-AF24-FEC1F9E43D86"); // АМ-4
							groupDevice.Driver = Drivers.FirstOrDefault(x => x.UID == groupDevice.DriverUID);
							device.Children.Add(groupDevice);
							groupDevice.IntAddress = child.IntAddress - localNoInPPU;
						}

						groupDevice.Children.Add(child);
						continue;
					}
					device.Children.Add(child);
				}
			}
			if ((pointer = DeviceFlash[60] * 256 * 256 + DeviceFlash[61] * 256 + DeviceFlash[62]) != 0)
				MessageBox.Show("Пока не определено"); // Внешние ИУ
			if ((pointer = DeviceFlash[66] * 256 * 256 + DeviceFlash[67] * 256 + DeviceFlash[68]) != 0)
				MessageBox.Show("Пока не определено"); // МУК
			if ((pointer = DeviceFlash[72] * 256 * 256 + DeviceFlash[73] * 256 + DeviceFlash[74]) != 0)
				MessageBox.Show("Пока не определено"); // БУНС
			if ((pointer = DeviceFlash[90] * 256 * 256 + DeviceFlash[91] * 256 + DeviceFlash[92]) != 0)
				MessageBox.Show("Пока не определено"); // Задвижки
			if ((pointer = DeviceFlash[102] * 256 * 256 + DeviceFlash[103] * 256 + DeviceFlash[104]) != 0)
				MessageBox.Show("Пока не определено"); // АМТ-4
			if ((pointer = DeviceFlash[114] * 256 * 256 + DeviceFlash[115] * 256 + DeviceFlash[116]) != 0)
				MessageBox.Show("Пока не определено"); // АСПТ
			if ((pointer = DeviceFlash[132] * 256 * 256 + DeviceFlash[133] * 256 + DeviceFlash[134]) != 0) // ИПР513-11Р
			{
				var count = DeviceFlash[136] * 256 + DeviceFlash[137]; // текущее число записей в таблице
				pointer -= 0x100;
				groupDevice = new Device();
				groupDevice.DriverUID = new Guid("AB3EF7B1-68AD-4A1B-88A8-997357C3FC5B"); // МРК-30
				groupDevice.Driver = Drivers.FirstOrDefault(x => x.UID == groupDevice.DriverUID);
				for (int i = 0; i < count; i++)
				{
					child = new Device();
					child.DriverUID = new Guid("D57CDEF3-ACBC-4773-955E-22A1F016D025"); // ИПР513-11Р
					child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
					child.IntAddress = DeviceRom[pointer] + 256 * (DeviceRom[pointer + 1] + 1);
					int zoneNo = DeviceRom[pointer + 5] * 256 + DeviceRom[pointer + 6];
					if (zoneNo != 0)
					{
						child.Zone =
							zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(
								x => (x.No == zoneNo) && x.PanelDevice.IntAddress == device.IntAddress).Zone;
						child.ZoneUID = child.Zone.UID;
					}
					var tableDynamicSize = DeviceRom[pointer + 7];
					pointer = pointer + 8 + tableDynamicSize; // указатель на следующую запись в таблице
					groupDevice.IntAddress = DeviceRom[pointer - 1] + 256 * child.ShleifNo;
					var oldDevice = device.Children.FirstOrDefault(x => x.IntAddress == groupDevice.IntAddress);
					if (oldDevice != null) // если уже есть такое групповое устройство, то берем его
					{
						groupDevice = oldDevice;
						groupDevice.Children.Add(child);
						continue;
					}
					groupDevice.Children.Add(child);
					device.Children.Add(groupDevice);
				}
			}
			if ((pointer = DeviceFlash[138] * 256 * 256 + DeviceFlash[139] * 256 + DeviceFlash[140]) != 0) // ИП 212-64Р
			{
				var count = DeviceFlash[142] * 256 + DeviceFlash[143]; // текущее число записей в таблице
				pointer -= 0x100;
				groupDevice = new Device();
				groupDevice.DriverUID = new Guid("AB3EF7B1-68AD-4A1B-88A8-997357C3FC5B"); // МРК-30
				groupDevice.Driver = Drivers.FirstOrDefault(x => x.UID == groupDevice.DriverUID);
				for (int i = 0; i < count; i++)
				{
					child = new Device();
					child.DriverUID = new Guid("CFD407D1-5D19-43EC-9650-A86EC4422EC6"); // ИП 212-64Р
					child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
					child.IntAddress = DeviceRom[pointer] + 256 * (DeviceRom[pointer + 1] + 1);
					int zoneNo = DeviceRom[pointer + 5] * 256 + DeviceRom[pointer + 6];
					if (zoneNo != 0)
					{
						child.Zone =
							zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(
								x => (x.No == zoneNo) && x.PanelDevice.IntAddress == device.IntAddress).Zone;
						child.ZoneUID = child.Zone.UID;
					}
					var tableDynamicSize = DeviceRom[pointer + 7];
					pointer = pointer + 8 + tableDynamicSize; // указатель на следующую запись в таблице
					groupDevice.IntAddress = DeviceRom[pointer - 1] + 256 * child.ShleifNo;
					var oldDevice = device.Children.FirstOrDefault(x => x.IntAddress == groupDevice.IntAddress);
					if (oldDevice != null) // если уже есть такое групповое устройство, то берем его
					{
						groupDevice = oldDevice;
						groupDevice.Children.Add(child);
						continue;
					}
					device.Children.Add(groupDevice);
				}
			}
			#endregion
			foreach (var childDevice in device.Children)
			{
				remoteDeviceConfiguration.Devices.Add(childDevice);
			}
			return remoteDeviceConfiguration;
		}
        public static List<byte> GetDeviceFlashUsb(Device device)
        {
            // Особенность чтение базы по Usb в том, что мы читаем блок не по 0xFF, а по 0x33
            var bytes = new List<byte>();
            var begin = _deviceFlashFirstIndex;
            #region Находим адрес конечного блока Flash и число байт в этом блоке
            bytes = CreateBytesArray(0x02, 0x38, BitConverter.GetBytes(begin).Reverse(), 0x33);
            var result = SendCode(bytes).Result.FirstOrDefault().Data;
            result.InsertRange(0, new List<byte> { 0, 0, 0, 0, 0 });
            result.RemoveRange(0, 7); // удаляем служебные байты (id - 4б, адрес приемника - 1б, адрес получателя - 1б, код функции - 1б)
            var end = 256 * 256* result[9] + 256* result[10] + result[11];
            #endregion
            var request = new List<byte>();
            for (int i = begin + 0x34; i < end - 0x34; i+=0x34)
            {
                bytes = CreateBytesArray(0x02, 0x38, BitConverter.GetBytes(i).Reverse(), 0x33);
                request = SendCode(bytes).Result.FirstOrDefault().Data;
                request.RemoveRange(0, 2);
                result.AddRange(request);
            }
            var residual = (end - begin)%0x34;
            bytes = CreateBytesArray(0x02, 0x38, BitConverter.GetBytes(end - residual).Reverse(), residual - 1);
            request = SendCode(bytes).Result.FirstOrDefault().Data;
            request.RemoveRange(0, 2);
            result.AddRange(request);
            return result;
        }
        public static List<byte> GetDeviceFlash(Device device)
        {
            var bytes = new List<byte>();
            var begin = _deviceFlashFirstIndex / 0x100;
            #region Находим адрес конечного блока Flash и число байт в этом блоке
            bytes = CreateBytesArray(BitConverter.GetBytes(++_usbRequestNo).Reverse(), device.Parent.IntAddress + 2,
            device.AddressOnShleif, 0x38, BitConverter.GetBytes(begin * 0x100).Reverse(), 0xFF);
            var result = SendCode(bytes).Result.FirstOrDefault().Data;
            result.RemoveRange(0, 7); // удаляем служебные байты (id - 4б, адрес приемника - 1б, адрес получателя - 1б, код функции - 1б)
            var end = 256 * result[9] + result[10];
            var count = result[11];
            #endregion Находим адрес конечного блока Rom и число байт в этом блоке
            var request = new List<byte>();
            #region Читаем все кроме последнего блока
            for (int i = begin + 1; i < end; i++)
            {
                bytes = CreateBytesArray(BitConverter.GetBytes(++_usbRequestNo).Reverse(), device.Parent.IntAddress + 2,
                device.AddressOnShleif, 0x38, BitConverter.GetBytes(i * 0x100).Reverse(), 0xFF);
                request = SendCode(bytes).Result.FirstOrDefault().Data;
                request.RemoveRange(0, 7);
                result.AddRange(request);
            }
            #endregion
            #region Читаем последний блок
            bytes = CreateBytesArray(BitConverter.GetBytes(++_usbRequestNo).Reverse(), device.Parent.IntAddress + 2,
            device.AddressOnShleif, 0x38, BitConverter.GetBytes(end * 0x100).Reverse(), count);
            request = SendCode(bytes).Result.FirstOrDefault().Data;
            request.RemoveRange(0, 7);
            result.AddRange(request);
            #endregion
            DeviceFlash = new List<byte>(result);
            #region Записываем БД DeviceFlash в deviceFlash.txt
            var deviceFlashTxt = new StreamWriter("..\\deviceFlash.txt");
            int j = 0;
            foreach (var b in DeviceFlash)
            {
                deviceFlashTxt.Write("{0} ", b.ToString("X2"));
                j++;
                if (j % 16 == 0)
                    deviceFlashTxt.Write("\n");
            }
            deviceFlashTxt.Close();
            #endregion
            return result;
        }
        public static List<byte> GetDeviceRomUsb(Device device)
        {
            var result = new List<byte>();
            var bytes = new List<byte>();
            var end = _deviceRomLastIndex;
            var request = new List<byte>();
            #region Читаем все кроме последнего блока
            for (int i = 0x100; i < end - 0x34; i+=0x34)
            {
                bytes = CreateBytesArray(0x02, 0x01, 0x52, BitConverter.GetBytes(i).Reverse(), 0x33);
                request = SendCode(bytes).Result.FirstOrDefault().Data;
                request.RemoveRange(0, 2);
                result.AddRange(request);
            }
            #endregion
            #region Читаем последний блок
            var residual = (end - 0x100) % 0x34;
            bytes = CreateBytesArray(0x02, 0x01, 0x52, BitConverter.GetBytes(end - residual).Reverse(), residual - 1);
            request = SendCode(bytes).Result.FirstOrDefault().Data;
            request.RemoveRange(0, 2);
            result.AddRange(request);
            #endregion
            return result;
        }
		public static List<byte> GetDeviceRom(Device device)
		{
			var result = new List<byte>();
			var bytes = new List<byte>();
			var end = _deviceRomLastIndex / 0x100;
			var count = _deviceRomLastIndex % 0x100;
			var request = new List<byte>();
            #region Читаем все кроме последнего блока
            for (int i = 1; i < end; i++)
			{
                bytes = CreateBytesArray(BitConverter.GetBytes(++_usbRequestNo).Reverse(), Convert.ToByte(device.Parent.IntAddress + 2),
                Convert.ToByte(device.AddressOnShleif), 0x01, 0x52, BitConverter.GetBytes(i * 0x100).Reverse(), Convert.ToByte(0xFF));
                request = SendCode(bytes).Result.FirstOrDefault().Data;
                request.RemoveRange(0, 7);
				result.AddRange(request);
            }
            #endregion
            #region Читаем последний блок
            bytes = CreateBytesArray(BitConverter.GetBytes(++_usbRequestNo).Reverse(), Convert.ToByte(device.Parent.IntAddress + 2),
            Convert.ToByte(device.AddressOnShleif), 0x01, 0x52, BitConverter.GetBytes(end * 0x100).Reverse(), Convert.ToByte(count));
            request = SendCode(bytes).Result.FirstOrDefault().Data;
            request.RemoveRange(0, 7);
            result.AddRange(request);
			#endregion
            #region Записываем БД DeviceRom в deviceRom.txt
            var deviceRomTxt = new StreamWriter("..\\deviceRom.txt");
			int j = 256;
			foreach (var b in result)
			{
				deviceRomTxt.Write("{0} ", b.ToString("X2"));
				j++;
				if (j % 16 == 0)
					deviceRomTxt.Write("\n");
			}
			deviceRomTxt.Close();
            #endregion
            return result;
		}
		private static void GetDeviceFlashFirstAndRomLastIndex(Device device, bool isUsb)
		{
			#region Находим адрес начального блока Flash
            List<byte> bytes;
            if(isUsb)
                bytes = CreateBytesArray(0x02, 0x01, 0x57);
            else
                bytes = CreateBytesArray(BitConverter.GetBytes(++_usbRequestNo).Reverse(), device.Parent.IntAddress + 2,
            device.AddressOnShleif, 0x01, 0x57);
            var result = SendCode(bytes).Result.FirstOrDefault().Data;
            if (isUsb)
                result.InsertRange(0, new List<byte>{0,0,0,0,0});
            var begin = 256 * result[8] + result[9];
			_deviceFlashFirstIndex = begin * 0x100;
			#endregion
			#region Находим адрес конечного блока Rom
            if (isUsb)
                bytes = CreateBytesArray(0x02, 0x38, BitConverter.GetBytes(_deviceFlashFirstIndex).Reverse(), 0x0B);
            else
                bytes = CreateBytesArray(BitConverter.GetBytes(++_usbRequestNo).Reverse(), device.Parent.IntAddress + 2,
                device.AddressOnShleif, 0x38, BitConverter.GetBytes(_deviceFlashFirstIndex).Reverse(), 0x0B);
            result = SendCode(bytes).Result.FirstOrDefault().Data;
            if (isUsb)
                result.InsertRange(0, new List<byte> { 0, 0, 0, 0, 0});
			_deviceRomLastIndex = result[13] * 256 * 256 + result[14] * 256 + result[15];
			#endregion
		}
	}
}