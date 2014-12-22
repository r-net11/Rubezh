using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using ReportsModule.ViewModels;

namespace ReportsModule
{
	public class TESTREPORT : FilteredSKDReportProvider<TestReportFilter>
	{
		public TESTREPORT()
			: base("FilteredTestReport", "FilteredTestReport", 0)
		{
		}
		public override FilterModel CreateFilterModel()
		{
			return new FilterModel()
			{
				HasPeriod = true,
				Columns = new Dictionary<string, string>()
				{
					{ "Column 10", "Column 10" },
					{ "Column 11", "Column 11" },
					{ "Column 12", "Column 12" },
					{ "Column 13", "Column 13" },
					{ "Column 14", "Column 14" },
					{ "Column 15", "Column 15" },
					{ "Column 16", "Column 16" },
					{ "Column 17", "Column 17" },
					{ "Column 18", "Column 18" },
					{ "Column 19", "Column 19" },
					{ "Column 20", "Column 20" },
					{ "Column 21", "Column 21" },
					{ "Column 22", "Column 22" },
					{ "Column 23", "Column 23" },
					{ "Column 24", "Column 24" },
					{ "Column 25", "Column 25" },
					{ "Column 26", "Column 26" },
					{ "Column 27", "Column 27" },
					{ "Column 28", "Column 28" },
					{ "Column 29", "Column 29" },
					{ "Column 30", "Column 30" },
					{ "Column 31", "Column 31" },
					{ "Column 32", "Column 32" },
				},
				Pages = new List<FilterContainerViewModel>()
				{
				}
			};
		}
	}
}
