
namespace RubezhAPI.Journal
{
	public enum JournalEventDescriptionType
	{
		[EventDescriptionAttribute("", Journal.JournalEventNameType.NULL)]
		NULL = 0,

		#region Команда оператора
		[EventDescriptionAttribute("Остановка пуска", JournalEventNameType.Команда_оператора)]
		Остановка_пуска = 1,

		[EventDescriptionAttribute("Выключить немедленно", JournalEventNameType.Команда_оператора)]
		Выключить_немедленно = 2,

		[EventDescriptionAttribute("Выключить", JournalEventNameType.Команда_оператора)]
		Выключить = 3,

		[EventDescriptionAttribute("Включить немедленно", JournalEventNameType.Команда_оператора)]
		Включить_немедленно = 4,

		[EventDescriptionAttribute("Выключить в автоматическом режиме", JournalEventNameType.Команда_оператора)]
		Выключить_в_автоматическом_режиме = 5,

		[EventDescriptionAttribute("Выключить немедленно в автоматическом режиме", JournalEventNameType.Команда_оператора)]
		Выключить_немедленно_в_автоматическом_режиме = 6,

		[EventDescriptionAttribute("Включить", JournalEventNameType.Команда_оператора)]
		Включить = 7,

		[EventDescriptionAttribute("Включить немедленно в автоматическом режиме", JournalEventNameType.Команда_оператора)]
		Включить_немедленно_в_автоматическом_режиме = 8,

		[EventDescriptionAttribute("Включить в автоматическом режиме", JournalEventNameType.Команда_оператора)]
		Включить_в_автоматическом_режиме = 9,

		[EventDescriptionAttribute("Перевод в ручной режим", JournalEventNameType.Команда_оператора)]
		Перевод_в_ручной_режим_Команда_оператора = 10,

		[EventDescriptionAttribute("Перевод в автоматический режим", JournalEventNameType.Команда_оператора)]
		Перевод_в_автоматический_режим_Команда_оператора = 11,

		[EventDescriptionAttribute("Перевод в отключенный режим", JournalEventNameType.Команда_оператора)]
		Перевод_в_отключенный_режим = 12,

		[EventDescriptionAttribute("Сброс", JournalEventNameType.Команда_оператора)]
		Сброс_Команда_оператора = 13,
		#endregion

		#region Ошибка_при_опросе_состояний_компонентов_ГК
		[EventDescriptionAttribute("Не найдено родительское устройство ГК", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_найдено_родительское_устройство_ГК = 14,

		[EventDescriptionAttribute("Старт мониторинга", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Старт_мониторинга = 15,

		[EventDescriptionAttribute("Не совпадает хэш", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_хэш = 16,

		[EventDescriptionAttribute("Совпадает хэш", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Совпадает_хэш = 17,

		[EventDescriptionAttribute("Не совпадает количество байт в пришедшем ответе", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_количество_байт_в_пришедшем_ответе = 18,

		[EventDescriptionAttribute("Не совпадает тип устройства", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_устройства = 19,

		[EventDescriptionAttribute("Не совпадает физический адрес устройства", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_физический_адрес_устройства = 20,

		[EventDescriptionAttribute("Не совпадает адрес на контроллере", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_адрес_на_контроллере = 21,

		[EventDescriptionAttribute("Не совпадает тип для зоны", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_для_зоны = 22,

		[EventDescriptionAttribute("Не совпадает тип для направления", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_для_направления = 23,

		[EventDescriptionAttribute("Не совпадает тип для НС", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_для_НС = 24,

		[EventDescriptionAttribute("Не совпадает тип для МПТ", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_для_МПТ = 25,

		[EventDescriptionAttribute("Не совпадает тип для Задержки", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_для_Задержки = 26,

		[EventDescriptionAttribute("Не совпадает тип для ПИМ", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_для_ПИМ = 27,

		[EventDescriptionAttribute("Не совпадает тип для охранной зоны", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_для_охранной_зоны = 28,

		[EventDescriptionAttribute("Не совпадает тип для кода", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_для_кода = 29,

		[EventDescriptionAttribute("Не совпадает описание компонента", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_описание_компонента = 30,

		[EventDescriptionAttribute("Не совпадает тип для точки доступа", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_для_точки_доступа = 31,
		#endregion

		[EventDescriptionAttribute("Ручник сорван", JournalEventNameType.Сработка_2)]
		Ручник_сорван = 32,

		#region Сработка_1
		[EventDescriptionAttribute("Срабатывание по дыму", JournalEventNameType.Сработка_1)]
		Срабатывание_по_дыму = 33,

		[EventDescriptionAttribute("Срабатывание по температуре", JournalEventNameType.Сработка_1)]
		Срабатывание_по_температуре = 34,

		[EventDescriptionAttribute("Срабатывание по градиенту температуры", JournalEventNameType.Сработка_1)]
		Срабатывание_по_градиенту_температуры = 35,
		#endregion

		#region Неисправность, Неисправность устранена
		[EventDescriptionAttribute("Напряжение питания устройства не в норме", JournalEventNameType.Неисправность)]
		Напряжение_питания_устройства_не_в_норме_Неисправность = 36,

		[EventDescriptionAttribute("Напряжение питания устройства не в норме", JournalEventNameType.Неисправность_устранена)]
		Неисправность_устраненаНапряжение_питания_устройства_не_в_норме_Неисправность_устранена = 37,

		[EventDescriptionAttribute("Оптический канал или фотоусилитель", JournalEventNameType.Неисправность)]
		Оптический_канал_или_фотоусилитель_Неисправность = 38,

		[EventDescriptionAttribute("Оптический канал или фотоусилитель", JournalEventNameType.Неисправность_устранена)]
		Оптический_канал_или_фотоусилитель_Неисправность_устранена = 39,

		[EventDescriptionAttribute("Температурный канал", JournalEventNameType.Неисправность)]
		Температурный_канал_Неисправность = 40,

		[EventDescriptionAttribute("Температурный канал", JournalEventNameType.Неисправность_устранена)]
		Температурный_канал_Неисправность_устранена = 41,

		[EventDescriptionAttribute("КЗ ШС", JournalEventNameType.Неисправность)]
		КЗ_ШС_Неисправность = 42,

		[EventDescriptionAttribute("КЗ ШС", JournalEventNameType.Неисправность_устранена)]
		КЗ_ШС_Неисправность_устранена = 43,

		[EventDescriptionAttribute("Обрыв ШС", JournalEventNameType.Неисправность)]
		Обрыв_ШС_Неисправность = 44,

		[EventDescriptionAttribute("Обрыв ШС", JournalEventNameType.Неисправность_устранена)]
		Обрыв_ШС_Неисправность_устранена = 45,

		[EventDescriptionAttribute("Вскрытие корпуса", JournalEventNameType.Неисправность)]
		Вскрытие_корпуса_Неисправность = 46,

		[EventDescriptionAttribute("Вскрытие корпуса", JournalEventNameType.Неисправность_устранена)]
		Вскрытие_корпуса_Неисправность_устранена = 47,

		[EventDescriptionAttribute("Контакт не переключается", JournalEventNameType.Неисправность)]
		Контакт_не_переключается_Неисправность = 48,

		[EventDescriptionAttribute("Контакт не переключается", JournalEventNameType.Неисправность_устранена)]
		Контакт_не_переключается_Неисправность_устранена = 49,

		[EventDescriptionAttribute("Напряжение запуска реле ниже нормы", JournalEventNameType.Неисправность)]
		Напряжение_запуска_реле_ниже_нормы_Неисправность = 50,

		[EventDescriptionAttribute("Напряжение запуска реле ниже нормы", JournalEventNameType.Неисправность_устранена)]
		Напряжение_запуска_реле_ниже_нормы_Неисправность_устранена = 51,

		[EventDescriptionAttribute("КЗ выхода", JournalEventNameType.Неисправность)]
		КЗ_выхода_Неисправность = 52,

		[EventDescriptionAttribute("КЗ выхода", JournalEventNameType.Неисправность_устранена)]
		КЗ_выхода_Неисправность_устранена = 53,

		[EventDescriptionAttribute("Обрыв выхода", JournalEventNameType.Неисправность)]
		Обрыв_выхода_Неисправность = 54,

		[EventDescriptionAttribute("Обрыв выхода", JournalEventNameType.Неисправность_устранена)]
		Обрыв_выхода_Неисправность_устранена = 55,

		[EventDescriptionAttribute("Напряжение питания ШС ниже нормы", JournalEventNameType.Неисправность)]
		Напряжение_питания_ШС_ниже_нормы_Неисправность = 56,

		[EventDescriptionAttribute("Напряжение питания ШС ниже нормы", JournalEventNameType.Неисправность_устранена)]
		Напряжение_питания_ШС_ниже_нормы_Неисправность_устранена = 57,

		[EventDescriptionAttribute("Ошибка памяти", JournalEventNameType.Неисправность)]
		Ошибка_памяти_Неисправность = 58,

		[EventDescriptionAttribute("Ошибка памяти", JournalEventNameType.Неисправность_устранена)]
		Ошибка_памяти_Неисправность_устранена = 59,

		[EventDescriptionAttribute("КЗ выхода 1", JournalEventNameType.Неисправность)]
		КЗ_выхода_1_Неисправность = 60,

		[EventDescriptionAttribute("КЗ выхода 1", JournalEventNameType.Неисправность_устранена)]
		КЗ_выхода_1_Неисправность_устранена = 61,

		[EventDescriptionAttribute("КЗ выхода 2", JournalEventNameType.Неисправность)]
		КЗ_выхода_2_Неисправность = 62,

		[EventDescriptionAttribute("КЗ выхода 2", JournalEventNameType.Неисправность_устранена)]
		КЗ_выхода_2_Неисправность_устранена = 63,

		[EventDescriptionAttribute("КЗ выхода 3", JournalEventNameType.Неисправность)]
		КЗ_выхода_3_Неисправность = 64,

		[EventDescriptionAttribute("КЗ выхода 3", JournalEventNameType.Неисправность_устранена)]
		КЗ_выхода_3_Неисправность_устранена = 65,

		[EventDescriptionAttribute("КЗ выхода 4", JournalEventNameType.Неисправность)]
		КЗ_выхода_4_Неисправность = 66,

		[EventDescriptionAttribute("КЗ выхода 4", JournalEventNameType.Неисправность_устранена)]
		КЗ_выхода_4_Неисправность_устранена = 67,

		[EventDescriptionAttribute("КЗ выхода 5", JournalEventNameType.Неисправность)]
		КЗ_выхода_5_Неисправность = 68,

		[EventDescriptionAttribute("КЗ выхода 5", JournalEventNameType.Неисправность_устранена)]
		КЗ_выхода_5_Неисправность_устранена = 69,

		[EventDescriptionAttribute("Обрыв выхода 1", JournalEventNameType.Неисправность)]
		Обрыв_выхода_1_Неисправность = 70,

		[EventDescriptionAttribute("Обрыв выхода 1", JournalEventNameType.Неисправность_устранена)]
		Обрыв_выхода_1_Неисправность_устранена = 71,

		[EventDescriptionAttribute("Обрыв выхода 2", JournalEventNameType.Неисправность)]
		Обрыв_выхода_2_Неисправность = 72,

		[EventDescriptionAttribute("Обрыв выхода 2", JournalEventNameType.Неисправность_устранена)]
		Обрыв_выхода_2_Неисправность_устранена = 73,

		[EventDescriptionAttribute("Обрыв выхода 3", JournalEventNameType.Неисправность)]
		Обрыв_выхода_3_Неисправность = 74,

		[EventDescriptionAttribute("Обрыв выхода 3", JournalEventNameType.Неисправность_устранена)]
		Обрыв_выхода_3_Неисправность_устранена = 75,

		[EventDescriptionAttribute("Обрыв выхода 4", JournalEventNameType.Неисправность)]
		Обрыв_выхода_4_Неисправность = 76,

		[EventDescriptionAttribute("Обрыв выхода 4", JournalEventNameType.Неисправность_устранена)]
		Обрыв_выхода_4_Неисправность_устранена = 77,

		[EventDescriptionAttribute("Обрыв выхода 5", JournalEventNameType.Неисправность)]
		Обрыв_выхода_5_Неисправность = 78,

		[EventDescriptionAttribute("Обрыв выхода 5", JournalEventNameType.Неисправность_устранена)]
		Обрыв_выхода_5_Неисправность_устранена = 79,

		[EventDescriptionAttribute("Блокировка пуска", JournalEventNameType.Неисправность)]
		Блокировка_пуска_Неисправность = 80,

		[EventDescriptionAttribute("Блокировка пуска", JournalEventNameType.Неисправность_устранена)]
		Блокировка_пуска_Неисправность_устранена = 81,

		[EventDescriptionAttribute("Низкое напряжение питания привода", JournalEventNameType.Неисправность)]
		Низкое_напряжение_питания_привода_Неисправность = 82,

		[EventDescriptionAttribute("Низкое напряжение питания привода", JournalEventNameType.Неисправность_устранена)]
		Низкое_напряжение_питания_привода_Неисправность_устранена = 83,

		[EventDescriptionAttribute("Обрыв кнопки НОРМА", JournalEventNameType.Неисправность)]
		Обрыв_кнопки_НОРМА_Неисправность = 84,

		[EventDescriptionAttribute("Обрыв кнопки НОРМА", JournalEventNameType.Неисправность_устранена)]
		Обрыв_кнопки_НОРМА_Неисправность_устранена = 85,

		[EventDescriptionAttribute("КЗ кнопки НОРМА", JournalEventNameType.Неисправность)]
		КЗ_кнопки_НОРМА_Неисправность = 86,

		[EventDescriptionAttribute("КЗ кнопки НОРМА", JournalEventNameType.Неисправность_устранена)]
		КЗ_кнопки_НОРМА_Неисправность_устранена = 87,

		[EventDescriptionAttribute("Обрыв кнопка ЗАЩИТА", JournalEventNameType.Неисправность)]
		Обрыв_кнопка_ЗАЩИТА_Неисправность = 88,

		[EventDescriptionAttribute("Обрыв кнопка ЗАЩИТА", JournalEventNameType.Неисправность_устранена)]
		Обрыв_кнопка_ЗАЩИТА_Неисправность_устранена = 89,

		[EventDescriptionAttribute("КЗ кнопки ЗАЩИТА", JournalEventNameType.Неисправность)]
		КЗ_кнопки_ЗАЩИТА_Неисправность = 90,

		[EventDescriptionAttribute("КЗ кнопки ЗАЩИТА", JournalEventNameType.Неисправность_устранена)]
		КЗ_кнопки_ЗАЩИТА_Неисправность_устранена = 91,

		[EventDescriptionAttribute("Обрыв концевого выключателя ОТКРЫТО", JournalEventNameType.Неисправность)]
		Обрыв_концевого_выключателя_ОТКРЫТО_Неисправность = 92,

		[EventDescriptionAttribute("Обрыв концевого выключателя ОТКРЫТО", JournalEventNameType.Неисправность_устранена)]
		Обрыв_концевого_выключателя_ОТКРЫТО_Неисправность_устранена = 93,

		[EventDescriptionAttribute("Обрыв концевого выключателя ЗАКРЫТО", JournalEventNameType.Неисправность)]
		Обрыв_концевого_выключателя_ЗАКРЫТО_Неисправность = 94,

		[EventDescriptionAttribute("Обрыв концевого выключателя ЗАКРЫТО", JournalEventNameType.Неисправность_устранена)]
		Обрыв_концевого_выключателя_ЗАКРЫТО_Неисправность_устранена = 95,

		[EventDescriptionAttribute("Обрыв цепи ПД ЗАЩИТА", JournalEventNameType.Неисправность)]
		Обрыв_цепи_ПД_ЗАЩИТА_Неисправность = 96,

		[EventDescriptionAttribute("Обрыв цепи ПД ЗАЩИТА", JournalEventNameType.Неисправность_устранена)]
		Обрыв_цепи_ПД_ЗАЩИТА_Неисправность_устранена = 97,

		[EventDescriptionAttribute("Замкнуты/разомкнуты оба концевика", JournalEventNameType.Неисправность)]
		Замкнуты_разомкнуты_оба_концевика_Неисправность = 98,

		[EventDescriptionAttribute("Замкнуты/разомкнуты оба концевика", JournalEventNameType.Неисправность_устранена)]
		Замкнуты_разомкнуты_оба_концевика_Неисправность_устранена = 99,

		[EventDescriptionAttribute("Превышение времени хода", JournalEventNameType.Неисправность)]
		Превышение_времени_хода_Неисправность = 100,

		[EventDescriptionAttribute("Превышение времени хода", JournalEventNameType.Неисправность_устранена)]
		Превышение_времени_хода_Неисправность_устранена = 101,

		[EventDescriptionAttribute("Обрыв в линии РЕЛЕ", JournalEventNameType.Неисправность)]
		Обрыв_в_линии_РЕЛЕ_Неисправность = 102,

		[EventDescriptionAttribute("Обрыв в линии РЕЛЕ", JournalEventNameType.Неисправность_устранена)]
		Обрыв_в_линии_РЕЛЕ_Неисправность_устранена = 103,

		[EventDescriptionAttribute("КЗ в линии РЕЛЕ", JournalEventNameType.Неисправность)]
		КЗ_в_линии_РЕЛЕ_Неисправность = 104,

		[EventDescriptionAttribute("КЗ в линии РЕЛЕ", JournalEventNameType.Неисправность_устранена)]
		КЗ_в_линии_РЕЛЕ_Неисправность_устранена = 105,

		[EventDescriptionAttribute("Выход 1", JournalEventNameType.Неисправность)]
		Выход_1_Неисправность = 106,

		[EventDescriptionAttribute("Выход 1", JournalEventNameType.Неисправность_устранена)]
		Выход_1_Неисправность_устранена = 107,

		[EventDescriptionAttribute("Выход 2", JournalEventNameType.Неисправность)]
		Выход_2_Неисправность = 108,

		[EventDescriptionAttribute("Выход 2", JournalEventNameType.Неисправность_устранена)]
		Выход_2_Неисправность_устранена = 109,

		[EventDescriptionAttribute("Выход 3", JournalEventNameType.Неисправность)]
		Выход_3_Неисправность = 110,

		[EventDescriptionAttribute("Выход 3", JournalEventNameType.Неисправность_устранена)]
		Выход_3_Неисправность_устранена = 111,

		[EventDescriptionAttribute("КЗ концевого выключателя ОТКРЫТО", JournalEventNameType.Неисправность)]
		КЗ_концевого_выключателя_ОТКРЫТО_Неисправность = 112,

		[EventDescriptionAttribute("КЗ концевого выключателя ОТКРЫТО", JournalEventNameType.Неисправность_устранена)]
		КЗ_концевого_выключателя_ОТКРЫТО_Неисправность_устранена = 113,

		[EventDescriptionAttribute("Обрыв муфтового выключателя ОТКРЫТО", JournalEventNameType.Неисправность)]
		Обрыв_муфтового_выключателя_ОТКРЫТО_Неисправность = 114,

		[EventDescriptionAttribute("Обрыв муфтового выключателя ОТКРЫТО", JournalEventNameType.Неисправность_устранена)]
		Обрыв_муфтового_выключателя_ОТКРЫТО_Неисправность_устранена = 115,

		[EventDescriptionAttribute("КЗ муфтового выключателя ОТКРЫТО", JournalEventNameType.Неисправность)]
		КЗ_муфтового_выключателя_ОТКРЫТО_Неисправность = 116,

		[EventDescriptionAttribute("КЗ муфтового выключателя ОТКРЫТО", JournalEventNameType.Неисправность_устранена)]
		КЗ_муфтового_выключателя_ОТКРЫТО_Неисправность_устранена = 117,

		[EventDescriptionAttribute("КЗ концевого выключателя ЗАКРЫТО", JournalEventNameType.Неисправность_устранена)]
		КЗ_концевого_выключателя_ЗАКРЫТО_Неисправность = 118,

		[EventDescriptionAttribute("КЗ концевого выключателя ЗАКРЫТО", JournalEventNameType.Неисправность)]
		КЗ_концевого_выключателя_ЗАКРЫТО_Неисправность_устранена = 119,

		[EventDescriptionAttribute("Обрыв муфтового выключателя ЗАКРЫТО", JournalEventNameType.Неисправность)]
		Обрыв_муфтового_выключателя_ЗАКРЫТО_Неисправность = 120,

		[EventDescriptionAttribute("Обрыв муфтового выключателя ЗАКРЫТО", JournalEventNameType.Неисправность_устранена)]
		Обрыв_муфтового_выключателя_ЗАКРЫТО_Неисправность_устранена = 121,

		[EventDescriptionAttribute("КЗ муфтового выключателя ЗАКРЫТО", JournalEventNameType.Неисправность)]
		КЗ_муфтового_выключателя_ЗАКРЫТО_Неисправность = 122,

		[EventDescriptionAttribute("КЗ муфтового выключателя ЗАКРЫТО", JournalEventNameType.Неисправность_устранена)]
		КЗ_муфтового_выключателя_ЗАКРЫТО_Неисправность_устранена = 123,

		[EventDescriptionAttribute("Обрыв кнопки Открыть УЗЗ/Закрыть УЗЗ", JournalEventNameType.Неисправность)]
		Обрыв_кнопки_Открыть_УЗЗ_Закрыть_УЗЗ_Неисправность = 124,

		[EventDescriptionAttribute("Обрыв кнопки Открыть УЗЗ/Закрыть УЗЗ", JournalEventNameType.Неисправность_устранена)]
		Обрыв_кнопки_Открыть_УЗЗ_Закрыть_УЗЗ_Неисправность_устранена = 125,

		[EventDescriptionAttribute("КЗ кнопки Открыть УЗЗ/Закрыть УЗЗ", JournalEventNameType.Неисправность)]
		КЗ_кнопки_Открыть_УЗЗ_Закрыть_УЗЗ_Неисправность = 126,

		[EventDescriptionAttribute("КЗ кнопки Открыть УЗЗ/Закрыть УЗЗ", JournalEventNameType.Неисправность_устранена)]
		КЗ_кнопки_Открыть_УЗЗ_Закрыть_УЗЗ_Неисправность_устранена = 127,

		[EventDescriptionAttribute("Обрыв кнопки СТОП УЗЗ", JournalEventNameType.Неисправность)]
		Обрыв_кнопки_СТОП_УЗЗ_Неисправность = 128,

		[EventDescriptionAttribute("Обрыв кнопки СТОП УЗЗ", JournalEventNameType.Неисправность_устранена)]
		Обрыв_кнопки_СТОП_УЗЗ_Неисправность_устранена = 129,

		[EventDescriptionAttribute("КЗ кнопки СТОП УЗЗ", JournalEventNameType.Неисправность)]
		КЗ_кнопки_СТОП_УЗЗ_Неисправность = 130,

		[EventDescriptionAttribute("КЗ кнопки СТОП УЗЗ", JournalEventNameType.Неисправность_устранена)]
		КЗ_кнопки_СТОП_УЗЗ_Неисправность_устранена = 131,

		[EventDescriptionAttribute("Обрыв давление низкое", JournalEventNameType.Неисправность)]
		Обрыв_давление_низкое_Неисправность = 132,

		[EventDescriptionAttribute("Обрыв давление низкое", JournalEventNameType.Неисправность_устранена)]
		Обрыв_давление_низкое_Неисправность_устранена = 133,

		[EventDescriptionAttribute("КЗ давление низкое", JournalEventNameType.Неисправность)]
		КЗ_давление_низкое_Неисправность = 134,

		[EventDescriptionAttribute("КЗ давление низкое", JournalEventNameType.Неисправность_устранена)]
		КЗ_давление_низкое_Неисправность_устранена = 135,

		[EventDescriptionAttribute("Таймаут по давлению", JournalEventNameType.Неисправность)]
		Таймаут_по_давлению_Неисправность = 136,

		[EventDescriptionAttribute("Таймаут по давлению", JournalEventNameType.Неисправность_устранена)]
		Таймаут_по_давлению_Неисправность_устранена = 137,

		[EventDescriptionAttribute("КВ/МВ", JournalEventNameType.Неисправность)]
		КВ_МВ_Неисправность = 138,

		[EventDescriptionAttribute("КВ/МВ", JournalEventNameType.Неисправность_устранена)]
		КВ_МВ_Неисправность_устранена = 139,

		[EventDescriptionAttribute("Не задан режим", JournalEventNameType.Неисправность)]
		Не_задан_режим_Неисправность = 140,

		[EventDescriptionAttribute("Не задан режим", JournalEventNameType.Неисправность_устранена)]
		Не_задан_режим_Неисправность_устранена = 141,

		[EventDescriptionAttribute("Отказ ШУЗ", JournalEventNameType.Неисправность)]
		Отказ_ШУЗ_Неисправность = 142,

		[EventDescriptionAttribute("Отказ ШУЗ", JournalEventNameType.Неисправность_устранена)]
		Отказ_ШУЗ_Неисправность_устранена = 143,

		[EventDescriptionAttribute("ДУ/ДД", JournalEventNameType.Неисправность)]
		ДУ_ДД_Неисправность = 144,

		[EventDescriptionAttribute("ДУ/ДД", JournalEventNameType.Неисправность_устранена)]
		ДУ_ДД_Неисправность_устранена = 145,

		[EventDescriptionAttribute("Обрыв входа 1", JournalEventNameType.Неисправность)]
		Обрыв_входа_1_Неисправность = 146,

		[EventDescriptionAttribute("Обрыв входа 1", JournalEventNameType.Неисправность_устранена)]
		Обрыв_входа_1_Неисправность_устранена = 147,

		[EventDescriptionAttribute("КЗ входа 1", JournalEventNameType.Неисправность)]
		КЗ_входа_1_Неисправность = 148,

		[EventDescriptionAttribute("КЗ входа 1", JournalEventNameType.Неисправность_устранена)]
		КЗ_входа_1_Неисправность_устранена = 149,

		[EventDescriptionAttribute("Обрыв входа 2", JournalEventNameType.Неисправность)]
		Обрыв_входа_2_Неисправность = 150,

		[EventDescriptionAttribute("Обрыв входа 2", JournalEventNameType.Неисправность_устранена)]
		Обрыв_входа_2_Неисправность_устранена = 151,

		[EventDescriptionAttribute("КЗ входа 2", JournalEventNameType.Неисправность)]
		КЗ_входа_2_Неисправность = 152,

		[EventDescriptionAttribute("КЗ входа 2", JournalEventNameType.Неисправность_устранена)]
		КЗ_входа_2_Неисправность_устранена = 153,

		[EventDescriptionAttribute("Обрыв входа 3", JournalEventNameType.Неисправность)]
		Обрыв_входа_3_Неисправность = 154,

		[EventDescriptionAttribute("Обрыв входа 3", JournalEventNameType.Неисправность_устранена)]
		Обрыв_входа_3_Неисправность_устранена = 155,

		[EventDescriptionAttribute("КЗ входа 3", JournalEventNameType.Неисправность)]
		КЗ_входа_3_Неисправность = 156,

		[EventDescriptionAttribute("КЗ входа 3", JournalEventNameType.Неисправность_устранена)]
		КЗ_входа_3_Неисправность_устранена = 157,

		[EventDescriptionAttribute("Обрыв входа 4", JournalEventNameType.Неисправность)]
		Обрыв_входа_4_Неисправность = 158,

		[EventDescriptionAttribute("Обрыв входа 4", JournalEventNameType.Неисправность_устранена)]
		Обрыв_входа_4_Неисправность_устранена = 159,

		[EventDescriptionAttribute("КЗ входа 4", JournalEventNameType.Неисправность)]
		КЗ_входа_4_Неисправность = 160,

		[EventDescriptionAttribute("КЗ входа 4", JournalEventNameType.Неисправность_устранена)]
		КЗ_входа_4_Неисправность_устранена = 161,

		[EventDescriptionAttribute("Не задан тип", JournalEventNameType.Неисправность)]
		Не_задан_тип_Неисправность = 162,

		[EventDescriptionAttribute("Не задан тип", JournalEventNameType.Неисправность_устранена)]
		Не_задан_тип_Неисправность_устранена = 163,

		[EventDescriptionAttribute("Отказ ПН", JournalEventNameType.Неисправность)]
		Отказ_ПН_Неисправность = 164,

		[EventDescriptionAttribute("Отказ ПН", JournalEventNameType.Неисправность_устранена)]
		Отказ_ПН_Неисправность_устранена = 165,

		[EventDescriptionAttribute("Отказ ШУН", JournalEventNameType.Неисправность)]
		Отказ_ШУН_Неисправность = 166,

		[EventDescriptionAttribute("Отказ ШУН", JournalEventNameType.Неисправность_устранена)]
		Отказ_ШУН_Неисправность_устранена = 167,

		[EventDescriptionAttribute("Питание 1", JournalEventNameType.Неисправность)]
		Питание_1_Неисправность = 168,

		[EventDescriptionAttribute("Питание 1", JournalEventNameType.Неисправность_устранена)]
		Питание_1_Неисправность_устранена = 169,

		[EventDescriptionAttribute("Питание 2", JournalEventNameType.Неисправность)]
		Питание_2_Неисправность = 170,

		[EventDescriptionAttribute("Питание 2", JournalEventNameType.Неисправность_устранена)]
		Питание_2_Неисправность_устранена = 171,

		[EventDescriptionAttribute("Отказ АЛС 1 или 2", JournalEventNameType.Неисправность)]
		Отказ_АЛС_1_или_2_Неисправность = 172,

		[EventDescriptionAttribute("Отказ АЛС 1 или 2", JournalEventNameType.Неисправность_устранена)]
		Отказ_АЛС_1_или_2_Неисправность_устранена = 173,

		[EventDescriptionAttribute("Отказ АЛС 3 или 4", JournalEventNameType.Неисправность)]
		Отказ_АЛС_3_или_4_Неисправность = 174,

		[EventDescriptionAttribute("Отказ АЛС 3 или 4", JournalEventNameType.Неисправность_устранена)]
		Отказ_АЛС_3_или_4_Неисправность_устранена = 175,

		[EventDescriptionAttribute("Отказ АЛС 5 или 6", JournalEventNameType.Неисправность)]
		Отказ_АЛС_5_или_6_Неисправность = 176,

		[EventDescriptionAttribute("Отказ АЛС 5 или 6", JournalEventNameType.Неисправность_устранена)]
		Отказ_АЛС_5_или_6_Неисправность_устранена = 177,

		[EventDescriptionAttribute("Отказ АЛС 7 или 8", JournalEventNameType.Неисправность)]
		Отказ_АЛС_7_или_8_Неисправность = 178,

		[EventDescriptionAttribute("Отказ АЛС 7 или 8", JournalEventNameType.Неисправность_устранена)]
		Отказ_АЛС_7_или_8_Неисправность_устранена = 179,

		[EventDescriptionAttribute("Обрыв цепи ПД НОРМА", JournalEventNameType.Неисправность)]
		Обрыв_цепи_ПД_НОРМА_Неисправность = 180,

		[EventDescriptionAttribute("Обрыв цепи ПД НОРМА", JournalEventNameType.Неисправность_устранена)]
		Обрыв_цепи_ПД_НОРМА_Неисправность_устранена = 181,

		[EventDescriptionAttribute("КЗ АЛС 1", JournalEventNameType.Неисправность)]
		КЗ_АЛС_1_Неисправность = 182,

		[EventDescriptionAttribute("КЗ АЛС 1", JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_1_Неисправность_устранена = 183,

		[EventDescriptionAttribute("КЗ АЛС 2", JournalEventNameType.Неисправность)]
		КЗ_АЛС_2_Неисправность = 184,

		[EventDescriptionAttribute("КЗ АЛС 2", JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_2_Неисправность_устранена = 185,

		[EventDescriptionAttribute("КЗ АЛС 3", JournalEventNameType.Неисправность)]
		КЗ_АЛС_3_Неисправность = 186,

		[EventDescriptionAttribute("КЗ АЛС 3", JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_3_Неисправность_устранена = 187,

		[EventDescriptionAttribute("КЗ АЛС 4", JournalEventNameType.Неисправность)]
		КЗ_АЛС_4_Неисправность = 188,

		[EventDescriptionAttribute("КЗ АЛС 4", JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_4_Неисправность_устранена = 189,

		[EventDescriptionAttribute("КЗ АЛС 5", JournalEventNameType.Неисправность)]
		КЗ_АЛС_5_Неисправность = 190,

		[EventDescriptionAttribute("КЗ АЛС 5", JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_5_Неисправность_устранена = 191,

		[EventDescriptionAttribute("КЗ АЛС 6", JournalEventNameType.Неисправность)]
		КЗ_АЛС_6_Неисправность = 192,

		[EventDescriptionAttribute("КЗ АЛС 6", JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_6_Неисправность_устранена = 193,

		[EventDescriptionAttribute("КЗ АЛС 7", JournalEventNameType.Неисправность)]
		КЗ_АЛС_7_Неисправность = 194,

		[EventDescriptionAttribute("КЗ АЛС 7", JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_7_Неисправность_устранена = 195,

		[EventDescriptionAttribute("КЗ АЛС 8", JournalEventNameType.Неисправность)]
		КЗ_АЛС_8_Неисправность = 196,

		[EventDescriptionAttribute("КЗ АЛС 8", JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_8_Неисправность_устранена = 197,

		[EventDescriptionAttribute("Истекло время вкл", JournalEventNameType.Неисправность)]
		Истекло_время_вкл_Неисправность = 198,

		[EventDescriptionAttribute("Истекло время вкл", JournalEventNameType.Неисправность_устранена)]
		Истекло_время_вкл_Неисправность_устранена = 199,

		[EventDescriptionAttribute("Истекло время выкл", JournalEventNameType.Неисправность)]
		Истекло_время_выкл_Неисправность = 200,

		[EventDescriptionAttribute("Истекло время выкл", JournalEventNameType.Неисправность_устранена)]
		Истекло_время_выкл_Неисправность_устранена = 201,

		[EventDescriptionAttribute("Контакт реле 1", JournalEventNameType.Неисправность)]
		Контакт_реле_1_Неисправность = 202,

		[EventDescriptionAttribute("Контакт реле 1", JournalEventNameType.Неисправность_устранена)]
		Контакт_реле_1_Неисправность_устранена = 203,

		[EventDescriptionAttribute("Контакт реле 2", JournalEventNameType.Неисправность)]
		Контакт_реле_2_Неисправность = 204,

		[EventDescriptionAttribute("Контакт реле 2", JournalEventNameType.Неисправность_устранена)]
		Контакт_реле_2_Неисправность_устранена = 205,

		[EventDescriptionAttribute("Обрыв кнопки ПУСК", JournalEventNameType.Неисправность)]
		Обрыв_кнопки_ПУСК_Неисправность = 206,

		[EventDescriptionAttribute("Обрыв кнопки ПУСК", JournalEventNameType.Неисправность_устранена)]
		Обрыв_кнопки_ПУСК_Неисправность_устранена = 207,

		[EventDescriptionAttribute("КЗ кнопки ПУСК", JournalEventNameType.Неисправность)]
		КЗ_кнопки_ПУСК_Неисправность = 208,

		[EventDescriptionAttribute("КЗ кнопки ПУСК", JournalEventNameType.Неисправность_устранена)]
		КЗ_кнопки_ПУСК_Неисправность_устранена = 209,

		[EventDescriptionAttribute("Обрыв кнопки СТОП", JournalEventNameType.Неисправность)]
		Обрыв_кнопки_СТОП_Неисправность = 210,

		[EventDescriptionAttribute("Обрыв кнопки СТОП", JournalEventNameType.Неисправность_устранена)]
		Обрыв_кнопки_СТОП_Неисправность_устранена = 211,

		[EventDescriptionAttribute("КЗ кнопки СТОП", JournalEventNameType.Неисправность)]
		КЗ_кнопки_СТОП_Неисправность = 212,

		[EventDescriptionAttribute("КЗ кнопки СТОП", JournalEventNameType.Неисправность_устранена)]
		КЗ_кнопки_СТОП_Неисправность_устранена = 213,

		[EventDescriptionAttribute("Отсутствуют или испорчены сообщения для воспроизведения", JournalEventNameType.Неисправность)]
		Отсутствуют_или_испорчены_сообщения_для_воспроизведения_Неисправность = 214,

		[EventDescriptionAttribute("Отсутствуют или испорчены сообщения для воспроизведения", JournalEventNameType.Неисправность_устранена)]
		Отсутствуют_или_испорчены_сообщения_для_воспроизведения_Неисправность_устранена = 215,

		[EventDescriptionAttribute("Выход", JournalEventNameType.Неисправность)]
		Выход_Неисправность = 216,

		[EventDescriptionAttribute("Выход", JournalEventNameType.Неисправность_устранена)]
		Выход_Неисправность_устранена = 217,

		[EventDescriptionAttribute("Обрыв Низкий уровень", JournalEventNameType.Неисправность)]
		Обрыв_Низкий_уровень_Неисправность = 218,

		[EventDescriptionAttribute("Обрыв Низкий уровень", JournalEventNameType.Неисправность_устранена)]
		Обрыв_Низкий_уровень_Неисправность_устранена = 219,

		[EventDescriptionAttribute("КЗ Низкий уровень", JournalEventNameType.Неисправность)]
		КЗ_Низкий_уровень_Неисправность = 220,

		[EventDescriptionAttribute("КЗ Низкий уровень", JournalEventNameType.Неисправность_устранена)]
		КЗ_Низкий_уровень_Неисправность_устранена = 221,

		[EventDescriptionAttribute("Обрыв Высокий уровень", JournalEventNameType.Неисправность)]
		Обрыв_Высокий_уровень_Неисправность = 222,

		[EventDescriptionAttribute("Обрыв Высокий уровень", JournalEventNameType.Неисправность_устранена)]
		Обрыв_Высокий_уровень_Неисправность_устранена = 223,

		[EventDescriptionAttribute("КЗ Высокий уровень", JournalEventNameType.Неисправность)]
		КЗ_Высокий_уровень_Неисправность = 224,

		[EventDescriptionAttribute("КЗ Высокий уровень", JournalEventNameType.Неисправность_устранена)]
		КЗ_Высокий_уровень_Неисправность_устранена = 225,

		[EventDescriptionAttribute("Обрыв Аварийный уровень", JournalEventNameType.Неисправность)]
		Обрыв_Аварийный_уровень_Неисправность = 226,

		[EventDescriptionAttribute("Обрыв Аварийный уровень", JournalEventNameType.Неисправность_устранена)]
		Обрыв_Аварийный_уровень_Неисправность_устранена = 227,

		[EventDescriptionAttribute("КЗ Аварийный уровень", JournalEventNameType.Неисправность)]
		КЗ_Аварийный_уровень_Неисправность = 228,

		[EventDescriptionAttribute("КЗ Аварийный уровень", JournalEventNameType.Неисправность_устранена)]
		КЗ_Аварийный_уровень_Неисправность_устранена = 229,

		[EventDescriptionAttribute("Аварийный уровень", JournalEventNameType.Неисправность)]
		Аварийный_уровень_Неисправность = 230,

		[EventDescriptionAttribute("Аварийный уровень", JournalEventNameType.Неисправность_устранена)]
		Аварийный_уровень_Неисправность_устранена = 231,

		[EventDescriptionAttribute("Питание силовое", JournalEventNameType.Неисправность)]
		Питание_силовое_Неисправность = 232,

		[EventDescriptionAttribute("Питание силовое", JournalEventNameType.Неисправность_устранена)]
		Питание_силовое_Неисправность_устранена = 233,

		[EventDescriptionAttribute("Питание контроллера", JournalEventNameType.Неисправность)]
		Питание_контроллера_Неисправность = 234,

		[EventDescriptionAttribute("Питание контроллера", JournalEventNameType.Неисправность_устранена)]
		Питание_контроллера_Неисправность_устранена = 235,

		[EventDescriptionAttribute("Несовместимость сигналов", JournalEventNameType.Неисправность)]
		Несовместимость_сигналов_Неисправность = 236,

		[EventDescriptionAttribute("Несовместимость сигналов", JournalEventNameType.Неисправность_устранена)]
		Несовместимость_сигналов_Неисправность_устранена = 237,

		[EventDescriptionAttribute("Обрыв цепи питания двигателя", JournalEventNameType.Неисправность)]
		Обрыв_цепи_питания_двигателя_Неисправность = 238,

		[EventDescriptionAttribute("Обрыв цепи питания двигателя", JournalEventNameType.Неисправность_устранена)]
		Обрыв_цепи_питания_двигателя_Неисправность_устранена = 239,

		[EventDescriptionAttribute("Обрыв Давление на выходе", JournalEventNameType.Неисправность)]
		Обрыв_Давление_на_выходе_Неисправность = 240,

		[EventDescriptionAttribute("Обрыв Давление на выходе", JournalEventNameType.Неисправность_устранена)]
		Обрыв_Давление_на_выходе_Неисправность_устранена = 241,

		[EventDescriptionAttribute("КЗ Давление на выходе", JournalEventNameType.Неисправность)]
		КЗ_Давление_на_выходе_Неисправность = 242,

		[EventDescriptionAttribute("КЗ Давление на выходе", JournalEventNameType.Неисправность_устранена)]
		КЗ_Давление_на_выходе_Неисправность_устранена = 243,

		[EventDescriptionAttribute("Обрыв ДУ ПУСК", JournalEventNameType.Неисправность)]
		Обрыв_ДУ_ПУСК_Неисправность = 244,

		[EventDescriptionAttribute("Обрыв ДУ ПУСК", JournalEventNameType.Неисправность_устранена)]
		Обрыв_ДУ_ПУСК_Неисправность_устранена = 245,

		[EventDescriptionAttribute("КЗ ДУ ПУСК", JournalEventNameType.Неисправность)]
		КЗ_ДУ_ПУСК_Неисправность = 246,

		[EventDescriptionAttribute("КЗ ДУ ПУСК", JournalEventNameType.Неисправность_устранена)]
		КЗ_ДУ_ПУСК_Неисправность_устранена = 247,

		[EventDescriptionAttribute("Обрыв ДУ СТОП", JournalEventNameType.Неисправность)]
		Обрыв_ДУ_СТОП_Неисправность = 248,

		[EventDescriptionAttribute("Обрыв ДУ СТОП", JournalEventNameType.Неисправность_устранена)]
		Обрыв_ДУ_СТОП_Неисправность_устранена = 249,

		[EventDescriptionAttribute("КЗ ДУ СТОП", JournalEventNameType.Неисправность)]
		КЗ_ДУ_СТОП_Неисправность = 250,

		[EventDescriptionAttribute("КЗ ДУ СТОП", JournalEventNameType.Неисправность_устранена)]
		КЗ_ДУ_СТОП_Неисправность_устранена = 251,

		[EventDescriptionAttribute("АЛС 1 Неизвестное устройство", JournalEventNameType.Неисправность)]
		АЛС_1_Неизвестное_устройство_Неисправность = 252,

		[EventDescriptionAttribute("АЛС 1 Неизвестное устройство", JournalEventNameType.Неисправность_устранена)]
		АЛС_1_Неизвестное_устройство_Неисправность_устранена = 253,

		[EventDescriptionAttribute("АЛС 2 Неизвестное устройство", JournalEventNameType.Неисправность)]
		АЛС_2_Неизвестное_устройство_Неисправность = 254,

		[EventDescriptionAttribute("АЛС 2 Неизвестное устройство", JournalEventNameType.Неисправность_устранена)]
		АЛС_2_Неизвестное_устройство_Неисправность_устранена = 255,

		[EventDescriptionAttribute("АЛС 3 Неизвестное устройство", JournalEventNameType.Неисправность)]
		АЛС_3_Неизвестное_устройство_Неисправность = 256,

		[EventDescriptionAttribute("АЛС 3 Неизвестное устройство", JournalEventNameType.Неисправность_устранена)]
		АЛС_3_Неизвестное_устройство_Неисправность_устранена = 257,

		[EventDescriptionAttribute("АЛС 4 Неизвестное устройство", JournalEventNameType.Неисправность)]
		АЛС_4_Неизвестное_устройство_Неисправность = 258,

		[EventDescriptionAttribute("АЛС 4 Неизвестное устройство", JournalEventNameType.Неисправность_устранена)]
		АЛС_4_Неизвестное_устройство_Неисправность_устранена = 259,

		[EventDescriptionAttribute("АЛС 5 Неизвестное устройство", JournalEventNameType.Неисправность)]
		АЛС_5_Неизвестное_устройство_Неисправность = 260,

		[EventDescriptionAttribute("АЛС 5 Неизвестное устройство", JournalEventNameType.Неисправность_устранена)]
		АЛС_5_Неизвестное_устройство_Неисправность_устранена = 261,

		[EventDescriptionAttribute("АЛС 6 Неизвестное устройство", JournalEventNameType.Неисправность)]
		АЛС_6_Неизвестное_устройство_Неисправность = 262,

		[EventDescriptionAttribute("АЛС 6 Неизвестное устройство", JournalEventNameType.Неисправность_устранена)]
		АЛС_6_Неизвестное_устройство_Неисправность_устранена = 263,

		[EventDescriptionAttribute("АЛС 7 Неизвестное устройство", JournalEventNameType.Неисправность)]
		АЛС_7_Неизвестное_устройство_Неисправность = 264,

		[EventDescriptionAttribute("АЛС 7 Неизвестное устройство", JournalEventNameType.Неисправность_устранена)]
		АЛС_7_Неизвестное_устройство_Неисправность_устранена = 265,

		[EventDescriptionAttribute("АЛС 8 Неизвестное устройство", JournalEventNameType.Неисправность)]
		АЛС_8_Неизвестное_устройство_Неисправность = 266,

		[EventDescriptionAttribute("АЛС 8 Неизвестное устройство", JournalEventNameType.Неисправность_устранена)]
		АЛС_8_Неизвестное_устройство_Неисправность_устранена = 267,

		[EventDescriptionAttribute("АЛС 1 Неизвестный тип устройства", JournalEventNameType.Неисправность)]
		АЛС_1_Неизвестный_тип_устройства_Неисправность = 268,

		[EventDescriptionAttribute("АЛС 1 Неизвестный тип устройства", JournalEventNameType.Неисправность_устранена)]
		АЛС_1_Неизвестный_тип_устройства_Неисправность_устранена = 269,

		[EventDescriptionAttribute("АЛС 2 Неизвестный тип устройства", JournalEventNameType.Неисправность)]
		АЛС_2_Неизвестный_тип_устройства_Неисправность = 270,

		[EventDescriptionAttribute("АЛС 2 Неизвестный тип устройства", JournalEventNameType.Неисправность_устранена)]
		АЛС_2_Неизвестный_тип_устройства_Неисправность_устранена = 271,

		[EventDescriptionAttribute("АЛС 3 Неизвестный тип устройства", JournalEventNameType.Неисправность)]
		АЛС_3_Неизвестный_тип_устройства_Неисправность = 272,

		[EventDescriptionAttribute("АЛС 3 Неизвестный тип устройства", JournalEventNameType.Неисправность_устранена)]
		АЛС_3_Неизвестный_тип_устройства_Неисправность_устранена = 273,

		[EventDescriptionAttribute("АЛС 4 Неизвестный тип устройства", JournalEventNameType.Неисправность)]
		АЛС_4_Неизвестный_тип_устройства_Неисправность = 274,

		[EventDescriptionAttribute("АЛС 4 Неизвестный тип устройства", JournalEventNameType.Неисправность_устранена)]
		АЛС_4_Неизвестный_тип_устройства_Неисправность_устранена = 275,

		[EventDescriptionAttribute("АЛС 5 Неизвестный тип устройства", JournalEventNameType.Неисправность)]
		АЛС_5_Неизвестный_тип_устройства_Неисправность = 276,

		[EventDescriptionAttribute("АЛС 5 Неизвестный тип устройства", JournalEventNameType.Неисправность_устранена)]
		АЛС_5_Неизвестный_тип_устройства_Неисправность_устранена = 277,

		[EventDescriptionAttribute("АЛС 6 Неизвестный тип устройства", JournalEventNameType.Неисправность)]
		АЛС_6_Неизвестный_тип_устройства_Неисправность = 278,

		[EventDescriptionAttribute("АЛС 6 Неизвестный тип устройства", JournalEventNameType.Неисправность_устранена)]
		АЛС_6_Неизвестный_тип_устройства_Неисправность_устранена = 279,

		[EventDescriptionAttribute("АЛС 7 Неизвестный тип устройства", JournalEventNameType.Неисправность)]
		АЛС_7_Неизвестный_тип_устройства_Неисправность = 280,

		[EventDescriptionAttribute("АЛС 7 Неизвестный тип устройства", JournalEventNameType.Неисправность_устранена)]
		АЛС_7_Неизвестный_тип_устройства_Неисправность_устранена = 281,

		[EventDescriptionAttribute("АЛС 8 Неизвестный тип устройства", JournalEventNameType.Неисправность)]
		АЛС_8_Неизвестный_тип_устройства_Неисправность = 282,

		[EventDescriptionAttribute("АЛС 8 Неизвестный тип устройства", JournalEventNameType.Неисправность_устранена)]
		АЛС_8_Неизвестный_тип_устройства_Неисправность_устранена = 283,

		[EventDescriptionAttribute("АЛС 1 Другой тип устройства", JournalEventNameType.Неисправность)]
		АЛС_1_Другой_тип_устройства_Неисправность = 284,

		[EventDescriptionAttribute("АЛС 1 Другой тип устройства", JournalEventNameType.Неисправность_устранена)]
		АЛС_1_Другой_тип_устройства_Неисправность_устранена = 285,

		[EventDescriptionAttribute("АЛС 2 Другой тип устройства", JournalEventNameType.Неисправность)]
		АЛС_2_Другой_тип_устройства_Неисправность = 286,

		[EventDescriptionAttribute("АЛС 2 Другой тип устройства", JournalEventNameType.Неисправность_устранена)]
		АЛС_2_Другой_тип_устройства_Неисправность_устранена = 287,

		[EventDescriptionAttribute("АЛС 3 Другой тип устройства", JournalEventNameType.Неисправность)]
		АЛС_3_Другой_тип_устройства_Неисправность = 288,

		[EventDescriptionAttribute("АЛС 3 Другой тип устройства", JournalEventNameType.Неисправность_устранена)]
		АЛС_3_Другой_тип_устройства_Неисправность_устранена = 289,

		[EventDescriptionAttribute("АЛС 4 Другой тип устройства", JournalEventNameType.Неисправность)]
		АЛС_4_Другой_тип_устройства_Неисправность = 290,

		[EventDescriptionAttribute("АЛС 4 Другой тип устройства", JournalEventNameType.Неисправность_устранена)]
		АЛС_4_Другой_тип_устройства_Неисправность_устранена = 291,

		[EventDescriptionAttribute("АЛС 5 Другой тип устройства", JournalEventNameType.Неисправность)]
		АЛС_5_Другой_тип_устройства_Неисправность = 292,

		[EventDescriptionAttribute("АЛС 5 Другой тип устройства", JournalEventNameType.Неисправность_устранена)]
		АЛС_5_Другой_тип_устройства_Неисправность_устранена = 293,

		[EventDescriptionAttribute("АЛС 6 Другой тип устройства", JournalEventNameType.Неисправность)]
		АЛС_6_Другой_тип_устройства_Неисправность = 294,

		[EventDescriptionAttribute("АЛС 6 Другой тип устройства", JournalEventNameType.Неисправность_устранена)]
		АЛС_6_Другой_тип_устройства_Неисправность_устранена = 295,

		[EventDescriptionAttribute("АЛС 7 Другой тип устройства", JournalEventNameType.Неисправность)]
		АЛС_7_Другой_тип_устройства_Неисправность = 296,

		[EventDescriptionAttribute("АЛС 7 Другой тип устройства", JournalEventNameType.Неисправность_устранена)]
		АЛС_7_Другой_тип_устройства_Неисправность_устранена = 297,

		[EventDescriptionAttribute("АЛС 8 Другой тип устройства", JournalEventNameType.Неисправность)]
		АЛС_8_Другой_тип_устройства_Неисправность = 298,

		[EventDescriptionAttribute("АЛС 8 Другой тип устройства", JournalEventNameType.Неисправность_устранена)]
		АЛС_8_Другой_тип_устройства_Неисправность_устранена = 299,

		[EventDescriptionAttribute("Обрыв АЛС 1-2", JournalEventNameType.Неисправность)]
		Обрыв_АЛС_1_2_Неисправность = 300,

		[EventDescriptionAttribute("Обрыв АЛС 1-2", JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_1_2_Неисправность_устранена = 301,

		[EventDescriptionAttribute("Обрыв АЛС 3-4", JournalEventNameType.Неисправность)]
		Обрыв_АЛС_3_4_Неисправность = 302,

		[EventDescriptionAttribute("Обрыв АЛС 3-4", JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_3_4_Неисправность_устранена = 303,

		[EventDescriptionAttribute("Обрыв АЛС 5-6", JournalEventNameType.Неисправность)]
		Обрыв_АЛС_5_6_Неисправность = 304,

		[EventDescriptionAttribute("Обрыв АЛС 5-6", JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_5_6_Неисправность_устранена = 305,

		[EventDescriptionAttribute("Обрыв АЛС 7-8", JournalEventNameType.Неисправность)]
		Обрыв_АЛС_7_8_Неисправность = 306,

		[EventDescriptionAttribute("Обрыв АЛС 7-8", JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_7_8_Неисправность_устранена = 307,

		[EventDescriptionAttribute("Обрыв АЛС 1", JournalEventNameType.Неисправность)]
		Обрыв_АЛС_1_Неисправность = 308,

		[EventDescriptionAttribute("Обрыв АЛС 1", JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_1_Неисправность_устранена = 309,

		[EventDescriptionAttribute("Обрыв АЛС 2", JournalEventNameType.Неисправность)]
		Обрыв_АЛС_2_Неисправность = 310,

		[EventDescriptionAttribute("Обрыв АЛС 2", JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_2_Неисправность_устранена = 311,

		[EventDescriptionAttribute("Обрыв АЛС 3", JournalEventNameType.Неисправность)]
		Обрыв_АЛС_3_Неисправность = 312,

		[EventDescriptionAttribute("Обрыв АЛС 3", JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_3_Неисправность_устранена = 313,

		[EventDescriptionAttribute("Обрыв АЛС 4", JournalEventNameType.Неисправность)]
		Обрыв_АЛС_4_Неисправность = 314,

		[EventDescriptionAttribute("Обрыв АЛС 4", JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_4_Неисправность_устранена = 315,

		[EventDescriptionAttribute("Обрыв АЛС 5", JournalEventNameType.Неисправность)]
		Обрыв_АЛС_5_Неисправность = 316,

		[EventDescriptionAttribute("Обрыв АЛС 5", JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_5_Неисправность_устранена = 317,

		[EventDescriptionAttribute("Обрыв АЛС 6", JournalEventNameType.Неисправность)]
		Обрыв_АЛС_6_Неисправность = 318,

		[EventDescriptionAttribute("Обрыв АЛС 6", JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_6_Неисправность_устранена = 319,

		[EventDescriptionAttribute("Обрыв АЛС 7", JournalEventNameType.Неисправность)]
		Обрыв_АЛС_7_Неисправность = 320,

		[EventDescriptionAttribute("Обрыв АЛС 7", JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_7_Неисправность_устранена = 321,

		[EventDescriptionAttribute("Обрыв АЛС 8", JournalEventNameType.Неисправность)]
		Обрыв_АЛС_8_Неисправность = 322,

		[EventDescriptionAttribute("Обрыв АЛС 8", JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_8_Неисправность_устранена = 323,

		[EventDescriptionAttribute("ОЛС", JournalEventNameType.Неисправность)]
		ОЛС_Неисправность = 324,

		[EventDescriptionAttribute("ОЛС", JournalEventNameType.Неисправность_устранена)]
		ОЛС_Неисправность_устранена = 325,

		[EventDescriptionAttribute("РЛС", JournalEventNameType.Неисправность)]
		РЛС_Неисправность = 326,

		[EventDescriptionAttribute("РЛС", JournalEventNameType.Неисправность_устранена)]
		РЛС_Неисправность_устранена = 327,

		[EventDescriptionAttribute("Потеря связи", JournalEventNameType.Неисправность)]
		Потеря_связи_Неисправность = 328,

		[EventDescriptionAttribute("Потеря связи", JournalEventNameType.Неисправность_устранена)]
		Потеря_связи_Неисправность_устранена = 329,

		[EventDescriptionAttribute("Отсутствие сетевого напряжения", JournalEventNameType.Неисправность)]
		Отсутствие_сетевого_напряжения_Неисправность = 330,

		[EventDescriptionAttribute("Отсутствие сетевого напряжения", JournalEventNameType.Неисправность_устранена)]
		Отсутствие_сетевого_напряжения_Неисправность_устранена = 331,

		[EventDescriptionAttribute("КЗ Выхода 1", JournalEventNameType.Неисправность)]
		КЗ_Выхода_1_Неисправность = 332,

		[EventDescriptionAttribute("КЗ Выхода 1", JournalEventNameType.Неисправность_устранена)]
		КЗ_Выхода_1_Неисправность_устранена = 333,

		[EventDescriptionAttribute("Перегрузка Выхода 1", JournalEventNameType.Неисправность)]
		Перегрузка_Выхода_1_Неисправность = 334,

		[EventDescriptionAttribute("Перегрузка Выхода 1", JournalEventNameType.Неисправность_устранена)]
		Перегрузка_Выхода_1_Неисправность_устранена = 335,

		[EventDescriptionAttribute("Напряжение Выхода 1 выше нормы", JournalEventNameType.Неисправность)]
		Напряжение_Выхода_1_выше_нормы_Неисправность = 336,

		[EventDescriptionAttribute("Напряжение Выхода 1 выше нормы", JournalEventNameType.Неисправность_устранена)]
		Напряжение_Выхода_1_выше_нормы_Неисправность_устранена = 337,

		[EventDescriptionAttribute("КЗ Выхода 2", JournalEventNameType.Неисправность)]
		КЗ_Выхода_2_Неисправность = 338,

		[EventDescriptionAttribute("КЗ Выхода 2", JournalEventNameType.Неисправность_устранена)]
		КЗ_Выхода_2_Неисправность_устранена = 339,

		[EventDescriptionAttribute("Перегрузка Выхода 2", JournalEventNameType.Неисправность)]
		Перегрузка_Выхода_2_Неисправность = 340,

		[EventDescriptionAttribute("Перегрузка Выхода 2", JournalEventNameType.Неисправность_устранена)]
		Перегрузка_Выхода_2_Неисправность_устранена = 341,

		[EventDescriptionAttribute("Напряжение Выхода 2 выше нормы", JournalEventNameType.Неисправность)]
		Напряжение_Выхода_2_выше_нормы_Неисправность = 342,

		[EventDescriptionAttribute("Напряжение Выхода 2 выше нормы", JournalEventNameType.Неисправность_устранена)]
		Напряжение_Выхода_2_выше_нормы_Неисправность_устранена = 343,

		[EventDescriptionAttribute("АКБ 1", JournalEventNameType.Неисправность)]
		АКБ_1_Неисправность = 344,

		[EventDescriptionAttribute("АКБ 1", JournalEventNameType.Неисправность_устранена)]
		АКБ_1_Неисправность_устранена = 345,

		[EventDescriptionAttribute("АКБ 1 Разряд", JournalEventNameType.Неисправность)]
		АКБ_1_Разряд_Неисправность = 346,

		[EventDescriptionAttribute("АКБ 1 Разряд", JournalEventNameType.Неисправность_устранена)]
		АКБ_1_Разряд_Неисправность_устранена = 347,

		[EventDescriptionAttribute("АКБ 1 Глубокий Разряд", JournalEventNameType.Неисправность)]
		АКБ_1_Глубокий_Разряд_Неисправность = 348,

		[EventDescriptionAttribute("АКБ 1 Глубокий Разряд", JournalEventNameType.Неисправность_устранена)]
		АКБ_1_Глубокий_Разряд_Неисправность_устранена = 349,

		[EventDescriptionAttribute("АКБ 1 Отсутствие", JournalEventNameType.Неисправность)]
		АКБ_1_Отсутствие_Неисправность = 350,

		[EventDescriptionAttribute("АКБ 1 Отсутствие", JournalEventNameType.Неисправность_устранена)]
		АКБ_1_Отсутствие_Неисправность_устранена = 351,

		[EventDescriptionAttribute("АКБ 2", JournalEventNameType.Неисправность)]
		АКБ_2_Неисправность = 352,

		[EventDescriptionAttribute("АКБ 2", JournalEventNameType.Неисправность_устранена)]
		АКБ_2_Неисправность_устранена = 353,

		[EventDescriptionAttribute("АКБ 2 Разряд", JournalEventNameType.Неисправность)]
		АКБ_2_Разряд_Неисправность = 354,

		[EventDescriptionAttribute("АКБ 2 Разряд", JournalEventNameType.Неисправность_устранена)]
		АКБ_2_Разряд_Неисправность_устранена = 355,

		[EventDescriptionAttribute("АКБ 2 Глубокий Разряд", JournalEventNameType.Неисправность)]
		АКБ_2_Глубокий_Разряд_Неисправность = 356,

		[EventDescriptionAttribute("АКБ 2 Глубокий Разряд", JournalEventNameType.Неисправность_устранена)]
		АКБ_2_Глубокий_Разряд_Неисправность_устранена = 357,

		[EventDescriptionAttribute("АКБ 2 Отсутствие", JournalEventNameType.Неисправность)]
		АКБ_2_Отсутствие_Неисправность = 358,

		[EventDescriptionAttribute("АКБ 2 Отсутствие", JournalEventNameType.Неисправность_устранена)]
		АКБ_2_Отсутствие_Неисправность_устранена = 359,

		[EventDescriptionAttribute("Контактор Открыть", JournalEventNameType.Неисправность)]
		Контактор_Открыть_Неисправность = 360,

		[EventDescriptionAttribute("Контактор Открыть", JournalEventNameType.Неисправность_устранена)]
		Контактор_Открыть_Неисправность_устранена = 361,

		[EventDescriptionAttribute("Контактор Закрыть", JournalEventNameType.Неисправность)]
		Контактор_Закрыть_Неисправность = 362,

		[EventDescriptionAttribute("Контактор Закрыть", JournalEventNameType.Неисправность_устранена)]
		Контактор_Закрыть_Неисправность_устранена = 363,

		[EventDescriptionAttribute("Обрыв ДУ Открыть", JournalEventNameType.Неисправность)]
		Обрыв_ДУ_Открыть_Неисправность = 364,

		[EventDescriptionAttribute("Обрыв ДУ Открыть", JournalEventNameType.Неисправность_устранена)]
		Обрыв_ДУ_Открыть_Неисправность_устранена = 365,

		[EventDescriptionAttribute("КЗ ДУ Открыть", JournalEventNameType.Неисправность)]
		КЗ_ДУ_Открыть_Неисправность = 366,

		[EventDescriptionAttribute("КЗ ДУ Открыть", JournalEventNameType.Неисправность_устранена)]
		КЗ_ДУ_Открыть_Неисправность_устранена = 367,

		[EventDescriptionAttribute("Обрыв ДУ Закрыть", JournalEventNameType.Неисправность)]
		Обрыв_ДУ_Закрыть_Неисправность = 368,

		[EventDescriptionAttribute("Обрыв ДУ Закрыть", JournalEventNameType.Неисправность_устранена)]
		Обрыв_ДУ_Закрыть_Неисправность_устранена = 369,

		[EventDescriptionAttribute("КЗ ДУ Закрыть", JournalEventNameType.Неисправность)]
		КЗ_ДУ_Закрыть_Неисправность = 370,

		[EventDescriptionAttribute("КЗ ДУ Закрыть", JournalEventNameType.Неисправность_устранена)]
		КЗ_ДУ_Закрыть_Неисправность_устранена = 371,

		[EventDescriptionAttribute("Обрыв ОГВ", JournalEventNameType.Неисправность)]
		Обрыв_ОГВ_Неисправность = 372,

		[EventDescriptionAttribute("Обрыв ОГВ", JournalEventNameType.Неисправность_устранена)]
		Обрыв_ОГВ_Неисправность_устранена = 373,

		[EventDescriptionAttribute("КЗ ОГВ", JournalEventNameType.Неисправность)]
		КЗ_ОГВ_Неисправность = 374,

		[EventDescriptionAttribute("КЗ ОГВ", JournalEventNameType.Неисправность_устранена)]
		КЗ_ОГВ_Неисправность_устранена = 375,

		[EventDescriptionAttribute("Истекло Время Хода", JournalEventNameType.Неисправность)]
		Истекло_Время_Хода_Неисправность = 376,

		[EventDescriptionAttribute("Истекло Время Хода", JournalEventNameType.Неисправность_устранена)]
		Истекло_Время_Хода_Неисправность_устранена = 377,

		[EventDescriptionAttribute("Сигнал МВ без КВ", JournalEventNameType.Неисправность)]
		Сигнал_МВ_без_КВ_Неисправность = 378,

		[EventDescriptionAttribute("Сигнал МВ без КВ", JournalEventNameType.Неисправность)]
		Сигнал_МВ_без_КВ_Неисправность_устранена = 379,

		[EventDescriptionAttribute("Сочетание КВ", JournalEventNameType.Неисправность)]
		Сочетание_КВ_Неисправность = 380,

		[EventDescriptionAttribute("Сочетание КВ", JournalEventNameType.Неисправность_устранена)]
		Сочетание_КВ_Неисправность_устранена = 381,

		[EventDescriptionAttribute("Сочетание МВ", JournalEventNameType.Неисправность)]
		Сочетание_МВ_Неисправность = 382,

		[EventDescriptionAttribute("Сочетание МВ", JournalEventNameType.Неисправность_устранена)]
		Сочетание_МВ_Неисправность_устранена = 383,

		[EventDescriptionAttribute("Сочетание ДНУ и ДВУ", JournalEventNameType.Неисправность)]
		Сочетание_ДНУ_и_ДВУ_Неисправность = 384,

		[EventDescriptionAttribute("Сочетание ДНУ и ДВУ", JournalEventNameType.Неисправность_устранена)]
		Сочетание_ДНУ_и_ДВУ_Неисправность_устранена = 385,

		[EventDescriptionAttribute("КЗ концевого выключателя НОРМА", JournalEventNameType.Неисправность)]
		КЗ_концевого_выключателя_НОРМА_Неисправность = 386,

		[EventDescriptionAttribute("КЗ концевого выключателя НОРМА", JournalEventNameType.Неисправность_устранена)]
		КЗ_концевого_выключателя_НОРМА_Неисправность_устранена = 387,

		[EventDescriptionAttribute("Обрыв концевого выключателя НОРМА", JournalEventNameType.Неисправность)]
		Обрыв_концевого_выключателя_НОРМА_Неисправность = 388,

		[EventDescriptionAttribute("Обрыв концевого выключателя НОРМА", JournalEventNameType.Неисправность_устранена)]
		Обрыв_концевого_выключателя_НОРМА_Неисправность_устранена = 389,

		[EventDescriptionAttribute("КЗ концевого выключателя ЗАЩИТА", JournalEventNameType.Неисправность)]
		КЗ_концевого_выключателя_ЗАЩИТА_Неисправность = 390,

		[EventDescriptionAttribute("КЗ концевого выключателя ЗАЩИТА", JournalEventNameType.Неисправность_устранена)]
		КЗ_концевого_выключателя_ЗАЩИТА_Неисправность_устранена = 391,

		[EventDescriptionAttribute("Обрыв концевого выключателя ЗАЩИТА", JournalEventNameType.Неисправность)]
		Обрыв_концевого_выключателя_ЗАЩИТА_Неисправность = 392,

		[EventDescriptionAttribute("Обрыв концевого выключателя ЗАЩИТА", JournalEventNameType.Неисправность_устранена)]
		Обрыв_концевого_выключателя_ЗАЩИТА_Неисправность_устранена = 393,
		#endregion

		#region Информация
		[EventDescriptionAttribute("Команда от прибора", JournalEventNameType.Информация)]
		Команда_от_прибора = 394,

		[EventDescriptionAttribute("Команда от кнопки", JournalEventNameType.Информация)]
		Команда_от_кнопки = 395,

		[EventDescriptionAttribute("Изменение автоматики по неисправности", JournalEventNameType.Информация)]
		Изменение_автоматики_по_неисправности = 396,

		[EventDescriptionAttribute("Изменение автомат по СТОП", JournalEventNameType.Информация)]
		Изменение_автомат_по_СТОП = 397,

		[EventDescriptionAttribute("Изменение автоматики по Д-О", JournalEventNameType.Информация)]
		Изменение_автоматики_по_Д_О = 398,

		[EventDescriptionAttribute("Изменение автоматики по ТМ", JournalEventNameType.Информация)]
		Изменение_автоматики_по_ТМ = 399,

		[EventDescriptionAttribute("Отсчет задержки", JournalEventNameType.Информация)]
		Отсчет_задержки_2 = 400,

		[EventDescriptionAttribute("Отлож пуск АУП Д-О", JournalEventNameType.Информация)]
		Отлож_пуск_АУП_Д_О = 401,

		[EventDescriptionAttribute("Пуск АУП завершен", JournalEventNameType.Информация)]
		Пуск_АУП_завершен = 402,

		[EventDescriptionAttribute("Стоп по кнопке СТОП", JournalEventNameType.Информация)]
		Стоп_по_кнопке_СТОП = 403,

		[EventDescriptionAttribute("Программирование мастер-ключа", JournalEventNameType.Информация)]
		Программирование_мастер_ключа = 404,

		[EventDescriptionAttribute("Датчик ДАВЛЕНИЕ", JournalEventNameType.Информация)]
		Датчик_ДАВЛЕНИЕ = 405,

		[EventDescriptionAttribute("Датчик МАССА", JournalEventNameType.Информация)]
		Датчик_МАССА = 406,

		[EventDescriptionAttribute("Сигнал из памяти", JournalEventNameType.Информация)]
		Сигнал_из_памяти = 407,

		[EventDescriptionAttribute("Сигнал аналог входа", JournalEventNameType.Информация)]
		Сигнал_аналог_входа = 408,

		[EventDescriptionAttribute("Замена списка на 1", JournalEventNameType.Информация)]
		Замена_списка_на_1 = 409,

		[EventDescriptionAttribute("Замена списка на 2", JournalEventNameType.Информация)]
		Замена_списка_на_2 = 410,

		[EventDescriptionAttribute("Замена списка на 3", JournalEventNameType.Информация)]
		Замена_списка_на_3 = 411,

		[EventDescriptionAttribute("Замена списка на 4", JournalEventNameType.Информация)]
		Замена_списка_на_4 = 412,

		[EventDescriptionAttribute("Замена списка на 5", JournalEventNameType.Информация)]
		Замена_списка_на_5 = 413,

		[EventDescriptionAttribute("Замена списка на 6", JournalEventNameType.Информация)]
		Замена_списка_на_6 = 414,

		[EventDescriptionAttribute("Замена списка на 7", JournalEventNameType.Информация)]
		Замена_списка_на_7 = 415,

		[EventDescriptionAttribute("Замена списка на 8", JournalEventNameType.Информация)]
		Замена_списка_на_8 = 416,

		[EventDescriptionAttribute("Низкий уровень", JournalEventNameType.Информация)]
		Низкий_уровень = 417,

		[EventDescriptionAttribute("Высокий уровень", JournalEventNameType.Информация)]
		Высокий_уровень = 418,

		[EventDescriptionAttribute("Уровень норма", JournalEventNameType.Информация)]
		Уровень_норма = 419,

		[EventDescriptionAttribute("Перевод в автоматический режим со шкафа", JournalEventNameType.Информация)]
		Перевод_в_автоматический_режим_со_шкафа = 420,

		[EventDescriptionAttribute("Перевод в ручной режим со шкафа", JournalEventNameType.Информация)]
		Перевод_в_ручной_режим_со_шкафа = 421,

		[EventDescriptionAttribute("Перевод в отключенный режим со шкафа", JournalEventNameType.Информация)]
		Перевод_в_отключенный_режим_со_шкафа = 422,

		[EventDescriptionAttribute("Неопределено", JournalEventNameType.Информация)]
		Неопределено = 423,

		[EventDescriptionAttribute("Пуск невозможен", JournalEventNameType.Информация)]
		Пуск_невозможен = 424,

		[EventDescriptionAttribute("Авария пневмоемкости", JournalEventNameType.Информация)]
		Авария_пневмоемкости = 425,

		[EventDescriptionAttribute("Аварийный уровень", JournalEventNameType.Информация)]
		Аварийный_уровень_Информация = 426,

		[EventDescriptionAttribute("Запрет пуска НС", JournalEventNameType.Информация)]
		Запрет_пуска_НС = 427,

		[EventDescriptionAttribute("Запрет пуска компрессора", JournalEventNameType.Информация)]
		Запрет_пуска_компрессора = 428,

		[EventDescriptionAttribute("Ввод 1", JournalEventNameType.Информация)]
		Ввод_1 = 429,

		[EventDescriptionAttribute("Ввод 2", JournalEventNameType.Информация)]
		Ввод_2 = 430,

		[EventDescriptionAttribute("Команда от логики", JournalEventNameType.Информация)]
		Команда_от_логики = 431,

		[EventDescriptionAttribute("Команда от ДУ", JournalEventNameType.Информация)]
		Команда_от_ДУ = 432,

		[EventDescriptionAttribute("Давление низкое", JournalEventNameType.Информация)]
		Давление_низкое = 433,

		[EventDescriptionAttribute("Давление высокое", JournalEventNameType.Информация)]
		Давление_высокое = 434,

		[EventDescriptionAttribute("Давление норма", JournalEventNameType.Информация)]
		Давление_норма = 435,

		[EventDescriptionAttribute("Давление неопределен", JournalEventNameType.Информация)]
		Давление_неопределен = 436,

		[EventDescriptionAttribute("Сигнал с ДВнР есть", JournalEventNameType.Информация)]
		Сигнал_с_ДВнР_есть = 437,

		[EventDescriptionAttribute("Сигнал с ДВнР нет", JournalEventNameType.Информация)]
		Сигнал_с_ДВнР_нет = 438,

		[EventDescriptionAttribute("ШУ вышел на режим", JournalEventNameType.Информация)]
		ШУ_вышел_на_режим = 439,

		[EventDescriptionAttribute("Блокировка пуска", JournalEventNameType.Информация)]
		Блокировка_пуска_2 = 440,

		[EventDescriptionAttribute("Блокировка пуска снята", JournalEventNameType.Информация)]
		Блокировка_пуска_снята = 441,

		[EventDescriptionAttribute("Остановлено", JournalEventNameType.Информация)]
		Остановлено = 442,

		[EventDescriptionAttribute("Отсчет задержки", JournalEventNameType.Информация)]
		Отсчет_задержки = 443,

		[EventDescriptionAttribute("Аварии пневмоемкости нет", JournalEventNameType.Информация)]
		Аварии_пневмоемкости_нет = 444,

		[EventDescriptionAttribute("Пуск с УЗ", JournalEventNameType.Информация)]
		Пуск_с_УЗ = 445,

		[EventDescriptionAttribute("Пуск автоматический", JournalEventNameType.Информация)]
		Пуск_автоматический = 446,

		[EventDescriptionAttribute("Пуск ручной", JournalEventNameType.Информация)]
		Пуск_ручной = 447,

		[EventDescriptionAttribute("Пуск с панели шкафа", JournalEventNameType.Информация)]
		Пуск_с_панели_шкафа = 448,

		[EventDescriptionAttribute("Стоп от переключателя", JournalEventNameType.Информация)]
		Стоп_от_переключателя = 449,

		[EventDescriptionAttribute("Стоп с панели шкафа", JournalEventNameType.Информация)]
		Стоп_с_панели_шкафа = 450,

		[EventDescriptionAttribute("Стоп по неисправности", JournalEventNameType.Информация)]
		Стоп_по_неисправности = 451,

		[EventDescriptionAttribute("Стоп с УЗ", JournalEventNameType.Информация)]
		Стоп_с_УЗ = 452,

		[EventDescriptionAttribute("Стоп автоматический", JournalEventNameType.Информация)]
		Стоп_автоматический = 453,

		[EventDescriptionAttribute("Стоп ручной", JournalEventNameType.Информация)]
		Стоп_ручной = 454,

		[EventDescriptionAttribute("Отсчет задержки включен", JournalEventNameType.Информация)]
		Отсчет_задержки_включен = 455,

		[EventDescriptionAttribute("Отсчет задержки выключен", JournalEventNameType.Информация)]
		Отсчет_задержки_выключен = 456,

		[EventDescriptionAttribute("Откр с УЗ", JournalEventNameType.Информация)]
		Откр_с_УЗ = 457,

		[EventDescriptionAttribute("Откр автоматический", JournalEventNameType.Информация)]
		Откр_автоматический = 458,

		[EventDescriptionAttribute("Откр ручной", JournalEventNameType.Информация)]
		Откр_ручной = 459,

		[EventDescriptionAttribute("Откр с панели шкафа", JournalEventNameType.Информация)]
		Откр_с_панели_шкафа = 460,

		[EventDescriptionAttribute("Закр от переключателя", JournalEventNameType.Информация)]
		Закр_от_переключателя = 461,

		[EventDescriptionAttribute("Закр с панели шкафа", JournalEventNameType.Информация)]
		Закр_с_панели_шкафа = 462,

		[EventDescriptionAttribute("Закр по неисправности", JournalEventNameType.Информация)]
		Закр_по_неисправности = 463,

		[EventDescriptionAttribute("Закр с УЗ", JournalEventNameType.Информация)]
		Закр_с_УЗ = 464,

		[EventDescriptionAttribute("Закр автоматический", JournalEventNameType.Информация)]
		Закр_автоматический = 465,

		[EventDescriptionAttribute("Закр ручной", JournalEventNameType.Информация)]
		Закр_ручной = 466,

		[EventDescriptionAttribute("Старт ОГВ", JournalEventNameType.Информация)]
		Старт_ОГВ = 467,

		[EventDescriptionAttribute("Стоп ОГВ", JournalEventNameType.Информация)]
		Стоп_ОГВ = 468,

		[EventDescriptionAttribute("Команда с панели шкафа", JournalEventNameType.Информация)]
		Команда_с_панели_шкафа = 469,

		[EventDescriptionAttribute("Ход на открытие", JournalEventNameType.Информация)]
		Ход_на_открытие = 470,

		[EventDescriptionAttribute("Ход на закрытие", JournalEventNameType.Информация)]
		Ход_на_закрытие = 471,

		[EventDescriptionAttribute("Уровень неопределен", JournalEventNameType.Информация)]
		Уровень_неопределен = 472,
		#endregion

		#region Вход_пользователя_в_прибор
		[EventDescriptionAttribute("Оператор", JournalEventNameType.Вход_пользователя_в_прибор)]
		Оператор_Вход_пользователя_в_прибор = 473,

		[EventDescriptionAttribute("Администратор", JournalEventNameType.Вход_пользователя_в_прибор)]
		Администратор_Вход_пользователя_в_прибор = 474,

		[EventDescriptionAttribute("Инсталлятор", JournalEventNameType.Вход_пользователя_в_прибор)]
		Инсталлятор_Вход_пользователя_в_прибор = 475,

		[EventDescriptionAttribute("Изготовитель", JournalEventNameType.Вход_пользователя_в_прибор)]
		Изготовитель_Вход_пользователя_в_прибор = 478,
		#endregion

		#region Выход_пользователя_из_прибора
		[EventDescriptionAttribute("Оператор", JournalEventNameType.Выход_пользователя_из_прибора)]
		Оператор_Выход_пользователя_из_прибора = 479,

		[EventDescriptionAttribute("Администратор", JournalEventNameType.Выход_пользователя_из_прибора)]
		Администратор_Выход_пользователя_из_прибора = 480,

		[EventDescriptionAttribute("Инсталлятор", JournalEventNameType.Выход_пользователя_из_прибора)]
		Инсталлятор_Выход_пользователя_из_прибора = 481,

		[EventDescriptionAttribute("Изготовитель", JournalEventNameType.Выход_пользователя_из_прибора)]
		Изготовитель_Выход_пользователя_из_прибора = 482,
		#endregion

		#region Запылённость
		[EventDescriptionAttribute("Предварительная", JournalEventNameType.Запыленность)]
		Предварительная_Запыленность = 483,

		[EventDescriptionAttribute("Критическая", JournalEventNameType.Запыленность)]
		Критическая_Запыленность = 484,
		#endregion

		#region Запылённость_устранена
		[EventDescriptionAttribute("Предварительная", JournalEventNameType.Запыленность_устранена)]
		Предварительная_Запыленность_устранена = 485,

		[EventDescriptionAttribute("Критическая", JournalEventNameType.Запыленность_устранена)]
		Критическая_Запыленность_устранена = 486,
		#endregion

		#region сотрудник
		[EventDescriptionAttribute("Добавление", JournalEventNameType.Редактирование_сотрудника)]
		Добавление_сотрудник = 487,

		[EventDescriptionAttribute("Редактирование", JournalEventNameType.Редактирование_сотрудника)]
		Редактирование_сотрудник = 488,

		[EventDescriptionAttribute("Удаление", JournalEventNameType.Редактирование_сотрудника)]
		Удаление_сотрудник = 489,

		[EventDescriptionAttribute("Восстановление", JournalEventNameType.Редактирование_сотрудника)]
		Восстановление_сотрудник = 490,
		#endregion

		#region отдел
		[EventDescriptionAttribute("Добавление", JournalEventNameType.Редактирование_отдела)]
		Добавление_отдел = 491,

		[EventDescriptionAttribute("Редактирование", JournalEventNameType.Редактирование_отдела)]
		Редактирование_отдел = 492,

		[EventDescriptionAttribute("Удаление", JournalEventNameType.Редактирование_отдела)]
		Удаление_отдел = 493,

		[EventDescriptionAttribute("Восстановление", JournalEventNameType.Редактирование_отдела)]
		Восстановление_отдел = 494,
		#endregion

		#region должность
		[EventDescriptionAttribute("Добавление", JournalEventNameType.Редактирование_должности)]
		Добавление_должность = 495,

		[EventDescriptionAttribute("Редактирование", JournalEventNameType.Редактирование_должности)]
		Редактирование_должность = 496,

		[EventDescriptionAttribute("Удаление", JournalEventNameType.Редактирование_должности)]
		Удаление_должность = 497,

		[EventDescriptionAttribute("Восстановление", JournalEventNameType.Редактирование_должности)]
		Восстановление_должность = 498,
		#endregion

		#region шаблона_доступ
		[EventDescriptionAttribute("Добавление", JournalEventNameType.Редактирование_шаблона_доступа)]
		Добавление_шаблона_доступ = 499,

		[EventDescriptionAttribute("Редактирование", JournalEventNameType.Редактирование_шаблона_доступа)]
		Редактирование_шаблона_доступ = 500,

		[EventDescriptionAttribute("Удаление", JournalEventNameType.Редактирование_шаблона_доступа)]
		Удаление_шаблона_доступ = 501,

		[EventDescriptionAttribute("Восстановление", JournalEventNameType.Редактирование_шаблона_доступа)]
		Восстановление_шаблона_доступ = 502,
		#endregion

		#region организация
		[EventDescriptionAttribute("Добавление", JournalEventNameType.Редактирование_организации)]
		Добавление_организация = 503,

		[EventDescriptionAttribute("Редактирование", JournalEventNameType.Редактирование_организации)]
		Редактирование_организация = 504,

		[EventDescriptionAttribute("Удаление", JournalEventNameType.Редактирование_организации)]
		Удаление_организация = 505,

		[EventDescriptionAttribute("Восстановление", JournalEventNameType.Редактирование_организации)]
		Восстановление_организация = 506,
		#endregion

		#region дополнительная_колонка
		[EventDescriptionAttribute("Добавление", JournalEventNameType.Редактирование_дополнительной_колонки)]
		Добавление_дополнительная_колонка = 507,

		[EventDescriptionAttribute("Редактирование", JournalEventNameType.Редактирование_дополнительной_колонки)]
		Редактирование_дополнительная_колонка = 508,

		[EventDescriptionAttribute("Удаление", JournalEventNameType.Редактирование_дополнительной_колонки)]
		Удаление_дополнительная_колонка = 509,

		[EventDescriptionAttribute("Восстановление", JournalEventNameType.Редактирование_дополнительной_колонки)]
		Восстановление_дополнительная_колонка = 510,
		#endregion

		#region дневной_график
		[EventDescriptionAttribute("Добавление", JournalEventNameType.Редактирование_дневного_графика)]
		Добавление_дневной_график = 511,

		[EventDescriptionAttribute("Редактирование", JournalEventNameType.Редактирование_дневного_графика)]
		Редактирование_дневной_график = 512,

		[EventDescriptionAttribute("Удаление", JournalEventNameType.Редактирование_дневного_графика)]
		Удаление_дневной_график = 513,

		[EventDescriptionAttribute("Восстановление", JournalEventNameType.Редактирование_дневного_графика)]
		Восстановление_дневной_график = 514,
		#endregion

		#region график_работы
		[EventDescriptionAttribute("Добавление", JournalEventNameType.Редактирование_графика_работы)]
		Добавление_график_работы = 515,

		[EventDescriptionAttribute("Редактирование", JournalEventNameType.Редактирование_графика_работы)]
		Редактирование_график_работы = 516,

		[EventDescriptionAttribute("Удаление", JournalEventNameType.Редактирование_графика_работы)]
		Удаление_график_работы = 517,

		[EventDescriptionAttribute("Восстановление", JournalEventNameType.Редактирование_графика_работы)]
		Восстановление_график_работы = 518,
		#endregion

		#region график_работы_сотрудника
		[EventDescriptionAttribute("Добавление", JournalEventNameType.Редактирование_графика_работы_сотрудника)]
		Добавление_график_работы_сотрудника = 519,

		[EventDescriptionAttribute("Редактирование", JournalEventNameType.Редактирование_графика_работы_сотрудника)]
		Редактирование_график_работы_сотрудника = 520,

		[EventDescriptionAttribute("Удаление", JournalEventNameType.Редактирование_графика_работы_сотрудника)]
		Удаление_график_работы_сотрудника = 521,

		[EventDescriptionAttribute("Восстановление", JournalEventNameType.Редактирование_графика_работы_сотрудника)]
		Восстановление_график_работы_сотрудника = 522,
		#endregion

		#region праздничный_день
		[EventDescriptionAttribute("Добавление", JournalEventNameType.Редактирование_праздничного_дня)]
		Добавление_праздничный_день = 523,

		[EventDescriptionAttribute("Редактирование", JournalEventNameType.Редактирование_праздничного_дня)]
		Редактирование_праздничный_день = 524,

		[EventDescriptionAttribute("Удаление", JournalEventNameType.Редактирование_праздничного_дня)]
		Удаление_праздничный_день = 525,

		[EventDescriptionAttribute("Восстановление", JournalEventNameType.Редактирование_праздничного_дня)]
		Восстановление_праздничный_день = 526,
		#endregion

		#region Проход_пользователя_разрешен
		[EventDescriptionAttribute("Вход", JournalEventNameType.Проход_пользователя_разрешен)]
		Вход_Глобал = 527,

		[EventDescriptionAttribute("Выход", JournalEventNameType.Проход_пользователя_разрешен)]
		Выход_Глобал = 528,
		#endregion

		#region Управление_ПМФ
		[EventDescriptionAttribute("Перевод в ручной режим", JournalEventNameType.Управление_ПМФ)]
		Перевод_в_ручной_режим_Управление_ПМФ = 529,

		[EventDescriptionAttribute("Перевод в автоматический режим", JournalEventNameType.Управление_ПМФ)]
		Перевод_в_автоматический_режим_Управление_ПМФ = 530,

		[EventDescriptionAttribute("Сброс", JournalEventNameType.Управление_ПМФ)]
		Сброс_Управление_ПМФ = 531,
		
		[EventDescriptionAttribute("Постановка на охрану", JournalEventNameType.Управление_ПМФ)]
		Постановка_на_охрану = 532,

		[EventDescriptionAttribute("Снятие с охраны", JournalEventNameType.Управление_ПМФ)]
		Снятие_с_охраны = 533,

		[EventDescriptionAttribute("Пуск", JournalEventNameType.Управление_ПМФ)]
		Пуск = 534,

		[EventDescriptionAttribute("Стоп", JournalEventNameType.Управление_ПМФ)]
		Стоп = 535,
		#endregion

		#region посетитель
		[EventDescriptionAttribute("Добавление", JournalEventNameType.Редактирование_посетителя)]
		Добавление_посетитель = 536,

		[EventDescriptionAttribute("Редактирование", JournalEventNameType.Редактирование_посетителя)]
		Редактирование_посетитель = 537,

		[EventDescriptionAttribute("Удаление", JournalEventNameType.Редактирование_посетителя)]
		Удаление_посетитель = 538,

		[EventDescriptionAttribute("Восстановление", JournalEventNameType.Редактирование_посетителя)]
		Восстановление_посетитель = 539,
		#endregion

		#region шаблон_пропуска
		[EventDescriptionAttribute("Добавление", JournalEventNameType.Редактирование_шаблона_пропуска)]
		Добавление_шаблон_пропуска = 540,

		[EventDescriptionAttribute("Редактирование", JournalEventNameType.Редактирование_шаблона_пропуска)]
		Редактирование_шаблон_пропуска = 541,

		[EventDescriptionAttribute("Удаление", JournalEventNameType.Редактирование_шаблона_пропуска)]
		Удаление_шаблон_пропуска = 542,

		[EventDescriptionAttribute("Восстановление", JournalEventNameType.Редактирование_шаблона_пропуска)]
		Восстановление_шаблон_пропуска = 543,
		#endregion

		#region управление турникетом
		[EventDescriptionAttribute("Открыть турникет на вход", JournalEventNameType.Команда_оператора)]
		Открыть_турникет_на_вход = 544,

		[EventDescriptionAttribute("Открыть турникет на выход", JournalEventNameType.Команда_оператора)]
		Открыть_турникет_на_выход = 545,

		[EventDescriptionAttribute("Перевести турникет в режим всегда открыто на вход", JournalEventNameType.Команда_оператора)]
		Перевести_турникет_в_режим_всегда_открыто_на_вход = 546,

		[EventDescriptionAttribute("Перевести турникет в режим всегда открыто на выход", JournalEventNameType.Команда_оператора)]
		Перевести_турникет_в_режим_всегда_открыто_на_выход = 547,

		[EventDescriptionAttribute("Перевести турникет в норму", JournalEventNameType.Команда_оператора)]
		Перевести_турникет_в_норму = 548,
		#endregion
	}
}