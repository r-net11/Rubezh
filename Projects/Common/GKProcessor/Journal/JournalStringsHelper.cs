using RubezhAPI.Journal;

namespace GKProcessor
{
	public static class JournalStringsHelper
	{
		public static JournalEventDescriptionType ToFire(byte b)
		{
			switch (b)
			{
				case 1: return JournalEventDescriptionType.Ручник_сорван;
				case 2: return JournalEventDescriptionType.Срабатывание_по_дыму;
				case 3: return JournalEventDescriptionType.Срабатывание_по_температуре;
				case 4: return JournalEventDescriptionType.Срабатывание_по_градиенту_температуры;
			}
			return JournalEventDescriptionType.NULL;
		}

		public static JournalEventNameType ToState(byte b)
		{
			switch (b)
			{
				case 1: return JournalEventNameType.Отсчет_задержки;
				case 2: return JournalEventNameType.Включено;
				case 3: return JournalEventNameType.Выключено;
				case 4: return JournalEventNameType.Включается;
				case 5: return JournalEventNameType.Выключается;
				case 6: return JournalEventNameType.Кнопка;
				case 7: return JournalEventNameType.Изменение_автоматики_по_неисправности;
				case 8: return JournalEventNameType.Изменение_автоматики_по_кнопке_СТОП;
				case 9: return JournalEventNameType.Изменение_автоматики_по_датчику_ДВЕРИ_ОКНА;
				case 10: return JournalEventNameType.Изменение_автоматики_по_ТМ;
				case 11: return JournalEventNameType.Автоматика_включена;
				case 12: return JournalEventNameType.Ручной_пуск_АУП_от_ИПР;
				case 13: return JournalEventNameType.Отложенный_пуск_АУП_по_датчику_ДВЕРИ_ОКНА;
				case 14: return JournalEventNameType.Пуск_АУП_завершен;
				case 15: return JournalEventNameType.Останов_тушения_по_кнопке_СТОП;
				case 16: return JournalEventNameType.Программирование_мастер_ключа;

				case 17: return JournalEventNameType.Отсчет_удержания;
				case 18: return JournalEventNameType.Уровень_высокий;
				case 19: return JournalEventNameType.Уровень_низкий;
				case 20: return JournalEventNameType.Ход_по_команде_с_УЗЗ;

				case 21: return JournalEventNameType.У_ДУ_сообщение_ПУСК_НЕВОЗМОЖЕН;
				case 22: return JournalEventNameType.Авария_пневмоемкости;
				case 23: return JournalEventNameType.Уровень_аварийный;
				case 24: return JournalEventNameType.Запрет_пуска_НС;
				case 25: return JournalEventNameType.Запрет_пуска_компрессора;
				case 26: return JournalEventNameType.Команда_с_УЗН;
				case 27: return JournalEventNameType.Перевод_в_режим_ручного_управления;

				case 30: return JournalEventNameType.Состояние_не_определено;
				case 31: return JournalEventNameType.Остановлено;
			}
			return JournalEventNameType.Состояние_Неизвестно;
		}

		public static JournalEventNameType ToValveState(byte b)
		{
			switch (b)
			{
				case 2: return JournalEventNameType.Открыто;
				case 3: return JournalEventNameType.Закрыто;
				case 4: return JournalEventNameType.Открытие;
				case 5: return JournalEventNameType.Закрытие;
				case 30: return JournalEventNameType.Остановлено;
			}
			return ToState(b);
		}

		public static JournalEventNameType ToGuardZoneState(byte b)
		{
			switch (b)
			{
				case 2: return JournalEventNameType.На_охране;
				case 3: return JournalEventNameType.Не_на_охране;
				case 4: return JournalEventNameType.Постановка_на_охрану;
				case 5: return JournalEventNameType.Снятие_с_охраны;
			}
			return ToState(b);
		}

		public static JournalEventDescriptionType ToFailure(byte b, bool isMVP = false)
		{
			switch (b)
			{
				case 1: return isMVP ? JournalEventDescriptionType.КЗ_АЛС_1 : JournalEventDescriptionType.Напряжение_питания_устройства_не_в_норме; // РМК, МПТ, МРО-2М
				case 2: return isMVP ? JournalEventDescriptionType.КЗ_АЛС_2 : JournalEventDescriptionType.Оптический_канал_или_фотоусилитель; // ИЗВЕЩАТЕЛИ
				case 3: return isMVP ? JournalEventDescriptionType.КЗ_АЛС_3 : JournalEventDescriptionType.Температурный_канал; // ИЗВЕЩАТЕЛИ
				case 4: return isMVP ? JournalEventDescriptionType.КЗ_АЛС_4 : JournalEventDescriptionType.КЗ_ШС; // МПТ, АМ
				case 5: return JournalEventDescriptionType.Обрыв_ШС; // МПТ, АМ
				//case 6: return "";
				//case 7: return "";
				case 8: return JournalEventDescriptionType.Вскрытие_корпуса; // АМ, ШУН, ШУЗ, КАУ, ГК
				case 9: return JournalEventDescriptionType.Контакт_не_переключается; // РМ
				case 10: return JournalEventDescriptionType.Напряжение_запуска_реле_ниже_нормы; // РМК
				case 11: return JournalEventDescriptionType.КЗ_выхода; // РМК
				case 12: return JournalEventDescriptionType.Обрыв_выхода; // РМК
				case 13: return JournalEventDescriptionType.Напряжение_питания_ШС_ниже_нормы; // МПТ
				case 14: return JournalEventDescriptionType.Ошибка_памяти; // МПТ
				case 15: return JournalEventDescriptionType.КЗ_выхода_1; // МПТ
				case 16: return JournalEventDescriptionType.КЗ_выхода_2; // МПТ
				case 17: return JournalEventDescriptionType.КЗ_выхода_3; // МПТ
				case 18: return JournalEventDescriptionType.КЗ_выхода_4; // МПТ
				case 19: return JournalEventDescriptionType.КЗ_выхода_5; // МПТ
				case 20: return JournalEventDescriptionType.Обрыв_выхода_1; // МПТ
				case 21: return JournalEventDescriptionType.Обрыв_выхода_2; // МПТ
				case 22: return JournalEventDescriptionType.Обрыв_выхода_3; // МПТ
				case 23: return JournalEventDescriptionType.Обрыв_выхода_4; // МПТ
				case 24: return JournalEventDescriptionType.Обрыв_выхода_5; // МПТ
				case 25: return JournalEventDescriptionType.Блокировка_пуска; // МДУ
				case 26: return JournalEventDescriptionType.Низкое_напряжение_питания_привода; // МДУ
				case 27: return JournalEventDescriptionType.Обрыв_кнопки_НОРМА; // МДУ
				case 28: return JournalEventDescriptionType.КЗ_кнопки_НОРМА; // МДУ
				case 29: return JournalEventDescriptionType.Обрыв_кнопка_ЗАЩИТА; // МДУ
				case 30: return JournalEventDescriptionType.КЗ_кнопки_ЗАЩИТА; // МДУ
				case 31: return JournalEventDescriptionType.Обрыв_концевого_выключателя_ОТКРЫТО; // МДУ
				case 32: return JournalEventDescriptionType.Обрыв_концевого_выключателя_ЗАКРЫТО; // МДУ
				case 33: return JournalEventDescriptionType.Обрыв_цепи_ПД_ЗАЩИТА; // МДУ
				case 34: return JournalEventDescriptionType.Замкнуты_разомкнуты_оба_концевика; // МДУ
				case 35: return JournalEventDescriptionType.Превышение_времени_хода; // МДУ, ШУЗ
				case 36: return JournalEventDescriptionType.Обрыв_в_линии_РЕЛЕ;
				case 37: return JournalEventDescriptionType.КЗ_в_линии_РЕЛЕ;
				case 38: return JournalEventDescriptionType.Выход_1;
				case 39: return JournalEventDescriptionType.Выход_2;
				case 40: return JournalEventDescriptionType.Выход_3;
				//case 41: return "";
				case 42: return JournalEventDescriptionType.Обрыв_концевого_выключателя_ОТКРЫТО; // ШУЗ
				case 43: return JournalEventDescriptionType.КЗ_концевого_выключателя_ОТКРЫТО; // ШУЗ
				case 44: return JournalEventDescriptionType.Обрыв_муфтового_выключателя_ОТКРЫТО; // ШУЗ
				case 45: return JournalEventDescriptionType.КЗ_муфтового_выключателя_ОТКРЫТО; // ШУЗ
				case 46: return JournalEventDescriptionType.Обрыв_концевого_выключателя_ЗАКРЫТО; // ШУЗ
				case 47: return JournalEventDescriptionType.КЗ_концевого_выключателя_ЗАКРЫТО; // ШУЗ
				case 48: return JournalEventDescriptionType.Обрыв_муфтового_выключателя_ЗАКРЫТО; // ШУЗ
				case 49: return JournalEventDescriptionType.КЗ_муфтового_выключателя_ЗАКРЫТО; // ШУЗ
				case 50: return JournalEventDescriptionType.Обрыв_кнопки_Открыть_УЗЗ_Закрыть_УЗЗ; // ШУЗ
				case 51: return JournalEventDescriptionType.КЗ_кнопки_Открыть_УЗЗ_Закрыть_УЗЗ; // ШУЗ
				case 52: return JournalEventDescriptionType.Обрыв_кнопки_СТОП_УЗЗ; // ШУЗ
				case 53: return JournalEventDescriptionType.КЗ_кнопки_СТОП_УЗЗ; // ШУЗ
				case 54: return JournalEventDescriptionType.Обрыв_давление_низкое;
				case 55: return JournalEventDescriptionType.КЗ_давление_низкое;
				case 56: return JournalEventDescriptionType.Таймаут_по_давлению;
				case 57: return JournalEventDescriptionType.КВ_МВ; // ШУЗ
				case 58: return JournalEventDescriptionType.Не_задан_режим; // ШУЗ
				case 59: return JournalEventDescriptionType.Отказ_ШУЗ; // ШУЗ
				case 60: return JournalEventDescriptionType.ДУ_ДД; // ШУН
				case 61: return JournalEventDescriptionType.Обрыв_входа_1; // ШУН
				case 62: return JournalEventDescriptionType.КЗ_входа_1; // ШУН
				case 63: return JournalEventDescriptionType.Обрыв_входа_2; // ШУН
				case 64: return JournalEventDescriptionType.КЗ_входа_2; // ШУН
				case 65: return JournalEventDescriptionType.Обрыв_входа_3; // ШУН
				case 66: return JournalEventDescriptionType.КЗ_входа_3; // ШУН
				case 67: return JournalEventDescriptionType.Обрыв_входа_4; // ШУН
				case 68: return JournalEventDescriptionType.КЗ_входа_4; // ШУН
				case 69: return JournalEventDescriptionType.Не_задан_тип; // ШУН
				case 70: return JournalEventDescriptionType.Отказ_ПН; // ШУН
				case 71: return JournalEventDescriptionType.Отказ_ШУН; // ШУН
				case 72: return JournalEventDescriptionType.Питание_1; // КАУ, ГК
				case 73: return JournalEventDescriptionType.Питание_2; // КАУ, ГК
				case 74: return JournalEventDescriptionType.Отказ_АЛС_1_или_2; // КАУ
				case 75: return JournalEventDescriptionType.Отказ_АЛС_3_или_4; // КАУ
				case 76: return JournalEventDescriptionType.Отказ_АЛС_5_или_6; // КАУ
				case 77: return JournalEventDescriptionType.Отказ_АЛС_7_или_8; // КАУ
				case 78: return JournalEventDescriptionType.Обрыв_цепи_ПД_НОРМА; // МДУ
				case 79: return JournalEventDescriptionType.КЗ_АЛС_1; // КАУ
				case 80: return JournalEventDescriptionType.КЗ_АЛС_2; // КАУ
				case 81: return JournalEventDescriptionType.КЗ_АЛС_3; // КАУ
				case 82: return JournalEventDescriptionType.КЗ_АЛС_4; // КАУ
				case 83: return JournalEventDescriptionType.КЗ_АЛС_5; // КАУ
				case 84: return JournalEventDescriptionType.КЗ_АЛС_6; // КАУ
				case 85: return JournalEventDescriptionType.КЗ_АЛС_7; // КАУ
				case 86: return JournalEventDescriptionType.КЗ_АЛС_8; // КАУ
				case 87: return JournalEventDescriptionType.Истекло_время_вкл;
				case 88: return JournalEventDescriptionType.Истекло_время_выкл;
				case 89: return JournalEventDescriptionType.Контакт_реле_1;
				case 90: return JournalEventDescriptionType.Контакт_реле_2;
				case 91: return JournalEventDescriptionType.Обрыв_кнопки_ПУСК; // МРО-2М
				case 92: return JournalEventDescriptionType.КЗ_кнопки_ПУСК; // МРО-2М
				case 93: return JournalEventDescriptionType.Обрыв_кнопки_СТОП; // МРО-2М
				case 94: return JournalEventDescriptionType.КЗ_кнопки_СТОП; // МРО-2М
				case 95: return JournalEventDescriptionType.Отсутствуют_или_испорчены_сообщения_для_воспроизведения; // МРО-2М
				case 96: return JournalEventDescriptionType.Выход;
				case 97: return JournalEventDescriptionType.Обрыв_Низкий_уровень; // ППУ
				case 98: return JournalEventDescriptionType.КЗ_Низкий_уровень; // ППУ
				case 99: return JournalEventDescriptionType.Обрыв_Высокий_уровень; // ППУ
				case 100: return JournalEventDescriptionType.КЗ_Высокий_уровень; // ППУ
				case 101: return JournalEventDescriptionType.Обрыв_Аварийный_уровень; // ППУ
				case 102: return JournalEventDescriptionType.КЗ_Аварийный_уровень; // ППУ
				case 103: return JournalEventDescriptionType.Аварийный_уровень; // ППУ
				case 104: return JournalEventDescriptionType.Питание_силовое; // ППУ
				case 105: return JournalEventDescriptionType.Питание_контроллера; // ППУ
				case 106: return JournalEventDescriptionType.Несовместимость_сигналов; // ППУ
				case 107: return JournalEventDescriptionType.Обрыв_цепи_питания_двигателя; // ППУ
				case 108: return JournalEventDescriptionType.Обрыв_Давление_на_выходе; // ППУ
				case 109: return JournalEventDescriptionType.КЗ_Давление_на_выходе; // ППУ
				case 110: return JournalEventDescriptionType.Обрыв_ДУ_ПУСК; // ППУ
				case 111: return JournalEventDescriptionType.КЗ_ДУ_ПУСК; // ППУ
				case 112: return JournalEventDescriptionType.Обрыв_ДУ_СТОП; // ППУ
				case 113: return JournalEventDescriptionType.КЗ_ДУ_СТОП; // ППУ
				case 148: return JournalEventDescriptionType.АЛС_1_Неизвестное_устройство;
				case 149: return JournalEventDescriptionType.АЛС_2_Неизвестное_устройство;
				case 150: return JournalEventDescriptionType.АЛС_3_Неизвестное_устройство;
				case 151: return JournalEventDescriptionType.АЛС_4_Неизвестное_устройство;
				case 152: return JournalEventDescriptionType.АЛС_5_Неизвестное_устройство;
				case 153: return JournalEventDescriptionType.АЛС_6_Неизвестное_устройство;
				case 154: return JournalEventDescriptionType.АЛС_7_Неизвестное_устройство;
				case 155: return JournalEventDescriptionType.АЛС_8_Неизвестное_устройство;
				case 156: return JournalEventDescriptionType.АЛС_1_Неизвестный_тип_устройства;
				case 157: return JournalEventDescriptionType.АЛС_2_Неизвестный_тип_устройства;
				case 158: return JournalEventDescriptionType.АЛС_3_Неизвестный_тип_устройства;
				case 159: return JournalEventDescriptionType.АЛС_4_Неизвестный_тип_устройства;
				case 160: return JournalEventDescriptionType.АЛС_5_Неизвестный_тип_устройства;
				case 161: return JournalEventDescriptionType.АЛС_6_Неизвестный_тип_устройства;
				case 162: return JournalEventDescriptionType.АЛС_7_Неизвестный_тип_устройства;
				case 163: return JournalEventDescriptionType.АЛС_8_Неизвестный_тип_устройства;
				case 164: return JournalEventDescriptionType.АЛС_1_Другой_тип_устройства;
				case 165: return JournalEventDescriptionType.АЛС_2_Другой_тип_устройства;
				case 166: return JournalEventDescriptionType.АЛС_3_Другой_тип_устройства;
				case 167: return JournalEventDescriptionType.АЛС_4_Другой_тип_устройства;
				case 168: return JournalEventDescriptionType.АЛС_5_Другой_тип_устройства;
				case 169: return JournalEventDescriptionType.АЛС_6_Другой_тип_устройства;
				case 170: return JournalEventDescriptionType.АЛС_7_Другой_тип_устройства;
				case 171: return JournalEventDescriptionType.АЛС_8_Другой_тип_устройства;
				case 172: return JournalEventDescriptionType.Контактор_Открыть;
				case 173: return JournalEventDescriptionType.Контактор_Закрыть;
				case 174: return JournalEventDescriptionType.Обрыв_ДУ_Открыть;
				case 175: return JournalEventDescriptionType.КЗ_ДУ_Открыть;
				case 176: return JournalEventDescriptionType.Обрыв_ДУ_Закрыть;
				case 177: return JournalEventDescriptionType.КЗ_ДУ_Закрыть;
				case 178: return JournalEventDescriptionType.Обрыв_ОГВ;
				case 179: return JournalEventDescriptionType.КЗ_ОГВ;
				case 180: return JournalEventDescriptionType.Истекло_Время_Хода;
				case 181: return JournalEventDescriptionType.Сигнал_МВ_без_КВ;
				case 182: return JournalEventDescriptionType.Сочетание_КВ;
				case 183: return JournalEventDescriptionType.Сочетание_МВ;
				case 184: return JournalEventDescriptionType.Сочетание_ДНУ_и_ДВУ;
				case 186: return JournalEventDescriptionType.Обрыв_концевого_выключателя_ЗАЩИТА;
				case 187: return JournalEventDescriptionType.КЗ_концевого_выключателя_ЗАЩИТА;
				case 188: return JournalEventDescriptionType.Обрыв_концевого_выключателя_НОРМА;
				case 189: return JournalEventDescriptionType.КЗ_концевого_выключателя_НОРМА;
				case 241: return JournalEventDescriptionType.Обрыв_АЛС_1_2;
				case 242: return JournalEventDescriptionType.Обрыв_АЛС_3_4;
				case 243: return JournalEventDescriptionType.Обрыв_АЛС_5_6;
				case 244: return JournalEventDescriptionType.Обрыв_АЛС_7_8;
				case 245: return JournalEventDescriptionType.Обрыв_АЛС_1;
				case 246: return JournalEventDescriptionType.Обрыв_АЛС_2;
				case 247: return JournalEventDescriptionType.Обрыв_АЛС_3;
				case 248: return JournalEventDescriptionType.Обрыв_АЛС_4;
				case 249: return JournalEventDescriptionType.Обрыв_АЛС_5;
				case 250: return JournalEventDescriptionType.Обрыв_АЛС_6;
				case 251: return JournalEventDescriptionType.Обрыв_АЛС_7;
				case 252: return JournalEventDescriptionType.Обрыв_АЛС_8;
				case 253: return JournalEventDescriptionType.ОЛС; // ГК
				case 254: return JournalEventDescriptionType.РЛС; // ГК
				case 255: return JournalEventDescriptionType.Потеря_связи;
			}
			return JournalEventDescriptionType.NULL;
		}

		//public static string ToBUSHFailure(byte b)
		//{
		//	switch (b)
		//	{
		//		//case 0: return "Вскрытие"; //
		//		case 1: return "Неисправность контакта"; //
		//		//case 2: return "Обрыв Низкий уровень"; //
		//		//case 3: return "КЗ Низкий уровень"; //
		//		//case 4: return "Обрыв Высокий уровень"; //
		//		//case 5: return "КЗ Высокий уровень"; //
		//		//case 6: return "Обрыв Аварийный уровень"; //
		//		//case 7: return "КЗ Аварийный уровень"; //
		//		//case 8: return "Аварийный уровень"; //
		//		//case 9: return "Питание силовое"; //
		//		//case 10: return "Питание контроллера"; //
		//		//case 11: return "Несовместимость сигналов"; //
		//		//case 12: return "Обрыв цепи питания двигателя"; //
		//		case 13: return "Обрыв Давление низкое"; //
		//		case 14: return "КЗ Давление низкое"; // 
		//		case 15: return "Таймаут по давлению"; //
		//		//case 16: return "Обрыв Давление на выходе"; //
		//		//case 17: return "КЗ Давление на выходе"; //
		//		//case 18: return "Обрыв ДУ ПУСК"; //
		//		//case 19: return "КЗ ДУ ПУСК"; //
		//		//case 20: return "Обрыв ДУ СТОП"; //
		//		//case 21: return "КЗ ДУ СТОП"; //
		//		//case 255: return "Потеря связи"; //
		//	}
		//	return "";
		//}

		public static JournalEventDescriptionType ToBatteryFailure(byte b)
		{
			switch (b)
			{
				case 1: return JournalEventDescriptionType.Отсутствие_сетевого_напряжения;//
				case 2: return JournalEventDescriptionType.Выход_1;
				case 3: return JournalEventDescriptionType.КЗ_Выхода_1;//
				case 4: return JournalEventDescriptionType.Перегрузка_Выхода_1;//
				case 5: return JournalEventDescriptionType.Напряжение_Выхода_1_выше_нормы;//
				case 6: return JournalEventDescriptionType.Выход_2;
				case 7: return JournalEventDescriptionType.КЗ_Выхода_2;//
				case 8: return JournalEventDescriptionType.Перегрузка_Выхода_2;//
				case 9: return JournalEventDescriptionType.Напряжение_Выхода_2_выше_нормы;//
				case 10: return JournalEventDescriptionType.АКБ_1;
				case 11: return JournalEventDescriptionType.АКБ_1_Разряд;//
				case 12: return JournalEventDescriptionType.АКБ_1_Глубокий_Разряд;//
				case 13: return JournalEventDescriptionType.АКБ_1_Отсутствие;//
				case 14: return JournalEventDescriptionType.АКБ_2;
				case 15: return JournalEventDescriptionType.АКБ_2_Разряд;//
				case 16: return JournalEventDescriptionType.АКБ_2_Глубокий_Разряд;//
				case 17: return JournalEventDescriptionType.АКБ_2_Отсутствие;//
				case 255: return JournalEventDescriptionType.Потеря_связи;//
			}
			return JournalEventDescriptionType.NULL;
		}

		public static JournalEventDescriptionType ToInformation(byte b)
		{
			switch (b)
			{
				case 1: return JournalEventDescriptionType.Команда_от_прибора;
				case 2: return JournalEventDescriptionType.Команда_от_кнопки;
				case 3: return JournalEventDescriptionType.Изменение_автоматики_по_неисправности;
				case 4: return JournalEventDescriptionType.Изменение_автомат_по_СТОП;
				case 5: return JournalEventDescriptionType.Изменение_автоматики_по_Д_О;
				case 6: return JournalEventDescriptionType.Изменение_автоматики_по_ТМ;
				case 7: return JournalEventDescriptionType.Отсчет_задержки_2;
				case 8: return JournalEventDescriptionType.Отлож_пуск_АУП_Д_О;
				case 9: return JournalEventDescriptionType.Пуск_АУП_завершен;
				case 10: return JournalEventDescriptionType.Стоп_по_кнопке_СТОП;
				case 11: return JournalEventDescriptionType.Программирование_мастер_ключа;
				case 12: return JournalEventDescriptionType.Датчик_ДАВЛЕНИЕ;
				case 13: return JournalEventDescriptionType.Датчик_МАССА;
				case 14: return JournalEventDescriptionType.Сигнал_из_памяти;
				case 15: return JournalEventDescriptionType.Сигнал_аналог_входа;
				case 16: return JournalEventDescriptionType.Замена_списка_на_1;
				case 17: return JournalEventDescriptionType.Замена_списка_на_2;
				case 18: return JournalEventDescriptionType.Замена_списка_на_3;
				case 19: return JournalEventDescriptionType.Замена_списка_на_4;
				case 20: return JournalEventDescriptionType.Замена_списка_на_5;
				case 21: return JournalEventDescriptionType.Замена_списка_на_6;
				case 22: return JournalEventDescriptionType.Замена_списка_на_7;
				case 23: return JournalEventDescriptionType.Замена_списка_на_8;
				case 24: return JournalEventDescriptionType.Низкий_уровень;
				case 25: return JournalEventDescriptionType.Высокий_уровень;
				case 26: return JournalEventDescriptionType.Уровень_норма;
				case 27: return JournalEventDescriptionType.Перевод_в_автоматический_режим_со_шкафа;
				case 28: return JournalEventDescriptionType.Перевод_в_ручной_режим_со_шкафа;
				case 29: return JournalEventDescriptionType.Перевод_в_отключенный_режим_со_шкафа;
				case 30: return JournalEventDescriptionType.Неопределено;
				case 31: return JournalEventDescriptionType.Пуск_невозможен;
				case 32: return JournalEventDescriptionType.Авария_пневмоемкости;
				case 33: return JournalEventDescriptionType.Аварийный_уровень_Информация;
				case 34: return JournalEventDescriptionType.Запрет_пуска_НС;
				case 35: return JournalEventDescriptionType.Запрет_пуска_компрессора;
				case 36: return JournalEventDescriptionType.Ввод_1;
				case 37: return JournalEventDescriptionType.Ввод_2;
				case 38: return JournalEventDescriptionType.Команда_от_логики;
				case 39: return JournalEventDescriptionType.Команда_от_ДУ;
				case 40: return JournalEventDescriptionType.Давление_низкое;
				case 41: return JournalEventDescriptionType.Давление_высокое;
				case 42: return JournalEventDescriptionType.Давление_норма;
				case 43: return JournalEventDescriptionType.Давление_неопределен;
				case 44: return JournalEventDescriptionType.Сигнал_с_ДВнР_есть;
				case 45: return JournalEventDescriptionType.Сигнал_с_ДВнР_нет;
				case 46: return JournalEventDescriptionType.ШУ_вышел_на_режим;
				case 47: return JournalEventDescriptionType.Блокировка_пуска_2;
				case 48: return JournalEventDescriptionType.Блокировка_пуска_снята;
				case 49: return JournalEventDescriptionType.Отсчет_задержки_2;
				case 50: return JournalEventDescriptionType.Аварии_пневмоемкости_нет;
				case 51: return JournalEventDescriptionType.Пуск_с_УЗ;
				case 52: return JournalEventDescriptionType.Пуск_автоматический;
				case 53: return JournalEventDescriptionType.Пуск_ручной;
				case 54: return JournalEventDescriptionType.Пуск_с_панели_шкафа;
				case 55: return JournalEventDescriptionType.Стоп_от_переключателя;
				case 56: return JournalEventDescriptionType.Стоп_с_панели_шкафа;
				case 57: return JournalEventDescriptionType.Стоп_по_неисправности;
				case 58: return JournalEventDescriptionType.Стоп_с_УЗ;
				case 59: return JournalEventDescriptionType.Стоп_автоматический;
				case 60: return JournalEventDescriptionType.Стоп_ручной;
				case 61: return JournalEventDescriptionType.Отсчет_задержки_включен;
				case 62: return JournalEventDescriptionType.Отсчет_задержки_выключен;
				case 63: return JournalEventDescriptionType.Откр_с_УЗ;
				case 64: return JournalEventDescriptionType.Откр_автоматический;
				case 65: return JournalEventDescriptionType.Откр_ручной;
				case 66: return JournalEventDescriptionType.Откр_с_панели_шкафа;
				case 67: return JournalEventDescriptionType.Закр_от_переключателя;
				case 68: return JournalEventDescriptionType.Закр_с_панели_шкафа;
				case 69: return JournalEventDescriptionType.Закр_по_неисправности;
				case 70: return JournalEventDescriptionType.Закр_с_УЗ;
				case 71: return JournalEventDescriptionType.Закр_автоматический;
				case 72: return JournalEventDescriptionType.Закр_ручной;
				case 73: return JournalEventDescriptionType.Старт_ОГВ;
				case 74: return JournalEventDescriptionType.Стоп_ОГВ;
				case 75: return JournalEventDescriptionType.Команда_с_панели_шкафа;
				case 76: return JournalEventDescriptionType.Ход_на_открытие;
				case 77: return JournalEventDescriptionType.Ход_на_закрытие;
				case 78: return JournalEventDescriptionType.Уровень_неопределен;
			}
			return JournalEventDescriptionType.NULL;
		}

		public static JournalEventDescriptionType ToValveInformation(byte b)
		{
			switch (b)
			{
				case 7: return JournalEventDescriptionType.Отсчет_задержки_2;
				case 30: return JournalEventDescriptionType.Остановлено;
			}
			return ToInformation(b);
		}

		public static JournalEventDescriptionType ToUser(byte b)
		{
			switch (b)
			{
				case 1: return JournalEventDescriptionType.Оператор;
				case 2: return JournalEventDescriptionType.Администратор;
				case 3: return JournalEventDescriptionType.Инсталлятор;
				case 4: return JournalEventDescriptionType.Изготовитель;
			}
			return JournalEventDescriptionType.NULL;
		}

		public static string GetPumpFailureMessage(string name, int pumpType)
		{
			switch (name)
			{
				case "Обрыв входа 1":
					if (pumpType == 0)
						return "Обрыв линии ЭКМ на входе насоса";
					if (pumpType == 1)
						return "Обрыв линии связи с датчиком низкого давления";
					if (pumpType == 2)
						return "Обрыв линии связи с датчиком верхнего уровня";
					break;
				case "КЗ входа 1":
					if (pumpType == 0)
						return "КЗ линии ЭКМ на входе насоса";
					if (pumpType == 1)
						return "КЗ линии связи с датчиком низкого давления";
					if (pumpType == 2)
						return "КЗ линии связи с датчиком верхнего уровня";
					break;

				case "Обрыв входа 2":
					if (pumpType == 0)
						return "Обрыв линии дистанционного управления";
					if (pumpType == 1)
						return "Обрыв линии связи с датчиком высокого давления";
					if (pumpType == 2)
						return "Обрыв линии связи с датчиком нижнего уровня";
					break;

				case "КЗ входа 2":
					if (pumpType == 0)
						return "КЗ линии дистанционного управления";
					if (pumpType == 1)
						return "КЗ линии связи с датчиком высокого давления";
					if (pumpType == 2)
						return "КЗ линии связи с датчиком нижнего уровня";
					break;

				case "Обрыв входа 3":
					if (pumpType == 0)
						return null;
					if (pumpType == 2)
						return "Обрыв линии связи с датчиком аварийного уровня";
					break;
				case "КЗ входа 3":
					if (pumpType == 0)
						return null;
					if (pumpType == 2)
						return "КЗ линии связи с датчиком аварийного уровня";
					break;

				case "Обрыв входа 4":
				case "КЗ входа 4":
					break;
			}
			return name;
		}
	}
}