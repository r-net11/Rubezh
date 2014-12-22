using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.SKD.ReportFilters
{
	public interface IReportFilterPeriod
	{
		ReportPeriodType PeriodType { get; set; }
		DateTime DateTimeFrom { get; set; }
		DateTime DateTimeTo { get; set; }
	}
}
