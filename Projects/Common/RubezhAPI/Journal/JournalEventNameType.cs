using RubezhAPI.GK;

namespace RubezhAPI.Journal
{
	public enum JournalEventNameType
	{
		[EventName(JournalSubsystemType.System, "NULL", XStateClass.No)]
		NULL = 0,

		[EventName(JournalSubsystemType.System, "Неизвестное событие", XStateClass.No)]
		Неизвестное_событие = 1,

		[EventName(JournalSubsystemType.GK, "Подтверждение тревоги", XStateClass.Fire1)]
		Подтверждение_тревоги = 2,

		[EventName(JournalSubsystemType.System, "Вход пользователя в систему", XStateClass.Info)]
		Вход_пользователя_в_систему = 3,

		[EventName(JournalSubsystemType.System, "Выход пользователя из системы", XStateClass.Info)]
		Выход_пользователя_из_системы = 4,

		[EventName(JournalSubsystemType.GK, "Зависание процесса опроса", XStateClass.Unknown)]
		Зависание_процесса_отпроса = 7,

		[EventName(JournalSubsystemType.System, "Отсутствует лицензия", XStateClass.HasNoLicense)]
		Отсутствует_лицензия = 8,

		[EventName(JournalSubsystemType.System, "Лицензия обнаружена", XStateClass.HasNoLicense)]
		Лицензия_обнаружена = 9,

		[EventName(JournalSubsystemType.GK, "Ошибка инициализации мониторинга", XStateClass.Unknown)]
		Ошибка_инициализации_мониторинга = 10,

		[EventName(JournalSubsystemType.System, "Отмена операции", XStateClass.TechnologicalRegime)]
		Отмена_операции = 11,

		[EventName(JournalSubsystemType.System, "Сообщение автоматизации", XStateClass.Info)]
		Сообщение_автоматизации = 12,

		[EventName(JournalSubsystemType.GK, "Обновление ПО прибора", XStateClass.Info)]
		Обновление_ПО_прибора = 13,

		[EventName(JournalSubsystemType.GK, "Запись конфигурации в прибор", XStateClass.Info)]
		Запись_конфигурации_в_прибор = 14,

		[EventName(JournalSubsystemType.GK, "Чтение конфигурации из прибора", XStateClass.Info)]
		Чтение_конфигурации_из_прибора = 15,

		[EventName(JournalSubsystemType.GK, "Запрос информации об устройстве", XStateClass.TechnologicalRegime)]
		Запрос_информации_об_устройстве = 16,

		[EventName(JournalSubsystemType.GK, "Синхронизация времени", XStateClass.Info)]
		Синхронизация_времени = 17,

		[EventName(JournalSubsystemType.GK, "Команда оператора", XStateClass.Info)]
		Команда_оператора = 18,

		[EventName(JournalSubsystemType.GK, "Ошибка при выполнении команды", XStateClass.Failure)]
		Ошибка_при_выполнении_команды = 19,

		[EventName(JournalSubsystemType.GK, "Ошибка при выполнении команды над устройством", XStateClass.Failure)]
		Ошибка_при_выполнении_команды_над_устройством = 20,

		[EventName(JournalSubsystemType.GK, "Нет связи с ГК", XStateClass.ConnectionLost)]
		Нет_связи_с_ГК = 21,

		[EventName(JournalSubsystemType.GK, "Связь с ГК восстановлена", XStateClass.ConnectionLost)]
		Связь_с_ГК_восстановлена = 22,

		[EventName(JournalSubsystemType.GK, "Конфигурация прибора не соответствует конфигурации ПК", XStateClass.Unknown)]
		Конфигурация_прибора_не_соответствует_конфигурации_ПК = 23,

		[EventName(JournalSubsystemType.GK, "Конфигурация прибора соответствует конфигурации ПК", XStateClass.Unknown)]
		Конфигурация_прибора_соответствует_конфигурации_ПК = 24,

		[EventName(JournalSubsystemType.GK, "Ошибка при синхронизации журнала", XStateClass.Unknown)]
		Ошибка_при_синхронизации_журнала = 25,

		[EventName(JournalSubsystemType.GK, "Ошибка при опросе состояний компонентов ГК", XStateClass.Unknown)]
		Ошибка_при_опросе_состояний_компонентов_ГК = 26,

		[EventName(JournalSubsystemType.GK, "Устранена ошибка при опросе состояний компонентов ГК", XStateClass.Unknown)]
		Устранена_ошибка_при_опросе_состояний_компонентов_ГК = 27,

		[EventName(JournalSubsystemType.GK, "Восстановление связи с прибором", XStateClass.ConnectionLost)]
		Восстановление_связи_с_прибором = 28,

		[EventName(JournalSubsystemType.GK, "Потеря связи с прибором", XStateClass.ConnectionLost)]
		Потеря_связи_с_прибором = 29,

		[EventName(JournalSubsystemType.GK, "База данных прибора не соответствует базе данных ПК", XStateClass.Unknown)]
		База_данных_прибора_не_соответствует_базе_данных_ПК = 30,

		[EventName(JournalSubsystemType.GK, "База данных прибора соответствует базе данных ПК", XStateClass.Unknown)]
		База_данных_прибора_соответствует_базе_данных_ПК = 31,

		[EventName(JournalSubsystemType.System, "Применение конфигурации", XStateClass.Info)]
		Применение_конфигурации = 32,

		[EventName(JournalSubsystemType.GK, "ГК в технологическом режиме", XStateClass.TechnologicalRegime)]
		ГК_в_технологическом_режиме = 33,

		[EventName(JournalSubsystemType.GK, "ГК в рабочем режиме", XStateClass.Info)]
		ГК_в_рабочем_режиме = 34,

		[EventName(JournalSubsystemType.GK, "Запись всех идентификаторов", XStateClass.Info)]
		Запись_всех_идентификаторов = 35,

		[EventName(JournalSubsystemType.GK, "Перевод в технологический режим", XStateClass.TechnologicalRegime)]
		Перевод_в_технологический_режим = 36,

		[EventName(JournalSubsystemType.GK, "Синхронизация времени прибора с временем ПК", XStateClass.Info)]
		Синхронизация_времени_прибора_с_временем_ПК = 37,

		[EventName(JournalSubsystemType.GK, "Смена ПО", XStateClass.TechnologicalRegime)]
		Смена_ПО = 38,

		[EventName(JournalSubsystemType.GK, "Смена БД", XStateClass.TechnologicalRegime)]
		Смена_БД = 39,

		[EventName(JournalSubsystemType.GK, "Перевод в рабочий режим", XStateClass.Info)]
		Перевод_в_рабочий_режим = 40,

		[EventName(JournalSubsystemType.GK, "Вход пользователя в прибор", XStateClass.Info)]
		Вход_пользователя_в_прибор = 41,

		[EventName(JournalSubsystemType.GK, "Выход пользователя из прибора", XStateClass.Info)]
		Выход_пользователя_из_прибора = 42,

		[EventName(JournalSubsystemType.GK, "Ошибка управления", XStateClass.Failure)]
		Ошибка_управления = 43,

		[EventName(JournalSubsystemType.GK, "Введен новый пользователь", XStateClass.Info)]
		Введен_новый_пользователь = 44,

		[EventName(JournalSubsystemType.GK, "Изменена учетная информация пользователя", XStateClass.Info)]
		Изменена_учетная_информация_пользователя = 45,

		[EventName(JournalSubsystemType.GK, "Произведена настройка сети", XStateClass.Info)]
		Произведена_настройка_сети = 46,

		[EventName(JournalSubsystemType.GK, "Неизвестный код события контроллера", XStateClass.Unknown)]
		Неизвестный_код_события_контроллекра = 47,

		[EventName(JournalSubsystemType.GK, "Неизвестный тип", XStateClass.Unknown)]
		Неизвестный_тип = 48,

		[EventName(JournalSubsystemType.GK, "Устройство с таким адресом не описано при конфигурации", XStateClass.Unknown)]
		Устройство_с_таким_адресом_не_описано_при_конфигурации = 49,

		[EventName(JournalSubsystemType.GK, "Неизвестный код события устройства", XStateClass.Unknown)]
		Неизвестный_код_события_устройства = 50,

		[EventName(JournalSubsystemType.GK, "При конфигурации описан другой тип", XStateClass.Unknown)]
		При_конфигурации_описан_другой_тип = 51,

		[EventName(JournalSubsystemType.GK, "Изменился заводской номер", XStateClass.Info)]
		Изменился_заводской_номер = 52,

		[EventName(JournalSubsystemType.GK, "Пожар-1", XStateClass.Fire1)]
		Пожар_1 = 53,

		[EventName(JournalSubsystemType.GK, "Сработка-1", XStateClass.Fire1)]
		Сработка_1 = 54,

		[EventName(JournalSubsystemType.GK, "Тревога", XStateClass.Fire1)]
		Тревога = 55,

		[EventName(JournalSubsystemType.GK, "Пожар-2", XStateClass.Fire2)]
		Пожар_2 = 56,

		[EventName(JournalSubsystemType.GK, "Сработка-2", XStateClass.Fire2)]
		Сработка_2 = 57,

		[EventName(JournalSubsystemType.GK, "Внимание", XStateClass.Attention)]
		Внимание = 58,

		[EventName(JournalSubsystemType.GK, "Неисправность", XStateClass.Failure)]
		Неисправность = 59,

		[EventName(JournalSubsystemType.GK, "Неисправность устранена", XStateClass.Norm)]
		Неисправность_устранена = 60,

		[EventName(JournalSubsystemType.GK, "Тест", XStateClass.Test)]
		Тест = 61,

		[EventName(JournalSubsystemType.GK, "Тест устранен", XStateClass.Test)]
		Тест_устранен = 62,

		[EventName(JournalSubsystemType.GK, "Запыленность", XStateClass.Service)]
		Запыленность = 63,

		[EventName(JournalSubsystemType.GK, "Запыленность устранена", XStateClass.Service)]
		Запыленность_устранена = 64,

		[EventName(JournalSubsystemType.GK, "Информация", XStateClass.Info)]
		Информация = 65,

		[EventName(JournalSubsystemType.GK, "Отсчет задержки", XStateClass.Info)]
		Отсчет_задержки = 67,

		[EventName(JournalSubsystemType.GK, "Включено", XStateClass.On)]
		Включено = 68,

		[EventName(JournalSubsystemType.GK, "Выключено", XStateClass.Off)]
		Выключено = 69,

		[EventName(JournalSubsystemType.GK, "Включается", XStateClass.TurningOn)]
		Включается = 70,

		[EventName(JournalSubsystemType.GK, "Выключается", XStateClass.TurningOff)]
		Выключается = 71,

		[EventName(JournalSubsystemType.GK, "Кнопка", XStateClass.Info)]
		Кнопка = 72,

		[EventName(JournalSubsystemType.GK, "Изменение автоматики по неисправности", XStateClass.AutoOff)]
		Изменение_автоматики_по_неисправности = 73,

		[EventName(JournalSubsystemType.GK, "Изменение автоматики по кнопке СТОП", XStateClass.AutoOff)]
		Изменение_автоматики_по_кнопке_СТОП = 74,

		[EventName(JournalSubsystemType.GK, "Изменение автоматики по датчику ДВЕРИ-ОКНА", XStateClass.AutoOff)]
		Изменение_автоматики_по_датчику_ДВЕРИ_ОКНА = 75,

		[EventName(JournalSubsystemType.GK, "Изменение автоматики по ТМ", XStateClass.AutoOff)]
		Изменение_автоматики_по_ТМ = 76,

		[EventName(JournalSubsystemType.GK, "Автоматика включена", XStateClass.AutoOff)]
		Автоматика_включена = 77,

		[EventName(JournalSubsystemType.GK, "Ручной пуск АУП от ИПР", XStateClass.On)]
		Ручной_пуск_АУП_от_ИПР = 78,

		[EventName(JournalSubsystemType.GK, "Отложенный пуск АУП по датчику ДВЕРИ-ОКНА", XStateClass.On)]
		Отложенный_пуск_АУП_по_датчику_ДВЕРИ_ОКНА = 79,

		[EventName(JournalSubsystemType.GK, "Пуск АУП завершен", XStateClass.On)]
		Пуск_АУП_завершен = 80,

		[EventName(JournalSubsystemType.GK, "Останов тушения по кнопке СТОП", XStateClass.Off)]
		Останов_тушения_по_кнопке_СТОП = 81,

		[EventName(JournalSubsystemType.GK, "Программирование мастер-ключа", XStateClass.Info)]
		Программирование_мастер_ключа = 82,

		[EventName(JournalSubsystemType.GK, "Отсчет удержания", XStateClass.Info)]
		Отсчет_удержания = 83,

		[EventName(JournalSubsystemType.GK, "Уровень высокий", XStateClass.Info)]
		Уровень_высокий = 84,

		[EventName(JournalSubsystemType.GK, "Уровень низкий", XStateClass.Info)]
		Уровень_низкий = 85,

		[EventName(JournalSubsystemType.GK, "Ход по команде с УЗЗ", XStateClass.On)]
		Ход_по_команде_с_УЗЗ = 86,

		[EventName(JournalSubsystemType.GK, "У ДУ сообщение ПУСК НЕВОЗМОЖЕН", XStateClass.Failure)]
		У_ДУ_сообщение_ПУСК_НЕВОЗМОЖЕН = 87,

		[EventName(JournalSubsystemType.GK, "Авария пневмоемкости", XStateClass.Failure)]
		Авария_пневмоемкости = 88,

		[EventName(JournalSubsystemType.GK, "Уровень аварийный", XStateClass.Failure)]
		Уровень_аварийный = 89,

		[EventName(JournalSubsystemType.GK, "Запрет пуска НС", XStateClass.Off)]
		Запрет_пуска_НС = 90,

		[EventName(JournalSubsystemType.GK, "Запрет пуска компрессора", XStateClass.Off)]
		Запрет_пуска_компрессора = 91,

		[EventName(JournalSubsystemType.GK, "Команда с УЗН", XStateClass.Info)]
		Команда_с_УЗН = 92,

		[EventName(JournalSubsystemType.GK, "Перевод в режим ручного управления", XStateClass.AutoOff)]
		Перевод_в_режим_ручного_управления = 93,

		[EventName(JournalSubsystemType.GK, "Состояние не определено", XStateClass.Unknown)]
		Состояние_не_определено = 94,

		[EventName(JournalSubsystemType.GK, "Остановлено", XStateClass.Off)]
		Остановлено = 95,

		[EventName(JournalSubsystemType.GK, "Состояние Неизвестно", XStateClass.Unknown)]
		Состояние_Неизвестно = 96,

		[EventName(JournalSubsystemType.GK, "Перевод в автоматический режим", XStateClass.Norm)]
		Перевод_в_автоматический_режим = 97,

		[EventName(JournalSubsystemType.GK, "Перевод в ручной режим", XStateClass.AutoOff)]
		Перевод_в_ручной_режим = 98,

		[EventName(JournalSubsystemType.GK, "Перевод в отключенный режим", XStateClass.Ignore)]
		Перевод_в_отключенный_режим = 99,

		[EventName(JournalSubsystemType.GK, "Перевод в неопределенный режим", XStateClass.Unknown)]
		Перевод_в_неопределенный_режим = 100,

		[EventName(JournalSubsystemType.GK, "Запись параметра", XStateClass.Info)]
		Запись_параметра = 101,

		[EventName(JournalSubsystemType.GK, "Норма", XStateClass.Norm)]
		Норма = 102,

		[EventName(JournalSubsystemType.GK, "Неизвестный код события объекта", XStateClass.Unknown)]
		Неизвестный_код_события_объекта = 103,

		//----------------------------------------------------

		[EventName(JournalSubsystemType.SKD, "Добавление карты", XStateClass.Info)]
		Добавление_карты = 147,

		[EventName(JournalSubsystemType.SKD, "Редактирование карты", XStateClass.Info)]
		Редактирование_карты = 148,

		[EventName(JournalSubsystemType.SKD, "Удаление карты", XStateClass.Info)]
		Удаление_карты = 149,

		[EventName(JournalSubsystemType.SKD, "Редактирование сотрудника", XStateClass.Info)]
		Редактирование_сотрудника = 150,

		[EventName(JournalSubsystemType.SKD, "Редактирование посетителя", XStateClass.Info)]
		Редактирование_посетителя = 218,

		[EventName(JournalSubsystemType.SKD, "Редактирование подразделения", XStateClass.Info)]
		Редактирование_отдела = 151,

		[EventName(JournalSubsystemType.SKD, "Редактирование должности", XStateClass.Info)]
		Редактирование_должности = 152,

		[EventName(JournalSubsystemType.SKD, "Редактирование шаблона доступа", XStateClass.Info)]
		Редактирование_шаблона_доступа = 153,

		[EventName(JournalSubsystemType.SKD, "Редактирование организации", XStateClass.Info)]
		Редактирование_организации = 154,

		[EventName(JournalSubsystemType.SKD, "Редактирование дополнительной колонки", XStateClass.Info)]
		Редактирование_дополнительной_колонки = 155,

		[EventName(JournalSubsystemType.SKD, "Редактирование дневного графика", XStateClass.Info)]
		Редактирование_дневного_графика = 156,

		[EventName(JournalSubsystemType.SKD, "Редактирование графика работы", XStateClass.Info)]
		Редактирование_графика_работы = 157,

		[EventName(JournalSubsystemType.SKD, "Редактирование графика работы сотрудника", XStateClass.Info)]
		Редактирование_графика_работы_сотрудника = 158,

		[EventName(JournalSubsystemType.SKD, "Редактирование сокращённого дня", XStateClass.Info)]
		Редактирование_праздничного_дня = 159,

		[EventName(JournalSubsystemType.SKD, "Редактирование шаблона пропуска", XStateClass.Info)]
		Редактирование_шаблона_пропуска = 160,

		[EventName(JournalSubsystemType.GK, "Редактирование графика ГК", XStateClass.Info)]
		Редактирование_графика_ГК = 161,

		[EventName(JournalSubsystemType.GK, "Редактирование дневного графика ГК", XStateClass.Info)]
		Редактирование_дневного_графика_ГК = 162,

		[EventName(JournalSubsystemType.SKD, "Внесение оправдательного документа", XStateClass.Info)]
		Внесение_оправдательного_документа = 174,

		[EventName(JournalSubsystemType.GK, "Добавление нового графика ГК", XStateClass.Info)]
		Добавление_нового_графика_ГК = 175,

		[EventName(JournalSubsystemType.GK, "Добавление нового дневного графика ГК", XStateClass.Info)]
		Добавление_нового_дневного_графика_ГК = 176,

		[EventName(JournalSubsystemType.GK, "Открыто", XStateClass.On)]
		Открыто = 177,

		[EventName(JournalSubsystemType.GK, "Закрыто", XStateClass.Off)]
		Закрыто = 178,

		[EventName(JournalSubsystemType.GK, "Открытие", XStateClass.On)]
		Открытие = 179,

		[EventName(JournalSubsystemType.GK, "Закрытие", XStateClass.Off)]
		Закрытие = 180,

		[EventName(JournalSubsystemType.GK, "Автопоиск", XStateClass.Info)]
		Автопоиск = 182,

		[EventName(JournalSubsystemType.GK, "Проход пользователя разрешен", XStateClass.Info)]
		Проход_пользователя_разрешен = 181,

		[EventName(JournalSubsystemType.GK, "На охране", XStateClass.On)]
		На_охране = 183,

		[EventName(JournalSubsystemType.GK, "Не на охране", XStateClass.Off)]
		Не_на_охране = 184,

		[EventName(JournalSubsystemType.GK, "Постановка на охрану", XStateClass.On)]
		Постановка_на_охрану = 185,

		[EventName(JournalSubsystemType.GK, "Снятие с охраны", XStateClass.Off)]
		Снятие_с_охраны = 186,

		[EventName(JournalSubsystemType.GK, "Рабочий график", XStateClass.TechnologicalRegime)]
		Рабочий_график = 187,

		[EventName(JournalSubsystemType.GK, "Удаление графика ГК", XStateClass.Info)]
		Удаление_графика_ГК = 199,

		[EventName(JournalSubsystemType.GK, "Удаление дневного графика ГК", XStateClass.Info)]
		Удаление_дневного_графика_ГК = 200,

		[EventName(JournalSubsystemType.GK, "Открытие зоны СКД", XStateClass.On)]
		Открытие_зоны_СКД = 212,

		[EventName(JournalSubsystemType.GK, "Закрытие зоны СКД", XStateClass.Off)]
		Закрытие_зоны_СКД = 213,

		[EventName(JournalSubsystemType.GK, "Проход пользователя запрещен", XStateClass.Attention)]
		Проход_пользователя_запрещен = 214,

		[EventName(JournalSubsystemType.Video, "Перевод в предустановку", XStateClass.On)]
		Перевод_в_предустановку = 215,

		[EventName(JournalSubsystemType.GK, "Начало мониторинга", XStateClass.Norm)]
		Начало_мониторинга = 216,

		[EventName(JournalSubsystemType.GK, "Управление ПМФ", XStateClass.Info)]
		Управление_ПМФ = 220,

		[EventName(JournalSubsystemType.Video, "Канал RVi поставлен на охрану", XStateClass.Info)]
		Канал_RVi_поставлен_на_охрану = 221,

		[EventName(JournalSubsystemType.Video, "Канал RVi снят с охраны", XStateClass.Info)]
		Канал_RVi_снят_с_охраны = 222,

		[EventName(JournalSubsystemType.Video, "Начата запись на канале RVi", XStateClass.Info)]
		Начата_запись_на_канале_RVi = 223,

		[EventName(JournalSubsystemType.Video, "Прекращена запись на канале RVi", XStateClass.Info)]
		Прекращена_запись_на_канале_RVi = 224
	}
}