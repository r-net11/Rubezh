using System.ComponentModel;

namespace StrazhAPI.Enums
{
	public enum ReportType
	{
		[Description("Сведения о пропусках")]
		CardsReport = 0,
		[Description("Список подразделений организаций")]
		DepartmentsReport = 1,
		[Description("Дисциплинарный отчет")]
		DisciplineReport = 2,
		[Description("Отчет по оправдательным документам")]
		DocumentsReport = 3,
		[Description("Список точек доступа")]
		DoorsReport = 4,
		[Description("Доступ в зоны сотрудников / посетителей")]
		EmployeeAccessReport = 5,
		[Description("Права доступа сотрудников / посетителей")]
		EmployeeDoorsReport = 6,
		[Description("Справка о сотруднике / посетителе")]
		EmployeeReport = 7,
		[Description("Маршрут сотрудника / посетителя")]
		EmployeeRootReport = 8,
		[Description("Местонахождение сотрудников / посетителей")]
		EmployeeZonesReport = 9,
		[Description("Отчет по событиям системы контроля доступа")]
		EventsReport = 10,
		[Description("Список должностей организации")]
		PositionsReport = 11,
		[Description("Отчет по графикам работы")]
		SchedulesReport = 12,
		[Description("Справка по отработанному времени")]
		WorkingTimeReport = 13
	}
}
