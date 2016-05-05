using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel.PeerResolvers;
using System.Text;
using LocalizationConveters;

namespace FiresecAPI.Enums
{
	public enum ReportType
	{
		//[Description("Сведения о пропусках")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ReportType), "CardsReport")]
		CardsReport = 0,

		//[Description("Список подразделений организаций")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ReportType), "DepartmentsReport")]
		DepartmentsReport = 1,
		
        //[Description("Дисциплинарный отчет")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ReportType), "DisciplineReport")]
		DisciplineReport = 2,
		
        //[Description("Отчет по оправдательным документам")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ReportType), "DocumentsReport")]
		DocumentsReport = 3,
		
        //[Description("Список точек доступа")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ReportType), "DoorsReport")]
		DoorsReport = 4,
		
        //[Description("Доступ в зоны сотрудников / посетителей")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ReportType), "EmployeeAccessReport")]
		EmployeeAccessReport = 5,
		
        //[Description("Права доступа сотрудников / посетителей")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ReportType), "EmployeeDoorsReport")]
		EmployeeDoorsReport = 6,
		
        //[Description("Справка о сотруднике / посетителе")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ReportType), "EmployeeReport")]
        EmployeeReport = 7,
		
        //[Description("Маршрут сотрудника / посетителя")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ReportType), "EmployeeRootReport")]
        EmployeeRootReport = 8,
		
        //[Description("Местонахождение сотрудников / посетителей")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ReportType), "EmployeeZonesReport")]
        EmployeeZonesReport = 9,
		
        //[Description("Отчет по событиям системы контроля доступа")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ReportType), "EventsReport")]
        EventsReport = 10,
		
        //[Description("Список должностей организации")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ReportType), "PositionsReport")]
        PositionsReport = 11,
		
        //[Description("Отчет по графикам работы")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ReportType), "SchedulesReport")]
        SchedulesReport = 12,
		
        //[Description("Справка по отработанному времени")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ReportType), "WorkingTimeReport")]
        WorkingTimeReport = 13
	}
}
