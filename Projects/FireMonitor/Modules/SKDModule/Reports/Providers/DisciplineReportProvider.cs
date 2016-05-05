using System.Collections.Generic;
using StrazhAPI.Models;
using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class DisciplineReportProvider : FilteredSKDReportProvider<DisciplineReportFilter>
	{
		public DisciplineReportProvider()
			: base("Дисциплинарный отчет", 421, SKDReportGroup.TimeTracking, PermissionType.Oper_Reports_Discipline)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string>
				{
					{ "Date", "Дата" },
					{ "Employee", "Сотрудник" },
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
