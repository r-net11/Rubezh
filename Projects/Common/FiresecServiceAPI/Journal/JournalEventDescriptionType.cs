using System.ComponentModel;

namespace FiresecAPI.Journal
{
	public enum JournalEventDescriptionType
	{
		[EventDescriptionAttribute("")]
		NULL = 1,

		[EventDescriptionAttribute("Остановка пуска", JournalEventNameType.Команда_оператора)]
		Остановка_пуска = 1000,

		[EventDescriptionAttribute("Выключить немедленно", JournalEventNameType.Команда_оператора)]
		Выключить_немедленно = 1001,

		[EventDescriptionAttribute("Выключить", JournalEventNameType.Команда_оператора)]
		Выключить = 1002,

		[EventDescriptionAttribute("Включить немедленно", JournalEventNameType.Команда_оператора)]
		Включить_немедленно = 1003,

		[EventDescriptionAttribute("Включить", JournalEventNameType.Команда_оператора)]
		Включить = 1004,

		[EventDescriptionAttribute("Перевод в ручной режим", JournalEventNameType.Команда_оператора)]
		Перевод_в_ручной_режим = 1005,

		[EventDescriptionAttribute("Перевод в автоматический режим", JournalEventNameType.Команда_оператора)]
		Перевод_в_автоматический_режим = 1006,

		[EventDescriptionAttribute("Перевод в отключенный режим", JournalEventNameType.Команда_оператора)]
		Перевод_в_отключенный_режим = 1007,

		[EventDescriptionAttribute("Сброс", JournalEventNameType.Команда_оператора)]
		Сброс = 1008,

		[EventDescriptionAttribute("Не найдено родительское устройство ГК", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_найдено_родительское_устройство_ГК = 1009,

		[EventDescriptionAttribute("Старт мониторинга", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Старт_мониторинга = 1010,

		[EventDescriptionAttribute("Не совпадает хэш", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_хэш = 1011,

		[EventDescriptionAttribute("Совпадает хэш", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Совпадает_хэш = 1012,

		[EventDescriptionAttribute("Не совпадает количество байт в пришедшем ответе", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_количество_байт_в_пришедшем_ответе = 1013,

		[EventDescriptionAttribute("Не совпадает тип устройства", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_устройства = 1014,

		[EventDescriptionAttribute("Не совпадает физический адрес устройства", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_физический_адрес_устройства = 1015,

		[EventDescriptionAttribute("Не совпадает адрес на контроллере", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_адрес_на_контроллере = 1016,

		[EventDescriptionAttribute("Не совпадает тип для зоны", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_для_зоны = 1017,

		[EventDescriptionAttribute("Не совпадает тип для направления", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_для_направления = 1018,

		[EventDescriptionAttribute("Не совпадает тип для НС", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_для_НС = 1019,

		[EventDescriptionAttribute("Не совпадает тип для МПТ", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_для_МПТ = 1020,

		[EventDescriptionAttribute("Не совпадает тип для Задержки", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_для_Задержки = 1021,

		[EventDescriptionAttribute("Не совпадает тип для ПИМ", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_для_ПИМ = 1022,

		[EventDescriptionAttribute("Не совпадает тип для охранной зоны", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_для_охранной_зоны = 1023,

		[EventDescriptionAttribute("Не совпадает тип для кода", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_для_кода = 1024,

		[EventDescriptionAttribute("Не совпадает описание компонента", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_описание_компонента = 1025,

		[EventDescriptionAttribute("Ручник сорван", JournalEventNameType.Сработка_1, JournalEventNameType.Сработка_2)]
		Ручник_сорван = 1026,

		[EventDescriptionAttribute("Срабатывание по дыму", JournalEventNameType.Сработка_1, JournalEventNameType.Сработка_2)]
		Срабатывание_по_дыму = 1027,

		[EventDescriptionAttribute("Срабатывание по температуре", JournalEventNameType.Сработка_1, JournalEventNameType.Сработка_2)]
		Срабатывание_по_температуре = 1028,

		[EventDescriptionAttribute("Срабатывание по градиенту температуры", JournalEventNameType.Сработка_1, JournalEventNameType.Сработка_2)]
		Срабатывание_по_градиенту_температуры = 1029,

		[EventDescriptionAttribute("Напряжение питания устройства не в норме", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Напряжение_питания_устройства_не_в_норме = 1030,

		[EventDescriptionAttribute("Оптический канал или фотоусилитель", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Оптический_канал_или_фотоусилитель = 1031,

		[EventDescriptionAttribute("Температурный канал", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Температурный_канал = 1032,

		[EventDescriptionAttribute("КЗ ШС", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_ШС = 1033,

		[EventDescriptionAttribute("Обрыв ШС", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_ШС = 1034,

		[EventDescriptionAttribute("Вскрытие корпуса", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Вскрытие_корпуса = 1035,

		[EventDescriptionAttribute("Контакт не переключается", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Контакт_не_переключается = 1036,

		[EventDescriptionAttribute("Напряжение запуска реле ниже нормы", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Напряжение_запуска_реле_ниже_нормы = 1037,

		[EventDescriptionAttribute("КЗ выхода", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_выхода = 1038,

		[EventDescriptionAttribute("Обрыв выхода", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_выхода = 1039,

		[EventDescriptionAttribute("Напряжение питания ШС ниже нормы", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Напряжение_питания_ШС_ниже_нормы = 1040,

		[EventDescriptionAttribute("Ошибка памяти", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Ошибка_памяти = 1041,

		[EventDescriptionAttribute("КЗ выхода 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_выхода_1 = 1042,

		[EventDescriptionAttribute("КЗ выхода 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_выхода_2 = 1043,

		[EventDescriptionAttribute("КЗ выхода 3", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_выхода_3 = 1044,

		[EventDescriptionAttribute("КЗ выхода 4", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_выхода_4 = 1045,

		[EventDescriptionAttribute("КЗ выхода 5", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_выхода_5 = 1046,

		[EventDescriptionAttribute("Обрыв выхода 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_выхода_1 = 1047,

		[EventDescriptionAttribute("Обрыв выхода 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_выхода_2 = 1048,

		[EventDescriptionAttribute("Обрыв выхода 3", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_выхода_3 = 1049,

		[EventDescriptionAttribute("Обрыв выхода 4", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_выхода_4 = 1050,

		[EventDescriptionAttribute("Обрыв выхода 5", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_выхода_5 = 1051,

		[EventDescriptionAttribute("Блокировка пуска", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Блокировка_пуска = 1052,

		[EventDescriptionAttribute("Низкое напряжение питания привода", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Низкое_напряжение_питания_привода = 1053,

		[EventDescriptionAttribute("Обрыв кнопки НОРМА", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_кнопки_НОРМА = 1054,

		[EventDescriptionAttribute("КЗ кнопки НОРМА", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_кнопки_НОРМА = 1055,

		[EventDescriptionAttribute("Обрыв кнопка ЗАЩИТА", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_кнопка_ЗАЩИТА = 1056,

		[EventDescriptionAttribute("КЗ кнопки ЗАЩИТА", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_кнопки_ЗАЩИТА = 1057,

		[EventDescriptionAttribute("Обрыв концевого выключателя ОТКРЫТО", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_концевого_выключателя_ОТКРЫТО = 1058,

		[EventDescriptionAttribute("Обрыв концевого выключателя ЗАКРЫТО", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_концевого_выключателя_ЗАКРЫТО = 1059,

		[EventDescriptionAttribute("Обрыв цепи 1 ДВИГАТЕЛЯ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_цепи_1_ДВИГАТЕЛЯ = 1060,

		[EventDescriptionAttribute("Замкнуты/разомкнуты оба концевика", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Замкнуты_разомкнуты_оба_концевика = 1061,

		[EventDescriptionAttribute("Превышение времени хода", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Превышение_времени_хода = 1062,

		[EventDescriptionAttribute("Обрыв в линии РЕЛЕ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_в_линии_РЕЛЕ = 1063,

		[EventDescriptionAttribute("КЗ в линии РЕЛЕ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_в_линии_РЕЛЕ = 1064,

		[EventDescriptionAttribute("Выход 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Выход_1 = 1065,

		[EventDescriptionAttribute("Выход 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Выход_2 = 1066,

		[EventDescriptionAttribute("Выход 3", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Выход_3 = 1067,

		[EventDescriptionAttribute("КЗ концевого выключателя ОТКРЫТО", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_концевого_выключателя_ОТКРЫТО = 1068,

		[EventDescriptionAttribute("Обрыв муфтового выключателя ОТКРЫТО", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_муфтового_выключателя_ОТКРЫТО = 1069,

		[EventDescriptionAttribute("КЗ муфтового выключателя ОТКРЫТО", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_муфтового_выключателя_ОТКРЫТО = 1070,

		[EventDescriptionAttribute("КЗ концевого выключателя ЗАКРЫТО", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_концевого_выключателя_ЗАКРЫТО = 1071,

		[EventDescriptionAttribute("Обрыв муфтового выключателя ЗАКРЫТО", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_муфтового_выключателя_ЗАКРЫТО = 1072,

		[EventDescriptionAttribute("КЗ муфтового выключателя ЗАКРЫТО", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_муфтового_выключателя_ЗАКРЫТО = 1073,

		[EventDescriptionAttribute("Обрыв кнопки Открыть УЗЗ/Закрыть УЗЗ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_кнопки_Открыть_УЗЗ_Закрыть_УЗЗ = 1074,

		[EventDescriptionAttribute("КЗ кнопки Открыть УЗЗ/Закрыть УЗЗ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_кнопки_Открыть_УЗЗ_Закрыть_УЗЗ = 1075,

		[EventDescriptionAttribute("Обрыв кнопки СТОП УЗЗ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_кнопки_СТОП_УЗЗ = 1076,

		[EventDescriptionAttribute("КЗ кнопки СТОП УЗЗ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_кнопки_СТОП_УЗЗ = 1077,

		[EventDescriptionAttribute("Обрыв давление низкое", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_давление_низкое = 1078,

		[EventDescriptionAttribute("КЗ давление низкое", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_давление_низкое = 1079,

		[EventDescriptionAttribute("Таймаут по давлению", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Таймаут_по_давлению = 1080,

		[EventDescriptionAttribute("КВ/МВ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КВ_МВ = 1081,

		[EventDescriptionAttribute("Не задан режим", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Не_задан_режим = 1082,

		[EventDescriptionAttribute("Отказ ШУЗ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Отказ_ШУЗ = 1083,

		[EventDescriptionAttribute("ДУ/ДД", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		ДУ_ДД = 1084,

		[EventDescriptionAttribute("Обрыв входа 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_входа_1 = 1085,

		[EventDescriptionAttribute("КЗ входа 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_входа_1 = 1086,

		[EventDescriptionAttribute("Обрыв входа 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_входа_2 = 1087,

		[EventDescriptionAttribute("КЗ входа 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_входа_2 = 1088,

		[EventDescriptionAttribute("Обрыв входа 3", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_входа_3 = 1089,

		[EventDescriptionAttribute("КЗ входа 3", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_входа_3 = 1090,

		[EventDescriptionAttribute("Обрыв входа 4", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_входа_4 = 1091,

		[EventDescriptionAttribute("КЗ входа 4", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_входа_4 = 1092,

		[EventDescriptionAttribute("Не задан тип", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Не_задан_тип = 1093,

		[EventDescriptionAttribute("Отказ ПН", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Отказ_ПН = 1094,

		[EventDescriptionAttribute("Отказ ШУН", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Отказ_ШУН = 1095,

		[EventDescriptionAttribute("Питание 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Питание_1 = 1096,

		[EventDescriptionAttribute("Питание 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Питание_2 = 1097,

		[EventDescriptionAttribute("Отказ АЛС 1 или 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Отказ_АЛС_1_или_2 = 1098,

		[EventDescriptionAttribute("Отказ АЛС 3 или 4", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Отказ_АЛС_3_или_4 = 1099,

		[EventDescriptionAttribute("Отказ АЛС 5 или 6", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Отказ_АЛС_5_или_6 = 1100,

		[EventDescriptionAttribute("Отказ АЛС 7 или 8", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Отказ_АЛС_7_или_8 = 1101,

		[EventDescriptionAttribute("Обрыв цепи 2 ДВИГАТЕЛЯ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_цепи_2_ДВИГАТЕЛЯ = 1102,

		[EventDescriptionAttribute("КЗ АЛС 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_1 = 1103,

		[EventDescriptionAttribute("КЗ АЛС 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_2 = 1104,

		[EventDescriptionAttribute("КЗ АЛС 3", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_3 = 1105,

		[EventDescriptionAttribute("КЗ АЛС 4", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_4 = 1106,

		[EventDescriptionAttribute("КЗ АЛС 5", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_5 = 1107,

		[EventDescriptionAttribute("КЗ АЛС 6", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_6 = 1108,

		[EventDescriptionAttribute("КЗ АЛС 7", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_7 = 1109,

		[EventDescriptionAttribute("КЗ АЛС 8", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_8 = 1110,

		[EventDescriptionAttribute("Истекло время вкл", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Истекло_время_вкл = 1111,

		[EventDescriptionAttribute("Истекло время выкл", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Истекло_время_выкл = 1112,

		[EventDescriptionAttribute("Контакт реле 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Контакт_реле_1 = 1113,

		[EventDescriptionAttribute("Контакт реле 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Контакт_реле_2 = 1114,

		[EventDescriptionAttribute("Обрыв кнопки ПУСК", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_кнопки_ПУСК = 1115,

		[EventDescriptionAttribute("КЗ кнопки ПУСК", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_кнопки_ПУСК = 1116,

		[EventDescriptionAttribute("Обрыв кнопки СТОП", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_кнопки_СТОП = 1117,

		[EventDescriptionAttribute("КЗ кнопки СТОП", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_кнопки_СТОП = 1118,

		[EventDescriptionAttribute("Отсутствуют или испорчены сообщения для воспроизведения", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Отсутствуют_или_испорчены_сообщения_для_воспроизведения = 1119,

		[EventDescriptionAttribute("Выход", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Выход = 1120,

		[EventDescriptionAttribute("Обрыв Низкий уровень", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_Низкий_уровень = 1121,

		[EventDescriptionAttribute("КЗ Низкий уровень", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_Низкий_уровень = 1122,

		[EventDescriptionAttribute("Обрыв Высокий уровень", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_Высокий_уровень = 1123,

		[EventDescriptionAttribute("КЗ Высокий уровень", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_Высокий_уровень = 1124,

		[EventDescriptionAttribute("Обрыв Аварийный уровень", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_Аварийный_уровень = 1125,

		[EventDescriptionAttribute("КЗ Аварийный уровень", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_Аварийный_уровень = 1126,

		[EventDescriptionAttribute("Аварийный уровень", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Аварийный_уровень = 1127,

		[EventDescriptionAttribute("Питание силовое", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Питание_силовое = 1128,

		[EventDescriptionAttribute("Питание контроллера", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Питание_контроллера = 1129,

		[EventDescriptionAttribute("Несовместимость сигналов", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Несовместимость_сигналов = 1130,

		[EventDescriptionAttribute("Неисправность одной или обеих фаз(контроль нагрузки)", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Неисправность_одной_или_обеих_фаз_контроль_нагрузки = 1131,

		[EventDescriptionAttribute("Обрыв Давление на выходе", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_Давление_на_выходе = 1132,

		[EventDescriptionAttribute("КЗ Давление на выходе", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_Давление_на_выходе = 1133,

		[EventDescriptionAttribute("Обрыв ДУ ПУСК", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_ДУ_ПУСК = 1134,

		[EventDescriptionAttribute("КЗ ДУ ПУСК", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_ДУ_ПУСК = 1135,

		[EventDescriptionAttribute("Обрыв ДУ СТОП", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_ДУ_СТОП = 1136,

		[EventDescriptionAttribute("КЗ ДУ СТОП", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_ДУ_СТОП = 1137,

		[EventDescriptionAttribute("АЛС 1 Неизвестное устройство", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_1_Неизвестное_устройство = 1138,

		[EventDescriptionAttribute("АЛС 2 Неизвестное устройство", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_2_Неизвестное_устройство = 1139,

		[EventDescriptionAttribute("АЛС 3 Неизвестное устройство", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_3_Неизвестное_устройство = 1140,

		[EventDescriptionAttribute("АЛС 4 Неизвестное устройство", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_4_Неизвестное_устройство = 1141,

		[EventDescriptionAttribute("АЛС 5 Неизвестное устройство", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_5_Неизвестное_устройство = 1142,

		[EventDescriptionAttribute("АЛС 6 Неизвестное устройство", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_6_Неизвестное_устройство = 1143,

		[EventDescriptionAttribute("АЛС 7 Неизвестное устройство", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_7_Неизвестное_устройство = 1144,

		[EventDescriptionAttribute("АЛС 8 Неизвестное устройство", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_8_Неизвестное_устройство = 1145,

		[EventDescriptionAttribute("АЛС 1 Неизвестный тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_1_Неизвестный_тип_устройства = 1146,

		[EventDescriptionAttribute("АЛС 2 Неизвестный тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_2_Неизвестный_тип_устройства = 1147,

		[EventDescriptionAttribute("АЛС 3 Неизвестный тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_3_Неизвестный_тип_устройства = 1148,

		[EventDescriptionAttribute("АЛС 4 Неизвестный тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_4_Неизвестный_тип_устройства = 1149,

		[EventDescriptionAttribute("АЛС 5 Неизвестный тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_5_Неизвестный_тип_устройства = 1150,

		[EventDescriptionAttribute("АЛС 6 Неизвестный тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_6_Неизвестный_тип_устройства = 1151,

		[EventDescriptionAttribute("АЛС 7 Неизвестный тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_7_Неизвестный_тип_устройства = 1152,

		[EventDescriptionAttribute("АЛС 8 Неизвестный тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_8_Неизвестный_тип_устройства = 1153,

		[EventDescriptionAttribute("АЛС 1 Другой тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_1_Другой_тип_устройства = 1154,

		[EventDescriptionAttribute("АЛС 2 Другой тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_2_Другой_тип_устройства = 1155,

		[EventDescriptionAttribute("АЛС 3 Другой тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_3_Другой_тип_устройства = 1156,

		[EventDescriptionAttribute("АЛС 4 Другой тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_4_Другой_тип_устройства = 1157,

		[EventDescriptionAttribute("АЛС 5 Другой тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_5_Другой_тип_устройства = 1158,

		[EventDescriptionAttribute("АЛС 6 Другой тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_6_Другой_тип_устройства = 1159,

		[EventDescriptionAttribute("АЛС 7 Другой тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_7_Другой_тип_устройства = 1160,

		[EventDescriptionAttribute("АЛС 8 Другой тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_8_Другой_тип_устройства = 1161,

		[EventDescriptionAttribute("Обрыв АЛС 1-2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_1_2 = 1162,

		[EventDescriptionAttribute("Обрыв АЛС 3-4", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_3_4 = 1163,

		[EventDescriptionAttribute("Обрыв АЛС 5-6", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_5_6 = 1164,

		[EventDescriptionAttribute("Обрыв АЛС 7-8", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_7_8 = 1165,

		[EventDescriptionAttribute("Обрыв АЛС 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_1 = 1166,

		[EventDescriptionAttribute("Обрыв АЛС 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_2 = 1167,

		[EventDescriptionAttribute("Обрыв АЛС 3", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_3 = 1168,

		[EventDescriptionAttribute("Обрыв АЛС 4", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_4 = 1169,

		[EventDescriptionAttribute("Обрыв АЛС 5", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_5 = 1170,

		[EventDescriptionAttribute("Обрыв АЛС 6", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_6 = 1171,

		[EventDescriptionAttribute("Обрыв АЛС 7", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_7 = 1172,

		[EventDescriptionAttribute("Обрыв АЛС 8", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_8 = 1173,

		[EventDescriptionAttribute("ОЛС", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		ОЛС = 1174,

		[EventDescriptionAttribute("РЛС", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		РЛС = 1175,

		[EventDescriptionAttribute("Потеря связи", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Потеря_связи = 1175,

		[EventDescriptionAttribute("Отсутствие сетевого напряжения", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Отсутствие_сетевого_напряжения = 1176,

		[EventDescriptionAttribute("КЗ Выхода 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_Выхода_1 = 1177,

		[EventDescriptionAttribute("Перегрузка Выхода 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Перегрузка_Выхода_1 = 1178,

		[EventDescriptionAttribute("Напряжение Выхода 1 выше нормы", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Напряжение_Выхода_1_выше_нормы = 1179,

		[EventDescriptionAttribute("КЗ Выхода 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_Выхода_2 = 1180,

		[EventDescriptionAttribute("Перегрузка Выхода 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Перегрузка_Выхода_2 = 1181,

		[EventDescriptionAttribute("Напряжение Выхода 2 выше нормы", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Напряжение_Выхода_2_выше_нормы = 1182,

		[EventDescriptionAttribute("АКБ 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АКБ_1 = 1183,

		[EventDescriptionAttribute("АКБ 1 Разряд", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АКБ_1_Разряд = 1184,

		[EventDescriptionAttribute("АКБ 1 Глубокий Разряд", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АКБ_1_Глубокий_Разряд = 1185,

		[EventDescriptionAttribute("АКБ 1 Отсутствие", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АКБ_1_Отсутствие = 1186,

		[EventDescriptionAttribute("АКБ 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АКБ_2 = 1187,

		[EventDescriptionAttribute("АКБ 2 Разряд", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АКБ_2_Разряд = 1188,

		[EventDescriptionAttribute("АКБ 2 Глубокий Разряд", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АКБ_2_Глубокий_Разряд = 1189,

		[EventDescriptionAttribute("АКБ 2 Отсутствие", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АКБ_2_Отсутствие = 1190,


		[EventDescriptionAttribute("Команда от прибора", JournalEventNameType.Информация)]
		Команда_от_прибора = 1191,

		[EventDescriptionAttribute("Команда от кнопки", JournalEventNameType.Информация)]
		Команда_от_кнопки = 1192,

		[EventDescriptionAttribute("Изменение автоматики по неисправности", JournalEventNameType.Информация)]
		Изменение_автоматики_по_неисправности = 1193,

		[EventDescriptionAttribute("Изменение автомат по СТОП", JournalEventNameType.Информация)]
		Изменение_автомат_по_СТОП = 1194,

		[EventDescriptionAttribute("Изменение автоматики по Д-О", JournalEventNameType.Информация)]
		Изменение_автоматики_по_Д_О = 1195,

		[EventDescriptionAttribute("Изменение автоматики по ТМ", JournalEventNameType.Информация)]
		Изменение_автоматики_по_ТМ = 1196,

		[EventDescriptionAttribute("Ручной пуск", JournalEventNameType.Информация)]
		Ручной_пуск = 1197,

		[EventDescriptionAttribute("Отлож пуск АУП Д-О", JournalEventNameType.Информация)]
		Отлож_пуск_АУП_Д_О = 1198,

		[EventDescriptionAttribute("Пуск АУП завершен", JournalEventNameType.Информация)]
		Пуск_АУП_завершен = 1199,

		[EventDescriptionAttribute("Стоп по кнопке СТОП", JournalEventNameType.Информация)]
		Стоп_по_кнопке_СТОП = 1200,

		[EventDescriptionAttribute("Программирование мастер-ключа", JournalEventNameType.Информация)]
		Программирование_мастер_ключа = 1201,

		[EventDescriptionAttribute("Датчик ДАВЛЕНИЕ", JournalEventNameType.Информация)]
		Датчик_ДАВЛЕНИЕ = 1202,

		[EventDescriptionAttribute("Датчик МАССА", JournalEventNameType.Информация)]
		Датчик_МАССА = 1203,

		[EventDescriptionAttribute("Сигнал из памяти", JournalEventNameType.Информация)]
		Сигнал_из_памяти = 1204,

		[EventDescriptionAttribute("Сигнал аналог входа", JournalEventNameType.Информация)]
		Сигнал_аналог_входа = 1205,

		[EventDescriptionAttribute("Замена списка на 1", JournalEventNameType.Информация)]
		Замена_списка_на_1 = 1206,

		[EventDescriptionAttribute("Замена списка на 2", JournalEventNameType.Информация)]
		Замена_списка_на_2 = 1207,

		[EventDescriptionAttribute("Замена списка на 3", JournalEventNameType.Информация)]
		Замена_списка_на_3 = 1208,

		[EventDescriptionAttribute("Замена списка на 4", JournalEventNameType.Информация)]
		Замена_списка_на_4 = 1209,

		[EventDescriptionAttribute("Замена списка на 5", JournalEventNameType.Информация)]
		Замена_списка_на_5 = 1210,

		[EventDescriptionAttribute("Замена списка на 6", JournalEventNameType.Информация)]
		Замена_списка_на_6 = 1211,

		[EventDescriptionAttribute("Замена списка на 7", JournalEventNameType.Информация)]
		Замена_списка_на_7 = 1212,

		[EventDescriptionAttribute("Замена списка на 8", JournalEventNameType.Информация)]
		Замена_списка_на_8 = 1213,

		[EventDescriptionAttribute("Низкий уровень", JournalEventNameType.Информация)]
		Низкий_уровень = 1214,

		[EventDescriptionAttribute("Высокий уровень", JournalEventNameType.Информация)]
		Высокий_уровень = 1215,

		[EventDescriptionAttribute("Уровень норма", JournalEventNameType.Информация)]
		Уровень_норма = 1216,

		[EventDescriptionAttribute("Перевод в автоматический режим со шкафа", JournalEventNameType.Информация)]
		Перевод_в_автоматический_режим_со_шкафа = 1217,

		[EventDescriptionAttribute("Перевод в ручной режим со шкафа", JournalEventNameType.Информация)]
		Перевод_в_ручной_режим_со_шкафа = 1218,

		[EventDescriptionAttribute("Перевод в отключенный режим со шкафа", JournalEventNameType.Информация)]
		Перевод_в_отключенный_режим_со_шкафа = 1219,

		[EventDescriptionAttribute("Неопределено", JournalEventNameType.Информация)]
		Неопределено = 1220,

		[EventDescriptionAttribute("Пуск невозможен", JournalEventNameType.Информация)]
		Пуск_невозможен = 1221,

		[EventDescriptionAttribute("Авария пневмоемкости", JournalEventNameType.Информация)]
		Авария_пневмоемкости = 1222,

		[EventDescriptionAttribute("Аварийный уровень", JournalEventNameType.Информация)]
		Аварийный_уровень_Информация = 1223,

		[EventDescriptionAttribute("Запрет пуска НС", JournalEventNameType.Информация)]
		Запрет_пуска_НС = 1224,

		[EventDescriptionAttribute("Запрет пуска компрессора", JournalEventNameType.Информация)]
		Запрет_пуска_компрессора = 1225,

		[EventDescriptionAttribute("Ввод 1", JournalEventNameType.Информация)]
		Ввод_1 = 1226,

		[EventDescriptionAttribute("Ввод 2", JournalEventNameType.Информация)]
		Ввод_2 = 1227,

		[EventDescriptionAttribute("Команда от логики", JournalEventNameType.Информация)]
		Команда_от_логики = 1228,

		[EventDescriptionAttribute("Команда от ДУ", JournalEventNameType.Информация)]
		Команда_от_ДУ = 1229,

		[EventDescriptionAttribute("Давление низкое", JournalEventNameType.Информация)]
		Давление_низкое = 1230,

		[EventDescriptionAttribute("Давление высокое", JournalEventNameType.Информация)]
		Давление_высокое = 1231,

		[EventDescriptionAttribute("Давление норма", JournalEventNameType.Информация)]
		Давление_норма = 1232,

		[EventDescriptionAttribute("Давление неопределен", JournalEventNameType.Информация)]
		Давление_неопределен = 1233,

		[EventDescriptionAttribute("Давление на выходе есть", JournalEventNameType.Информация)]
		Давление_на_выходе_есть = 1234,

		[EventDescriptionAttribute("Давления на выходе нет", JournalEventNameType.Информация)]
		Давления_на_выходе_нет = 1235,

		[EventDescriptionAttribute("Выключить", JournalEventNameType.Информация)]
		Выключить_Информация = 1236,

		[EventDescriptionAttribute("Стоп", JournalEventNameType.Информация)]
		Стоп = 1237,

		[EventDescriptionAttribute("Запрет пуска", JournalEventNameType.Информация)]
		Запрет_пуска = 1238,

		[EventDescriptionAttribute("Оператор", JournalEventNameType.Вход_пользователя_в_прибор, JournalEventNameType.Выход_пользователя_из_прибора)]
		Оператор = 1239,

		[EventDescriptionAttribute("Администратор", JournalEventNameType.Вход_пользователя_в_прибор, JournalEventNameType.Выход_пользователя_из_прибора)]
		Администратор = 1240,

		[EventDescriptionAttribute("Инсталлятор", JournalEventNameType.Вход_пользователя_в_прибор, JournalEventNameType.Выход_пользователя_из_прибора)]
		Инсталлятор = 1241,

		[EventDescriptionAttribute("Изготовитель", JournalEventNameType.Вход_пользователя_в_прибор, JournalEventNameType.Выход_пользователя_из_прибора)]
		Изготовитель = 1242,

		[EventDescriptionAttribute("Кнопка", JournalEventNameType.Тест, JournalEventNameType.Тест_устранен)]
		Кнопка = 1243,

		[EventDescriptionAttribute("Указка", JournalEventNameType.Тест, JournalEventNameType.Тест_устранен)]
		Указка = 1244,

		[EventDescriptionAttribute("Предварительная", JournalEventNameType.Запыленность, JournalEventNameType.Запыленность_устранена)]
		Предварительная = 1245,

		[EventDescriptionAttribute("Критическая", JournalEventNameType.Запыленность, JournalEventNameType.Запыленность_устранена)]
		Критическая = 1246,

		[EventDescriptionAttribute("Добавление или редактирование", JournalEventNameType.Редактирование_сотрудника, JournalEventNameType.Редактирование_отдела, JournalEventNameType.Редактирование_должности, JournalEventNameType.Редактирование_шаблона_доступа, JournalEventNameType.Редактирование_организации, JournalEventNameType.Редактирование_дополнительной_колонки, JournalEventNameType.Редактирование_дневного_графика, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы_сотрудника, JournalEventNameType.Редактирование_праздничного_дня)]
		Добавление_или_редактирование = 2000,

		[EventDescriptionAttribute("Редактирование", JournalEventNameType.Редактирование_сотрудника, JournalEventNameType.Редактирование_отдела, JournalEventNameType.Редактирование_должности, JournalEventNameType.Редактирование_шаблона_доступа, JournalEventNameType.Редактирование_организации, JournalEventNameType.Редактирование_дополнительной_колонки, JournalEventNameType.Редактирование_дневного_графика, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы_сотрудника, JournalEventNameType.Редактирование_праздничного_дня)]
		Редактирование = 2001,

		[EventDescriptionAttribute("Удаление", JournalEventNameType.Редактирование_сотрудника, JournalEventNameType.Редактирование_отдела, JournalEventNameType.Редактирование_должности, JournalEventNameType.Редактирование_шаблона_доступа, JournalEventNameType.Редактирование_организации, JournalEventNameType.Редактирование_дополнительной_колонки, JournalEventNameType.Редактирование_дневного_графика, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы_сотрудника, JournalEventNameType.Редактирование_праздничного_дня)]
		Удаление = 2002,

		[EventDescriptionAttribute("Восстановление", JournalEventNameType.Редактирование_сотрудника, JournalEventNameType.Редактирование_отдела, JournalEventNameType.Редактирование_должности, JournalEventNameType.Редактирование_шаблона_доступа, JournalEventNameType.Редактирование_организации, JournalEventNameType.Редактирование_дополнительной_колонки, JournalEventNameType.Редактирование_дневного_графика, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы_сотрудника, JournalEventNameType.Редактирование_праздничного_дня)]
		Восстановление = 2003,

		[EventDescriptionAttribute("Метод открытия Неизвестно", JournalEventNameType.Проход_разрешен, JournalEventNameType.Проход_запрещен)]
		Метод_открытия_Неизвестно = 2004,

		[EventDescriptionAttribute("Метод открытия Пароль", JournalEventNameType.Проход_разрешен, JournalEventNameType.Проход_запрещен)]
		Метод_открытия_Пароль = 2005,

		[EventDescriptionAttribute("Метод открытия Карта", JournalEventNameType.Проход_разрешен, JournalEventNameType.Проход_запрещен)]
		Метод_открытия_Карта = 2006,

		[EventDescriptionAttribute("Метод открытия Сначала карта", JournalEventNameType.Проход_разрешен, JournalEventNameType.Проход_запрещен)]
		Метод_открытия_Сначала_карта = 2007,

		[EventDescriptionAttribute("Метод открытия Сначала пароль", JournalEventNameType.Проход_разрешен, JournalEventNameType.Проход_запрещен)]
		Метод_открытия_Сначала_пароль = 2008,

		[EventDescriptionAttribute("Метод открытия Удаленно", JournalEventNameType.Проход_разрешен, JournalEventNameType.Проход_запрещен)]
		Метод_открытия_Удаленно = 2009,

		[EventDescriptionAttribute("Метод открытия Кнопка", JournalEventNameType.Проход_разрешен, JournalEventNameType.Проход_запрещен)]
		Метод_открытия_Кнопка = 2010,
	}
}