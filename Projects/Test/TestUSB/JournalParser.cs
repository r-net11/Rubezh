using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestUSB
{
	public class JournalParser
	{
		List<byte> bytes;
		JournalItem journalItem;

		public JournalParser(List<byte> rawBytes)
		{
			bytes = rawBytes;
		}

	    private int _no = 0;
		public JournalItem Parce()
		{
			journalItem = new JournalItem();
			var timeBytes = bytes.GetRange(1, 4);
			journalItem.Date = TimeParceHelper.Parce(timeBytes);
			var eventName = MetadataHelper.GetEventByCode(bytes[0]);
			ExactEventForFlag(eventName, bytes[5]);
            bool s = journalItem.EventName.Contains("[");
		    journalItem.No = _no;
            _no++;
			journalItem.Flag = bytes[5];
			journalItem.ShleifNo = bytes[6];
			journalItem.IntType = bytes[7];
            if (journalItem.IntType != 0)
            {
                var deviceUid = MainWindow.GetUidById((ushort) journalItem.IntType).ToString().ToUpper();
                var device = "Неизвестное устройство";
                if (deviceUid != "00000000-0000-0000-0000-000000000000")
                    device = MetadataHelper.Metadata.deviceTables.FirstOrDefault(x => ((x.deviceDriverID != null) && (x.deviceDriverID.Equals(deviceUid)))).shortName;
                journalItem.Description = "Устройство: " + device;
            }
		    journalItem.Address = bytes[8];
			journalItem.State = bytes[9];
			journalItem.ZoneNo = bytes[10] * 256 + bytes[11];
			journalItem.DescriptorNo = bytes[12] * 256 * 256 + bytes[13] * 256 + bytes[14];

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
				var secondParts = secondPart.Split('/');
				if (flag < secondParts.Count())
				{
					var choise = secondParts[flag];
					journalItem.EventName = firstPart + choise;
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