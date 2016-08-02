using StrazhAPI.GK;

namespace StrazhAPI.Journal
{
	public enum JournalEventNameType
	{
		[EventName(JournalSubsystemType.System, "NULL", XStateClass.No)]
		NULL = 0,

		[EventName(JournalSubsystemType.System, "Неизвестное событие", XStateClass.No)]
		Неизвестное_событие = 1,

		[EventName(JournalSubsystemType.System, "Вход пользователя в систему", XStateClass.Info)]
		Вход_пользователя_в_систему = 3,

		[EventName(JournalSubsystemType.System, "Выход пользователя из системы", XStateClass.Info)]
		Выход_пользователя_из_системы = 4,

		[EventName(JournalSubsystemType.System, "Дежурство сдал", XStateClass.Info)]
		Дежурство_сдал = 5,

		[EventName(JournalSubsystemType.System, "Дежурство принял", XStateClass.Info)]
		Дежурство_принял = 6,

		[EventName(JournalSubsystemType.System, "Лицензия отсутствует", XStateClass.HasNoLicense)]
		Отсутствует_лицензия = 8,

		[EventName(JournalSubsystemType.System, "Лицензия обнаружена", XStateClass.HasLicense)]
		Лицензия_обнаружена = 9,

		[EventName(JournalSubsystemType.System, "Отмена операции", XStateClass.TechnologicalRegime)]
		Отмена_операции = 11,

		[EventName(JournalSubsystemType.System, "Сообщение автоматизации", XStateClass.Info)]
		Сообщение_автоматизации = 12,

		[EventName(JournalSubsystemType.System, "Применение конфигурации", XStateClass.Info)]
		Применение_конфигурации = 32,

		[EventName(JournalSubsystemType.SKD, "Потеря связи", XStateClass.ConnectionLost)]
		Потеря_связи = 103,

		[EventName(JournalSubsystemType.SKD, "Восстановление связи", XStateClass.ConnectionLost)]
		Восстановление_связи = 104,

		[EventName(JournalSubsystemType.SKD, "Проход разрешен", XStateClass.Info)]
		Проход_разрешен = 105,

		[EventName(JournalSubsystemType.SKD, "Проход запрещен", XStateClass.Attention)]
		Проход_запрещен = 106,

		[EventName(JournalSubsystemType.SKD, "Дверь не закрыта", XStateClass.Attention)]
		Дверь_не_закрыта_начало = 107,

		[EventName(JournalSubsystemType.SKD, "Взлом", XStateClass.Attention)]
		Взлом = 108,

		[EventName(JournalSubsystemType.SKD, "Повторный проход", XStateClass.Attention)]
		Повторный_проход = 109,

		[EventName(JournalSubsystemType.SKD, "Принуждение", XStateClass.Fire1)]
		Принуждение = 110,

		[EventName(JournalSubsystemType.SKD, "Открытие двери", XStateClass.On)]
		Открытие_двери = 111,

		[EventName(JournalSubsystemType.SKD, "Закрытие двери", XStateClass.Off)]
		Закрытие_двери = 112,

		[EventName(JournalSubsystemType.SKD, "Неизвестный статус двери", XStateClass.Unknown)]
		Неизвестный_статус_двери = 113,

		[EventName(JournalSubsystemType.SKD, "Вскрытие контроллера", XStateClass.Attention)]
		Вскрытие_контроллера_начало = 114,

		[EventName(JournalSubsystemType.SKD, "Множественный проход", XStateClass.Attention)]
		Множественный_проход = 115,

		[EventName(JournalSubsystemType.SKD, "Проход по отпечатку пальца", XStateClass.Info)]
		Проход_по_отпечатку_пальца = 116,

		[EventName(JournalSubsystemType.SKD, "Местная тревога", XStateClass.Attention)]
		Местная_тревога_начало = 117,

		[EventName(JournalSubsystemType.SKD, "Сброс Контроллера", XStateClass.TechnologicalRegime)]
		Сброс_Контроллера = 118,

		[EventName(JournalSubsystemType.SKD, "Перезагрузка Контроллера", XStateClass.TechnologicalRegime)]
		Перезагрузка_Контроллера = 119,

		[EventName(JournalSubsystemType.SKD, "Запись графиков доступа", XStateClass.TechnologicalRegime)]
		Запись_графиков_доступа = 120,

		[EventName(JournalSubsystemType.SKD, "Запись пропусков", XStateClass.TechnologicalRegime)]
		Перезапись_всех_карт = 121,

		[EventName(JournalSubsystemType.SKD, "Обновление ПО Контроллера", XStateClass.TechnologicalRegime)]
		Обновление_ПО_Контроллера = 122,

		[EventName(JournalSubsystemType.SKD, "Запрос конфигурации контроллера", XStateClass.TechnologicalRegime)]
		Запрос_конфигурации_контроллера = 123,

		[EventName(JournalSubsystemType.SKD, "Синхронизация времени контроллера", XStateClass.Info)]
		Синхронизация_времени_контроллера = 124,

		[EventName(JournalSubsystemType.SKD, "Запрос конфигурации двери", XStateClass.TechnologicalRegime)]
		Запрос_конфигурации_двери = 125,

		[EventName(JournalSubsystemType.SKD, "Запись конфигурации двери", XStateClass.TechnologicalRegime)]
		Запись_конфигурации_двери = 126,

		[EventName(JournalSubsystemType.SKD, "Запрос направления контроллера", XStateClass.TechnologicalRegime)]
		Запрос_направления_контроллера = 127,

		[EventName(JournalSubsystemType.SKD, "Запись направления контроллера", XStateClass.TechnologicalRegime)]
		Запись_направления_контроллера = 128,

		[EventName(JournalSubsystemType.SKD, "Запись пароля контроллера", XStateClass.TechnologicalRegime)]
		Запись_пароля_контроллера = 129,

		[EventName(JournalSubsystemType.SKD, "Запрос временных настроек контроллера", XStateClass.TechnologicalRegime)]
		Запрос_временных_настроек_контроллера = 130,

		[EventName(JournalSubsystemType.SKD, "Запись временных настроек контроллера", XStateClass.TechnologicalRegime)]
		Запись_временных_настроек_контроллера = 131,

		[EventName(JournalSubsystemType.SKD, "Запрос сетевых настроек контроллера", XStateClass.TechnologicalRegime)]
		Запрос_сетевых_настроек_контроллера = 132,

		[EventName(JournalSubsystemType.SKD, "Запись сетевых настроек контроллера", XStateClass.TechnologicalRegime)]
		Запись_сетевых_настроек_контроллера = 133,

		[EventName(JournalSubsystemType.SKD, "Команда \"Открыть\" удаленно", XStateClass.On, "Команда \"Открыть\" замок удаленно")]
		Команда_на_открытие_двери = 134,

		[EventName(JournalSubsystemType.SKD, "Команда \"Закрыть\" удаленно", XStateClass.Off, "Команда \"Закрыть\" замок удаленно")]
		Команда_на_закрытие_двери = 135,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод в режим \"Открыто\"", XStateClass.On, "Команда на перевод замка в режим \"Открыто\"")]
		Команда_на_перевод_замка_в_режим_Открыто = 136,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод в режим \"Закрыто\"", XStateClass.Off, "Команда на перевод замка в режим \"Закрыто\"")]
		Команда_на_перевод_замка_в_режим_Закрыто = 137,

		[EventName(JournalSubsystemType.SKD, "Команда \"Открыть\" удаленно", XStateClass.On, "Команда \"Открыть\" зону удаленно")]
		Команда_на_открытие_зоны = 138,

		[EventName(JournalSubsystemType.SKD, "Команда \"Закрыть\" удаленно", XStateClass.Off, "Команда \"Закрыть\" зону удаленно")]
		Команда_на_закрытие_зоны = 139,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод в режим \"Открыто\"", XStateClass.On, "Команда на перевод зоны в режим \"Открыто\"")]
		Команда_на_перевод_зоны_в_режим_Открыто = 140,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод в режим \"Закрыто\"", XStateClass.Off, "Команда на перевод зоны в режим \"Закрыто\"")]
		Команда_на_перевод_зоны_в_режим_Закрыто = 141,

		[EventName(JournalSubsystemType.SKD, "Команда \"Открыть\" удаленно", XStateClass.On, "Команда \"Открыть\" точку доступа удаленно")]
		Команда_на_открытие_точки_доступа = 142,

		[EventName(JournalSubsystemType.SKD, "Команда \"Закрыть\" удаленно", XStateClass.Off, "Команда \"Закрыть\" точку доступа удаленно")]
		Команда_на_закрытие_точки_доступа = 143,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод в режим \"Открыто\"", XStateClass.On, "Команда на перевод точки доступа в режим \"Открыто\"")]
		Команда_на_перевод_точки_доступа_в_режим_Открыто = 144,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод в режим \"Закрыто\"", XStateClass.Off, "Команда на перевод точки доступа в режим \"Закрыто\"")]
		Команда_на_перевод_точки_доступа_в_режим_Закрыто = 145,

		[EventName(JournalSubsystemType.SKD, "Добавление карты", XStateClass.Info)]
		Добавление_карты = 146,

		[EventName(JournalSubsystemType.SKD, "Редактирование карты", XStateClass.Info)]
		Редактирование_карты = 147,

		[EventName(JournalSubsystemType.SKD, "Удаление карты", XStateClass.Info)]
		Удаление_карты = 148,

		[EventName(JournalSubsystemType.SKD, "Редактирование сотрудника", XStateClass.Info)]
		Редактирование_сотрудника = 149,

		[EventName(JournalSubsystemType.SKD, "Редактирование подразделения", XStateClass.Info)]
		Редактирование_отдела = 150,

		[EventName(JournalSubsystemType.SKD, "Редактирование должности", XStateClass.Info)]
		Редактирование_должности = 151,

		[EventName(JournalSubsystemType.SKD, "Редактирование шаблона доступа", XStateClass.Info)]
		Редактирование_шаблона_доступа = 152,

		[EventName(JournalSubsystemType.SKD, "Редактирование организации", XStateClass.Info)]
		Редактирование_организации = 153,

		[EventName(JournalSubsystemType.SKD, "Редактирование дополнительной колонки", XStateClass.Info)]
		Редактирование_дополнительной_колонки = 154,

		[EventName(JournalSubsystemType.SKD, "Редактирование дневного графика", XStateClass.Info)]
		Редактирование_дневного_графика = 155,

		[EventName(JournalSubsystemType.SKD, "Редактирование графика работы", XStateClass.Info)]
		Редактирование_графика_работы = 156,

		[EventName(JournalSubsystemType.SKD, "Редактирование графика работы сотрудника", XStateClass.Info)]
		Редактирование_графика_работы_сотрудника = 157,

		[EventName(JournalSubsystemType.SKD, "Редактирование праздничного дня", XStateClass.Info)]
		Редактирование_праздничного_дня = 158,

		[EventName(JournalSubsystemType.SKD, "Редактирование шаблона пропуска", XStateClass.Info)]
		Редактирование_шаблона_пропуска = 160,

		[EventName(JournalSubsystemType.SKD, "Добавление нового сотрудника", XStateClass.Info)]
		Добавление_нового_сотрудника = 161,

		[EventName(JournalSubsystemType.SKD, "Добавление нового подразделения", XStateClass.Info)]
		Добавление_нового_отдела = 162,

		[EventName(JournalSubsystemType.SKD, "Добавление новой должности", XStateClass.Info)]
		Добавление_новой_должности = 163,

		[EventName(JournalSubsystemType.SKD, "Добавление нового шаблона доступа", XStateClass.Info)]
		Добавление_нового_шаблона_доступа = 164,

		[EventName(JournalSubsystemType.SKD, "Добавление новой организации", XStateClass.Info)]
		Добавление_новой_организации = 165,

		[EventName(JournalSubsystemType.SKD, "Добавление новой дополнительной колонки", XStateClass.Info)]
		Добавление_новой_дополнительной_колонки = 166,

		[EventName(JournalSubsystemType.SKD, "Добавление нового дневного графика", XStateClass.Info)]
		Добавление_нового_дневного_графика = 167,

		[EventName(JournalSubsystemType.SKD, "Добавление нового графика работы", XStateClass.Info)]
		Добавление_нового_графика_работы = 168,

		[EventName(JournalSubsystemType.SKD, "Добавление нового графика работы сотрудника", XStateClass.Info)]
		Добавление_нового_графика_работы_сотрудника = 169,

		[EventName(JournalSubsystemType.SKD, "Добавление нового праздничного дня", XStateClass.Info)]
		Добавление_нового_праздничного_дня = 170,

		[EventName(JournalSubsystemType.SKD, "Добавление нового шаблона пропуска", XStateClass.Info)]
		Добавление_нового_шаблона_пропуска = 171,

		[EventName(JournalSubsystemType.SKD, "Внесение оправдательного документа", XStateClass.Info)]
		AddTimeTrackDocument = 172,

		[EventName(JournalSubsystemType.SKD, "Удаление сотрудника", XStateClass.Info)]
		Удаление_сотрудника = 184,

		[EventName(JournalSubsystemType.SKD, "Удаление подразделения", XStateClass.Info)]
		Удаление_отдела = 185,

		[EventName(JournalSubsystemType.SKD, "Удаление должности", XStateClass.Info)]
		Удаление_должности = 186,

		[EventName(JournalSubsystemType.SKD, "Удаление шаблона доступа", XStateClass.Info)]
		Удаление_шаблона_доступа = 187,

		[EventName(JournalSubsystemType.SKD, "Удаление организации", XStateClass.Info)]
		Удаление_организации = 188,

		[EventName(JournalSubsystemType.SKD, "Удаление дополнительной колонки", XStateClass.Info)]
		Удаление_дополнительной_колонки = 189,

		[EventName(JournalSubsystemType.SKD, "Удаление дневного графика", XStateClass.Info)]
		Удаление_дневного_графика = 190,

		[EventName(JournalSubsystemType.SKD, "Удаление графика работы", XStateClass.Info)]
		Удаление_графика_работы = 191,

		[EventName(JournalSubsystemType.SKD, "Удаление графика работы сотрудника", XStateClass.Info)]
		Удаление_графика_работы_сотрудника = 192,

		[EventName(JournalSubsystemType.SKD, "Удаление праздничного дня", XStateClass.Info)]
		Удаление_праздничного_дня = 193,

		[EventName(JournalSubsystemType.SKD, "Удаление шаблона пропуска", XStateClass.Info)]
		Удаление_шаблона_пропуска = 194,

		[EventName(JournalSubsystemType.SKD, "Восстановление сотрудника", XStateClass.Info)]
		Восстановление_сотрудника = 195,

		[EventName(JournalSubsystemType.SKD, "Восстановление подразделения", XStateClass.Info)]
		Восстановление_отдела = 196,

		[EventName(JournalSubsystemType.SKD, "Восстановление должности", XStateClass.Info)]
		Восстановление_должности = 197,

		[EventName(JournalSubsystemType.SKD, "Восстановление шаблона доступа", XStateClass.Info)]
		Восстановление_шаблона_доступа = 198,

		[EventName(JournalSubsystemType.SKD, "Восстановление организации", XStateClass.Info)]
		Восстановление_организации = 199,

		[EventName(JournalSubsystemType.SKD, "Восстановление дополнительной колонки", XStateClass.Info)]
		Восстановление_дополнительной_колонки = 200,

		[EventName(JournalSubsystemType.SKD, "Восстановление дневного графика", XStateClass.Info)]
		Восстановление_дневного_графика = 201,

		[EventName(JournalSubsystemType.SKD, "Восстановление графика работы", XStateClass.Info)]
		Восстановление_графика_работы = 202,

		[EventName(JournalSubsystemType.SKD, "Восстановление графика работы сотрудника", XStateClass.Info)]
		Восстановление_графика_работы_сотрудника = 203,

		[EventName(JournalSubsystemType.SKD, "Восстановление праздничного дня", XStateClass.Info)]
		Восстановление_праздничного_дня = 204,

		[EventName(JournalSubsystemType.SKD, "Восстановление шаблона пропуска", XStateClass.Info)]
		Восстановление_шаблона_пропуска = 205,

		[EventName(JournalSubsystemType.Video, "Перевод в предустановку", XStateClass.On)]
		Перевод_в_предустановку = 209,

		[EventName(JournalSubsystemType.SKD, "Открытие замка", XStateClass.On)]
		Открытие_замка_двери = 210,

		[EventName(JournalSubsystemType.SKD, "Закрытие замка", XStateClass.Off)]
		Закрытие_замка_двери = 211,

		[EventName(JournalSubsystemType.SKD, "Неизвестный статус замка двери", XStateClass.Unknown)]
		Неизвестный_статус_замка_двери = 212,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод в режим \"Норма\"", XStateClass.Norm, "Команда на перевод точки доступа в режим \"Норма\"")]
		Команда_на_перевод_точки_доступа_в_режим_Норма = 213,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод в режим \"Норма\"", XStateClass.Norm, "Команда на перевод зоны в режим \"Норма\"")]
		Команда_на_перевод_зоны_в_режим_Норма = 214,

		[EventName(JournalSubsystemType.SKD, "Команда на перевод в режим \"Норма\"", XStateClass.Norm, "Команда на перевод замка в режим \"Норма\"")]
		Команда_на_перевод_замка_в_режим_Норма = 215,

		[EventName(JournalSubsystemType.SKD, "Запрос настроек запрета повторного прохода", XStateClass.TechnologicalRegime)]
		Запрос_antipassback_настройки_контроллера = 216,

		[EventName(JournalSubsystemType.SKD, "Запись настроек запрета повторного прохода", XStateClass.TechnologicalRegime)]
		Запись_antipassback_настройки_контроллера = 217,

		[EventName(JournalSubsystemType.SKD, "Запрос настроек блокировки одновременного прохода", XStateClass.TechnologicalRegime)]
		Запрос_interlock_настройки_контроллера = 218,

		[EventName(JournalSubsystemType.SKD, "Запись настроек блокировки одновременного прохода", XStateClass.TechnologicalRegime)]
		Запись_interlock_настройки_контроллера = 219,

		[EventName(JournalSubsystemType.SKD, "Подтверждение тревоги", XStateClass.Fire1)]
		Подтверждение_тревоги = 220,

		[EventName(JournalSubsystemType.SKD, "Команда на сброс состояния \"Взлом\"", XStateClass.Info, "Команда на сброс состояния замка \"Взлом\"")]
		Команда_на_сброс_состояния_взлом_замка = 221,

		[EventName(JournalSubsystemType.SKD, "Команда на сброс состояния \"Взлом\"", XStateClass.Info, "Команда на сброс состояния точки доступа \"Взлом\"")]
		Команда_на_сброс_состояния_взлом_точки_доступа = 222,

		[EventName(JournalSubsystemType.SKD, "Команда на сброс состояния \"Взлом\"", XStateClass.Info, "Команда на сброс состояния зоны \"Взлом\"")]
		Команда_на_сброс_состояния_взлом_зоны = 223,

		[EventName(JournalSubsystemType.SKD, "Запрос паролей замков", XStateClass.TechnologicalRegime)]
		GetControllerLocksPasswords = 224,

		[EventName(JournalSubsystemType.SKD, "Запись паролей замков", XStateClass.TechnologicalRegime)]
		SetControllerLocksPasswords = 225,

		[EventName(JournalSubsystemType.SKD, "Перевод в режим \"Открыто\"", XStateClass.On, "Перевод замка в режим \"Открыто\"")]
		Перевод_замка_в_режим_Открыто = 226,

		[EventName(JournalSubsystemType.SKD, "Перевод в режим \"Закрыто\"", XStateClass.Off, "Перевод замка в режим \"Закрыто\"")]
		Перевод_замка_в_режим_Закрыто = 227,

		[EventName(JournalSubsystemType.SKD, "Перевод в режим \"Норма\"", XStateClass.Norm, "Перевод замка в режим \"Норма\"")]
		Перевод_замка_в_режим_Норма = 228,

		[EventName(JournalSubsystemType.SKD, "Перевод в режим \"Открыто\"", XStateClass.On, "Перевод точки доступа в режим \"Открыто\"")]
		Перевод_точки_доступа_в_режим_Открыто = 229,

		[EventName(JournalSubsystemType.SKD, "Перевод в режим \"Закрыто\"", XStateClass.Off, "Перевод точки доступа в режим \"Закрыто\"")]
		Перевод_точки_доступа_в_режим_Закрыто = 230,

		[EventName(JournalSubsystemType.SKD, "Перевод в режим \"Норма\"", XStateClass.Norm, "Перевод точки доступа в режим \"Норма\"")]
		Перевод_точки_доступа_в_режим_Норма = 231,

		[EventName(JournalSubsystemType.SKD, "Перевод в режим \"Открыто\"", XStateClass.On, "Перевод зоны в режим \"Открыто\"")]
		Перевод_зоны_в_режим_Открыто = 232,

		[EventName(JournalSubsystemType.SKD, "Перевод в режим \"Закрыто\"", XStateClass.Off, "Перевод зоны в режим \"Закрыто\"")]
		Перевод_зоны_в_режим_Закрыто = 233,

		[EventName(JournalSubsystemType.SKD, "Перевод в режим \"Норма\"", XStateClass.Norm, "Перевод зоны в режим \"Норма\"")]
		Перевод_зоны_в_режим_Норма = 234,

		[EventName(JournalSubsystemType.SKD, "Сброс состояния \"Взлом\"", XStateClass.Info, "Сброс состояния замка \"Взлом\"")]
		Сброс_состояния_взлом_замка = 235,

		[EventName(JournalSubsystemType.SKD, "Сброс состояния \"Взлом\"", XStateClass.Info, "Сброс состояния точки доступа \"Взлом\"")]
		Сброс_состояния_взлом_точки_доступа = 236,

		[EventName(JournalSubsystemType.SKD, "Сброс состояния \"Взлом\"", XStateClass.Info, "Сброс состояния зоны \"Взлом\"")]
		Сброс_состояния_взлом_зоны = 237,

		[EventName(JournalSubsystemType.SKD, "Сброс ограничения на повторный проход", XStateClass.Info)]
		Сброс_антипессбэка_для_выбранной_ТД = 238,

		[EventName(JournalSubsystemType.SKD, "Сброс ограничения на повторный проход для ВСЕХ пропусков", XStateClass.Info)]
		Сброс_антипессбэка_для_всех_пропусков = 239,

		[EventName(JournalSubsystemType.SKD, "Добавление интервала", XStateClass.Info)]
		Добавление_интервала = 240,

		[EventName(JournalSubsystemType.SKD, "Изменение границ интервала", XStateClass.Info)]
		Изменение_границы_интервала = 241,

		[EventName(JournalSubsystemType.SKD, "Удаление интервала", XStateClass.Info)]
		Удаление_интервала = 242,

		[EventName(JournalSubsystemType.SKD, "Принудительное закрытие интервала", XStateClass.Info)]
		Закрытие_интервала = 243,

		[EventName(JournalSubsystemType.SKD, "Установка признака \"Не учитывать в расчетах\"", XStateClass.Info)]
		Установка_неУчитывать_в_расчетах = 244,

		[EventName(JournalSubsystemType.SKD, "Снятие признака \"Не учитывать в расчетах\"", XStateClass.Info)]
		Снятие_неУчитывать_в_расчетах = 245,

		[EventName(JournalSubsystemType.SKD, "Восстановление: Вскрытие контроллера", XStateClass.Attention)]
		Вскрытие_контроллера_конец = 246,

		[EventName(JournalSubsystemType.SKD, "Восстановление: Дверь не закрыта", XStateClass.Attention)]
		Дверь_не_закрыта_конец = 247,

		[EventName(JournalSubsystemType.SKD, "Восстановление: Местная тревога", XStateClass.Attention)]
		Местная_тревога_конец = 248,

		[EventName(JournalSubsystemType.SKD, "Сброс ограничения на повторный проход для всех точек доступа", XStateClass.Info)]
		Сброс_антипессбэка_для_всех_ТД = 249,

		[EventName(JournalSubsystemType.SKD, "Редактирование оправдательного документа", XStateClass.Info)]
		EditTimeTrackDocument = 250,

		[EventName(JournalSubsystemType.SKD, "Удаление оправдательного документа", XStateClass.Info)]
		RemoveTimeTrackDocument = 251,
	}
}