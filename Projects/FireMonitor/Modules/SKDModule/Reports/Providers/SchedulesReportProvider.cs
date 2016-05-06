using System.Collections.Generic;
using StrazhAPI.Models;
using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class SchedulesReportProvider : FilteredSKDReportProvider<SchedulesReportFilter>
	{
		public SchedulesReportProvider()
			: base("Отчет по графикам работы", 422, SKDReportGroup.TimeTracking, PermissionType.Oper_Reports_Schedules)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "Employee", "Сотрудник" },
					{ "Organisation", "Организация" },
					{ "Department", "Подразделение" },
					{ "Position", "Должность" },
					{ "Schedule", "График" },
				},
				Pages = new List<FilterContainerViewModel>()
				{
					new OrganizationPageViewModel(true),
					new DepartmentPageViewModel(),
					new PositionPageViewModel(),
					new EmployeePageViewModel(),
					new ScheduleSchemePageViewModel(),
				},
			};
		}
	}
}
