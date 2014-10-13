using System.ComponentModel;
using FiresecAPI.GK;

namespace FiresecAPI.Journal
{
	public enum JournalEventNameType
	{
		[DescriptionAttribute("")]
		NULL = 1,

		[EventName(JournalSubsystemType.System, "Неизвестное событие", XStateClass.No)]
		Неизвестное_событие = 2,

		[EventName(JournalSubsystemType.System, "Подтверждение тревоги", XStateClass.Fire1)]
		Подтверждение_тревоги = 3,

		[EventName(JournalSubsystemType.System, "Вход пользователя в систему", XStateClass.Info)]
		Вход_пользователя_в_систему = 4,

		[EventName(JournalSubsystemType.System, "Выход пользователя из системы", XStateClass.Info)]
		Выход_пользователя_из_системы = 5,

		[EventName(JournalSubsystemType.System, "Дежурство сдал", XStateClass.Info)]
		Дежурство_сдал = 6,

		[EventName(JournalSubsystemType.System, "Дежурство принял", XStateClass.Info)]
		Дежурство_принял = 7,

		[EventName(JournalSubsystemType.System, "Зависание процесса отпроса", XStateClass.Unknown)]
		Зависание_процесса_отпроса = 8,

		[EventName(JournalSubsystemType.System, "Отсутствует лицензия", XStateClass.HasNoLicense)]
		Отсутствует_лицензия = 9,

		[EventName(JournalSubsystemType.System, "Лицензия обнаружена", XStateClass.HasNoLicense)]
		Лицензия_обнаружена = 10,

		[EventName(JournalSubsystemType.System, "Ошибка инициализации мониторинга", XStateClass.Unknown)]
		Ошибка_инициализации_мониторинга = 11,

		[EventName(JournalSubsystemType.System, "Отмена операции", XStateClass.TechnologicalRegime)]
		Отмена_операции = 12,

		[EventName(JournalSubsystemType.System, "Сообщение автоматизации", XStateClass.Info)]
		Сообщение_автоматизации = 13,

		[EventName(JournalSubsystemType.GK, "Обновление ПО прибора", XStateClass.Info)]
		Обновление_ПО_прибора = 1001,

		[EventName(JournalSubsystemType.GK, "Запись конфигурации в прибор", XStateClass.Info)]
		Запись_конфигурации_в_прибор = 1002,

		[EventName(JournalSubsystemType.GK, "Чтение конфигурации из прибора", XStateClass.Info)]
		Чтение_конфигурации_из_прибора = 1003,

		[EventName(JournalSubsystemType.GK, "Запрос информации об устройстве", XStateClass.TechnologicalRegime)]
		Запрос_информации_об_устройстве = 1004,

		[EventName(JournalSubsystemType.GK, "Синхронизация времени", XStateClass.Info)]
		Синхронизация_времени = 1005,

		[EventName(JournalSubsystemType.GK, "Команда оператора", XStateClass.Info)]
		Команда_оператора = 1006,

		[EventName(JournalSubsystemType.GK, "Ошибка при выполнении команды", XStateClass.Failure)]
		Ошибка_при_выполнении_команды = 1007,

		[EventName(JournalSubsystemType.GK, "Ошибка при выполнении команды над устройством", XStateClass.Failure)]
		Ошибка_при_выполнении_команды_над_устройством = 1008,

		[EventName(JournalSubsystemType.GK, "Нет связи с ГК", XStateClass.ConnectionLost)]
		Нет_связи_с_ГК = 1009,

		[EventName(JournalSubsystemType.GK, "Связь с ГК восстановлена", XStateClass.ConnectionLost)]
		Связь_с_ГК_восстановлена = 1010,

		[EventName(JournalSubsystemType.GK, "Конфигурация прибора не соответствует конфигурации ПК", XStateClass.Unknown)]
		Конфигурация_прибора_не_соответствует_конфигурации_ПК = 1011,

		[EventName(JournalSubsystemType.GK, "Конфигурация прибора соответствует конфигурации ПК", XStateClass.Unknown)]
		Конфигурация_прибора_соответствует_конфигурации_ПК = 1012,

		[EventName(JournalSubsystemType.GK, "Ошибка при синхронизации журнала", XStateClass.Unknown)]
		Ошибка_при_синхронизации_журнала = 1013,

		[EventName(JournalSubsystemType.GK, "Ошибка при опросе состояний компонентов ГК", XStateClass.Unknown)]
		Ошибка_при_опросе_состояний_компонентов_ГК = 1014,

		[EventName(JournalSubsystemType.GK, "Устранена ошибка при опросе состояний компонентов ГК", XStateClass.Unknown)]
		Устранена_ошибка_при_опросе_состояний_компонентов_ГК = 1015,

		[EventName(JournalSubsystemType.GK, "Восстановление связи с прибором", XStateClass.ConnectionLost)]
		Восстановление_связи_с_прибором = 1016,

		[EventName(JournalSubsystemType.GK, "Потеря связи с прибором", XStateClass.ConnectionLost)]
		Потеря_связи_с_прибором = 1017,

		[EventName(JournalSubsystemType.System, "База данных прибора не соответствует базе данных ПК", XStateClass.Unknown)]
		База_данных_прибора_не_соответствует_базе_данных_ПК = 1018,

		[EventName(JournalSubsystemType.GK, "База данных прибора соответствует базе данных ПК", XStateClass.Unknown)]
		База_данных_прибора_соответствует_базе_данных_ПК = 1019,

		[EventName(JournalSubsystemType.GK, "Применение конфигурации", XStateClass.Info)]
		Применение_конфигурации = 1020,

		[EventName(JournalSubsystemType.GK, "ГК в технологическом режиме", XStateClass.TechnologicalRegime)]
		ГК_в_технологическом_режиме = 1021,

		[EventName(JournalSubsystemType.GK, "ГК в рабочем режиме", XStateClass.Info)]
		ГК_в_рабочем_режиме = 1022,

		[EventName(JournalSubsystemType.GK, "Запись всех идентификаторов", XStateClass.Info)]
		Запись_всех_идентификаторов = 1023,

		[EventName(JournalSubsystemType.GK, "Перевод в технологический режим", XStateClass.TechnologicalRegime)]
		Перевод_в_технологический_режим = 1024,

		[EventName(JournalSubsystemType.GK, "Синхронизация времени прибора с временем ПК", XStateClass.Info)]
		Синхронизация_времени_прибора_с_временем_ПК = 1025,

		[EventName(JournalSubsystemType.GK, "Смена ПО", XStateClass.TechnologicalRegime)]
		Смена_ПО = 1026,

		[EventName(JournalSubsystemType.GK, "Смена БД", XStateClass.TechnologicalRegime)]
		Смена_БД = 1027,

		[EventName(JournalSubsystemType.GK, "Перевод в рабочий режим", XStateClass.Info)]
		Перевод_в_рабочий_режим = 1028,

		[EventName(JournalSubsystemType.GK, "Вход пользователя в прибор", XStateClass.Info)]
		Вход_пользователя_в_прибор = 1029,

		[EventName(JournalSubsystemType.GK, "Выход пользователя из прибора", XStateClass.Info)]
		Выход_пользователя_из_прибора = 1030,

		[EventName(JournalSubsystemType.GK, "Ошибка управления", XStateClass.Failure)]
		Ошибка_управления = 1031,

		[EventName(JournalSubsystemType.GK, "Введен новый пользователь", XStateClass.Info)]
		Введен_новый_пользователь = 1032,

		[EventName(JournalSubsystemType.GK, "Изменена учетная информация пользователя", XStateClass.Info)]
		Изменена_учетная_информация_пользователя = 1033,

		[EventName(JournalSubsystemType.GK, "Произведена настройка сети", XStateClass.Info)]
		Произведена_настройка_сети = 1034,

		[EventName(JournalSubsystemType.GK, "Неизвестный код события контроллекра", XStateClass.Unknown)]
		Неизвестный_код_события_контроллекра = 1035,

		[EventName(JournalSubsystemType.GK, "Неизвестный тип", XStateClass.Unknown)]
		Неизвестный_тип = 1036,

		[EventName(JournalSubsystemType.GK, "Устройство с таким адресом не описано при конфигурации", XStateClass.Unknown)]
		Устройство_с_таким_адресом_не_описано_при_конфигурации = 1037,

		[EventName(JournalSubsystemType.GK, "Неизвестный код события устройства", XStateClass.Unknown)]
		Неизвестный_код_события_устройства = 1038,

		[EventName(JournalSubsystemType.GK, "При конфигурации описан другой тип", XStateClass.Unknown)]
		При_конфигурации_описан_другой_тип = 1039,

		[EventName(JournalSubsystemType.GK, "Изменился заводской номер", XStateClass.Info)]
		Изменился_заводской_номер = 1040,

		[EventName(JournalSubsystemType.GK, "Пожар-1", XStateClass.Fire1)]
		Пожар_1 = 1041,

		[EventName(JournalSubsystemType.GK, "Сработка-1", XStateClass.Fire1)]
		Сработка_1 = 1042,

		[EventName(JournalSubsystemType.GK, "Сработка охранной зоны", XStateClass.Fire1)]
		Сработка_Охранной_Зоны = 1043,

		[EventName(JournalSubsystemType.GK, "Пожар-2", XStateClass.Fire2)]
		Пожар_2 = 1044,

		[EventName(JournalSubsystemType.GK, "Сработка-2", XStateClass.Fire2)]
		Сработка_2 = 1045,

		[EventName(JournalSubsystemType.GK, "Внимание", XStateClass.Attention)]
		Внимание = 1046,

		[EventName(JournalSubsystemType.GK, "Неисправность", XStateClass.Failure)]
		Неисправность = 1047,

		[EventName(JournalSubsystemType.GK, "Неисправность устранена", XStateClass.Norm)]
		Неисправность_устранена = 1048,

		[EventName(JournalSubsystemType.GK, "Тест", XStateClass.Test)]
		Тест = 1049,

		[EventName(JournalSubsystemType.GK, "Тест устранен", XStateClass.Test)]
		Тест_устранен = 1050,

		[EventName(JournalSubsystemType.GK, "Запыленность", XStateClass.Service)]
		Запыленность = 1051,

		[EventName(JournalSubsystemType.GK, "Запыленность устранена", XStateClass.Service)]
		Запыленность_устранена = 1052,

		[EventName(JournalSubsystemType.GK, "Информация", XStateClass.Info)]
		Информация = 1053,

		[EventName(JournalSubsystemType.GK, "Отсчет задержки", XStateClass.Info)]
		Отсчет_задержки = 1054,

		[EventName(JournalSubsystemType.GK, "Включено", XStateClass.On)]
		Включено = 1055,

		[EventName(JournalSubsystemType.GK, "Выключено", XStateClass.Off)]
		Выключено = 1056,

		[EventName(JournalSubsystemType.GK, "Включается", XStateClass.TurningOn)]
		Включается = 1057,

		[EventName(JournalSubsystemType.GK, "Выключается", XStateClass.TurningOff)]
		Выключается = 1058,

		[EventName(JournalSubsystemType.GK, "Кнопка", XStateClass.Info)]
		Кнопка = 1059,

		[EventName(JournalSubsystemType.GK, "Изменение автоматики по неисправности", XStateClass.AutoOff)]
		Изменение_автоматики_по_неисправности = 1060,

		[EventName(JournalSubsystemType.GK, "Изменение автоматики по кнопке СТОП", XStateClass.AutoOff)]
		Изменение_автоматики_по_кнопке_СТОП = 1061,

		[EventName(JournalSubsystemType.GK, "Изменение автоматики по датчику ДВЕРИ-ОКНА", XStateClass.AutoOff)]
		Изменение_автоматики_по_датчику_ДВЕРИ_ОКНА = 1062,

		[EventName(JournalSubsystemType.GK, "Изменение автоматики по ТМ", XStateClass.AutoOff)]
		Изменение_автоматики_по_ТМ = 1063,

		[EventName(JournalSubsystemType.GK, "Автоматика включена", XStateClass.AutoOff)]
		Автоматика_включена = 1064,

		[EventName(JournalSubsystemType.GK, "Ручной пуск АУП от ИПР", XStateClass.On)]
		Ручной_пуск_АУП_от_ИПР = 1065,

		[EventName(JournalSubsystemType.GK, "Отложенный пуск АУП по датчику ДВЕРИ-ОКНА", XStateClass.On)]
		Отложенный_пуск_АУП_по_датчику_ДВЕРИ_ОКНА = 1066,

		[EventName(JournalSubsystemType.GK, "Пуск АУП завершен", XStateClass.On)]
		Пуск_АУП_завершен = 1067,

		[EventName(JournalSubsystemType.GK, "Останов тушения по кнопке СТОП", XStateClass.Off)]
		Останов_тушения_по_кнопке_СТОП = 1068,

		[EventName(JournalSubsystemType.GK, "Программирование мастер-ключа", XStateClass.Info)]
		Программирование_мастер_ключа = 1069,

		[EventName(JournalSubsystemType.GK, "Отсчет удержания", XStateClass.Info)]
		Отсчет_удержания = 1070,

		[EventName(JournalSubsystemType.GK, "Уровень высокий", XStateClass.Info)]
		Уровень_высокий = 1071,

		[EventName(JournalSubsystemType.GK, "Уровень низкий", XStateClass.Info)]
		Уровень_низкий = 1072,

		[EventName(JournalSubsystemType.GK, "Ход по команде с УЗЗ", XStateClass.On)]
		Ход_по_команде_с_УЗЗ = 1073,

		[EventName(JournalSubsystemType.GK, "У ДУ сообщение ПУСК НЕВОЗМОЖЕН", XStateClass.Failure)]
		У_ДУ_сообщение_ПУСК_НЕВОЗМОЖЕН = 1074,

		[EventName(JournalSubsystemType.GK, "Авария пневмоемкости", XStateClass.Failure)]
		Авария_пневмоемкости = 1075,

		[EventName(JournalSubsystemType.GK, "Уровень аварийный", XStateClass.Failure)]
		Уровень_аварийный = 1076,

		[EventName(JournalSubsystemType.GK, "Запрет пуска НС", XStateClass.Off)]
		Запрет_пуска_НС = 1077,

		[EventName(JournalSubsystemType.GK, "Запрет пуска компрессора", XStateClass.Off)]
		Запрет_пуска_компрессора = 1078,

		[EventName(JournalSubsystemType.GK, "Команда с УЗН", XStateClass.Info)]
		Команда_с_УЗН = 1079,

		[EventName(JournalSubsystemType.GK, "Перевод в режим ручного управления", XStateClass.AutoOff)]
		Перевод_в_режим_ручного_управления = 1080,

		[EventName(JournalSubsystemType.GK, "Состояние не определено", XStateClass.Unknown)]
		Состояние_не_определено = 1081,

		[EventName(JournalSubsystemType.GK, "Остановлено", XStateClass.Off)]
		Остановлено = 1082,

		[EventName(JournalSubsystemType.GK, "Состояние Неизвестно", XStateClass.Unknown)]
		Состояние_Неизвестно = 1083,

		[EventName(JournalSubsystemType.GK, "Перевод в автоматический режим", XStateClass.Norm)]
		Перевод_в_автоматический_режим = 1084,

		[EventName(JournalSubsystemType.GK, "Перевод в ручной режим", XStateClass.AutoOff)]
		Перевод_в_ручной_режим = 1085,

		[EventName(JournalSubsystemType.GK, "Перевод в отключенный режим", XStateClass.Ignore)]
		Перевод_в_отключенный_режим = 1086,

		[EventName(JournalSubsystemType.GK, "Перевод в неопределенный режим", XStateClass.Unknown)]
		Перевод_в_неопределенный_режим = 1087,

		[EventName(JournalSubsystemType.GK, "Запись параметра", XStateClass.Info)]
		Запись_параметра = 1088,

		[EventName(JournalSubsystemType.GK, "Норма", XStateClass.Norm)]
		Норма = 1089,

		[EventName(JournalSubsystemType.GK, "Неизвестный код события объекта", XStateClass.Unknown)]
		Неизвестный_код_события_объекта = 1090,

		[EventName(JournalSubsystemType.SKD, "Потеря связи", XStateClass.ConnectionLost)]
		Потеря_связи = 2000,

		[EventName(JournalSubsystemType.SKD, "Восстановление связи", XStateClass.ConnectionLost)]
		Восстановление_связи = 2001,

		[EventName(JournalSubsystemType.SKD, "Проход разрешен", XStateClass.Info)]
		Проход_разрешен = 2002,

		[EventName(JournalSubsystemType.SKD, "Проход запрещен", XStateClass.Attention)]
		Проход_запрещен = 2003,

		[EventName(JournalSubsystemType.SKD, "Дверь не закрыта", XStateClass.Failure)]
		Дверь_не_закрыта = 2004,

		[EventName(JournalSubsystemType.SKD, "Взлом", XStateClass.Attention)]
		Взлом = 2005,

		[EventName(JournalSubsystemType.SKD, "Повторный_проход", XStateClass.Attention)]
		Повторный_проход = 2006,

		[EventName(JournalSubsystemType.SKD, "Принуждение", XStateClass.Fire1)]
		Принуждение = 2007,

		[EventName(JournalSubsystemType.SKD, "Открытие двери", XStateClass.On)]
		Открытие_двери = 2008,

		[EventName(JournalSubsystemType.SKD, "Закрытие двери", XStateClass.Off)]
		Закрытие_двери = 2009,

		[EventName(JournalSubsystemType.SKD, "Неизвестный статус двери", XStateClass.Unknown)]
		Неизвестный_статус_двери = 2010,

		[EventName(JournalSubsystemType.SKD, "Вскрытие контроллера", XStateClass.Attention)]
		Вскрытие_контроллера = 2011,

		[EventName(JournalSubsystemType.SKD, "Сброс Контроллера", XStateClass.TechnologicalRegime)]
		Сброс_Контроллера = 2012,

		[EventName(JournalSubsystemType.SKD, "Перезагрузка Контроллера", XStateClass.TechnologicalRegime)]
		Перезагрузка_Контроллера = 2013,

		[EventName(JournalSubsystemType.SKD, "Запись графиков работы", XStateClass.TechnologicalRegime)]
		Запись_графиков_работы = 2014,

		[EventName(JournalSubsystemType.SKD, "Перезапись всех карт", XStateClass.TechnologicalRegime)]
		Перезапись_всех_карт = 2015,

		[EventName(JournalSubsystemType.SKD, "Обновление ПО Контроллера", XStateClass.TechnologicalRegime)]
		Обновление_ПО_Контроллера = 2016,

		[EventName(JournalSubsystemType.SKD, "Запрос конфигурации контроллера", XStateClass.TechnologicalRegime)]
		Запрос_конфигурации_контроллера = 2017,

		[EventName(JournalSubsystemType.SKD, "Синхронизация времени контроллера", XStateClass.Info)]
		Синхронизация_времени_контроллера = 2018,

		[EventName(JournalSubsystemType.SKD, "Запрос конфигурации двери", XStateClass.TechnologicalRegime)]
		Запрос_конфигурации_двери = 2019,

		[EventName(JournalSubsystemType.SKD, "Запись конфигурации двери", XStateClass.TechnologicalRegime)]
		Запись_конфигурации_двери = 2020,

		[EventName(JournalSubsystemType.SKD, "Запрос направления контроллера", XStateClass.TechnologicalRegime)]
		Запрос_направления_контроллера = 2021,

		[EventName(JournalSubsystemType.SKD, "Запись направления контроллера", XStateClass.TechnologicalRegime)]
		Запись_направления_контроллера = 2022,

		[EventName(JournalSubsystemType.SKD, "Запись пароля контроллера", XStateClass.TechnologicalRegime)]
		Запись_пароля_контроллера = 2023,

		[EventName(JournalSubsystemType.SKD, "Запрос временных настроек контроллера", XStateClass.TechnologicalRegime)]
		Запрос_временных_настроек_контроллера = 2024,

		[EventName(JournalSubsystemType.SKD, "Запись временных настроек контроллера", XStateClass.TechnologicalRegime)]
		Запись_временных_настроек_контроллера = 2025,

		[EventName(JournalSubsystemType.SKD, "Запрос сетевых настроек контроллера", XStateClass.TechnologicalRegime)]
		Запрос_сетевых_настроек_контроллера = 2026,

		[EventName(JournalSubsystemType.SKD, "Запись сетевых настроек контроллера", XStateClass.TechnologicalRegime)]
		Запись_сетевых_настроек_контроллера = 2027,

		[EventName(JournalSubsystemType.SKD, "Команда на открытие двери", XStateClass.On)]
		Команда_на_открытие_двери = 2028,

		[EventName(JournalSubsystemType.SKD, "Команда на закрытие двери", XStateClass.Off)]
		Команда_на_закрытие_двери = 2029,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод двери в режим Открыто", XStateClass.On)]
		Команда_на_перевод_двери_в_режим_Открыто = 2030,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод двери в режим Закрыто", XStateClass.Off)]
		Команда_на_перевод_двери_в_режим_Закрыто = 2031,

		[EventName(JournalSubsystemType.SKD, "Команда на открытие зоны", XStateClass.On)]
		Команда_на_открытие_зоны = 2032,

		[EventName(JournalSubsystemType.SKD, "Команда на закрытие зоны", XStateClass.Off)]
		Команда_на_закрытие_зоны = 2033,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод зоны в режим Открыто", XStateClass.On)]
		Команда_на_перевод_зоны_в_режим_Открыто = 2034,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод зоны в режим Закрыто", XStateClass.Off)]
		Команда_на_перевод_зоны_в_режим_Закрыто = 2035,

		[EventName(JournalSubsystemType.SKD, "Команда на открытие точки доступа", XStateClass.On)]
		Команда_на_открытие_точки_доступа = 2036,

		[EventName(JournalSubsystemType.SKD, "Команда на закрытие точки доступа", XStateClass.Off)]
		Команда_на_закрытие_точки_доступа = 2037,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод точки доступа в режим Открыто", XStateClass.On)]
		Команда_на_перевод_точки_доступа_в_режим_Открыто = 2038,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод точки доступа в режим Закрыто", XStateClass.Off)]
		Команда_на_перевод_точки_доступа_в_режим_Закрыто = 2039,

		[EventName(JournalSubsystemType.SKD, "Добавление карты", XStateClass.Info)]
		Добавление_карты = 2040,

		[EventName(JournalSubsystemType.SKD, "Редактирование карты", XStateClass.Info)]
		Редактирование_карты = 2041,

		[EventName(JournalSubsystemType.SKD, "Удаление карты", XStateClass.Info)]
		Удаление_карты = 2042,

		[EventName(JournalSubsystemType.SKD, "Редактирование сотрудника", XStateClass.Info)]
		Редактирование_сотрудника = 2043,

		[EventName(JournalSubsystemType.SKD, "Редактирование отдела", XStateClass.Info)]
		Редактирование_отдела = 2044,

		[EventName(JournalSubsystemType.SKD, "Редактирование должности", XStateClass.Info)]
		Редактирование_должности = 2045,

		[EventName(JournalSubsystemType.SKD, "Редактирование шаблона доступа", XStateClass.Info)]
		Редактирование_шаблона_доступа = 2046,

		[EventName(JournalSubsystemType.SKD, "Редактирование организации", XStateClass.Info)]
		Редактирование_организации = 2047,

		[EventName(JournalSubsystemType.SKD, "Редактирование дополнительной колонки", XStateClass.Info)]
		Редактирование_дополнительной_колонки = 2048,

		[EventName(JournalSubsystemType.SKD, "Редактирование дневного графика", XStateClass.Info)]
		Редактирование_дневного_графика = 2049,

		[EventName(JournalSubsystemType.SKD, "Редактирование графика работы", XStateClass.Info)]
		Редактирование_графика_работы = 2050,

		[EventName(JournalSubsystemType.SKD, "Редактирование графика работы сотрудника", XStateClass.Info)]
		Редактирование_графика_работы_сотрудника = 2051,

		[EventName(JournalSubsystemType.SKD, "Редактирование праздничного дня", XStateClass.Info)]
		Редактирование_праздничного_дня = 2052,

		[EventName(JournalSubsystemType.SKD, "Внесение оправдательного документа", XStateClass.Info)]
		Внесение_оправдательного_документа = 2053,
	}
}