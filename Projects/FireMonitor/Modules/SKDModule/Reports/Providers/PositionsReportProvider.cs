using System.Collections.Generic;
using RubezhAPI.Models;
using RubezhAPI.SKD.ReportFilters;
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

		public override FilterModel InitializeFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "Position", "Должность" },
					{ "Description", "Примечание" },
				},
				Pages = new List<FilterContainerViewModel>()
				{
					new OrganizationPageViewModel(false),
					new PositionPageViewModel(),
				},
			};
		}
	}
}
