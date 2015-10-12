using System.Collections.Generic;
using RubezhAPI.Models;
using RubezhAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class WorkingTimeReportProvider : FilteredSKDReportProvider<WorkingTimeReportFilter>
	{
		public WorkingTimeReportProvider()
			: base("Справка по отработанному времени", 424, SKDReportGroup.TimeTracking, PermissionType.Oper_Reports_WorkTime)
		{
		}

		public override FilterModel InitializeFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "Employee", "Сотрудник" },
					{ "Department", "Подразделение" },
					{ "Position", "Должность" },
					{ "Balance", "Баланс отработанного времени" },
					{ "TotalBalance", "Итоговый баланс" },
				},
				Pages = new List<FilterContainerViewModel>()
				{
					new OrganizationPageViewModel(true),
					new DepartmentPageViewModel(),
					new PositionPageViewModel(),
					new EmployeePageViewModel(),
				},
			};
		}
	}
}
