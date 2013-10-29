using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.XModels;
using FiresecClient;
using XFiresecAPI;
using System.Runtime.Serialization;

namespace Common.GK
{
	[DataContract]
	public class InternalJournalItem
	{
		[DataMember]
		public int GKNo { get; private set; }
		[DataMember]
		string GKIpAddress;
		[DataMember]
		public JournalItemType JournalItemType { get; private set; }
		[DataMember]
		public Guid ObjectUID { get; private set; }
		[DataMember]
		string EventName;
		[DataMember]
		string UserFriendlyEventName;
		[DataMember]
		JournalYesNoType EventYesNo;
		[DataMember]
		string EventDescription;
		[DataMember]
		string UserName;
		[DataMember]
		int ObjectState;
		[DataMember]
		XStateClass StateClass;

		[DataMember]
		public ushort GKObjectNo { get; private set; }
		[DataMember]
		public int UNUSED_KAUNo { get; private set; }

		[DataMember]
		DateTime DeviceDateTime;
		[DataMember]
		DateTime SystemDateTime;

		[DataMember]
		ushort UNUSED_KAUAddress { get; set; }
		[DataMember]
		JournalSourceType Source { get; set; }
		[DataMember]
		byte Code { get; set; }

		[DataMember]
		ushort UNUSED_ObjectNo;
		[DataMember]
		public ushort ObjectDeviceType { get; private set; }
		[DataMember]
		public ushort ObjectDeviceAddress { get; private set; }
		[DataMember]
		int UNUSED_ObjectFactoryNo;

		[DataMember]
		public XDevice Device { get; private set; }
		[DataMember]
		public XZone Zone { get; private set; }
		[DataMember]
		public XDirection Direction { get; private set; }

		XSubsystemType SubsytemType
		{
			get
			{
				if (JournalItemType == GK.JournalItemType.System)
					return XSubsystemType.System;
				else
					return XSubsystemType.GK;
			}
		}

		void InitializeFromObjectUID()
		{
			if (GKObjectNo != 0)
			{
				Device = XManager.Devices.FirstOrDefault(x => x.GetDatabaseNo(DatabaseType.Gk) == GKObjectNo);
				if (Device != null)
				{
					JournalItemType = JournalItemType.Device;
					ObjectUID = Device.UID;
				}
				Zone = XManager.Zones.FirstOrDefault(x => x.GetDatabaseNo(DatabaseType.Gk) == GKObjectNo);
				if (Zone != null)
				{
					JournalItemType = JournalItemType.Zone;
					ObjectUID = Zone.UID;
				}
				Direction = XManager.Directions.FirstOrDefault(x => x.GetDatabaseNo(DatabaseType.Gk) == GKObjectNo);
				if (Direction != null)
				{
					JournalItemType = JournalItemType.Direction;
					ObjectUID = Direction.UID;
				}
			}
		}

		public JournalItem ToJournalItem()
		{
			var journalItem = new JournalItem()
			{
				GKIpAddress = GKIpAddress,
				GKJournalRecordNo = GKNo,
				DeviceDateTime = DeviceDateTime,
				SystemDateTime = SystemDateTime,
				ObjectUID = ObjectUID,
				Name = EventName,
				UserFriendlyName = UserFriendlyEventName,
				YesNo = EventYesNo,
				Description = EventDescription,
				ObjectState = ObjectState,
				JournalItemType = JournalItemType,
				GKObjectNo = GKObjectNo,
				UserName = UserName,
				SubsystemType = SubsytemType,
				StateClass = StateClass,
				InternalJournalItem = this
			};

			if (Source == JournalSourceType.Object)
			{
				var stateBits = XStatesHelper.StatesFromInt(ObjectState);
				var stateClasses = XStatesHelper.StateBitsToStateClasses(stateBits, false, false, false, false, false);
				journalItem.ObjectStateClass = XStatesHelper.GetMinStateClass(stateClasses);
			}
			else
			{
				journalItem.ObjectStateClass = XStateClass.Info;
			}

			return journalItem;
		}

		void InitializeDateTime(List<byte> bytes)
		{
			var day = bytes[32 + 4];
			var month = bytes[32 + 5];
			var year = bytes[32 + 6];
			var hour = bytes[32 + 7];
			var minute = bytes[32 + 8];
			var second = bytes[32 + 9];
			try
			{
				DeviceDateTime = new DateTime(2000 + year, month, day, hour, minute, second);
			}
			catch
			{
				DeviceDateTime = DateTime.MinValue;
			}
			SystemDateTime = DateTime.Now;
		}

		public InternalJournalItem(XDevice gkDevice, List<byte> bytes)
		{
			GKIpAddress = XManager.GetIpAddress(gkDevice);
			GKNo = BytesHelper.SubstructInt(bytes, 0);
			GKObjectNo = BytesHelper.SubstructShort(bytes, 4);
			UNUSED_KAUNo = BytesHelper.SubstructInt(bytes, 32);

			JournalItemType = JournalItemType.GK;
			ObjectUID = gkDevice.UID;
			InitializeFromObjectUID();

			InitializeDateTime(bytes);

			UNUSED_KAUAddress = BytesHelper.SubstructShort(bytes, 32 + 10);
			Source = (JournalSourceType)(int)(bytes[32 + 12]);
			Code = bytes[32 + 13];

			StateClass = XStateClass.No;
			switch (Source)
			{
				case JournalSourceType.Controller:
					switch (Code)
					{
						case 0:
							EventName = "Технология";
							UserFriendlyEventName = "Перевод в технологический режим";
							break;

						case 2:
							EventName = "Установка часов";
							UserFriendlyEventName = "Синхронизация времени прибора с временем ПК";
							break;

						case 3:
							EventName = "Запись информации о блоке";
							break;

						case 4:
							EventName = "Смена ПО";
							break;

						case 5:
							EventName = "Смена БД";
							break;

						case 6:
							EventName = "Работа";
							UserFriendlyEventName = "Работа шкафа";
							break;

						case 7:
							EventName = "Вход пользователя в прибор";
							EventDescription = StringHelper.ToUser(bytes[32 + 15]);
							var bytes1 = bytes.GetRange(6, 31 - 6 + 1);
							var bytes2 = bytes.GetRange(16, 21 - 16 + 1);
							bytes1.AddRange(bytes2);
							UserName = Encoding.Default.GetString(bytes1.ToArray(), 0, bytes1.Count);
							JournalItemType = JournalItemType.User;
							break;

						case 8:
							EventName = "Выход пользователя из прибора";
							EventDescription = StringHelper.ToUser(bytes[32 + 15]);
							bytes1 = bytes.GetRange(6, 31 - 6 + 1);
							bytes2 = bytes.GetRange(48, 53 - 48 + 1);
							bytes1.AddRange(bytes2);
							UserName = Encoding.Default.GetString(bytes1.ToArray(), 0, bytes1.Count);
							JournalItemType = JournalItemType.User;
							break;

						case 9:
							EventName = "Ошибка управления";
							GKObjectNo = BytesHelper.SubstructShort(bytes, 18);
							break;

						case 10:
							EventName = "Введен новый пользователь";
							JournalItemType = JournalItemType.User;
							break;

						case 11:
							EventName = "Изменена учетная информация пользователя";
							JournalItemType = JournalItemType.User;
							break;

						case 12:
							EventName = "Произведена настройка сети";
							break;

						default:
							EventName = "Неизвестный код события контроллекра";
							EventDescription = Code.ToString();
							break;
					}
					break;

				case JournalSourceType.Device:
					var unknownType = BytesHelper.SubstructShort(bytes, 32 + 14);
					var unknownAddress = BytesHelper.SubstructShort(bytes, 32 + 16);
					var presentationAddress = (unknownAddress / 256 + 1).ToString() + "." + (unknownAddress % 256).ToString();
					var driverName = unknownType.ToString();
					var driver = XManager.Drivers.FirstOrDefault(x => x.DriverTypeNo == unknownType);
					if (driver != null)
					{
						driverName = driver.ShortName;
					};
					var unknownDescription = "Тип: " + driverName + " Адрес: " + presentationAddress;
					switch (Code)
					{
						case 0:
							EventName = "Неизвестный тип";
							EventDescription = unknownDescription;
							break;

						case 1:
							EventName = "Устройство с таким адресом не описано при конфигурации";
							EventDescription = unknownDescription;
							break;

						default:
							EventName = "Неизвестный код события устройства";
							EventDescription = Code.ToString();
							break;
					}
					break;

				case JournalSourceType.Object:
					UNUSED_ObjectNo = BytesHelper.SubstructShort(bytes, 32 + 18);
					ObjectDeviceType = BytesHelper.SubstructShort(bytes, 32 + 20);
					ObjectDeviceAddress = BytesHelper.SubstructShort(bytes, 32 + 22);
					UNUSED_ObjectFactoryNo = BytesHelper.SubstructInt(bytes, 32 + 24);
					ObjectState = BytesHelper.SubstructInt(bytes, 32 + 28);
					switch (Code)
					{
						case 0:
							EventName = "При конфигурации описан другой тип";
							var realType = BytesHelper.SubstructShort(bytes, 32 + 14);
							var realDriverString = "Неизвестный тип " + realType.ToString();
							var realDriver = XManager.Drivers.FirstOrDefault(x => x.DriverTypeNo == realType);
							if (realDriver != null)
							{
								realDriverString = realDriver.ShortName;
							}
							EventDescription = "Действительный тип: " + realDriverString;
							break;
						case 1:
							EventName = "Изменился заводской номер";
							EventDescription = "Старый заводсткой номер: " + BytesHelper.SubstructInt(bytes, 32 + 14).ToString();
							break;
						case 2:
							EventName = "Пожар-1";
							if (JournalItemType == GK.JournalItemType.Device)
								EventName = "Сработка-1";
							EventDescription = StringHelper.ToFire(bytes[32 + 15]);
							break;

						case 3:
							EventName = "Пожар-2";
							if (JournalItemType == GK.JournalItemType.Device)
								EventName = "Сработка-2";
							EventDescription = StringHelper.ToFire(bytes[32 + 15]);
							break;

						case 4:
							EventName = "Внимание";
							break;

						case 5:
							EventName = "Неисправность";
							EventYesNo = StringHelper.ToYesNo(bytes[32 + 14]);

							if (EventYesNo == JournalYesNoType.No)
								UserFriendlyEventName = "Неисправность устранена";

							if (ObjectDeviceType == 0xE0)
								EventDescription = StringHelper.ToBUSHFailure(bytes[32 + 15]);
							else
								EventDescription = StringHelper.ToFailure(bytes[32 + 15]);
							break;

						case 6:
							UserFriendlyEventName = EventName = "Тест";
							EventYesNo = StringHelper.ToYesNo(bytes[32 + 14]);

							switch (bytes[32 + 15])
							{
								case 1:
									EventDescription = "Кнопка";
									UserFriendlyEventName = "Тест кнопка";
									break;

								case 2:
									EventDescription = "Указка";
									UserFriendlyEventName = "Тест лазер";
									break;
							}

							if (EventYesNo == JournalYesNoType.No)
								UserFriendlyEventName += " устранено";

							break;

						case 7:
							UserFriendlyEventName = EventName = "Запыленность";
							EventYesNo = StringHelper.ToYesNo(bytes[32 + 14]);

							switch (bytes[32 + 15])
							{
								case 1:
									EventDescription = "Предварительная";
									UserFriendlyEventName = "Предварительная запыленность";
									break;

								case 2:
									EventDescription = "Критическая";
									UserFriendlyEventName = "Критическая запыленность";
									break;
							}

							if (EventYesNo == JournalYesNoType.No)
								UserFriendlyEventName += " устранена";
							break;

						case 8:
							EventName = "Информация";
							if (ObjectDeviceType == 0xE0)
								EventDescription = StringHelper.ToBUSHInformation(bytes[32 + 15]);
							else
								EventDescription = StringHelper.ToInformation(bytes[32 + 15]);
							break;

						case 9:
							EventName = "Состояние";
							switch (bytes[32 + 15])
							{
								case 2:
									EventDescription = "Включено";
									UserFriendlyEventName = "Включено";
									StateClass = XStateClass.On;
									break;

								case 3:
									EventDescription = "Выключено";
									UserFriendlyEventName = "Выключено";
									StateClass = XStateClass.Off;
									break;

								case 4:
									EventDescription = "Включается";
									UserFriendlyEventName = "Включается";
									StateClass = XStateClass.TurningOn;
									break;

								case 5:
									EventDescription = "Выключается";
									UserFriendlyEventName = "Выключается";
									StateClass = XStateClass.TurningOff;
									break;

								case 30:
									EventDescription = "Не определено";
									UserFriendlyEventName = "Состояние не определено";
									StateClass = XStateClass.Unknown;
									break;

								case 31:
									EventDescription = "Остановлено";
									UserFriendlyEventName = "Остановлено";
									StateClass = XStateClass.Info;
									break;
							}
							break;

						case 10:
							EventName = "Режим работы";
							switch (bytes[32 + 15])
							{
								case 0:
									EventDescription = "Автоматика";
									UserFriendlyEventName = "Перевод в автоматический режим";
									StateClass = XStateClass.Norm;
									break;

								case 1:
									EventDescription = "Ручное";
									UserFriendlyEventName = "Перевод в ручной режим";
									StateClass = XStateClass.AutoOff;
									break;

								case 2:
									EventDescription = "Отключение";
									UserFriendlyEventName = "Перевод в отключенный режим";
									StateClass = XStateClass.Off;
									break;

								case 3:
									EventDescription = "Не определено";
									UserFriendlyEventName = "Перевод в неопределенный режим";
									StateClass = XStateClass.Unknown;
									break;
							}
							break;

						case 13:
							EventName = "Параметры";
							UserFriendlyEventName = "Запись параметра объекта";
							EventDescription = "Номер объекта КАУ: " + Code;
							break;

						case 14:
							EventName = "Норма";
							break;

						default:
							EventName = "Неизвестный код события объекта:";
							EventDescription = Code.ToString();
							break;
					}
					break;
			}

			if (Device != null && Device.Driver.DriverType == XDriverType.Pump && EventName == "Неисправность")
			{
				EventDescription = StringHelper.GetPumpFailureMessage(EventDescription, Device.IntAddress);
			}

			if (string.IsNullOrEmpty(UserFriendlyEventName))
				UserFriendlyEventName = EventName;

			if (StateClass == XStateClass.No)
				StateClass = JournalDescriptionStateHelper.GetStateClassByName(EventName);
		}
	}

	public static class StringHelper
	{
		public static JournalYesNoType ToYesNo(byte b)
		{
			if (b == 0)
				return JournalYesNoType.No;
			if (b == 1)
				return JournalYesNoType.Yes;
			return JournalYesNoType.Unknown;
		}

		public static string ToFire(byte b)
		{
			switch (b)
			{
				case 1: return "Ручник сорван";
				case 2: return "Срабатывание по дыму";
				case 3: return "Срабатывание по температуре";
				case 4: return "Срабатывание по градиенту температуры";
			}
			return "";
		}

		public static string ToFailure(byte b)
		{
			switch (b)
			{
				case 1: return "Напряжение питания устройства не в норме"; // РМК, МПТ, МРО-2М
				case 2: return "Оптический канал или фотоусилитель"; // ИЗВЕЩАТЕЛИ
				case 3: return "Температурный канал"; // ИЗВЕЩАТЕЛИ
				case 4: return "КЗ ШС"; // МПТ, АМ
				case 5: return "Обрыв ШС"; // МПТ, АМ
				case 6: return "";
				case 7: return "";
				case 8: return "Вскрытие корпуса"; // АМ
				case 9: return "Контакт не переключается"; // РМ
				case 10: return "Напряжение запуска реле ниже нормы"; // РМК
				case 11: return "КЗ выхода"; // РМК
				case 12: return "Обрыв выхода"; // РМК
				case 13: return "Напряжение питания ШС ниже нормы"; // МПТ
				case 14: return "Ошибка памяти"; // МПТ
				case 15: return "КЗ выхода 1"; // МПТ
				case 16: return "КЗ выхода 2"; // МПТ
				case 17: return "КЗ выхода 3"; // МПТ
				case 18: return "КЗ выхода 4"; // МПТ
				case 19: return "КЗ выхода 5"; // МПТ
				case 20: return "Обрыв выхода 1"; // МПТ
				case 21: return "Обрыв выхода 2"; // МПТ
				case 22: return "Обрыв выхода 3"; // МПТ
				case 23: return "Обрыв выхода 4"; // МПТ
				case 24: return "Обрыв выхода 5"; // МПТ
				case 25: return "Блокировка пуска"; // МДУ
				case 26: return "Низкое напряжение питания привода"; // МДУ
				case 27: return "Обрыв кнопки НОРМА"; // МДУ
				case 28: return "КЗ кнопки НОРМА"; // МДУ
				case 29: return "Обрыв кнопка ЗАЩИТА"; // МДУ
				case 30: return "КЗ кнопки ЗАЩИТА"; // МДУ
				case 31: return "Обрыв концевого выключателя ОТКРЫТО"; // МДУ
				case 32: return "Обрыв концевого выключателя ЗАКРЫТО"; // МДУ
				case 33: return "Обрыв цепи 1 ДВИГАТЕЛЯ"; // МДУ
				case 34: return "Замкнуты/разомкнуты оба концевика"; // МДУ
				case 35: return "Превышение времени хода"; // МДУ
				case 36: return "Обр в линии РЕЛЕ    ";
				case 37: return "КЗ в линии РЕЛЕ     ";
				case 38: return "Выход 1             ";
				case 39: return "Выход 2             ";
				case 40: return "Выход 3             ";
				case 41: return "";
				case 42: return "Обрыв концевого выключателя ОТКРЫТО"; // ШУЗ
				case 43: return "КЗ концевого выключателя ОТКРЫТО"; // ШУЗ
				case 44: return "Обрыв муфтового выключателя ОТКРЫТО"; // ШУЗ
				case 45: return "КЗ муфтового выключателя ОТКРЫТО"; // ШУЗ
				case 46: return "Обрыв концевого выключателя ЗАКРЫТО"; // ШУЗ
				case 47: return "КЗ концевого выключателя ЗАКРЫТО"; // ШУЗ
				case 48: return "Обрыв муфтового выключателя ЗАКРЫТО"; // ШУЗ
				case 49: return "КЗ муфтового выключателя ЗАКРЫТО"; // ШУЗ
				case 50: return "Обрыв кнопки Открыть УЗЗ/Закрыть УЗЗ"; // ШУЗ
				case 51: return "КЗ кнопки Открыть УЗЗ/Закрыть УЗЗ"; // ШУЗ
				case 52: return "Обрыв кнопки СТОП УЗЗ"; // ШУЗ
				case 53: return "КЗ кнопки СТОП УЗЗ"; // ШУЗ
				case 54: return "Обрыв давление низкое ";
				case 55: return "КЗ давление низкое  ";
				case 56: return "Таймаут по давлению ";
				case 57: return "КВ/МВ"; // ШУЗ
				case 58: return "Не задан режим"; // ШУЗ
				case 59: return "Отказ ШУЗ"; // ШУЗ
				case 60: return "ДУ/ДД"; // ШУН
				case 61: return "Обрыв входа 9"; // ШУН
				case 62: return "КЗ входа 9"; // ШУН
				case 63: return "Обрыв входа 10"; // ШУН
				case 64: return "КЗ входа 10"; // ШУН
				case 65: return "Обрыв входа 11"; // ШУН
				case 66: return "КЗ входа 11"; // ШУН
				case 67: return "Обрыв входа 12"; // ШУН
				case 68: return "КЗ входа 12"; // ШУН
				case 69: return "Не задан тип"; // ШУН
				case 70: return "Отказ ПН"; // ШУН
				case 71: return "Отказ ШУН"; // ШУН
				case 72: return "Питание 1"; // КАУ, ГК
				case 73: return "Питание 2"; // КАУ, ГК
				case 74: return "Отказ АЛС 1 или 2"; // КАУ
				case 75: return "Отказ АЛС 3 или 4"; // КАУ
				case 76: return "Отказ АЛС 5 или 6"; // КАУ
				case 77: return "Отказ АЛС 7 или 8"; // КАУ
				case 78: return "Обрыв цепи 2 ДВИГАТЕЛЯ"; // МДУ
				case 79: return "КЗ АЛС 1"; // КАУ
				case 80: return "КЗ АЛС 2"; // КАУ
				case 81: return "КЗ АЛС 3"; // КАУ
				case 82: return "КЗ АЛС 4"; // КАУ
				case 83: return "КЗ АЛС 5"; // КАУ
				case 84: return "КЗ АЛС 6"; // КАУ
				case 85: return "КЗ АЛС 7"; // КАУ
				case 86: return "КЗ АЛС 8"; // КАУ
				case 87: return "Истекло время вкл   ";
				case 88: return "Истекло время выкл  ";
				case 89: return "Контакт реле 1      ";
				case 90: return "Контакт реле 2      ";
				case 91: return "Обрыв кнопки ПУСК"; // МРО-2М
				case 92: return "КЗ кнопки ПУСК"; // МРО-2М
				case 93: return "Обрыв кнопки СТОП"; // МРО-2М
				case 94: return "КЗ кнопки СТОП"; // МРО-2М
				case 95: return "Отсутствуют или испорчены сообщения для воспроизведения"; // МРО-2М
				case 96: return "Выход               ";
				case 97: return "Обр Уровень низкий  ";
				case 98: return "КЗ  Уровень низкий  ";
				case 99: return "Обр Уровень высокий ";
				case 100: return "КЗ  Уровень высокий ";
				case 101: return "Обр Уровень аварийн ";
				case 102: return "КЗ  Уровень аварийн ";
				case 103: return "Уровень аварийный   ";
				case 104: return "Питание силовое     ";
				case 105: return "Питание контроллера ";
				case 106: return "Несоответствие      ";
				case 107: return "Фаза                ";
				case 108: return "Обр Давление на вых ";
				case 109: return "КЗ  Давление на вых ";
				case 110: return "Обр ДУ ПУСК         ";
				case 111: return "КЗ  ДУ ПУСК         ";
				case 112: return "Обр ДУ СТОП         ";
				case 113: return "КЗ  ДУ СТОП         ";
				case 241: return "Обрыв АЛС 1-2      ";
				case 242: return "Обрыв АЛС 3-4      ";
				case 243: return "Обрыв АЛС 5-6      ";
				case 244: return "Обрыв АЛС 7-8      ";
				case 245: return "Обрыв АЛС 1        ";
				case 246: return "Обрыв АЛС 2        ";
				case 247: return "Обрыв АЛС 3        ";
				case 248: return "Обрыв АЛС 4        ";
				case 249: return "Обрыв АЛС 5        ";
				case 250: return "Обрыв АЛС 6        ";
				case 251: return "Обрыв АЛС 7        ";
				case 252: return "Обрыв АЛС 8        ";
				case 253: return "ОЛС"; // ГК
				case 254: return "РЛС"; // ГК
				case 255: return "Потеря связи";
			}
			return "";
		}

		public static string ToBUSHFailure(byte b)
		{
			switch (b)
			{
				case 0: return "Вскрытие";
				case 1: return "Контакт не переключ";
				case 2: return "Обрыв Уровень низкий";
				case 3: return "КЗ Уровень низкий";
				case 4: return "Обрыв Уровень высокий";
				case 5: return "КЗ Уровень высокий";
				case 6: return "Обрыв Уровень аварийный";
				case 7: return "КЗ Уровень аварийный";
				case 8: return "Уровень аварийный";
				case 9: return "Питание силовое";
				case 10: return "Питание контроллера";
				case 11: return "Несоответствие";
				case 12: return "Фаза";
				case 13: return "Обрыв Давление низкое";
				case 14: return "КЗ Давление низкое";
				case 15: return "Таймаут по давлению";
				case 16: return "Обрыв Давлен на выходе";
				case 17: return "КЗ Давлен на выходе";
				case 18: return "Обрыв ДУ ПУСК";
				case 19: return "КЗ ДУ ПУСК";
				case 20: return "Обрыв ДУ СТОП";
				case 21: return "КЗ ДУ СТОП";
			}
			return "";
		}

		public static string ToInformation(byte b)
		{
			switch (b)
			{
				case 1: return "Команда от прибора  ";
				case 2: return "Команда от кнопки   ";
				case 3: return "Изм автомат по Н    ";
				case 4: return "Изм автомат по СТОП ";
				case 5: return "Изм автомат по Д-О  ";
				case 6: return "Изм автомат по ТМ   ";
				case 7: return "Ручной пуск         ";
				case 8: return "Отлож пуск АУП Д-О  ";
				case 9: return "Пуск АУП завершен   ";
				case 10: return "Стоп по кнопке СТОП ";
				case 11: return "Прогр мастер-ключа  ";
				case 12: return "Датчик ДАВЛЕНИЕ     ";
				case 13: return "Датчик МАССА        ";
				case 14: return "Сигнал из памяти    ";
				case 15: return "Сигнал аналог входа ";
				case 16: return "Замена списка на 1  ";
				case 17: return "Замена списка на 2  ";
				case 18: return "Замена списка на 3  ";
				case 19: return "Замена списка на 4  ";
				case 20: return "Замена списка на 5  ";
				case 21: return "Замена списка на 6  ";
				case 22: return "Замена списка на 7  ";
				case 23: return "Замена списка на 8  ";
				case 24: return "Уровень низкий      ";
				case 25: return "Уровень высокий     ";
				case 26: return "Уровень норма       ";
				case 27: return "Автоматика          ";
				case 28: return "Ручное              ";
				case 29: return "Отключение          ";
				case 30: return "Неопределено        ";
				case 31: return "Пуск невозможен     ";
				case 32: return "Авария пневмоемкости";
				case 33: return "Уровень аварийный   ";
				case 34: return "Запрет пуска НС     ";
				case 35: return "Запрет пуска компрес";
				case 36: return "Ввод 1              ";
				case 37: return "Ввод 2              ";
				case 38: return "Команда от логики   ";
				case 39: return "Команда от ДУ       ";
				case 40: return "Давление низкое     ";
				case 41: return "Давление высокое    ";
				case 42: return "Давление норма      ";
				case 43: return "Давление неопределен";
				case 44: return "Давление на вых есть";
				case 45: return "Давление на вых нет ";
				case 46: return "Выключить           ";
				case 47: return "Стоп                ";
				case 48: return "Запрет пуска        ";
				case 49: return "Ручной пуск         ";
			}
			return "";
		}

		public static string ToBUSHInformation(byte b)
		{
			switch (b)
			{
				case 0: return "Уровень низкий      ";
				case 1: return "Уровень высокий     ";
				case 2: return "Уровень аварийный   ";
				case 3: return "Уровень норма       ";
				case 4: return "Команда от прибора  ";
				case 5: return "Команда от кнопки   ";
				case 6: return "Команда от логики   ";
				case 7: return "Команда от ДУ       ";
				case 8: return "Давление низкое     ";
				case 9: return "Давление норма      ";
				case 10: return "Давление высокое    ";
				case 11: return "Давление на вых есть";
				case 12: return "Давление на вых нет ";
			}
			return "";
		}

		public static string ToUser(byte b)
		{
			switch (b)
			{
				case 0: return "Оператор";
				case 1: return "Администратор";
				case 2: return "Инсталлятор";
				case 3: return "Изготовитель";
			}
			return "";
		}

		public static string GetPumpFailureMessage(string name, int address)
		{
			switch(name)
			{
				case "Обрыв входа 9":
					if(address <= 8)
						return "Обрыв ЭКМ на входе насоса";
					else
						return "Обрыв ДД/ДУ ПУСК";
				case "КЗ входа 9":
					if (address <= 8)
						return "КЗ ЭКМ на входе насоса";
					else
						return "КЗ ДД/ДУ ПУСК";

				case "Обрыв входа 10":
					if (address <= 8)
						return "Обрыв УЗН СТАРТ";
					else
						return "Обрыв ДД/ДУ СТОП";
				case "КЗ входа 10":
					if (address <= 8)
						return "КЗ УЗН СТАРТ";
					else
						return "КЗ ДД/ДУ СТОП";

				case "Обрыв входа 11":
					if (address <= 8)
						return "Обрыв УЗН СТОП";
					else if (address == 14 || address == 15)
						return "Обрыв ДД АВАРИЯ";
					break;
				case "КЗ входа 11":
					if (address <= 8)
						return "КЗ УЗН СТОП";
					else if (address == 14 || address == 15)
						return "КЗ ДД АВАРИЯ";
					break;

				case "Обрыв входа 12":
				case "КЗ входа 12":
					break;
			}
			return name;
		}
	}
}