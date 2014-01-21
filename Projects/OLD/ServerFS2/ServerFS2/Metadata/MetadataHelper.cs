using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using Common;
using FiresecAPI.Models;
using Infrastructure.Common;
using System.Diagnostics;

namespace ServerFS2
{
	public static class MetadataHelper
	{
		public static Rubezh2010.driverConfig Metadata { get; private set; }

		public static void Initialize()
		{
			try
			{
				var fileName = AppDataFolderHelper.GetServerAppDataPath("rubezh2010.xml");
				if (!File.Exists(fileName))
					fileName = @"Metadata\rubezh2010.xml";
				using (var fileStream = new FileStream(fileName, FileMode.Open))
				{
					var serializer = new XmlSerializer(typeof(Rubezh2010.driverConfig));
					var deserializesObject = serializer.Deserialize(fileStream);
					Metadata = (Rubezh2010.driverConfig)deserializesObject;
				}
			}
			catch (Exception e)
			{
				MessageBox.Show("MetadataHelper");
				Logger.Error(e, "MetadataHelper.Initialize");
			}
		}

		static void Test1()
		{
			foreach (var metadataDeviceState in MetadataHelper.Metadata.deviceStates)
			{
				var bits = GetBits(metadataDeviceState);
				if (bits != null)
				{
					var metadataDeviceTable = Metadata.deviceTables.FirstOrDefault(x => x.tableType == metadataDeviceState.tableType);
					var deviceName = metadataDeviceState.tableType;
					if (metadataDeviceTable != null)
						deviceName = metadataDeviceTable.deviceName;
					Trace.WriteLine(deviceName + " " + metadataDeviceState.name);
				}
			}
		}

		public static Rubezh2010.driverConfigDeviceTablesDeviceTable GetMetadataDeviceTable(Device device)
		{
			foreach (var metadataDeviceTableItem in MetadataHelper.Metadata.deviceTables)
			{
				if (metadataDeviceTableItem.deviceDriverID != null)
				{
					var guid = new Guid(metadataDeviceTableItem.deviceDriverID);
					if (guid == device.DriverUID)
					{
						return metadataDeviceTableItem;
					}
				}
			}
			return null;
		}

		public static string GetDeviceTableNo(Device device)
		{
			if (device != null)
			{
				var metadataDeviceTable = Metadata.deviceTables.FirstOrDefault(x => x.deviceDriverID != null && x.deviceDriverID == device.Driver.UID.ToString().ToUpper());
				if (metadataDeviceTable != null)
				{
					return metadataDeviceTable.tableType;
				}
			}
			return null;
		}

		public static int GetBitNo(Rubezh2010.driverConfigDeviceStatesDeviceState metadataDeviceState)
		{
			string bitNoString = null;
			if (metadataDeviceState.bitno != null)
				bitNoString = metadataDeviceState.bitno;
			if (metadataDeviceState.bitNo != null)
				bitNoString = metadataDeviceState.bitNo;
			if (metadataDeviceState.Bitno != null)
				bitNoString = metadataDeviceState.Bitno;
			
			if (bitNoString != null)
			{
				int bitNo = -1;
				var result = Int32.TryParse(bitNoString, out bitNo);
				if (result)
				{
					return bitNo;
				}
			}
			return -1;
		}

		public static int GetIntBitNo(Rubezh2010.driverConfigDeviceStatesDeviceState metadataDeviceState)
		{
			string bitNoString = null;
			if (metadataDeviceState.intBitno != null)
				bitNoString = metadataDeviceState.intBitno;
			if (metadataDeviceState.Intbitno != null)
				bitNoString = metadataDeviceState.Intbitno;

			if (bitNoString != null)
			{
				int bitNo = -1;
				var result = Int32.TryParse(bitNoString, out bitNo);
				if (result)
				{
					return bitNo;
				}
			}
			return -1;
		}

		public static List<int> GetBits(Rubezh2010.driverConfigDeviceStatesDeviceState metadataDeviceState)
		{
			string bits = null;
			if (metadataDeviceState.bits != null)
				bits = metadataDeviceState.bits;
			if (metadataDeviceState.Bits != null)
				bits = metadataDeviceState.Bits;

			if (bits != null && bits.Contains('-'))
			{
				var values = bits.Split('-');
				if (values.Count() == 2)
				{
					var result = new List<int>();
					int parsedInt = 0;
					if (Int32.TryParse(values[0], out parsedInt))
					{
						result.Add(parsedInt);
						if (Int32.TryParse(values[1], out parsedInt))
						{
							result.Add(parsedInt);
						}
					}
					if (metadataDeviceState.value != null)
					{
						if (Int32.TryParse(metadataDeviceState.value, out parsedInt))
						{
							result.Add(parsedInt);
							return result;
						}
					}
				}
			}
			return null;
		}

		public static List<Rubezh2010.driverConfigDeviceStatesDeviceState> GetMetadataDeviceStates(Device device, bool isFireAndWarning)
		{
			//Test1();
			var result = new List<Rubezh2010.driverConfigDeviceStatesDeviceState>();

			var tableNo = MetadataHelper.GetDeviceTableNo(device);
			if (tableNo != null)
			{
				foreach (var metadataDeviceState in MetadataHelper.Metadata.deviceStates)
				{
					if (metadataDeviceState.tableType == tableNo)
					{
						if (metadataDeviceState.ID == "MDU_Test")
							continue;
						result.Add(metadataDeviceState);
					}
				}

				foreach (var metadataDeviceState in MetadataHelper.Metadata.deviceStates)
				{
					if (metadataDeviceState.tableType == null && metadataDeviceState.notForTableType != tableNo)
					{
						var bitNo = GetBitNo(metadataDeviceState);
						var intBitNo = GetIntBitNo(metadataDeviceState);
						var bits = GetBits(metadataDeviceState);
						if (bitNo != -1)
						{
							if (!result.Any(x => GetBitNo(x) == bitNo))
							{
								if (!isFireAndWarning)
								{
									if (metadataDeviceState.ID == "Alarm" || metadataDeviceState.ID == "Warning")
									{
										switch (device.Driver.DriverType)
										{
											case DriverType.HandDetector:
											case DriverType.RadioHandDetector:
												if (metadataDeviceState.ID == "Warning")
													continue;
												break;

											default:
												if (metadataDeviceState.ID == "Alarm")
													continue;
												break;

										}
									}
								}
								result.Add(metadataDeviceState);
							}
						}
						else
						{
							if (intBitNo != -1)
							{
								if (!result.Any(x => GetIntBitNo(x) == intBitNo))
								{
									result.Add(metadataDeviceState);
								}
							}
						}

						if (bitNo == -1 && intBitNo == -1 && bits == null)
						{
							result.Add(metadataDeviceState);
						}
					}
				}
			}

			return result;
		}

		public static string GetEventMessage(int eventCode, string deviceTableType)
		{
			string stringCode = "$" + eventCode.ToString("X2");
			var metadataEvent = Metadata.events.FirstOrDefault(x => x.rawEventCode == stringCode);
			if (metadataEvent != null)
			{
				if (metadataEvent.alterFor != null && deviceTableType != null)
				{
					var metadataAlterFor = metadataEvent.alterFor.FirstOrDefault(x => x.tableType == deviceTableType);
					if (metadataAlterFor != null)
					{
						return metadataAlterFor.eventMessage;
					}
				}
				return metadataEvent.eventMessage;
			}
			return "Неизвестный код события " + eventCode.ToString("x2");
		}

		public static bool HasDevise(int eventCode)
		{
			string stringCode = "$" + eventCode.ToString("X2");
			var metadataEvent = Metadata.events.FirstOrDefault(x => x.rawEventCode == stringCode);
			if (metadataEvent != null)
			{
				return metadataEvent.hasDevice != null && metadataEvent.hasDevice == "1";
			}
			return false;
		}

		public static bool HasZone(int eventCode)
		{
			string stringCode = "$" + eventCode.ToString("X2");
			var metadataEvent = Metadata.events.FirstOrDefault(x => x.rawEventCode == stringCode);
			if (metadataEvent != null)
			{
				return metadataEvent.hasZoneExt != null && metadataEvent.hasZoneExt == "1";
			}
			return false;
		}

		public static string GetEventStateClassString(int eventCode, int additionalEventCode)
		{
			string stringCode = "$" + eventCode.ToString("X2");
			var metadataEvent = Metadata.events.FirstOrDefault(x => x.rawEventCode == stringCode);
			if (metadataEvent != null)
			{
				if (metadataEvent.eventClassAll != null)
					return metadataEvent.eventClassAll;

				switch (additionalEventCode)
				{
					case 0:
						return metadataEvent.eventClass;

					case 1:
						return metadataEvent.eventClass1;

					case 2:
						return metadataEvent.eventClass2;

					case 3:
						return metadataEvent.eventClass3;

					case 4:
						return metadataEvent.eventClass4;

					case 5:
						return metadataEvent.eventClass5;

					case 6:
						return metadataEvent.eventClass6;

					case 7:
						return metadataEvent.eventClass7;

					case 8:
						return metadataEvent.eventClass8;

					case 9:
						return metadataEvent.eventClass9;

					case 10:
						return metadataEvent.eventClass10;

					case 12:
						return metadataEvent.eventClass12;

					case 14:
						return metadataEvent.eventClass14;

					case 15:
						return metadataEvent.eventClass15;

					case 16:
						return metadataEvent.eventClass16;

					case 17:
						return metadataEvent.eventClass17;

					case 18:
						return metadataEvent.eventClass18;

					case 20:
						return metadataEvent.eventClass20;

					case 22:
						return metadataEvent.eventClass22;
				}
			}
			return null;
		}

		public static string GetExactEventForFlag(string eventName, int flag)
		{
			var firstIndex = eventName.IndexOf("[");
			var lastIndex = eventName.IndexOf("]");
			if (firstIndex != -1 && lastIndex != -1)
			{
				var firstPart = eventName.Substring(0, firstIndex);
				var secondPart = eventName.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
				var secondParts = secondPart.Split('/');
				if (flag < secondParts.Count())
				{
					var choise = secondParts[flag];
					return firstPart + choise;
				}
			}
			return eventName;
		}

		public static Guid GetUidById(int driverTypeNo)
		{
			switch (driverTypeNo)
			{
				case 0x7C: return new Guid("584BC59A-28D5-430B-90BF-592E40E843A6"); // "УОО-ТЛ"
				case 0x7E: return new Guid("043fbbe0-8733-4c8d-be0c-e5820dbf7039"); // "Модуль дымоудаления-1"
				case 0x51: return new Guid("dba24d99-b7e1-40f3-a7f7-8a47d4433392"); // "Пожарная адресная метка АМ-1"
				case 0x52: return new Guid("CD7FCB14-F808-415C-A8B7-11C512C275B4"); // "Кнопка останова СПТ"
				case 0x54: return new Guid("E8C04507-0C9D-429C-9BBE-166C3ECA4B5C"); // "Кнопка запуска СПТ"
				case 0x53: return new Guid("1909EBDF-467D-4565-AD5C-CD5D9084E4C3"); // "Кнопка управления автоматикой"
				case 0x58: return new Guid("2F875F0C-54AA-47CE-B639-FE5E3ED9841B"); // "Кнопка вкл автоматики ШУЗ и насосов в направлении"
				case 0x5A: return new Guid("935B0020-889B-4A94-9563-EC0E4127E8E3"); // "Кнопка разблокировки автоматики ШУЗ в направлении"
				case 0x59: return new Guid("032CDF7B-6787-4612-B3D1-03E0D3FD2F53"); // "Кнопка выкл автоматики ШУЗ и насосов в направлении"
				case 0x34: return new Guid("efca74b2-ad85-4c30-8de8-8115cc6dfdd2"); // "Охранная адресная метка АМ1-О"
				case 0xD2: return new Guid("f5a34ce2-322e-4ed9-a75f-fc8660ae33d8"); // "Технологическая адресная метка АМ1-Т"
				case 0x50: return new Guid("d8997f3b-64c4-4037-b176-de15546ce568"); // "Пожарная адресная метка АМП"
				case 0x70: return new Guid("8bff7596-aef4-4bee-9d67-1ae3dc63ca94"); // "Шкаф управления насосом"
				case 0x71: return new Guid("4935848f-0084-4151-a0c8-3a900e3cb5c5"); // "Шкаф управления задвижкой"
				case 0x60: return new Guid("37f13667-bc77-4742-829b-1c43fa404c1f"); // "Пожарный комбинированный извещатель ИП212/101-64-А2R1"
				case 0x55: return new Guid("641fa899-faa0-455b-b626-646e5fbe785a"); // "Ручной извещатель ИПР513-11"
				case 0x62: return new Guid("799686b6-9cfa-4848-a0e7-b33149ab940c"); // "Пожарный тепловой извещатель ИП 101-29-A3R1"
				case 0x76: return new Guid("33a85f87-e34c-45d6-b4ce-a4fb71a36c28"); // "Модуль пожаротушения МПТ-1"
				case 0x74: return new Guid("2d078d43-4d3b-497c-9956-990363d9b19b"); // "Модуль речевого оповещения МРО-2"
				case 0x75: return new Guid("4a60242a-572e-41a8-8b87-2fe6b6dc4ace"); // "Релейный исполнительный модуль РМ-1"
				case 0x61: return new Guid("1e045ad6-66f9-4f0b-901c-68c46c89e8da"); // "Пожарный дымовой извещатель ИП 212-64"
				case 0x103: return new Guid("200EED4B-3402-45B4-8122-AE51A4841E18"); // "Индикатор ГК"
				case 0x104: return new Guid("DEAA33C2-0EAA-4D4D-BA31-FCDBE0AD149A"); // "Линия ГК"
				case 0x105: return new Guid("1AC85436-61BC-441B-B6BF-C6A0FA62748B"); // "Реле ГК"
				case 0x102: return new Guid("4993E06C-85D1-4F20-9887-4C5F67C450E8"); // "Контроллер адресных устройств"
				default: return new Guid("00000000-0000-0000-0000-000000000000"); // "Неизвестное устройство"
			}
		}

		public static ushort GetIdByUid(Guid driverUid)
		{
			switch (driverUid.ToString())
			{
				case "584BC59A-28D5-430B-90BF-592E40E843A6": return 0x7C; // "УОО-ТЛ"
				case "043fbbe0-8733-4c8d-be0c-e5820dbf7039": return 0x7E; // "Модуль дымоудаления-1"
				case "dba24d99-b7e1-40f3-a7f7-8a47d4433392": return 0x51; // "Пожарная адресная метка АМ-1"
				case "CD7FCB14-F808-415C-A8B7-11C512C275B4": return 0x52; // "Кнопка останова СПТ"
				case "E8C04507-0C9D-429C-9BBE-166C3ECA4B5C": return 0x54; // "Кнопка запуска СПТ"
				case "1909EBDF-467D-4565-AD5C-CD5D9084E4C3": return 0x53; // "Кнопка управления автоматикой"
				case "2F875F0C-54AA-47CE-B639-FE5E3ED9841B": return 0x58; // "Кнопка вкл автоматики ШУЗ и насосов в направлении"
				case "935B0020-889B-4A94-9563-EC0E4127E8E3": return 0x5A; // "Кнопка разблокировки автоматики ШУЗ в направлении"
				case "032CDF7B-6787-4612-B3D1-03E0D3FD2F53": return 0x59; // "Кнопка выкл автоматики ШУЗ и насосов в направлении"
				case "efca74b2-ad85-4c30-8de8-8115cc6dfdd2": return 0x34; // "Охранная адресная метка АМ1-О"
				case "f5a34ce2-322e-4ed9-a75f-fc8660ae33d8": return 0xD2; // "Технологическая адресная метка АМ1-Т"
				case "d8997f3b-64c4-4037-b176-de15546ce568": return 0x50; // "Пожарная адресная метка АМП"
				case "8bff7596-aef4-4bee-9d67-1ae3dc63ca94": return 0x70; // "Шкаф управления насосом"
				case "4935848f-0084-4151-a0c8-3a900e3cb5c5": return 0x71; // "Шкаф управления задвижкой"
				case "37f13667-bc77-4742-829b-1c43fa404c1f": return 0x60; // "Пожарный комбинированный извещатель ИП212/101-64-А2R1"
				case "641fa899-faa0-455b-b626-646e5fbe785a": return 0x55; // "Ручной извещатель ИПР513-11"
				case "799686b6-9cfa-4848-a0e7-b33149ab940c": return 0x62; // "Пожарный тепловой извещатель ИП 101-29-A3R1"
				case "33a85f87-e34c-45d6-b4ce-a4fb71a36c28": return 0x76; // "Модуль пожаротушения МПТ-1"
				case "2d078d43-4d3b-497c-9956-990363d9b19b": return 0x74; // "Модуль речевого оповещения МРО-2"
				case "4a60242a-572e-41a8-8b87-2fe6b6dc4ace": return 0x75; // "Релейный исполнительный модуль РМ-1"
				case "1e045ad6-66f9-4f0b-901c-68c46c89e8da": return 0x61; // "Пожарный дымовой извещатель ИП 212-64"
				case "200EED4B-3402-45B4-8122-AE51A4841E18": return 0x103; // "Индикатор ГК"
				case "DEAA33C2-0EAA-4D4D-BA31-FCDBE0AD149A": return 0x104; // "Линия ГК"
				case "1AC85436-61BC-441B-B6BF-C6A0FA62748B": return 0x105; // "Реле ГК"
				case "4993E06C-85D1-4F20-9887-4C5F67C450E8": return 0x102; // "Контроллер адресных устройств"
				default: return 0x00; // "Неизвестное устройство"
			}
		}

		public static string GetDeviceStateEventEnter(Rubezh2010.driverConfigDeviceStatesDeviceStateEnter deviceStateEnter, int no)
		{
			switch (no)
			{
				case 0:
					return deviceStateEnter.@event;
				case 1:
					return deviceStateEnter.event1;
				case 2:
					return deviceStateEnter.event2;
				case 3:
					return deviceStateEnter.event3;
				case 4:
					return deviceStateEnter.event4;
				case 6:
					return deviceStateEnter.event6;
				case 8:
					return deviceStateEnter.event8;
				case 10:
					return deviceStateEnter.event10;
				case 12:
					return deviceStateEnter.event12;
				case 14:
					return deviceStateEnter.event14;
				case 16:
					return deviceStateEnter.event16;
				case 20:
					return deviceStateEnter.event20;
				case 24:
					return deviceStateEnter.event24;
				default:
					return null;
			}
		}

		public static string GetZoneStateEventEnter(Rubezh2010.driverConfigDeviceStatesDeviceStateEnter deviceStateEnter, int no)
		{
			switch (no)
			{
				case 0:
					return deviceStateEnter.zoneEvent;
				case 2:
					return deviceStateEnter.zoneEvent2;
				default:
					return null;
			}
		}

		public static string GetDeviceStateEventLeave(Rubezh2010.driverConfigDeviceStatesDeviceStateLeave deviceStateLeave, int no)
		{
			switch (no)
			{
				case 0:
					return deviceStateLeave.@event;
				case 1:
					return deviceStateLeave.event1;
				case 2:
					return deviceStateLeave.event2;
				case 3:
					return deviceStateLeave.event3;
				case 4:
					return deviceStateLeave.event4;
				case 5:
					return deviceStateLeave.event5;
				case 6:
					return deviceStateLeave.event6;
				case 7:
					return deviceStateLeave.event7;
				case 8:
					return deviceStateLeave.event8;
				case 9:
					return deviceStateLeave.event9;
				case 10:
					return deviceStateLeave.event10;
				case 12:
					return deviceStateLeave.event12;
				case 14:
					return deviceStateLeave.event14;
				case 16:
					return deviceStateLeave.event16;
				case 18:
					return deviceStateLeave.event18;
				case 20:
					return deviceStateLeave.event20;
				case 22:
					return deviceStateLeave.event22;
				case 24:
					return deviceStateLeave.event24;
				case 26:
					return deviceStateLeave.event26;
				case 30:
					return deviceStateLeave.event30;
				default:
					return null;
			}
		}

		public static string GetZoneStateEventLeave(Rubezh2010.driverConfigDeviceStatesDeviceStateLeave deviceStateLeave, int no)
		{
			switch (no)
			{
				case 0:
					return deviceStateLeave.zoneEvent;
				case 1:
					return deviceStateLeave.zoneEvent1;
				case 2:
					return deviceStateLeave.zoneEvent2;
				default:
					return null;
			}
		}
	}
}