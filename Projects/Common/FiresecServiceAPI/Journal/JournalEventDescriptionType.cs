namespace FiresecAPI.Journal
{
	public enum JournalEventDescriptionType
	{
		[EventDescriptionAttribute("NULL")]
		NULL = 0,

		[EventDescriptionAttribute("Добавление или редактирование", JournalEventNameType.Редактирование_сотрудника, JournalEventNameType.Редактирование_отдела, JournalEventNameType.Редактирование_должности, JournalEventNameType.Редактирование_шаблона_доступа, JournalEventNameType.Редактирование_организации, JournalEventNameType.Редактирование_дополнительной_колонки, JournalEventNameType.Редактирование_дневного_графика, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы_сотрудника, JournalEventNameType.Редактирование_праздничного_дня)]
		Добавление_или_редактирование = 253,

		[EventDescriptionAttribute("Редактирование", JournalEventNameType.Редактирование_сотрудника, JournalEventNameType.Редактирование_отдела, JournalEventNameType.Редактирование_должности, JournalEventNameType.Редактирование_шаблона_доступа, JournalEventNameType.Редактирование_организации, JournalEventNameType.Редактирование_дополнительной_колонки, JournalEventNameType.Редактирование_дневного_графика, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы_сотрудника, JournalEventNameType.Редактирование_праздничного_дня)]
		Редактирование = 254,

		[EventDescriptionAttribute("Удаление", JournalEventNameType.Редактирование_сотрудника, JournalEventNameType.Редактирование_отдела, JournalEventNameType.Редактирование_должности, JournalEventNameType.Редактирование_шаблона_доступа, JournalEventNameType.Редактирование_организации, JournalEventNameType.Редактирование_дополнительной_колонки, JournalEventNameType.Редактирование_дневного_графика, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы_сотрудника, JournalEventNameType.Редактирование_праздничного_дня)]
		Удаление = 255,

		[EventDescriptionAttribute("Восстановление", JournalEventNameType.Редактирование_сотрудника, JournalEventNameType.Редактирование_отдела, JournalEventNameType.Редактирование_должности, JournalEventNameType.Редактирование_шаблона_доступа, JournalEventNameType.Редактирование_организации, JournalEventNameType.Редактирование_дополнительной_колонки, JournalEventNameType.Редактирование_дневного_графика, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы_сотрудника, JournalEventNameType.Редактирование_праздничного_дня)]
		Восстановление = 256,

		[EventDescriptionAttribute("Метод открытия Неизвестно", JournalEventNameType.Проход_разрешен, JournalEventNameType.Проход_запрещен)]
		Метод_открытия_Неизвестно = 257,

		[EventDescriptionAttribute("Метод открытия Пароль", JournalEventNameType.Проход_разрешен, JournalEventNameType.Проход_запрещен)]
		Метод_открытия_Пароль = 258,

		[EventDescriptionAttribute("Метод открытия Карта", JournalEventNameType.Проход_разрешен, JournalEventNameType.Проход_запрещен)]
		Метод_открытия_Карта = 259,

		[EventDescriptionAttribute("Метод открытия Сначала карта", JournalEventNameType.Проход_разрешен, JournalEventNameType.Проход_запрещен)]
		Метод_открытия_Сначала_карта = 260,

		[EventDescriptionAttribute("Метод открытия Сначала пароль", JournalEventNameType.Проход_разрешен, JournalEventNameType.Проход_запрещен)]
		Метод_открытия_Сначала_пароль = 261,

		[EventDescriptionAttribute("Метод открытия Удаленно", JournalEventNameType.Проход_разрешен, JournalEventNameType.Проход_запрещен)]
		Метод_открытия_Удаленно = 262,

		[EventDescriptionAttribute("Метод открытия Кнопка", JournalEventNameType.Проход_разрешен, JournalEventNameType.Проход_запрещен)]
		Метод_открытия_Кнопка = 263
	}
}