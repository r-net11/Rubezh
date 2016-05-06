using System.Collections.Generic;
using StrazhAPI.Models;
using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class WorkingTimeReportProvider : FilteredSKDReportProvider<WorkingTimeReportFilter>
	{
		public WorkingTimeReportProvider() : base("Справка по отработанному времени", 424, SKDReportGroup.TimeTracking, PermissionType.Oper_Reports_WorkTime)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel
			{
				Columns = new Dictionary<string, string>
				{
					{ "Employee", "Сотрудник" },
					{ "Department", "Подразделение" },
					{ "Position", "Должность" },
					{ "Balance", "Баланс" }
				},
				Pages = new List<FilterContainerViewModel>
				{
					new OrganizationPageViewModel(true),
					new DepartmentPageViewModel(),
					new PositionPageViewModel(),
					new EmployeePageViewModel()
				},
			};
		}
	}
}
