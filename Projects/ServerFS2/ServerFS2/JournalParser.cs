using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FiresecAPI;
using ServerFS2.DataBase;

namespace ServerFS2
{
	public class JournalParser
	{
		readonly List<byte> _bytes;
		readonly List<byte> _allBytes;

		public JournalParser(List<byte> allbytes)
		{
			_allBytes = allbytes;
			_bytes = new List<byte>(_allBytes);
			_bytes.RemoveRange(0, 7);
		}

		public JournalItem Parce()
		{
			JournalItem _journalItem = new JournalItem();
			foreach (var b in _allBytes)
				_journalItem.ByteTracer += b.ToString("X2") + " ";
			if (_bytes.Count < 32)
			{
				Trace.WriteLine(_journalItem.ByteTracer);
				return null;
			}
			var timeBytes = _bytes.GetRange(1, 4);
			_journalItem.Date = TimeParceHelper.Parce(timeBytes);
			var bitsExtracter = new BitsExtracter(timeBytes);
			var day = bitsExtracter.Get(0, 4);
			var month = bitsExtracter.Get(5, 8);
			var year = bitsExtracter.Get(9, 14);
			var hour = bitsExtracter.Get(15, 19);
			var min = bitsExtracter.Get(20, 25);
			var sec = bitsExtracter.Get(26, 31);
			_journalItem.IntDate = (uint)sec + (uint)min * 60 + (uint)hour * 60 * 60 + (uint)day * 60 * 60 * 24 + (uint)month * 60 * 60 * 24 * 30 + (uint)year * 60 * 60 * 24 * 30 * 12;
			var eventName = MetadataHelper.GetEventByCode(_bytes[0]);
			_journalItem.EventName = ExactEventForFlag(eventName, _bytes[5]);
			_journalItem.Flag = _bytes[5];
			_journalItem.ShleifNo = _bytes[6] + 1;
			_journalItem.IntType = _bytes[7];
			_journalItem.FirstAddress = _bytes[17] + 1;
			_journalItem.Address = _bytes[8];
			_journalItem.State = _bytes[9];
			_journalItem.ZoneNo = _bytes[10] * 256 + _bytes[11];
			_journalItem.DescriptorNo = _bytes[12] * 256 * 256 + _bytes[13] * 256 + _bytes[14];
			if (_bytes[0] == 0x83)
				_journalItem.Description += "Выход: " + _journalItem.ShleifNo + "\n";
			if (_bytes[0] == 0x0F)
				_journalItem.Description += "АЛС: " + _journalItem.ShleifNo + "\n";
			// Системная неисправность
			if (_bytes[0] == 0x0D)
			{
				if (_journalItem.State == 0x20)
					_journalItem.Description += "база (сигнатура) повреждена или отсутствует\n";
			}
			if (_journalItem.ZoneNo != 0)
			{
				_journalItem.Description += "Зона: " + _journalItem.ZoneNo + "\n";
			}

			//Охранные события (сброс тревоги, постановка, снятие)

			#region

			if (_bytes[0] == 0x28)
			{
				switch (_allBytes[24])
				{
					case 0:
						{
							_journalItem.Description += "команда с компьютера\n";
							if (_allBytes[23] == 0)
								_journalItem.Description += "через USB\n";
							else
								_journalItem.Description += "через канал МС " + _allBytes[23] + "\n";
							break;
						}
					case 3:
						_journalItem.Description += "Прибор: Рубеж-БИ Адрес:" + _allBytes[23] + "\n";
						break;
					case 7:
						_journalItem.Description += "Прибор: Рубеж-ПДУ Адрес:" + _allBytes[23] + "\n";
						break;
					case 9:
						_journalItem.Description += "Прибор: Рубеж-ПДУ-ПТ Адрес:" + _allBytes[23] + "\n";
						break;
					case 100:
						_journalItem.Description += "Устройство: МС-3 Адрес:" + _allBytes[23] + "\n";
						break;
					case 101:
						_journalItem.Description += "Устройство: МС-4 Адрес:" + _allBytes[23] + "\n";
						break;
					case 102:
						_journalItem.Description += "Устройство: УОО-ТЛ Адрес:" + _allBytes[23] + "\n";
						break;
					default:
						_journalItem.Description += "Неизв. устр." + "(" + _allBytes[24] + ") Адрес:" + _allBytes[23] + "\n";
						break;
				}
				if (_journalItem.IntType == 0x00)
					_journalItem.IntType = 0x75;
			}

			#endregion

			// Потеря связи с мониторинговой станцией (БИ, ПДУ, УОО-ТЛ, МС-1, МС-2)

			#region

			if (_bytes[0] == 0x85)
			{
				switch (_bytes[7])
				{
					case 3:
						_journalItem.Description += "Прибор: Рубеж-БИ Адрес:" + _bytes[6] + "\n";
						break;
					case 7:
						_journalItem.Description += "Прибор: Рубеж-ПДУ Адрес:" + _bytes[6] + "\n";
						break;
					case 100:
						_journalItem.Description += "Устройство: МС-3 Адрес:" + _bytes[6] + "\n";
						break;
					case 101:
						_journalItem.Description += "Устройство: МС-4 Адрес:" + _bytes[6] + "\n";
						break;
					case 102:
						_journalItem.Description += "Устройство: УОО-ТЛ Адрес:" + _bytes[6] + "\n";
						break;
					default:
						_journalItem.Description += "Неизв. устр." + "(" + _bytes[7] + ") Адрес:" + _bytes[6] + "\n";
						break;
				}
				return _journalItem;
			}

			#endregion

			int tableType = 99999;
			if (_journalItem.IntType != 0)
			{
				var deviceUid = MetadataHelper.GetUidById((ushort)_journalItem.IntType).ToString().ToUpper();
				var device = "Неизвестное устройство";
				if (deviceUid != "00000000-0000-0000-0000-000000000000")
				{
					device = MetadataHelper.Metadata.deviceTables.FirstOrDefault(x => ((x.deviceDriverID != null) && (x.deviceDriverID.Equals(deviceUid)))).shortName;
					tableType = Convert.ToInt32(MetadataHelper.Metadata.deviceTables.FirstOrDefault(x => ((x.deviceDriverID != null) && (x.deviceDriverID.Equals(deviceUid)))).tableType);
				}
				if (_journalItem.IntType == 1)
					device = "АСПТ " + (_journalItem.ShleifNo - 1) + ".";
				_journalItem.Description += "Устройство: " + device + " " + _journalItem.FirstAddress + "." + _journalItem.Address + "\n";
			}
			// Детализация событий
			if (tableType != 99999)
			{
				var even = MetadataHelper.Metadata.events.FirstOrDefault(x => x.rawEventCode == "$" + _bytes[0].ToString("X2"));
				if (even.detailsFor != null)
				{
					var details = even.detailsFor.FirstOrDefault(x => x.tableType == tableType.ToString());
					if (details != null)
					{
						var dictionaryName =
							even.detailsFor.FirstOrDefault(x => x.tableType == tableType.ToString()).dictionary;
						var dictionary = MetadataHelper.Metadata.dictionary.FirstOrDefault(x => x.name == dictionaryName);
						var bitState = new BitArray(new int[] { _journalItem.State });
						foreach (var bit in dictionary.bit)
						{
							if (bitState.Get(Convert.ToInt32(bit.no)))
								_journalItem.Description += dictionary.bit.FirstOrDefault(x => x.no == bit.no).value + "\n";
						}
					}
				}
			}
			_journalItem.EventClass = SetEventClass(_bytes[0], _bytes[5]);
			return _journalItem;
		}

		public FSJournalItem FSParce()
		{
			FSJournalItem _fsjournalItem = new FSJournalItem();
			var timeBytes = _bytes.GetRange(1, 4);
			_fsjournalItem.DeviceTime = DateTime.Parse(TimeParceHelper.Parce(timeBytes));
			_fsjournalItem.SystemTime = DateTime.Now;
			var bitsExtracter = new BitsExtracter(timeBytes);
			var eventName = MetadataHelper.GetEventByCode(_bytes[0]);
			_fsjournalItem.Description = GetEvent(eventName, _bytes[5]);
			var Flag = _bytes[5];
			var ShleifNo = _bytes[6] + 1;
			_fsjournalItem.DeviceCategory = _bytes[7];
			var FirstAddress = _bytes[17] + 1;
			var Address = _bytes[8];
			var byteState = _bytes[9];
			_fsjournalItem.StateType = (StateType)byteState;
			var readenZoneNo = _bytes[10] * 256 + _bytes[11];

			var zone = ConfigurationManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == readenZoneNo);
			_fsjournalItem.ZoneName = zone.Name;
			var DescriptorNo = _bytes[12] * 256 * 256 + _bytes[13] * 256 + _bytes[14];
			if (_bytes[0] == 0x83)
				_fsjournalItem.Detalization += "Выход: " + ShleifNo + "\n";
			if (_bytes[0] == 0x0F)
				_fsjournalItem.Detalization += "АЛС: " + ShleifNo + "\n";

			// Системная неисправность
			if (_bytes[0] == 0x0D)
			{
				if (byteState == 0x20)
					_fsjournalItem.Detalization += "база (сигнатура) повреждена или отсутствует\n";
			}
			if (zone.No != 0)
			{
				_fsjournalItem.Detalization += "Зона: " + zone.No + "\n";
			}

			//Охранные события (сброс тревоги, постановка, снятие)

			#region

			if (_bytes[0] == 0x28)
			{
				switch (_allBytes[24])
				{
					case 0:
						{
							_fsjournalItem.Detalization += "команда с компьютера\n";
							if (_allBytes[23] == 0)
								_fsjournalItem.Detalization += "через USB\n";
							else
								_fsjournalItem.Detalization += "через канал МС " + _allBytes[23] + "\n";
							break;
						}
					case 3:
						_fsjournalItem.Detalization += "Прибор: Рубеж-БИ Адрес:" + _allBytes[23] + "\n";
						break;
					case 7:
						_fsjournalItem.Detalization += "Прибор: Рубеж-ПДУ Адрес:" + _allBytes[23] + "\n";
						break;
					case 9:
						_fsjournalItem.Detalization += "Прибор: Рубеж-ПДУ-ПТ Адрес:" + _allBytes[23] + "\n";
						break;
					case 100:
						_fsjournalItem.Detalization += "Устройство: МС-3 Адрес:" + _allBytes[23] + "\n";
						break;
					case 101:
						_fsjournalItem.Detalization += "Устройство: МС-4 Адрес:" + _allBytes[23] + "\n";
						break;
					case 102:
						_fsjournalItem.Detalization += "Устройство: УОО-ТЛ Адрес:" + _allBytes[23] + "\n";
						break;
					default:
						_fsjournalItem.Detalization += "Неизв. устр." + "(" + _allBytes[24] + ") Адрес:" + _allBytes[23] + "\n";
						break;
				}
				if (_fsjournalItem.DeviceCategory == 0x00)
					_fsjournalItem.DeviceCategory = 0x75;
			}

			#endregion

			// Потеря связи с мониторинговой станцией (БИ, ПДУ, УОО-ТЛ, МС-1, МС-2)

			#region

			if (_bytes[0] == 0x85)
			{
				switch (_bytes[7])
				{
					case 3:
						_fsjournalItem.Detalization += "Прибор: Рубеж-БИ Адрес:" + _bytes[6] + "\n";
						break;
					case 7:
						_fsjournalItem.Detalization += "Прибор: Рубеж-ПДУ Адрес:" + _bytes[6] + "\n";
						break;
					case 100:
						_fsjournalItem.Detalization += "Устройство: МС-3 Адрес:" + _bytes[6] + "\n";
						break;
					case 101:
						_fsjournalItem.Detalization += "Устройство: МС-4 Адрес:" + _bytes[6] + "\n";
						break;
					case 102:
						_fsjournalItem.Description += "Устройство: УОО-ТЛ Адрес:" + _bytes[6] + "\n";
						break;
					default:
						_fsjournalItem.Detalization += "Неизв. устр." + "(" + _bytes[7] + ") Адрес:" + _bytes[6] + "\n";
						break;
				}
				return _fsjournalItem;
			}

			#endregion

			int tableType = 99999;
			if (_fsjournalItem.DeviceCategory != 0)
			{
				_fsjournalItem.DeviceUID = MetadataHelper.GetUidById((ushort)_fsjournalItem.DeviceCategory);
				var deviceUid = _fsjournalItem.DeviceUID.ToString().ToUpper();
				var device = "Неизвестное устройство";
				if (deviceUid != "00000000-0000-0000-0000-000000000000")
				{
					device = MetadataHelper.Metadata.deviceTables.FirstOrDefault(x => ((x.deviceDriverID != null) && (x.deviceDriverID.Equals(deviceUid)))).shortName;
					_fsjournalItem.DeviceName = device;
					tableType = Convert.ToInt32(MetadataHelper.Metadata.deviceTables.FirstOrDefault(x => ((x.deviceDriverID != null) && (x.deviceDriverID.Equals(deviceUid)))).tableType);
				}
				if (_fsjournalItem.DeviceCategory == 1)
					device = "АСПТ " + (ShleifNo - 1) + ".";
				_fsjournalItem.Detalization += "Устройство: " + device + " " + FirstAddress + "." + Address + "\n";
			}
			// Детализация событий
			if (tableType != 99999)
			{
				var even = MetadataHelper.Metadata.events.FirstOrDefault(x => x.rawEventCode == "$" + _bytes[0].ToString("X2"));
				if (even.detailsFor != null)
				{
					var details = even.detailsFor.FirstOrDefault(x => x.tableType == tableType.ToString());
					if (details != null)
					{
						var dictionaryName =
							even.detailsFor.FirstOrDefault(x => x.tableType == tableType.ToString()).dictionary;
						var dictionary = MetadataHelper.Metadata.dictionary.FirstOrDefault(x => x.name == dictionaryName);
						var bitState = new BitArray(new int[] { byteState });
						foreach (var bit in dictionary.bit)
						{
							if (bitState.Get(Convert.ToInt32(bit.no)))
								_fsjournalItem.Detalization += dictionary.bit.FirstOrDefault(x => x.no == bit.no).value + "\n";
						}
					}
				}
			}

			_fsjournalItem.PanelName = _fsjournalItem.DeviceName;
			_fsjournalItem.PanelUID = _fsjournalItem.DeviceUID;
			_fsjournalItem.UserName = "Usr";
			return _fsjournalItem;
		}

		private string GetEvent(string eventName, int flag)
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
					return firstPart + choise + thirdPart;
				}
			}
			return eventName;
		}

		private string ExactEventForFlag(string eventName, int flag)
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
					return firstPart + choise + thirdPart; ;
				}
			}
			return eventName;
		}

		private int SetEventClass(int eventCode, int flag)
		{
			int eventClass = -1;
			var stringEventClass = GetEventClass(eventCode, flag);
			if (!string.IsNullOrEmpty(stringEventClass))
			{
				try
				{
					eventClass = int.Parse(stringEventClass);
				}
				catch { }
			}
			return eventClass;
		}

		private string GetEventClass(int eventCode, int flag)
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