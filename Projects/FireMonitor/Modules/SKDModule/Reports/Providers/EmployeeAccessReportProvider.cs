using System.Collections.Generic;
using Localization.SKD.Common;
using StrazhAPI.Models;
using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class EmployeeAccessReportProvider : FilteredSKDReportProvider<EmployeeAccessReportFilter>
	{
		public EmployeeAccessReportProvider()
            : base(CommonResources.ZoneAccess, 412, SKDReportGroup.HR, PermissionType.Oper_Reports_Employees_Access)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string>
				{
					{ "Zone", CommonResources.Zone },
					{ "Type", CommonResources.PasscardType },
					{ "Number", CommonResources.PasscardNumber },
					{ "Organisation", CommonResources.Organization },
					{ "Department", CommonResources.Department },
					{ "Position", CommonResources.Position },
					{ "Employee", CommonResources.Employee },
					{ "Template", CommonResources.AccessTemplate },
				},
				Pages = new List<FilterContainerViewModel>()
				{
					new PassCardTypePageViewModel() { IsActive = true },
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