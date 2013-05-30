using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.XModels;
using FiresecClient;
using XFiresecAPI;
using System.Text;

namespace Common.GK
{
	public class InternalJournalItem
	{
		public int GKNo { get; private set; }
		string GKIpAddress;
		string StringDate;
		JournalItemType JournalItemType;
		Guid ObjectUID;
		string EventName;
		JournalYesNoType EventYesNo;
		string EventDescription;
		string UserName;
		int ObjectState;

		ushort GKObjectNo;
		int KAUNo;

		byte Day;
		byte Month;
		byte Year;
		byte Hour;
		byte Minute;
		byte Second;
		DateTime DateTime;

		ushort KAUAddress { get; set; }
		JournalSourceType Source { get; set; }
		byte Code { get; set; }

		ushort ObjectNo;
		public ushort ObjectDeviceType { get; private set; }
		public ushort ObjectDeviceAddress { get; private set; }
		int ObjectFactoryNo;

		void InitializeFromObjectUID()
		{
			if (GKObjectNo != 0)
			{
				var device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.GetDatabaseNo(DatabaseType.Gk) == GKObjectNo);
				if (device != null)
				{
					JournalItemType = JournalItemType.Device;
					ObjectUID = device.UID;
				}
				var zone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.GetDatabaseNo(DatabaseType.Gk) == GKObjectNo);
				if (zone != null)
				{
					JournalItemType = JournalItemType.Zone;
					ObjectUID = zone.UID;
				}
				var direction = XManager.DeviceConfiguration.Directions.FirstOrDefault(x => x.GetDatabaseNo(DatabaseType.Gk) == GKObjectNo);
				if (direction != null)
				{
					JournalItemType = JournalItemType.Direction;
					ObjectUID = direction.UID;
				}
			}
		}

		public JournalItem ToJournalItem()
		{
			var journalItem = new JournalItem()
			{
				GKIpAddress = GKIpAddress,
				GKJournalRecordNo = GKNo,
				DateTime = DateTime,
				ObjectUID = ObjectUID,
				Name = EventName,
				YesNo = EventYesNo,
				Description = EventDescription,
				ObjectState = ObjectState,
				JournalItemType = JournalItemType,
				GKObjectNo = GKObjectNo,
				UserName = UserName,
				InternalJournalItem = this
			};

			var states = XStatesHelper.StatesFromInt(journalItem.ObjectState);
			var stateClasses = XStateClassHelper.Convert(states, false, false);

			if (Source == JournalSourceType.Object)
				journalItem.StateClass = XStateClassHelper.GetMinStateClass(stateClasses);
			else
				journalItem.StateClass = XStateClass.Info;

			return journalItem;
		}

		public InternalJournalItem(XDevice gkDevice, List<byte> bytes)
		{
			GKIpAddress = XManager.GetIpAddress(gkDevice);
			GKNo = BytesHelper.SubstructInt(bytes, 0);
			GKObjectNo = BytesHelper.SubstructShort(bytes, 4);
			KAUNo = BytesHelper.SubstructInt(bytes, 32);

			JournalItemType = JournalItemType.GK;
			ObjectUID = gkDevice.UID;
			InitializeFromObjectUID();

			Day = bytes[32 + 4];
			Month = bytes[32 + 5];
			Year = bytes[32 + 6];
			Hour = bytes[32 + 7];
			Minute = bytes[32 + 8];
			Second = bytes[32 + 9];
			StringDate = Day.ToString() + "/" + Month.ToString() + "/" + Year.ToString() + " " + Hour.ToString() + ":" + Minute.ToString() + ":" + Second.ToString();
			try
			{
				DateTime = new DateTime(2000 + Year, Month, Day, Hour, Minute, Second);
			}
			catch
			{
				DateTime = DateTime.MinValue;
			}

			KAUAddress = BytesHelper.SubstructShort(bytes, 32 + 10);
			Source = (JournalSourceType)(int)(bytes[32 + 12]);
			Code = bytes[32 + 13];

			switch (Source)
			{
				case JournalSourceType.Controller:
					switch (Code)
					{
						case 0:
							EventName = "Технология";
							break;

						case 1:
							EventName = "Очистка журнала";
							break;

						case 2:
							EventName = "Установка часов";
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
							break;

						case 7:
							EventName = "Вход пользователя в систему";
							EventDescription = StringHelper.ToUser(bytes[32 + 15]);
							var bytes1 = bytes.GetRange(6, 31 - 6 + 1);
							var bytes2 = bytes.GetRange(16, 21 - 16 + 1);
							bytes1.AddRange(bytes2);
							UserName = Encoding.Default.GetString(bytes1.ToArray(), 0, bytes1.Count);
							JournalItemType = JournalItemType.User;
							break;

						case 8:
							EventName = "Выход пользователя из системы";
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
					}
					break;

				case JournalSourceType.Device:
					var unknownType = BytesHelper.SubstructShort(bytes, 32 + 14);
					var unknownAddress = BytesHelper.SubstructShort(bytes, 32 + 16);
					var presentationAddress = (unknownAddress / 256 + 1).ToString() + "." + (unknownAddress % 256).ToString();
					var driverName = unknownType.ToString();
					var driver = XManager.DriversConfiguration.XDrivers.FirstOrDefault(x => x.DriverTypeNo == unknownType);
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
					}
					break;

				case JournalSourceType.Object:
					ObjectNo = BytesHelper.SubstructShort(bytes, 32 + 18);
					ObjectDeviceType = BytesHelper.SubstructShort(bytes, 32 + 20);
					ObjectDeviceAddress = BytesHelper.SubstructShort(bytes, 32 + 22);
					ObjectFactoryNo = BytesHelper.SubstructInt(bytes, 32 + 24);
					ObjectState = BytesHelper.SubstructInt(bytes, 32 + 28);
					switch (Code)
					{
						case 0:
							EventName = "При конфигурации описан другой тип";
							var realType = BytesHelper.SubstructShort(bytes, 32 + 14);
							var realDriverString = "Неизвестный тип " + realType.ToString();
							var realDriver = XManager.DriversConfiguration.XDrivers.FirstOrDefault(x => x.DriverTypeNo == realType);
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
							if(JournalItemType == GK.JournalItemType.Device)
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
							EventDescription = StringHelper.ToFailure(bytes[32 + 15]);
							break;

						case 6:
							EventName = "Тест";
							EventYesNo = StringHelper.ToYesNo(bytes[32 + 14]);
							EventDescription = StringHelper.ToTest(bytes[32 + 15]);
							break;

						case 7:
							EventName = "Запыленность";
							EventYesNo = StringHelper.ToYesNo(bytes[32 + 14]);
							EventDescription = StringHelper.ToDustinness(bytes[32 + 15]);
							break;

						case 8:
							EventName = "Информация";
							EventDescription = StringHelper.ToInformation(bytes[32 + 15]);
							break;

						case 9:
							EventName = "Состояние";
							EventDescription = StringHelper.ToState(bytes[32 + 15]);
							break;

						case 10:
							EventName = "Режим работы";
							EventDescription = StringHelper.ToRegime(bytes[32 + 15]);
							break;

						case 13:
							EventName = "Параметры";
							EventYesNo = StringHelper.ToYesNo(bytes[32 + 14]);
							break;

						case 14:
							EventName = "Норма";
							EventYesNo = StringHelper.ToYesNo(bytes[32 + 14]);
							break;
					}
					break;
			}
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

		public static string ToRegime(byte b)
		{
			switch (b)
			{
				case 0: return "Автоматика";
				case 1: return "Ручное";
				case 2: return "Отключение";
				case 3: return "Не определено";
			}
			return "";
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
				case 0: return "                    ";
				case 1: return "Питание             ";
				case 2: return "Оптич канал/Фотоуси ";
				case 3: return "Температурный канал ";
				case 4: return "КЗ ШС               ";
				case 5: return "Обрыв ШС            ";
				case 6: return "Датчик ДАВЛЕНИЕ     ";
				case 7: return "Датчик МАССА        ";
				case 8: return "Вскрытие            ";
				case 9: return "Контакт не переключ ";
				case 10: return "U запуска не норма  ";
				case 11: return "КЗ выхода           ";
				case 12: return "Обрыв выхода        ";
				case 13: return "U ШС не норма       ";
				case 14: return "Ошибка памяти       ";
				case 15: return "КЗ выхода 1         ";
				case 16: return "КЗ выхода 2         ";
				case 17: return "КЗ выхода 3         ";
				case 18: return "КЗ выхода 4         ";
				case 19: return "КЗ выхода 5         ";
				case 20: return "Обрыв выхода 1      ";
				case 21: return "Обрыв выхода 2      ";
				case 22: return "Обрыв выхода 3      ";
				case 23: return "Обрыв выхода 4      ";
				case 24: return "Обрыв выхода 5      ";
				case 25: return "Несовместим команды ";
				case 26: return "U привода не норма  ";
				case 27: return "Обр кнопка НОРМА    ";
				case 28: return "КЗ кнопка НОРМА     ";
				case 29: return "Обр кнопка ЗАЩИТА   ";
				case 30: return "КЗ кнопка ЗАЩИТА    ";
				case 31: return "Обр конц ОТКРЫТО    ";
				case 32: return "Обр конц ЗАКРЫТО    ";
				case 33: return "Обр цепи 1 ДВИГАТЕЛЯ";
				case 34: return "З/Р оба концевика   ";
				case 35: return "Истекло время хода  ";
				case 36: return "Обр в линии РЕЛЕ    ";
				case 37: return "КЗ в линии РЕЛЕ     ";
				case 38: return "Выход 1             ";
				case 39: return "Выход 2             ";
				case 40: return "Выход 3             ";
				case 41: return "                    ";
				case 42: return "Обр конц ОТКРЫТО    ";
				case 43: return "КЗ конц ОТКРЫТО     ";
				case 44: return "Обр муфт ОТКРЫТО    ";
				case 45: return "КЗ муфт ОТКРЫТО     ";
				case 46: return "Обр конц ЗАКРЫТО    ";
				case 47: return "КЗ конц ЗАКРЫТО     ";
				case 48: return "Обр муфт ЗАКРЫТО    ";
				case 49: return "КЗ муфт ЗАКРЫТО     ";
				case 50: return "Обр кнопка ОУЗЗ/ЗУЗЗ";
				case 51: return "КЗ кнопка ОУЗЗ/ЗУЗЗ ";
				case 52: return "Обр кнопка СТОП УЗЗ ";
				case 53: return "КЗ кнопка СТОП УЗЗ  ";
				case 54: return "                    ";
				case 55: return "                    ";
				case 56: return "                    ";
				case 57: return "КВ/МВ               ";
				case 58: return "Не задан режим      ";
				case 59: return "Отказ ШУЗ           ";
				case 60: return "ДУ/ДД               ";
				case 61: return "Обрыв входа 9       ";
				case 62: return "КЗ входа 9          ";
				case 63: return "Обрыв входа 10      ";
				case 64: return "КЗ входа 10         ";
				case 65: return "Обрыв входа 11      ";
				case 66: return "КЗ входа 11         ";
				case 67: return "Обрыв входа 12      ";
				case 68: return "КЗ входа 12         ";
				case 69: return "Не задан тип        ";
				case 70: return "Отказ ПН            ";
				case 71: return "Отказ ШУН           ";
				case 72: return "Питание 1           ";
				case 73: return "Питание 2           ";
				case 74: return "АЛС 1-2             ";
				case 75: return "АЛС 3-4             ";
				case 76: return "АЛС 5-6             ";
				case 77: return "АЛС 7-8             ";
				case 78: return "Обр цепи 2 ДВИГАТЕЛЯ";
				case 79: return "КЗ АЛС1             ";
				case 80: return "КЗ АЛС2             ";
				case 81: return "КЗ АЛС3             ";
				case 82: return "КЗ АЛС4             ";
				case 83: return "КЗ АЛС5             ";
				case 84: return "КЗ АЛС6             ";
				case 85: return "КЗ АЛС7             ";
				case 86: return "КЗ АЛС8             ";
				case 87: return "Истекло время вкл   ";
				case 88: return "Истекло время выкл  ";
				case 89: return "Контакт реле 1      ";
				case 90: return "Контакт реле 2      ";
				case 91: return "Обр кнопка ПУСК     ";
				case 92: return "КЗ кнопка ПУСК      ";
				case 93: return "Обр кнопка СТОП     ";
				case 94: return "КЗ кнопка СТОП      ";
				case 95: return "Сообщения           ";
				case 96: return "Выход               ";
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
				case 253: return "ОЛС                ";
				case 254: return "РЛС                ";
				case 255: return "Потеря связи       ";
			}
			return "";
		}

		public static string ToTest(byte b)
		{
			switch (b)
			{
				case 1: return "Кнопка";
				case 2: return "Указка";
			}
			return "";
		}

		public static string ToDustinness(byte b)
		{
			switch (b)
			{
				case 1: return "Предварительная";
				case 2: return "Критическая";
			}
			return "";
		}

		public static string ToInformation(byte b)
		{
			switch (b)
			{
				case 0: return "                    ";
				case 1: return "Команда от ППКП     ";
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

		public static string ToState(byte b)
		{
			switch (b)
			{
				case 2: return "Включено";
				case 3: return "Выключено";
				case 4: return "Включается";
				case 5: return "Выключается";
				case 30: return "Не определено";
				case 31: return "Остановлено";
			}
			return "";
		}
	}
}