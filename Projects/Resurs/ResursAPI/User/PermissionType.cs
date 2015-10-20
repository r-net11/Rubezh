using System.ComponentModel;

namespace ResursAPI
{
	public enum PermissionType
	{
		[DescriptionAttribute("Просмотр Пользователей")]
		ViewUser = 0,

		[DescriptionAttribute("Редактирование Пользователя")]
		EditUser = 1,

		[DescriptionAttribute("Просмотр Устройств")]
		ViewDevice = 2,

		[DescriptionAttribute("Редактирование Устройства")]
		EditDevice = 3,

		[DescriptionAttribute("Просмотр Абонентов")]
		ViewConsumer = 4,

		[DescriptionAttribute("Редактирование Абонента")]
		EditConsumer = 5,

		[DescriptionAttribute("Просмотр Тарифов")]
		ViewTariff = 6,

		[DescriptionAttribute("Редактирование Тарифа ")]
		EditTariff = 7,

		[DescriptionAttribute("Просмотр Отчетов")]
		ViewReport = 8,

		[DescriptionAttribute("Просмотр Графиков")]
		ViewPlot = 9,

		[DescriptionAttribute("Просмотр Журнала событий")]
		ViewJournal = 10,

	}
}