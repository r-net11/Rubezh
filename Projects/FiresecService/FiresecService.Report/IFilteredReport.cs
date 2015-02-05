using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.SKD.ReportFilters;

namespace FiresecService.Report
{
	public interface IFilteredReport
	{
		void ApplyFilter(SKDReportFilter filter);
	}
}
