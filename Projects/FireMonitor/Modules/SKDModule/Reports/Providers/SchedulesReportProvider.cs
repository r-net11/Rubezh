using System.Collections.Generic;
using Localization.SKD.Common;
using StrazhAPI.Models;
using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class SchedulesReportProvider : FilteredSKDReportProvider<SchedulesReportFilter>
	{
		public SchedulesReportProvider()
			: base(CommonResources.WorkSchedulesReport, 422, SKDReportGroup.TimeTracking, PermissionType.Oper_Reports_Schedules)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "Employee", CommonResources.Employee },
					{ "Organisation", CommonResources.Organization },
					{ "Department", CommonResources.Department },
					{ "Position", CommonResources.Position },
					{ "Schedule", CommonResources.Schedule },
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
