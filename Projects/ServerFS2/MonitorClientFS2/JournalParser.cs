using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using ServerFS2;
using ServerFS2.ConfigurationWriter;
using ServerFS2.DataBase;
using ServerFS2.Helpers;

namespace MonitorClientFS2
{
	public class JournalParser
	{
		public static FSJournalItem FSParce(List<byte> allBytes)
		{
			if (!IsValidInput(allBytes))
				return null;

			List<byte> bytes = new List<byte>(allBytes);
			bytes.RemoveRange(0, 7);
			var fsJournalItem = new FSJournalItem();

			var timeBytes = bytes.GetRange(1, 4);
			fsJournalItem.DeviceTime = ParceDateTime(timeBytes);
			fsJournalItem.SystemTime = DateTime.Now;
			fsJournalItem.Description = GetEventName(bytes, fsJournalItem);

			var shleifNo = bytes[6] + 1;
			if (bytes[0] == 0x83)
				fsJournalItem.Detalization += "Выход: " + shleifNo + "\n";
			if (bytes[0] == 0x0F)
				fsJournalItem.Detalization += "АЛС: " + shleifNo + "\n";

			fsJournalItem.DeviceCategory = bytes[7];

			var panelAddress = allBytes[5];
			fsJournalItem.PanelAddress = panelAddress;
			var deviceAddress = allBytes[15];
			fsJournalItem.DeviceAddress = deviceAddress;

			fsJournalItem.PanelDevice = ConfigurationManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.IntAddress == panelAddress && x.Driver.IsPanel);
			if (fsJournalItem.PanelDevice != null)
			{
				fsJournalItem.PanelUID = fsJournalItem.PanelDevice.UID;
				fsJournalItem.PanelName = fsJournalItem.PanelDevice.Driver.Name;

				fsJournalItem.Device = ConfigurationManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.IntAddress == deviceAddress + 256 * shleifNo && x.ParentPanel == fsJournalItem.PanelDevice);
				if (fsJournalItem.Device != null)
				{
					fsJournalItem.DeviceUID = fsJournalItem.Device.UID;
					fsJournalItem.DeviceName = fsJournalItem.Device.Driver.Name;
				}

				fsJournalItem.SubsystemType = GetSubsystemType(fsJournalItem.PanelDevice);
			}

			var byteState = bytes[9];
			if (byteState <= 8)
				fsJournalItem.StateType = (StateType)byteState;
			else
				fsJournalItem.StateType = StateType.No;

			fsJournalItem.StateWord = byteState;

			// Системная неисправность
			if (bytes[0] == 0x0D && byteState == 0x20)
			{
				fsJournalItem.Detalization += "база (сигнатура) повреждена или отсутствует\n";
			}

			SetZone(bytes, fsJournalItem);
			SetGuardEvents(allBytes, fsJournalItem);

			// Потеря связи с мониторинговой станцией (БИ, ПДУ, УОО-ТЛ, МС-1, МС-2)
			fsJournalItem.Detalization += LostConnection(bytes);

			//_fsjournalItem.DeviceUID = MetadataHelper.GetUidById((ushort)_fsjournalItem.DeviceCategory);
			//var deviceUid = fsJournalItem.DeviceUID.ToString().ToUpper();
			//var _device = MetadataHelper.Metadata.deviceTables.FirstOrDefault(x => ((x.deviceDriverID != null) && (x.deviceDriverID.Equals(deviceUid))));

			//if (_device != null)
			//{
			//    if (_fsjournalItem.DeviceCategory == 1)
			//        _fsjournalItem.DeviceName = "АСПТ " + (ShleifNo - 1) + ".";
			//    else
			//        _fsjournalItem.DeviceName = _device.shortName;
			//}
			//else
			//    _fsjournalItem.DeviceName = "Неизвестное устройство";

			fsJournalItem.Detalization += GetEventDetalization(bytes, fsJournalItem);
			fsJournalItem.UserName = "Usr";
			return fsJournalItem;
		}

		public static bool IsValidInput(List<byte> bytes)
		{
			return bytes.Count > 20;
		}

		private static string GetEventDetalization(List<byte> bytes, FSJournalItem fsJournalItem)
		{
			try
			{
				int tableType = 99999;
				var firstAddress = bytes[17] + 1;
				var address = bytes[8];
				string detalization = String.Empty;
				if (fsJournalItem.DeviceUID != Guid.Empty && fsJournalItem.DeviceCategory != 0)
				{
					tableType = Convert.ToInt32(MetadataHelper.Metadata.deviceTables.FirstOrDefault(x => ((x.deviceDriverID != null) && (x.deviceDriverID.Equals(fsJournalItem.DeviceUID.ToString().ToUpper())))).tableType);
				}
				detalization += "Устройство: " + fsJournalItem.DeviceName + " " + firstAddress + "." + address + "\n";
				var metadataEvent = MetadataHelper.Metadata.events.FirstOrDefault(x => x.rawEventCode == "$" + bytes[0].ToString("X2"));
				if (metadataEvent.detailsFor != null && tableType != 99999)
				{
					var dictionaryName = metadataEvent.detailsFor.FirstOrDefault(x => x.tableType == tableType.ToString()).dictionary;
					var dictionary = MetadataHelper.Metadata.dictionary.FirstOrDefault(x => x.name == dictionaryName);
					var bitState = new BitArray(new int[] { bytes[9] });
					foreach (var bit in dictionary.bit)
						if (bitState.Get(Convert.ToInt32(bit.no)))
							detalization = dictionary.bit.FirstOrDefault(x => x.no == bit.no).value + "\n";
				}
				return detalization;
			}
			catch
			{
				return "Детализация не прочитана";
			}
		}

		private static SubsystemType GetSubsystemType(Device panelDevice)
		{
			switch(panelDevice.Driver.DriverType)
			{
				case DriverType.Rubezh_2OP:
				case DriverType.USB_Rubezh_2OP:
					return SubsystemType.Guard;

				case DriverType.Rubezh_10AM:
				case DriverType.Rubezh_2AM:
				case DriverType.Rubezh_4A:
				case DriverType.USB_Rubezh_2AM:
				case DriverType.USB_Rubezh_4A:
					return SubsystemType.Fire;

				default:
					return SubsystemType.Other;
			}
		}

		// Потеря связи с мониторинговой станцией (БИ, ПДУ, УОО-ТЛ, МС-1, МС-2)
		private static string LostConnection(List<byte> bytes)
		{
			string detalization = "";
			if (bytes[0] == 0x85)
			{
				switch (bytes[7])
				{
					case 3:
						detalization += "Прибор: Рубеж-БИ Адрес:" + bytes[6] + "\n";
						break;
					case 7:
						detalization += "Прибор: Рубеж-ПДУ Адрес:" + bytes[6] + "\n";
						break;
					case 100:
						detalization += "Устройство: МС-3 Адрес:" + bytes[6] + "\n";
						break;
					case 101:
						detalization += "Устройство: МС-4 Адрес:" + bytes[6] + "\n";
						break;
					case 102:
						detalization += "Устройство: УОО-ТЛ Адрес:" + bytes[6] + "\n";
						break;
					default:
						detalization += "Неизв. устр." + "(" + bytes[7] + ") Адрес:" + bytes[6] + "\n";
						break;
				}
			}
			return detalization;
		}

		private static void SetGuardEvents(List<byte> allBytes, FSJournalItem fsJournalItem)
		{
			if (allBytes[7] == 0x28)
			{
				switch (allBytes[24])
				{
					case 0:
						{
							fsJournalItem.Detalization += "команда с компьютера\n";
							if (allBytes[23] == 0)
								fsJournalItem.Detalization += "через USB\n";
							else
								fsJournalItem.Detalization += "через канал МС " + allBytes[23] + "\n";
							break;
						}
					case 3:
						fsJournalItem.Detalization += "Прибор: Рубеж-БИ Адрес:" + allBytes[23] + "\n";
						break;
					case 7:
						fsJournalItem.Detalization += "Прибор: Рубеж-ПДУ Адрес:" + allBytes[23] + "\n";
						break;
					case 9:
						fsJournalItem.Detalization += "Прибор: Рубеж-ПДУ-ПТ Адрес:" + allBytes[23] + "\n";
						break;
					case 100:
						fsJournalItem.Detalization += "Устройство: МС-3 Адрес:" + allBytes[23] + "\n";
						break;
					case 101:
						fsJournalItem.Detalization += "Устройство: МС-4 Адрес:" + allBytes[23] + "\n";
						break;
					case 102:
						fsJournalItem.Detalization += "Устройство: УОО-ТЛ Адрес:" + allBytes[23] + "\n";
						break;
					default:
						fsJournalItem.Detalization += "Неизв. устр." + "(" + allBytes[24] + ") Адрес:" + allBytes[23] + "\n";
						break;
				}
				if (fsJournalItem.DeviceCategory == 0x00)
					fsJournalItem.DeviceCategory = 0x75;
			}
		}

		private static void SetZone(List<byte> bytes, FSJournalItem fsJournalItem)
		{
			var zone = ConfigurationManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == BytesHelper.ExtractShort(bytes, 10));
			if (zone != null)
			{
				fsJournalItem.ZoneName = zone.Name;
				fsJournalItem.Detalization += "Зона: " + zone.No + "\n";
			}
		}

		private static string GetEventName(List<byte> bytes, FSJournalItem fsJournalItem)
		{
			int eventChoiceNo = bytes[5];
			fsJournalItem.EventChoiceNo = eventChoiceNo;
			var eventName = MetadataHelper.GetEventByCode(bytes[0]);
			var firstIndex = eventName.IndexOf("[");
			var lastIndex = eventName.IndexOf("]");
			if (firstIndex != -1 && lastIndex != -1)
			{
				var firstPart = eventName.Substring(0, firstIndex);
				var secondPart = eventName.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
				var thirdPart = eventName.Substring(lastIndex + 1);
				var secondParts = secondPart.Split('/');
				if (eventChoiceNo < secondParts.Count())
				{
					var choise = secondParts[eventChoiceNo];
					return firstPart + choise + thirdPart;
				}
			}
			return eventName;
		}

		private static DateTime ParceDateTime(List<byte> bytes)
		{
			var bitsExtracter = new BitsExtracter(bytes);
			var day = bitsExtracter.Get(0, 4);
			var month = bitsExtracter.Get(5, 8);
			var year = bitsExtracter.Get(9, 14);
			var hour = bitsExtracter.Get(15, 19);
			var min = bitsExtracter.Get(20, 25);
			var sec = bitsExtracter.Get(26, 31);
			var resultString = day.ToString() + "/" + month.ToString() + "/" + (year + 2000).ToString() + " " + hour.ToString() + ":" + min.ToString() + ":" + sec.ToString();
			DateTime result;
			if (DateTime.TryParse(resultString, out result))
				return result;
			else
				return DateTime.Now;
		}
	}
}