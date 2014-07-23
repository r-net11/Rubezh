using System.ComponentModel;
using FiresecAPI.GK;

namespace FiresecAPI.Journal
{
	public enum JournalEventNameType
	{
		[DescriptionAttribute("")]
		NULL,

		[EventDescription(JournalSubsystemType.System, "Неизвестное событие", XStateClass.No)]
		Неизвестное_событие,

		[EventDescription(JournalSubsystemType.System, "Подтверждение тревоги", XStateClass.Fire1)]
		Подтверждение_тревоги,

		[EventDescription(JournalSubsystemType.System, "Вход пользователя в систему", XStateClass.Info)]
		Вход_пользователя_в_систему,

		[EventDescription(JournalSubsystemType.System, "Выход пользователя из системы", XStateClass.Info)]
		Выход_пользователя_из_системы,

		[EventDescription(JournalSubsystemType.System, "Дежурство сдал", XStateClass.Info)]
		Дежурство_сдал,

		[EventDescription(JournalSubsystemType.System, "Дежурство принял", XStateClass.Info)]
		Дежурство_принял,

		[EventDescription(JournalSubsystemType.System, "Зависание процесса отпроса", XStateClass.Unknown)]
		Зависание_процесса_отпроса,

		[EventDescription(JournalSubsystemType.System, "Отсутствует лицензия", XStateClass.HasNoLicense)]
		Отсутствует_лицензия,

		[EventDescription(JournalSubsystemType.System, "Лицензия обнаружена", XStateClass.HasNoLicense)]
		Лицензия_обнаружена,

		[EventDescription(JournalSubsystemType.System, "Ошибка инициализации мониторинга", XStateClass.Unknown)]
		Ошибка_инициализации_мониторинга,

		[EventDescription(JournalSubsystemType.System, "Отмена операции", XStateClass.TechnologicalRegime)]
		Отмена_операции,

		[EventDescription(JournalSubsystemType.GK, "Обновление ПО прибора", XStateClass.Info)]
		Обновление_ПО_прибора,

		[EventDescription(JournalSubsystemType.GK, "Запись конфигурации в прибор", XStateClass.Info)]
		Запись_конфигурации_в_прибор,

		[EventDescription(JournalSubsystemType.GK, "Чтение конфигурации из прибора", XStateClass.Info)]
		Чтение_конфигурации_из_прибора,

		[EventDescription(JournalSubsystemType.GK, "Запрос информации об устройстве", XStateClass.TechnologicalRegime)]
		Запрос_информации_об_устройстве,

		[EventDescription(JournalSubsystemType.GK, "Синхронизация времени", XStateClass.Info)]
		Синхронизация_времени,

		[EventDescription(JournalSubsystemType.GK, "Команда оператора", XStateClass.Info)]
		Команда_оператора,

		[EventDescription(JournalSubsystemType.GK, "Ошибка при выполнении команды", XStateClass.Failure)]
		Ошибка_при_выполнении_команды,

		[EventDescription(JournalSubsystemType.GK, "Ошибка при выполнении команды над устройством", XStateClass.Failure)]
		Ошибка_при_выполнении_команды_над_устройством,

		[EventDescription(JournalSubsystemType.GK, "Нет связи с ГК", XStateClass.ConnectionLost)]
		Нет_связи_с_ГК,

		[EventDescription(JournalSubsystemType.GK, "Связь с ГК восстановлена", XStateClass.ConnectionLost)]
		Связь_с_ГК_восстановлена,

		[EventDescription(JournalSubsystemType.GK, "Конфигурация прибора не соответствует конфигурации ПК", XStateClass.Unknown)]
		Конфигурация_прибора_не_соответствует_конфигурации_ПК,

		[EventDescription(JournalSubsystemType.GK, "Конфигурация прибора соответствует конфигурации ПК", XStateClass.Unknown)]
		Конфигурация_прибора_соответствует_конфигурации_ПК,

		[EventDescription(JournalSubsystemType.GK, "Ошибка при синхронизации журнала", XStateClass.Unknown)]
		Ошибка_при_синхронизации_журнала,

		[EventDescription(JournalSubsystemType.GK, "Ошибка при опросе состояний компонентов ГК", XStateClass.Unknown)]
		Ошибка_при_опросе_состояний_компонентов_ГК,

		[EventDescription(JournalSubsystemType.GK, "Устранена ошибка при опросе состояний компонентов ГК", XStateClass.Unknown)]
		Устранена_ошибка_при_опросе_состояний_компонентов_ГК,

		[EventDescription(JournalSubsystemType.GK, "Восстановление связи с прибором", XStateClass.ConnectionLost)]
		Восстановление_связи_с_прибором,

		[EventDescription(JournalSubsystemType.GK, "Потеря связи с прибором", XStateClass.ConnectionLost)]
		Потеря_связи_с_прибором,

		[EventDescription(JournalSubsystemType.System, "База данных прибора не соответствует базе данных ПК", XStateClass.Unknown)]
		База_данных_прибора_не_соответствует_базе_данных_ПК,

		[EventDescription(JournalSubsystemType.GK, "База данных прибора соответствует базе данных ПК", XStateClass.Unknown)]
		База_данных_прибора_соответствует_базе_данных_ПК,

		[EventDescription(JournalSubsystemType.GK, "Применение конфигурации", XStateClass.Info)]
		Применение_конфигурации,

		[EventDescription(JournalSubsystemType.GK, "ГК в технологическом режиме", XStateClass.TechnologicalRegime)]
		ГК_в_технологическом_режиме,

		[EventDescription(JournalSubsystemType.GK, "ГК в рабочем режиме", XStateClass.Info)]
		ГК_в_рабочем_режиме,

		[EventDescription(JournalSubsystemType.GK, "Запись всех идентификаторов", XStateClass.Info)]
		Запись_всех_идентификаторов,

		[EventDescription(JournalSubsystemType.GK, "Перевод в технологический режим", XStateClass.TechnologicalRegime)]
		Перевод_в_технологический_режим,

		[EventDescription(JournalSubsystemType.GK, "Синхронизация времени прибора с временем ПК", XStateClass.Info)]
		Синхронизация_времени_прибора_с_временем_ПК,

		[EventDescription(JournalSubsystemType.GK, "Смена ПО", XStateClass.TechnologicalRegime)]
		Смена_ПО,

		[EventDescription(JournalSubsystemType.GK, "Смена БД", XStateClass.TechnologicalRegime)]
		Смена_БД,

		[EventDescription(JournalSubsystemType.GK, "Перевод в рабочий режим", XStateClass.Info)]
		Перевод_в_рабочий_режим,

		[EventDescription(JournalSubsystemType.GK, "Вход пользователя в прибор", XStateClass.Info)]
		Вход_пользователя_в_прибор,

		[EventDescription(JournalSubsystemType.GK, "Выход пользователя из прибора", XStateClass.Info)]
		Выход_пользователя_из_прибора,

		[EventDescription(JournalSubsystemType.GK, "Ошибка управления", XStateClass.Failure)]
		Ошибка_управления,

		[EventDescription(JournalSubsystemType.GK, "Введен новый пользователь", XStateClass.Info)]
		Введен_новый_пользователь,

		[EventDescription(JournalSubsystemType.GK, "Изменена учетная информация пользователя", XStateClass.Info)]
		Изменена_учетная_информация_пользователя,

		[EventDescription(JournalSubsystemType.GK, "Произведена настройка сети", XStateClass.Info)]
		Произведена_настройка_сети,

		[EventDescription(JournalSubsystemType.GK, "Неизвестный код события контроллекра", XStateClass.Unknown)]
		Неизвестный_код_события_контроллекра,

		[EventDescription(JournalSubsystemType.GK, "Неизвестный тип", XStateClass.Unknown)]
		Неизвестный_тип,

		[EventDescription(JournalSubsystemType.GK, "Устройство с таким адресом не описано при конфигурации", XStateClass.Unknown)]
		Устройство_с_таким_адресом_не_описано_при_конфигурации,

		[EventDescription(JournalSubsystemType.GK, "Неизвестный код события устройства", XStateClass.Unknown)]
		Неизвестный_код_события_устройства,

		[EventDescription(JournalSubsystemType.GK, "При конфигурации описан другой тип", XStateClass.Unknown)]
		При_конфигурации_описан_другой_тип,

		[EventDescription(JournalSubsystemType.GK, "Изменился заводской номер", XStateClass.Info)]
		Изменился_заводской_номер,

		[EventDescription(JournalSubsystemType.GK, "Пожар-1", XStateClass.Fire1)]
		Пожар_1,

		[EventDescription(JournalSubsystemType.GK, "Сработка-1", XStateClass.Fire1)]
		Сработка_1,

		[EventDescription(JournalSubsystemType.GK, "Пожар-2", XStateClass.Fire2)]
		Пожар_2,

		[EventDescription(JournalSubsystemType.GK, "Сработка-2", XStateClass.Fire2)]
		Сработка_2,

		[EventDescription(JournalSubsystemType.GK, "Внимание", XStateClass.Attention)]
		Внимание,

		[EventDescription(JournalSubsystemType.GK, "Неисправность", XStateClass.Failure)]
		Неисправность,

		[EventDescription(JournalSubsystemType.GK, "Неисправность устранена", XStateClass.Norm)]
		Неисправность_устранена,

		[EventDescription(JournalSubsystemType.GK, "Тест", XStateClass.Test)]
		Тест,

		[EventDescription(JournalSubsystemType.GK, "Тест устранен", XStateClass.Test)]
		Тест_устранен,

		[EventDescription(JournalSubsystemType.GK, "Запыленность", XStateClass.Service)]
		Запыленность,

		[EventDescription(JournalSubsystemType.GK, "Запыленность устранена", XStateClass.Service)]
		Запыленность_устранена,

		[EventDescription(JournalSubsystemType.GK, "Информация", XStateClass.Info)]
		Информация,

		[EventDescription(JournalSubsystemType.GK, "Отсчет задержки", XStateClass.Info)]
		Отсчет_задержки,

		[EventDescription(JournalSubsystemType.GK, "Включено", XStateClass.On)]
		Включено,

		[EventDescription(JournalSubsystemType.GK, "Выключено", XStateClass.Off)]
		Выключено,

		[EventDescription(JournalSubsystemType.GK, "Включается", XStateClass.TurningOn)]
		Включается,

		[EventDescription(JournalSubsystemType.GK, "Выключается", XStateClass.TurningOff)]
		Выключается,

		[EventDescription(JournalSubsystemType.GK, "Кнопка", XStateClass.Info)]
		Кнопка,

		[EventDescription(JournalSubsystemType.GK, "Изменение автоматики по неисправности", XStateClass.AutoOff)]
		Изменение_автоматики_по_неисправности,

		[EventDescription(JournalSubsystemType.GK, "Изменение автоматики по кнопке СТОП", XStateClass.AutoOff)]
		Изменение_автоматики_по_кнопке_СТОП,

		[EventDescription(JournalSubsystemType.GK, "Изменение автоматики по датчику ДВЕРИ-ОКНА", XStateClass.AutoOff)]
		Изменение_автоматики_по_датчику_ДВЕРИ_ОКНА,

		[EventDescription(JournalSubsystemType.GK, "Изменение автоматики по ТМ", XStateClass.AutoOff)]
		Изменение_автоматики_по_ТМ,

		[EventDescription(JournalSubsystemType.GK, "Автоматика включена", XStateClass.AutoOff)]
		Автоматика_включена,

		[EventDescription(JournalSubsystemType.GK, "Ручной пуск АУП от ИПР", XStateClass.On)]
		Ручной_пуск_АУП_от_ИПР,

		[EventDescription(JournalSubsystemType.GK, "Отложенный пуск АУП по датчику ДВЕРИ-ОКНА", XStateClass.On)]
		Отложенный_пуск_АУП_по_датчику_ДВЕРИ_ОКНА,

		[EventDescription(JournalSubsystemType.GK, "Пуск АУП завершен", XStateClass.On)]
		Пуск_АУП_завершен,

		[EventDescription(JournalSubsystemType.GK, "Останов тушения по кнопке СТОП", XStateClass.Off)]
		Останов_тушения_по_кнопке_СТОП,

		[EventDescription(JournalSubsystemType.GK, "Программирование мастер-ключа", XStateClass.Info)]
		Программирование_мастер_ключа,

		[EventDescription(JournalSubsystemType.GK, "Отсчет удержания", XStateClass.Info)]
		Отсчет_удержания,

		[EventDescription(JournalSubsystemType.GK, "Уровень высокий", XStateClass.Info)]
		Уровень_высокий,

		[EventDescription(JournalSubsystemType.GK, "Уровень низкий", XStateClass.Info)]
		Уровень_низкий,

		[EventDescription(JournalSubsystemType.GK, "Ход по команде с УЗЗ", XStateClass.On)]
		Ход_по_команде_с_УЗЗ,

		[EventDescription(JournalSubsystemType.GK, "У ДУ сообщение ПУСК НЕВОЗМОЖЕН", XStateClass.Failure)]
		У_ДУ_сообщение_ПУСК_НЕВОЗМОЖЕН,

		[EventDescription(JournalSubsystemType.GK, "Авария пневмоемкости", XStateClass.Failure)]
		Авария_пневмоемкости,

		[EventDescription(JournalSubsystemType.GK, "Уровень аварийный", XStateClass.Failure)]
		Уровень_аварийный,

		[EventDescription(JournalSubsystemType.GK, "Запрет пуска НС", XStateClass.Off)]
		Запрет_пуска_НС,

		[EventDescription(JournalSubsystemType.GK, "Запрет пуска компрессора", XStateClass.Off)]
		Запрет_пуска_компрессора,

		[EventDescription(JournalSubsystemType.GK, "Команда с УЗН", XStateClass.Info)]
		Команда_с_УЗН,

		[EventDescription(JournalSubsystemType.GK, "Перевод в режим ручного управления", XStateClass.AutoOff)]
		Перевод_в_режим_ручного_управления,

		[EventDescription(JournalSubsystemType.GK, "Состояние не определено", XStateClass.Unknown)]
		Состояние_не_определено,

		[EventDescription(JournalSubsystemType.GK, "Остановлено", XStateClass.Off)]
		Остановлено,

		[EventDescription(JournalSubsystemType.GK, "Состояние Неизвестно", XStateClass.Unknown)]
		Состояние_Неизвестно,

		[EventDescription(JournalSubsystemType.GK, "Перевод в автоматический режим", XStateClass.Norm)]
		Перевод_в_автоматический_режим,

		[EventDescription(JournalSubsystemType.GK, "Перевод в ручной режим", XStateClass.AutoOff)]
		Перевод_в_ручной_режим,

		[EventDescription(JournalSubsystemType.GK, "Перевод в отключенный режим", XStateClass.Ignore)]
		Перевод_в_отключенный_режим,

		[EventDescription(JournalSubsystemType.GK, "Перевод в неопределенный режим", XStateClass.Unknown)]
		Перевод_в_неопределенный_режим,

		[EventDescription(JournalSubsystemType.GK, "Запись параметра", XStateClass.Info)]
		Запись_параметра,

		[EventDescription(JournalSubsystemType.GK, "Норма", XStateClass.Norm)]
		Норма,

		[EventDescription(JournalSubsystemType.GK, "Неизвестный код события объекта", XStateClass.Unknown)]
		Неизвестный_код_события_объекта,

		[EventDescription(JournalSubsystemType.SKD, "Потеря связи", XStateClass.ConnectionLost)]
		Потеря_связи,

		[EventDescription(JournalSubsystemType.SKD, "Восстановление связи", XStateClass.ConnectionLost)]
		Восстановление_связи,

		[EventDescription(JournalSubsystemType.SKD, "Проход", XStateClass.Info)]
		Проход,

		[EventDescription(JournalSubsystemType.SKD, "Дверь не закрыта", XStateClass.Failure)]
		Дверь_не_закрыта,

		[EventDescription(JournalSubsystemType.SKD, "Взлом", XStateClass.Attention)]
		Взлом,

		[EventDescription(JournalSubsystemType.SKD, "Повторный_проход", XStateClass.Attention)]
		Повторный_проход,

		[EventDescription(JournalSubsystemType.SKD, "Принуждение", XStateClass.Attention)]
		Принуждение,

		[EventDescription(JournalSubsystemType.SKD, "Открытие двери", XStateClass.On)]
		Открытие_двери,

		[EventDescription(JournalSubsystemType.SKD, "Закрытие двери", XStateClass.Off)]
		Закрытие_двери,

		[EventDescription(JournalSubsystemType.SKD, "Неизвестный статус двери", XStateClass.Unknown)]
		Неизвестный_статус_двери,

		[EventDescription(JournalSubsystemType.SKD, "Запрос пароля", XStateClass.TechnologicalRegime)]
		Запрос_пароля,

		[EventDescription(JournalSubsystemType.SKD, "Установка пароля", XStateClass.TechnologicalRegime)]
		Установка_пароля,

		[EventDescription(JournalSubsystemType.SKD, "Сброс Контроллера", XStateClass.TechnologicalRegime)]
		Сброс_Контроллера,

		[EventDescription(JournalSubsystemType.SKD, "Перезагрузка Контроллера", XStateClass.TechnologicalRegime)]
		Перезагрузка_Контроллера,

		[EventDescription(JournalSubsystemType.SKD, "Запись графиков работы", XStateClass.TechnologicalRegime)]
		Запись_графиков_работы,

		[EventDescription(JournalSubsystemType.SKD, "Обновление ПО Контроллера", XStateClass.TechnologicalRegime)]
		Обновление_ПО_Контроллера,

		[EventDescription(JournalSubsystemType.SKD, "Запрос конфигурации двери", XStateClass.TechnologicalRegime)]
		Запрос_конфигурации_двери,

		[EventDescription(JournalSubsystemType.SKD, "Запись конфигурации двери", XStateClass.TechnologicalRegime)]
		Запись_конфигурации_двери,

		[EventDescription(JournalSubsystemType.SKD, "Команда на открытие двери", XStateClass.On)]
		Команда_на_открытие_двери,

		[EventDescription(JournalSubsystemType.SKD, "Команда на закрытие двери", XStateClass.Off)]
		Команда_на_закрытие_двери,

		[EventDescription(JournalSubsystemType.SKD, "Команда на открытие зоны", XStateClass.On)]
		Команда_на_открытие_зоны,

		[EventDescription(JournalSubsystemType.SKD, "Команда на закрытие зоны", XStateClass.Off)]
		Команда_на_закрытие_зоны,

		[EventDescription(JournalSubsystemType.SKD, "Команда на открытие точки доступа", XStateClass.On)]
		Команда_на_открытие_точки_доступа,

		[EventDescription(JournalSubsystemType.SKD, "Команда на закрытие точки доступа", XStateClass.Off)]
		Команда_на_закрытие_точки_доступа,

		[EventDescription(JournalSubsystemType.SKD, "Добавление карты", XStateClass.Info)]
		Добавление_карты,

		[EventDescription(JournalSubsystemType.SKD, "Редактирование карты", XStateClass.Info)]
		Редактирование_карты,

		[EventDescription(JournalSubsystemType.SKD, "Удаление карты", XStateClass.Info)]
		Удаление_карты,
	}
}