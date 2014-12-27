using System.Collections.Generic;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;

namespace SKDModule.Reports.Providers
{
    public class ReportProvider422 : FilteredSKDReportProvider<SKDReportFilter>
	{
		public ReportProvider422()
            : base("FilteredTestReport", "422. Отчет по графикам работы", 422, SKDReportGroup.TimeTracking)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "c01", "Сотрудник" },
					{ "c02", "Организация" },
					{ "c03", "Отдел" },
					{ "c04", "Должность" },
					{ "c05", "График" },
				},
			};
		}
	}
}
