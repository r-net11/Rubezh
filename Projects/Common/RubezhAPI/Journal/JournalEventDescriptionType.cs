
namespace RubezhAPI.Journal
{
	public enum JournalEventDescriptionType
	{
		[EventDescriptionAttribute("")]
		NULL = 0,

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

		[EventDescriptionAttribute("Перевод в ручной режим", JournalEventNameType.Команда_оператора, JournalEventNameType.Управление_ПМФ)]
		Перевод_в_ручной_режим = 10,

		[EventDescriptionAttribute("Перевод в автоматический режим", JournalEventNameType.Команда_оператора, JournalEventNameType.Управление_ПМФ)]
		Перевод_в_автоматический_режим = 11,

		[EventDescriptionAttribute("Перевод в отключенный режим", JournalEventNameType.Команда_оператора)]
		Перевод_в_отключенный_режим = 12,

		[EventDescriptionAttribute("Сброс", JournalEventNameType.Команда_оператора, JournalEventNameType.Управление_ПМФ)]
		Сброс = 13,

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

		[EventDescriptionAttribute("Ручник сорван", JournalEventNameType.Сработка_2)]
		Ручник_сорван = 31,

		[EventDescriptionAttribute("Срабатывание по дыму", JournalEventNameType.Сработка_1)]
		Срабатывание_по_дыму = 32,

		[EventDescriptionAttribute("Срабатывание по температуре", JournalEventNameType.Сработка_1)]
		Срабатывание_по_температуре = 33,

		[EventDescriptionAttribute("Срабатывание по градиенту температуры", JournalEventNameType.Сработка_1)]
		Срабатывание_по_градиенту_температуры = 34,

		[EventDescriptionAttribute("Напряжение питания устройства не в норме", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Напряжение_питания_устройства_не_в_норме = 35,

		[EventDescriptionAttribute("Оптический канал или фотоусилитель", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Оптический_канал_или_фотоусилитель = 36,

		[EventDescriptionAttribute("Температурный канал", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Температурный_канал = 37,

		[EventDescriptionAttribute("КЗ ШС", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_ШС = 38,

		[EventDescriptionAttribute("Обрыв ШС", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_ШС = 39,

		[EventDescriptionAttribute("Вскрытие корпуса", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Вскрытие_корпуса = 40,

		[EventDescriptionAttribute("Контакт не переключается", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Контакт_не_переключается = 41,

		[EventDescriptionAttribute("Напряжение запуска реле ниже нормы", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Напряжение_запуска_реле_ниже_нормы = 42,

		[EventDescriptionAttribute("КЗ выхода", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_выхода = 43,

		[EventDescriptionAttribute("Обрыв выхода", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_выхода = 44,

		[EventDescriptionAttribute("Напряжение питания ШС ниже нормы", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Напряжение_питания_ШС_ниже_нормы = 45,

		[EventDescriptionAttribute("Ошибка памяти", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Ошибка_памяти = 46,

		[EventDescriptionAttribute("КЗ выхода 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_выхода_1 = 47,

		[EventDescriptionAttribute("КЗ выхода 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_выхода_2 = 48,

		[EventDescriptionAttribute("КЗ выхода 3", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_выхода_3 = 49,

		[EventDescriptionAttribute("КЗ выхода 4", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_выхода_4 = 50,

		[EventDescriptionAttribute("КЗ выхода 5", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_выхода_5 = 51,

		[EventDescriptionAttribute("Обрыв выхода 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_выхода_1 = 52,

		[EventDescriptionAttribute("Обрыв выхода 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_выхода_2 = 53,

		[EventDescriptionAttribute("Обрыв выхода 3", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_выхода_3 = 54,

		[EventDescriptionAttribute("Обрыв выхода 4", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_выхода_4 = 55,

		[EventDescriptionAttribute("Обрыв выхода 5", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_выхода_5 = 56,

		[EventDescriptionAttribute("Блокировка пуска", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Блокировка_пуска = 57,

		[EventDescriptionAttribute("Низкое напряжение питания привода", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Низкое_напряжение_питания_привода = 58,

		[EventDescriptionAttribute("Обрыв кнопки НОРМА", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_кнопки_НОРМА = 59,

		[EventDescriptionAttribute("КЗ кнопки НОРМА", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_кнопки_НОРМА = 60,

		[EventDescriptionAttribute("Обрыв кнопка ЗАЩИТА", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_кнопка_ЗАЩИТА = 61,

		[EventDescriptionAttribute("КЗ кнопки ЗАЩИТА", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_кнопки_ЗАЩИТА = 62,

		[EventDescriptionAttribute("Обрыв концевого выключателя ОТКРЫТО", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_концевого_выключателя_ОТКРЫТО = 63,

		[EventDescriptionAttribute("Обрыв концевого выключателя ЗАКРЫТО", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_концевого_выключателя_ЗАКРЫТО = 64,

		[EventDescriptionAttribute("Обрыв цепи ПД ЗАЩИТА", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_цепи_ПД_ЗАЩИТА = 65,

		[EventDescriptionAttribute("Замкнуты/разомкнуты оба концевика", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Замкнуты_разомкнуты_оба_концевика = 66,

		[EventDescriptionAttribute("Превышение времени хода", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Превышение_времени_хода = 67,

		[EventDescriptionAttribute("Обрыв в линии РЕЛЕ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_в_линии_РЕЛЕ = 68,

		[EventDescriptionAttribute("КЗ в линии РЕЛЕ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_в_линии_РЕЛЕ = 69,

		[EventDescriptionAttribute("Выход 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Выход_1 = 70,

		[EventDescriptionAttribute("Выход 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Выход_2 = 71,

		[EventDescriptionAttribute("Выход 3", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Выход_3 = 72,

		[EventDescriptionAttribute("КЗ концевого выключателя ОТКРЫТО", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_концевого_выключателя_ОТКРЫТО = 73,

		[EventDescriptionAttribute("Обрыв муфтового выключателя ОТКРЫТО", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_муфтового_выключателя_ОТКРЫТО = 74,

		[EventDescriptionAttribute("КЗ муфтового выключателя ОТКРЫТО", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_муфтового_выключателя_ОТКРЫТО = 75,

		[EventDescriptionAttribute("КЗ концевого выключателя ЗАКРЫТО", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_концевого_выключателя_ЗАКРЫТО = 76,

		[EventDescriptionAttribute("Обрыв муфтового выключателя ЗАКРЫТО", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_муфтового_выключателя_ЗАКРЫТО = 77,

		[EventDescriptionAttribute("КЗ муфтового выключателя ЗАКРЫТО", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_муфтового_выключателя_ЗАКРЫТО = 78,

		[EventDescriptionAttribute("Обрыв кнопки Открыть УЗЗ/Закрыть УЗЗ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_кнопки_Открыть_УЗЗ_Закрыть_УЗЗ = 79,

		[EventDescriptionAttribute("КЗ кнопки Открыть УЗЗ/Закрыть УЗЗ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_кнопки_Открыть_УЗЗ_Закрыть_УЗЗ = 80,

		[EventDescriptionAttribute("Обрыв кнопки СТОП УЗЗ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_кнопки_СТОП_УЗЗ = 81,

		[EventDescriptionAttribute("КЗ кнопки СТОП УЗЗ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_кнопки_СТОП_УЗЗ = 82,

		[EventDescriptionAttribute("Обрыв давление низкое", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_давление_низкое = 83,

		[EventDescriptionAttribute("КЗ давление низкое", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_давление_низкое = 84,

		[EventDescriptionAttribute("Таймаут по давлению", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Таймаут_по_давлению = 85,

		[EventDescriptionAttribute("КВ/МВ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КВ_МВ = 86,

		[EventDescriptionAttribute("Не задан режим", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Не_задан_режим = 87,

		[EventDescriptionAttribute("Отказ ШУЗ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Отказ_ШУЗ = 88,

		[EventDescriptionAttribute("ДУ/ДД", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		ДУ_ДД = 89,

		[EventDescriptionAttribute("Обрыв входа 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_входа_1 = 90,

		[EventDescriptionAttribute("КЗ входа 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_входа_1 = 91,

		[EventDescriptionAttribute("Обрыв входа 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_входа_2 = 92,

		[EventDescriptionAttribute("КЗ входа 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_входа_2 = 93,

		[EventDescriptionAttribute("Обрыв входа 3", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_входа_3 = 94,

		[EventDescriptionAttribute("КЗ входа 3", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_входа_3 = 95,

		[EventDescriptionAttribute("Обрыв входа 4", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_входа_4 = 96,

		[EventDescriptionAttribute("КЗ входа 4", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_входа_4 = 97,

		[EventDescriptionAttribute("Не задан тип", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Не_задан_тип = 98,

		[EventDescriptionAttribute("Отказ ПН", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Отказ_ПН = 99,

		[EventDescriptionAttribute("Отказ ШУН", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Отказ_ШУН = 100,

		[EventDescriptionAttribute("Питание 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Питание_1 = 101,

		[EventDescriptionAttribute("Питание 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Питание_2 = 102,

		[EventDescriptionAttribute("Отказ АЛС 1 или 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Отказ_АЛС_1_или_2 = 103,

		[EventDescriptionAttribute("Отказ АЛС 3 или 4", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Отказ_АЛС_3_или_4 = 104,

		[EventDescriptionAttribute("Отказ АЛС 5 или 6", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Отказ_АЛС_5_или_6 = 105,

		[EventDescriptionAttribute("Отказ АЛС 7 или 8", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Отказ_АЛС_7_или_8 = 106,

		[EventDescriptionAttribute("Обрыв цепи ПД НОРМА", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_цепи_ПД_НОРМА = 107,

		[EventDescriptionAttribute("КЗ АЛС 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_1 = 108,

		[EventDescriptionAttribute("КЗ АЛС 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_2 = 109,

		[EventDescriptionAttribute("КЗ АЛС 3", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_3 = 110,

		[EventDescriptionAttribute("КЗ АЛС 4", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_4 = 111,

		[EventDescriptionAttribute("КЗ АЛС 5", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_5 = 112,

		[EventDescriptionAttribute("КЗ АЛС 6", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_6 = 113,

		[EventDescriptionAttribute("КЗ АЛС 7", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_7 = 114,

		[EventDescriptionAttribute("КЗ АЛС 8", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_8 = 115,

		[EventDescriptionAttribute("Истекло время вкл", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Истекло_время_вкл = 116,

		[EventDescriptionAttribute("Истекло время выкл", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Истекло_время_выкл = 117,

		[EventDescriptionAttribute("Контакт реле 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Контакт_реле_1 = 118,

		[EventDescriptionAttribute("Контакт реле 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Контакт_реле_2 = 119,

		[EventDescriptionAttribute("Обрыв кнопки ПУСК", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_кнопки_ПУСК = 120,

		[EventDescriptionAttribute("КЗ кнопки ПУСК", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_кнопки_ПУСК = 121,

		[EventDescriptionAttribute("Обрыв кнопки СТОП", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_кнопки_СТОП = 122,

		[EventDescriptionAttribute("КЗ кнопки СТОП", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_кнопки_СТОП = 123,

		[EventDescriptionAttribute("Отсутствуют или испорчены сообщения для воспроизведения", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Отсутствуют_или_испорчены_сообщения_для_воспроизведения = 124,

		[EventDescriptionAttribute("Выход", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Выход = 125,

		[EventDescriptionAttribute("Обрыв Низкий уровень", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_Низкий_уровень = 126,

		[EventDescriptionAttribute("КЗ Низкий уровень", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_Низкий_уровень = 127,

		[EventDescriptionAttribute("Обрыв Высокий уровень", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_Высокий_уровень = 128,

		[EventDescriptionAttribute("КЗ Высокий уровень", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_Высокий_уровень = 129,

		[EventDescriptionAttribute("Обрыв Аварийный уровень", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_Аварийный_уровень = 130,

		[EventDescriptionAttribute("КЗ Аварийный уровень", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_Аварийный_уровень = 131,

		[EventDescriptionAttribute("Аварийный уровень", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Аварийный_уровень = 132,

		[EventDescriptionAttribute("Питание силовое", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Питание_силовое = 133,

		[EventDescriptionAttribute("Питание контроллера", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Питание_контроллера = 134,

		[EventDescriptionAttribute("Несовместимость сигналов", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Несовместимость_сигналов = 135,

		[EventDescriptionAttribute("Обрыв цепи питания двигателя", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_цепи_питания_двигателя = 136,

		[EventDescriptionAttribute("Обрыв Давление на выходе", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_Давление_на_выходе = 137,

		[EventDescriptionAttribute("КЗ Давление на выходе", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_Давление_на_выходе = 138,

		[EventDescriptionAttribute("Обрыв ДУ ПУСК", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_ДУ_ПУСК = 139,

		[EventDescriptionAttribute("КЗ ДУ ПУСК", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_ДУ_ПУСК = 140,

		[EventDescriptionAttribute("Обрыв ДУ СТОП", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_ДУ_СТОП = 141,

		[EventDescriptionAttribute("КЗ ДУ СТОП", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_ДУ_СТОП = 142,

		[EventDescriptionAttribute("АЛС 1 Неизвестное устройство", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_1_Неизвестное_устройство = 143,

		[EventDescriptionAttribute("АЛС 2 Неизвестное устройство", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_2_Неизвестное_устройство = 144,

		[EventDescriptionAttribute("АЛС 3 Неизвестное устройство", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_3_Неизвестное_устройство = 145,

		[EventDescriptionAttribute("АЛС 4 Неизвестное устройство", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_4_Неизвестное_устройство = 146,

		[EventDescriptionAttribute("АЛС 5 Неизвестное устройство", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_5_Неизвестное_устройство = 147,

		[EventDescriptionAttribute("АЛС 6 Неизвестное устройство", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_6_Неизвестное_устройство = 148,

		[EventDescriptionAttribute("АЛС 7 Неизвестное устройство", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_7_Неизвестное_устройство = 149,

		[EventDescriptionAttribute("АЛС 8 Неизвестное устройство", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_8_Неизвестное_устройство = 150,

		[EventDescriptionAttribute("АЛС 1 Неизвестный тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_1_Неизвестный_тип_устройства = 151,

		[EventDescriptionAttribute("АЛС 2 Неизвестный тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_2_Неизвестный_тип_устройства = 152,

		[EventDescriptionAttribute("АЛС 3 Неизвестный тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_3_Неизвестный_тип_устройства = 153,

		[EventDescriptionAttribute("АЛС 4 Неизвестный тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_4_Неизвестный_тип_устройства = 154,

		[EventDescriptionAttribute("АЛС 5 Неизвестный тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_5_Неизвестный_тип_устройства = 155,

		[EventDescriptionAttribute("АЛС 6 Неизвестный тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_6_Неизвестный_тип_устройства = 156,

		[EventDescriptionAttribute("АЛС 7 Неизвестный тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_7_Неизвестный_тип_устройства = 157,

		[EventDescriptionAttribute("АЛС 8 Неизвестный тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_8_Неизвестный_тип_устройства = 158,

		[EventDescriptionAttribute("АЛС 1 Другой тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_1_Другой_тип_устройства = 159,

		[EventDescriptionAttribute("АЛС 2 Другой тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_2_Другой_тип_устройства = 160,

		[EventDescriptionAttribute("АЛС 3 Другой тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_3_Другой_тип_устройства = 161,

		[EventDescriptionAttribute("АЛС 4 Другой тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_4_Другой_тип_устройства = 162,

		[EventDescriptionAttribute("АЛС 5 Другой тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_5_Другой_тип_устройства = 163,

		[EventDescriptionAttribute("АЛС 6 Другой тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_6_Другой_тип_устройства = 164,

		[EventDescriptionAttribute("АЛС 7 Другой тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_7_Другой_тип_устройства = 165,

		[EventDescriptionAttribute("АЛС 8 Другой тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_8_Другой_тип_устройства = 166,

		[EventDescriptionAttribute("Обрыв АЛС 1-2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_1_2 = 167,

		[EventDescriptionAttribute("Обрыв АЛС 3-4", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_3_4 = 168,

		[EventDescriptionAttribute("Обрыв АЛС 5-6", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_5_6 = 169,

		[EventDescriptionAttribute("Обрыв АЛС 7-8", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_7_8 = 170,

		[EventDescriptionAttribute("Обрыв АЛС 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_1 = 171,

		[EventDescriptionAttribute("Обрыв АЛС 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_2 = 172,

		[EventDescriptionAttribute("Обрыв АЛС 3", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_3 = 173,

		[EventDescriptionAttribute("Обрыв АЛС 4", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_4 = 174,

		[EventDescriptionAttribute("Обрыв АЛС 5", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_5 = 175,

		[EventDescriptionAttribute("Обрыв АЛС 6", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_6 = 176,

		[EventDescriptionAttribute("Обрыв АЛС 7", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_7 = 177,

		[EventDescriptionAttribute("Обрыв АЛС 8", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_8 = 178,

		[EventDescriptionAttribute("ОЛС", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		ОЛС = 179,

		[EventDescriptionAttribute("РЛС", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		РЛС = 180,

		[EventDescriptionAttribute("Потеря связи", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Потеря_связи = 181,

		[EventDescriptionAttribute("Отсутствие сетевого напряжения", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Отсутствие_сетевого_напряжения = 182,

		[EventDescriptionAttribute("КЗ Выхода 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_Выхода_1 = 183,

		[EventDescriptionAttribute("Перегрузка Выхода 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Перегрузка_Выхода_1 = 184,

		[EventDescriptionAttribute("Напряжение Выхода 1 выше нормы", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Напряжение_Выхода_1_выше_нормы = 185,

		[EventDescriptionAttribute("КЗ Выхода 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_Выхода_2 = 186,

		[EventDescriptionAttribute("Перегрузка Выхода 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Перегрузка_Выхода_2 = 187,

		[EventDescriptionAttribute("Напряжение Выхода 2 выше нормы", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Напряжение_Выхода_2_выше_нормы = 188,

		[EventDescriptionAttribute("АКБ 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АКБ_1 = 189,

		[EventDescriptionAttribute("АКБ 1 Разряд", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АКБ_1_Разряд = 190,

		[EventDescriptionAttribute("АКБ 1 Глубокий Разряд", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АКБ_1_Глубокий_Разряд = 191,

		[EventDescriptionAttribute("АКБ 1 Отсутствие", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АКБ_1_Отсутствие = 192,

		[EventDescriptionAttribute("АКБ 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АКБ_2 = 193,

		[EventDescriptionAttribute("АКБ 2 Разряд", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АКБ_2_Разряд = 194,

		[EventDescriptionAttribute("АКБ 2 Глубокий Разряд", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АКБ_2_Глубокий_Разряд = 195,

		[EventDescriptionAttribute("АКБ 2 Отсутствие", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АКБ_2_Отсутствие = 196,


		[EventDescriptionAttribute("Команда от прибора", JournalEventNameType.Информация)]
		Команда_от_прибора = 197,

		[EventDescriptionAttribute("Команда от кнопки", JournalEventNameType.Информация)]
		Команда_от_кнопки = 198,

		[EventDescriptionAttribute("Изменение автоматики по неисправности", JournalEventNameType.Информация)]
		Изменение_автоматики_по_неисправности = 199,

		[EventDescriptionAttribute("Изменение автомат по СТОП", JournalEventNameType.Информация)]
		Изменение_автомат_по_СТОП = 200,

		[EventDescriptionAttribute("Изменение автоматики по Д-О", JournalEventNameType.Информация)]
		Изменение_автоматики_по_Д_О = 201,

		[EventDescriptionAttribute("Изменение автоматики по ТМ", JournalEventNameType.Информация)]
		Изменение_автоматики_по_ТМ = 202,

		[EventDescriptionAttribute("Отсчет задержки", JournalEventNameType.Информация)]
		Отсчет_задержки_2 = 203,

		[EventDescriptionAttribute("Отлож пуск АУП Д-О", JournalEventNameType.Информация)]
		Отлож_пуск_АУП_Д_О = 204,

		[EventDescriptionAttribute("Пуск АУП завершен", JournalEventNameType.Информация)]
		Пуск_АУП_завершен = 205,

		[EventDescriptionAttribute("Стоп по кнопке СТОП", JournalEventNameType.Информация)]
		Стоп_по_кнопке_СТОП = 206,

		[EventDescriptionAttribute("Программирование мастер-ключа", JournalEventNameType.Информация)]
		Программирование_мастер_ключа = 207,

		[EventDescriptionAttribute("Датчик ДАВЛЕНИЕ", JournalEventNameType.Информация)]
		Датчик_ДАВЛЕНИЕ = 208,

		[EventDescriptionAttribute("Датчик МАССА", JournalEventNameType.Информация)]
		Датчик_МАССА = 209,

		[EventDescriptionAttribute("Сигнал из памяти", JournalEventNameType.Информация)]
		Сигнал_из_памяти = 210,

		[EventDescriptionAttribute("Сигнал аналог входа", JournalEventNameType.Информация)]
		Сигнал_аналог_входа = 311,

		[EventDescriptionAttribute("Замена списка на 1", JournalEventNameType.Информация)]
		Замена_списка_на_1 = 312,

		[EventDescriptionAttribute("Замена списка на 2", JournalEventNameType.Информация)]
		Замена_списка_на_2 = 211,

		[EventDescriptionAttribute("Замена списка на 3", JournalEventNameType.Информация)]
		Замена_списка_на_3 = 212,

		[EventDescriptionAttribute("Замена списка на 4", JournalEventNameType.Информация)]
		Замена_списка_на_4 = 213,

		[EventDescriptionAttribute("Замена списка на 5", JournalEventNameType.Информация)]
		Замена_списка_на_5 = 214,

		[EventDescriptionAttribute("Замена списка на 6", JournalEventNameType.Информация)]
		Замена_списка_на_6 = 215,

		[EventDescriptionAttribute("Замена списка на 7", JournalEventNameType.Информация)]
		Замена_списка_на_7 = 216,

		[EventDescriptionAttribute("Замена списка на 8", JournalEventNameType.Информация)]
		Замена_списка_на_8 = 217,

		[EventDescriptionAttribute("Низкий уровень", JournalEventNameType.Информация)]
		Низкий_уровень = 218,

		[EventDescriptionAttribute("Высокий уровень", JournalEventNameType.Информация)]
		Высокий_уровень = 219,

		[EventDescriptionAttribute("Уровень норма", JournalEventNameType.Информация)]
		Уровень_норма = 220,

		[EventDescriptionAttribute("Перевод в автоматический режим со шкафа", JournalEventNameType.Информация)]
		Перевод_в_автоматический_режим_со_шкафа = 221,

		[EventDescriptionAttribute("Перевод в ручной режим со шкафа", JournalEventNameType.Информация)]
		Перевод_в_ручной_режим_со_шкафа = 222,

		[EventDescriptionAttribute("Перевод в отключенный режим со шкафа", JournalEventNameType.Информация)]
		Перевод_в_отключенный_режим_со_шкафа = 223,

		[EventDescriptionAttribute("Неопределено", JournalEventNameType.Информация)]
		Неопределено = 224,

		[EventDescriptionAttribute("Пуск невозможен", JournalEventNameType.Информация)]
		Пуск_невозможен = 225,

		[EventDescriptionAttribute("Авария пневмоемкости", JournalEventNameType.Информация)]
		Авария_пневмоемкости = 226,

		[EventDescriptionAttribute("Аварийный уровень", JournalEventNameType.Информация)]
		Аварийный_уровень_Информация = 227,

		[EventDescriptionAttribute("Запрет пуска НС", JournalEventNameType.Информация)]
		Запрет_пуска_НС = 228,

		[EventDescriptionAttribute("Запрет пуска компрессора", JournalEventNameType.Информация)]
		Запрет_пуска_компрессора = 229,

		[EventDescriptionAttribute("Ввод 1", JournalEventNameType.Информация)]
		Ввод_1 = 230,

		[EventDescriptionAttribute("Ввод 2", JournalEventNameType.Информация)]
		Ввод_2 = 231,

		[EventDescriptionAttribute("Команда от логики", JournalEventNameType.Информация)]
		Команда_от_логики = 232,

		[EventDescriptionAttribute("Команда от ДУ", JournalEventNameType.Информация)]
		Команда_от_ДУ = 233,

		[EventDescriptionAttribute("Давление низкое", JournalEventNameType.Информация)]
		Давление_низкое = 234,

		[EventDescriptionAttribute("Давление высокое", JournalEventNameType.Информация)]
		Давление_высокое = 235,

		[EventDescriptionAttribute("Давление норма", JournalEventNameType.Информация)]
		Давление_норма = 236,

		[EventDescriptionAttribute("Давление неопределен", JournalEventNameType.Информация)]
		Давление_неопределен = 237,

		[EventDescriptionAttribute("Сигнал с ДВнР есть", JournalEventNameType.Информация)]
		Сигнал_с_ДВнР_есть = 238,

		[EventDescriptionAttribute("Сигнал с ДВнР нет", JournalEventNameType.Информация)]
		Сигнал_с_ДВнР_нет = 239,

		[EventDescriptionAttribute("ШУ вышел на режим", JournalEventNameType.Информация)]
		ШУ_вышел_на_режим = 240,

		[EventDescriptionAttribute("Блокировка пуска", JournalEventNameType.Информация)]
		Блокировка_пуска_2 = 241,

		[EventDescriptionAttribute("Блокировка пуска снята", JournalEventNameType.Информация)]
		Блокировка_пуска_снята = 242,

		[EventDescriptionAttribute("Оператор", JournalEventNameType.Вход_пользователя_в_прибор, JournalEventNameType.Выход_пользователя_из_прибора)]
		Оператор = 243,

		[EventDescriptionAttribute("Администратор", JournalEventNameType.Вход_пользователя_в_прибор, JournalEventNameType.Выход_пользователя_из_прибора)]
		Администратор = 244,

		[EventDescriptionAttribute("Инсталлятор", JournalEventNameType.Вход_пользователя_в_прибор, JournalEventNameType.Выход_пользователя_из_прибора)]
		Инсталлятор = 245,

		[EventDescriptionAttribute("Изготовитель", JournalEventNameType.Вход_пользователя_в_прибор, JournalEventNameType.Выход_пользователя_из_прибора)]
		Изготовитель = 246,

		[EventDescriptionAttribute("Предварительная", JournalEventNameType.Запыленность, JournalEventNameType.Запыленность_устранена)]
		Предварительная = 249,

		[EventDescriptionAttribute("Критическая", JournalEventNameType.Запыленность, JournalEventNameType.Запыленность_устранена)]
		Критическая = 250,

		[EventDescriptionAttribute("Добавление или редактирование", 
			JournalEventNameType.Редактирование_сотрудника, 
			JournalEventNameType.Редактирование_отдела, 
			JournalEventNameType.Редактирование_должности, 
			JournalEventNameType.Редактирование_шаблона_доступа, 
			JournalEventNameType.Редактирование_организации, 
			JournalEventNameType.Редактирование_дополнительной_колонки,
			JournalEventNameType.Редактирование_дневного_графика,
			JournalEventNameType.Редактирование_графика_работы, 
			JournalEventNameType.Редактирование_графика_работы_сотрудника, 
			JournalEventNameType.Редактирование_праздничного_дня)]
		Добавление_или_редактирование = 251,

		[EventDescriptionAttribute("Редактирование", 
			JournalEventNameType.Редактирование_сотрудника, 
			JournalEventNameType.Редактирование_отдела, 
			JournalEventNameType.Редактирование_должности, 
			JournalEventNameType.Редактирование_шаблона_доступа, 
			JournalEventNameType.Редактирование_организации, 
			JournalEventNameType.Редактирование_дополнительной_колонки, 
			JournalEventNameType.Редактирование_дневного_графика, 
			JournalEventNameType.Редактирование_графика_работы, 
			JournalEventNameType.Редактирование_графика_работы_сотрудника,
			JournalEventNameType.Редактирование_праздничного_дня)]
		Редактирование = 252,

		[EventDescriptionAttribute("Удаление", 
			JournalEventNameType.Редактирование_сотрудника,
			JournalEventNameType.Редактирование_отдела, 
			JournalEventNameType.Редактирование_должности,
			JournalEventNameType.Редактирование_шаблона_доступа,
			JournalEventNameType.Редактирование_организации,
			JournalEventNameType.Редактирование_дополнительной_колонки, 
			JournalEventNameType.Редактирование_дневного_графика, 
			JournalEventNameType.Редактирование_графика_работы, 
			JournalEventNameType.Редактирование_графика_работы_сотрудника, 
			JournalEventNameType.Редактирование_праздничного_дня)]
		Удаление = 253,

		[EventDescriptionAttribute("Восстановление", 
			JournalEventNameType.Редактирование_сотрудника, 
			JournalEventNameType.Редактирование_отдела, 
			JournalEventNameType.Редактирование_должности, 
			JournalEventNameType.Редактирование_шаблона_доступа, 
			JournalEventNameType.Редактирование_организации, 
			JournalEventNameType.Редактирование_дополнительной_колонки,
			JournalEventNameType.Редактирование_дневного_графика, 
			JournalEventNameType.Редактирование_графика_работы, 
			JournalEventNameType.Редактирование_графика_работы_сотрудника, 
			JournalEventNameType.Редактирование_праздничного_дня)]
		Восстановление = 254,

		//--------------------------------

		[EventDescriptionAttribute("Остановлено", JournalEventNameType.Информация)]
		Остановлено = 262,

		[EventDescriptionAttribute("Отсчет задержки", JournalEventNameType.Информация)]
		Отсчет_задержки = 263,

		[EventDescriptionAttribute("Контактор Открыть", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Контактор_Открыть = 264,

		[EventDescriptionAttribute("Контактор Закрыть", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Контактор_Закрыть = 265,

		[EventDescriptionAttribute("Обрыв ДУ Открыть", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_ДУ_Открыть = 267,

		[EventDescriptionAttribute("КЗ ДУ Открыть", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_ДУ_Открыть = 268,

		[EventDescriptionAttribute("Обрыв ДУ Закрыть", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_ДУ_Закрыть = 269,

		[EventDescriptionAttribute("КЗ ДУ Закрыть", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_ДУ_Закрыть = 270,

		[EventDescriptionAttribute("Обрыв ОГВ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_ОГВ = 271,

		[EventDescriptionAttribute("КЗ ОГВ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_ОГВ = 272,

		[EventDescriptionAttribute("Истекло Время Хода", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Истекло_Время_Хода = 273,

		[EventDescriptionAttribute("Сигнал МВ без КВ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Сигнал_МВ_без_КВ = 274,

		[EventDescriptionAttribute("Сочетание КВ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Сочетание_КВ = 275,

		[EventDescriptionAttribute("Сочетание МВ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Сочетание_МВ = 276,

		[EventDescriptionAttribute("Сочетание ДНУ и ДВУ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Сочетание_ДНУ_и_ДВУ = 278,

		[EventDescriptionAttribute("Аварии пневмоемкости нет", JournalEventNameType.Информация)]
		Аварии_пневмоемкости_нет = 279,

		[EventDescriptionAttribute("Пуск с УЗ", JournalEventNameType.Информация)]
		Пуск_с_УЗ = 280,

		[EventDescriptionAttribute("Пуск автоматический", JournalEventNameType.Информация)]
		Пуск_автоматический = 281,

		[EventDescriptionAttribute("Пуск ручной", JournalEventNameType.Информация)]
		Пуск_ручной = 282,

		[EventDescriptionAttribute("Пуск с панели шкафа", JournalEventNameType.Информация)]
		Пуск_с_панели_шкафа = 283,

		[EventDescriptionAttribute("Стоп от переключателя", JournalEventNameType.Информация)]
		Стоп_от_переключателя = 284,

		[EventDescriptionAttribute("Стоп с панели шкафа", JournalEventNameType.Информация)]
		Стоп_с_панели_шкафа = 285,

		[EventDescriptionAttribute("Стоп по неисправности", JournalEventNameType.Информация)]
		Стоп_по_неисправности = 286,

		[EventDescriptionAttribute("Стоп с УЗ", JournalEventNameType.Информация)]
		Стоп_с_УЗ = 287,

		[EventDescriptionAttribute("Стоп автоматический", JournalEventNameType.Информация)]
		Стоп_автоматический = 288,

		[EventDescriptionAttribute("Стоп ручной", JournalEventNameType.Информация)]
		Стоп_ручной = 289,

		[EventDescriptionAttribute("Отсчет задержки включен", JournalEventNameType.Информация)]
		Отсчет_задержки_включен = 290,

		[EventDescriptionAttribute("Отсчет задержки выключен", JournalEventNameType.Информация)]
		Отсчет_задержки_выключен = 291,

		[EventDescriptionAttribute("Откр с УЗ", JournalEventNameType.Информация)]
		Откр_с_УЗ = 292,

		[EventDescriptionAttribute("Откр автоматический", JournalEventNameType.Информация)]
		Откр_автоматический = 293,

		[EventDescriptionAttribute("Откр ручной", JournalEventNameType.Информация)]
		Откр_ручной = 294,

		[EventDescriptionAttribute("Откр с панели шкафа", JournalEventNameType.Информация)]
		Откр_с_панели_шкафа = 295,

		[EventDescriptionAttribute("Закр от переключателя", JournalEventNameType.Информация)]
		Закр_от_переключателя = 296,

		[EventDescriptionAttribute("Закр с панели шкафа", JournalEventNameType.Информация)]
		Закр_с_панели_шкафа = 297,

		[EventDescriptionAttribute("Закр по неисправности", JournalEventNameType.Информация)]
		Закр_по_неисправности = 298,

		[EventDescriptionAttribute("Закр с УЗ", JournalEventNameType.Информация)]
		Закр_с_УЗ = 299,

		[EventDescriptionAttribute("Закр автоматический", JournalEventNameType.Информация)]
		Закр_автоматический = 300,

		[EventDescriptionAttribute("Закр ручной", JournalEventNameType.Информация)]
		Закр_ручной = 301,

		[EventDescriptionAttribute("Старт ОГВ", JournalEventNameType.Информация)]
		Старт_ОГВ = 302,

		[EventDescriptionAttribute("Стоп ОГВ", JournalEventNameType.Информация)]
		Стоп_ОГВ = 303,

		[EventDescriptionAttribute("Команда с панели шкафа", JournalEventNameType.Информация)]
		Команда_с_панели_шкафа = 304,

		[EventDescriptionAttribute("Ход на открытие", JournalEventNameType.Информация)]
		Ход_на_открытие = 305,

		[EventDescriptionAttribute("Ход на закрытие", JournalEventNameType.Информация)]
		Ход_на_закрытие = 306,

		[EventDescriptionAttribute("Уровень неопределен", JournalEventNameType.Информация)]
		Уровень_неопределен = 307,

		[EventDescriptionAttribute("Не совпадает тип для точки доступа", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_для_точки_доступа = 308,

		[EventDescriptionAttribute("Вход", JournalEventNameType.Проход_пользователя_разрешен)]
		Вход_Глобал = 309,

		[EventDescriptionAttribute("Выход", JournalEventNameType.Проход_пользователя_разрешен)]
		Выход_Глобал = 310,

		[EventDescriptionAttribute("Постановка на охрану", JournalEventNameType.Управление_ПМФ)]
		Постановка_на_охрану = 311,

		[EventDescriptionAttribute("Снятие с охраны", JournalEventNameType.Управление_ПМФ)]
		Снятие_с_охраны = 312,

		[EventDescriptionAttribute("Пуск", JournalEventNameType.Управление_ПМФ)]
		Пуск = 313,

		[EventDescriptionAttribute("Стоп", JournalEventNameType.Управление_ПМФ)]
		Стоп = 314,

		[EventDescriptionAttribute("КЗ концевого выключателя НОРМА", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_концевого_выключателя_НОРМА = 315,

		[EventDescriptionAttribute("Обрыв концевого выключателя НОРМА", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_концевого_выключателя_НОРМА = 316,

		[EventDescriptionAttribute("КЗ концевого выключателя ЗАЩИТА", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_концевого_выключателя_ЗАЩИТА = 317,

		[EventDescriptionAttribute("Обрыв концевого выключателя ЗАЩИТА", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_концевого_выключателя_ЗАЩИТА = 318,
	}
}