using System.Collections.Generic;
using Localization.SKD.Common;
using StrazhAPI.Models;
using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class DisciplineReportProvider : FilteredSKDReportProvider<DisciplineReportFilter>
	{
		public DisciplineReportProvider()
            : base(CommonResources.DisciplineReport, 421, SKDReportGroup.TimeTracking, PermissionType.Oper_Reports_Discipline)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string>
				{
					{ "Date", CommonResources.Date },
					{ "Employee", CommonResources.Employee },
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
