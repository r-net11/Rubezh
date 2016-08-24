using System.Collections.Generic;
using Localization.SKD.Common;
using StrazhAPI.Models;
using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class EmployeeRootReportProvider : FilteredSKDReportProvider<EmployeeRootReportFilter>
	{
		public EmployeeRootReportProvider()
			: base(CommonResources.Route, 402, SKDReportGroup.Events, PermissionType.Oper_Reports_EmployeeRoot)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "Name", CommonResources.Visitor },
					{ "Organisation", CommonResources.Organization },
					{ "Department", CommonResources.Department },
					{ "Position", CommonResources.PositionMaintainer },
				},
				Pages = new List<FilterContainerViewModel>()
				{
					new ZonePageViewModel(),
					new OrganizationPageViewModel(true),
					new DepartmentPageViewModel(),
					new PositionPageViewModel(),
					new EmployeePageViewModel(),
				},
			};
		}
	}
}
