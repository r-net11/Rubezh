using System.Collections.Generic;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class ReportProvider421 : FilteredSKDReportProvider<ReportFilter421>
	{
		public ReportProvider421()
			: base("Report421", "Дисциплинарный отчет", 421, SKDReportGroup.TimeTracking)
		{
		}

		public override FilterModel GetFilterModel()
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
