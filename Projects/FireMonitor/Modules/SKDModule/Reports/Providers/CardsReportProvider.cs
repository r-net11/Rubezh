using System.Collections.Generic;
using Localization.SKD.Common;
using StrazhAPI.Models;
using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class CardsReportProvider : FilteredSKDReportProvider<CardsReportFilter>
	{
		public CardsReportProvider()
			: base(CommonResources.PasscardDetails, 411, SKDReportGroup.HR, PermissionType.Oper_Reports_Cards)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "Type", CommonResources.Status },
					{ "Number", CommonResources.Number },
					{ "Organisation", CommonResources.Organization },
					{ "Department", CommonResources.Department },
					{ "Position", CommonResources.Position },
					{ "Employee", CommonResources.Employee },
					{ "Period", CommonResources.Validaty },
					{ "AllowedPassCount", CommonResources.PassNumber },
				},
				MainViewModel = new PassCardMainPageViewModel(),
				Pages = new List<FilterContainerViewModel>()
				{
					new PassCardTypePageViewModel(),
					new OrganizationPageViewModel(true),
					new DepartmentPageViewModel(),
					new PositionPageViewModel(),
					new EmployeePageViewModel(),
				},
			};
		}
	}
}