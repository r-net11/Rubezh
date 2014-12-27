using System.Collections.Generic;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;

namespace SKDModule.Reports.Providers
{
    public class ReportProvider421 : FilteredSKDReportProvider<SKDReportFilter>
	{
		public ReportProvider421()
            : base("FilteredTestReport", "421. Дисциплинарный отчет", 421, SKDReportGroup.TimeTracking)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel()
			{
				HasPeriod = true,
				Columns = new Dictionary<string, string> 
				{ 
					{ "c01", "Дата" },
					{ "c02", "Приход" },
					{ "c03", "Уход" },
					{ "c04", "Сотрудник" },
					{ "c05", "Организация" },
					{ "c06", "Отдел" },
					{ "c07", "Р/В" },
					{ "c08", "Опоздание" },
					{ "c09", "Уход раньше" },
					{ "c10", "Отсутствие" },
					{ "c11", "Переработка" },
				},
			};
		}
	}
}
