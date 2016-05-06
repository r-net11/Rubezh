using System;

namespace StrazhAPI.SKD.ReportFilters
{
	public interface IReportFilterPeriod
	{
		ReportPeriodType PeriodType { get; set; }

		DateTime DateTimeFrom { get; set; }

		DateTime DateTimeTo { get; set; }
	}
}