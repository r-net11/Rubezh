using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using System.Collections.Generic;

namespace SKDModule.Reports.Providers
{
	public class ReportProvider431 : FilteredSKDReportProvider<TestReportFilter>
	{
		public ReportProvider431()
			: base("FilteredTestReport", "431. Список точек доступа", 431)
		{
		}

		public override FilterModel CreateFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "c01", "№ точки доступа" },
					{ "c02", "Название" },
					{ "c03", "Контроллер" },
					{ "c04", "IP - адрес" },
					{ "c05", "Организация" },
					{ "c06", "Считыватель 1" },
					{ "c07", "Считыватель 2" },
					{ "c08", "Зона 1" },
					{ "c09", "Зона 2" },
					{ "c10", "Примечание" },
				},
			};
		}
	}
}
