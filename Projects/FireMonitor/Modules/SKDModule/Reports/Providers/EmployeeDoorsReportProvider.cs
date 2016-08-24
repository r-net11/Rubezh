using System.Collections.Generic;
using Localization.SKD.Common;
using StrazhAPI.Models;
using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class EmployeeDoorsReportProvider : FilteredSKDReportProvider<EmployeeDoorsReportFilter>
	{
		public EmployeeDoorsReportProvider()
			: base(CommonResources.EmplAccessPermissions, 413, SKDReportGroup.HR, PermissionType.Oper_Reports_Employees_Rights)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel
			{
				Columns = new Dictionary<string, string>
				{
					{ "AccessPoint", CommonResources.Name },
					{ "ZoneOut", CommonResources.FromZone },
					{ "ZoneIn", CommonResources.ToZone },
					{ "Enter", CommonResources.Enter },
					{ "Exit", CommonResources.Exit },
					{ "Type", CommonResources.PasscardType },
					{ "Employee", CommonResources.Visitor },
					{ "Organisation", CommonResources.Organization },
					{ "Department", CommonResources.Department },
					{ "Position", CommonResources.PositionMaintainer },
				},
				Pages = new List<FilterContainerViewModel>
				{
					new PassCardTypePageViewModel() { IsActive = true },
					new ZonePageViewModel(),
					new SchedulePageViewModel(),
					new OrganizationPageViewModel(true),
					new DepartmentPageViewModel(),
					new PositionPageViewModel(),
					new EmployeePageViewModel(),
				},
			};
		}
	}
}