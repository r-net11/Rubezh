using RubezhAPI.Models;
using RubezhAPI.SKD.ReportFilters;
using Infrastructure.Common.Windows.SKDReports;

namespace GKModule.Reports.Providers
{
	class DevicesReportProvider : FilteredSKDReportProvider<SKDReportFilter>
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
			};
		}
	}
}