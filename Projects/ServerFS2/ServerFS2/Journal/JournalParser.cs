using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FS2Api;
using Rubezh2010;

namespace ServerFS2
{
	public class JournalParser
	{
		DeviceConfiguration DeviceConfiguration { get; set; }
		FSInternalJournal FSInternalJournal { get; set; }
		FS2JournalItem FS2JournalItem { get; set; }
		List<byte> Bytes { get; set; }
		driverConfigEventsEvent MetadataEvent { get; set; }

		public FS2JournalItem Parce(DeviceConfiguration deviceConfiguration, Device panelDevice, List<byte> bytes, int journalType)
		{
			if (bytes.Count != 32)
				return null;
			Bytes = bytes;
			DeviceConfiguration = deviceConfiguration;

			FSInternalJournal = new FSInternalJournal()
			{
				ShleifNo = bytes[17] + 1,
				EventCode = bytes[0],
				AdditionalEventCode = bytes[5],
				DeviceType = bytes[7],
				AddressOnShleif = bytes[8],
				State = bytes[9],
				UnusedDescriptorNo = BytesHelper.ExtractTriple(bytes, 12),
				ZoneNo = BytesHelper.ExtractShort(bytes, 10)
			};

			FS2JournalItem = new FS2JournalItem()
			{
				BytesString = BytesHelper.BytesToString(bytes),
				PanelDevice = panelDevice,
				PanelUID = panelDevice.UID,
				PanelName = panelDevice.DottedPresentationNameAndAddress,
				DeviceTime = TimeParceHelper.ParceDateTime(bytes.GetRange(1, 4)),
				SystemTime = DateTime.Now,
				EventCode = FSInternalJournal.EventCode,
				AdditionalEventCode = FSInternalJournal.AdditionalEventCode
			};
			switch (journalType)
			{
				case 0x00:
					FS2JournalItem.SubsystemType = SubsystemType.Fire;
					break;
				case 0x02:
					FS2JournalItem.SubsystemType = SubsystemType.Guard;
					break;
			}

			MetadataEvent = MetadataHelper.Metadata.events.FirstOrDefault(x => x.rawEventCode == "$" + FSInternalJournal.EventCode.ToString("X2"));

			if (MetadataHelper.HasDevise(FSInternalJournal.EventCode))
			{
				var intAddress = FSInternalJournal.AddressOnShleif + 256 * FSInternalJournal.ShleifNo;
				FS2JournalItem.Device = ConfigurationManager.Devices.FirstOrDefault(x => x.IntAddress == intAddress && x.ParentPanel == FS2JournalItem.PanelDevice && !x.Driver.IsGroupDevice);
				if (FS2JournalItem.Device != null)
				{
					FS2JournalItem.DeviceUID = FS2JournalItem.Device.UID;
					FS2JournalItem.DeviceName = FS2JournalItem.Device.DottedPresentationNameAndAddress;

					if (FSInternalJournal.DeviceType == 1)
						FS2JournalItem.DeviceName = "АСПТ " + (FSInternalJournal.ShleifNo - 1) + ".";
				}
				else
				{
					if (FSInternalJournal.DeviceType > 0)
					{
						var driverUID = MetadataHelper.GetUidById(FSInternalJournal.DeviceType);
						if (driverUID != Guid.Empty)
						{
							var driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.UID == driverUID);
							FS2JournalItem.DeviceName = driver.ShortName + " " + FSInternalJournal.ShleifNo + "." + FSInternalJournal.AddressOnShleif;
						}
					}
				}
			}

			InitializeZone();
			FS2JournalItem.StateType = GetEventStateType();
			FS2JournalItem.Description = GetEventName();
			FS2JournalItem.Detalization = GetDetalization();
			FS2JournalItem.UserName = GetUserName();

			//Initialize_0x80_Event();

			return FS2JournalItem;
		}

		void Initialize_0x80_Event()
		{
			if (FSInternalJournal.EventCode == 0x80 && Bytes[6] != 0)
			{
				switch (FSInternalJournal.AdditionalEventCode)
				{
					case 1:
						FS2JournalItem.Description = "Авария ввода";
						FS2JournalItem.Detalization = "Номер ввода: 1";
						break;

					case 2:
						FS2JournalItem.Description = "Авария ввода 2";
						FS2JournalItem.Detalization = "Номер ввода: 2";
						break;
				}
			}
		}

		string GetEventName()
		{
			if (FS2JournalItem.Device != null && FS2JournalItem.Device.Driver.DriverType == DriverType.AM1_T && FSInternalJournal.EventCode == 58)
				return GetEventNameForAM1_T();

			var stringTableType = MetadataHelper.GetDeviceTableNo(FS2JournalItem.Device);
			var eventName = MetadataHelper.GetEventMessage(FSInternalJournal.EventCode, stringTableType);
			var firstIndex = eventName.IndexOf("[");
			var lastIndex = eventName.IndexOf("]");
			if (firstIndex != -1 && lastIndex != -1)
			{
				var firstPart = eventName.Substring(0, firstIndex);
				var secondPart = eventName.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
				var thirdPart = eventName.Substring(lastIndex + 1);
				var secondParts = secondPart.Split('/');
				if (FSInternalJournal.AdditionalEventCode < secondParts.Count())
				{
					var choise = secondParts[FSInternalJournal.AdditionalEventCode];
					return firstPart + choise + thirdPart;
				}
			}
			return eventName;
		}

		string GetEventNameForAM1_T()
		{
			switch (FSInternalJournal.AdditionalEventCode)
			{
				case 0:
					var property = FS2JournalItem.Device.Properties.FirstOrDefault(x => x.Name == "Event1");
					if (property != null)
						return property.Value;
					break;

				case 1:
					property = FS2JournalItem.Device.Properties.FirstOrDefault(x => x.Name == "Event2");
					if (property != null)
						return property.Value;
					break;
			}
			return "";
		}

		string GetDetalization()
		{
			var result = "";
			result += GetDetalizationForAM1_O();
			result += GetDetalizationForConnectionLost();
			result += GetDetalizationForDevice();
			result += GetDetalizationForEvent();

			if (FSInternalJournal.EventCode == 0x0D && FSInternalJournal.State == 0x20)
			{
				result += "база (сигнатура) повреждена или отсутствует\n";
			}

			if (!string.IsNullOrEmpty(FS2JournalItem.DeviceName))
			{
				result += "Устройство: " + FS2JournalItem.DeviceName + "\n";
			}
			if (!string.IsNullOrEmpty(FS2JournalItem.ZoneName))
			{
				result += "Зона: " + FS2JournalItem.ZoneName + "\n";
			}

			if (result.EndsWith("\n"))
				result = result.Remove(result.Length - 1, 1);
			return result;
		}

		string GetDetalizationForDevice()
		{
			string result = "";
			try
			{
				if (FS2JournalItem.DeviceUID != Guid.Empty && FSInternalJournal.DeviceType != 0)
				{
					var stringTableType = MetadataHelper.GetDeviceTableNo(FS2JournalItem.Device);
					if (stringTableType != null && MetadataEvent != null && MetadataEvent.detailsFor != null)
					{
						var metadataDetailsFor = MetadataEvent.detailsFor.FirstOrDefault(x => x.tableType == stringTableType);
						if (metadataDetailsFor != null)
						{
							var metadataDictionary = MetadataHelper.Metadata.dictionary.FirstOrDefault(x => x.name == metadataDetailsFor.dictionary);
							var stateBitArray = new BitArray(new int[] { FSInternalJournal.State });
							var stateValue = "0x" + FSInternalJournal.State.ToString("X2");
							foreach (var bit in metadataDictionary.bit)
							{
								if (bit.val == stateValue && bit.no == null)
								{
									result += bit.value + "\n";
									break;
								}
								if (bit.no != null && stateBitArray.Get(Convert.ToInt32(bit.no)))
								{
									result += metadataDictionary.bit.FirstOrDefault(x => x.no == bit.no).value + "\n";
								}
							}
						}
					}
				}
			}
			catch
			{
				Logger.Error("JournalParser.GetEventDetalization");
			}
			return result;
		}

		string GetDetalizationForAM1_O()
		{
			if (FSInternalJournal.EventCode != 0x83)
			{
				if (FSInternalJournal.DeviceType == 0x34)
				{
					switch (Bytes[22])
					{
						case 0:
							return "Тип датчика: Без типа\n";
						case 1:
							return "Тип датчика: Стекло\n";
						case 2:
							return "Тип датчика: Дверь\n";
						case 3:
							return "Тип датчика: Объем\n";
						case 4:
							return "Тип датчика: Тревожная кнопка\n";
					}
				}
			}
			return "";
		}

		string GetDetalizationForEvent()
		{
			switch (FSInternalJournal.EventCode)
			{
				case 0x31:
				case 0x45:
					if (Bytes[14] == 0)
						return "команда с компьютера\n";
					break;

				case 0x28:
					if (FS2JournalItem.DeviceCategory == 0x00)
						FS2JournalItem.DeviceCategory = 0x75;

					switch (Bytes[17])
					{
						case 0:
							{
								var result = "команда с компьютера\n";
								if (Bytes[16] == 0)
									result += "через USB\n";
								else
									result += "через канал МС " + Bytes[16] + "\n";
								return result;
							}
						case 3:
							return "Прибор: Рубеж-БИ Адрес:" + Bytes[16] + "\n";
						case 7:
							return "Прибор: Рубеж-ПДУ Адрес:" + Bytes[16] + "\n";
						case 9:
							return "Прибор: Рубеж-ПДУ-ПТ Адрес:" + Bytes[16] + "\n";
						case 100:
							return "Устройство: МС-3 Адрес:" + Bytes[16] + "\n";
						case 101:
							return "Устройство: МС-4 Адрес:" + Bytes[16] + "\n";
						case 102:
							return "Устройство: УОО-ТЛ Адрес:" + Bytes[16] + "\n";
					}
					break;
			}
			return "";
		}

		string GetDetalizationForConnectionLost()
		{
			if (FSInternalJournal.EventCode == 0x85)
			{
				switch (FSInternalJournal.DeviceType)
				{
					case 3:
						return "Прибор: Рубеж-БИ Адрес:" + (FSInternalJournal.ShleifNo - 1).ToString() + "\n";
					case 7:
						return "Прибор: Рубеж-ПДУ Адрес:" + (FSInternalJournal.ShleifNo - 1).ToString() + "\n";
					case 100:
						return "Устройство: МС-3 Адрес:" + (FSInternalJournal.ShleifNo - 1).ToString() + "\n";
					case 101:
						return "Устройство: МС-4 Адрес:" + (FSInternalJournal.ShleifNo - 1).ToString() + "\n";
					case 102:
						return "Устройство: УОО-ТЛ Адрес:" + (FSInternalJournal.ShleifNo - 1).ToString() + "\n";
				}
			}
			return "";
		}

		void InitializeZone()
		{
			if (DeviceConfiguration != null)
			{
				var localzone = DeviceConfiguration.Zones.FirstOrDefault(x => x.LocalDeviceNo == FSInternalJournal.ZoneNo);
				if (localzone != null)
				{
					var zone = ConfigurationManager.Zones.FirstOrDefault(x => x.No == localzone.No);
					if (zone != null)
					{
						FS2JournalItem.Zone = zone;
						FS2JournalItem.ZoneUID = zone.UID;
						FS2JournalItem.ZoneNo = zone.No;
						FS2JournalItem.ZoneName = zone.No + "." + zone.Name;
						return;
					}
				}
			}
			else
			{
				if (FSInternalJournal.ZoneNo > 0)
					FS2JournalItem.ZoneName = FSInternalJournal.ZoneNo.ToString();
			}
		}

		StateType GetEventStateType()
		{
			try
			{
				var stringStateType = MetadataHelper.GetEventStateClassString(FSInternalJournal.EventCode, FSInternalJournal.AdditionalEventCode);
				if (stringStateType != null)
				{
					var intStateType = Int32.Parse(stringStateType);
					return (StateType)intStateType;
				}
				else
				{
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "JournalParser.GetEventStateType");
			}
			return StateType.Norm;
		}

		string GetUserName()
		{
			switch (FSInternalJournal.EventCode)
			{
				case (0x35):
				case (0x32):
					switch (Bytes[18])
					{
						case (0x6f):
							return "Дежурный";
						case (0x2a):
							return "Инсталлятор";
						case (0x4c):
							return "Администратор";
					}
					break;
				case (0x3b):
				case (0x3f):
				case (0x34):
				case (0x04):
					switch (Bytes[18])
					{
						case (0xef):
							return "Дежурный";
						case (0xaa):
							return "Инсталлятор";
						case (0xcc):
							return "Администратор";
						case (0x4d):
							return "Кнопка управления СПТ";
						case (0x4e):
							return "Кнопка ПУСК/СТОП";
						case (0x01):
							return "ЭДУ-ПТ1";
						case (0x02):
							return "ЭДУ-ПТ2";
						case (0x04):
							return "ЭДУ-ПТ3";
						case (0x05):
							return "Кнопка ДУ";
						case (0x06):
							return "Панель шкафа";
						case (0x08):
							return "ЭДУ-ПТ4";
					}
					break;
				case (0x45):
				case (0x31):
				case (0x46):
					FS2JournalItem.GuardUser = ConfigurationManager.DeviceConfiguration.GuardUsers.FirstOrDefault(x => x.Id == Bytes[21]);
					if (FS2JournalItem.GuardUser != null)
					{
						return FS2JournalItem.GuardUser.Name;
					}
					break;
			}
			return null;
		}
	}
}