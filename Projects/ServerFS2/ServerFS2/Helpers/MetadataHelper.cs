using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace ServerFS2
{
	public static class MetadataHelper
	{
		public static Rubezh2010.driverConfig Metadata { get; private set; }

		public static void Initialize()
		{
			using (var fileStream = new FileStream("rubezh2010.xml", FileMode.Open))
			{
				var serializer = new XmlSerializer(typeof(Rubezh2010.driverConfig));
				Metadata = (Rubezh2010.driverConfig)serializer.Deserialize(fileStream);
			}
		}

		public static string GetEventByCode(int eventCode)
		{
			string stringCode = "$" + eventCode.ToString("X2");
			var nativeEvent = Metadata.events.FirstOrDefault(x => x.rawEventCode == stringCode);
			if (nativeEvent != null)
				return nativeEvent.eventMessage;
			return "Неизвестный код события " + eventCode.ToString("x2");
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

        public static Guid GetUidById(ushort driverTypeNo)
        {
            switch (driverTypeNo)
            {
                case 0x7C:  return new Guid("584BC59A-28D5-430B-90BF-592E40E843A6"); // "УОО-ТЛ"
                case 0x7E:  return new Guid("043fbbe0-8733-4c8d-be0c-e5820dbf7039"); // "Модуль дымоудаления-1"
                case 0x51:  return new Guid("dba24d99-b7e1-40f3-a7f7-8a47d4433392"); // "Пожарная адресная метка АМ-1"
                case 0x34:  return new Guid("efca74b2-ad85-4c30-8de8-8115cc6dfdd2"); // "Охранная адресная метка АМ1-О"
                case 0xD2:  return new Guid("f5a34ce2-322e-4ed9-a75f-fc8660ae33d8"); // "Технологическая адресная метка АМ1-Т"
                case 0x50:  return new Guid("d8997f3b-64c4-4037-b176-de15546ce568"); // "Пожарная адресная метка АМП"
                case 0x70:  return new Guid("8bff7596-aef4-4bee-9d67-1ae3dc63ca94"); // "Шкаф управления насосом"
                case 0x71:  return new Guid("4935848f-0084-4151-a0c8-3a900e3cb5c5"); // "Шкаф управления задвижкой"
                case 0x60:  return new Guid("37f13667-bc77-4742-829b-1c43fa404c1f"); // "Пожарный комбинированный извещатель ИП212/101-64-А2R1"
                case 0x55:  return new Guid("641fa899-faa0-455b-b626-646e5fbe785a"); // "Ручной извещатель ИПР513-11"
                case 0x62:  return new Guid("799686b6-9cfa-4848-a0e7-b33149ab940c"); // "Пожарный тепловой извещатель ИП 101-29-A3R1"
                case 0x76:  return new Guid("33a85f87-e34c-45d6-b4ce-a4fb71a36c28"); // "Модуль пожаротушения МПТ-1"
                case 0x74:  return new Guid("2d078d43-4d3b-497c-9956-990363d9b19b"); // "Модуль речевого оповещения МРО-2"
                case 0x75:  return new Guid("4a60242a-572e-41a8-8b87-2fe6b6dc4ace"); // "Релейный исполнительный модуль РМ-1"
                case 0x61:  return new Guid("1e045ad6-66f9-4f0b-901c-68c46c89e8da"); // "Пожарный дымовой извещатель ИП 212-64"
                case 0x103: return new Guid("200EED4B-3402-45B4-8122-AE51A4841E18"); // "Индикатор ГК"
                case 0x104: return new Guid("DEAA33C2-0EAA-4D4D-BA31-FCDBE0AD149A"); // "Линия ГК"
                case 0x105: return new Guid("1AC85436-61BC-441B-B6BF-C6A0FA62748B"); // "Реле ГК"
                case 0x102: return new Guid("4993E06C-85D1-4F20-9887-4C5F67C450E8"); // "Контроллер адресных устройств"
                default:    return new Guid("00000000-0000-0000-0000-000000000000"); // "Неизвестное устройство"
            }
        }

        public static ushort GetIdByUid(Guid driverUid)
        {
            switch (driverUid.ToString())
            {
                case "584BC59A-28D5-430B-90BF-592E40E843A6": return 0x7C; // "УОО-ТЛ"
                case "043fbbe0-8733-4c8d-be0c-e5820dbf7039": return 0x7E; // "Модуль дымоудаления-1"
                case "dba24d99-b7e1-40f3-a7f7-8a47d4433392": return 0x51; // "Пожарная адресная метка АМ-1"
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
	}
}