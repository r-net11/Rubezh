using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using System.Collections.Generic;

namespace SKDModule.Reports.Providers
{
	public class ReportProvider423 : FilteredSKDReportProvider<SKDReportFilter>
	{
		public ReportProvider423()
            : base("FilteredTestReport", "423. Отчет по оправдательным документам", 423, SKDReportGroup.TimeTracking)
		{
		}

		public override FilterModel CreateFilterModel()
		{
			return new FilterModel()
			{
				HasPeriod = true,
				Columns = new Dictionary<string, string> 
				{ 
					{ "c01", "Сотрудник" },
					{ "c02", "Дата начала" },
					{ "c03", "Дата окончания" },
					{ "c04", "Время начала" },
					{ "c05", "Время окончания" },
					{ "c06", "Тип" },
					{ "c07", "Документ" },
					{ "c08", "Буквенный код" },
					{ "c09", "Числовой код" },
				},
			};
		}
	}
}
