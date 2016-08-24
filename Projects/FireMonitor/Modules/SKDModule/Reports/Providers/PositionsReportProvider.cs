using System.Collections.Generic;
using Localization.SKD.Common;
using StrazhAPI.Models;
using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class PositionsReportProvider : FilteredSKDReportProvider<PositionsReportFilter>
	{
		public PositionsReportProvider()
			: base(CommonResources.OrganizationPositionList, 416, SKDReportGroup.HR, PermissionType.Oper_Reports_Positions)
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
					{ "Position", CommonResources.Position },
					{ "Description", CommonResources.Note },
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
