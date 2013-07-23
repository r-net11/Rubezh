using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FS2Api;
using ServerFS2.Monitoring;
using Rubezh2010;
using System.Diagnostics;

namespace ServerFS2
{
	public class JournalParser
	{
		DeviceConfiguration DeviceConfiguration;
		public FSInternalJournal FSInternalJournal { get; private set; }
		public FS2JournalItem FS2JournalItem { get; private set; }
        driverConfigEventsEvent MetadataEvent; 
		public List<byte> Bytes { get; private set; }

		public FS2JournalItem Parce(DeviceConfiguration deviceConfiguration, Device panelDevice, List<byte> bytes)
		{
			if (bytes.Count != 32)
				return null;
			Bytes = bytes;
            DeviceConfiguration = deviceConfiguration;
			FSInternalJournal = new FSInternalJournal();
			FS2JournalItem = new FS2JournalItem();
			FS2JournalItem.BytesString = BytesHelper.BytesToString(bytes);
			FS2JournalItem.PanelDevice = panelDevice;

			FSInternalJournal.ShleifNo = bytes[17] + 1;
			FSInternalJournal.EventCode = bytes[0];
			FSInternalJournal.AdditionalEventCode = bytes[5];

			FSInternalJournal.DeviceType = bytes[7];
			FSInternalJournal.AddressOnShleif = bytes[8];
			FSInternalJournal.State = bytes[9];

			FSInternalJournal.UnusedDescriptorNo = BytesHelper.ExtractTriple(bytes, 12);

			var timeBytes = bytes.GetRange(1, 4);
			FS2JournalItem.DeviceTime = TimeParceHelper.ParceDateTime(timeBytes);
			FS2JournalItem.SystemTime = DateTime.Now;
			FS2JournalItem.EventCode = FSInternalJournal.EventCode;
			FS2JournalItem.EventChoiceNo = FSInternalJournal.AdditionalEventCode;

			FS2JournalItem.PanelUID = FS2JournalItem.PanelDevice.UID;
			FS2JournalItem.PanelName = FS2JournalItem.PanelDevice.DottedPresentationNameAndAddress;

            MetadataEvent = MetadataHelper.Metadata.events.FirstOrDefault(x => x.rawEventCode == "$" + FSInternalJournal.EventCode.ToString("X2"));

			if (MetadataHelper.HasDevise(FSInternalJournal.EventCode))
			{
				var intAddress = FSInternalJournal.AddressOnShleif + 256 * FSInternalJournal.ShleifNo;
				FS2JournalItem.DeviceAddress = FSInternalJournal.AddressOnShleif;
				FS2JournalItem.Device = ConfigurationManager.Devices.FirstOrDefault(x => x.IntAddress == intAddress && x.ParentPanel == FS2JournalItem.PanelDevice && !x.Driver.IsGroupDevice);
				if (FS2JournalItem.Device != null)
				{
					FS2JournalItem.DeviceUID = FS2JournalItem.Device.UID;
					FS2JournalItem.DeviceName = FS2JournalItem.Device.DottedPresentationNameAndAddress;
				}
			}

			if (MetadataHelper.HasZone(FSInternalJournal.EventCode))
			{
				FSInternalJournal.ZoneNo = BytesHelper.ExtractShort(bytes, 10);
				InitializeZone();
			}

			FS2JournalItem.StateType = GetEventStateType();
			FS2JournalItem.Description = GetEventName();
			FS2JournalItem.SubsystemType = GetSubsystemType(FS2JournalItem.PanelDevice);

			if (FSInternalJournal.DeviceType == 1)
				FS2JournalItem.DeviceName = "АСПТ " + (FSInternalJournal.ShleifNo - 1) + ".";

			InitializeDetalization();
			InitializeGuardEvents();

			FS2JournalItem.EventClass = GetIntEventClass();

            GetUserName();
            return FS2JournalItem;
		}

        void GetUserName()
        {
            switch (FSInternalJournal.EventCode)
            {
                case (0x35):
                case (0x32):
                    switch (Bytes[18])
                    {
                        case (0x6f):
                            FS2JournalItem.UserName = "Дежурный";
                            break;
                        case (0x2a):
                            FS2JournalItem.UserName = "Инсталлятор";
                            break;
                        case (0x4c):
                            FS2JournalItem.UserName = "Администратор";
                            break;
                        default:
                            break;
                    }
                    break;
                case (0x3b):
                case (0x3f):
                case (0x34):
                case (0x04):
                    switch (Bytes[18])
                    {
                        case (0xef):
                            FS2JournalItem.UserName = "Дежурный";
                            break;
                        case (0xaa):
                            FS2JournalItem.UserName = "Инсталлятор";
                            break;
                        case (0xcc):
                            FS2JournalItem.UserName = "Администратор";
                            break;
                        case (0x4d):
                            FS2JournalItem.UserName = "Кнопка управления СПТ";
                            break;
                        case (0x4e):
                            FS2JournalItem.UserName = "Кнопка ПУСК/СТОП";
                            break;
                        case (0x01):
                            FS2JournalItem.UserName = "ЭДУ-ПТ1";
                            break;
                        case (0x02):
                            FS2JournalItem.UserName = "ЭДУ-ПТ2";
                            break;
                        case (0x04):
                            FS2JournalItem.UserName = "ЭДУ-ПТ3";
                            break;
                        case (0x05):
                            FS2JournalItem.UserName = "Кнопка ДУ";
                            break;
                        case (0x06):
                            FS2JournalItem.UserName = "Панель шкафа";
                            break;
                        case (0x08):
                            FS2JournalItem.UserName = "ЭДУ-ПТ4";
                            break;
                        default:
                            break;
                    }
                    break;
                case (0x45):
                case (0x31):
                case (0x46):
                    var guardUser = ConfigurationManager.DeviceConfiguration.GuardUsers.FirstOrDefault(x => x.Id == Bytes[21]);
                    FS2JournalItem.GuardUser = guardUser;
                    FS2JournalItem.UserName = guardUser.Name;
                    break;
                default:
                    break;
            }
        }

		void InitializeDetalization()
		{
			FS2JournalItem.Detalization = "";
			//if (FSInternalJournal.ShleifNo == 0x83)
			//    FS2JournalItem.Detalization += "Выход: " + FSInternalJournal.ShleifNo + "\n";
			//if (FSInternalJournal.ShleifNo == 0x0F)
			//    FS2JournalItem.Detalization += "АЛС: " + FSInternalJournal.ShleifNo + "\n";

			if (FSInternalJournal.EventCode == 0x0D && FS2JournalItem.StateByte == 0x20)
			{
				FS2JournalItem.Detalization += "база (сигнатура) повреждена или отсутствует\n";
			}

			if (FS2JournalItem.Device != null)
			{
				FS2JournalItem.Detalization += "Устройство: " + FS2JournalItem.Device.DottedPresentationNameAndAddress + "\n";
			}
			if (FS2JournalItem.Zone != null)
			{
				FS2JournalItem.Detalization += "Зона: " + FS2JournalItem.Zone.No + "\n";
			}

			FS2JournalItem.Detalization += GetDetalizationForConnectionLost();
			FS2JournalItem.Detalization += GetEventDetalization();
		}

		string GetEventDetalization()
		{
			try
			{
				string result = "";
				if (FS2JournalItem.DeviceUID != Guid.Empty && FSInternalJournal.DeviceType != 0)
				{
					var stringTableType = MetadataHelper.GetDeviceTableNo(FS2JournalItem.Device);
					if (stringTableType != null)
					{
						if (MetadataEvent.detailsFor != null)
						{
							var metadataDetailsFor = MetadataEvent.detailsFor.FirstOrDefault(x => x.tableType == stringTableType);
							if (metadataDetailsFor != null)
							{
								var metadataDictionary = MetadataHelper.Metadata.dictionary.FirstOrDefault(x => x.name == metadataDetailsFor.dictionary);
								var bitState = new BitArray(new int[] { FSInternalJournal.State });
								foreach (var bit in metadataDictionary.bit)
								{
									string stateVal = "0x" + FSInternalJournal.State.ToString("X2");
									if (bit.val == stateVal && bit.no == null)
									{
										result += bit.value + "\n";
										break;
									}
									if (bit.no != null && bitState.Get(Convert.ToInt32(bit.no)))
									{
										result += metadataDictionary.bit.FirstOrDefault(x => x.no == bit.no).value + "\n";
									}
								}
							}
						}
					}
				}
				return result;
			}
			catch
			{
				return "Детализация не прочитана";
			}
		}

		static SubsystemType GetSubsystemType(Device panelDevice)
		{
			switch (panelDevice.Driver.DriverType)
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
					//default:
					//    return "Неизв. устр." + "(" + FSInternalJournal.DeviceType.ToString() + ") Адрес:" + (FSInternalJournal.ShleifNo - 1).ToString() + "\n";
				}
			}
			return "";
		}

		void InitializeGuardEvents()
		{
			if (FSInternalJournal.EventCode == 0x28)
			{
				switch (Bytes[17])
				{
					case 0:
						{
							FS2JournalItem.Detalization += "команда с компьютера\n";
							if (Bytes[16] == 0)
								FS2JournalItem.Detalization += "через USB\n";
							else
								FS2JournalItem.Detalization += "через канал МС " + Bytes[16] + "\n";
							break;
						}
					case 3:
						FS2JournalItem.Detalization += "Прибор: Рубеж-БИ Адрес:" + Bytes[16] + "\n";
						break;
					case 7:
						FS2JournalItem.Detalization += "Прибор: Рубеж-ПДУ Адрес:" + Bytes[16] + "\n";
						break;
					case 9:
						FS2JournalItem.Detalization += "Прибор: Рубеж-ПДУ-ПТ Адрес:" + Bytes[16] + "\n";
						break;
					case 100:
						FS2JournalItem.Detalization += "Устройство: МС-3 Адрес:" + Bytes[16] + "\n";
						break;
					case 101:
						FS2JournalItem.Detalization += "Устройство: МС-4 Адрес:" + Bytes[16] + "\n";
						break;
					case 102:
						FS2JournalItem.Detalization += "Устройство: УОО-ТЛ Адрес:" + Bytes[16] + "\n";
						break;
					//default:
					//    FS2JournalItem.Detalization += "Неизв. устр." + "(" + Bytes[17] + ") Адрес:" + Bytes[16] + "\n";
					//    break;
				}
				if (FS2JournalItem.DeviceCategory == 0x00)
					FS2JournalItem.DeviceCategory = 0x75;
			}
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
                FS2JournalItem.ZoneName = "Номер зоны не прочитан";
			}
		}

		string GetEventName()
		{
			if (FS2JournalItem.Device != null && FS2JournalItem.Device.Driver.DriverType == DriverType.AM1_T && FSInternalJournal.EventCode == 58)
				return GetEventNameAMT();

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

		string GetEventNameAMT()
		{
			if (FSInternalJournal.AdditionalEventCode == 0)
				return FS2JournalItem.Device.Properties.FirstOrDefault(x => x.Name == "Event1").Value;
			else if (FSInternalJournal.AdditionalEventCode == 1)
				return FS2JournalItem.Device.Properties.FirstOrDefault(x => x.Name == "Event2").Value;
			else
				return "";
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

		int GetIntEventClass()
		{
			var stringCode = "$" + FSInternalJournal.EventCode.ToString("X2");
			var metadataEvent = MetadataHelper.Metadata.events.FirstOrDefault(x => x.rawEventCode == stringCode);
			if (metadataEvent == null)
				return -1;

			if (metadataEvent.detailsFor != null)
			{
			}

			var stringEventClass = "";
			if (metadataEvent.eventClassAll != null)
			{
				stringEventClass = metadataEvent.eventClassAll;
			}
			else
			{
				stringEventClass = MetadataHelper.GetAdditionalEventClass(metadataEvent, FSInternalJournal.AdditionalEventCode);
			}

			if (!string.IsNullOrEmpty(stringEventClass))
			{
				try
				{
					return int.Parse(stringEventClass);
				}
				catch { }
			}
			return -1;
		}
	}
}