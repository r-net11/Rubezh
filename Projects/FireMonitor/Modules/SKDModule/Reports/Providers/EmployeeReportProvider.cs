using System.Collections.Generic;
using Localization.SKD.Common;
using StrazhAPI.Models;
using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class EmployeeReportProvider : FilteredSKDReportProvider<EmployeeReportFilter>
	{
		public EmployeeReportProvider()
			: base(CommonResources.EmployeeInfo, 418, SKDReportGroup.HR, PermissionType.Oper_Reports_Employee)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "Employee", CommonResources.Visitor },
					{ "Number", CommonResources.PersonalNumber },
					{ "Organisation", CommonResources.Organization },
					{ "Department", CommonResources.Department },
					{ "Position", CommonResources.PositionMaintainer },
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
