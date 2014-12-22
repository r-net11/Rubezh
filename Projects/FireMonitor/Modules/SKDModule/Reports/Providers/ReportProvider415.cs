using System.Collections.Generic;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;

namespace SKDModule.Reports.Providers
{
	public class ReportProvider415 : FilteredSKDReportProvider<TestReportFilter>
	{
		public ReportProvider415()
			: base("FilteredTestReport", "415. Отчет \"Список отделов организации\"", 415)
		{
		}

		public override FilterModel CreateFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "c01", "Отдел" },
					{ "c02", "Телефон" },
					{ "c03", "Руководитель" },
					{ "c04", "Подразделение" },
					{ "c05", "Примечание" },
				},
			};
		}
	}
}
