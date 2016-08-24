using System.Collections.Generic;
using Localization.SKD.Common;
using StrazhAPI.Models;
using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class DocumentsReportProvider : FilteredSKDReportProvider<DocumentsReportFilter>
	{
		public DocumentsReportProvider()
			: base(CommonResources.DocumentsReport, 423, SKDReportGroup.TimeTracking, PermissionType.Oper_Reports_Documents)
		{
		}

		public override FilterModel GetFilterModel()
		{
			var organisationPage = new OrganizationPageViewModel(false);
			organisationPage.CheckFirstOrganisation(Filter);

			return new FilterModel
			{
				Columns = new Dictionary<string, string>
				{
					{ "Employee", CommonResources.Employee },
					{ "StartDateTime", CommonResources.StartDate },
					{ "EndDateTime", CommonResources.EndDate },
					{ "DocumentType", CommonResources.Type },
					{ "DocumentName", CommonResources.Document },
					{ "DocumentShortName", CommonResources.LiteralCode },
					{ "DocumentCode", CommonResources.NumericCode },
				},
				Pages = new List<FilterContainerViewModel>
				{
					organisationPage,
					new DepartmentPageViewModel(),
					new EmployeePageViewModel(),
					new DocumentFilterPageViewModel(),
				},
			};
		}
	}
}
