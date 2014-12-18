using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.SKDReports;
using Infrastructure.Common.Windows;
using FiresecAPI.SKD.ReportFilters;

namespace ReportsModule.Test
{
	public class TestReportProvider : FilteredSKDReportProvider<TestReportFilter>
	{
		public TestReportProvider(bool isDataReport, int index = 1)
			: base(isDataReport ? "TestDataReport" : "TestReport", (isDataReport ? "TestDataReport_" : "TestReport_") + index, index, null, ChangeFilter)
		{
			Filter = new TestReportFilter();
			Filter.Timestamp = DateTime.Now;
		}
		private static bool ChangeFilter(TestReportFilter filter)
		{
			filter.Timestamp = DateTime.Now;
			MessageBoxService.Show(filter.Timestamp.ToString());
			return true;
		}
	}
}
