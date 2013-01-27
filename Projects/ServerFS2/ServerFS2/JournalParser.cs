using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace ServerFS2
{
	public class JournalParser
	{
		List<byte> bytes;
        List<byte> allBytes;
		JournalItem journalItem;

		public JournalParser(List<byte> rawBytes, List<byte> allbytes)
		{
			bytes = rawBytes;
		    allBytes = allbytes;
		}

		public JournalItem Parce()
		{
			journalItem = new JournalItem();
            foreach (var b in allBytes)
                journalItem.ByteTracer += b.ToString("X2") + " ";
			var timeBytes = bytes.GetRange(1, 4);
			journalItem.Date = TimeParceHelper.Parce(timeBytes);
            var bitsExtracter = new BitsExtracter(timeBytes);
            var day = bitsExtracter.Get(0, 4);
            var month = bitsExtracter.Get(5, 8);
            var year = bitsExtracter.Get(9, 14);
            var hour = bitsExtracter.Get(15, 19);
            var min = bitsExtracter.Get(20, 25);
            var sec = bitsExtracter.Get(26, 31);
            journalItem.IntDate = (uint)sec + (uint)min * 60 + (uint)hour * 60 * 60 + (uint)day * 60 * 60 * 24 + (uint)month * 60 * 60 * 24 * 30 + (uint)year * 60 * 60 * 24 * 30 * 12;
			var eventName = MetadataHelper.GetEventByCode(bytes[0]);
			ExactEventForFlag(eventName, bytes[5]);
			journalItem.Flag = bytes[5];
			journalItem.ShleifNo = bytes[6] + 1;
			journalItem.IntType = bytes[7];
		    journalItem.FirstAddress = bytes[17] + 1;
		    journalItem.Address = bytes[8];
			journalItem.State = bytes[9];
			journalItem.ZoneNo = bytes[10] * 256 + bytes[11];
			journalItem.DescriptorNo = bytes[12] * 256 * 256 + bytes[13] * 256 + bytes[14];
            if (bytes[0] == 0x83)
                journalItem.Description += "Выход: " + journalItem.ShleifNo + "\n";
            if (bytes[0] == 0x0F)
                journalItem.Description += "АЛС: " + journalItem.ShleifNo + "\n";
            // Системная неисправность
            if (bytes[0] == 0x0D)
            {
                if (journalItem.State == 0x20)
                    journalItem.Description += "база (сигнатура) повреждена или отсутствует\n";
            }
            if (journalItem.ZoneNo != 0)
            {
                journalItem.Description += "Зона: " + journalItem.ZoneNo + "\n";
            }

            //Охранные события (сброс тревоги, постановка, снятие)
            #region
            if (bytes[0] == 0x28) 
            {
                switch (allBytes[24])
                {
                    case 0:
                        {
                            journalItem.Description += "команда с компьютера\n";
                            if (allBytes[23] == 0)
                                journalItem.Description += "через USB\n";
                            else
                                journalItem.Description += "через канал МС " + allBytes[23] + "\n";
                            break;
                        }
                    case 3:
                        journalItem.Description += "Прибор: Рубеж-БИ Адрес:" + allBytes[23] + "\n";
                        break;
                    case 7:
                        journalItem.Description += "Прибор: Рубеж-ПДУ Адрес:" + allBytes[23] + "\n";
                        break;
                    case 9:
                        journalItem.Description += "Прибор: Рубеж-ПДУ-ПТ Адрес:" + allBytes[23] + "\n";
                        break;
                    case 100:
                        journalItem.Description += "Устройство: МС-3 Адрес:" + allBytes[23] + "\n";
                        break;
                    case 101:
                        journalItem.Description += "Устройство: МС-4 Адрес:" + allBytes[23] + "\n";
                        break;
                    case 102:
                        journalItem.Description += "Устройство: УОО-ТЛ Адрес:" + allBytes[23] + "\n";
                        break;
                    default:
                        journalItem.Description += "Неизв. устр." + "(" + allBytes[24] + ") Адрес:" + allBytes[23] + "\n";
                        break;
                }
                if (journalItem.IntType == 0x00)
                    journalItem.IntType = 0x75;
            }
            #endregion

            // Потеря связи с мониторинговой станцией (БИ, ПДУ, УОО-ТЛ, МС-1, МС-2)
            #region
            if (bytes[0] == 0x85)
            {
                switch (bytes[7])
                {
                    case 3:
                        journalItem.Description += "Прибор: Рубеж-БИ Адрес:" + bytes[6] + "\n";
                        break;
                    case 7:
                        journalItem.Description += "Прибор: Рубеж-ПДУ Адрес:" + bytes[6] + "\n";
                        break;
                    case 100:
                        journalItem.Description += "Устройство: МС-3 Адрес:" + bytes[6] + "\n";
                        break;
                    case 101:
                        journalItem.Description += "Устройство: МС-4 Адрес:" + bytes[6] + "\n";
                        break;
                    case 102:
                        journalItem.Description += "Устройство: УОО-ТЛ Адрес:" + bytes[6] + "\n";
                        break;
                    default:
                        journalItem.Description += "Неизв. устр." + "(" + bytes[7] + ") Адрес:" + bytes[6] + "\n";
                        break;
                }
                return journalItem;
            }
		    #endregion
            int tableType = 99999;
            if (journalItem.IntType != 0)
            {
                var deviceUid = MetadataHelper.GetUidById((ushort)journalItem.IntType).ToString().ToUpper();
                var device = "Неизвестное устройство";
                if (deviceUid != "00000000-0000-0000-0000-000000000000")
                {
                    device = MetadataHelper.Metadata.deviceTables.FirstOrDefault(x => ((x.deviceDriverID != null) && (x.deviceDriverID.Equals(deviceUid)))).shortName;
                    tableType = Convert.ToInt32(MetadataHelper.Metadata.deviceTables.FirstOrDefault(x => ((x.deviceDriverID != null) && (x.deviceDriverID.Equals(deviceUid)))).tableType);
                }
                if (journalItem.IntType == 1)
                    device = "АСПТ " + (journalItem.ShleifNo - 1) + ".";
                journalItem.Description += "Устройство: " + device + " " + journalItem.FirstAddress + "." + journalItem.Address + "\n";
            }
            // Детализация событий
            if (tableType != 99999)
            {
                var even = MetadataHelper.Metadata.events.FirstOrDefault(x => x.rawEventCode == "$" + bytes[0].ToString("X2"));
                if (even.detailsFor != null)
                {
                    var details = even.detailsFor.FirstOrDefault(x => x.tableType == tableType.ToString());
                    if (details != null)
                    {
                        var dictionaryName =
                            even.detailsFor.FirstOrDefault(x => x.tableType == tableType.ToString()).dictionary;
                        var dictionary = MetadataHelper.Metadata.dictionary.FirstOrDefault(x => x.name == dictionaryName);
                        var bitState = new BitArray(new int[] {journalItem.State});
                        foreach (var bit in dictionary.bit)
                        {
                            if (bitState.Get(Convert.ToInt32(bit.no)))
                                journalItem.Description += dictionary.bit.FirstOrDefault(x => x.no == bit.no).value + "\n";
                        }
                    }
                }
            }
			SetEventClass(bytes[0], bytes[5]);
			return journalItem;
		}

		void ExactEventForFlag(string eventName, int flag)
		{
            var firstIndex = eventName.IndexOf("[");
			var lastIndex = eventName.IndexOf("]");
			if (firstIndex != -1 && lastIndex != -1)
			{
				var firstPart = eventName.Substring(0, firstIndex);
				var secondPart = eventName.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
			    var thirdPart = eventName.Substring(lastIndex + 1);
				var secondParts = secondPart.Split('/');
				if (flag < secondParts.Count())
				{
					var choise = secondParts[flag];
					journalItem.EventName = firstPart + choise + thirdPart;
					return;
				}
			}
			journalItem.EventName = eventName;
		}

		void SetEventClass(int eventCode, int flag)
		{
			journalItem.EventClass = -1;
			var stringEventClass = GetEventClass(eventCode, flag);
			if (!string.IsNullOrEmpty(stringEventClass))
			{
				try
				{
					journalItem.EventClass = int.Parse(stringEventClass);
				}
				catch { }
			}
		}

		string GetEventClass(int eventCode, int flag)
		{
			string stringCode = "$" + eventCode.ToString("X2");
			var nativeEvent = MetadataHelper.Metadata.events.FirstOrDefault(x => x.rawEventCode == stringCode);
            if (nativeEvent == null)
                return "Неизвестный код события 0x" + eventCode;
			if (nativeEvent.detailsFor != null && nativeEvent.detailsFor.Count() > 0)
			{
				var detailsFor = nativeEvent.detailsFor;
			}
			if (nativeEvent != null)
			{
				if (nativeEvent.eventClassAll != null)
					return nativeEvent.eventClassAll;
				switch (flag)
				{
					case 0:
						return nativeEvent.eventClass;
					case 1:
						return nativeEvent.eventClass1;
					case 2:
						return nativeEvent.eventClass2;
					case 3:
						return nativeEvent.eventClass3;
					case 4:
						return nativeEvent.eventClass4;
					case 5:
						return nativeEvent.eventClass5;
					case 6:
						return nativeEvent.eventClass6;
					case 7:
						return nativeEvent.eventClass7;
					case 8:
						return nativeEvent.eventClass8;
					case 9:
						return nativeEvent.eventClass9;
					case 10:
						return nativeEvent.eventClass10;
					case 12:
						return nativeEvent.eventClass12;
					case 14:
						return nativeEvent.eventClass14;
					case 15:
						return nativeEvent.eventClass15;
					case 16:
						return nativeEvent.eventClass16;
					case 17:
						return nativeEvent.eventClass18;
					case 20:
						return nativeEvent.eventClass20;
					case 22:
						return nativeEvent.eventClass22;
				}
			}
			return null;
		}
	}
}