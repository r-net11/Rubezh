using FiresecAPI.GK;

namespace FiresecAPI.Journal
{
	public enum JournalEventNameType
	{
		[EventName(JournalSubsystemType.System, "NULL", XStateClass.No)]
		NULL,

		[EventName(JournalSubsystemType.System, "Сброс ограничения на повторный проход", XStateClass.Info)]
		Сброс_антипессбэка_для_выбранной_ТД,

		[EventName(JournalSubsystemType.System, "Сброс ограничения на повторный проход для ВСЕХ пропусков", XStateClass.Info)]
		Сброс_антипессбэка_для_всех_пропусков,

		[EventName(JournalSubsystemType.SKD, "Добавление интервала", XStateClass.Info)]
		Добавление_интервала,

		[EventName(JournalSubsystemType.SKD, "Изменение границ интервала", XStateClass.Info)]
		Изменение_границы_интервала,

		[EventName(JournalSubsystemType.SKD, "Удаление интервала", XStateClass.Info)]
		Удаление_интервала,

		[EventName(JournalSubsystemType.SKD, "Принудительное закрытие интервала", XStateClass.Info)]
		Закрытие_интервала,

		[EventName(JournalSubsystemType.SKD, "Установка признака \"Не учитывать в расчетах\"", XStateClass.Info)]
		Установка_неУчитывать_в_расчетах,

		[EventName(JournalSubsystemType.SKD, "Снятие признака \"Не учитывать в расчетах\"", XStateClass.Info)]
		Снятие_неУчитывать_в_расчетах,

		[EventName(JournalSubsystemType.System, "Неизвестное событие", XStateClass.No)]
		Неизвестное_событие,

		[EventName(JournalSubsystemType.System, "Вход пользователя в систему", XStateClass.Info)]
		Вход_пользователя_в_систему,

		[EventName(JournalSubsystemType.System, "Выход пользователя из системы", XStateClass.Info)]
		Выход_пользователя_из_системы,

		[EventName(JournalSubsystemType.System, "Дежурство сдал", XStateClass.Info)]
		Дежурство_сдал,

		[EventName(JournalSubsystemType.System, "Дежурство принял", XStateClass.Info)]
		Дежурство_принял,

		[EventName(JournalSubsystemType.System, "Отсутствует лицензия", XStateClass.HasNoLicense)]
		Отсутствует_лицензия,

		[EventName(JournalSubsystemType.System, "Лицензия обнаружена", XStateClass.HasNoLicense)]
		Лицензия_обнаружена,

		[EventName(JournalSubsystemType.System, "Отмена операции", XStateClass.TechnologicalRegime)]
		Отмена_операции,

		[EventName(JournalSubsystemType.System, "Сообщение автоматизации", XStateClass.Info)]
		Сообщение_автоматизации,

		[EventName(JournalSubsystemType.System, "Применение конфигурации", XStateClass.Info)]
		Применение_конфигурации,

		[EventName(JournalSubsystemType.SKD, "Потеря связи", XStateClass.ConnectionLost)]
		Потеря_связи,

		[EventName(JournalSubsystemType.SKD, "Восстановление связи", XStateClass.ConnectionLost)]
		Восстановление_связи,

		[EventName(JournalSubsystemType.SKD, "Проход разрешен", XStateClass.Info)]
		Проход_разрешен,

		[EventName(JournalSubsystemType.SKD, "Проход запрещен", XStateClass.Attention)]
		Проход_запрещен,

		[EventName(JournalSubsystemType.SKD, "Дверь не закрыта", XStateClass.Attention)]
		Дверь_не_закрыта,

		[EventName(JournalSubsystemType.SKD, "Взлом", XStateClass.Attention)]
		Взлом,

		[EventName(JournalSubsystemType.SKD, "Повторный проход", XStateClass.Attention)]
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

		[EventName(JournalSubsystemType.SKD, "Запись графиков доступа", XStateClass.TechnologicalRegime)]
		Запись_графиков_доступа,

		[EventName(JournalSubsystemType.SKD, "Запись пропусков", XStateClass.TechnologicalRegime)]
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

		[EventName(JournalSubsystemType.SKD, "Команда \"Открыть\" замок удаленно", XStateClass.On)]
		Команда_на_открытие_двери,

		[EventName(JournalSubsystemType.SKD, "Команда \"Закрыть\" замок удаленно", XStateClass.Off)]
		Команда_на_закрытие_двери,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод замка в режим \"Открыто\"", XStateClass.On)]
		Команда_на_перевод_двери_в_режим_Открыто,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод замка в режим \"Закрыто\"", XStateClass.Off)]
		Команда_на_перевод_двери_в_режим_Закрыто,

		[EventName(JournalSubsystemType.SKD, "Команда \"Открыть\" зону удаленно", XStateClass.On)]
		Команда_на_открытие_зоны,

		[EventName(JournalSubsystemType.SKD, "Команда \"Закрыть\" зону удаленно", XStateClass.Off)]
		Команда_на_закрытие_зоны,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод зоны в режим \"Открыто\"", XStateClass.On)]
		Команда_на_перевод_зоны_в_режим_Открыто,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод зоны в режим \"Закрыто\"", XStateClass.Off)]
		Команда_на_перевод_зоны_в_режим_Закрыто,

		[EventName(JournalSubsystemType.SKD, "Команда \"Открыть\" точку доступа удаленно", XStateClass.On)]
		Команда_на_открытие_точки_доступа,

		[EventName(JournalSubsystemType.SKD, "Команда \"Закрыть\" точку доступа удаленно", XStateClass.Off)]
		Команда_на_закрытие_точки_доступа,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод точки доступа в режим \"Открыто\"", XStateClass.On)]
		Команда_на_перевод_точки_доступа_в_режим_Открыто,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод точки доступа в режим \"Закрыто\"", XStateClass.Off)]
		Команда_на_перевод_точки_доступа_в_режим_Закрыто,

		[EventName(JournalSubsystemType.SKD, "Добавление карты", XStateClass.Info)]
		Добавление_карты,

		[EventName(JournalSubsystemType.SKD, "Редактирование карты", XStateClass.Info)]
		Редактирование_карты,

		[EventName(JournalSubsystemType.SKD, "Удаление карты", XStateClass.Info)]
		Удаление_карты,

		[EventName(JournalSubsystemType.SKD, "Редактирование сотрудника", XStateClass.Info)]
		Редактирование_сотрудника,

		[EventName(JournalSubsystemType.SKD, "Редактирование подразделения", XStateClass.Info)]
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

		[EventName(JournalSubsystemType.SKD, "Редактирование сокращённого дня", XStateClass.Info)]
		Редактирование_праздничного_дня,

		[EventName(JournalSubsystemType.SKD, "Редактирование шаблона пропуска", XStateClass.Info)]
		Редактирование_шаблона_пропуска,

		[EventName(JournalSubsystemType.SKD, "Добавление нового сотрудника", XStateClass.Info)]
		Добавление_нового_сотрудника,

		[EventName(JournalSubsystemType.SKD, "Добавление нового подразделения", XStateClass.Info)]
		Добавление_нового_отдела,

		[EventName(JournalSubsystemType.SKD, "Добавление новой должности", XStateClass.Info)]
		Добавление_новой_должности,

		[EventName(JournalSubsystemType.SKD, "Добавление нового шаблона доступа", XStateClass.Info)]
		Добавление_нового_шаблона_доступа,

		[EventName(JournalSubsystemType.SKD, "Добавление новой организации", XStateClass.Info)]
		Добавление_новой_организации,

		[EventName(JournalSubsystemType.SKD, "Добавление новой дополнительной колонки", XStateClass.Info)]
		Добавление_новой_дополнительной_колонки,

		[EventName(JournalSubsystemType.SKD, "Добавление нового дневного графика", XStateClass.Info)]
		Добавление_нового_дневного_графика,

		[EventName(JournalSubsystemType.SKD, "Добавление нового графика работы", XStateClass.Info)]
		Добавление_нового_графика_работы,

		[EventName(JournalSubsystemType.SKD, "Добавление нового графика работы сотрудника", XStateClass.Info)]
		Добавление_нового_графика_работы_сотрудника,

		[EventName(JournalSubsystemType.SKD, "Добавление нового сокращённого дня", XStateClass.Info)]
		Добавление_нового_праздничного_дня,

		[EventName(JournalSubsystemType.SKD, "Добавление нового шаблона пропуска", XStateClass.Info)]
		Добавление_нового_шаблона_пропуска,

		[EventName(JournalSubsystemType.SKD, "Внесение оправдательного документа", XStateClass.Info)]
		Внесение_оправдательного_документа,

		[EventName(JournalSubsystemType.SKD, "Удаление сотрудника", XStateClass.Info)]
		Удаление_сотрудника,

		[EventName(JournalSubsystemType.SKD, "Удаление подразделения", XStateClass.Info)]
		Удаление_отдела,

		[EventName(JournalSubsystemType.SKD, "Удаление должности", XStateClass.Info)]
		Удаление_должности,

		[EventName(JournalSubsystemType.SKD, "Удаление шаблона доступа", XStateClass.Info)]
		Удаление_шаблона_доступа,

		[EventName(JournalSubsystemType.SKD, "Удаление организации", XStateClass.Info)]
		Удаление_организации,

		[EventName(JournalSubsystemType.SKD, "Удаление дополнительной колонки", XStateClass.Info)]
		Удаление_дополнительной_колонки,

		[EventName(JournalSubsystemType.SKD, "Удаление дневного графика", XStateClass.Info)]
		Удаление_дневного_графика,

		[EventName(JournalSubsystemType.SKD, "Удаление графика работы", XStateClass.Info)]
		Удаление_графика_работы,

		[EventName(JournalSubsystemType.SKD, "Удаление графика работы сотрудника", XStateClass.Info)]
		Удаление_графика_работы_сотрудника,

		[EventName(JournalSubsystemType.SKD, "Удаление сокращённого дня", XStateClass.Info)]
		Удаление_праздничного_дня,

		[EventName(JournalSubsystemType.SKD, "Удаление шаблона пропуска", XStateClass.Info)]
		Удаление_шаблона_пропуска,

		[EventName(JournalSubsystemType.SKD, "Восстановление сотрудника", XStateClass.Info)]
		Восстановление_сотрудника,

		[EventName(JournalSubsystemType.SKD, "Восстановление подразделения", XStateClass.Info)]
		Восстановление_отдела,

		[EventName(JournalSubsystemType.SKD, "Восстановление должности", XStateClass.Info)]
		Восстановление_должности,

		[EventName(JournalSubsystemType.SKD, "Восстановление шаблона доступа", XStateClass.Info)]
		Восстановление_шаблона_доступа,

		[EventName(JournalSubsystemType.SKD, "Восстановление организации", XStateClass.Info)]
		Восстановление_организации,

		[EventName(JournalSubsystemType.SKD, "Восстановление дополнительной колонки", XStateClass.Info)]
		Восстановление_дополнительной_колонки,

		[EventName(JournalSubsystemType.SKD, "Восстановление дневного графика", XStateClass.Info)]
		Восстановление_дневного_графика,

		[EventName(JournalSubsystemType.SKD, "Восстановление графика работы", XStateClass.Info)]
		Восстановление_графика_работы,

		[EventName(JournalSubsystemType.SKD, "Восстановление графика работы сотрудника", XStateClass.Info)]
		Восстановление_графика_работы_сотрудника,

		[EventName(JournalSubsystemType.SKD, "Восстановление сокращённого дня", XStateClass.Info)]
		Восстановление_праздничного_дня,

		[EventName(JournalSubsystemType.SKD, "Восстановление шаблона пропуска", XStateClass.Info)]
		Восстановление_шаблона_пропуска,

		[EventName(JournalSubsystemType.Video, "Перевод в предустановку", XStateClass.On)]
		Перевод_в_предустановку,

		[EventName(JournalSubsystemType.SKD, "Открытие замка", XStateClass.On)]
		Открытие_замка_двери,

		[EventName(JournalSubsystemType.SKD, "Закрытие замка", XStateClass.Off)]
		Закрытие_замка_двери,

		[EventName(JournalSubsystemType.SKD, "Неизвестный статус замка двери", XStateClass.Unknown)]
		Неизвестный_статус_замка_двери,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод точки доступа в режим \"Норма\"", XStateClass.Norm)]
		Команда_на_перевод_точки_доступа_в_режим_Норма,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод зоны в режим \"Норма\"", XStateClass.Norm)]
		Команда_на_перевод_зоны_в_режим_Норма,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод замка в режим \"Норма\"", XStateClass.Norm)]
		Команда_на_перевод_двери_в_режим_Норма,

		[EventName(JournalSubsystemType.SKD, "Запрос настроек запрета повторного прохода", XStateClass.TechnologicalRegime)]
		Запрос_antipassback_настройки_контроллера,

		[EventName(JournalSubsystemType.SKD, "Запись настроек запрета повторного прохода", XStateClass.TechnologicalRegime)]
		Запись_antipassback_настройки_контроллера,

		[EventName(JournalSubsystemType.SKD, "Запрос настроек блокировки одновременного прохода", XStateClass.TechnologicalRegime)]
		Запрос_interlock_настройки_контроллера,

		[EventName(JournalSubsystemType.SKD, "Запись настроек блокировки одновременного прохода", XStateClass.TechnologicalRegime)]
		Запись_interlock_настройки_контроллера,

		[EventName(JournalSubsystemType.SKD, "Подтверждение тревоги", XStateClass.Fire1)]
		Подтверждение_тревоги,

		[EventName(JournalSubsystemType.SKD, "Команда на сброс состояния замка \"Взлом\"", XStateClass.Info)]
		Сброс_состояния_взлом_замка,

		[EventName(JournalSubsystemType.SKD, "Команда на сброс состояния точки доступа \"Взлом\"", XStateClass.Info)]
		Сброс_состояния_взлом_точки_доступа,

		[EventName(JournalSubsystemType.SKD, "Команда на сброс состояния зоны \"Взлом\"", XStateClass.Info)]
		Сброс_состояния_взлом_зоны,

		[EventName(JournalSubsystemType.SKD, "Запрос паролей замков", XStateClass.TechnologicalRegime)]
		GetControllerLocksPasswords,

		[EventName(JournalSubsystemType.SKD, "Запись паролей замков", XStateClass.TechnologicalRegime)]
		SetControllerLocksPasswords,
	}
}