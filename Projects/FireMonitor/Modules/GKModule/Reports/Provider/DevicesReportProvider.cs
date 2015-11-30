using System.Collections.Generic;
using RubezhAPI.Models;
using RubezhAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using GKModule.ViewModels;

namespace GKModule.Reports.Providers
{
	class DevicesReportProvider : FilteredSKDReportProvider<DevicesReportFilter>
	{
		public DevicesReportProvider()
			: base("Список устройств", 433, SKDReportGroup.Configuration, PermissionType.Oper_Reports_Devices)
		{
		}
		public override FilterModel InitializeFilterModel()
		{
			return new FilterModel
			{
				AllowSort = false,
				Pages = new List<FilterContainerViewModel>
				{
					new DevicesPageViewModel()
				}
			};
		}
	}
}