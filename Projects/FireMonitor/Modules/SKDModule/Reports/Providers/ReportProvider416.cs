using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using System.Collections.Generic;

namespace SKDModule.Reports.Providers
{
	public class ReportProvider416 : FilteredSKDReportProvider<TestReportFilter>
	{
		public ReportProvider416()
            : base("FilteredTestReport", "416. Отчет \"Список должностей организации\"", 416, SKDReportGroup.HR)
		{
		}

		public override FilterModel CreateFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "c01", "Должность" },
					{ "c02", "Примечание" },
				},
			};
		}
	}
}
