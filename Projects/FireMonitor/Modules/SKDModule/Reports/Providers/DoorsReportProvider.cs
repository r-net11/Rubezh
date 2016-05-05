using System.Collections.Generic;
using StrazhAPI.Models;
using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class DoorsReportProvider : FilteredSKDReportProvider<DoorsReportFilter>
	{
		public DoorsReportProvider()
			: base("Список точек доступа", 431, SKDReportGroup.Configuration, PermissionType.Oper_Reports_Doors)
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