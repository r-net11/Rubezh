using System.ComponentModel;

namespace ResursAPI
{
	public enum PermissionType
	{
		[DescriptionAttribute("Просмотр Пользователей")]
		User = 0,

		[DescriptionAttribute("Редактирование Пользователя")]
		EditUser = 1,

		[DescriptionAttribute("Просмотр Устройств")]
		Device = 2,

		[DescriptionAttribute("Редактирование Устройства")]
		EditDevice = 3,

		[DescriptionAttribute("Просмотр Абонентов")]
		Consumer,

		[DescriptionAttribute("Редактирование Абонента")]
		EditConsumer,

		[DescriptionAttribute("Просмотр Тарифов")]
		Tariff = 6,

		[DescriptionAttribute("Редактирование Тарифа ")]
		EditTariff = 7,

		[DescriptionAttribute("Просмотр Отчетов")]
		Report = 8,

		[DescriptionAttribute("Просмотр Графиков")]
		Plot = 9,

		[DescriptionAttribute("Просмотр Журнала событий")]
		Journal = 10,

	}
}