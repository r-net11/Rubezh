using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.XModels;
using FiresecClient;
using XFiresecAPI;

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
        bool EventYesNo;
        string EventDescription;
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
				InternalJournalItem = this
			};

            var states = XStatesHelper.StatesFromInt(journalItem.ObjectState);
			var stateClasses = XStateClassHelper.Convert(states, false, false);
            
			if(Source == JournalSourceType.Object)
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
							EventName = "Пользователь";
							break;

						case 8:
							EventName = "Администратор";
							break;

						case 9:
							EventName = "Ошибка управления";
							break;
                    }
                    break;

                case JournalSourceType.Device:
                    switch (Code)
                    {
                        case 0:
                            EventName = "Неизвестный тип";
                            break;

                        case 1:
                            EventName = "Устройство с таким адресом не описано при конфигурации";
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
                            EventDescription = BytesHelper.SubstructShort(bytes, 32 + 14).ToString();
                            break;
                        case 1:
                            EventName = "Изменился заводской номер";
                            EventDescription = BytesHelper.SubstructInt(bytes, 32 + 14).ToString();
                            break;
                        case 2:
                            EventName = "Пожар";
                            EventYesNo = StringHelper.ToYesNo(bytes[32 + 14]);
                            EventDescription = StringHelper.ToFire(bytes[32 + 15]);
                            break;

                        case 3:
                            EventName = "Пожар-2";
                            break;

                        case 4:
                            EventName = "Внимание";
                            EventYesNo = StringHelper.ToYesNo(bytes[32 + 14]);
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
                            EventName = "Управление";
                            EventYesNo = StringHelper.ToYesNo(bytes[32 + 14]);
                            EventDescription = StringHelper.ToControl(bytes[32 + 15]);
                            break;

                        case 9:
                            EventName = "Состояние";
                            EventYesNo = StringHelper.ToYesNo(bytes[32 + 14]);
                            EventDescription = StringHelper.ToState(bytes[32 + 15]);
                            break;

                        case 10:
                            EventName = "Режим работы";
                            EventYesNo = StringHelper.ToYesNo(bytes[32 + 14]);
                            break;

                        case 11:
                            EventName = "Дежурный";
                            EventYesNo = StringHelper.ToYesNo(bytes[32 + 14]);
                            break;

                        case 12:
                            EventName = "Отключение";
                            EventYesNo = StringHelper.ToYesNo(bytes[32 + 14]);
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
        public static bool ToYesNo(byte b)
        {
            if (b == 0)
                return false;
            if (b == 1)
                return true;
			return false;
        }

        public static string ToRegime(byte b)
        {
            switch (b)
            {
                case 0: return "Автомат";
                case 1: return "Ручной";
                case 2: return "Отключен";
                case 3: return "Неопределен";
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
                case 1: return "Неисправность питания";
                case 2: return "Неисправность оптического канала или фотоусилителя";
                case 3: return "Неисправность температурного канала";
                case 4: return "Кз ШС";
                case 5: return "Обрыв ШС";
                case 6: return "Состояние датчика давления";
                case 7: return "Состояние датчика массы";
                case 8: return "Вскрытие";
                case 9: return "Реле не реагирует на команды (контакт не переключается)";
                case 10: return "Напряжение запуска реле ниже нормы";
                case 11: return "Кз выхода";
                case 12: return "Обрыв выхода";
                case 13: return "Напряжение питания ШС ниже нормы";
                case 14: return "Ошибка памяти";
                case 15: return "Кз выхода 1";
                case 16: return "Кз выхода 2";
                case 17: return "Кз выхода 3";
                case 18: return "Кз выхода 4";
                case 19: return "Кз выхода 5";
                case 20: return "Обрыв выхода 1";
                case 21: return "Обрыв выхода 2";
                case 22: return "Обрыв выхода 3";
                case 23: return "Обрыв выхода 4";
                case 24: return "Обрыв выхода 5";
                case 25: return "Несовместимость команд";
                case 26: return "Низкое напряжение питания привода";
                case 27: return "Обрыв в цепи НОРМА";
                case 28: return "Кз  в цепи НОРМА";
                case 29: return "Обрыв  в цепи ЗАЩИТА";
                case 30: return "Кз  в цепи ЗАЩИТА";
                case 31: return "Обрыв  в цепи ОТКРЫТО";
                case 32: return "Обрыв  в цепи ЗАКРЫТО";
                case 33: return "Обрыв в цепи ДВИГАТЕЛЬ";
                case 34: return "Замкнуты/разомкнуты оба концевика";
                case 35: return "Превышение времени хода";
                case 36: return "Обрыв в линии РЕЛЕ";
                case 37: return "Кз в линии РЕЛЕ";
                case 38: return "Неисправность выхода 1";
                case 39: return "Неисправность выхода 2";
                case 40: return "Неисправность выхода 3";
                case 41: return "Нет питания на вводе";
                case 42: return "Обрыв шлейфа с концевого выключателя ОТКРЫТО";
                case 43: return "Кз шлейфа с концевого выключателя ОТКРЫТО";
                case 44: return "Обрыв шлейфа с муфтового выключателя ОТКРЫТО";
                case 45: return "Кз шлейфа с муфтового выключателя ОТКРЫТО";
                case 46: return "Обрыв шлейфа с концевого выключателя ЗАКРЫТО";
                case 47: return "Кз шлейфа с концевого выключателя ЗАКРЫТО";
                case 48: return "Обрыв шлейфа с муфтового выключателя ЗАКРЫТО/ДУ ЗАКРЫТЬ";
                case 49: return "Кз шлейфа с муфтового выключателя ЗАКРЫТО/ ДУ ЗАКРЫТЬ";
                case 50: return "Обрыв шлейфа с муфтового выключателя ОТКРЫТЬ УЗЗ/ЗАКРЫТЬ УЗЗ";
                case 51: return "Кз шлейфа с муфтового выключателя ОТКРЫТЬ УЗЗ/ЗАКРЫТЬ УЗЗ";
                case 52: return "Обрыв шлейфа с муфтового выключателя СТОП УЗЗ";
                case 53: return "Кз шлейфа с муфтового выключателя СТОП УЗЗ";
                case 57: return "Неисправность КВ/МВ";
                case 58: return "Не задан режим";
                case 59: return "Отказ ШУЗ";
                case 60: return "Неисправность ДУ/ДД";
                case 61: return "Обрыв вх 9";
                case 62: return "Кз вх 9";
                case 63: return "Обрыв вх 10";
                case 64: return "Кз вх 10";
                case 65: return "Обрыв вх 11";
                case 66: return "Кз вх 11";
                case 67: return "Обрыв вх 12";
                case 68: return "Кз вх 12";
                case 69: return "Не задан тип";
                case 70: return "Отказ ПН";
                case 71: return "Отказ ШУН";
                case 72: return "Неисправность питания основного";
                case 73: return "Неисправность питания резервного";
                case 74: return "Неисправность шлейфа 1, 2";
                case 75: return "Неисправность шлейфа 3, 4";
                case 76: return "Неисправность шлейфа 5, 6";
                case 77: return "Неисправность шлейфа 7, 8";
                case 255: return "Потеря связи";
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

        public static string ToControl(byte b)
        {
            switch (b)
            {
                case 1: return "Пуск";
                case 2: return "Отмена задержки";
                case 3: return "Аналоговый вход - память; источник сигнала)";
                case 4: return "Выключить";
                case 5: return "Стоп";
                case 6: return "Запрет пуска";
            }
            return "";
        }

        public static string ToState(byte b)
        {
            switch (b)
            {
                case 1: return "Отсчет задержки";
                case 2: return "Включено";
                case 3: return "Выключено";
                case 4: return "Включается";
                case 5: return "Выключается";
                case 6: return "Кнопка (0 — ППКП ; источник команды)";
                case 7: return "Изменение автоматики по неисправности";
                case 8: return "Изменение автоматики по кнопке СТОП";
                case 9: return "Изменение автоматики по датчику ДВЕРИ-ОКНА";
                case 10: return "Изменение автоматики по ТМ";
                case 11: return "Автоматика включена";
                case 12: return "Ручной пуск АУП от ИПР";
                case 13: return "Отложенный пуск АУП по датчику ДВЕРИ-ОКНА";
                case 14: return "Пуск АУП завершен";
                case 15: return "Останов тушения по кнопке СТОП";
                case 16: return "Программирование мастер-ключа";
                case 17: return "Отсчет удержания";
                case 18: return "Уровень высокий";
                case 19: return "Уровень низкий";
                case 20: return "Ход по команде с УЗЗ";
                case 21: return "У ДУ сообщение ПУСК НЕВОЗМОЖЕН";
                case 22: return "Авария пневмоемкости";
                case 23: return "Уровень аварийный";
                case 24: return "Запрет пуска НС";
                case 25: return "Запрет пуска компрессора";
                case 26: return "Команда с УЗН";
                case 27: return "Перевод в режим ручного управления";
            }
            return "";
        }
    }
}