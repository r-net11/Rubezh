using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using System.Collections.Generic;

namespace SKDModule.Reports.Providers
{
    public class ReportProvider412 : FilteredSKDReportProvider<SKDReportFilter>
	{
		public ReportProvider412()
			: base("FilteredTestReport", "412. Отчет по доступу в зоны", 412, SKDReportGroup.HR)
		{
		}

		public override FilterModel CreateFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "c01", "Зона" },
					{ "c02", "Тип пропуска" },
					{ "c03", "Номер пропуска" },
					{ "c04", "Организация" },
					{ "c05", "Отдел" },
					{ "c06", "Должность" },
					{ "c07", "Сотрудник" },
					{ "c08", "Шаблон доступа" },
				},
			};
		}
	}
}
