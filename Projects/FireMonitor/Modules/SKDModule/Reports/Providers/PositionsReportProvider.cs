using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class PositionsReportProvider : FilteredSKDReportProvider<PositionsReportFilter>
	{
		public PositionsReportProvider()
			: base("Список должностей организации", 416, SKDReportGroup.HR, PermissionType.Oper_Reports_Positions)
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
					{ "Position", "Должность" },
					{ "Description", "Примечание" },
				},
				Pages = new List<FilterContainerViewModel>
				{
					organisationPage,
					new PositionPageViewModel(),
				},
			};
		}
	}
}
