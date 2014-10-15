using System.ComponentModel;
using FiresecAPI.GK;

namespace FiresecAPI.Journal
{
	public enum JournalEventNameType
	{
		[DescriptionAttribute("")]
		NULL,

		[EventName(JournalSubsystemType.System, "Неизвестное событие", XStateClass.No)]
		Неизвестное_событие,

		[EventName(JournalSubsystemType.System, "Подтверждение тревоги", XStateClass.Fire1)]
		Подтверждение_тревоги,

		[EventName(JournalSubsystemType.System, "Вход пользователя в систему", XStateClass.Info)]
		Вход_пользователя_в_систему,

		[EventName(JournalSubsystemType.System, "Выход пользователя из системы", XStateClass.Info)]
		Выход_пользователя_из_системы,

		[EventName(JournalSubsystemType.System, "Дежурство сдал", XStateClass.Info)]
		Дежурство_сдал,

		[EventName(JournalSubsystemType.System, "Дежурство принял", XStateClass.Info)]
		Дежурство_принял,

		[EventName(JournalSubsystemType.System, "Зависание процесса отпроса", XStateClass.Unknown)]
		Зависание_процесса_отпроса,

		[EventName(JournalSubsystemType.System, "Отсутствует лицензия", XStateClass.HasNoLicense)]
		Отсутствует_лицензия,

		[EventName(JournalSubsystemType.System, "Лицензия обнаружена", XStateClass.HasNoLicense)]
		Лицензия_обнаружена,

		[EventName(JournalSubsystemType.System, "Ошибка инициализации мониторинга", XStateClass.Unknown)]
		Ошибка_инициализации_мониторинга,

		[EventName(JournalSubsystemType.System, "Отмена операции", XStateClass.TechnologicalRegime)]
		Отмена_операции,

		[EventName(JournalSubsystemType.System, "Сообщение автоматизации", XStateClass.Info)]
		Сообщение_автоматизации,

		[EventName(JournalSubsystemType.GK, "Обновление ПО прибора", XStateClass.Info)]
		Обновление_ПО_прибора,

		[EventName(JournalSubsystemType.GK, "Запись конфигурации в прибор", XStateClass.Info)]
		Запись_конфигурации_в_прибор,

		[EventName(JournalSubsystemType.GK, "Чтение конфигурации из прибора", XStateClass.Info)]
		Чтение_конфигурации_из_прибора,

		[EventName(JournalSubsystemType.GK, "Запрос информации об устройстве", XStateClass.TechnologicalRegime)]
		Запрос_информации_об_устройстве,

		[EventName(JournalSubsystemType.GK, "Синхронизация времени", XStateClass.Info)]
		Синхронизация_времени,

		[EventName(JournalSubsystemType.GK, "Команда оператора", XStateClass.Info)]
		Команда_оператора,

		[EventName(JournalSubsystemType.GK, "Ошибка при выполнении команды", XStateClass.Failure)]
		Ошибка_при_выполнении_команды,

		[EventName(JournalSubsystemType.GK, "Ошибка при выполнении команды над устройством", XStateClass.Failure)]
		Ошибка_при_выполнении_команды_над_устройством,

		[EventName(JournalSubsystemType.GK, "Нет связи с ГК", XStateClass.ConnectionLost)]
		Нет_связи_с_ГК,

		[EventName(JournalSubsystemType.GK, "Связь с ГК восстановлена", XStateClass.ConnectionLost)]
		Связь_с_ГК_восстановлена,

		[EventName(JournalSubsystemType.GK, "Конфигурация прибора не соответствует конфигурации ПК", XStateClass.Unknown)]
		Конфигурация_прибора_не_соответствует_конфигурации_ПК,

		[EventName(JournalSubsystemType.GK, "Конфигурация прибора соответствует конфигурации ПК", XStateClass.Unknown)]
		Конфигурация_прибора_соответствует_конфигурации_ПК,

		[EventName(JournalSubsystemType.GK, "Ошибка при синхронизации журнала", XStateClass.Unknown)]
		Ошибка_при_синхронизации_журнала,

		[EventName(JournalSubsystemType.GK, "Ошибка при опросе состояний компонентов ГК", XStateClass.Unknown)]
		Ошибка_при_опросе_состояний_компонентов_ГК,

		[EventName(JournalSubsystemType.GK, "Устранена ошибка при опросе состояний компонентов ГК", XStateClass.Unknown)]
		Устранена_ошибка_при_опросе_состояний_компонентов_ГК,

		[EventName(JournalSubsystemType.GK, "Восстановление связи с прибором", XStateClass.ConnectionLost)]
		Восстановление_связи_с_прибором,

		[EventName(JournalSubsystemType.GK, "Потеря связи с прибором", XStateClass.ConnectionLost)]
		Потеря_связи_с_прибором,

		[EventName(JournalSubsystemType.System, "База данных прибора не соответствует базе данных ПК", XStateClass.Unknown)]
		База_данных_прибора_не_соответствует_базе_данных_ПК,

		[EventName(JournalSubsystemType.GK, "База данных прибора соответствует базе данных ПК", XStateClass.Unknown)]
		База_данных_прибора_соответствует_базе_данных_ПК,

		[EventName(JournalSubsystemType.GK, "Применение конфигурации", XStateClass.Info)]
		Применение_конфигурации,

		[EventName(JournalSubsystemType.GK, "ГК в технологическом режиме", XStateClass.TechnologicalRegime)]
		ГК_в_технологическом_режиме,

		[EventName(JournalSubsystemType.GK, "ГК в рабочем режиме", XStateClass.Info)]
		ГК_в_рабочем_режиме,

		[EventName(JournalSubsystemType.GK, "Запись всех идентификаторов", XStateClass.Info)]
		Запись_всех_идентификаторов,

		[EventName(JournalSubsystemType.GK, "Перевод в технологический режим", XStateClass.TechnologicalRegime)]
		Перевод_в_технологический_режим,

		[EventName(JournalSubsystemType.GK, "Синхронизация времени прибора с временем ПК", XStateClass.Info)]
		Синхронизация_времени_прибора_с_временем_ПК,

		[EventName(JournalSubsystemType.GK, "Смена ПО", XStateClass.TechnologicalRegime)]
		Смена_ПО,

		[EventName(JournalSubsystemType.GK, "Смена БД", XStateClass.TechnologicalRegime)]
		Смена_БД,

		[EventName(JournalSubsystemType.GK, "Перевод в рабочий режим", XStateClass.Info)]
		Перевод_в_рабочий_режим,

		[EventName(JournalSubsystemType.GK, "Вход пользователя в прибор", XStateClass.Info)]
		Вход_пользователя_в_прибор,

		[EventName(JournalSubsystemType.GK, "Выход пользователя из прибора", XStateClass.Info)]
		Выход_пользователя_из_прибора,

		[EventName(JournalSubsystemType.GK, "Ошибка управления", XStateClass.Failure)]
		Ошибка_управления,

		[EventName(JournalSubsystemType.GK, "Введен новый пользователь", XStateClass.Info)]
		Введен_новый_пользователь,

		[EventName(JournalSubsystemType.GK, "Изменена учетная информация пользователя", XStateClass.Info)]
		Изменена_учетная_информация_пользователя,

		[EventName(JournalSubsystemType.GK, "Произведена настройка сети", XStateClass.Info)]
		Произведена_настройка_сети,

		[EventName(JournalSubsystemType.GK, "Неизвестный код события контроллекра", XStateClass.Unknown)]
		Неизвестный_код_события_контроллекра,

		[EventName(JournalSubsystemType.GK, "Неизвестный тип", XStateClass.Unknown)]
		Неизвестный_тип,

		[EventName(JournalSubsystemType.GK, "Устройство с таким адресом не описано при конфигурации", XStateClass.Unknown)]
		Устройство_с_таким_адресом_не_описано_при_конфигурации,

		[EventName(JournalSubsystemType.GK, "Неизвестный код события устройства", XStateClass.Unknown)]
		Неизвестный_код_события_устройства,

		[EventName(JournalSubsystemType.GK, "При конфигурации описан другой тип", XStateClass.Unknown)]
		При_конфигурации_описан_другой_тип,

		[EventName(JournalSubsystemType.GK, "Изменился заводской номер", XStateClass.Info)]
		Изменился_заводской_номер,

		[EventName(JournalSubsystemType.GK, "Пожар-1", XStateClass.Fire1)]
		Пожар_1,

		[EventName(JournalSubsystemType.GK, "Сработка-1", XStateClass.Fire1)]
		Сработка_1,

		[EventName(JournalSubsystemType.GK, "Сработка охранной зоны", XStateClass.Fire1)]
		Сработка_Охранной_Зоны,

		[EventName(JournalSubsystemType.GK, "Пожар-2", XStateClass.Fire2)]
		Пожар_2,

		[EventName(JournalSubsystemType.GK, "Сработка-2", XStateClass.Fire2)]
		Сработка_2,

		[EventName(JournalSubsystemType.GK, "Внимание", XStateClass.Attention)]
		Внимание,

		[EventName(JournalSubsystemType.GK, "Неисправность", XStateClass.Failure)]
		Неисправность,

		[EventName(JournalSubsystemType.GK, "Неисправность устранена", XStateClass.Norm)]
		Неисправность_устранена,

		[EventName(JournalSubsystemType.GK, "Тест", XStateClass.Test)]
		Тест,

		[EventName(JournalSubsystemType.GK, "Тест устранен", XStateClass.Test)]
		Тест_устранен,

		[EventName(JournalSubsystemType.GK, "Запыленность", XStateClass.Service)]
		Запыленность,

		[EventName(JournalSubsystemType.GK, "Запыленность устранена", XStateClass.Service)]
		Запыленность_устранена,

		[EventName(JournalSubsystemType.GK, "Информация", XStateClass.Info)]
		Информация,

		[EventName(JournalSubsystemType.GK, "Отсчет задержки", XStateClass.Info)]
		Отсчет_задержки,

		[EventName(JournalSubsystemType.GK, "Включено", XStateClass.On)]
		Включено,

		[EventName(JournalSubsystemType.GK, "Выключено", XStateClass.Off)]
		Выключено,

		[EventName(JournalSubsystemType.GK, "Включается", XStateClass.TurningOn)]
		Включается,

		[EventName(JournalSubsystemType.GK, "Выключается", XStateClass.TurningOff)]
		Выключается,

		[EventName(JournalSubsystemType.GK, "Кнопка", XStateClass.Info)]
		Кнопка,

		[EventName(JournalSubsystemType.GK, "Изменение автоматики по неисправности", XStateClass.AutoOff)]
		Изменение_автоматики_по_неисправности,

		[EventName(JournalSubsystemType.GK, "Изменение автоматики по кнопке СТОП", XStateClass.AutoOff)]
		Изменение_автоматики_по_кнопке_СТОП,

		[EventName(JournalSubsystemType.GK, "Изменение автоматики по датчику ДВЕРИ-ОКНА", XStateClass.AutoOff)]
		Изменение_автоматики_по_датчику_ДВЕРИ_ОКНА,

		[EventName(JournalSubsystemType.GK, "Изменение автоматики по ТМ", XStateClass.AutoOff)]
		Изменение_автоматики_по_ТМ,

		[EventName(JournalSubsystemType.GK, "Автоматика включена", XStateClass.AutoOff)]
		Автоматика_включена,

		[EventName(JournalSubsystemType.GK, "Ручной пуск АУП от ИПР", XStateClass.On)]
		Ручной_пуск_АУП_от_ИПР,

		[EventName(JournalSubsystemType.GK, "Отложенный пуск АУП по датчику ДВЕРИ-ОКНА", XStateClass.On)]
		Отложенный_пуск_АУП_по_датчику_ДВЕРИ_ОКНА,

		[EventName(JournalSubsystemType.GK, "Пуск АУП завершен", XStateClass.On)]
		Пуск_АУП_завершен,

		[EventName(JournalSubsystemType.GK, "Останов тушения по кнопке СТОП", XStateClass.Off)]
		Останов_тушения_по_кнопке_СТОП,

		[EventName(JournalSubsystemType.GK, "Программирование мастер-ключа", XStateClass.Info)]
		Программирование_мастер_ключа,

		[EventName(JournalSubsystemType.GK, "Отсчет удержания", XStateClass.Info)]
		Отсчет_удержания,

		[EventName(JournalSubsystemType.GK, "Уровень высокий", XStateClass.Info)]
		Уровень_высокий,

		[EventName(JournalSubsystemType.GK, "Уровень низкий", XStateClass.Info)]
		Уровень_низкий,

		[EventName(JournalSubsystemType.GK, "Ход по команде с УЗЗ", XStateClass.On)]
		Ход_по_команде_с_УЗЗ,

		[EventName(JournalSubsystemType.GK, "У ДУ сообщение ПУСК НЕВОЗМОЖЕН", XStateClass.Failure)]
		У_ДУ_сообщение_ПУСК_НЕВОЗМОЖЕН,

		[EventName(JournalSubsystemType.GK, "Авария пневмоемкости", XStateClass.Failure)]
		Авария_пневмоемкости,

		[EventName(JournalSubsystemType.GK, "Уровень аварийный", XStateClass.Failure)]
		Уровень_аварийный,

		[EventName(JournalSubsystemType.GK, "Запрет пуска НС", XStateClass.Off)]
		Запрет_пуска_НС,

		[EventName(JournalSubsystemType.GK, "Запрет пуска компрессора", XStateClass.Off)]
		Запрет_пуска_компрессора,

		[EventName(JournalSubsystemType.GK, "Команда с УЗН", XStateClass.Info)]
		Команда_с_УЗН,

		[EventName(JournalSubsystemType.GK, "Перевод в режим ручного управления", XStateClass.AutoOff)]
		Перевод_в_режим_ручного_управления,

		[EventName(JournalSubsystemType.GK, "Состояние не определено", XStateClass.Unknown)]
		Состояние_не_определено,

		[EventName(JournalSubsystemType.GK, "Остановлено", XStateClass.Off)]
		Остановлено,

		[EventName(JournalSubsystemType.GK, "Состояние Неизвестно", XStateClass.Unknown)]
		Состояние_Неизвестно,

		[EventName(JournalSubsystemType.GK, "Перевод в автоматический режим", XStateClass.Norm)]
		Перевод_в_автоматический_режим,

		[EventName(JournalSubsystemType.GK, "Перевод в ручной режим", XStateClass.AutoOff)]
		Перевод_в_ручной_режим,

		[EventName(JournalSubsystemType.GK, "Перевод в отключенный режим", XStateClass.Ignore)]
		Перевод_в_отключенный_режим,

		[EventName(JournalSubsystemType.GK, "Перевод в неопределенный режим", XStateClass.Unknown)]
		Перевод_в_неопределенный_режим,

		[EventName(JournalSubsystemType.GK, "Запись параметра", XStateClass.Info)]
		Запись_параметра,

		[EventName(JournalSubsystemType.GK, "Норма", XStateClass.Norm)]
		Норма,

		[EventName(JournalSubsystemType.GK, "Неизвестный код события объекта", XStateClass.Unknown)]
		Неизвестный_код_события_объекта,

		[EventName(JournalSubsystemType.SKD, "Потеря связи", XStateClass.ConnectionLost)]
		Потеря_связи,

		[EventName(JournalSubsystemType.SKD, "Восстановление связи", XStateClass.ConnectionLost)]
		Восстановление_связи,

		[EventName(JournalSubsystemType.SKD, "Проход разрешен", XStateClass.Info)]
		Проход_разрешен,

		[EventName(JournalSubsystemType.SKD, "Проход запрещен", XStateClass.Attention)]
		Проход_запрещен,

		[EventName(JournalSubsystemType.SKD, "Дверь не закрыта", XStateClass.Failure)]
		Дверь_не_закрыта,

		[EventName(JournalSubsystemType.SKD, "Взлом", XStateClass.Attention)]
		Взлом,

		[EventName(JournalSubsystemType.SKD, "Повторный_проход", XStateClass.Attention)]
		Повторный_проход,

		[EventName(JournalSubsystemType.SKD, "Принуждение", XStateClass.Fire1)]
		Принуждение,

		[EventName(JournalSubsystemType.SKD, "Открытие двери", XStateClass.On)]
		Открытие_двери,

		[EventName(JournalSubsystemType.SKD, "Закрытие двери", XStateClass.Off)]
		Закрытие_двери,

		[EventName(JournalSubsystemType.SKD, "Неизвестный статус двери", XStateClass.Unknown)]
		Неизвестный_статус_двери,

		[EventName(JournalSubsystemType.SKD, "Вскрытие контроллера", XStateClass.Attention)]
		Вскрытие_контроллера,

		[EventName(JournalSubsystemType.SKD, "Множественный проход", XStateClass.Attention)]
		Множественный_проход,

		[EventName(JournalSubsystemType.SKD, "Проход по отпечатку пальца", XStateClass.Info)]
		Проход_по_отпечатку_пальца,

		[EventName(JournalSubsystemType.SKD, "Местная тревога", XStateClass.Attention)]
		Местная_тревога,

		[EventName(JournalSubsystemType.SKD, "Сброс Контроллера", XStateClass.TechnologicalRegime)]
		Сброс_Контроллера,

		[EventName(JournalSubsystemType.SKD, "Перезагрузка Контроллера", XStateClass.TechnologicalRegime)]
		Перезагрузка_Контроллера,

		[EventName(JournalSubsystemType.SKD, "Запись графиков работы", XStateClass.TechnologicalRegime)]
		Запись_графиков_работы,

		[EventName(JournalSubsystemType.SKD, "Перезапись всех карт", XStateClass.TechnologicalRegime)]
		Перезапись_всех_карт,

		[EventName(JournalSubsystemType.SKD, "Обновление ПО Контроллера", XStateClass.TechnologicalRegime)]
		Обновление_ПО_Контроллера,

		[EventName(JournalSubsystemType.SKD, "Запрос конфигурации контроллера", XStateClass.TechnologicalRegime)]
		Запрос_конфигурации_контроллера,

		[EventName(JournalSubsystemType.SKD, "Синхронизация времени контроллера", XStateClass.Info)]
		Синхронизация_времени_контроллера,

		[EventName(JournalSubsystemType.SKD, "Запрос конфигурации двери", XStateClass.TechnologicalRegime)]
		Запрос_конфигурации_двери,

		[EventName(JournalSubsystemType.SKD, "Запись конфигурации двери", XStateClass.TechnologicalRegime)]
		Запись_конфигурации_двери,

		[EventName(JournalSubsystemType.SKD, "Запрос направления контроллера", XStateClass.TechnologicalRegime)]
		Запрос_направления_контроллера,

		[EventName(JournalSubsystemType.SKD, "Запись направления контроллера", XStateClass.TechnologicalRegime)]
		Запись_направления_контроллера,

		[EventName(JournalSubsystemType.SKD, "Запись пароля контроллера", XStateClass.TechnologicalRegime)]
		Запись_пароля_контроллера,

		[EventName(JournalSubsystemType.SKD, "Запрос временных настроек контроллера", XStateClass.TechnologicalRegime)]
		Запрос_временных_настроек_контроллера,

		[EventName(JournalSubsystemType.SKD, "Запись временных настроек контроллера", XStateClass.TechnologicalRegime)]
		Запись_временных_настроек_контроллера,

		[EventName(JournalSubsystemType.SKD, "Запрос сетевых настроек контроллера", XStateClass.TechnologicalRegime)]
		Запрос_сетевых_настроек_контроллера,

		[EventName(JournalSubsystemType.SKD, "Запись сетевых настроек контроллера", XStateClass.TechnologicalRegime)]
		Запись_сетевых_настроек_контроллера,

		[EventName(JournalSubsystemType.SKD, "Команда на открытие двери", XStateClass.On)]
		Команда_на_открытие_двери,

		[EventName(JournalSubsystemType.SKD, "Команда на закрытие двери", XStateClass.Off)]
		Команда_на_закрытие_двери,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод двери в режим Открыто", XStateClass.On)]
		Команда_на_перевод_двери_в_режим_Открыто,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод двери в режим Закрыто", XStateClass.Off)]
		Команда_на_перевод_двери_в_режим_Закрыто,

		[EventName(JournalSubsystemType.SKD, "Команда на открытие зоны", XStateClass.On)]
		Команда_на_открытие_зоны,

		[EventName(JournalSubsystemType.SKD, "Команда на закрытие зоны", XStateClass.Off)]
		Команда_на_закрытие_зоны,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод зоны в режим Открыто", XStateClass.On)]
		Команда_на_перевод_зоны_в_режим_Открыто,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод зоны в режим Закрыто", XStateClass.Off)]
		Команда_на_перевод_зоны_в_режим_Закрыто,

		[EventName(JournalSubsystemType.SKD, "Команда на открытие точки доступа", XStateClass.On)]
		Команда_на_открытие_точки_доступа,

		[EventName(JournalSubsystemType.SKD, "Команда на закрытие точки доступа", XStateClass.Off)]
		Команда_на_закрытие_точки_доступа,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод точки доступа в режим Открыто", XStateClass.On)]
		Команда_на_перевод_точки_доступа_в_режим_Открыто,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод точки доступа в режим Закрыто", XStateClass.Off)]
		Команда_на_перевод_точки_доступа_в_режим_Закрыто,

		[EventName(JournalSubsystemType.SKD, "Добавление карты", XStateClass.Info)]
		Добавление_карты,

		[EventName(JournalSubsystemType.SKD, "Редактирование карты", XStateClass.Info)]
		Редактирование_карты,

		[EventName(JournalSubsystemType.SKD, "Удаление карты", XStateClass.Info)]
		Удаление_карты,

		[EventName(JournalSubsystemType.SKD, "Редактирование сотрудника", XStateClass.Info)]
		Редактирование_сотрудника,

		[EventName(JournalSubsystemType.SKD, "Редактирование отдела", XStateClass.Info)]
		Редактирование_отдела,

		[EventName(JournalSubsystemType.SKD, "Редактирование должности", XStateClass.Info)]
		Редактирование_должности,

		[EventName(JournalSubsystemType.SKD, "Редактирование шаблона доступа", XStateClass.Info)]
		Редактирование_шаблона_доступа,

		[EventName(JournalSubsystemType.SKD, "Редактирование организации", XStateClass.Info)]
		Редактирование_организации,

		[EventName(JournalSubsystemType.SKD, "Редактирование дополнительной колонки", XStateClass.Info)]
		Редактирование_дополнительной_колонки,

		[EventName(JournalSubsystemType.SKD, "Редактирование дневного графика", XStateClass.Info)]
		Редактирование_дневного_графика,

		[EventName(JournalSubsystemType.SKD, "Редактирование графика работы", XStateClass.Info)]
		Редактирование_графика_работы,

		[EventName(JournalSubsystemType.SKD, "Редактирование графика работы сотрудника", XStateClass.Info)]
		Редактирование_графика_работы_сотрудника,

		[EventName(JournalSubsystemType.SKD, "Редактирование праздничного дня", XStateClass.Info)]
		Редактирование_праздничного_дня,

		[EventName(JournalSubsystemType.SKD, "Внесение оправдательного документа", XStateClass.Info)]
		Внесение_оправдательного_документа,
	}
}