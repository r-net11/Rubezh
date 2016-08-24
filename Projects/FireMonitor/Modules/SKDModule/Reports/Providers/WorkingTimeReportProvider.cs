using System.Collections.Generic;
using Localization.SKD.Common;
using StrazhAPI.Models;
using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class WorkingTimeReportProvider : FilteredSKDReportProvider<WorkingTimeReportFilter>
	{
		public WorkingTimeReportProvider() : base(CommonResources.WorkTimeInfo, 424, SKDReportGroup.TimeTracking, PermissionType.Oper_Reports_WorkTime)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel
			{
				Columns = new Dictionary<string, string>
				{
					{ "Employee", CommonResources.Employee },
					{ "Department", CommonResources.Department },
					{ "Position", CommonResources.Position },
					{ "Balance", CommonResources.Balance }
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
