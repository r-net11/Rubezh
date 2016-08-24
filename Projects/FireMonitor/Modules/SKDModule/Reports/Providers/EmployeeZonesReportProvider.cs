using System.Collections.Generic;
using Localization.SKD.Common;
using StrazhAPI.Models;
using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class EmployeeZonesReportProvider : FilteredSKDReportProvider<EmployeeZonesReportFilter>
	{
		public EmployeeZonesReportProvider()
			: base(CommonResources.Location, 417, SKDReportGroup.HR, PermissionType.Oper_Reports_EmployeeZone)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "Zone", CommonResources.Zone },
					{ "EnterDateTime", CommonResources.EnterDateTime },
					{ "Employee", CommonResources.Visitor },
					{ "Orgnisation", CommonResources.Organization },
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
				MainViewModel = new PlacementMainPageViewModel(),
			};
		}
	}
}