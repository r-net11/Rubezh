using System.Collections.Generic;
using RubezhAPI.Models;
using RubezhAPI.SKD.ReportFilters;
using Infrastructure.Common.Windows.SKDReports;
using GKModule.ViewModels;

namespace GKModule.Reports.Providers
{
	public class DoorsReportProvider : FilteredSKDReportProvider<DoorsReportFilter>
	{
		public DoorsReportProvider()
			: base("Список точек доступа", 431, SKDReportGroup.Configuration, PermissionType.Oper_Reports_Doors)
		{
		}

		public override FilterModel InitializeFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "Number", "№ точки доступа" },
					{ "Door", "Название" },
					{ "Organisation", "Организация" },
					{ "EnterReader", "Устройство вход" },
					{ "ExitReader", "Устройство  выход" },
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