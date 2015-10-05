using System.ComponentModel;

namespace ResursAPI
{
	public enum PermissionType
	{
		[DescriptionAttribute("Вкладка Устройства")]
		Device,

		[DescriptionAttribute("Редактирование вкладки Устройства")]
		EditDevice,

		[DescriptionAttribute("Вкладка Абоненты")]
		Apartment,

		[DescriptionAttribute("Редактирование вкладки Абоненты")]
		EditApartment,

		[DescriptionAttribute("Вкладка  Пользователи")]
		User,

		[DescriptionAttribute("Редактирование вкладки Пользователи")]
		EditUser,

		[DescriptionAttribute("Вкладка Тарифы")]
		Tariff,

		[DescriptionAttribute("Редактирование вкладки Тарифы ")]
		EditTariff,

		[DescriptionAttribute("Вкладка Отчеты")]
		Report,

		[DescriptionAttribute("Редактирование вкладки Отчеты")]
		EditReport,

		[DescriptionAttribute("Вкладка Графики")]
		Plot,

		[DescriptionAttribute("Редактирование вкладки Графики")]
		EditPlot,

		[DescriptionAttribute("Вкладка Журнал событий")]
		Journal,

		[DescriptionAttribute("Редактирование вкладки Журнал событий")]
		EditJournal,
	}
}