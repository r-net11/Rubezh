namespace StrazhAPI.Journal
{
	public enum JournalEventDescriptionType
	{
		[EventDescription("NULL")]
		NULL = 0,

		[EventDescription("Добавление или редактирование", JournalEventNameType.Редактирование_сотрудника, JournalEventNameType.Редактирование_отдела, JournalEventNameType.Редактирование_должности, JournalEventNameType.Редактирование_шаблона_доступа, JournalEventNameType.Редактирование_организации, JournalEventNameType.Редактирование_дополнительной_колонки, JournalEventNameType.Редактирование_дневного_графика, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы_сотрудника, JournalEventNameType.Редактирование_праздничного_дня)]
		Добавление_или_редактирование = 253,

		[EventDescription("Редактирование", JournalEventNameType.Редактирование_сотрудника, JournalEventNameType.Редактирование_отдела, JournalEventNameType.Редактирование_должности, JournalEventNameType.Редактирование_шаблона_доступа, JournalEventNameType.Редактирование_организации, JournalEventNameType.Редактирование_дополнительной_колонки, JournalEventNameType.Редактирование_дневного_графика, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы_сотрудника, JournalEventNameType.Редактирование_праздничного_дня)]
		Редактирование = 254,

		[EventDescription("Удаление", JournalEventNameType.Редактирование_сотрудника, JournalEventNameType.Редактирование_отдела, JournalEventNameType.Редактирование_должности, JournalEventNameType.Редактирование_шаблона_доступа, JournalEventNameType.Редактирование_организации, JournalEventNameType.Редактирование_дополнительной_колонки, JournalEventNameType.Редактирование_дневного_графика, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы_сотрудника, JournalEventNameType.Редактирование_праздничного_дня)]
		Удаление = 255,

		[EventDescription("Восстановление", JournalEventNameType.Редактирование_сотрудника, JournalEventNameType.Редактирование_отдела, JournalEventNameType.Редактирование_должности, JournalEventNameType.Редактирование_шаблона_доступа, JournalEventNameType.Редактирование_организации, JournalEventNameType.Редактирование_дополнительной_колонки, JournalEventNameType.Редактирование_дневного_графика, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы_сотрудника, JournalEventNameType.Редактирование_праздничного_дня)]
		Восстановление = 256,

		[EventDescription("Метод открытия Неизвестно", JournalEventNameType.Проход_разрешен, JournalEventNameType.Проход_запрещен)]
		Метод_открытия_Неизвестно = 257,

		[EventDescription("Метод открытия Пароль", JournalEventNameType.Проход_разрешен, JournalEventNameType.Проход_запрещен)]
		Метод_открытия_Пароль = 258,

		[EventDescription("Метод открытия Карта", JournalEventNameType.Проход_разрешен, JournalEventNameType.Проход_запрещен)]
		Метод_открытия_Карта = 259,

		[EventDescription("Метод открытия Сначала карта", JournalEventNameType.Проход_разрешен, JournalEventNameType.Проход_запрещен)]
		Метод_открытия_Сначала_карта = 260,

		[EventDescription("Метод открытия Сначала пароль", JournalEventNameType.Проход_разрешен, JournalEventNameType.Проход_запрещен)]
		Метод_открытия_Сначала_пароль = 261,

		[EventDescription("Метод открытия Удаленно", JournalEventNameType.Проход_разрешен, JournalEventNameType.Проход_запрещен)]
		Метод_открытия_Удаленно = 262,

		[EventDescription("Метод открытия Кнопка", JournalEventNameType.Проход_разрешен, JournalEventNameType.Проход_запрещен)]
		Метод_открытия_Кнопка = 263
	}
}