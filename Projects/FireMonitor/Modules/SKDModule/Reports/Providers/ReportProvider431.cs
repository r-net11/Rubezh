using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using System.Collections.Generic;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
    public class ReportProvider431 : FilteredSKDReportProvider<ReportFilter431>
	{
		public ReportProvider431()
			: base("Report431", "431. Список точек доступа", 431, SKDReportGroup.Configuration)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "Number", "№ точки доступа" },
					{ "Door", "Название" },
					{ "Controller", "Контроллер" },
					{ "IP", "IP - адрес" },
					{ "Organisation", "Организация" },
					{ "EnterReader", "Считыватель 1" },
					{ "ExitReader", "Считыватель 2" },
					{ "EnterZone", "Зона 1" },
					{ "ExitZone", "Зона 2" },
					{ "Comment", "Примечание" },
				},
                Pages = new List<FilterContainerViewModel>()
				{
                    new DoorPageViewModel(),
					new ZonePageViewModel(),
					new OrganizationPageViewModel(true),
				},
            };
		}
	}
}
