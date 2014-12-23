using System.Collections.Generic;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;

namespace SKDModule.Reports.Providers
{
	public class ReportProvider432 : FilteredSKDReportProvider<TestReportFilter>
	{
		public ReportProvider432()
            : base("FilteredTestReport", "432. Список устройств", 432, SKDReportGroup.Configuration)
		{
		}

		public override FilterModel CreateFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "c01", "Название контроллера" },
					{ "c02", "IP - адрес" },
					{ "c03", "Считыватели (фактич.)" },
					{ "c04", "Считыватели (макс.)" },
					{ "c05", "Кнопки \"Выход\" (фактич.)" },
					{ "c06", "Кнопки \"Выход\" (макс.)" },
					{ "c07", "Замки (фактич.)" },
					{ "c08", "Замки (макс.)" },
					{ "c09", "Датчики двери (фактич.)" },
					{ "c10", "Датчики двери (макс.)" },
				},
			};
		}
	}
}
