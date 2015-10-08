using System.ComponentModel;

namespace ResursAPI
{
	public enum PermissionType
	{
		[DescriptionAttribute("Вкладка  Пользователи")]
		User = 0,

		[DescriptionAttribute("Редактирование вкладки Пользователи")]
		EditUser = 1,

		[DescriptionAttribute("Вкладка Устройства")]
		Device = 2,

		[DescriptionAttribute("Редактирование вкладки Устройства")]
		EditDevice = 3,

		[DescriptionAttribute("Вкладка Абоненты")]
		Apartment = 4,

		[DescriptionAttribute("Редактирование вкладки Абоненты")]
		EditApartment = 5,

		[DescriptionAttribute("Вкладка Тарифы")]
		Tariff = 6,

		[DescriptionAttribute("Редактирование вкладки Тарифы ")]
		EditTariff = 7,

		[DescriptionAttribute("Вкладка Отчеты")]
		Report = 8,

		[DescriptionAttribute("Редактирование вкладки Отчеты")]
		EditReport = 9,

		[DescriptionAttribute("Вкладка Графики")]
		Plot = 10,

		[DescriptionAttribute("Редактирование вкладки Графики")]
		EditPlot,

		[DescriptionAttribute("Вкладка Журнал событий")]
		Journal = 11,

		[DescriptionAttribute("Редактирование вкладки Журнал событий")]
		EditJournal = 12,
	}
}