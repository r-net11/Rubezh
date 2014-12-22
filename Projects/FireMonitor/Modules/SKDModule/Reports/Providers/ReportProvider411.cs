using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using System.Collections.Generic;

namespace SKDModule.Reports.Providers
{
	public class ReportProvider411 : FilteredSKDReportProvider<TestReportFilter>
	{
		public ReportProvider411()
			: base("FilteredTestReport", "411. Отчет по пропускам", 411)
		{
		}

		public override FilterModel CreateFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "c01", "Статус" },
					{ "c02", "Номер" },
					{ "c03", "Организация" },
					{ "c04", "Отдел" },
					{ "c05", "Должность" },
					{ "c06", "Сотрудник" },
					{ "c07", "Срок действия" },
				},
			};
		}
	}
}
