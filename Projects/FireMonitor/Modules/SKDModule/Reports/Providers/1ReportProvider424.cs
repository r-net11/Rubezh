using System.Collections.Generic;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;

namespace SKDModule.Reports.Providers
{
	public class ReportProvider424 : FilteredSKDReportProvider<TestReportFilter>
	{
		public ReportProvider424()
            : base("FilteredTestReport", "424. Справка по отработанному времени", 424, SKDReportGroup.TimeTracking)
		{
		}

		public override FilterModel CreateFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "c01", "Сотрудник" },
					{ "c02", "Отработанное время" },
					{ "c03", "Организация" },
					{ "c04", "Отдел" },
					{ "c05", "Должность" },
				},
			};
		}
	}
}
