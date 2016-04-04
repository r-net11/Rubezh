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

		public static JournalEventDescriptionType ToFailure(byte b, bool isFailure, bool isMVP = false)
		{
			switch (b)
			{
				case 1:
					if (isMVP)
						return isFailure ? JournalEventDescriptionType.КЗ_АЛС_1_Неисправность : JournalEventDescriptionType.КЗ_АЛС_1_Неисправность_устранена;
					else
						return isFailure ? JournalEventDescriptionType.Напряжение_питания_устройства_не_в_норме_Неисправность : JournalEventDescriptionType.Неисправность_устраненаНапряжение_питания_устройства_не_в_норме_Неисправность_устранена;
				case 2:
					if (isMVP)
						return isFailure ? JournalEventDescriptionType.КЗ_АЛС_2_Неисправность : JournalEventDescriptionType.КЗ_АЛС_2_Неисправность_устранена;
					else
						return isFailure ? JournalEventDescriptionType.Оптический_канал_или_фотоусилитель_Неисправность : JournalEventDescriptionType.Оптический_канал_или_фотоусилитель_Неисправность_устранена;
				case 3:
					if (isMVP)
						return isFailure ? JournalEventDescriptionType.КЗ_АЛС_3_Неисправность : JournalEventDescriptionType.КЗ_АЛС_3_Неисправность_устранена;
					else
						return isFailure ? JournalEventDescriptionType.Температурный_канал_Неисправность : JournalEventDescriptionType.Температурный_канал_Неисправность_устранена;
				case 4:
					if (isMVP)
						return isFailure ? JournalEventDescriptionType.КЗ_АЛС_4_Неисправность : JournalEventDescriptionType.КЗ_АЛС_4_Неисправность_устранена;
					else
						return isFailure ? JournalEventDescriptionType.КЗ_ШС_Неисправность : JournalEventDescriptionType.КЗ_ШС_Неисправность_устранена;
				case 5: return isFailure ? JournalEventDescriptionType.Обрыв_ШС_Неисправность : JournalEventDescriptionType.Обрыв_ШС_Неисправность_устранена;
				//case 6: return "";
				//case 7: return "";
				case 8: return isFailure ? JournalEventDescriptionType.Вскрытие_корпуса_Неисправность : JournalEventDescriptionType.Вскрытие_корпуса_Неисправность_устранена;
				case 9: return isFailure ? JournalEventDescriptionType.Контакт_не_переключается_Неисправность : JournalEventDescriptionType.Контакт_не_переключается_Неисправность_устранена;
				case 10: return isFailure ? JournalEventDescriptionType.Напряжение_запуска_реле_ниже_нормы_Неисправность : JournalEventDescriptionType.Напряжение_запуска_реле_ниже_нормы_Неисправность_устранена;
				case 11: return isFailure ? JournalEventDescriptionType.КЗ_выхода_Неисправность : JournalEventDescriptionType.КЗ_выхода_Неисправность_устранена;
				case 12: return isFailure ? JournalEventDescriptionType.Обрыв_выхода_Неисправность : JournalEventDescriptionType.Обрыв_выхода_Неисправность_устранена;
				case 13: return isFailure ? JournalEventDescriptionType.Напряжение_питания_ШС_ниже_нормы_Неисправность : JournalEventDescriptionType.Напряжение_питания_ШС_ниже_нормы_Неисправность_устранена;
				case 14: return isFailure ? JournalEventDescriptionType.Ошибка_памяти_Неисправность : JournalEventDescriptionType.Ошибка_памяти_Неисправность_устранена;
				case 15: return isFailure ? JournalEventDescriptionType.КЗ_выхода_1_Неисправность : JournalEventDescriptionType.КЗ_выхода_1_Неисправность_устранена;
				case 16: return isFailure ? JournalEventDescriptionType.КЗ_выхода_2_Неисправность : JournalEventDescriptionType.КЗ_выхода_2_Неисправность_устранена;
				case 17: return isFailure ? JournalEventDescriptionType.КЗ_выхода_3_Неисправность : JournalEventDescriptionType.КЗ_выхода_3_Неисправность_устранена;
				case 18: return isFailure ? JournalEventDescriptionType.КЗ_выхода_4_Неисправность : JournalEventDescriptionType.КЗ_выхода_4_Неисправность_устранена; // МПТ
				case 19: return isFailure ? JournalEventDescriptionType.КЗ_выхода_5_Неисправность : JournalEventDescriptionType.КЗ_выхода_5_Неисправность_устранена; // МПТ
				case 20: return isFailure ? JournalEventDescriptionType.Обрыв_выхода_1_Неисправность : JournalEventDescriptionType.Обрыв_выхода_1_Неисправность_устранена; // МПТ
				case 21: return isFailure ? JournalEventDescriptionType.Обрыв_выхода_2_Неисправность : JournalEventDescriptionType.Обрыв_выхода_2_Неисправность_устранена; // МПТ
				case 22: return isFailure ? JournalEventDescriptionType.Обрыв_выхода_3_Неисправность : JournalEventDescriptionType.Обрыв_выхода_3_Неисправность_устранена; // МПТ
				case 23: return isFailure ? JournalEventDescriptionType.Обрыв_выхода_4_Неисправность : JournalEventDescriptionType.Обрыв_выхода_4_Неисправность_устранена; // МПТ
				case 24: return isFailure ? JournalEventDescriptionType.Обрыв_выхода_5_Неисправность : JournalEventDescriptionType.Обрыв_выхода_5_Неисправность_устранена; // МПТ
				case 25: return isFailure ? JournalEventDescriptionType.Блокировка_пуска_Неисправность : JournalEventDescriptionType.Блокировка_пуска_Неисправность_устранена; // МДУ
				case 26: return isFailure ? JournalEventDescriptionType.Низкое_напряжение_питания_привода_Неисправность : JournalEventDescriptionType.Низкое_напряжение_питания_привода_Неисправность_устранена; // МДУ
				case 27: return isFailure ? JournalEventDescriptionType.Обрыв_кнопки_НОРМА_Неисправность : JournalEventDescriptionType.Обрыв_кнопки_НОРМА_Неисправность_устранена; // МДУ
				case 28: return isFailure ? JournalEventDescriptionType.КЗ_кнопки_НОРМА_Неисправность : JournalEventDescriptionType.КЗ_кнопки_НОРМА_Неисправность_устранена; // МДУ
				case 29: return isFailure ? JournalEventDescriptionType.Обрыв_кнопка_ЗАЩИТА_Неисправность : JournalEventDescriptionType.Обрыв_кнопка_ЗАЩИТА_Неисправность_устранена; // МДУ
				case 30: return isFailure ? JournalEventDescriptionType.КЗ_кнопки_ЗАЩИТА_Неисправность : JournalEventDescriptionType.КЗ_кнопки_ЗАЩИТА_Неисправность_устранена; // МДУ
				case 31: return isFailure ? JournalEventDescriptionType.Обрыв_концевого_выключателя_ОТКРЫТО_Неисправность : JournalEventDescriptionType.Обрыв_концевого_выключателя_ОТКРЫТО_Неисправность_устранена; // МДУ
				case 32: return isFailure ? JournalEventDescriptionType.Обрыв_концевого_выключателя_ЗАКРЫТО_Неисправность : JournalEventDescriptionType.Обрыв_концевого_выключателя_ЗАКРЫТО_Неисправность_устранена; // МДУ
				case 33: return isFailure ? JournalEventDescriptionType.Обрыв_цепи_ПД_ЗАЩИТА_Неисправность : JournalEventDescriptionType.Обрыв_цепи_ПД_ЗАЩИТА_Неисправность_устранена; // МДУ
				case 34: return isFailure ? JournalEventDescriptionType.Замкнуты_разомкнуты_оба_концевика_Неисправность : JournalEventDescriptionType.Замкнуты_разомкнуты_оба_концевика_Неисправность_устранена; // МДУ
				case 35: return isFailure ? JournalEventDescriptionType.Превышение_времени_хода_Неисправность : JournalEventDescriptionType.Превышение_времени_хода_Неисправность_устранена; // МДУ, ШУЗ
				case 36: return isFailure ? JournalEventDescriptionType.Обрыв_в_линии_РЕЛЕ_Неисправность : JournalEventDescriptionType.Обрыв_в_линии_РЕЛЕ_Неисправность_устранена;
				case 37: return isFailure ? JournalEventDescriptionType.КЗ_в_линии_РЕЛЕ_Неисправность : JournalEventDescriptionType.КЗ_в_линии_РЕЛЕ_Неисправность_устранена;
				case 38: return isFailure ? JournalEventDescriptionType.Выход_1_Неисправность : JournalEventDescriptionType.Выход_1_Неисправность_устранена;
				case 39: return isFailure ? JournalEventDescriptionType.Выход_2_Неисправность : JournalEventDescriptionType.Выход_2_Неисправность_устранена;
				case 40: return isFailure ? JournalEventDescriptionType.Выход_3_Неисправность : JournalEventDescriptionType.Выход_3_Неисправность_устранена; 
				//case 41: return "";
				case 42: return isFailure ? JournalEventDescriptionType.Обрыв_концевого_выключателя_ОТКРЫТО_Неисправность : JournalEventDescriptionType.Обрыв_концевого_выключателя_ОТКРЫТО_Неисправность_устранена; // ШУЗ
				case 43: return isFailure ? JournalEventDescriptionType.КЗ_концевого_выключателя_ОТКРЫТО_Неисправность : JournalEventDescriptionType.КЗ_концевого_выключателя_ОТКРЫТО_Неисправность_устранена; // ШУЗ
				case 44: return isFailure ? JournalEventDescriptionType.Обрыв_муфтового_выключателя_ОТКРЫТО_Неисправность : JournalEventDescriptionType.Обрыв_муфтового_выключателя_ОТКРЫТО_Неисправность_устранена; // ШУЗ
				case 45: return isFailure ? JournalEventDescriptionType.КЗ_муфтового_выключателя_ОТКРЫТО_Неисправность : JournalEventDescriptionType.КЗ_муфтового_выключателя_ОТКРЫТО_Неисправность_устранена; // ШУЗ
				case 46: return isFailure ? JournalEventDescriptionType.Обрыв_концевого_выключателя_ЗАКРЫТО_Неисправность : JournalEventDescriptionType.Обрыв_концевого_выключателя_ЗАКРЫТО_Неисправность_устранена; // ШУЗ
				case 47: return isFailure ? JournalEventDescriptionType.КЗ_концевого_выключателя_ЗАКРЫТО_Неисправность : JournalEventDescriptionType.КЗ_концевого_выключателя_ЗАКРЫТО_Неисправность_устранена; // ШУЗ
				case 48: return isFailure ? JournalEventDescriptionType.Обрыв_муфтового_выключателя_ЗАКРЫТО_Неисправность : JournalEventDescriptionType.Обрыв_муфтового_выключателя_ЗАКРЫТО_Неисправность_устранена; // ШУЗ
				case 49: return isFailure ? JournalEventDescriptionType.КЗ_муфтового_выключателя_ЗАКРЫТО_Неисправность : JournalEventDescriptionType.КЗ_муфтового_выключателя_ЗАКРЫТО_Неисправность_устранена; // ШУЗ
				case 50: return isFailure ? JournalEventDescriptionType.Обрыв_кнопки_Открыть_УЗЗ_Закрыть_УЗЗ_Неисправность : JournalEventDescriptionType.Обрыв_кнопки_Открыть_УЗЗ_Закрыть_УЗЗ_Неисправность_устранена; // ШУЗ
				case 51: return isFailure ? JournalEventDescriptionType.КЗ_кнопки_Открыть_УЗЗ_Закрыть_УЗЗ_Неисправность : JournalEventDescriptionType.КЗ_кнопки_Открыть_УЗЗ_Закрыть_УЗЗ_Неисправность_устранена; // ШУЗ
				case 52: return isFailure ? JournalEventDescriptionType.Обрыв_кнопки_СТОП_УЗЗ_Неисправность : JournalEventDescriptionType.Обрыв_кнопки_СТОП_УЗЗ_Неисправность_устранена; // ШУЗ
				case 53: return isFailure ? JournalEventDescriptionType.КЗ_кнопки_СТОП_УЗЗ_Неисправность : JournalEventDescriptionType.КЗ_кнопки_СТОП_УЗЗ_Неисправность_устранена; // ШУЗ
				case 54: return isFailure ? JournalEventDescriptionType.Обрыв_давление_низкое_Неисправность : JournalEventDescriptionType.Обрыв_давление_низкое_Неисправность_устранена;
				case 55: return isFailure ? JournalEventDescriptionType.КЗ_давление_низкое_Неисправность : JournalEventDescriptionType.КЗ_давление_низкое_Неисправность_устранена;
				case 56: return isFailure ? JournalEventDescriptionType.Таймаут_по_давлению_Неисправность : JournalEventDescriptionType.Таймаут_по_давлению_Неисправность_устранена;
				case 57: return isFailure ? JournalEventDescriptionType.КВ_МВ_Неисправность : JournalEventDescriptionType.КВ_МВ_Неисправность_устранена;  // ШУЗ
				case 58: return isFailure ? JournalEventDescriptionType.Не_задан_режим_Неисправность : JournalEventDescriptionType.Не_задан_режим_Неисправность_устранена;  // ШУЗ
				case 59: return isFailure ? JournalEventDescriptionType.Отказ_ШУЗ_Неисправность : JournalEventDescriptionType.Отказ_ШУЗ_Неисправность_устранена;  // ШУЗ
				case 60: return isFailure ? JournalEventDescriptionType.ДУ_ДД_Неисправность : JournalEventDescriptionType.ДУ_ДД_Неисправность_устранена;  // ШУЗ
				case 61: return isFailure ? JournalEventDescriptionType.Обрыв_входа_1_Неисправность : JournalEventDescriptionType.Обрыв_входа_1_Неисправность_устранена;  // ШУН
				case 62: return isFailure ? JournalEventDescriptionType.КЗ_входа_1_Неисправность : JournalEventDescriptionType.КЗ_входа_1_Неисправность_устранена;  // ШУН
				case 63: return isFailure ? JournalEventDescriptionType.Обрыв_входа_2_Неисправность : JournalEventDescriptionType.Обрыв_входа_2_Неисправность_устранена;  // ШУН
				case 64: return isFailure ? JournalEventDescriptionType.КЗ_входа_2_Неисправность : JournalEventDescriptionType.КЗ_входа_2_Неисправность_устранена;  // ШУН
				case 65: return isFailure ? JournalEventDescriptionType.Обрыв_входа_3_Неисправность : JournalEventDescriptionType.Обрыв_входа_3_Неисправность_устранена;  // ШУН
				case 66: return isFailure ? JournalEventDescriptionType.КЗ_входа_3_Неисправность : JournalEventDescriptionType.КЗ_входа_3_Неисправность_устранена;  // ШУН
				case 67: return isFailure ? JournalEventDescriptionType.Обрыв_входа_4_Неисправность : JournalEventDescriptionType.Обрыв_входа_4_Неисправность_устранена;  // ШУН
				case 68: return isFailure ? JournalEventDescriptionType.КЗ_входа_4_Неисправность : JournalEventDescriptionType.КЗ_входа_4_Неисправность_устранена;  // ШУН
				case 69: return isFailure ? JournalEventDescriptionType.Не_задан_тип_Неисправность : JournalEventDescriptionType.Не_задан_тип_Неисправность_устранена;  // ШУН
				case 70: return isFailure ? JournalEventDescriptionType.Отказ_ПН_Неисправность : JournalEventDescriptionType.Отказ_ПН_Неисправность_устранена;  // ШУН
				case 71: return isFailure ? JournalEventDescriptionType.Отказ_ШУН_Неисправность : JournalEventDescriptionType.Отказ_ШУН_Неисправность_устранена;  // ШУН
				case 72: return isFailure ? JournalEventDescriptionType.Питание_1_Неисправность : JournalEventDescriptionType.Питание_1_Неисправность_устранена;  // КАУ, ГК
				case 73: return isFailure ? JournalEventDescriptionType.Питание_2_Неисправность : JournalEventDescriptionType.Питание_2_Неисправность_устранена;  // КАУ, ГК
				case 74: return isFailure ? JournalEventDescriptionType.Отказ_АЛС_1_или_2_Неисправность : JournalEventDescriptionType.Отказ_АЛС_1_или_2_Неисправность_устранена;  // КАУ
				case 75: return isFailure ? JournalEventDescriptionType.Отказ_АЛС_3_или_4_Неисправность : JournalEventDescriptionType.Отказ_АЛС_3_или_4_Неисправность_устранена;  // КАУ
				case 76: return isFailure ? JournalEventDescriptionType.Отказ_АЛС_5_или_6_Неисправность : JournalEventDescriptionType.Отказ_АЛС_5_или_6_Неисправность_устранена;  // КАУ
				case 77: return isFailure ? JournalEventDescriptionType.Отказ_АЛС_7_или_8_Неисправность : JournalEventDescriptionType.Отказ_АЛС_7_или_8_Неисправность_устранена;  // КАУ
				case 78: return isFailure ? JournalEventDescriptionType.Обрыв_цепи_ПД_НОРМА_Неисправность : JournalEventDescriptionType.Обрыв_цепи_ПД_НОРМА_Неисправность_устранена;  // МДУ
				case 79: return isFailure ? JournalEventDescriptionType.КЗ_АЛС_1_Неисправность : JournalEventDescriptionType.КЗ_АЛС_1_Неисправность_устранена; // КАУ
				case 80: return isFailure ? JournalEventDescriptionType.КЗ_АЛС_2_Неисправность : JournalEventDescriptionType.КЗ_АЛС_2_Неисправность_устранена; // КАУ
				case 81: return isFailure ? JournalEventDescriptionType.КЗ_АЛС_3_Неисправность : JournalEventDescriptionType.КЗ_АЛС_3_Неисправность_устранена; // КАУ
				case 82: return isFailure ? JournalEventDescriptionType.КЗ_АЛС_4_Неисправность : JournalEventDescriptionType.КЗ_АЛС_4_Неисправность_устранена; // КАУ
				case 83: return isFailure ? JournalEventDescriptionType.КЗ_АЛС_5_Неисправность : JournalEventDescriptionType.КЗ_АЛС_5_Неисправность_устранена; // КАУ
				case 84: return isFailure ? JournalEventDescriptionType.КЗ_АЛС_6_Неисправность : JournalEventDescriptionType.КЗ_АЛС_6_Неисправность_устранена; // КАУ
				case 85: return isFailure ? JournalEventDescriptionType.КЗ_АЛС_7_Неисправность : JournalEventDescriptionType.КЗ_АЛС_7_Неисправность_устранена; // КАУ
				case 86: return isFailure ? JournalEventDescriptionType.КЗ_АЛС_8_Неисправность : JournalEventDescriptionType.КЗ_АЛС_8_Неисправность_устранена; // КАУ
				case 87: return isFailure ? JournalEventDescriptionType.Истекло_время_вкл_Неисправность : JournalEventDescriptionType.Истекло_время_вкл_Неисправность_устранена;
				case 88: return isFailure ? JournalEventDescriptionType.Истекло_время_выкл_Неисправность : JournalEventDescriptionType.Истекло_время_выкл_Неисправность_устранена;
				case 89: return isFailure ? JournalEventDescriptionType.Контакт_реле_1_Неисправность : JournalEventDescriptionType.Контакт_реле_1_Неисправность_устранена;
				case 90: return isFailure ? JournalEventDescriptionType.Контакт_реле_2_Неисправность : JournalEventDescriptionType.Контакт_реле_2_Неисправность_устранена;
				case 91: return isFailure ? JournalEventDescriptionType.Обрыв_кнопки_ПУСК_Неисправность : JournalEventDescriptionType.Обрыв_кнопки_ПУСК_Неисправность_устранена; // МРО-2М
				case 92: return isFailure ? JournalEventDescriptionType.КЗ_кнопки_ПУСК_Неисправность : JournalEventDescriptionType.КЗ_кнопки_ПУСК_Неисправность_устранена; // МРО-2М
				case 93: return isFailure ? JournalEventDescriptionType.Обрыв_кнопки_СТОП_Неисправность : JournalEventDescriptionType.Обрыв_кнопки_СТОП_Неисправность_устранена; // МРО-2М
				case 94: return isFailure ? JournalEventDescriptionType.КЗ_кнопки_СТОП_Неисправность : JournalEventDescriptionType.КЗ_кнопки_СТОП_Неисправность_устранена; // МРО-2М
				case 95: return isFailure ? JournalEventDescriptionType.Отсутствуют_или_испорчены_сообщения_для_воспроизведения_Неисправность : JournalEventDescriptionType.Отсутствуют_или_испорчены_сообщения_для_воспроизведения_Неисправность_устранена; // МРО-2М
				case 96: return isFailure ? JournalEventDescriptionType.Выход_Неисправность : JournalEventDescriptionType.Выход_Неисправность_устранена;
				case 97: return isFailure ? JournalEventDescriptionType.Обрыв_Низкий_уровень_Неисправность : JournalEventDescriptionType.Обрыв_Низкий_уровень_Неисправность_устранена; // ППУ
				case 98: return isFailure ? JournalEventDescriptionType.КЗ_Низкий_уровень_Неисправность : JournalEventDescriptionType.КЗ_Низкий_уровень_Неисправность_устранена; // ППУ
				case 99: return isFailure ? JournalEventDescriptionType.Обрыв_Высокий_уровень_Неисправность : JournalEventDescriptionType.Обрыв_Высокий_уровень_Неисправность_устранена; // ППУ
				case 100: return isFailure ? JournalEventDescriptionType.КЗ_Высокий_уровень_Неисправность : JournalEventDescriptionType.КЗ_Высокий_уровень_Неисправность_устранена; // ППУ
				case 101: return isFailure ? JournalEventDescriptionType.Обрыв_Аварийный_уровень_Неисправность : JournalEventDescriptionType.Обрыв_Аварийный_уровень_Неисправность_устранена; // ППУ
				case 102: return isFailure ? JournalEventDescriptionType.КЗ_Аварийный_уровень_Неисправность : JournalEventDescriptionType.КЗ_Аварийный_уровень_Неисправность_устранена; // ППУ
				case 103: return isFailure ? JournalEventDescriptionType.Аварийный_уровень_Неисправность : JournalEventDescriptionType.Аварийный_уровень_Неисправность_устранена; // ППУ
				case 104: return isFailure ? JournalEventDescriptionType.Питание_силовое_Неисправность : JournalEventDescriptionType.Питание_силовое_Неисправность_устранена; // ППУ
				case 105: return isFailure ? JournalEventDescriptionType.Питание_контроллера_Неисправность : JournalEventDescriptionType.Питание_контроллера_Неисправность_устранена; // ППУ
				case 106: return isFailure ? JournalEventDescriptionType.Несовместимость_сигналов_Неисправность : JournalEventDescriptionType.Несовместимость_сигналов_Неисправность_устранена; // ППУ
				case 107: return isFailure ? JournalEventDescriptionType.Обрыв_цепи_питания_двигателя_Неисправность : JournalEventDescriptionType.Обрыв_цепи_питания_двигателя_Неисправность_устранена; // ППУ
				case 108: return isFailure ? JournalEventDescriptionType.Обрыв_Давление_на_выходе_Неисправность : JournalEventDescriptionType.Обрыв_Давление_на_выходе_Неисправность_устранена; // ППУ
				case 109: return isFailure ? JournalEventDescriptionType.КЗ_Давление_на_выходе_Неисправность : JournalEventDescriptionType.КЗ_Давление_на_выходе_Неисправность_устранена; // ППУ
				case 110: return isFailure ? JournalEventDescriptionType.Обрыв_ДУ_ПУСК_Неисправность : JournalEventDescriptionType.Обрыв_ДУ_ПУСК_Неисправность_устранена; // ППУ
				case 111: return isFailure ? JournalEventDescriptionType.КЗ_ДУ_ПУСК_Неисправность : JournalEventDescriptionType.КЗ_ДУ_ПУСК_Неисправность_устранена; // ППУ
				case 112: return isFailure ? JournalEventDescriptionType.Обрыв_ДУ_СТОП_Неисправность : JournalEventDescriptionType.Обрыв_ДУ_СТОП_Неисправность_устранена; // ППУ
				case 113: return isFailure ? JournalEventDescriptionType.КЗ_ДУ_СТОП_Неисправность : JournalEventDescriptionType.КЗ_ДУ_СТОП_Неисправность_устранена; // ППУ
				case 148: return isFailure ? JournalEventDescriptionType.АЛС_1_Неизвестное_устройство_Неисправность : JournalEventDescriptionType.АЛС_1_Неизвестное_устройство_Неисправность_устранена;
				case 149: return isFailure ? JournalEventDescriptionType.АЛС_2_Неизвестное_устройство_Неисправность : JournalEventDescriptionType.АЛС_2_Неизвестное_устройство_Неисправность_устранена;
				case 150: return isFailure ? JournalEventDescriptionType.АЛС_3_Неизвестное_устройство_Неисправность : JournalEventDescriptionType.АЛС_3_Неизвестное_устройство_Неисправность_устранена;
				case 151: return isFailure ? JournalEventDescriptionType.АЛС_4_Неизвестное_устройство_Неисправность : JournalEventDescriptionType.АЛС_4_Неизвестное_устройство_Неисправность_устранена;
				case 152: return isFailure ? JournalEventDescriptionType.АЛС_5_Неизвестное_устройство_Неисправность : JournalEventDescriptionType.АЛС_5_Неизвестное_устройство_Неисправность_устранена;
				case 153: return isFailure ? JournalEventDescriptionType.АЛС_6_Неизвестное_устройство_Неисправность : JournalEventDescriptionType.АЛС_6_Неизвестное_устройство_Неисправность_устранена;
				case 154: return isFailure ? JournalEventDescriptionType.АЛС_7_Неизвестное_устройство_Неисправность : JournalEventDescriptionType.АЛС_7_Неизвестное_устройство_Неисправность_устранена;
				case 155: return isFailure ? JournalEventDescriptionType.АЛС_8_Неизвестное_устройство_Неисправность : JournalEventDescriptionType.АЛС_8_Неизвестное_устройство_Неисправность_устранена;
				case 156: return isFailure ? JournalEventDescriptionType.АЛС_1_Неизвестный_тип_устройства_Неисправность : JournalEventDescriptionType.АЛС_1_Неизвестный_тип_устройства_Неисправность_устранена;
				case 157: return isFailure ? JournalEventDescriptionType.АЛС_2_Неизвестный_тип_устройства_Неисправность : JournalEventDescriptionType.АЛС_2_Неизвестный_тип_устройства_Неисправность_устранена;
				case 158: return isFailure ? JournalEventDescriptionType.АЛС_3_Неизвестный_тип_устройства_Неисправность : JournalEventDescriptionType.АЛС_3_Неизвестный_тип_устройства_Неисправность_устранена;
				case 159: return isFailure ? JournalEventDescriptionType.АЛС_4_Неизвестный_тип_устройства_Неисправность : JournalEventDescriptionType.АЛС_4_Неизвестный_тип_устройства_Неисправность_устранена;
				case 160: return isFailure ? JournalEventDescriptionType.АЛС_5_Неизвестный_тип_устройства_Неисправность : JournalEventDescriptionType.АЛС_5_Неизвестный_тип_устройства_Неисправность_устранена;
				case 161: return isFailure ? JournalEventDescriptionType.АЛС_6_Неизвестный_тип_устройства_Неисправность : JournalEventDescriptionType.АЛС_6_Неизвестный_тип_устройства_Неисправность_устранена;
				case 162: return isFailure ? JournalEventDescriptionType.АЛС_7_Неизвестный_тип_устройства_Неисправность : JournalEventDescriptionType.АЛС_7_Неизвестный_тип_устройства_Неисправность_устранена;
				case 163: return isFailure ? JournalEventDescriptionType.АЛС_8_Неизвестный_тип_устройства_Неисправность : JournalEventDescriptionType.АЛС_8_Неизвестный_тип_устройства_Неисправность_устранена;
				case 164: return isFailure ? JournalEventDescriptionType.АЛС_1_Другой_тип_устройства_Неисправность : JournalEventDescriptionType.АЛС_1_Другой_тип_устройства_Неисправность_устранена;
				case 165: return isFailure ? JournalEventDescriptionType.АЛС_2_Другой_тип_устройства_Неисправность : JournalEventDescriptionType.АЛС_2_Другой_тип_устройства_Неисправность_устранена;
				case 166: return isFailure ? JournalEventDescriptionType.АЛС_3_Другой_тип_устройства_Неисправность : JournalEventDescriptionType.АЛС_3_Другой_тип_устройства_Неисправность_устранена;
				case 167: return isFailure ? JournalEventDescriptionType.АЛС_4_Другой_тип_устройства_Неисправность : JournalEventDescriptionType.АЛС_4_Другой_тип_устройства_Неисправность_устранена;
				case 168: return isFailure ? JournalEventDescriptionType.АЛС_5_Другой_тип_устройства_Неисправность : JournalEventDescriptionType.АЛС_5_Другой_тип_устройства_Неисправность_устранена;
				case 169: return isFailure ? JournalEventDescriptionType.АЛС_6_Другой_тип_устройства_Неисправность : JournalEventDescriptionType.АЛС_6_Другой_тип_устройства_Неисправность_устранена;
				case 170: return isFailure ? JournalEventDescriptionType.АЛС_7_Другой_тип_устройства_Неисправность : JournalEventDescriptionType.АЛС_7_Другой_тип_устройства_Неисправность_устранена;
				case 171: return isFailure ? JournalEventDescriptionType.АЛС_8_Другой_тип_устройства_Неисправность : JournalEventDescriptionType.АЛС_8_Другой_тип_устройства_Неисправность_устранена;
				case 172: return isFailure ? JournalEventDescriptionType.Контактор_Открыть_Неисправность : JournalEventDescriptionType.Контактор_Открыть_Неисправность_устранена;
				case 173: return isFailure ? JournalEventDescriptionType.Контактор_Закрыть_Неисправность : JournalEventDescriptionType.Контактор_Закрыть_Неисправность_устранена;
				case 174: return isFailure ? JournalEventDescriptionType.Обрыв_ДУ_Открыть_Неисправность : JournalEventDescriptionType.Обрыв_ДУ_Открыть_Неисправность_устранена;
				case 175: return isFailure ? JournalEventDescriptionType.КЗ_ДУ_Открыть_Неисправность : JournalEventDescriptionType.КЗ_ДУ_Открыть_Неисправность_устранена;
				case 176: return isFailure ? JournalEventDescriptionType.Обрыв_ДУ_Открыть_Неисправность : JournalEventDescriptionType.Обрыв_ДУ_Открыть_Неисправность_устранена;
				case 177: return isFailure ? JournalEventDescriptionType.КЗ_ДУ_Открыть_Неисправность : JournalEventDescriptionType.КЗ_ДУ_Открыть_Неисправность_устранена;
				case 178: return isFailure ? JournalEventDescriptionType.Обрыв_ОГВ_Неисправность : JournalEventDescriptionType.Обрыв_ОГВ_Неисправность_устранена;
				case 179: return isFailure ? JournalEventDescriptionType.КЗ_ОГВ_Неисправность : JournalEventDescriptionType.КЗ_ОГВ_Неисправность_устранена;
				case 180: return isFailure ? JournalEventDescriptionType.Истекло_Время_Хода_Неисправность : JournalEventDescriptionType.Истекло_Время_Хода_Неисправность_устранена;
				case 181: return isFailure ? JournalEventDescriptionType.Сигнал_МВ_без_КВ_Неисправность : JournalEventDescriptionType.Сигнал_МВ_без_КВ_Неисправность_устранена;
				case 182: return isFailure ? JournalEventDescriptionType.Сочетание_КВ_Неисправность : JournalEventDescriptionType.Сочетание_КВ_Неисправность_устранена;
				case 183: return isFailure ? JournalEventDescriptionType.Сочетание_МВ_Неисправность : JournalEventDescriptionType.Сочетание_МВ_Неисправность_устранена;
				case 184: return isFailure ? JournalEventDescriptionType.Сочетание_ДНУ_и_ДВУ_Неисправность : JournalEventDescriptionType.Сочетание_ДНУ_и_ДВУ_Неисправность_устранена;
				case 186: return isFailure ? JournalEventDescriptionType.Обрыв_концевого_выключателя_ЗАЩИТА_Неисправность : JournalEventDescriptionType.Обрыв_концевого_выключателя_ЗАЩИТА_Неисправность_устранена;
				case 187: return isFailure ? JournalEventDescriptionType.КЗ_концевого_выключателя_ЗАЩИТА_Неисправность : JournalEventDescriptionType.КЗ_концевого_выключателя_ЗАЩИТА_Неисправность_устранена;
				case 188: return isFailure ? JournalEventDescriptionType.Обрыв_концевого_выключателя_НОРМА_Неисправность : JournalEventDescriptionType.Обрыв_концевого_выключателя_НОРМА_Неисправность_устранена;
				case 189: return isFailure ? JournalEventDescriptionType.КЗ_концевого_выключателя_НОРМА_Неисправность : JournalEventDescriptionType.КЗ_концевого_выключателя_НОРМА_Неисправность_устранена;
				case 241: return isFailure ? JournalEventDescriptionType.Обрыв_АЛС_1_2_Неисправность : JournalEventDescriptionType.Обрыв_АЛС_1_2_Неисправность_устранена;
				case 242: return isFailure ? JournalEventDescriptionType.Обрыв_АЛС_3_4_Неисправность : JournalEventDescriptionType.Обрыв_АЛС_3_4_Неисправность_устранена;
				case 243: return isFailure ? JournalEventDescriptionType.Обрыв_АЛС_5_6_Неисправность : JournalEventDescriptionType.Обрыв_АЛС_5_6_Неисправность_устранена;
				case 244: return isFailure ? JournalEventDescriptionType.Обрыв_АЛС_7_8_Неисправность : JournalEventDescriptionType.Обрыв_АЛС_7_8_Неисправность_устранена;
				case 245: return isFailure ? JournalEventDescriptionType.Обрыв_АЛС_1_Неисправность : JournalEventDescriptionType.Обрыв_АЛС_1_Неисправность_устранена;
				case 246: return isFailure ? JournalEventDescriptionType.Обрыв_АЛС_2_Неисправность : JournalEventDescriptionType.Обрыв_АЛС_2_Неисправность_устранена;
				case 247: return isFailure ? JournalEventDescriptionType.Обрыв_АЛС_3_Неисправность : JournalEventDescriptionType.Обрыв_АЛС_3_Неисправность_устранена;
				case 248: return isFailure ? JournalEventDescriptionType.Обрыв_АЛС_4_Неисправность : JournalEventDescriptionType.Обрыв_АЛС_4_Неисправность_устранена;
				case 249: return isFailure ? JournalEventDescriptionType.Обрыв_АЛС_5_Неисправность : JournalEventDescriptionType.Обрыв_АЛС_5_Неисправность_устранена;
				case 250: return isFailure ? JournalEventDescriptionType.Обрыв_АЛС_6_Неисправность : JournalEventDescriptionType.Обрыв_АЛС_6_Неисправность_устранена;
				case 251: return isFailure ? JournalEventDescriptionType.Обрыв_АЛС_7_Неисправность : JournalEventDescriptionType.Обрыв_АЛС_7_Неисправность_устранена;
				case 252: return isFailure ? JournalEventDescriptionType.Обрыв_АЛС_8_Неисправность : JournalEventDescriptionType.Обрыв_АЛС_8_Неисправность_устранена;
				case 253: return isFailure ? JournalEventDescriptionType.ОЛС_Неисправность : JournalEventDescriptionType.ОЛС_Неисправность_устранена; // ГК
				case 254: return isFailure ? JournalEventDescriptionType.РЛС_Неисправность : JournalEventDescriptionType.РЛС_Неисправность_устранена; // ГК
				case 255: return isFailure ? JournalEventDescriptionType.Потеря_связи_Неисправность : JournalEventDescriptionType.Потеря_связи_Неисправность_устранена;
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

		public static JournalEventDescriptionType ToBatteryFailure(byte b, bool isFailure)
		{
			switch (b)
			{
				case 1: return isFailure ? JournalEventDescriptionType.Отсутствие_сетевого_напряжения_Неисправность : JournalEventDescriptionType.Отсутствие_сетевого_напряжения_Неисправность_устранена;
				case 2: return isFailure ? JournalEventDescriptionType.Выход_1_Неисправность : JournalEventDescriptionType.Выход_1_Неисправность_устранена;
				case 3: return isFailure ? JournalEventDescriptionType.КЗ_Выхода_1_Неисправность : JournalEventDescriptionType.КЗ_Выхода_1_Неисправность_устранена;
				case 4: return isFailure ? JournalEventDescriptionType.Перегрузка_Выхода_1_Неисправность : JournalEventDescriptionType.Перегрузка_Выхода_1_Неисправность_устранена;
				case 5: return isFailure ? JournalEventDescriptionType.Напряжение_Выхода_1_выше_нормы_Неисправность : JournalEventDescriptionType.Напряжение_Выхода_1_выше_нормы_Неисправность_устранена;
				case 6: return isFailure ? JournalEventDescriptionType.Выход_2_Неисправность : JournalEventDescriptionType.Выход_2_Неисправность_устранена;
				case 7: return isFailure ? JournalEventDescriptionType.КЗ_Выхода_2_Неисправность : JournalEventDescriptionType.КЗ_Выхода_2_Неисправность_устранена;
				case 8: return isFailure ? JournalEventDescriptionType.Перегрузка_Выхода_2_Неисправность : JournalEventDescriptionType.Перегрузка_Выхода_2_Неисправность_устранена;
				case 9: return isFailure ? JournalEventDescriptionType.Напряжение_Выхода_2_выше_нормы_Неисправность : JournalEventDescriptionType.Напряжение_Выхода_2_выше_нормы_Неисправность_устранена;
				case 10: return isFailure ? JournalEventDescriptionType.АКБ_1_Неисправность : JournalEventDescriptionType.АКБ_1_Неисправность_устранена;
				case 11: return isFailure ? JournalEventDescriptionType.АКБ_1_Разряд_Неисправность : JournalEventDescriptionType.АКБ_1_Разряд_Неисправность_устранена;
				case 12: return isFailure ? JournalEventDescriptionType.АКБ_1_Глубокий_Разряд_Неисправность : JournalEventDescriptionType.АКБ_1_Глубокий_Разряд_Неисправность_устранена;
				case 13: return isFailure ? JournalEventDescriptionType.АКБ_1_Отсутствие_Неисправность : JournalEventDescriptionType.АКБ_1_Отсутствие_Неисправность_устранена;
				case 14: return isFailure ? JournalEventDescriptionType.АКБ_2_Неисправность : JournalEventDescriptionType.АКБ_2_Неисправность_устранена;
				case 15: return isFailure ? JournalEventDescriptionType.АКБ_2_Разряд_Неисправность : JournalEventDescriptionType.АКБ_2_Разряд_Неисправность_устранена;
				case 16: return isFailure ? JournalEventDescriptionType.АКБ_2_Глубокий_Разряд_Неисправность : JournalEventDescriptionType.АКБ_2_Глубокий_Разряд_Неисправность_устранена;
				case 17: return isFailure ? JournalEventDescriptionType.АКБ_2_Отсутствие_Неисправность : JournalEventDescriptionType.АКБ_2_Отсутствие_Неисправность_устранена;
				case 255: return isFailure ? JournalEventDescriptionType.Потеря_связи_Неисправность : JournalEventDescriptionType.АКБ_2_Отсутствие_Неисправность_устранена;
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

		public static JournalEventDescriptionType ToUser(byte b, bool isEnter)
		{
			switch (b)
			{
				case 1: return isEnter ? JournalEventDescriptionType.Оператор_Вход_пользователя_в_прибор : JournalEventDescriptionType.Оператор_Выход_пользователя_из_прибора;
				case 2: return isEnter ? JournalEventDescriptionType.Администратор_Вход_пользователя_в_прибор : JournalEventDescriptionType.Администратор_Выход_пользователя_из_прибора;
				case 3: return isEnter ? JournalEventDescriptionType.Инсталлятор_Вход_пользователя_в_прибор : JournalEventDescriptionType.Инсталлятор_Выход_пользователя_из_прибора;
				case 4: return isEnter ? JournalEventDescriptionType.Изготовитель_Вход_пользователя_в_прибор : JournalEventDescriptionType.Изготовитель_Выход_пользователя_из_прибора;
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