using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using System.Collections.Generic;

namespace SKDModule.Reports.Providers
{
	public class ReportProvider402 : FilteredSKDReportProvider<TestReportFilter>
	{
		public ReportProvider402()
            : base("FilteredTestReport", "402. Отчет о маршруте сотрудника/посетителя", 402, SKDReportGroup.Events)
		{
		}

		public override FilterModel CreateFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "c01", "Сотруднки (Посетитель)" },
					{ "c02", "Организация" },
					{ "c03", "Отдел" },
					{ "c04", "Должность (Сопровождающий)" },
				},
			};
		}
	}
}
