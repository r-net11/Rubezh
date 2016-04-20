using System.Collections.Generic;
using RubezhAPI.Models;
using RubezhAPI.SKD.ReportFilters;
using Infrastructure.Common.Windows.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class DisciplineReportProvider : FilteredSKDReportProvider<DisciplineReportFilter>
	{
		public DisciplineReportProvider()
			: base("Дисциплинарный отчет", 421, SKDReportGroup.TimeTracking, PermissionType.Oper_Reports_Discipline)
		{
		}

		
		public override FilterModel InitializeFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "Date", "Дата" },
					{ "FirstEnter", "Приход" },
					{ "LastExit", "Уход" },
					{ "Employee", "Сотрудник" },
					{ "Organisation", "Организация" },
					{ "Department", "Подразделение" },
					{ "IsHoliday", "Р/В" },
					{ "Late", "Опоздание" },
					{ "EarlyLeave", "Уход раньше" },
					{ "Absence", "Отсутствие" },
					{ "Overtime", "Переработка" },
				},
				Pages = new List<FilterContainerViewModel>()
				{
					new OrganizationPageViewModel(true),
					new DepartmentPageViewModel(),
					new EmployeePageViewModel(),
					new ScheduleSchemePageViewModel(),
					new DisciplinaryFilterPageViewModel(),
				},
			};
		}
	}
}
